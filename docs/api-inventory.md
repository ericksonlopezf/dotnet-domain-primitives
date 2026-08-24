# Public API Inventory

This inventory was derived from `PublicAPI.Shipped.txt` and `PublicAPI.Unshipped.txt` across all source projects.

> **Legend:** ⚠️ = Deprecated with `[Obsolete(error: false)]`; will be removed in v3.0.

## Attributes — `EricksonLopez.DomainPrimitives` Namespace

### Primitive Type Attributes

| Attribute | Purpose | Notes |
|:----------|:--------|:------|
| `StringPrimitiveAttribute` | Marks a struct as a string-backed primitive | |
| `NumericPrimitiveAttribute<TValue>` | Marks a struct as a numeric-backed primitive | |
| `DatePrimitiveAttribute` | Marks a struct as a date-backed primitive | |
| `StrongIdAttribute<TValue>` | Marks a struct as a strongly-typed ID | |
| `ValueObjectAttribute` | Marks a struct as a multi-property value object | |
| `SmartEnumAttribute<TValue>` | Marks a struct as an AOT-safe smart enum | |

### Validation Attributes

| Attribute | Purpose |
|:----------|:--------|
| `NotEmptyAttribute` | Rejects empty or whitespace-only values |
| `LengthAttribute` | Validates exact length with `ErrorCode` and `ErrorMessage` properties |
| `MinLengthAttribute` | Validates minimum length |
| `MaxLengthAttribute` | Validates maximum length |
| `RangeAttribute` | Validates numeric or date range |
| `DomainRangeAttribute` | Domain-specific range constraint |
| `PrimitiveRangeAttribute` | Range constraint for primitive types |
| `DateRangeAttribute` | Validates date within a range |
| `RegexAttribute` | Applies a custom regex pattern |

### Normalization Attributes — `EricksonLopez.DomainPrimitives.Normalization`

| Attribute | Effect |
|:----------|:-------|
| `TrimAttribute` | Trims whitespace |
| `LowerCaseAttribute` | Converts to lowercase invariant |
| `UpperCaseAttribute` | Converts to uppercase invariant |
| `NormalizeWhitespaceAttribute` | Collapses internal whitespace |
| `NormalizeAttribute<TNormalizer>` | Applies a custom `INormalizer<T>` |

### String Semantic Shortcuts

| Attribute | Category |
|:----------|:---------|
| `EmailAttribute` | Identity |
| `UsernameAttribute` | Identity |
| `PasswordHashAttribute` | Identity |
| `UrlAttribute` | Network |
| `IPAddressAttribute` | Network |
| `MacAddressAttribute` | Network |
| `PhoneAttribute` | Commerce |
| `CountryCodeAttribute` | Commerce |
| `CurrencyCodeAttribute` | Commerce |
| `LanguageCodeAttribute` | Commerce |
| `IBANAttribute` | Commerce |
| `SlugAttribute` | Content |
| `HexColorAttribute` | Content |
| `ISBNAttribute` | Content |
| `VINAttribute` | Content |

### Numeric Semantic Shortcuts

| Attribute | Category |
|:----------|:---------|
| `AgeAttribute` | Person |
| `MoneyAttribute` | Finance |
| `PercentageAttribute` | Finance |
| `PriceAttribute` | Finance |
| `TaxRateAttribute` | Finance |
| `DiscountAttribute` | Finance |
| `RatingAttribute` | Measurement |
| `ScoreAttribute` | Measurement |
| `QuantityAttribute` | Measurement |
| `WeightAttribute` | Measurement |
| `HeightAttribute` | Measurement |
| `DistanceAttribute` | Measurement |
| `TemperatureAttribute` | Measurement |
| `LatitudeAttribute` | Geo |
| `LongitudeAttribute` | Geo |

### Date Semantic Shortcuts

| Attribute | Purpose |
|:----------|:--------|
| `BirthDateAttribute` | Validates date is in the past |
| `ExpirationDateAttribute` | Validates date is in the future |
| `BusinessDateAttribute` | Rejects weekends |
| `FiscalYearAttribute` | Fiscal year date |
| `MonthAttribute` | Month value (1–12) |
| `QuarterAttribute` | Quarter value (1–4) |
| `WeekAttribute` | ISO week number |
| `TimeRangeAttribute` | Time range validation |

### Deprecated Attributes ⚠️

| Attribute | Status | Replacement |
|:----------|:-------|:------------|
| `FluentValidationAttribute` | `[Obsolete]` — package removed per rfc-0004 | Use `TryCreate` at application boundary |
| `EFCoreAttribute` | `[Obsolete]` — auto-discovery replaces per-type attribute | Remove attribute; install EFCore package |
| `DapperAttribute` | `[Obsolete]` — auto-discovery replaces per-type attribute | Remove attribute; install Dapper package |
| `AspNetCoreAttribute` | `[Obsolete]` — auto-discovery replaces per-type attribute | Remove attribute; install AspNetCore package |
| `OpenApiAttribute` | `[Obsolete]` — auto-discovery replaces per-type attribute | Remove attribute; install OpenApi package |
| `MapsterAttribute` | `[Obsolete]` — auto-discovery replaces per-type attribute | Remove attribute; install Mapster package |
| `JsonAttribute` | `[Obsolete]` — STJ converters are now inline-generated (adr-011) | No replacement needed; converters auto-generated |

## Interfaces — `EricksonLopez.DomainPrimitives` Namespace

| Interface | Purpose |
|:----------|:--------|
| `IDomainPrimitive<TSelf, TValue>` | Base contract for all domain primitives |
| `IDomainPrimitive<TSelf>` | Base contract for primitives with single implicit value |
| `IStrongId<TSelf, TValue>` | Contract for strongly-typed identifiers |
| `INormalizer<T>` | Contract for custom normalizers |

## Exceptions

| Type | Assembly | Namespace | Notes |
|:-----|:---------|:----------|:------|
| `DomainPrimitiveValidationException` | Core | `EricksonLopez.DomainPrimitives` | Thrown by `Create()`. Inherits `ArgumentException`. Access `.Error` (`PrimitiveError`) for structured error info. |
| `DomainPrimitiveFormatException` | Core | `EricksonLopez.DomainPrimitives` | ⚠️ Deprecated — `Parse()` now throws `System.FormatException` per rfc-0003 |

## Enums

| Type | Values | Notes |
|:-----|:-------|:------|
| `NumericOperations` | `None`, `Addition`, `Subtraction`, `ScalarMultiplication`, `ScalarDivision`, `Negation`, `Additive`, `Multiplicative`, `All` | Bitflags for generated arithmetic operators |
| `ArithmeticPolicy` | ⚠️ Deprecated alias for `NumericOperations` | Per AUDIT-CRIT-004 |
| `DatePrimitiveKind` | `DateOnly`, `DateTime`, `DateTimeOffset`, `TimeOnly` | Backing type selector for `[DatePrimitive]` |

## Diagnostics — `EricksonLopez.DomainPrimitives.Diagnostics` Namespace

> **Note:** These types are provided by `EricksonLopez.DomainPrimitives` (Core package) since v1.0.0.

| Type | Purpose |
|:-----|:--------|
| `DomainPrimitivesMetrics` | `System.Diagnostics.Metrics.Meter`-based metrics |
| `DomainPrimitivesDiagnostics` | `DiagnosticSource`-based diagnostics |
| `DomainPrimitiveEventSource` | `EventSource`-based events |
| `ValidationFailurePayload` | Diagnostic payload for validation failures |
| `ValidationSuccessPayload` | Diagnostic payload for validation successes |
| `ValidationFailureEventArgs` | EventArgs for validation failure events |

## Validation — `EricksonLopez.DomainPrimitives.Validation` Namespace

| Type | Purpose |
|:-----|:--------|
| `ICustomValidator<T>` | Contract for custom validators used with `[CustomValidator<T>]` |
| `CustomValidatorAttribute<TValidator>` | Applies a custom `ICustomValidator<T>` to a primitive |
| `PrimitiveError` | Zero-allocation `readonly record struct` representing a single validation error. Contains `Code`, `Message`, `IsError`, `None`, and `Create()` members. |

## Testing — `EricksonLopez.DomainPrimitives.Testing` Namespace

| Type | Purpose |
|:-----|:--------|
| `DomainPrimitiveAssertionsExtensions` | AwesomeAssertions extensions for domain primitive assertions |
| `DomainPrimitiveFakeFactory` | Generates valid fakes for domain primitives |
| `DomainPrimitiveScenarios` | Pre-built test scenario collections |
| `DomainPrimitiveTestBuilder` | Fluent test data builder |
| `DomainPrimitiveVerifyExtensions` | Verify.Xunit extensions for snapshot testing |

## Utilities

| Type | Namespace | Purpose |
|:-----|:----------|:--------|
| `PrimitiveBuilder<TPrimitive, TValue>` | `EricksonLopez.DomainPrimitives.Advanced` | Fluent builder for test/sample construction |
| `PrimitiveCollectionExtensions` | `EricksonLopez.DomainPrimitives` | LINQ extension methods for collections of domain primitives |
| `PrimitiveError` | `EricksonLopez.DomainPrimitives.Validation` | Zero-allocation error struct for `TryCreate` out parameter |
