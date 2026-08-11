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

/// <summary>
/// Detects when a domain primitive type declares a public constructor that can bypass
/// the source-generated validation pipeline.
/// </summary>
/// <remarks>
/// <para>
/// Reports <c>DP0012</c> when a <c>readonly partial record struct</c> decorated with a
/// domain primitive attribute also declares a <c>public</c> constructor.
/// </para>
/// <para>
/// The source generator intentionally generates a <c>private</c> constructor to force
/// consumers to use <c>Create()</c> or <c>TryCreate()</c>. A public constructor
/// bypasses this invariant and allows the creation of unvalidated instances.
/// </para>
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PublicConstructorBypassAnalyzer : DiagnosticAnalyzer
{
    private static readonly string[] DomainPrimitiveAttributeNames =
    [
        "StrongIdAttribute", "StringPrimitiveAttribute", "NumericPrimitiveAttribute",
        "DatePrimitiveAttribute", "SmartEnumAttribute",
        "EmailAttribute", "PhoneAttribute", "UrlAttribute", "SlugAttribute",
        "CountryCodeAttribute", "LanguageCodeAttribute", "CurrencyCodeAttribute",
        "UsernameAttribute", "PasswordHashAttribute", "HexColorAttribute",
        "IPAddressAttribute", "MacAddressAttribute", "IBANAttribute", "ISBNAttribute", "VINAttribute",
        "MoneyAttribute", "PercentageAttribute", "LatitudeAttribute", "LongitudeAttribute",
        "AgeAttribute", "WeightAttribute", "HeightAttribute", "DistanceAttribute",
        "TemperatureAttribute", "ScoreAttribute", "QuantityAttribute", "PriceAttribute",
        "TaxRateAttribute", "DiscountAttribute", "RatingAttribute",
        "BirthDateAttribute", "ExpirationDateAttribute", "BusinessDateAttribute",
        "FiscalYearAttribute", "MonthAttribute", "QuarterAttribute", "WeekAttribute",
        "DateRangeAttribute", "TimeRangeAttribute"
    ];

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(DiagnosticDescriptors.DP0012_PublicConstructorBypass);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
    }

    private static void AnalyzeNamedType(SymbolAnalysisContext context)
    {
        var typeSymbol = (INamedTypeSymbol)context.Symbol;

        // Must be a value type (struct) with partial declaration
        if (!typeSymbol.IsValueType) return;
        if (!typeSymbol.IsRecord) return; // We only care about record structs

        // Check if decorated with a domain primitive attribute
        var hasDomainPrimitiveAttribute = typeSymbol.GetAttributes().Any(attr =>
            attr.AttributeClass != null &&
            DomainPrimitiveAttributeNames.Contains(attr.AttributeClass.Name));

        if (!hasDomainPrimitiveAttribute) return;

        // Check for public constructors declared in source (not generated)
        foreach (var constructor in typeSymbol.Constructors)
        {
            // Skip the compiler-generated copy constructor and parameterless default
            if (constructor.IsImplicitlyDeclared) continue;
            if (constructor.Parameters.Length == 0) continue; // default ctor is always implicit for struct

            // Skip constructors marked as generated
            if (IsInGeneratedCode(constructor)) continue;

            if (constructor.DeclaredAccessibility == Accessibility.Public)
            {
                // Find the location of the constructor declaration in source
                var location = constructor.Locations.FirstOrDefault(l => l.IsInSource);
                if (location == null) continue;

                context.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.DP0012_PublicConstructorBypass,
                    location,
                    typeSymbol.Name));
            }
        }
    }

    private static bool IsInGeneratedCode(ISymbol symbol)
    {
        foreach (var location in symbol.Locations)
        {
            if (!location.IsInSource) continue;

            var syntaxTree = location.SourceTree;
            if (syntaxTree == null) continue;

            // Generated files typically contain ".g." in their path
            var filePath = syntaxTree.FilePath;
            if (filePath.Contains(".g.cs") || filePath.Contains(".generated.cs"))
                return true;
        }
        return false;
    }
}
