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
/// Enforces structural declaration requirements on domain primitive types, ensuring that
/// every decorated type is declared as a <c>readonly partial record struct</c>.
/// </summary>
/// <remarks>
/// <para>Reports the following diagnostics:</para>
/// <list type="bullet">
///   <item><term>DP0001</term><description>Type is not <c>partial</c>.</description></item>
///   <item><term>DP0002</term><description>Type is not <c>readonly</c>.</description></item>
///   <item><term>DP0003</term><description>Type is not a <c>record struct</c>.</description></item>
/// </list>
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class StructDeclarationAnalyzer : DiagnosticAnalyzer
{
    internal static readonly string[] TriggerAttributes =
    {
        "StrongId", "StringPrimitive", "NumericPrimitive", "DatePrimitive", "ValueObject",
        "Email", "Phone", "Url", "Slug", "CountryCode", "LanguageCode", "CurrencyCode", "Username",
        "PasswordHash", "HexColor", "IPAddress", "MacAddress", "IBAN", "ISBN", "VIN",
        "Money", "Percentage", "Latitude", "Longitude", "Age", "Weight", "Height", "Distance", "Temperature", "Score",
        "Quantity", "Price", "TaxRate", "Discount", "Rating",
        "BirthDate", "ExpirationDate", "BusinessDate", "FiscalYear", "Month", "Quarter", "Week", "DateRange", "TimeRange"
    };

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(
            DiagnosticDescriptors.DP0001_MustBePartial,
            DiagnosticDescriptors.DP0002_MustBeReadonly,
            DiagnosticDescriptors.DP0003_MustBeRecordStruct);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        
        context.RegisterSyntaxNodeAction(AnalyzeStructDeclaration, SyntaxKind.StructDeclaration, SyntaxKind.RecordStructDeclaration);
    }

    private void AnalyzeStructDeclaration(SyntaxNodeAnalysisContext context)
    {
        var typeDecl = (TypeDeclarationSyntax)context.Node;
        
        bool hasDomainAttribute = typeDecl.AttributeLists
            .SelectMany(l => l.Attributes)
            .Any(attr =>
            {
                var rawName = attr.Name switch
                {
                    QualifiedNameSyntax q => q.Right.Identifier.Text,
                    AliasQualifiedNameSyntax a => a.Name.Identifier.Text,
                    _ => ((SimpleNameSyntax)attr.Name).Identifier.Text
                };
                var stripped = rawName.EndsWith("Attribute", StringComparison.Ordinal)
                    ? rawName.Substring(0, rawName.Length - 9)
                    : rawName;
                return System.Array.IndexOf(TriggerAttributes, stripped) >= 0;
            });

        if (!hasDomainAttribute) return;

        var symbol = context.SemanticModel.GetDeclaredSymbol(typeDecl, context.CancellationToken) as INamedTypeSymbol;
        if (symbol is null) return;

        // DP0003: Must be record struct
        if (!typeDecl.IsKind(SyntaxKind.RecordStructDeclaration))
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.DP0003_MustBeRecordStruct, 
                typeDecl.Identifier.GetLocation(), 
                symbol.Name));
            return; // If it's not a record struct, no need to check further modifiers since the fix is completely structural
        }

        // DP0001: Must be partial
        bool isPartial = typeDecl.Modifiers.Any(SyntaxKind.PartialKeyword);
        if (!isPartial)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.DP0001_MustBePartial, 
                typeDecl.Identifier.GetLocation(), 
                symbol.Name));
        }

        // DP0002: Must be readonly
        bool isReadonly = typeDecl.Modifiers.Any(SyntaxKind.ReadOnlyKeyword);
        if (!isReadonly)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.DP0002_MustBeReadonly, 
                typeDecl.Identifier.GetLocation(), 
                symbol.Name));
        }
    }
}


