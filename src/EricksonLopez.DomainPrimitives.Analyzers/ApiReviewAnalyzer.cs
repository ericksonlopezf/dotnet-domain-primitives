// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Threading.Tasks;

namespace EricksonLopez.DomainPrimitives.Analyzers;

/// <summary>
/// Enforces API surface quality rules on domain primitive types, including
/// API budget limits, missing XML documentation, and invalid factory method names.
/// </summary>
/// <remarks>
/// <para>Reports the following diagnostics:</para>
/// <list type="bullet">
///   <item><term>DP0014</term><description>API surface budget exceeded.</description></item>
///   <item><term>DP0015</term><description>Public member missing XML documentation.</description></item>
///   <item><term>DP0016</term><description>Custom factory method uses a non-standard name.</description></item>
/// </list>
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ApiReviewAnalyzer : DiagnosticAnalyzer
{
    internal static readonly ImmutableHashSet<string> PrimitiveAttributeNames = ImmutableHashSet.Create(
        "StringPrimitive", "StringPrimitiveAttribute",
        "NumericPrimitive", "NumericPrimitiveAttribute",
        "DatePrimitive", "DatePrimitiveAttribute",
        "StrongId", "StrongIdAttribute",
        "ValueObject", "ValueObjectAttribute",
        "SmartEnum", "SmartEnumAttribute"
    );

    internal static readonly ImmutableHashSet<string> ValidFactoryNames = ImmutableHashSet.Create(
        "Create", "TryCreate", "Parse", "TryParse"
    );

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
        DiagnosticDescriptors.DP0014_ApiSurfaceBudgetExceeded,
        DiagnosticDescriptors.DP0015_MissingXmlDocumentation,
        DiagnosticDescriptors.DP0016_InvalidFactoryMethodName
    );

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // TD-002: Use RegisterSymbolStartAction instead of RegisterSymbolAction.
        // RegisterSymbolAction is called once per ISymbol regardless of how many partial
        // declarations exist, BUT it lacks access to individual syntax for budget checks.
        // RegisterSymbolStartAction gives us a per-symbol scope with EndAction for aggregation.
        context.RegisterSymbolStartAction(symbolStart =>
        {
            var namedType = (INamedTypeSymbol)symbolStart.Symbol;

            var primitiveType = namedType.GetAttributes()
                .FirstOrDefault(a => a.AttributeClass != null && PrimitiveAttributeNames.Contains(a.AttributeClass.Name))
                ?.AttributeClass?.Name;

            if (primitiveType is null)
                return;

            // Register member-level actions within this symbol scope
            symbolStart.RegisterSymbolEndAction(symbolEnd =>
            {
                AnalyzeSymbol((INamedTypeSymbol)symbolEnd.Symbol, primitiveType, symbolEnd);
            });
        }, SymbolKind.NamedType);
    }

    private static void AnalyzeSymbol(INamedTypeSymbol namedTypeSymbol, string primitiveType, SymbolAnalysisContext context)
    {
        var publicMembers = namedTypeSymbol.GetMembers().Where(m =>
            m.DeclaredAccessibility == Accessibility.Public &&
            !m.IsImplicitlyDeclared &&
            m.Kind is SymbolKind.Method or SymbolKind.Property or SymbolKind.Field).ToList();

        int maxBudget = primitiveType switch
        {
            "NumericPrimitiveAttribute" => 27,
            "StrongIdAttribute" => 15,
            "DatePrimitiveAttribute" => 23,
            "ValueObjectAttribute" => 20 + publicMembers.Count(m => m.Kind is SymbolKind.Property or SymbolKind.Field),
            "SmartEnumAttribute" => 12 + publicMembers.Count(m => m.IsStatic && m.Kind is SymbolKind.Field or SymbolKind.Property),
            _ => 25
        };

        // Check DP0014: API Surface Budget Exceeded
        if (publicMembers.Count > maxBudget)
        {
            var diagnostic = Diagnostic.Create(DiagnosticDescriptors.DP0014_ApiSurfaceBudgetExceeded, namedTypeSymbol.Locations[0], namedTypeSymbol.Name, publicMembers.Count, maxBudget);
            context.ReportDiagnostic(diagnostic);
        }

        foreach (var member in publicMembers)
        {
            // Skip methods inherited from object/ValueType/etc that might be overridden unless they are newly introduced
            if (member is IMethodSymbol methodSymbol)
            {
                if (methodSymbol.MethodKind != MethodKind.Ordinary) continue;
                if (methodSymbol.IsOverride) continue; // Skip overridden methods like ToString, GetHashCode

                // Check DP0016: Invalid Factory Method Name
                if (methodSymbol.IsStatic && SymbolEqualityComparer.Default.Equals(methodSymbol.ReturnType, namedTypeSymbol))
                {
                    if (!ValidFactoryNames.Contains(methodSymbol.Name))
                    {
                        var diagnostic = Diagnostic.Create(DiagnosticDescriptors.DP0016_InvalidFactoryMethodName, methodSymbol.Locations[0], methodSymbol.Name, namedTypeSymbol.Name);
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }

            // Check DP0015: Missing XML Documentation
            // Only warn on members declared in source, not generated ones
            if (member.DeclaringSyntaxReferences.Any())
            {
                // If the member has no documentation comment, warn.
                string xmlDocs = member.GetDocumentationCommentXml(cancellationToken: context.CancellationToken);
                if (string.IsNullOrWhiteSpace(xmlDocs))
                {
                    var diagnostic = Diagnostic.Create(DiagnosticDescriptors.DP0015_MissingXmlDocumentation, member.Locations[0], member.Name, namedTypeSymbol.Name);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}


