using System;
using System.Collections.Generic;

namespace EricksonLopez.DomainPrimitives.Testing;

/// <summary>
/// Provides deterministic, pre-defined valid and invalid test values for domain primitive scenarios.
/// </summary>
/// <remarks>
/// <para>
/// Unlike random data generators (e.g., Bogus, AutoFixture), <see cref="DomainPrimitiveFakeFactory"/>
/// returns <strong>deterministic</strong> values to ensure reproducible tests.
/// </para>
/// <para>
/// All valid values are real-world representative samples. Invalid values are carefully chosen
/// to cover boundary conditions and common mistake patterns.
/// </para>
/// </remarks>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static class DomainPrimitiveFakeFactory
{
    // ─── String Primitives ────────────────────────────────────────────────────

    /// <summary>Gets a set of valid RFC 5322 email addresses for testing.</summary>
    public static IReadOnlyList<string> ValidEmails { get; } =
    [
        "user@example.com",
        "user.name+tag@example.co.uk",
        "firstname.lastname@subdomain.example.com",
        "x@example.com",
        "valid-email@domain.org"
    ];

    /// <summary>Gets the first valid email — convenient for single-value tests.</summary>
    public static string ValidEmail => ValidEmails[0];

    /// <summary>Gets a set of invalid email addresses for negative testing.</summary>
    public static IReadOnlyList<string> InvalidEmails { get; } =
    [
        "",
        "   ",
        "notanemail",
        "@missing-local.org",
        "missing-at-sign",
        "missing-domain@",
        "two@@at.com",
        "space in@email.com",
        "toolong" + new string('a', 320) + "@example.com"
    ];

    /// <summary>Gets a set of valid E.164 phone numbers for testing.</summary>
    public static IReadOnlyList<string> ValidPhones { get; } =
    [
        "+12125551234",
        "+442071234567",
        "+34911234567",
        "+525512345678"
    ];

    /// <summary>Gets the first valid phone — convenient for single-value tests.</summary>
    public static string ValidPhone => ValidPhones[0];

    /// <summary>Gets a set of invalid phone numbers for negative testing.</summary>
    public static IReadOnlyList<string> InvalidPhones { get; } =
    [
        "",
        "5551234",           // Missing country code
        "+1",                // Too short
        "+(12) 345-6789",    // With formatting characters
        "+9999999999999999"  // Too long
    ];

    /// <summary>Gets a set of valid URLs for testing.</summary>
    public static IReadOnlyList<string> ValidUrls { get; } =
    [
        "https://www.example.com",
        "https://example.com/path?query=1",
        "http://localhost:5000/api/v1",
        "https://sub.domain.example.org/page#anchor"
    ];

    /// <summary>Gets the first valid URL — convenient for single-value tests.</summary>
    public static string ValidUrl => ValidUrls[0];

    /// <summary>Gets a set of invalid URLs for negative testing.</summary>
    public static IReadOnlyList<string> InvalidUrls { get; } =
    [
        "",
        "not-a-url",
        "ftp://blocked-scheme.com",
        "javascript:alert('xss')",
        "/relative/path"
    ];

    /// <summary>Gets a set of valid URL slugs for testing.</summary>
    public static IReadOnlyList<string> ValidSlugs { get; } =
    [
        "my-article-title",
        "product-123",
        "a",
        "hello-world-2024"
    ];

    /// <summary>Gets the first valid slug — convenient for single-value tests.</summary>
    public static string ValidSlug => ValidSlugs[0];

    /// <summary>Gets a set of invalid URL slugs for negative testing.</summary>
    public static IReadOnlyList<string> InvalidSlugs { get; } =
    [
        "",
        "   ",
        "Has Spaces",
        "HAS_UPPERCASE",
        "special!chars@here",
        new string('a', 201) // Over max length
    ];

    /// <summary>Gets a set of valid ISO 3166-1 alpha-2 country codes for testing.</summary>
    public static IReadOnlyList<string> ValidCountryCodes { get; } =
    [
        "US", "GB", "DE", "ES", "FR", "JP", "CN", "BR"
    ];

    /// <summary>Gets the first valid country code — convenient for single-value tests.</summary>
    public static string ValidCountryCode => ValidCountryCodes[0];

    // ─── Numeric Primitives ───────────────────────────────────────────────────

    /// <summary>Gets a set of valid monetary amounts for testing.</summary>
    public static IReadOnlyList<decimal> ValidMoneyAmounts { get; } =
    [
        0m, 0.01m, 9.99m, 100m, 9999999.99m
    ];

    /// <summary>Gets the first valid money amount — convenient for single-value tests.</summary>
    public static decimal ValidMoneyAmount => ValidMoneyAmounts[0];

    /// <summary>Gets a set of invalid monetary amounts (negative) for negative testing.</summary>
    public static IReadOnlyList<decimal> InvalidMoneyAmounts { get; } =
    [
        -0.01m, -1m, decimal.MinValue
    ];

    /// <summary>Gets a set of valid age values for testing.</summary>
    public static IReadOnlyList<int> ValidAges { get; } = [0, 1, 18, 65, 100, 150];

    /// <summary>Gets the first valid age — convenient for single-value tests.</summary>
    public static int ValidAge => ValidAges[2]; // 18

    /// <summary>Gets a set of invalid age values for negative testing.</summary>
    public static IReadOnlyList<int> InvalidAges { get; } = [-1, 151, int.MaxValue];

    /// <summary>Gets a set of valid latitude values for testing.</summary>
    public static IReadOnlyList<double> ValidLatitudes { get; } = [-90.0, -45.5, 0.0, 45.5, 90.0];

    /// <summary>Gets a set of invalid latitude values for negative testing.</summary>
    public static IReadOnlyList<double> InvalidLatitudes { get; } = [-90.1, 90.1, double.MaxValue];

    /// <summary>Gets a set of valid longitude values for testing.</summary>
    public static IReadOnlyList<double> ValidLongitudes { get; } = [-180.0, -90.0, 0.0, 90.0, 180.0];

    /// <summary>Gets a set of invalid longitude values for negative testing.</summary>
    public static IReadOnlyList<double> InvalidLongitudes { get; } = [-180.1, 180.1, double.MinValue];

    /// <summary>Gets a set of valid percentages (0-100) for testing.</summary>
    public static IReadOnlyList<decimal> ValidPercentages { get; } = [0m, 25.5m, 50m, 100m];

    /// <summary>Gets a set of invalid percentages for negative testing.</summary>
    public static IReadOnlyList<decimal> InvalidPercentages { get; } = [-0.01m, 100.01m, 150m];

    /// <summary>Gets a set of valid weight values in kg for testing.</summary>
    public static IReadOnlyList<double> ValidWeights { get; } = [0.1, 70.5, 500.0, 1000.0];

    /// <summary>Gets a set of invalid weight values for negative testing.</summary>
    public static IReadOnlyList<double> InvalidWeights { get; } = [-1.0, 1000.1];

    /// <summary>Gets a set of valid height values in cm for testing.</summary>
    public static IReadOnlyList<double> ValidHeights { get; } = [1.0, 175.5, 290.0, 300.0];

    /// <summary>Gets a set of invalid height values for negative testing.</summary>
    public static IReadOnlyList<double> InvalidHeights { get; } = [-5.0, 300.1];

    /// <summary>Gets a set of valid distance values in meters for testing.</summary>
    public static IReadOnlyList<double> ValidDistances { get; } = [0.0, 1000.5, 40075000.0];

    /// <summary>Gets a set of invalid distance values for negative testing.</summary>
    public static IReadOnlyList<double> InvalidDistances { get; } = [-0.1, -100.0];

    /// <summary>Gets a set of valid temperature values in Celsius for testing.</summary>
    public static IReadOnlyList<double> ValidTemperatures { get; } = [-273.15, 0.0, 36.6, 100.0];

    /// <summary>Gets a set of invalid temperature values (below absolute zero) for negative testing.</summary>
    public static IReadOnlyList<double> InvalidTemperatures { get; } = [-273.16, -500.0];

    /// <summary>Gets a set of valid scores (0-100) for testing.</summary>
    public static IReadOnlyList<int> ValidScores { get; } = [0, 50, 100];

    /// <summary>Gets a set of invalid scores for negative testing.</summary>
    public static IReadOnlyList<int> InvalidScores { get; } = [-1, 101];

    /// <summary>Gets a set of valid non-negative quantities for testing.</summary>
    public static IReadOnlyList<int> ValidQuantities { get; } = [0, 1, 100, 1000];

    /// <summary>Gets a set of invalid quantities for negative testing.</summary>
    public static IReadOnlyList<int> InvalidQuantities { get; } = [-1, -100];

    /// <summary>Gets a set of valid prices for testing.</summary>
    public static IReadOnlyList<decimal> ValidPrices { get; } = [0m, 19.99m, 1500m];

    /// <summary>Gets a set of invalid prices for negative testing.</summary>
    public static IReadOnlyList<decimal> InvalidPrices { get; } = [-0.01m, -100m];

    /// <summary>Gets a set of valid tax rates for testing.</summary>
    public static IReadOnlyList<decimal> ValidTaxRates { get; } = [0m, 16m, 21m, 100m];

    /// <summary>Gets a set of invalid tax rates for negative testing.</summary>
    public static IReadOnlyList<decimal> InvalidTaxRates { get; } = [-0.1m, 100.1m];

    /// <summary>Gets a set of valid discount percentages for testing.</summary>
    public static IReadOnlyList<decimal> ValidDiscounts { get; } = [0m, 10m, 50m, 100m];

    /// <summary>Gets a set of invalid discount percentages for negative testing.</summary>
    public static IReadOnlyList<decimal> InvalidDiscounts { get; } = [-1m, 101m];

    // ─── IDs ─────────────────────────────────────────────────────────────────

    /// <summary>Gets a set of known non-empty GUIDs for testing.</summary>
    public static IReadOnlyList<Guid> ValidGuids { get; } =
    [
        new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
        new Guid("00000000-0000-0000-0000-000000000001"),
        new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff")
    ];

    /// <summary>Gets the first valid GUID — convenient for single-value tests.</summary>
    public static Guid ValidGuid => ValidGuids[0];

    /// <summary>Gets a set of GUID strings for testing string-based parsing.</summary>
    public static IReadOnlyList<string> ValidGuidStrings { get; } =
    [
        "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "3FA85F64-5717-4562-B3FC-2C963F66AFA6",  // Upper case
        "{3fa85f64-5717-4562-b3fc-2c963f66afa6}", // With braces
        "3fa85f6457174562b3fc2c963f66afa6"         // Without hyphens
    ];

    /// <summary>Gets a set of invalid GUID strings for negative testing.</summary>
    public static IReadOnlyList<string> InvalidGuidStrings { get; } =
    [
        "",
        "not-a-guid",
        "3fa85f64-5717-4562-b3fc",  // Too short
        "3fa85f64-5717-4562-b3fc-2c963f66afa6-extra" // Too long
    ];

    // ─── Date Primitives ─────────────────────────────────────────────────────

    /// <summary>Gets current date in UTC for date-based testing.</summary>
    /// <remarks>
    /// <b>Timezone note:</b> Always uses <c>DateTime.UtcNow</c> (UTC), not the local system clock.
    /// This ensures consistent behavior across CI runners in different timezones.
    /// If your domain model uses local time, be aware that date-boundary tests (e.g., "today is valid",
    /// "yesterday is invalid") may behave differently if evaluated near midnight UTC.
    /// </remarks>
    public static DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);

    /// <summary>Gets a valid birth date for deterministic testing (30 years in the past, UTC).</summary>
    /// <remarks>
    /// This is a live-computed property (not static readonly). Each call returns 30 years before
    /// the current UTC date. For snapshot-based tests, capture the value once and reuse it.
    /// </remarks>
    public static DateOnly ValidBirthDate => Today.AddYears(-30);

    /// <summary>Gets a past date (5 years ago) for past date testing.</summary>
    public static DateOnly PastDate => Today.AddYears(-5);

    /// <summary>Gets a future date (5 years ahead) for expiration date testing.</summary>
    public static DateOnly FutureDate => Today.AddYears(5);

    /// <summary>Gets a set of valid future expiration dates for testing.</summary>
    public static List<DateOnly> ValidExpirationDates { get; } =
    [
        Today.AddDays(1), Today.AddMonths(6), Today.AddYears(2)
    ];

    /// <summary>Gets a set of invalid expiration dates (in the past) for negative testing.</summary>
    public static List<DateOnly> InvalidExpirationDates { get; } =
    [
        Today.AddDays(-1), Today.AddYears(-1)
    ];

    /// <summary>Gets valid business dates (Monday-Friday) for testing, computed dynamically from today.</summary>
    /// <remarks>Dates are computed at class initialization time to avoid clock drift within a single test run.</remarks>
    public static List<DateOnly> ValidBusinessDates { get; } = GetNextWeekdays(2);

    /// <summary>Gets invalid business dates (weekends) for testing.</summary>
    public static List<DateOnly> InvalidBusinessDates { get; } = GetNextWeekendDays(2);

    /// <summary>Finds the next <paramref name="count"/> weekday dates starting from tomorrow.</summary>
    private static List<DateOnly> GetNextWeekdays(int count)
    {
        var result = new List<DateOnly>(count);
        var candidate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1);
        while (result.Count < count)
        {
            if (candidate.DayOfWeek != DayOfWeek.Saturday && candidate.DayOfWeek != DayOfWeek.Sunday)
                result.Add(candidate);
            candidate = candidate.AddDays(1);
        }
        return result;
    }

    /// <summary>Finds the next <paramref name="count"/> weekend dates starting from tomorrow.</summary>
    private static List<DateOnly> GetNextWeekendDays(int count)
    {
        var result = new List<DateOnly>(count);
        var candidate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1);
        while (result.Count < count)
        {
            if (candidate.DayOfWeek == DayOfWeek.Saturday || candidate.DayOfWeek == DayOfWeek.Sunday)
                result.Add(candidate);
            candidate = candidate.AddDays(1);
        }
        return result;
    }

    /// <summary>Gets valid fiscal years for testing.</summary>
    public static IReadOnlyList<int> ValidFiscalYears { get; } = [1900, 2024, 2026];

    /// <summary>Gets invalid fiscal years for testing.</summary>
    public static IReadOnlyList<int> InvalidFiscalYears { get; } = [1899, 0, -1];

    // ─── Domain Shortcut Fake Data ───────────────────────────────────────────

    /// <summary>Gets valid ISO currency codes for testing.</summary>
    public static IReadOnlyList<string> ValidCurrencyCodes { get; } = ["USD", "EUR", "GBP", "JPY", "CAD"];

    /// <summary>Gets valid IBAN strings for testing.</summary>
    public static IReadOnlyList<string> ValidIBANs { get; } = ["DE89370400440532013000", "GB29NWBK60161331926819"];

    /// <summary>Gets valid ISBN strings for testing.</summary>
    public static IReadOnlyList<string> ValidISBNs { get; } = ["978-3-16-148410-0", "978-0-306-40615-7"];

    /// <summary>Gets valid VIN strings for testing.</summary>
    public static IReadOnlyList<string> ValidVINs { get; } = ["1HGCR2F83HA000000", "1FA6P8CF0H5100000"];

    /// <summary>Gets valid Hex Colors for testing.</summary>
    public static IReadOnlyList<string> ValidHexColors { get; } = ["#FF5733", "#00FF00", "#000000", "#FFFFFF"];

    /// <summary>Gets valid ratings (0-5 scale) for testing.</summary>
    public static IReadOnlyList<decimal> ValidRatings { get; } = [0.0m, 2.5m, 4.0m, 4.8m, 5.0m];
}
