using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace EricksonLopez.DomainPrimitives.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ApiReviewAnalyzer : DiagnosticAnalyzer
{
    private static readonly ImmutableHashSet<string> PrimitiveAttributeNames = ImmutableHashSet.Create(
        "StringPrimitive", "StringPrimitiveAttribute",
        "NumericPrimitive", "NumericPrimitiveAttribute",
        "DatePrimitive", "DatePrimitiveAttribute",
        "StrongId", "StrongIdAttribute",
        "ValueObject", "ValueObjectAttribute",
        "SmartEnum", "SmartEnumAttribute"
    );

    private static readonly ImmutableHashSet<string> ValidFactoryNames = ImmutableHashSet.Create(
        "Create", "TryCreate", "Parse", "TryParse"
    );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
        DiagnosticDescriptors.DP0014_ApiSurfaceBudgetExceeded,
        DiagnosticDescriptors.DP0015_MissingXmlDocumentation,
        DiagnosticDescriptors.DP0016_InvalidFactoryMethodName
    );

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
            if (symbolStart.Symbol is not INamedTypeSymbol namedType)
                return;

            // Check if it's a domain primitive
            string primitiveType = string.Empty;
            foreach (var attribute in namedType.GetAttributes())
            {
                if (attribute.AttributeClass != null && PrimitiveAttributeNames.Contains(attribute.AttributeClass.Name))
                {
                    primitiveType = attribute.AttributeClass.Name;
                    break;
                }
            }

            if (string.IsNullOrEmpty(primitiveType))
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

        int maxBudget = 25; // Default fallback
        if (primitiveType.Contains("String")) maxBudget = 25;
        else if (primitiveType.Contains("Numeric")) maxBudget = 27;
        else if (primitiveType.Contains("StrongId")) maxBudget = 15;
        else if (primitiveType.Contains("Date")) maxBudget = 23;
        else if (primitiveType.Contains("ValueObject"))
        {
            var fields = publicMembers.Count(m => m.Kind == SymbolKind.Property || m.Kind == SymbolKind.Field);
            maxBudget = 20 + fields;
        }
        else if (primitiveType.Contains("SmartEnum"))
        {
            var enumMembers = publicMembers.Count(m => m.IsStatic && (m.Kind == SymbolKind.Field || m.Kind == SymbolKind.Property));
            maxBudget = 12 + enumMembers;
        }

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
            var syntaxRef = member.DeclaringSyntaxReferences.FirstOrDefault();
            if (syntaxRef != null)
            {
                var syntaxNode = syntaxRef.GetSyntax(context.CancellationToken);
                
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
