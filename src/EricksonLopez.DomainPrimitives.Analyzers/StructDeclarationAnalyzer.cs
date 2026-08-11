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
public sealed class StructDeclarationAnalyzer : DiagnosticAnalyzer
{
    private static readonly string[] TriggerAttributes =
    {
        "StrongId", "StringPrimitive", "NumericPrimitive", "DatePrimitive", "ValueObject",
        "Email", "Phone", "Url", "Slug", "CountryCode", "LanguageCode", "CurrencyCode", "Username",
        "PasswordHash", "HexColor", "IPAddress", "MacAddress", "IBAN", "ISBN", "VIN",
        "Money", "Percentage", "Latitude", "Longitude", "Age", "Weight", "Height", "Distance", "Temperature", "Score",
        "Quantity", "Price", "TaxRate", "Discount", "Rating",
        "BirthDate", "ExpirationDate", "BusinessDate", "FiscalYear", "Month", "Quarter", "Week", "DateRange", "TimeRange"
    };

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(
            DiagnosticDescriptors.DP0001_MustBePartial,
            DiagnosticDescriptors.DP0002_MustBeReadonly,
            DiagnosticDescriptors.DP0003_MustBeRecordStruct);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        
        context.RegisterSyntaxNodeAction(AnalyzeStructDeclaration, SyntaxKind.StructDeclaration, SyntaxKind.RecordStructDeclaration);
    }

    private void AnalyzeStructDeclaration(SyntaxNodeAnalysisContext context)
    {
        var typeDecl = (TypeDeclarationSyntax)context.Node;
        
        bool hasDomainAttribute = false;
        foreach (var attrList in typeDecl.AttributeLists)
        {
            foreach (var attr in attrList.Attributes)
            {
                var rawName = attr.Name switch
                {
                    QualifiedNameSyntax q => q.Right is GenericNameSyntax g ? g.Identifier.Text : q.Right.Identifier.Text,
                    SimpleNameSyntax s => s.Identifier.Text,
                    _ => attr.Name.ToString()
                };
                var stripped = rawName.EndsWith("Attribute", System.StringComparison.Ordinal)
                    ? rawName.Substring(0, rawName.Length - 9)
                    : rawName;
                
                if (System.Array.IndexOf(TriggerAttributes, stripped) >= 0)
                {
                    hasDomainAttribute = true;
                    break;
                }
            }
            if (hasDomainAttribute) break;
        }

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
