using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace EricksonLopez.DomainPrimitives.Testing;

/// <summary>
/// Provides pre-defined test scenarios for each category of domain primitive, covering
/// valid inputs, invalid inputs, boundary conditions, and normalization scenarios.
/// </summary>
/// <remarks>
/// <para>
/// Use these scenarios in parameterized tests (e.g., xUnit <c>[MemberData]</c> or
/// <c>[InlineData]</c>) to ensure comprehensive coverage without duplicating test data.
/// </para>
/// </remarks>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static class DomainPrimitiveScenarios
{
    // ─── Email Scenarios ──────────────────────────────────────────────────────

    /// <summary>
    /// Gets valid email inputs that should pass <c>[Email]</c> validation.
    /// </summary>
    public static IEnumerable<object[]> ValidEmailInputs => new List<object[]>
    {
        new object[] { "user@example.com" },
        new object[] { "USER@EXAMPLE.COM" },      // Should be normalized to lowercase
        new object[] { " user@example.com " },    // Should be trimmed
        new object[] { "user.name+tag@example.co.uk" },
        new object[] { "x@y.io" },
    };

    /// <summary>
    /// Gets invalid email inputs that should fail <c>[Email]</c> validation.
    /// </summary>
    public static IEnumerable<object[]> InvalidEmailInputs => new List<object[]>
    {
        new object[] { "" },
        new object[] { "   " },
        new object[] { "notanemail" },
        new object[] { "@missing-local.org" },
        new object[] { "missing@" },
        new object[] { "two@@at.com" },
        new object[] { "has space@domain.com" },
    };

    // ─── Phone Scenarios ─────────────────────────────────────────────────────

    /// <summary>
    /// Gets valid E.164 phone number inputs.
    /// </summary>
    public static IEnumerable<object[]> ValidPhoneInputs => new List<object[]>
    {
        new object[] { "+12125551234" },
        new object[] { "+442071234567" },
        new object[] { "+34911234567" },
    };

    /// <summary>
    /// Gets invalid phone number inputs.
    /// </summary>
    public static IEnumerable<object[]> InvalidPhoneInputs => new List<object[]>
    {
        new object[] { "" },
        new object[] { "5551234" },
        new object[] { "not-a-phone" },
        new object[] { "+1 (555) 123-4567" },  // With formatting
    };

    // ─── Slug Scenarios ───────────────────────────────────────────────────────

    /// <summary>
    /// Gets valid URL slug inputs.
    /// </summary>
    public static IEnumerable<object[]> ValidSlugInputs => new List<object[]>
    {
        new object[] { "my-article" },
        new object[] { "product-123" },
        new object[] { "a" },
        new object[] { "hello-world" },
    };

    /// <summary>
    /// Gets invalid URL slug inputs.
    /// </summary>
    public static IEnumerable<object[]> InvalidSlugInputs => new List<object[]>
    {
        new object[] { "" },
        new object[] { "Has Spaces" },
        new object[] { "UPPERCASE" },
        new object[] { "special!@#" },
    };

    // ─── StrongId<Guid> Scenarios ─────────────────────────────────────────────

    /// <summary>
    /// Gets valid GUID strings for <c>[StrongId&lt;Guid&gt;]</c> parsing.
    /// </summary>
    public static IEnumerable<object[]> ValidGuidStrings => new List<object[]>
    {
        new object[] { "3fa85f64-5717-4562-b3fc-2c963f66afa6" },
        new object[] { "3FA85F64-5717-4562-B3FC-2C963F66AFA6" },
        new object[] { "{3fa85f64-5717-4562-b3fc-2c963f66afa6}" },
    };

    /// <summary>
    /// Gets invalid GUID strings for <c>[StrongId&lt;Guid&gt;]</c> negative testing.
    /// </summary>
    public static IEnumerable<object[]> InvalidGuidStrings => new List<object[]>
    {
        new object[] { "" },
        new object[] { "not-a-guid" },
        new object[] { "00000000-0000-0000-0000-00000000000Z" },
    };

    // ─── Numeric Range Scenarios ───────────────────────────────────────────────

    /// <summary>
    /// Gets valid age values (0–150).
    /// </summary>
    public static IEnumerable<object[]> ValidAgeValues => new List<object[]>
    {
        new object[] { 0 },
        new object[] { 1 },
        new object[] { 18 },
        new object[] { 65 },
        new object[] { 150 },
    };

    /// <summary>
    /// Gets invalid age values (outside 0–150).
    /// </summary>
    public static IEnumerable<object[]> InvalidAgeValues => new List<object[]>
    {
        new object[] { -1 },
        new object[] { 151 },
        new object[] { int.MinValue },
        new object[] { int.MaxValue },
    };

    /// <summary>
    /// Gets valid percentage values (0–100).
    /// </summary>
    public static IEnumerable<object[]> ValidPercentageValues => new List<object[]>
    {
        new object[] { 0.0m },
        new object[] { 50.0m },
        new object[] { 99.99m },
        new object[] { 100.0m },
    };

    /// <summary>
    /// Gets invalid percentage values (outside 0–100).
    /// </summary>
    public static IEnumerable<object[]> InvalidPercentageValues => new List<object[]>
    {
        new object[] { -0.01m },
        new object[] { 100.01m },
        new object[] { decimal.MinValue },
    };

    // ─── Normalization Scenarios ───────────────────────────────────────────────

    /// <summary>
    /// Gets pairs of (rawInput, expectedNormalized) for email address normalization.
    /// Demonstrates that <c>[Trim]</c> and <c>[LowerCase]</c> are applied before validation.
    /// </summary>
    public static IEnumerable<object[]> EmailNormalizationScenarios => new List<object[]>
    {
        new object[] { " USER@EXAMPLE.COM ", "user@example.com" },
        new object[] { "User.Name@Example.COM", "user.name@example.com" },
        new object[] { "  test@test.io  ", "test@test.io" },
    };

    /// <summary>
    /// Gets pairs of (rawInput, expectedNormalized) for country code normalization.
    /// Demonstrates that <c>[Trim]</c> and <c>[UpperCase]</c> are applied.
    /// </summary>
    public static IEnumerable<object[]> CountryCodeNormalizationScenarios => new List<object[]>
    {
        new object[] { " us ", "US" },
        new object[] { "gb", "GB" },
        new object[] { "  de  ", "DE" },
    };
}
