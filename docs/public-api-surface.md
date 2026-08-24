# Public API Surface

This document defines the core public API surface, design decisions, and NativeAOT implications of `EricksonLopez.DomainPrimitives`.

## Core Abstractions

The library provides several marker attributes located in the `EricksonLopez.DomainPrimitives.Abstractions` assembly. These are intentionally kept as lightweight as possible.

### `[StrongId<T>]`
Applied to a `readonly partial record struct`. Emits the backing field of type `T`, parsing, equality, and formatting logic.
- Supported underlying types: `Guid`, `int`, `long`, `string`, `short`, `byte`, etc.
- Implements: `IEquatable<T>`, `ISpanParsable<T>`, `ISpanFormattable`, `IComparable<T>`.

### `[StringPrimitive]`
Applied to a `readonly partial record struct` representing a domain-validated string.
- Companion attributes: `[Trim]`, `[LowerCase]`, `[UpperCase]`, `[MaxLength(int)]`, `[MinLength(int)]`, `[Regex(pattern)]`.
- Shortcuts: `[Email]`, `[Phone]`, `[Url]`.

### Design of Core Types

The generated code strictly enforces the following patterns:
- **Value Semantics:** Generated types are `readonly record struct`, which avoids heap allocation (zero-boxing).
- **Immutability:** Types are strictly immutable.
- **Thread-Safety:** As immutable value types, they are inherently thread-safe.
- **Factory Methods:** Types are instantiated via `YourId.Create()` (generates a new Guid), `YourId.Create(T value)` (wraps an existing value), or `YourPrimitive.Parse(string value)` (throws `FormatException` on failure).
- **Zero-Allocation Error Pattern:** For validation boundaries, we emit `TryCreate(TValue value, out TSelf result, out PrimitiveError error)`, which returns `bool` and avoids heap allocations on the success path.

## NativeAOT and Trimming Compatibility

A key differentiator of this library is its complete absence of runtime reflection.

> [!TIP]
> **100% NativeAOT Compatible**
> Because all parsing, formatting, database mapping, and JSON serialization are emitted at compile-time via Incremental Source Generators, the library is entirely trim-safe and NativeAOT friendly. 
> There are no `[RequiresUnreferencedCode]` or `[RequiresDynamicCode]` annotations anywhere in the generated output.

## Integrations API Surface

### ASP.NET Core & JSON
- `AddDomainPrimitives()`: Configures `JsonSerializerOptions` to include generated converters.
- `AddDomainPrimitivesModelBinding()`: Configures ASP.NET Core MVC to bind route parameters and query strings to Domain Primitives.
- `AddDomainPrimitivesOpenApi()`: Configures Swashbuckle to map Domain Primitives to their primitive counterparts (e.g., `string`, `integer`, `uuid`) in the OpenAPI schema.

### Entity Framework Core
- `ConfigureDomainPrimitives(this ModelConfigurationBuilder)`: Auto-generated extension method that registers EF Core `ValueConverter` instances for all domain primitives in the assembly during `ConfigureConventions`.

### Dapper
- `DapperDomainPrimitivesRegistration.RegisterAll()`: Auto-generated static method that registers `SqlMapper.TypeHandler` for all domain primitives in the assembly, bypassing reflection-based type handlers.

> [!NOTE]
> **FluentValidation integration removed.** The `EricksonLopez.DomainPrimitives.FluentValidation` package was removed per rfc-0004. Validation should be performed at the application boundary via `TryCreate`. If FluentValidation is used downstream, wrap the result of `TryCreate` in a FluentValidation rule manually.
