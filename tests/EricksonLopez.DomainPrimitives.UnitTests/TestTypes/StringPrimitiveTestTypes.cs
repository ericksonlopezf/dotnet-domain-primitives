using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.DomainPrimitives;



namespace EricksonLopez.DomainPrimitives.Tests.TestTypes;

/// <summary>
/// Basic string primitive with trim + length validation.
/// </summary>
[StringPrimitive]
[Trim]
[MinLength(1)]
[MaxLength(100)]
public readonly partial record struct FirstName;

/// <summary>
/// Email address using the [Email] domain shortcut.
/// </summary>
[Email]
public readonly partial record struct EmailAddress;

/// <summary>
/// ISO 3166-1 alpha-2 country code (e.g., "US", "DE").
/// </summary>
[CountryCode]
public readonly partial record struct Country;

/// <summary>
/// String primitive with regex validation.
/// </summary>
[StringPrimitive]
[Trim]
[Regex(@"^[A-Z]{2}-\d{4}$", ErrorMessage = "Must be in format XX-0000")]
public readonly partial record struct ProductCode;

/// <summary>
/// String primitive with multiple normalizations (trim + uppercase + whitespace).
/// </summary>
[StringPrimitive]
[Trim]
[UpperCase]
[NormalizeWhitespace]
[NotEmpty]
public readonly partial record struct DisplayName;

/// <summary>
/// URL with custom allowed schemes.
/// </summary>
[Url(AllowedSchemes = new[] { "https", "ftp" })]
public readonly partial record struct SecureFtpUrl;

/// <summary>
/// Password hash — sensitive type. Error messages must not include the rejected value (SEC-005).
/// </summary>
[PasswordHash]
public readonly partial record struct PasswordHashValue;

/// <summary>
/// API secret token — sensitive type. Error messages must not include the rejected value (SEC-005).
/// </summary>
[StringPrimitive]
[NotEmpty]
[MinLength(32)]
public readonly partial record struct ApiSecret;

/// <summary>
/// Lowercase email for NFC normalization tests.
/// </summary>
[StringPrimitive]
[Trim]
[LowerCase]
[MinLength(1)]
[MaxLength(254)]
public readonly partial record struct LowercaseTag;
