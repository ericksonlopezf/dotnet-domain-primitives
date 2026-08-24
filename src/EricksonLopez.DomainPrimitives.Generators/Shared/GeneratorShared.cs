// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EricksonLopez.DomainPrimitives.Generators.Shared;

internal static class GeneratorShared
{
    // ─── Fully-qualified attribute names ─────────────────────────────────────
    // Used by ForAttributeWithMetadataName for precise, cache-aware attribute lookup.
    // NOTE: Generic attributes must use the metadata name WITHOUT type arguments: "AttrName`1".

    internal const string StringPrimitiveFqn  = "EricksonLopez.DomainPrimitives.StringPrimitiveAttribute";
    internal const string NumericPrimitiveFqn = "EricksonLopez.DomainPrimitives.NumericPrimitiveAttribute`1";
    internal const string StrongIdFqn         = "EricksonLopez.DomainPrimitives.StrongIdAttribute`1";
    internal const string DatePrimitiveFqn    = "EricksonLopez.DomainPrimitives.DatePrimitiveAttribute";
    internal const string ValueObjectFqn      = "EricksonLopez.DomainPrimitives.ValueObjectAttribute";
    internal const string SmartEnumFqn        = "EricksonLopez.DomainPrimitives.SmartEnumAttribute`1";
    internal const string DefaultsFqn         = "EricksonLopez.DomainPrimitives.DomainPrimitivesDefaultsAttribute";

    // String domain shortcut FQNs
    internal const string EmailFqn        = "EricksonLopez.DomainPrimitives.EmailAttribute";
    internal const string PhoneFqn        = "EricksonLopez.DomainPrimitives.PhoneAttribute";
    internal const string UrlFqn          = "EricksonLopez.DomainPrimitives.UrlAttribute";
    internal const string SlugFqn         = "EricksonLopez.DomainPrimitives.SlugAttribute";
    internal const string CountryCodeFqn  = "EricksonLopez.DomainPrimitives.CountryCodeAttribute";
    internal const string LanguageCodeFqn = "EricksonLopez.DomainPrimitives.LanguageCodeAttribute";
    internal const string CurrencyCodeFqn = "EricksonLopez.DomainPrimitives.CurrencyCodeAttribute";
    internal const string UsernameFqn     = "EricksonLopez.DomainPrimitives.UsernameAttribute";
    internal const string PasswordHashFqn = "EricksonLopez.DomainPrimitives.PasswordHashAttribute";
    internal const string HexColorFqn     = "EricksonLopez.DomainPrimitives.HexColorAttribute";
    internal const string IPAddressFqn    = "EricksonLopez.DomainPrimitives.IPAddressAttribute";
    internal const string MacAddressFqn   = "EricksonLopez.DomainPrimitives.MacAddressAttribute";
    internal const string IBANFqn         = "EricksonLopez.DomainPrimitives.IBANAttribute";
    internal const string ISBNFqn         = "EricksonLopez.DomainPrimitives.ISBNAttribute";
    internal const string VINFqn          = "EricksonLopez.DomainPrimitives.VINAttribute";

    // Numeric domain shortcut FQNs
    internal const string MoneyFqn       = "EricksonLopez.DomainPrimitives.MoneyAttribute";
    internal const string PercentageFqn  = "EricksonLopez.DomainPrimitives.PercentageAttribute";
    internal const string LatitudeFqn    = "EricksonLopez.DomainPrimitives.LatitudeAttribute";
    internal const string LongitudeFqn   = "EricksonLopez.DomainPrimitives.LongitudeAttribute";
    internal const string AgeFqn         = "EricksonLopez.DomainPrimitives.AgeAttribute";
    internal const string WeightFqn      = "EricksonLopez.DomainPrimitives.WeightAttribute";
    internal const string HeightFqn      = "EricksonLopez.DomainPrimitives.HeightAttribute";
    internal const string DistanceFqn    = "EricksonLopez.DomainPrimitives.DistanceAttribute";
    internal const string TemperatureFqn = "EricksonLopez.DomainPrimitives.TemperatureAttribute";
    internal const string ScoreFqn       = "EricksonLopez.DomainPrimitives.ScoreAttribute";
    internal const string QuantityFqn    = "EricksonLopez.DomainPrimitives.QuantityAttribute";
    internal const string PriceFqn       = "EricksonLopez.DomainPrimitives.PriceAttribute";
    internal const string TaxRateFqn     = "EricksonLopez.DomainPrimitives.TaxRateAttribute";
    internal const string DiscountFqn    = "EricksonLopez.DomainPrimitives.DiscountAttribute";
    internal const string RatingFqn      = "EricksonLopez.DomainPrimitives.RatingAttribute";

    // Date domain shortcut FQNs
    internal const string BirthDateFqn     = "EricksonLopez.DomainPrimitives.BirthDateAttribute";
    internal const string ExpirationDateFqn = "EricksonLopez.DomainPrimitives.ExpirationDateAttribute";
    internal const string BusinessDateFqn  = "EricksonLopez.DomainPrimitives.BusinessDateAttribute";
    internal const string FiscalYearFqn    = "EricksonLopez.DomainPrimitives.FiscalYearAttribute";
    internal const string MonthFqn         = "EricksonLopez.DomainPrimitives.MonthAttribute";
    internal const string QuarterFqn       = "EricksonLopez.DomainPrimitives.QuarterAttribute";
    internal const string WeekFqn          = "EricksonLopez.DomainPrimitives.WeekAttribute";
    internal const string DateRangeFqn     = "EricksonLopez.DomainPrimitives.DateRangeAttribute";
    internal const string TimeRangeFqn     = "EricksonLopez.DomainPrimitives.TimeRangeAttribute";

    // ─── Legacy predicate (kept for generators that scan multiple attributes) ─



    /// <summary>
    /// Lightweight syntax-only predicate: checks only that the node is a readonly record struct.
    /// Used as the <c>syntaxPredicate</c> in <c>ForAttributeWithMetadataName</c>-based pipelines
    /// where the attribute matching is delegated to the Roslyn incremental framework.
    /// This predicate is O(1) and allocation-free; Roslyn's incremental pipeline caches its result.
    /// </summary>
    public static bool IsReadonlyRecordStruct(SyntaxNode node, CancellationToken _)
    {
        if (node is not RecordDeclarationSyntax rds)
            return false;
        if (!rds.ClassOrStructKeyword.IsKind(SyntaxKind.StructKeyword))
            return false;
        return rds.Modifiers.Any(SyntaxKind.ReadOnlyKeyword);
    }
}




