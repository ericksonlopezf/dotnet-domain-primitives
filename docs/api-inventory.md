# Public API Inventory — EricksonLopez.DomainPrimitives Ecosystem

This inventory is the **single authoritative source of truth** for the public API surface of the `EricksonLopez.DomainPrimitives` repository. It covers all types, interfaces, attributes, structs, extension methods, diagnostics, and testing utilities across Core Library and Infrastructure packages.

Every table follows the strict 7-column schema:
`| Name | Namespace | Responsibility | Dependencies | Use Cases | Complexity Level | Existing Example |`

> **Complexity Levels:**
> - **Basic**: Declarative primitive attributes, strong IDs, basic validation, and default serialization.
> - **Intermediate**: Normalization pipelines, collection extensions, EF Core/Dapper persistence, and ASP.NET Core model binding.
> - **Advanced**: Custom validators (`ICustomValidator<T>`), custom normalizers (`INormalizer<T>`), `PrimitiveBuilder<T, V>`, diagnostics listeners, and testing harness utilities.

---

## 1. Core Interfaces & Marker Contracts

| Name | Namespace | Responsibility | Dependencies | Use Cases | Complexity Level | Existing Example |
|---|---|---|---|---|---|---|
| `IDomainPrimitive<TSelf, TValue>` | `EricksonLopez.DomainPrimitives` | Defines base contract for strongly-typed domain primitives with value encapsulation and factory methods (`Value`, `Create`, `TryCreate`). | .NET BCL | Foundation of any struct decorated as a domain primitive. | Basic | Yes (`01-GettingStarted`, `14-Performance`) |
| `IDomainPrimitive<TSelf>` | `EricksonLopez.DomainPrimitives` | Contract for domain primitive metadata (`IsDefault`, `PrimitiveName`). | .NET BCL | Default state checks and diagnostic logging. | Basic | Yes (`14-Performance`, `18-Observability`) |
| `IStrongId<TSelf, TValue>` | `EricksonLopez.DomainPrimitives` | Specialized contract for strongly-typed identifiers (`Create()`, `Create(TValue)`, `Empty`). | `IDomainPrimitive<TSelf, TValue>` | Entity and aggregate root identifiers in DDD. | Basic | Yes (`01-GettingStarted`, `05-StronglyTypedIds`) |
| `INormalizer<T>` | `EricksonLopez.DomainPrimitives` | Contract for deterministic normalizers applied prior to invariant validation. | .NET BCL | Sanitization and canonical formatting of strings, numbers, or dates before primitive instantiation. | Advanced | Yes (`22-CustomImplementations`) |
| `ICustomValidator<T>` | `EricksonLopez.DomainPrimitives.Validation` | Contract for domain validators with decoupled complex logic. | `PrimitiveError` | Business rule validation requiring specialized algorithms (e.g. Luhn algorithm, check digits). | Advanced | Yes (`22-CustomImplementations`) |
| `ValueObject` | `EricksonLopez.DomainPrimitives` | Abstract base class for composite Value Objects. | .NET BCL | Multi-property value object hierarchies without source generator. | Intermediate | Yes (`04-ValueObjects`) |

---

## 2. Error Models and Exceptions

| Name | Namespace | Responsibility | Dependencies | Use Cases | Complexity Level | Existing Example |
|---|---|---|---|---|---|---|
| `PrimitiveError` | `EricksonLopez.DomainPrimitives.Validation` | Immutable, zero-allocation error struct returned by `TryCreate`. Contains `Code`, `Message`, `IsError`, and sentinel constants (`None`). | .NET BCL | Error handling in high-throughput critical paths and Railway-Oriented Programming. | Basic | Yes (`01-GettingStarted`, `02-FirstResult`, `03-Errors`) |
| `DomainPrimitiveValidationException` | `EricksonLopez.DomainPrimitives` | Strongly-typed exception thrown by `Create()` when invariant validation fails. Inherits from `ArgumentException` and exposes structured `Error`. | `PrimitiveError`, `ArgumentException` | Validation failures in trusted constructors and entity factories. | Basic | Yes (`03-Errors`, `19-UnitTesting`) |

---

## 3. Primitive Generation Attributes

| Name | Namespace | Responsibility | Dependencies | Use Cases | Complexity Level | Existing Example |
|---|---|---|---|---|---|---|
| `StringPrimitiveAttribute` | `EricksonLopez.DomainPrimitives` | Decorates a `readonly partial record struct` to generate a string-backed primitive with parsing, operators, and serialization. | Source Generators | Text-based domain primitives with custom validation and normalization rules. | Basic | Yes (`01-GettingStarted`, `04-ValueObjects`) |
| `NumericPrimitiveAttribute<TValue>` | `EricksonLopez.DomainPrimitives` | Decorates a struct to generate a numeric primitive (`int`, `decimal`, `double`, etc.) with arithmetic operators and range checks. | Source Generators | Quantities, monetary amounts, measurements, and percentages. | Basic | Yes (`04-ValueObjects`, `14-Performance`) |
| `DatePrimitiveAttribute` | `EricksonLopez.DomainPrimitives` | Decorates a struct to generate a temporal primitive (`DateOnly`, `DateTime`, `DateTimeOffset`, `TimeOnly`). | Source Generators | Birth dates, expiration dates, timestamps, and audit records. | Basic | Yes (`04-ValueObjects`, `06-EntitiesAndAggregates`) |
| `StrongIdAttribute<TValue>` | `EricksonLopez.DomainPrimitives` | Generates a strongly-typed identifier with automated ID factory methods (`New()`, `Guid.NewGuid()`). | Source Generators | Unique aggregate and entity identifiers (`CustomerId`, `OrderId`). | Basic | Yes (`01-GettingStarted`, `05-StronglyTypedIds`) |
| `ValueObjectAttribute` | `EricksonLopez.DomainPrimitives` | Decorates a struct to generate a multi-property Value Object with value equality and BCL interface implementations. | Source Generators | Composite value objects (e.g. `Address`, `MoneyAmount`). | Intermediate | Yes (`04-ValueObjects`) |
| `SmartEnumAttribute<TValue>` | `EricksonLopez.DomainPrimitives` | Generates a NativeAOT-safe Smart Enum with static enumeration (`All`), name/value lookups, and pattern matching (`Match`). | Source Generators | Enriched domain enumerations with behavior and domain attributes. | Intermediate | Yes (`11-SmartEnums`) |
| `DomainPrimitivesDefaultsAttribute` | `EricksonLopez.DomainPrimitives` | Assembly-level attribute to configure global default invariants (`Trim`, `NotEmpty`, `MaxLength`, custom `ExceptionType`). | Source Generators | Enforcing organizational standards across an entire project. | Intermediate | Yes (`09-SourceGenerators`) |

---

## 4. Constraint and Validation Attributes

| Name | Namespace | Responsibility | Dependencies | Use Cases | Complexity Level | Existing Example |
|---|---|---|---|---|---|---|
| `NotEmptyAttribute` | `EricksonLopez.DomainPrimitives` | Rejects null, empty, or whitespace-only values. Error code: `EMPTY`. | Source Generators | Strings and identifiers that must mandatorily contain data. | Basic | Yes (`01-GettingStarted`, `03-Errors`) |
| `LengthAttribute` | `EricksonLopez.DomainPrimitives` | Validates string length within an exact interval `[min, max]`. Error code: `LENGTH`. | Source Generators | Postal codes, document identifiers, bounded strings. | Basic | Yes (`03-Errors`, `04-ValueObjects`) |
| `ExactLengthAttribute` | `EricksonLopez.DomainPrimitives` | Shortcut for `Length(n, n)` requiring exact character length. Error code: `LENGTH`. | Source Generators | Fixed-length tokens, hash digests, ISO codes. | Basic | Yes (`04-ValueObjects`) |
| `MinLengthAttribute` | `EricksonLopez.DomainPrimitives` | Enforces minimum string length. Error code: `LENGTH`. | Source Generators | Usernames, minimum description bounds, passwords. | Basic | Yes (`03-Errors`) |
| `MaxLengthAttribute` | `EricksonLopez.DomainPrimitives` | Enforces maximum string length. Error code: `LENGTH`. | Source Generators | Database column-bounded strings. | Basic | Yes (`03-Errors`) |
| `PrimitiveRangeAttribute` | `EricksonLopez.DomainPrimitives` | Constrains numeric value to an inclusive range `[min, max]`. Supports doubles or strings for decimal precision. Error: `RANGE`. | Source Generators | Prices, percentages, ages, physical measurements. | Basic | Yes (`03-Errors`, `04-ValueObjects`) |
| `DateRangeAttribute` | `EricksonLopez.DomainPrimitives` | Validates date falls within a specific temporal range. Error code: `RANGE`. | Source Generators | Fiscal periods, contractual validity dates. | Intermediate | Yes (`04-ValueObjects`) |
| `RegexAttribute` | `EricksonLopez.DomainPrimitives` | Applies a NativeAOT-compatible compiled regular expression. Error code: `FORMAT`. | Source Generators | Formatted identifiers, serial numbers, customs codes. | Intermediate | Yes (`03-Errors`, `04-ValueObjects`) |
| `CustomValidatorAttribute<TValidator>` | `EricksonLopez.DomainPrimitives.Validation` | Binds an `ICustomValidator<T>` implementation to the primitive. | `ICustomValidator<T>` | Algorithmic domain validations exceeding declarative attributes. | Advanced | Yes (`22-CustomImplementations`) |

---

## 5. Normalization Attributes

| Name | Namespace | Responsibility | Dependencies | Use Cases | Complexity Level | Existing Example |
|---|---|---|---|---|---|---|
| `TrimAttribute` | `EricksonLopez.DomainPrimitives` | Strips leading and trailing whitespace prior to invariant validation. | Source Generators | Standard user input sanitization. | Basic | Yes (`01-GettingStarted`, `04-ValueObjects`) |
| `TrimStartAttribute` | `EricksonLopez.DomainPrimitives` | Strips leading whitespace prior to validation. | Source Generators | Strings where trailing whitespace is domain-significant. | Intermediate | Yes (`04-ValueObjects`) |
| `TrimEndAttribute` | `EricksonLopez.DomainPrimitives` | Strips trailing whitespace prior to validation. | Source Generators | Formatted lines where indentation is domain-significant. | Intermediate | Yes (`04-ValueObjects`) |
| `LowerCaseAttribute` | `EricksonLopez.DomainPrimitives` | Converts string to invariant lowercase prior to validation. | Source Generators | Normalizing emails, slugs, and usernames. | Basic | Yes (`01-GettingStarted`, `04-ValueObjects`) |
| `UpperCaseAttribute` | `EricksonLopez.DomainPrimitives` | Converts string to invariant uppercase prior to validation. | Source Generators | ISO country codes, currency codes, vehicle registrations. | Basic | Yes (`04-ValueObjects`) |
| `NormalizeWhitespaceAttribute` | `EricksonLopez.DomainPrimitives` | Collapses consecutive internal spaces into a single space. | Source Generators | Full names, addresses, titles. | Intermediate | Yes (`04-ValueObjects`) |
| `NormalizeAttribute<TNormalizer>` | `EricksonLopez.DomainPrimitives` | Applies a custom normalizer implementing `INormalizer<T>`. | `INormalizer<T>` | Diacritic removal, Unicode NFKC canonical form, custom sanitization. | Advanced | Yes (`22-CustomImplementations`) |

---

## 6. Built-in String Semantic Shortcuts (15 Attributes)

| Name | Namespace | Responsibility | Dependencies | Use Cases | Complexity Level | Existing Example |
|---|---|---|---|---|---|---|
| `EmailAttribute` | `EricksonLopez.DomainPrimitives` | Shortcut: `[StringPrimitive]`, `[Trim]`, `[LowerCase]`, RFC 5321/5322 validation. | Source Generators | Customer and user email addresses. | Basic | Yes (`01-GettingStarted`, `20-EndToEndApplication`) |
| `PhoneAttribute` | `EricksonLopez.DomainPrimitives` | Shortcut: `[StringPrimitive]`, `[Trim]`, E.164 international telephone format. | Source Generators | Mobile and contact phone numbers. | Basic | Yes (`04-ValueObjects`, `20-EndToEndApplication`) |
| `UrlAttribute` | `EricksonLopez.DomainPrimitives` | Shortcut: `[StringPrimitive]`, `[Trim]`, valid absolute HTTP/HTTPS URI. | Source Generators | Website links, webhook URLs, avatar images. | Basic | Yes (`04-ValueObjects`) |
| `SlugAttribute` | `EricksonLopez.DomainPrimitives` | Shortcut: `[StringPrimitive]`, `[Trim]`, `[LowerCase]`, alphanumeric URL slug with hyphens. | Source Generators | SEO-friendly slugs and blog post identifiers. | Basic | Yes (`04-ValueObjects`) |
| `UsernameAttribute` | `EricksonLopez.DomainPrimitives` | Shortcut: `[StringPrimitive]`, `[Trim]`, `[LowerCase]`, alphanumeric username. | Source Generators | Usernames and social media handles. | Basic | Yes (`04-ValueObjects`) |
| `PasswordHashAttribute` | `EricksonLopez.DomainPrimitives` | Shortcut: `[StringPrimitive]`, `[NotEmpty]`, PII protection (omitted from logs). | Source Generators | Cryptographic password hashes (BCrypt, Argon2). | Basic | Yes (`04-ValueObjects`) |
| `CountryCodeAttribute` | `EricksonLopez.DomainPrimitives` | Shortcut: `[StringPrimitive]`, `[Trim]`, `[UpperCase]`, ISO 3166-1 alpha-2/3 country code. | Source Generators | International billing and physical addresses. | Basic | Yes (`04-ValueObjects`) |
| `LanguageCodeAttribute` | `EricksonLopez.DomainPrimitives` | Shortcut: `[StringPrimitive]`, `[Trim]`, `[LowerCase]`, BCP 47 / ISO 639-1 language code. | Source Generators | Localization preferences and content targeting. | Basic | Yes (`04-ValueObjects`) |
| `CurrencyCodeAttribute` | `EricksonLopez.DomainPrimitives` | Shortcut: `[StringPrimitive]`, `[Trim]`, `[UpperCase]`, ISO 4217 currency code (USD, EUR). | Source Generators | Financial transactions and bank accounts. | Basic | Yes (`04-ValueObjects`) |
| `IBANAttribute` | `EricksonLopez.DomainPrimitives` | Shortcut: `[StringPrimitive]`, `[Trim]`, `[UpperCase]`, International Bank Account Number. | Source Generators | European and international bank accounts. | Intermediate | Yes (`04-ValueObjects`) |
| `ISBNAttribute` | `EricksonLopez.DomainPrimitives` | Shortcut: `[StringPrimitive]`, `[Trim]`, ISBN-10 or ISBN-13 book identifier. | Source Generators | Publishing and bibliographic catalogs. | Intermediate | Yes (`04-ValueObjects`) |
| `VINAttribute` | `EricksonLopez.DomainPrimitives` | Shortcut: `[StringPrimitive]`, `[Trim]`, `[UpperCase]`, 17-character Vehicle Identification Number. | Source Generators | Automotive and fleet management. | Intermediate | Yes (`04-ValueObjects`) |
| `IPAddressAttribute` | `EricksonLopez.DomainPrimitives` | Shortcut: `[StringPrimitive]`, `[Trim]`, valid IPv4 or IPv6 network address. | Source Generators | Security audit trails and network access control. | Basic | Yes (`04-ValueObjects`) |
| `MacAddressAttribute` | `EricksonLopez.DomainPrimitives` | Shortcut: `[StringPrimitive]`, `[Trim]`, hardware MAC network address. | Source Generators | Hardware device tracking and IoT endpoints. | Intermediate | Yes (`04-ValueObjects`) |
| `HexColorAttribute` | `EricksonLopez.DomainPrimitives` | Shortcut: `[StringPrimitive]`, `[Trim]`, `[UpperCase]`, web hex color (`#FFF`, `#FFFFFF`). | Source Generators | Visual themes and design system properties. | Basic | Yes (`04-ValueObjects`) |

---

## 7. Built-in Numeric Semantic Shortcuts (15 Attributes)

| Name | Namespace | Responsibility | Dependencies | Use Cases | Complexity Level | Existing Example |
|---|---|---|---|---|---|---|
| `MoneyAttribute` | `EricksonLopez.DomainPrimitives` | Numeric shortcut: non-negative decimal (`>= 0`). | Source Generators | Account balances, financial amounts, payouts. | Basic | Yes (`04-ValueObjects`, `20-EndToEndApplication`) |
| `PriceAttribute` | `EricksonLopez.DomainPrimitives` | Numeric shortcut: strictly positive decimal (`> 0`). | Source Generators | Product catalog retail prices. | Basic | Yes (`04-ValueObjects`, `16-EFCoreIntegration`) |
| `TaxRateAttribute` | `EricksonLopez.DomainPrimitives` | Numeric shortcut: tax ratio decimal (`0.0` to `1.0`). | Source Generators | VAT and sales tax computations. | Intermediate | Yes (`04-ValueObjects`) |
| `DiscountAttribute` | `EricksonLopez.DomainPrimitives` | Numeric shortcut: discount percentage decimal (`0` to `100`). | Source Generators | Promotional discounts and markdowns. | Basic | Yes (`04-ValueObjects`) |
| `PercentageAttribute` | `EricksonLopez.DomainPrimitives` | Numeric shortcut: percentage double (`0.0` to `100.0`). | Source Generators | Task completion progress, market shares. | Basic | Yes (`04-ValueObjects`) |
| `RatingAttribute` | `EricksonLopez.DomainPrimitives` | Numeric shortcut: rating score (e.g. 1 to 5 scale). | Source Generators | User reviews and customer feedback. | Basic | Yes (`04-ValueObjects`) |
| `ScoreAttribute` | `EricksonLopez.DomainPrimitives` | Numeric shortcut: non-negative gaming or metric score. | Source Generators | Gamification, leaderboard points. | Basic | Yes (`04-ValueObjects`) |
| `QuantityAttribute` | `EricksonLopez.DomainPrimitives` | Numeric shortcut: strictly positive integer (`> 0`). | Source Generators | Order line item units, inventory counts. | Basic | Yes (`04-ValueObjects`, `20-EndToEndApplication`) |
| `WeightAttribute` | `EricksonLopez.DomainPrimitives` | Numeric shortcut: strictly positive physical weight. | Source Generators | Shipping logistics, package handling. | Basic | Yes (`04-ValueObjects`) |
| `HeightAttribute` | `EricksonLopez.DomainPrimitives` | Numeric shortcut: strictly positive physical height. | Source Generators | Dimension specifications, biometric profiles. | Basic | Yes (`04-ValueObjects`) |
| `DistanceAttribute` | `EricksonLopez.DomainPrimitives` | Numeric shortcut: non-negative physical distance. | Source Generators | Delivery route calculations, telemetry. | Basic | Yes (`04-ValueObjects`) |
| `TemperatureAttribute` | `EricksonLopez.DomainPrimitives` | Numeric shortcut: physical temperature (above absolute zero). | Source Generators | IoT sensors, cold chain monitoring. | Intermediate | Yes (`04-ValueObjects`) |
| `LatitudeAttribute` | `EricksonLopez.DomainPrimitives` | Numeric shortcut: geographic latitude coordinate (`-90.0` to `+90.0`). | Source Generators | GPS mapping and location services. | Basic | Yes (`04-ValueObjects`) |
| `LongitudeAttribute` | `EricksonLopez.DomainPrimitives` | Numeric shortcut: geographic longitude coordinate (`-180.0` to `+180.0`). | Source Generators | GPS mapping and geographic fencing. | Basic | Yes (`04-ValueObjects`) |
| `AgeAttribute` | `EricksonLopez.DomainPrimitives` | Numeric shortcut: human age in completed years (`0` to `150`). | Source Generators | User profile validation and age eligibility checks. | Basic | Yes (`04-ValueObjects`) |

---

## 8. Built-in Temporal Semantic Shortcuts (8 Attributes)

| Name | Namespace | Responsibility | Dependencies | Use Cases | Complexity Level | Existing Example |
|---|---|---|---|---|---|---|
| `BirthDateAttribute` | `EricksonLopez.DomainPrimitives` | Date primitive: validates date is in the past with configurable max age. | Source Generators | Human birth dates and historical events. | Basic | Yes (`04-ValueObjects`) |
| `ExpirationDateAttribute` | `EricksonLopez.DomainPrimitives` | Date primitive: validates date is in the future. | Source Generators | Credit card expiration, pharmaceutical lot validity. | Basic | Yes (`04-ValueObjects`) |
| `BusinessDateAttribute` | `EricksonLopez.DomainPrimitives` | Date primitive: rejects weekend days (Saturdays and Sundays). | Source Generators | Bank settlement days and trading windows. | Intermediate | Yes (`04-ValueObjects`) |
| `FiscalYearAttribute` | `EricksonLopez.DomainPrimitives` | Date primitive: represents a specific fiscal year. | Source Generators | Corporate accounting and tax audits. | Intermediate | Yes (`04-ValueObjects`) |
| `MonthAttribute` | `EricksonLopez.DomainPrimitives` | Numeric primitive: validates calendar months (`1` to `12`). | Source Generators | Monthly reporting and calendar scheduling. | Basic | Yes (`04-ValueObjects`) |
| `QuarterAttribute` | `EricksonLopez.DomainPrimitives` | Numeric primitive: validates financial quarters (`1` to `4`). | Source Generators | Quarterly earnings reports (Q1–Q4). | Basic | Yes (`04-ValueObjects`) |
| `WeekAttribute` | `EricksonLopez.DomainPrimitives` | Numeric primitive: validates ISO week numbers (`1` to `53`). | Source Generators | Production planning and payroll work weeks. | Basic | Yes (`04-ValueObjects`) |
| `TimeRangeAttribute` | `EricksonLopez.DomainPrimitives` | Temporal primitive: restricts time of day to an operating window. | Source Generators | Delivery slots and store operating hours. | Intermediate | Yes (`04-ValueObjects`) |

---

## 9. Enums and Options

| Name | Namespace | Responsibility | Dependencies | Use Cases | Complexity Level | Existing Example |
|---|---|---|---|---|---|---|
| `DatePrimitiveKind` | `EricksonLopez.DomainPrimitives` | Underlying storage kind for `[DatePrimitive]` (`DateOnly`, `DateTime`, `DateTimeOffset`, `TimeOnly`). | .NET BCL | Selecting temporal resolution. | Basic | Yes (`04-ValueObjects`) |
| `NumericOperations` | `EricksonLopez.DomainPrimitives` | Bitwise flags to selectively enable overloaded arithmetic operators (`Addition`, `Subtraction`, `ScalarMultiplication`, etc.). | .NET BCL | Precise control over which math operations a numeric primitive exposes. | Intermediate | Yes (`14-Performance`) |

---

## 10. Fluent Utilities and Collections

| Name | Namespace | Responsibility | Dependencies | Use Cases | Complexity Level | Existing Example |
|---|---|---|---|---|---|---|
| `PrimitiveBuilder<TPrimitive, TValue>` | `EricksonLopez.DomainPrimitives.Advanced` | Fluent builder to construct and validate primitives programmatically (`For()`, `WithValue()`, `Must()`, `BuildOrThrow()`, `Build()`). | `IDomainPrimitive<T, V>` | Dynamic validation and incremental construction in tests and factories. | Advanced | Yes (`22-CustomImplementations`) |
| `PrimitiveCollectionExtensions` | `EricksonLopez.DomainPrimitives` | LINQ and Span extensions (`ToDomainPrimitiveList`, `ToDomainPrimitiveArray`) with overloads for `IEnumerable<T>` and `ReadOnlySpan<T>`. | .NET BCL | Bulk mapping raw scalar sequences into validated domain primitives. | Intermediate | Yes (`13-DomainCollections`) |

---

## 11. Diagnostics and Observability

| Name | Namespace | Responsibility | Dependencies | Use Cases | Complexity Level | Existing Example |
|---|---|---|---|---|---|---|
| `DomainPrimitivesDiagnostics` | `EricksonLopez.DomainPrimitives.Diagnostics` | Exposes `DiagnosticListener` (`Source`) and OpenTelemetry `Meter` for creation and failure metrics. | `System.Diagnostics` | OpenTelemetry and Application Insights integration. | Advanced | Yes (`18-Observability`) |
| `DomainPrimitivesMetrics` | `EricksonLopez.DomainPrimitives.Diagnostics` | High-performance OpenTelemetry counters (`validation.success`, `validation.failure`, `creation`). | `System.Diagnostics.Metrics` | Prometheus/Grafana domain health dashboards. | Advanced | Yes (`18-Observability`) |
| `DomainPrimitiveEventSource` | `EricksonLopez.DomainPrimitives.Diagnostics` | Static event source (`OnValidationFailed`) for logging failures without DI container requirements. | .NET BCL | Routing validation anomalies to loggers or audit trails. | Intermediate | Yes (`18-Observability`) |
| `ValidationFailureEventArgs` | `EricksonLopez.DomainPrimitives.Diagnostics` | Event arguments struct for `OnValidationFailed` (`PrimitiveName`, `ErrorType`, `ErrorMessage`). | .NET BCL | Diagnostic event payload data. | Basic | Yes (`18-Observability`) |
| `ValidationFailurePayload` | `EricksonLopez.DomainPrimitives.Diagnostics` | Record struct payload for failure events emitted via `DiagnosticSource`. | .NET BCL | Low-level telemetry subscribers. | Advanced | Yes (`18-Observability`) |
| `ValidationSuccessPayload` | `EricksonLopez.DomainPrimitives.Diagnostics` | Record struct payload for success events emitted via `DiagnosticSource`. | .NET BCL | Success auditing. | Advanced | Yes (`18-Observability`) |

---

## 12. Infrastructure: ASP.NET Core (`EricksonLopez.DomainPrimitives.AspNetCore`)

| Name | Namespace | Responsibility | Dependencies | Use Cases | Complexity Level | Existing Example |
|---|---|---|---|---|---|---|
| `DomainPrimitivesMvcBuilderExtensions` | `EricksonLopez.DomainPrimitives.AspNetCore` | `AddDomainPrimitivesModelBinding` extensions on `MvcOptions` and `IServiceCollection`. | ASP.NET Core MVC | Automated model binder registration across Controllers and Minimal APIs. | Basic | Yes (`15-AspNetCoreIntegration`, `20-EndToEndApplication`) |
| `DomainPrimitiveModelBinder<T>` | `EricksonLopez.DomainPrimitives.AspNetCore` | `IModelBinder` deserializing primitives from route parameters, query strings, and form values via `Parse`/`Create`. | ASP.NET Core MVC | HTTP parameter binding (`[FromRoute]`, `[FromQuery]`). | Intermediate | Yes (`15-AspNetCoreIntegration`) |
| `DomainPrimitiveValidator` | `EricksonLopez.DomainPrimitives.AspNetCore` | Static helper integrating primitive validation with `IValidatableObject` and `ValidationResult`. | `System.ComponentModel.DataAnnotations` | Complex DTO validation integration. | Intermediate | Yes (`15-AspNetCoreIntegration`) |
| `DomainPrimitiveValidationAttribute<TPrimitive, TValue>` | `EricksonLopez.DomainPrimitives.AspNetCore` | Declarative `ValidationAttribute` to validate raw DTO properties against primitive invariants. | `System.ComponentModel.DataAnnotations` | Declarative DTO validation in controllers and gRPC/WCF contracts. | Intermediate | Yes (`15-AspNetCoreIntegration`) |

---

## 13. Infrastructure: Persistence (EF Core and Dapper)

| Name | Namespace | Responsibility | Dependencies | Use Cases | Complexity Level | Existing Example |
|---|---|---|---|---|---|---|
| `DomainPrimitivesEFCoreExtensions` | `EricksonLopez.DomainPrimitives.EFCore.Generated` | Extension `ConfigureDomainPrimitives(this ModelConfigurationBuilder)` auto-mapping all generated `ValueConverter<T, V>`. | EF Core | Zero-code convention configuration in `DbContext.ConfigureConventions`. | Basic | Yes (`16-EFCoreIntegration`, `20-EndToEndApplication`) |
| `{Primitive}ValueConverter` | `EricksonLopez.DomainPrimitives.EFCore.Generated` | Generated sealed class inheriting `ValueConverter<T, BackingType>` for database persistence. | EF Core | Two-way mapping between domain primitive and relational column. | Intermediate | Yes (`16-EFCoreIntegration`) |
| `DomainPrimitiveTypeHandler<TPrimitive, TValue>` | `EricksonLopez.DomainPrimitives.Dapper` | Generic `SqlMapper.TypeHandler<T>` for column serialization and parsing in Dapper. | Dapper | Manual or fallback mapping in micro-ORMs. | Intermediate | Yes (`23-DapperIntegration`) |
| `DapperDomainPrimitivesRegistration` | `EricksonLopez.DomainPrimitives.Dapper.Generated` | Static generated class with `RegisterAll()` method registering all project `TypeHandler` instances. | Dapper | Application startup initialization for Dapper with thread-safe idempotency. | Basic | Yes (`23-DapperIntegration`) |

---

## 14. Infrastructure: OpenAPI and Newtonsoft.Json Serialization

| Name | Namespace | Responsibility | Dependencies | Use Cases | Complexity Level | Existing Example |
|---|---|---|---|---|---|---|
| `DomainPrimitivesSchemaFilter` | `EricksonLopez.DomainPrimitives.OpenApi.Generated` | Generated Swashbuckle `ISchemaFilter` representing domain primitives by their underlying scalar type in Swagger. | Swashbuckle SwaggerGen | Accurate OpenAPI schema generation in Swagger UI. | Intermediate | Yes (`24-OpenApiIntegration`) |
| `NewtonsoftJsonExtensions` | `EricksonLopez.DomainPrimitives.NewtonsoftJson` | `AddDomainPrimitives` extension methods on `JsonSerializerSettings` and `JsonSerializer`. | Newtonsoft.Json | Legacy projects or pipelines requiring Newtonsoft.Json. | Intermediate | Yes (`08-SerializationAndMapping`) |
| `DomainPrimitivesContractResolver` | `EricksonLopez.DomainPrimitives.NewtonsoftJson` | `DefaultContractResolver` auto-injecting primitive converters without requiring explicit attribute decorations. | Newtonsoft.Json | Transparent serialization in APIs using Newtonsoft contracts. | Intermediate | Yes (`08-SerializationAndMapping`) |
| `DomainPrimitiveUniversalNewtonsoftJsonConverter` | `EricksonLopez.DomainPrimitives.NewtonsoftJson` | Universal `JsonConverter` serializing any domain primitive to its underlying value. | Newtonsoft.Json | Polymorphic serialization support in Newtonsoft.Json. | Intermediate | Yes (`08-SerializationAndMapping`) |
| `DomainPrimitiveNewtonsoftJsonConverter<TPrimitive, TValue>` | `EricksonLopez.DomainPrimitives.NewtonsoftJson` | Strongly-typed `JsonConverter<TPrimitive>` for high performance with Newtonsoft.Json. | Newtonsoft.Json | Direct typed conversion of specific primitives. | Intermediate | Yes (`08-SerializationAndMapping`) |

---

## 15. Testing SDK (`EricksonLopez.DomainPrimitives.Testing`)

| Name | Namespace | Responsibility | Dependencies | Use Cases | Complexity Level | Existing Example |
|---|---|---|---|---|---|---|
| `DomainPrimitiveAssertionsExtensions` | `EricksonLopez.DomainPrimitives.Testing` | Fluent assertion extensions (`HavePrimitiveValue`, `ThrowDomainPrimitiveException`, `ThrowDomainPrimitiveExceptionWithPrimitiveErrorCode`). | AwesomeAssertions | Expressive, readable unit assertions on domain primitives. | Basic | Yes (`19-UnitTesting`) |
| `DomainPrimitiveFakeFactory` | `EricksonLopez.DomainPrimitives.Testing` | Deterministic test factory providing collections of valid and invalid values (`Strings`, `Numerics`, `Dates`, `Identifiers`, `Shortcuts`). | .NET BCL | Deterministic, reproducible test data generation without randomness. | Basic | Yes (`19-UnitTesting`) |
| `DomainPrimitiveScenarios` | `EricksonLopez.DomainPrimitives.Testing` | Parameterized test scenarios ready for `[MemberData]` in xUnit (`ValidEmailInputs`, `InvalidEmailInputs`, `ValidPhoneInputs`, etc.). | .NET BCL | Systematic boundary coverage in parameterized tests. | Basic | Yes (`19-UnitTesting`) |
| `DomainPrimitiveTestBuilder` | `EricksonLopez.DomainPrimitives.Testing` | Test utility providing `Create<T, V>()`, `AssertCreationFails<T, V>()`, and `CreateUnvalidated<T, V>()` for legacy test scenarios. | Reflection (testing only) | Controlled construction and exception assertions in test suites. | Advanced | Yes (`19-UnitTesting`) |
| `DomainPrimitiveVerifyExtensions` | `EricksonLopez.DomainPrimitives.Testing` | `Initialize()` method for snapshot testing with Verify, serializing primitives as scalar values. | Verify.Xunit | Snapshot and visual regression testing. | Intermediate | Yes (`19-UnitTesting`) |
