# Cookbook: EricksonLopez.DomainPrimitives

A comprehensive recipe guide covering domain modeling patterns, semantic shortcuts, validation pipelines, framework integrations, and testing strategies.

---

## Table of Contents

1. [Strongly-Typed Identifiers](#1-strongly-typed-identifiers)
2. [String Semantic Shortcuts (15 Built-in Types)](#2-string-semantic-shortcuts-15-built-in-types)
3. [Numeric Semantic Shortcuts (15 Built-in Types)](#3-numeric-semantic-shortcuts-15-built-in-types)
4. [Temporal Primitives (DateOnly & DateTime)](#4-temporal-primitives-dateonly--datetime)
5. [Multi-Property Composite Value Objects](#5-multi-property-composite-value-objects)
6. [Compile-Time Exhaustive Smart Enums](#6-compile-time-exhaustive-smart-enums)
7. [Safe Parsing & Zero-Alloc Validation Pipelines](#7-safe-parsing--zero-alloc-validation-pipelines)
8. [Fluent Validation with PrimitiveBuilder](#8-fluent-validation-with-primitivebuilder)
9. [Bulk Collection Operations](#9-bulk-collection-operations)
10. [Entity Framework Core Integration (Zero-Contamination)](#10-entity-framework-core-integration-zero-contamination)
11. [Dapper Integration (Auto-Registration)](#11-dapper-integration-auto-registration)
12. [ASP.NET Core Minimal APIs & Model Binding](#12-aspnet-core-minimal-apis--model-binding)
13. [Object Mapping (Mapperly & Mapster)](#13-object-mapping-mapperly--mapster)
14. [Newtonsoft.Json Migration Integration](#14-newtonsoftjson-migration-integration)
15. [Testing with DomainPrimitiveFakeFactory](#15-testing-with-domainprimitivefakefactory)
16. [Integrating TryCreate with Result Libraries (Railway-Oriented Programming)](#16-integrating-trycreate-with-result-libraries-railway-oriented-programming)

---

## 1. Strongly-Typed Identifiers

**Problem:** Avoid "Primitive Obsession" and accidental cross-assignment between different entity IDs (e.g. passing `OrderId` into `CustomerId`).

**Solution:** Use `[StrongId<T>]` on a `readonly partial record struct`.

```csharp
using EricksonLopez.DomainPrimitives;

[StrongId<Guid>]
public readonly partial record struct CustomerId;

[StrongId<long>]
public readonly partial record struct OrderId;
```

**Features Generated:**
- Factory methods: `CustomerId.Create(guid)`, `CustomerId.Create()` (generates new `Guid`), `CustomerId.TryCreate(...)`.
- Non-empty validation by default: rejects `Guid.Empty` and `0` when configured.
- Native `System.Text.Json` converter, `TypeConverter`, and `IFormattable` / `ISpanParsable<T>`.

---

## 2. String Semantic Shortcuts (15 Built-in Types)

DomainPrimitives includes 15 ready-to-use semantic string shortcuts that apply normalization (Unicode NFC, trim, casing) and strict validation rules with zero boilerplate:

```csharp
using EricksonLopez.DomainPrimitives;

// Identity & Communications
[Email]
public readonly partial record struct EmailAddress;

[Phone]
public readonly partial record struct PhoneNumber;

[Url]
public readonly partial record struct WebsiteUrl;

[Slug]
public readonly partial record struct ArticleSlug;

[Username]
public readonly partial record struct AccountUsername;

[PasswordHash]
public readonly partial record struct HashedPassword;

// Geographic & International
[CountryCode] // ISO 3166-1 alpha-2 / alpha-3
public readonly partial record struct CountryIsoCode;

[LanguageCode] // ISO 639-1
public readonly partial record struct LanguageIsoCode;

[CurrencyCode] // ISO 4217
public readonly partial record struct CurrencyIsoCode;

[IBAN] // International Bank Account Number
public readonly partial record struct BankIban;

// Identifiers & Standards
[ISBN] // International Standard Book Number (ISBN-10 / ISBN-13)
public readonly partial record struct BookIsbn;

[VIN] // Vehicle Identification Number
public readonly partial record struct VehicleVin;

// Web & Networking
[IPAddress] // IPv4 / IPv6 validated
public readonly partial record struct ClientIpAddress;

[MacAddress] // Hardware MAC address
public readonly partial record struct DeviceMacAddress;

[HexColor] // Hexadecimal web color code (#FFF, #FFFFFF)
public readonly partial record struct ThemeColor;
```

---

## 3. Numeric Semantic Shortcuts (15 Built-in Types)

Built-in numeric shortcuts with pre-configured domain ranges, constraints, and operator rules:

```csharp
using EricksonLopez.DomainPrimitives;

// Commerce & Financial
[Money] // decimal, NonNegative
public readonly partial record struct AccountBalance;

[Price] // decimal, Positive
public readonly partial record struct ProductPrice;

[TaxRate] // decimal (0.0 to 1.0)
public readonly partial record struct VatRate;

[Discount] // decimal (0 to 100)
public readonly partial record struct PromotionDiscount;

// Measurements & Units
[Percentage] // double (0 to 100)
public readonly partial record struct CompletionPercentage;

[Latitude] // double (-90 to +90)
public readonly partial record struct GeoLatitude;

[Longitude] // double (-180 to +180)
public readonly partial record struct GeoLongitude;

[Age] // int (0 to 150)
public readonly partial record struct UserAge;

[Weight] // double (Positive)
public readonly partial record struct PackageWeight;

[Height] // double (Positive)
public readonly partial record struct PersonHeight;

[Distance] // double (NonNegative)
public readonly partial record struct TravelDistance;

[Temperature] // double (Celsius / Kelvin bounds)
public readonly partial record struct RoomTemperature;

// Business Metrics
[Score] // int (0 to 1000)
public readonly partial record struct CreditScore;

[Quantity] // int (NonNegative)
public readonly partial record struct StockQuantity;

[Rating] // double (1.0 to 5.0)
public readonly partial record struct CustomerReviewRating;
```

---

## 4. Temporal Primitives (DateOnly & DateTime)

Use `[DatePrimitive]` to model calendar dates and timestamps with past/future invariant guarantees:

```csharp
using EricksonLopez.DomainPrimitives;

[DatePrimitive(Kind = DatePrimitiveKind.DateOnly, PastOnly = true)]
public readonly partial record struct DateOfBirth;

[DatePrimitive(Kind = DatePrimitiveKind.DateTime, FutureOnly = true)]
public readonly partial record struct ReservationDeadline;
```

---

## 5. Multi-Property Composite Value Objects

Composite Value Objects model multiple cohesive properties that must satisfy cross-property invariants:

```csharp
using EricksonLopez.DomainPrimitives;
using EricksonLopez.DomainPrimitives.Validation;

[ValueObject]
public readonly partial record struct Address(string Street, string City, string State, string ZipCode)
{
    static partial void Validate(ref Address value, ref PrimitiveError error)
    {
        if (string.IsNullOrWhiteSpace(value.Street))
            error = new PrimitiveError("Address.EmptyStreet", "Street cannot be empty.");
        else if (string.IsNullOrWhiteSpace(value.City))
            error = new PrimitiveError("Address.EmptyCity", "City cannot be empty.");
    }
}
```

---

## 6. Compile-Time Exhaustive Smart Enums

Model type-safe, AOT-friendly enums with O(1) lookups and compile-time exhaustiveness:

```csharp
using EricksonLopez.DomainPrimitives;

[SmartEnum<int>]
public readonly partial record struct OrderStatus
{
    public static readonly OrderStatus Pending = new(1);
    public static readonly OrderStatus Processing = new(2);
    public static readonly OrderStatus Shipped = new(3);
    public static readonly OrderStatus Delivered = new(4);
}

// Compile-time exhaustive pattern matching:
var statusDescription = status.Match(
    whenPending: () => "Awaiting processing",
    whenProcessing: () => "Being prepared in warehouse",
    whenShipped: () => "In transit with courier",
    whenDelivered: () => "Successfully delivered");
```

---

## 7. Safe Parsing & Zero-Alloc Validation Pipelines

Handle user input cleanly without incurring exception throwing overhead:

```csharp
using EricksonLopez.DomainPrimitives.Validation;

// TryCreate with out PrimitiveError (Stack-allocated readonly struct)
if (EmailAddress.TryCreate(userInput, out var email, out PrimitiveError error))
{
    Console.WriteLine($"Normalized valid email: {email.Value}");
}
else
{
    Console.WriteLine($"Validation failed [{error.Code}]: {error.Message}");
}

// High-performance UTF-8 byte span parsing (Minimal APIs, HTTP pipelines):
ReadOnlySpan<byte> utf8Buffer = "user@example.com"u8;
if (EmailAddress.TryParse(utf8Buffer, null, out var parsedEmail))
{
    // Zero string allocations on parse validation
}
```

---

## 8. Fluent Validation with PrimitiveBuilder

Build and validate primitives programmatically with ad-hoc business rules:

```csharp
using EricksonLopez.DomainPrimitives.Advanced;

var promoCode = PrimitiveBuilder<VoucherCode, string>
    .For()
    .WithValue("SUMMER2026")
    .Must(code => code.StartsWith("SUMMER"), "INVALID_SEASON", "Code must start with current season.")
    .BuildOrThrow();
```

---

## 9. Bulk Collection Operations

Transform and validate collections of raw values into typed domain primitive collections using `PrimitiveCollectionExtensions`:

```csharp
using EricksonLopez.DomainPrimitives;

var rawIds = new[] { 1, 2, 3, 42 };

// IEnumerable<int> → List<ProductId> (throws DomainPrimitiveValidationException on first invalid)
List<ProductId> productIds = rawIds.ToDomainPrimitiveList<ProductId, int>();

// IEnumerable<string> → EmailAddress[]
string[] rawEmails = ["alice@test.com", "bob@test.com"];
EmailAddress[] addresses = rawEmails.ToDomainPrimitiveArray<EmailAddress, string>();

// ReadOnlySpan<string> → EmailAddress[] (zero-copy span path, .NET 7+)
ReadOnlySpan<string> span = rawEmails.AsSpan();
EmailAddress[] fromSpan = span.ToDomainPrimitiveArray<EmailAddress, string>();
```

**Note:** All three overloads call `Create()` internally and throw `DomainPrimitiveValidationException` on the first invalid element. To collect valid/invalid separately, iterate manually with `TryCreate()`:

```csharp
var valid = new List<EmailAddress>();
var errors = new List<(string raw, PrimitiveError error)>();

foreach (var raw in rawEmails)
{
    if (EmailAddress.TryCreate(raw, out var email, out var error))
        valid.Add(email);
    else
        errors.Add((raw, error));
}
```

---

## 10. Entity Framework Core Integration (Zero-Contamination)

Keep your domain pure without any EF Core attributes in the domain project.

```csharp
using EricksonLopez.DomainPrimitives.EFCore.Generated;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Customer> Customers => Set<Customer>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Auto-discovers and registers ValueConverters and max lengths for all primitives
        configurationBuilder.ConfigureDomainPrimitives();
    }
}
```

---

## 11. Dapper Integration (Auto-Registration)

Automatically register Dapper `SqlMapper.TypeHandler` for all primitives at startup:

```csharp
using EricksonLopez.DomainPrimitives.Dapper.Generated;

// In Program.cs
DapperDomainPrimitivesRegistration.RegisterAll();

// Queries automatically map strongly-typed primitives
var customer = await connection.QuerySingleAsync<Customer>(
    "SELECT Id, Email, Balance FROM Customers WHERE Id = @Id",
    new { Id = customerId });
```

---

## 12. ASP.NET Core Minimal APIs & Model Binding

Primitives bind natively in ASP.NET Core route parameters, query strings, and request bodies thanks to `IParsable<T>` and `IUtf8SpanParsable<T>`:

```csharp
var app = WebApplication.Create();

// Automatically parsed from route via IParsable<CustomerId>
app.MapGet("/customers/{id}", (CustomerId id) => Results.Ok(new { Id = id.Value }));

// Automatically bound and validated from JSON body
app.MapPost("/customers", (CreateCustomerRequest request) => 
{
    // request properties are already strongly-typed primitives
    return Results.Created($"/customers/{request.Id}", request);
});
```

---

## 13. Object Mapping (Mapperly & Mapster)

### Mapperly (Zero-Configuration via Explicit Operators)
DomainPrimitives generates explicit operators (`(string)email`, `(EmailAddress)rawString`) that Mapperly discovers out of the box:

```csharp
using Riok.Mapperly.Abstractions;

[Mapper]
public partial class UserMapper
{
    public partial UserDto ToDto(User user);
    public partial User ToEntity(UserDto dto);
}
```

### Mapster
Because DomainPrimitives generates explicit conversion operators for all scalar primitives, Mapster resolves scalar conversions automatically without extra packages:

```csharp
using Mapster;

// Mapster resolves (string)customer.Email and (EmailAddress)dto.Email automatically
var dto = customer.Adapt<CustomerDto>();
var domain = dto.Adapt<Customer>();
```

---

## 14. Newtonsoft.Json Migration Integration

For legacy systems using `Newtonsoft.Json`, reference `EricksonLopez.DomainPrimitives.NewtonsoftJson`:

```csharp
using EricksonLopez.DomainPrimitives.NewtonsoftJson;
using Newtonsoft.Json;

var settings = new JsonSerializerSettings();
settings.AddDomainPrimitives(); // Registers ContractResolver and converters
```

---

## 15. Testing with DomainPrimitiveFakeFactory

Generate realistic domain test data using `EricksonLopez.DomainPrimitives.Testing`:

```csharp
using EricksonLopez.DomainPrimitives.Testing;

// --- DomainPrimitiveFakeFactory: curated collections of valid/invalid inputs ---

// String collections
string[] validEmails   = DomainPrimitiveFakeFactory.Strings.ValidEmails;    // RFC 5321 valid
string[] invalidEmails = DomainPrimitiveFakeFactory.Strings.InvalidEmails;  // known invalid patterns
string[] validPhones   = DomainPrimitiveFakeFactory.Strings.ValidPhones;
string[] validUrls     = DomainPrimitiveFakeFactory.Strings.ValidUrls;
string[] validSlugs    = DomainPrimitiveFakeFactory.Strings.ValidSlugs;
string[] validCodes    = DomainPrimitiveFakeFactory.Strings.ValidCountryCodes;

// Numeric collections
decimal[] validAmounts = DomainPrimitiveFakeFactory.Numerics.ValidMoneyAmounts;
int[]     validAges    = DomainPrimitiveFakeFactory.Numerics.ValidAges;
double[]  validLats    = DomainPrimitiveFakeFactory.Numerics.ValidLatitudes;
int[]     validScores  = DomainPrimitiveFakeFactory.Numerics.ValidScores;
int[]     validQtys    = DomainPrimitiveFakeFactory.Numerics.ValidQuantities;

// --- DomainPrimitiveTestBuilder: xUnit/NUnit/MSTest helpers ---

// Create a valid primitive (throws if invalid)
var email = DomainPrimitiveTestBuilder.Create<EmailAddress, string>("user@example.com");

// Assert creation fails and capture the exception
var ex = DomainPrimitiveTestBuilder.AssertCreationFails<EmailAddress, string>("");

// Create unvalidated (for persistence/mapping tests with dirty data only):
var dirty = DomainPrimitiveTestBuilder.CreateUnvalidated<EmailAddress, string>("not-an-email");

// --- DomainPrimitiveScenarios: grouped scenario data for parameterized tests ---

string[] validInputs   = DomainPrimitiveScenarios.ValidEmailInputs;
string[] invalidInputs = DomainPrimitiveScenarios.InvalidEmailInputs;
string[] guidStrings   = DomainPrimitiveScenarios.ValidGuidStrings;
int[]    validAgeVals  = DomainPrimitiveScenarios.ValidAgeValues;
int[]    badAgeVals    = DomainPrimitiveScenarios.InvalidAgeValues;

// Normalization scenarios (raw → expected normalized form):
var normScenarios = DomainPrimitiveScenarios.EmailNormalizationScenarios;
foreach (var (raw, expected) in normScenarios)
    Console.WriteLine($"{raw} → {expected}");
```

**Note:** `DomainPrimitiveAssertionsExtensions` (`HavePrimitiveValue<>`, `ThrowDomainPrimitiveException`, `ShouldFailCreationWith<>`, etc.) requires an `AwesomeAssertions`-based test runner (xUnit, NUnit, MSTest) and cannot be used in console applications.

---

## 16. Integrating TryCreate with Result Libraries (Railway-Oriented Programming)

**Problem:** You want to use Railway-Oriented Programming (ROP) or a `Result<T>` monad in your application layer (such as `EricksonLopez.Result`, `ErrorOr`, `FluentResults`, or `LanguageExt`) without coupling your core domain primitives or sacrificing zero-allocation performance at the domain level.

**Solution:** As decided in [ADR-025](adr/adr-025-reject-result-as-primary-api.md), `DomainPrimitives` generates an uncoupled, zero-allocation `TryCreate(raw, out TSelf result, out PrimitiveError error)` BCL-style pattern. In your application layer, define a thin (3-5 lines) extension method or adapter to bridge `TryCreate` with your chosen Result library.

### Example 1: Integrating with `EricksonLopez.Result`

```csharp
using EricksonLopez.DomainPrimitives;
using EricksonLopez.Results;

public static class DomainResultExtensions
{
    public static Result<EmailAddress> ToResult(string raw) =>
        EmailAddress.TryCreate(raw, out var email, out var error)
            ? Result<EmailAddress>.Success(email)
            : Result<EmailAddress>.Failure(error.Code, error.Message);

    public static Result<MoneyAmount> ToResult(decimal raw) =>
        MoneyAmount.TryCreate(raw, out var money, out var error)
            ? Result<MoneyAmount>.Success(money)
            : Result<MoneyAmount>.Failure(error.Code, error.Message);
}
```

### Example 2: Generic Adapter for Any Domain Primitive

```csharp
public delegate bool TryCreateDelegate<TRaw, TPrimitive>(TRaw raw, out TPrimitive result, out PrimitiveError error);

public static class ResultBridge
{
    public static Result<TPrimitive> FromPrimitive<TRaw, TPrimitive>(
        TRaw raw,
        TryCreateDelegate<TRaw, TPrimitive> tryCreate)
    {
        return tryCreate(raw, out var result, out var error)
            ? Result<TPrimitive>.Success(result)
            : Result<TPrimitive>.Failure(error.Code, error.Message);
    }
}

// Usage:
Result<EmailAddress> emailResult = ResultBridge.FromPrimitive<string, EmailAddress>(
    input, 
    EmailAddress.TryCreate);
```

### Example 3: Integrating with Third-Party Result Libraries (`ErrorOr`, `FluentResults`)

```csharp
// ErrorOr adapter
public static ErrorOr<EmailAddress> ToErrorOr(string raw) =>
    EmailAddress.TryCreate(raw, out var email, out var error)
        ? email
        : Error.Validation(error.Code, error.Message);

// FluentResults adapter
public static FluentResults.Result<EmailAddress> ToFluentResult(string raw) =>
    EmailAddress.TryCreate(raw, out var email, out var error)
        ? FluentResults.Result.Ok(email)
        : FluentResults.Result.Fail(new FluentResults.Error(error.Message).WithMetadata("Code", error.Code));
```

**Key Benefits:**
- **Zero Heap Allocations in Domain:** Core validations remain 0-allocation value types on the stack.
- **Zero Library Coupling:** Neither `DomainPrimitives` nor your `Result` package depends on the other.
- **Ecosystem Flexibility:** Use any functional library version in application handlers without dependency conflicts.

