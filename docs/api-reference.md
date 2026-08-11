# API Reference

This reference details the main methods and types exposed by `EricksonLopez.DomainPrimitives`. For the complete list of supported attributes, see the [API Inventory](api-inventory.md).

## Core Interfaces

### `IDomainPrimitive<TSelf, TValue>`
Base interface implemented by all domain primitives.

| Member | Signature | Description |
|:-------|:----------|:------------|
| `Value` | `TValue Value { get; }` | Gets the raw value encapsulated by the primitive. |
| `Create` | `static TSelf Create(TValue value)` | Creates a new instance. Throws `DomainPrimitiveValidationException` if validation fails. Use when data comes from a trusted source. |
| `TryCreate` | `static bool TryCreate(TValue value, out TSelf result, out PrimitiveError error)` | Attempts to create a new instance. Returns `true` on success; sets `error` on failure. Zero-allocation on the success path. |

### `IStrongId<TSelf, TValue>`
Base interface for strongly-typed identifiers.

| Member | Signature | Description |
|:-------|:----------|:------------|
| `Create()` | `static TSelf Create()` | Generates a new identifier (Guid-backed: `Guid.NewGuid()`). |
| `Create(TValue)` | `static TSelf Create(TValue value)` | Creates a typed ID from an existing value. |
| `TryCreate` | `static bool TryCreate(TValue value, out TSelf result, out PrimitiveError error)` | Attempts creation without exceptions. |
| `Empty` | `static TSelf Empty { get; }` | Returns the empty/default identifier. Rejected by default for Guid-backed IDs per RFC-0002. |

### `PrimitiveError`
Struct returned via `out` parameter on validation failure. Zero heap allocation on the success path.

| Member | Description |
|:-------|:------------|
| `string? Code` | Short error code (e.g., `"FORMAT"`, `"LENGTH"`, `"RANGE"`). Null on `PrimitiveError.None`. |
| `string? Message` | Human-readable description. Never echoes user input (SEC-005). |
| `static PrimitiveError None` | Sentinel value indicating no error. |
| `static PrimitiveError Create(string code, string message)` | Factory method. |

## Exceptions

| Type | When Thrown | Base |
|:-----|:------------|:-----|
| `DomainPrimitiveValidationException` | `Create()` receives an invalid value | `System.Exception` |
| `System.FormatException` | `Parse()` / `TryParse()` receives unparseable input (per RFC-0003) | `System.FormatException` |
| `DomainPrimitiveFormatException` | **Deprecated** `[Obsolete]` — use `System.FormatException` catch | `System.FormatException` |

## Generator Attributes

### `[StringPrimitive]`
Marks a `readonly partial record struct` for source generation of a string-backed primitive.
```csharp
[StringPrimitive]
[Trim][LowerCase][MaxLength(254)]
public readonly partial record struct EmailHandle;
```

### `[NumericPrimitive<TValue>]`
Marks a struct for generation of a numeric primitive backed by `TValue` (int, decimal, double, etc.).
```csharp
[NumericPrimitive<decimal>]
[Range(0, 100)]
public readonly partial record struct Percentage;
```

### `[DatePrimitive]`
Marks a struct for generation of a date-backed primitive (`DateOnly` or `DateTime`).

### `[StrongId<TValue>]`
Generates a strongly-typed identifier. Default `TValue` options: `Guid`, `int`, `long`, `string`.
```csharp
[StrongId<Guid>]
public readonly partial record struct OrderId;
```

### `[ValueObject]`
Marks a `readonly partial record struct` for generation of a multi-property value object.

### `[SmartEnum<TValue>]`
Generates an AOT-safe strongly-typed enum with source-generated `GetAll()`, `FromName()`, and `Match<TResult>()`.

## Normalization Attributes (Namespace: `EricksonLopez.DomainPrimitives.Normalization`)

| Attribute | Effect |
|:----------|:-------|
| `[Trim]` | Trims leading/trailing whitespace before validation |
| `[LowerCase]` | Converts to lowercase invariant before validation |
| `[UpperCase]` | Converts to uppercase invariant before validation |
| `[NormalizeWhitespace]` | Collapses internal whitespace runs to single space |

## Semantic Shortcut Attributes

These imply `[StringPrimitive]` or `[NumericPrimitive<T>]` plus a standard normalization and validation rule set.

### String Shortcuts

| Attribute | Implies |
|:----------|:--------|
| `[Email]` | `[StringPrimitive]`, `[Trim]`, `[LowerCase]`, RFC 5321 regex |
| `[Url]` | `[StringPrimitive]`, `[Trim]`, absolute URL with http/https |
| `[Phone]` | `[StringPrimitive]`, `[Trim]`, E.164 format |
| `[CountryCode]` | `[StringPrimitive]`, `[Trim]`, `[UpperCase]`, ISO 3166-1 alpha-2 |
| `[CurrencyCode]` | `[StringPrimitive]`, `[Trim]`, `[UpperCase]`, ISO 4217 |
| `[LanguageCode]` | `[StringPrimitive]`, `[Trim]`, `[LowerCase]`, BCP 47 |
| `[IBAN]` | `[StringPrimitive]`, `[Trim]`, `[UpperCase]`, IBAN format |
| `[Username]` | `[StringPrimitive]`, `[Trim]`, `[LowerCase]`, alphanumeric + underscore |
| `[PasswordHash]` | `[StringPrimitive]`, `[NotEmpty]`, PII-safe (SEC-005) |
| `[Slug]` | `[StringPrimitive]`, `[Trim]`, `[LowerCase]`, URL slug format |
| `[HexColor]` | `[StringPrimitive]`, `[Trim]`, `[UpperCase]`, hex color format |
| `[ISBN]` | `[StringPrimitive]`, `[Trim]`, ISBN-10 or ISBN-13 |
| `[IPAddress]` | `[StringPrimitive]`, `[Trim]`, IPv4 or IPv6 |
| `[MacAddress]` | `[StringPrimitive]`, `[Trim]`, MAC address |
| `[VIN]` | `[StringPrimitive]`, `[Trim]`, `[UpperCase]`, VIN format |

### Numeric Shortcuts

| Attribute | Backing Type | Range |
|:----------|:------------|:------|
| `[Age]` | `int` | 0–150 |
| `[Money]` | `decimal` | ≥ 0 |
| `[Percentage]` | `decimal` | 0–100 |
| `[Price]` | `decimal` | ≥ 0 |
| `[TaxRate]` | `decimal` | 0–100 |
| `[Discount]` | `decimal` | 0–100 |
| `[Rating]` | `decimal` | 0–5 |
| `[Score]` | `decimal` | 0–100 |
| `[Quantity]` | `int` | ≥ 0 |
| `[Latitude]` | `double` | -90 to 90 |
| `[Longitude]` | `double` | -180 to 180 |
| `[Weight]` | `decimal` | ≥ 0 |
| `[Height]` | `decimal` | ≥ 0 |
| `[Distance]` | `decimal` | ≥ 0 |
| `[Temperature]` | `double` | unrestricted |

## Integration Extension Methods

### `EricksonLopez.DomainPrimitives.EFCore`

Auto-discovered via source generator. Registers all `ValueConverter<TDomain, TValue>` implementations automatically on `ModelBuilder`.

### `EricksonLopez.DomainPrimitives.Dapper`

Auto-discovered via source generator. Generates `SqlMapper.TypeHandler` implementations for all domain primitives. Call `DomainPrimitivesDapperExtensions.RegisterAll()` at application startup.

### `EricksonLopez.DomainPrimitives.AspNetCore`

Auto-discovered via source generator. Generates `IModelBinder` implementations for route and query string binding. Register via `services.AddDomainPrimitives()` or `builder.Services.AddControllers().AddDomainPrimitives()`.

## `PrimitiveBuilder<TPrimitive, TValue>`

Fluent builder for test and sample data construction.

```csharp
var email = PrimitiveBuilder<ContactEmail, string>
    .For("user@example.com")
    .Must(v => v.Contains('@'), "FORMAT", "Must be an email")
    .BuildOrThrow(); // throws DomainPrimitiveValidationException on failure
```

| Method | Return | Description |
|:-------|:-------|:------------|
| `For(TValue value)` | `PrimitiveBuilder<T,V>` | Static factory — sets value |
| `Must(Func<TValue,bool>, code, msg)` | `PrimitiveBuilder<T,V>` | Adds a validation predicate |
| `Build(out TPrimitive, out PrimitiveError)` | `bool` | Non-throwing build |
| `BuildOrThrow()` | `TPrimitive` | Throwing build |
