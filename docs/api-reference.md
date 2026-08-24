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

### `IDomainPrimitive<TSelf>`
Base contract members present on all generated primitives.

| Member | Signature | Description |
|:-------|:----------|:------------|
| `IsDefault` | `bool IsDefault { get; }` | Returns `true` if this struct holds its default (uninitialized) value. |
| `PrimitiveName` | `static string PrimitiveName { get; }` | **.NET 7+ only.** Returns the canonical name of the primitive type (e.g., `"EmailAddress"`). Useful for logging, metrics, and diagnostics. |

### `IStrongId<TSelf, TValue>`
Base interface for strongly-typed identifiers.

| Member | Signature | Description |
|:-------|:----------|:------------|
| `Create()` | `static TSelf Create()` | Generates a new identifier (Guid-backed: `Guid.NewGuid()`). |
| `Create(TValue)` | `static TSelf Create(TValue value)` | Creates a typed ID from an existing value. |
| `TryCreate` | `static bool TryCreate(TValue value, out TSelf result, out PrimitiveError error)` | Attempts creation without exceptions. |
| `Empty` | `static TSelf Empty { get; }` | Returns the empty/default identifier. Rejected by default for Guid-backed IDs per rfc-0002. |

### `PrimitiveError`
Namespace: `EricksonLopez.DomainPrimitives.Validation`. Struct returned via `out` parameter on validation failure. Zero heap allocation on the success path.

| Member | Description |
|:-------|:------------|
| `string? Code` | Short error code (e.g., `"FORMAT"`, `"LENGTH"`, `"RANGE"`). Null on `PrimitiveError.None`. |
| `string? Message` | Human-readable description. Never echoes user input (SEC-005). |
| `bool IsError` | Returns `true` if this instance represents a validation error; `false` for `PrimitiveError.None`. |
| `static PrimitiveError None` | Sentinel value indicating no error. Default value of the struct. |
| `static PrimitiveError Create(string code, string message)` | Factory method. |

## Exceptions

| Type | When Thrown | Base |
|:-----|:------------|:-----|
| `DomainPrimitiveValidationException` | `Create()` receives an invalid value | `ArgumentException` |
| `System.FormatException` | `Parse()` / `TryParse()` receives unparseable input (per rfc-0003) | `System.FormatException` |
| `DomainPrimitiveFormatException` | **Deprecated** `[Obsolete]` — use `System.FormatException` catch | `System.FormatException` |

> [!IMPORTANT]
> `DomainPrimitiveValidationException` inherits from `ArgumentException`. You can catch it with either `catch (DomainPrimitiveValidationException)` or `catch (ArgumentException)`. Access the structured error via the `.Error` property (`PrimitiveError`).

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
Marks a struct for generation of a date-backed primitive.

| `Kind` value | Backing type |
|:------------|:-------------|
| `DatePrimitiveKind.DateOnly` (default) | `System.DateOnly` |
| `DatePrimitiveKind.DateTime` | `System.DateTime` |
| `DatePrimitiveKind.DateTimeOffset` | `System.DateTimeOffset` |
| `DatePrimitiveKind.TimeOnly` | `System.TimeOnly` |

### `[StrongId<TValue>]`
Generates a strongly-typed identifier. Default `TValue` options: `Guid`, `int`, `long`, `string`.
```csharp
[StrongId<Guid>]
public readonly partial record struct OrderId;
```

### `[ValueObject]`
Marks a `readonly partial record struct` for generation of a multi-property value object via source generator.

### `[SmartEnum<TValue>]`
Generates an AOT-safe strongly-typed enum with source-generated `GetAll()`, `FromName()`, and `Match<TResult>()`.

## Normalization Attributes

Namespace: `EricksonLopez.DomainPrimitives` (root). `LowerCaseAttribute` and `NormalizeWhitespaceAttribute` are in `EricksonLopez.DomainPrimitives.Normalization`.

| Attribute | Namespace | Effect |
|:----------|:----------|:-------|
| `[Trim]` | Root | Trims leading/trailing whitespace before validation |
| `[TrimStart]` | Root | Trims leading whitespace only |
| `[TrimEnd]` | Root | Trims trailing whitespace only |
| `[LowerCase]` | `.Normalization` | Converts to lowercase invariant before validation |
| `[UpperCase]` | Root | Converts to uppercase invariant before validation |
| `[NormalizeWhitespace]` | `.Normalization` | Collapses internal whitespace runs to single space |
| `[Normalize<TNormalizer>]` | Root | Applies a custom `INormalizer<T>` implementation |

## Validation Constraint Attributes

Namespace: `EricksonLopez.DomainPrimitives`. Applied directly to string and numeric primitives to constrain the allowed value space.

| Attribute | Applies to | Description |
|:----------|:-----------|:------------|
| `[NotEmpty]` | String | Rejects empty or whitespace-only values. Error code: `"EMPTY"` |
| `[MinLength(n)]` | String | Value must have ≥ `n` characters (inclusive). Error code: `"LENGTH"` |
| `[MaxLength(n)]` | String | Value must have ≤ `n` characters (inclusive). Error code: `"LENGTH"` |
| `[Length(min, max)]` | String | Combined min+max length in a single attribute. Error code: `"LENGTH"` |
| `[ExactLength(n)]` | String | Value must have **exactly** `n` characters. Shorthand for `[Length(n,n)]`. Error code: `"LENGTH"` |
| `[Regex("pattern")]` | String | Value must match the regex pattern. Supports `AllowMultiple`. Error code: `"FORMAT"` |
| `[PrimitiveRange(min, max)]` | Numeric | Value must be within `[min, max]`. Accepts `double` or `(string, string)` overload for exact `decimal` precision. Error code: `"RANGE"` |

> [!NOTE]
> `[MaxLength(n)]` on a struct overrides the assembly-level `[DomainPrimitivesDefaults(MaxLength = 4096)]` default for that specific primitive.

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

| Attribute | Backing Type | Range / Notes |
|:----------|:------------|:--------------|
| `[Age]` | `int` | 0–150 |
| `[Money]` | `decimal` | ≥ 0 |
| `[Percentage]` | `decimal` | 0–100 |
| `[Price]` | `decimal` | ≥ 0 |
| `[TaxRate]` | `decimal` | 0–100 |
| `[Discount]` | `decimal` | 0–100 |
| `[Rating]` | `decimal` | 0–5 |
| `[Score]` | `int` | 0–100 (integer scale; use `[NumericPrimitive<decimal>]` for decimal scores) |
| `[Quantity]` | `int` | ≥ 0 |
| `[Latitude]` | `double` | -90 to 90 |
| `[Longitude]` | `double` | -180 to 180 |
| `[Weight]` | `double` | 0–1000 kg (SI) |
| `[Height]` | `double` | 0–300 cm |
| `[Distance]` | `double` | 0–`double.MaxValue` meters |
| `[Temperature]` | `double` | -273.15 (absolute zero) to `double.MaxValue` (Celsius) |

## Integration Extension Methods

### `EricksonLopez.DomainPrimitives.EFCore`

Auto-discovered via source generator. Registers all `ValueConverter<TDomain, TValue>` implementations automatically on `ModelBuilder`.

### `EricksonLopez.DomainPrimitives.Dapper`

Auto-discovered via source generator. Generates `SqlMapper.TypeHandler` implementations for all domain primitives. At application startup, call the generated registration method:

```csharp
// Generated in: EricksonLopez.DomainPrimitives.Dapper.Generated namespace
DapperDomainPrimitivesRegistration.RegisterAll();
```

This call is idempotent (safe to call multiple times). The generated class is emitted by `EricksonLopez.DomainPrimitives.Dapper.SourceGenerators` into your project at compile time.

### `EricksonLopez.DomainPrimitives.AspNetCore`

Auto-discovered via source generator. Generates `IModelBinder` implementations for route and query string binding. Register at startup using either:

```csharp
// Option A: IServiceCollection extension
builder.Services.AddDomainPrimitivesModelBinding();

// Option B: Fine-grained MvcOptions extension
builder.Services.AddControllers(options => options.AddDomainPrimitivesModelBinding());
```

## `PrimitiveBuilder<TPrimitive, TValue>`

Fluent builder for constructing domain primitives programmatically with ad-hoc validation rules.
Located in `EricksonLopez.DomainPrimitives.Advanced`.

```csharp
using EricksonLopez.DomainPrimitives.Advanced;

var promoCode = PrimitiveBuilder<VoucherCode, string>
    .For()                                       // Creates empty builder
    .WithValue("SUMMER2026")                      // Sets the value to build
    .Must(v => v.StartsWith("SUMMER"), "INVALID_SEASON", "Code must start with current season.")
    .BuildOrThrow();                              // throws DomainPrimitiveValidationException

// Non-throwing build:
bool ok = PrimitiveBuilder<VoucherCode, string>
    .For()
    .WithValue("WINTER2026")
    .Build(out var result);
```

| Method | Return | Description |
|:-------|:-------|:------------|
| `static For()` | `PrimitiveBuilder<T,V>` | Creates a new empty builder |
| `WithValue(TValue value)` | `PrimitiveBuilder<T,V>` | Sets the value to build |
| `Must(Func<TValue,bool>, code, msg)` | `PrimitiveBuilder<T,V>` | Adds a custom validation predicate |
| `Build(out TPrimitive)` | `bool` | Non-throwing build; returns false on failure |
| `BuildOrThrow()` | `TPrimitive` | Throwing build |
| `BuildResult()` | `object` | **⚠️ Deprecated** — use `BuildOrThrow()` or `Build()` instead. Will be removed in v3.0. |

> [!IMPORTANT]
> `PrimitiveBuilder<>` requires the type to be decorated with `[NumericPrimitive<T>]` or `[StringPrimitive]` (not shortcut attributes like `[Score]`) to satisfy the `IDomainPrimitive<TSelf, TValue>` constraint.

---

## `PrimitiveCollectionExtensions`

Bulk-convert raw value collections to typed domain primitive collections.

| Method | Overload | Description |
|:-------|:---------|:------------|
| `ToDomainPrimitiveList<T,V>()` | `IEnumerable<V>` | Converts to `List<T>`, throws on first invalid element |
| `ToDomainPrimitiveArray<T,V>()` | `IEnumerable<V>` | Converts to `T[]`, throws on first invalid element |
| `ToDomainPrimitiveArray<T,V>()` | `ReadOnlySpan<V>` | Zero-copy span path (NET 7+) |

---

## `[assembly: DomainPrimitivesDefaults]`

Assembly-level attribute that sets global defaults for all string primitives in an assembly.

```csharp
// Must appear after 'using' directives, before any top-level statements or type declarations
[assembly: DomainPrimitivesDefaults(Trim = true, NotEmpty = false, MaxLength = 4096)]
```

| Property | Type | Default | Description |
|:---------|:-----|:--------|:------------|
| `Trim` | `bool` | `false` | If true, auto-trims all string primitives in the assembly |
| `NotEmpty` | `bool` | `false` | If true, rejects empty strings for all string primitives |
| `MaxLength` | `int` | `4096` | Global maximum string length (SEC-001 security gate). Set to `0` to disable. |
| `ExceptionType` | `Type?` | `null` (uses `DomainPrimitiveValidationException`) | Custom exception type. Must have a public `(string message)` constructor (validated by DP0017). See [adr-034](../docs/adr/adr-034-configurable-exception-type.md). |

Individual `[MaxLength]`, `[NotEmpty]`, `[Trim]` attributes on a struct override these assembly defaults.

---

## `ValueObject` (Abstract Base Class)

Namespace: `EricksonLopez.DomainPrimitives`. Provides structural equality semantics for multi-property value objects via C# `record class` inheritance.

> [!IMPORTANT]
> **`ValueObject` (base class) vs `[ValueObject]` (attribute):** These are two distinct mechanisms:
> - **`ValueObject` base class** — inherit from this for multi-property value objects that live as reference types (e.g., `Money`, `Address`). Compiler generates value equality automatically.
> - **`[ValueObject]` attribute** — apply to `readonly partial record struct` for source-generated, allocation-free, AOT-safe value objects.

```csharp
// ✅ Using ValueObject base class (reference type, structural equality)
public sealed record Money(decimal Amount, string Currency) : ValueObject;

// ✅ Using [ValueObject] attribute (struct, source-generated, AOT-safe)
[ValueObject]
public readonly partial record struct Address;
```

---

## Diagnostics (`EricksonLopez.DomainPrimitives.Diagnostics`)

The Core package (`EricksonLopez.DomainPrimitives`) provides built-in observability hooks.

### `DomainPrimitivesMetrics`

OpenTelemetry `System.Diagnostics.Metrics.Meter`-based counters.

| Member | Signature | Description |
|:-------|:----------|:------------|
| `MeterName` | `static readonly string` | Name of the `Meter` (`"EricksonLopez.DomainPrimitives"`). |
| `IsEnabled` | `static bool IsEnabled { get; set; }` | Globally enables/disables metrics collection. Default: `true`. |
| `RecordCreation` | `static void RecordCreation(string primitiveName)` | Increments the `domain_primitive.creation` counter. |
| `RecordValidationSuccess` | `static void RecordValidationSuccess(string primitiveName)` | Increments the `domain_primitive.validation.success` counter. |
| `RecordValidationFailure` | `static void RecordValidationFailure(string primitiveName, string errorType, string errorMessage)` | Increments the `domain_primitive.validation.failure` counter. |

```csharp
// Register OpenTelemetry meter:
using var meterProvider = Sdk.CreateMeterProviderBuilder()
    .AddMeter(DomainPrimitivesMetrics.MeterName)
    .AddPrometheusExporter()
    .Build();
```

### `DomainPrimitivesDiagnostics`

`System.Diagnostics.DiagnosticListener`-based event source.

| Member | Description |
|:-------|:------------|
| `ListenerName` | `static readonly string` — Listener name for subscribing. |
| `Meter` | `static readonly Meter` — The shared `Meter` instance. |
| `WriteValidationSuccess(string)` | Writes a `ValidationSuccess` event. |
| `WriteValidationFailure(string, string, string)` | Writes a `ValidationFailure` event. |

### `DomainPrimitiveEventSource`

Static event source for consuming validation events without DI coupling.

| Member | Description |
|:-------|:------------|
| `OnValidationFailed` | `static event EventHandler<ValidationFailureEventArgs>?` — Subscribe to receive validation failure events. |

```csharp
// Subscribe at application startup:
DomainPrimitiveEventSource.OnValidationFailed += (_, e) =>
    logger.LogWarning("Validation failed: {Primitive} — [{Code}] {Message}",
        e.PrimitiveName, e.ErrorType, e.ErrorMessage);
```
