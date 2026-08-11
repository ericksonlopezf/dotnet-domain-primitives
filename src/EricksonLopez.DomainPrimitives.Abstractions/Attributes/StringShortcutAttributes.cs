using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Shortcut for an email address domain primitive.
/// Equivalent to: <c>[StringPrimitive] [Trim] [LowerCase] [MaxLength(320)] [Regex(RFC5322)]</c>.
/// </summary>
/// <remarks>
/// <para>Generates RFC 5322-compliant email validation with automatic trimming and lowercasing.</para>
/// <para>Override <see cref="MaxLength"/> to change the default 320-character limit.</para>
/// </remarks>
/// <example>
/// <code>
/// [Email]
/// public readonly partial record struct EmailAddress;
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class EmailAttribute : Attribute
{
    /// <summary>Maximum length. Default: 320 (RFC 5321).</summary>
    public int MaxLength { get; init; } = 320;
}

/// <summary>
/// Shortcut for a phone number domain primitive.
/// Equivalent to: <c>[StringPrimitive] [Trim] [Regex(E.164)]</c>.
/// </summary>
/// <remarks>
/// Generates E.164-compliant phone number validation.
/// </remarks>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class PhoneAttribute : Attribute
{
}

/// <summary>
/// Shortcut for a URL domain primitive.
/// Equivalent to: <c>[StringPrimitive] [Trim]</c> with <see cref="System.Uri"/> validation.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class UrlAttribute : Attribute
{
    /// <summary>
    /// Allowed URI schemes. Default: <c>["https", "http"]</c>.
    /// </summary>
    public string[] AllowedSchemes { get; init; } = ["https", "http"];
}

/// <summary>
/// Shortcut for a URL slug domain primitive.
/// Equivalent to: <c>[StringPrimitive] [Trim] [LowerCase] [Regex(slug-pattern)]</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class SlugAttribute : Attribute
{
    /// <summary>Maximum length. Default: 200.</summary>
    public int MaxLength { get; init; } = 200;
}

/// <summary>
/// Shortcut for an ISO 3166-1 alpha-2 country code (e.g., "US", "DE").
/// Equivalent to: <c>[StringPrimitive] [Trim] [UpperCase] [Length(2, 2)]</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class CountryCodeAttribute : Attribute
{
}

/// <summary>
/// Shortcut for an ISO 639-1 language code (e.g., "en", "es").
/// Equivalent to: <c>[StringPrimitive] [Trim] [LowerCase] [Length(2, 2)]</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class LanguageCodeAttribute : Attribute
{
}

/// <summary>
/// Shortcut for an ISO 4217 currency code (e.g., "USD", "EUR").
/// Equivalent to: <c>[StringPrimitive] [Trim] [UpperCase] [Length(3, 3)]</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class CurrencyCodeAttribute : Attribute
{
}

/// <summary>
/// Shortcut for a username domain primitive.
/// Equivalent to: <c>[StringPrimitive] [Trim] [Regex(alphanumeric + ._-)]</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class UsernameAttribute : Attribute
{
    /// <summary>Minimum length. Default: 3.</summary>
    public int MinLength { get; init; } = 3;

    /// <summary>Maximum length. Default: 50.</summary>
    public int MaxLength { get; init; } = 50;
}

/// <summary>
/// Shortcut for a password hash domain primitive.
/// Equivalent to: <c>[StringPrimitive] [NotEmpty]</c>.
/// </summary>
/// <remarks>
/// No normalization is applied — hashes must never be trimmed or case-changed.
/// </remarks>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class PasswordHashAttribute : Attribute
{
}

/// <summary>
/// Shortcut for a CSS hex color domain primitive (e.g., "#FF5733").
/// Equivalent to: <c>[StringPrimitive] [Trim] [UpperCase] [Regex(hex-color)]</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class HexColorAttribute : Attribute
{
}

/// <summary>
/// Shortcut for an IPv4 or IPv6 address domain primitive.
/// Equivalent to: <c>[StringPrimitive] [Trim] [Regex(ip-pattern)]</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class IPAddressAttribute : Attribute
{
}

/// <summary>
/// Shortcut for a MAC address domain primitive.
/// Equivalent to: <c>[StringPrimitive] [Trim] [UpperCase] [Regex(mac-pattern)]</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class MacAddressAttribute : Attribute
{
}

/// <summary>
/// Shortcut for an International Bank Account Number (IBAN) domain primitive.
/// Equivalent to: <c>[StringPrimitive] [Trim] [UpperCase] [Regex(iban-pattern)]</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class IBANAttribute : Attribute
{
}

/// <summary>
/// Shortcut for an International Standard Book Number (ISBN) domain primitive.
/// Equivalent to: <c>[StringPrimitive] [Trim] [Regex(isbn-pattern)]</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class ISBNAttribute : Attribute
{
}

/// <summary>
/// Shortcut for a Vehicle Identification Number (VIN) domain primitive.
/// Equivalent to: <c>[StringPrimitive] [Trim] [UpperCase] [Regex(vin-pattern)]</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class VINAttribute : Attribute
{
}
