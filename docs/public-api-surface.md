# Public API Surface

This document defines the core public API surface, design decisions, and NativeAOT implications of `EricksonLopez.DomainPrimitives`.

## Core Abstractions

The library provides marker attributes and contracts located in the `EricksonLopez.DomainPrimitives.Abstractions` assembly. These are intentionally kept lightweight and zero-dependency (`netstandard2.0; net8.0; net9.0; net10.0`).

### `[StrongId<T>]`
Applied to a `readonly partial record struct`. Emits the backing field of type `T`, parsing, equality, and formatting logic.
- Supported underlying types: `Guid`, `int`, `long`, `string`, `short`, `byte`, etc.
- Implements: `IDomainPrimitive<TSelf, TValue>`, `IStrongId<TSelf, TValue>`, `IEquatable<T>`, `ISpanParsable<T>`, `ISpanFormattable`, `IComparable<T>`.

### `[StringPrimitive]`
Applied to a `readonly partial record struct` representing a domain-validated string.
- Companion attributes: `[Trim]`, `[LowerCase]`, `[UpperCase]`, `[MaxLength(int)]`, `[MinLength(int)]`, `[Regex(pattern)]`.
- Prepackaged shortcuts: `[Email]`, `[Phone]`, `[Url]`, `[Slug]`, `[CountryCode]`, `[IBAN]`, `[ISBN]`, `[HexColor]`, `[MACAddress]`, `[IPAddress]`.
- Implements: `IDomainPrimitive<TSelf, string>`, `IParsable<T>`, `ISpanParsable<T>`, `IUtf8SpanParsable<T>`, `IFormattable`, `ISpanFormattable`, `IUtf8SpanFormattable`.

### `[NumericPrimitive<T>]`
Applied to a `readonly partial record struct` representing a domain-validated numeric value.
- Backing types: `decimal`, `double`, `float`, `int`, `long`, `short`.
- Prepackaged shortcuts: `[Money]`, `[Price]`, `[TaxRate]`, `[Percentage]`, `[Quantity]`, `[Rating]`.
- Implements: `IDomainPrimitive<TSelf, T>`, `IComparable<T>`, arithmetic operator overloads.

### `[DatePrimitive]`
Applied to a `readonly partial record struct` representing a domain-validated temporal type.
- Backing types: `DateOnly`, `DateTime`, `DateTimeOffset`.
- Prepackaged shortcuts: `[BirthDate]`, `[PastDate]`, `[FutureDate]`, `[ExpirationDate]`.
- Implements: `IDomainPrimitive<TSelf, TDate>`, `IComparable<T>`.

### `[SmartEnum]` & `[SmartFlagEnum]`
Base class hierarchies and generators for type-safe polymorphic enums with compile-time zero-allocation matching (`Match<TResult>`, `Map<TResult>`, `Switch`).
- Case-insensitive lookups via `FromName(name, ignoreCase: true)` and `TryFromName(name, ignoreCase: true, out result)`.

### `[ValueObject]`
Applied to a `readonly partial record struct` for composite value objects with structural equality and domain invariant validation.
- Implements `IParsable<T>`, `ISpanParsable<T>`, `IUtf8SpanParsable<T>`, `ISpanFormattable`.

### Design of Core Types

The generated code strictly enforces the following patterns:
- **Value Semantics:** Generated types are `readonly record struct`, which avoids heap allocation (zero-boxing).
- **Immutability:** Types are strictly immutable. Deep collection immutability is enforced via Roslyn analyzer **DP0018**.
- **Thread-Safety:** As immutable value types, they are inherently thread-safe.
- **Factory Methods:** Types are instantiated via `YourId.Create()` (generates a new Guid), `YourId.Create(T value)` (wraps an existing value), or `YourPrimitive.Parse(string value)` (throws `FormatException` on failure).
- **Zero-Allocation Error Pattern:** For validation boundaries, we emit `TryCreate(TValue value, out TSelf result, out PrimitiveError error)`, which returns `bool` and avoids heap allocations on the success path.

## NativeAOT and Trimming Compatibility

A key differentiator of this library is its complete absence of runtime reflection in core and source-generated converters.

> [!TIP]
> **100% NativeAOT Compatible**
> Because all parsing, formatting, database mapping, and JSON serialization are emitted at compile-time via Incremental Source Generators, the library is entirely trim-safe and NativeAOT friendly. 
> There are no `[RequiresUnreferencedCode]` or `[RequiresDynamicCode]` annotations in core or source-generated components.

## Integrations API Surface

### ASP.NET Core & JSON (`EricksonLopez.DomainPrimitives.AspNetCore`)
- `AddDomainPrimitives()`: Configures `JsonSerializerOptions` to include generated converters.
- `AddDomainPrimitivesModelBinding()`: Configures ASP.NET Core MVC to bind route parameters and query strings to Domain Primitives.
- `DomainPrimitiveValidationAttribute<TPrimitive, TValue>`: Validation attribute for properties to participate in standard DataAnnotations validation.
- `DomainPrimitiveValidator.Validate<TPrimitive, TValue>()`: Helper enabling custom DTOs implementing `IValidatableObject` to validate raw inputs into domain primitives.

### OpenAPI / Swagger (`EricksonLopez.DomainPrimitives.OpenApi`)
- `AddDomainPrimitivesOpenApi()`: Configures Swashbuckle schema filters to map Domain Primitives to their primitive counterparts (e.g., `string`, `integer`, `uuid`) in the OpenAPI schema.

### Entity Framework Core (`EricksonLopez.DomainPrimitives.EFCore`)
- `ConfigureDomainPrimitives(this ModelConfigurationBuilder)`: Auto-generated extension method that registers EF Core `ValueConverter` instances for all domain primitives in the assembly during `ConfigureConventions`.

### Dapper (`EricksonLopez.DomainPrimitives.Dapper`)
- `DapperDomainPrimitivesRegistration.RegisterAll()`: Auto-generated static method that registers `SqlMapper.TypeHandler` for all domain primitives in the assembly, bypassing reflection-based type handlers.
- `DomainPrimitivesJsonRegistration.RegisterAll(JsonSerializerOptions)`: Emitted by `JsonConverterGenerator` for compile-time JSON converter registration across multi-assembly projects.

### Newtonsoft.Json Migration (`EricksonLopez.DomainPrimitives.NewtonsoftJson`)
- `DomainPrimitiveNewtonsoftJsonConverter<TPrimitive, TValue>` & `DomainPrimitivesContractResolver`: Integration layer for legacy JSON.NET applications. Explicitly annotated with `[RequiresDynamicCode]` (not NativeAOT compatible by design).

## Architectural Roslyn Analyzers

18 active diagnostic analyzers (**DP0001–DP0018**) enforce modeling integrity:
- **DP0001–DP0005**: Must be `readonly partial record struct`, private constructor enforcement.
- **DP0006–DP0010**: Correct attribute usage, normalization precedence, and exception configuration.
- **DP0014**: API surface budget enforcement.
- **DP0017**: Valid custom exception type verification.
- **DP0018**: ValueObject collection immutability (prohibits mutable arrays or lists, enforces `ImmutableArray<T>`).

> [!NOTE]
> **FluentValidation integration removed.** The `EricksonLopez.DomainPrimitives.FluentValidation` package was removed per rfc-0004. Validation should be performed at the application boundary via `TryCreate`. If FluentValidation is used downstream, wrap the result of `TryCreate` in a FluentValidation rule manually.
