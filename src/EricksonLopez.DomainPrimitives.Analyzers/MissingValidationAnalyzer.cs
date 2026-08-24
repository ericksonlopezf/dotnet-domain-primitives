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
/// Detects domain primitive types that declare no validation rules, ensuring that
/// every primitive enforces at least one invariant to justify its existence.
/// </summary>
/// <remarks>
/// Reports <c>DP0009</c> when a <c>[StringPrimitive]</c>, <c>[NumericPrimitive]</c>, or
/// <c>[DatePrimitive]</c> type carries no validation or domain-shortcut attribute.
/// Also reports <c>DP0009</c> for <c>[StrongId&lt;string&gt;]</c> types that carry no
/// length or format constraint, as an unconstrained string id is semantically
/// equivalent to a plain <see langword="string"/>.
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MissingValidationAnalyzer : DiagnosticAnalyzer
{
    internal static readonly string[] ValidationAttributeNames =
    [
        "MinLengthAttribute", "MaxLengthAttribute", "LengthAttribute", 
        "RegexAttribute", "RangeAttribute", "PrimitiveRangeAttribute", "NotEmptyAttribute", "CustomValidatorAttribute"
    ];

    internal static readonly string[] DomainShortcutAttributeNames =
    [
        "EmailAttribute", "PhoneAttribute", "UrlAttribute", "SlugAttribute",
        "CountryCodeAttribute", "LanguageCodeAttribute", "CurrencyCodeAttribute",
        "UsernameAttribute", "PasswordHashAttribute", "HexColorAttribute",
        "IPAddressAttribute", "MacAddressAttribute", "IBANAttribute", "ISBNAttribute", "VINAttribute",
        "LatitudeAttribute", "LongitudeAttribute", "AgeAttribute", "WeightAttribute", "HeightAttribute",
        "DistanceAttribute", "TemperatureAttribute", "ScoreAttribute", "QuantityAttribute",
        "PriceAttribute", "TaxRateAttribute", "DiscountAttribute", "RatingAttribute",
        "PercentageAttribute", "MoneyAttribute", "BirthDateAttribute", "ExpirationDateAttribute",
        "BusinessDateAttribute", "FiscalYearAttribute", "MonthAttribute", "QuarterAttribute",
        "WeekAttribute", "DateRangeAttribute", "TimeRangeAttribute"
    ];

    /// <summary>
    /// Constraint attributes that are valid for string-backed StrongId types.
    /// </summary>
    internal static readonly string[] StringIdConstraintAttributeNames =
    [
        "MinLengthAttribute", "MaxLengthAttribute", "LengthAttribute", "RegexAttribute"
    ];

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => 
        ImmutableArray.Create(DiagnosticDescriptors.DP0009_MissingValidation);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeStructDeclaration, SyntaxKind.StructDeclaration, SyntaxKind.RecordStructDeclaration);
    }

    private void AnalyzeStructDeclaration(SyntaxNodeAnalysisContext context)
    {
        var typeDeclaration = (TypeDeclarationSyntax)context.Node;
        var symbol = (INamedTypeSymbol)context.SemanticModel.GetDeclaredSymbol(typeDeclaration, context.CancellationToken)!;

        bool isPrimitive = false;
        bool hasValidation = false;
        bool hasShortcut = false;
        bool isStringBackedStrongId = false;
        bool hasStringIdConstraint = false;

        foreach (var attr in symbol.GetAttributes())
        {
            var attrClass = attr.AttributeClass!;
            var ns = attrClass.ContainingNamespace?.ToDisplayString();
            if (ns != "EricksonLopez.DomainPrimitives" && ns != "EricksonLopez.DomainPrimitives.Validation")
                continue;

            var name = attrClass.Name;

            if (name == "StringPrimitiveAttribute" || name == "NumericPrimitiveAttribute" || name == "DatePrimitiveAttribute")
            {
                isPrimitive = true;
                if (attr.NamedArguments.Any(arg => (arg.Key == "PastOnly" || arg.Key == "FutureOnly") && arg.Value.Value is true))
                {
                    hasValidation = true;
                }
            }

            // MED-001: Detect StrongId<string> — emits DP0009 if no string constraints present.
            // StrongId<string> is valid but must have at least one constraint to be meaningful.
            if (name == "StrongIdAttribute" && attrClass.TypeArguments.Length == 1)
            {
                var backingType = attrClass.TypeArguments[0];
                if (backingType.SpecialType == SpecialType.System_String)
                {
                    isStringBackedStrongId = true;
                }
            }
            
            if (ValidationAttributeNames.Contains(name))
            {
                hasValidation = true;
            }

            if (StringIdConstraintAttributeNames.Contains(name))
            {
                hasStringIdConstraint = true;
            }
            
            if (DomainShortcutAttributeNames.Contains(name))
            {
                hasShortcut = true;
            }
        }

        if (isPrimitive && !hasValidation && !hasShortcut)
        {
            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptors.DP0009_MissingValidation,
                typeDeclaration.Identifier.GetLocation(),
                symbol.Name);
            
            context.ReportDiagnostic(diagnostic);
        }

        // MED-001: StrongId<string> without any length/format constraints → warn.
        // A StrongId<string> with no constraints is semantically equivalent to a plain string,
        // defeating the purpose of domain primitives. Add [MinLength], [MaxLength], [Length], or [Regex].
        if (isStringBackedStrongId && !hasStringIdConstraint)
        {
            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptors.DP0009_MissingValidation,
                typeDeclaration.Identifier.GetLocation(),
                $"{symbol.Name} (StrongId<string> without length or format constraints — add [MinLength], [MaxLength], or [Regex])");

            context.ReportDiagnostic(diagnostic);
        }
    }
}


