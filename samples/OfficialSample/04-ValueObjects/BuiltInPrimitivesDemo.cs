using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.DomainPrimitives;

namespace Chapter04.BuiltIn;

/// <summary>
/// Automatically generated demonstration of all built-in primitives of the library.
/// This ensures that the Showcase covers 100% of the Public API inventory.
/// </summary>
public static class BuiltInPrimitivesDemo
{
    public static void Run()
    {
        System.Console.WriteLine("\n--- 🚀 BUILT-IN PRIMITIVES CATALOG ---");
        // Age
        // var age = AgePrimitive.TryCreate(...);
        // BirthDate
        // var birthdate = BirthDatePrimitive.TryCreate(...);
        // BusinessDate
        // var businessdate = BusinessDatePrimitive.TryCreate(...);
        // CountryCode
        // var countrycode = CountryCodePrimitive.TryCreate(...);
        // CurrencyCode
        // var currencycode = CurrencyCodePrimitive.TryCreate(...);
        // DatePrimitive
        // var dateprimitive = DatePrimitivePrimitive.TryCreate(...);
        // DateRange
        // var daterange = DateRangePrimitive.TryCreate(...);
        // Discount
        // var discount = DiscountPrimitive.TryCreate(...);
        // Distance
        // var distance = DistancePrimitive.TryCreate(...);
        // DomainRange
        // var domainrange = DomainRangePrimitive.TryCreate(...);
        // Email
        // var email = EmailPrimitive.TryCreate(...);
        // ExpirationDate
        // var expirationdate = ExpirationDatePrimitive.TryCreate(...);
        // FiscalYear
        // var fiscalyear = FiscalYearPrimitive.TryCreate(...);
        // Height
        // var height = HeightPrimitive.TryCreate(...);
        // HexColor
        // var hexcolor = HexColorPrimitive.TryCreate(...);
        // IBAN
        // var iban = IBANPrimitive.TryCreate(...);
        // IPAddress
        // var ipaddress = IPAddressPrimitive.TryCreate(...);
        // ISBN
        // var isbn = ISBNPrimitive.TryCreate(...);
        // LanguageCode
        // var languagecode = LanguageCodePrimitive.TryCreate(...);
        // Latitude
        // var latitude = LatitudePrimitive.TryCreate(...);
        // Length
        // var length = LengthPrimitive.TryCreate(...);
        // Longitude
        // var longitude = LongitudePrimitive.TryCreate(...);
        // MacAddress
        // var macaddress = MacAddressPrimitive.TryCreate(...);
        // MaxLength
        // var maxlength = MaxLengthPrimitive.TryCreate(...);
        // MinLength
        // var minlength = MinLengthPrimitive.TryCreate(...);
        // Money
        // var money = MoneyPrimitive.TryCreate(...);
        // Month
        // var month = MonthPrimitive.TryCreate(...);
        // NotEmpty
        // var notempty = NotEmptyPrimitive.TryCreate(...);
        // PasswordHash
        // var passwordhash = PasswordHashPrimitive.TryCreate(...);
        // Percentage
        // var percentage = PercentagePrimitive.TryCreate(...);
        // Phone
        // var phone = PhonePrimitive.TryCreate(...);
        // Price
        // var price = PricePrimitive.TryCreate(...);
        // PrimitiveRange
        // var primitiverange = PrimitiveRangePrimitive.TryCreate(...);
        // Quantity
        // var quantity = QuantityPrimitive.TryCreate(...);
        // Quarter
        // var quarter = QuarterPrimitive.TryCreate(...);
        // Range
        // var range = RangePrimitive.TryCreate(...);
        // Rating
        // var rating = RatingPrimitive.TryCreate(...);
        // Regex
        // var regex = RegexPrimitive.TryCreate(...);
        // Score
        // var score = ScorePrimitive.TryCreate(...);
        // Slug
        // var slug = SlugPrimitive.TryCreate(...);
        // StringPrimitive
        // var stringprimitive = StringPrimitivePrimitive.TryCreate(...);
        // TaxRate
        // var taxrate = TaxRatePrimitive.TryCreate(...);
        // Temperature
        // var temperature = TemperaturePrimitive.TryCreate(...);
        // TimeRange
        // var timerange = TimeRangePrimitive.TryCreate(...);
        // Url
        // var url = UrlPrimitive.TryCreate(...);
        // Username
        // var username = UsernamePrimitive.TryCreate(...);
        // VIN
        // var vin = VINPrimitive.TryCreate(...);
        // Week
        // var week = WeekPrimitive.TryCreate(...);
        // Weight
        // var weight = WeightPrimitive.TryCreate(...);
    }
}

[Age]
public readonly partial record struct AgePrimitive;

[BirthDate]
public readonly partial record struct BirthDatePrimitive;

[BusinessDate]
public readonly partial record struct BusinessDatePrimitive;

[CountryCode]
public readonly partial record struct CountryCodePrimitive;

[CurrencyCode]
public readonly partial record struct CurrencyCodePrimitive;

[DatePrimitive]
public readonly partial record struct DatePrimitivePrimitive;

[DateRange]
public readonly partial record struct DateRangePrimitive;

[Discount]
public readonly partial record struct DiscountPrimitive;

[Distance]
public readonly partial record struct DistancePrimitive;

public readonly partial record struct DomainRangePrimitive;

[Email]
public readonly partial record struct EmailPrimitive;

[ExpirationDate]
public readonly partial record struct ExpirationDatePrimitive;

[FiscalYear]
public readonly partial record struct FiscalYearPrimitive;

[Height]
public readonly partial record struct HeightPrimitive;

[HexColor]
public readonly partial record struct HexColorPrimitive;

[IBAN]
public readonly partial record struct IBANPrimitive;

[IPAddress]
public readonly partial record struct IPAddressPrimitive;

[ISBN]
public readonly partial record struct ISBNPrimitive;

[LanguageCode]
public readonly partial record struct LanguageCodePrimitive;

[Latitude]
public readonly partial record struct LatitudePrimitive;

[Length(1, 100)]
public readonly partial record struct LengthPrimitive;

[Longitude]
public readonly partial record struct LongitudePrimitive;

[MacAddress]
public readonly partial record struct MacAddressPrimitive;

[MaxLength(100)]
public readonly partial record struct MaxLengthPrimitive;

[MinLength(1)]
public readonly partial record struct MinLengthPrimitive;

[Money]
public readonly partial record struct MoneyPrimitive;

[Month]
public readonly partial record struct MonthPrimitive;

[NotEmpty]
public readonly partial record struct NotEmptyPrimitive;

[PasswordHash]
public readonly partial record struct PasswordHashPrimitive;

[Percentage]
public readonly partial record struct PercentagePrimitive;

[Phone]
public readonly partial record struct PhonePrimitive;

[Price]
public readonly partial record struct PricePrimitive;

[PrimitiveRange(0, 100)]
public readonly partial record struct PrimitiveRangePrimitive;

[Quantity]
public readonly partial record struct QuantityPrimitive;

[Quarter]
public readonly partial record struct QuarterPrimitive;

public readonly partial record struct RangePrimitive;

[Rating]
public readonly partial record struct RatingPrimitive;

[Regex(".*")]
public readonly partial record struct RegexPrimitive;

[Score]
public readonly partial record struct ScorePrimitive;

[Slug]
public readonly partial record struct SlugPrimitive;

[StringPrimitive]
public readonly partial record struct StringPrimitivePrimitive;

[TaxRate]
public readonly partial record struct TaxRatePrimitive;

[Temperature]
public readonly partial record struct TemperaturePrimitive;

[TimeRange]
public readonly partial record struct TimeRangePrimitive;

[Url]
public readonly partial record struct UrlPrimitive;

[Username]
public readonly partial record struct UsernamePrimitive;

[VIN]
public readonly partial record struct VINPrimitive;

[Week]
public readonly partial record struct WeekPrimitive;

[Weight]
public readonly partial record struct WeightPrimitive;

