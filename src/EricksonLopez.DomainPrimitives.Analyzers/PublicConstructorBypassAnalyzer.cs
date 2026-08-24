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
    internal static readonly string[] DomainPrimitiveAttributeNames =
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

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(DiagnosticDescriptors.DP0012_PublicConstructorBypass);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
    }

    private static void AnalyzeNamedType(SymbolAnalysisContext context)
    {
        var typeSymbol = (INamedTypeSymbol)context.Symbol;

        // Must be a value type (struct) with record declaration
        if (!typeSymbol.IsValueType || !typeSymbol.IsRecord) return;

        // Check if decorated with a domain primitive attribute
        var hasDomainPrimitiveAttribute = typeSymbol.GetAttributes().Any(attr =>
            attr.AttributeClass != null &&
            DomainPrimitiveAttributeNames.Contains(attr.AttributeClass.Name));

        if (!hasDomainPrimitiveAttribute) return;

        // Check for public constructors declared in source (not generated)
        foreach (var constructor in typeSymbol.Constructors)
        {
            if (constructor.DeclaredAccessibility != Accessibility.Public || constructor.Parameters.Length == 0)
                continue;

            foreach (var location in constructor.Locations)
            {
                if (location.IsInSource && !IsInGeneratedCode(location))
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        DiagnosticDescriptors.DP0012_PublicConstructorBypass,
                        location,
                        typeSymbol.Name));
                }
            }
        }
    }

    /// <summary>
    /// Returns <see langword="true"/> when <paramref name="location"/> resides in a
    /// source-generated file, identified by the <c>.g.cs</c> or <c>.generated.cs</c> suffix.
    /// </summary>
    /// <param name="location">The source location to inspect.</param>
    internal static bool IsInGeneratedCode(Location location)
    {
        var filePath = location.SourceTree?.FilePath;
        return filePath != null && (filePath.EndsWith(".g.cs", StringComparison.OrdinalIgnoreCase) || filePath.EndsWith(".generated.cs", StringComparison.OrdinalIgnoreCase));
    }
}


