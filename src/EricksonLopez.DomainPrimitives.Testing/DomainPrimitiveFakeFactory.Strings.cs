// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;

namespace EricksonLopez.DomainPrimitives.Testing;

public static partial class DomainPrimitiveFakeFactory
{
    /// <summary>Provides deterministic fake test data for string-based domain primitives.</summary>
    public static class Strings
    {

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
    }
}

