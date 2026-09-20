# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [3.0.0] — 2026-09-20

### Breaking Changes

- **BC-009: StrongId explicit conversion to backing type on default instance throws `InvalidOperationException`**
  - **What changed:** Generated `explicit operator` converting a Strongly-Typed ID to its backing primitive type (`Guid`, `int`, etc.) now guards against default (uninitialized) instances.
  - **Previous behavior:** Casting `(Guid)default(UserId)` or `(int)default(OrderId)` returned `Guid.Empty` or `0`.
  - **Current behavior:** Casting a default Strongly-Typed ID throws `System.InvalidOperationException` with message `"Cannot convert a default <TypeName> to <BackingType>. Check IsDefault before casting."`.
  - **Affected consumers:** Any code performing explicit casts from uninitialized or `default(TStrongId)` instances to raw backing primitives.
  - **Migration:** Guard casts using the `.IsDefault` property or `.Value` getter instead of direct unchecked casting from default instances.

- **BC-010: StrongId explicit conversion from backing type validates via `Create()`**
  - **What changed:** Generated `explicit operator` converting from the raw backing type to the Strongly-Typed ID now routes through `Create(value)` instead of invoking the internal struct constructor `new(value)`.
  - **Previous behavior:** Casting `(UserId)Guid.Empty` bypassed all invariant checks (e.g., `RejectEmpty`).
  - **Current behavior:** Casting `(UserId)Guid.Empty` invokes `Create(value)` and throws `DomainPrimitiveValidationException` (or configured custom exception) if `RejectEmpty` is enabled.
  - **Affected consumers:** Codebases relying on explicit cast syntax `(TStrongId)rawValue` to construct identifiers from potentially empty or invalid raw values.
  - **Migration:** Ensure raw values satisfy domain invariants before casting, or use `TryCreate(value, out var id, out var err)` for graceful fallback handling.

- **BC-011: StrongId `ToString()` returns `string.Empty` on default instances**
  - **What changed:** Formatter methods `ToString()` and `ToString(format, formatProvider)` on Guid- and Integer-backed StrongIds now inspect `IsDefault`.
  - **Previous behavior:** `default(UserId).ToString()` emitted `"00000000-0000-0000-0000-000000000000"`; `default(OrderId).ToString()` emitted `"0"`.
  - **Current behavior:** Formatter methods return `string.Empty` when `IsDefault` is true.
  - **Affected consumers:** Any consumers asserting, logging, or formatting default Strongly-Typed IDs expecting `"00000000-0000-0000-0000-000000000000"` or `"0"`.
  - **Migration:** Check `id.IsDefault` before formatting, or adapt format assertions to accept `string.Empty` for uninitialized instances.

- **BC-012: StrongId `TryParse` validates domain invariants and rejects `Guid.Empty`**
  - **What changed:** Generated `TryParse(string, ...)` and `TryParse(ReadOnlySpan<char>, ...)` methods now delegate to `TryCreate` instead of instantiating directly via struct constructor.
  - **Previous behavior:** `UserId.TryParse("00000000-0000-0000-0000-000000000000", out var id)` returned `true` even when `RejectEmpty` was true.
  - **Current behavior:** `TryParse` returns `false` when input represents `Guid.Empty` and `RejectEmpty` is enabled.
  - **Affected consumers:** Codebases expecting `TryParse` to succeed on `Guid.Empty` string representations for types with `RejectEmpty = true`.
  - **Migration:** If empty identifiers are semantically permissible in your domain, specify `[StrongId(RejectEmpty = false)]`. Otherwise, handle `false` returns from `TryParse`.

- **BC-013: DatePrimitive explicit conversion to backing temporal type on default instance throws `InvalidOperationException`**
  - **What changed:** Generated explicit operator converting a DatePrimitive to its underlying temporal type (`DateTime`, `DateOnly`, etc.) now guards against default instances.
  - **Previous behavior:** `(DateTime)default(BirthDate)` returned `0001-01-01 00:00:00`.
  - **Current behavior:** Casting a default DatePrimitive throws `System.InvalidOperationException` with message `"Cannot convert a default <TypeName> to <BackingType>. Check IsDefault before casting."`.
  - **Affected consumers:** Consumers casting default DatePrimitive instances to raw temporal types.
  - **Migration:** Check `datePrimitive.IsDefault` before casting, or access `.Value` safely.

- **BC-014: DatePrimitive `ToString()` returns `string.Empty` on default instances**
  - **What changed:** `ToString()` and `ToString(format, formatProvider)` on generated DatePrimitives now check `IsDefault`.
  - **Previous behavior:** `default(BirthDate).ToString()` returned `"01/01/0001 00:00:00"`.
  - **Current behavior:** `ToString()` returns `string.Empty` when `IsDefault` is true.
  - **Affected consumers:** String formatting or template rendering of uninitialized DatePrimitives.
  - **Migration:** Verify `IsDefault` prior to display formatting, or expect `string.Empty` for default values.

- **BC-015: DatePrimitive `DateTime` factory normalizes unspecified and local kinds to UTC**
  - **What changed:** `Create(DateTime)` and `TryCreate(DateTime, ...)` now normalize incoming `DateTimeKind`.
  - **Previous behavior:** Incoming `DateTime` values were stored with their original `Kind` untouched.
  - **Current behavior:** `DateTimeKind.Unspecified` is assigned `DateTimeKind.Utc` via `DateTime.SpecifyKind(value, DateTimeKind.Utc)`; `DateTimeKind.Local` is converted via `value.ToUniversalTime()`.
  - **Affected consumers:** Systems passing local or unspecified `DateTime` values expecting local time retention without conversion.
  - **Migration:** Explicitly pass UTC `DateTime` instances, or account for UTC normalization when querying the stored `.Value`.

- **BC-016: NumericPrimitive arithmetic operators enforce `checked` arithmetic**
  - **What changed:** Generated overloaded arithmetic operators (`+`, `-`, `*`, `/`) and unary negation now wrap arithmetic operations in `checked(...)`.
  - **Previous behavior:** Operations overflowing the range of the backing type wrapped around silently.
  - **Current behavior:** Arithmetic overflow throws `System.OverflowException`.
  - **Affected consumers:** Calculations on NumericPrimitives near boundary limits that relied on unchecked wrap-around.
  - **Migration:** Catch `OverflowException` or perform range pre-validation before performing arithmetic operations.

- **BC-017: SmartEnum `All` collection changed from mutable array (`T[]`) to `ReadOnlyCollection<T>`**
  - **What changed:** The static `All` collection is now wrapped with `Array.AsReadOnly(...)`.
  - **Previous behavior:** `All` was backed by a mutable array `T[]` exposed through `IReadOnlyList<T>`, allowing casts to `(T[])` and element mutation.
  - **Current behavior:** `All` is backed by `ReadOnlyCollection<T>`. Explicit casts to `(T[])All` fail at runtime.
  - **Affected consumers:** Downstream code casting `(MyEnum[])MyEnum.All`.
  - **Migration:** Consume `All` strictly as `IReadOnlyList<T>`, or invoke `.ToArray()` if an independent array instance is required.

- **BC-018: ValueObject `System.Text.Json` deserializer throws `JsonException` on missing non-nullable properties**
  - **What changed:** Generated `JsonConverter<T>` for `[ValueObject]` now tracks property presence during JSON object token parsing.
  - **Previous behavior:** Omitted non-nullable JSON properties silently received `default` values and were forwarded into `Create(...)`.
  - **Current behavior:** If a declared non-nullable property is omitted or contains JSON `null`, deserialization throws `JsonException($"Required property '{p.Name}' was missing or null in JSON object for {typeName}.")`.
  - **Affected consumers:** API clients or message consumers receiving partial JSON payloads for composite Value Objects.
  - **Migration:** Ensure JSON payloads supply all required non-nullable properties, or mark optional properties as nullable (`T?`).

- **BC-019: ValueObject and scalar primitives serialize default instances as JSON `null`**
  - **What changed:** Generated `System.Text.Json` converter `Write(...)` methods for ValueObjects and domain primitives now verify `value.IsDefault`.
  - **Previous behavior:** Default ValueObjects serialized as `{ "prop1": ..., "prop2": ... }` with default fields; default primitives serialized as raw zero/empty values.
  - **Current behavior:** `value.IsDefault` writes a JSON `null` value via `writer.WriteNullValue()`.
  - **Affected consumers:** Systems expecting empty JSON objects or raw zero values when serializing default domain structs.
  - **Migration:** Update consumers expecting `{}` to handle JSON `null`, or ensure primitives are initialized via `Create(...)` rather than `default`.

- **BC-020: `PrimitiveCollectionExtensions.ToDomainPrimitiveList` wraps validation failures into `ArgumentException`**
  - **What changed:** `ToDomainPrimitiveList<TPrimitive, TValue>` now catches `DomainPrimitiveValidationException` and rethrows as `ArgumentException` including element index.
  - **Previous behavior:** Threw `DomainPrimitiveValidationException` directly out of the extension method.
  - **Current behavior:** Throws `System.ArgumentException` with the original `DomainPrimitiveValidationException` nested in `ex.InnerException`.
  - **Affected consumers:** Callers with specific `catch (DomainPrimitiveValidationException)` blocks around `ToDomainPrimitiveList`.
  - **Migration:** Update exception handling to `catch (ArgumentException ex)` and inspect `ex.InnerException as DomainPrimitiveValidationException`.

- **BC-021: Roslyn Analyzer DP0018 (`ValueObjectMutableCollection`) enabled with `Warning` severity**
  - **What changed:** Added analyzer DP0018 checking for mutable collection types (`T[]`, `List<T>`, `Dictionary<K, V>`, `HashSet<T>`) within `[ValueObject]` definitions.
  - **Previous behavior:** No analyzer warned on collection properties in ValueObjects.
  - **Current behavior:** Emits diagnostic `DP0018` with severity `Warning`. In projects with `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`, this breaks compilation.
  - **Affected consumers:** Projects defining ValueObjects with arrays or mutable collections compiled under TreatWarningsAsErrors.
  - **Migration:** Replace mutable collections with `ImmutableArray<T>` or `IReadOnlyList<T>`, or apply `#pragma warning disable DP0018` locally if mutation safety is externally guaranteed.

- **BC-022: Roslyn Analyzer DP0001 (`PublicConstructorBypass`) now flags explicit parameterless constructors**
  - **What changed:** Analyzer DP0001 was updated to flag any public constructor that is not implicitly compiler-generated (`constructor.IsImplicitlyDeclared`).
  - **Previous behavior:** Parameterless constructors (`Parameters.Length == 0`) were skipped, allowing explicit `public MyPrimitive() { }` declarations without diagnostics.
  - **Current behavior:** Explicit public parameterless constructors on domain primitives are reported with diagnostic `DP0001` (`Warning`).
  - **Affected consumers:** Projects that explicitly defined parameterless constructors on domain primitive types compiled under TreatWarningsAsErrors.
  - **Migration:** Remove explicit public parameterless constructors and rely on `Create(...)` factory methods.

- **BC-023: NewtonsoftJson converters throw `JsonSerializationException` on `null` for non-nullable primitives**
  - **What changed:** `DomainPrimitiveNewtonsoftJsonConverter` and `DomainPrimitiveUniversalNewtonsoftJsonConverter` now reject JSON `null` for non-nullable primitive types.
  - **Previous behavior:** Deserializing `null` returned `default` (or `Activator.CreateInstance(objectType)`).
  - **Current behavior:** Throws `JsonSerializationException($"Cannot deserialize null into non-nullable domain primitive '{objectType.Name}'.")`.
  - **Affected consumers:** Applications using Newtonsoft.Json deserializing payloads with `null` fields into non-nullable domain primitives.
  - **Migration:** Ensure incoming payloads supply valid primitive values, or declare destination properties as nullable `TPrimitive?`.

- **BC-024: EF Core Value Converter Generator corrected inverted `DatePrimitiveKind` mapping for `DateTimeOffset` and `TimeOnly`**
  - **What changed:** In `EFCoreValueConverterGenerator`, the integer mapping for `DatePrimitiveKind.DateTimeOffset` (`2`) and `DatePrimitiveKind.TimeOnly` (`3`) was corrected.
  - **Previous behavior:** `Kind = 2` mapped to `TimeOnly` and `Kind = 3` mapped to `DateTimeOffset`.
  - **Current behavior:** `Kind = 2` maps to `DateTimeOffset` and `Kind = 3` maps to `TimeOnly`.
  - **Affected consumers:** EF Core entity models utilizing `[DatePrimitive(Kind = DatePrimitiveKind.DateTimeOffset)]` or `[DatePrimitive(Kind = DatePrimitiveKind.TimeOnly)]`.
  - **Migration:** Regenerate EF Core model snapshots and migrations to align with corrected converter types.

### Added

- **`DapperTypeHandlerGenerator`: Convention and interface discovery for Dapper TypeHandler generation**
  - Expanded generator discovery to identify domain types implementing `IDomainPrimitive<TSelf, TValue>` or adhering to standard ValueObject conventions across referenced project assemblies, automatically emitting zero-overhead Dapper `SqlMapper.TypeHandler<T>` registrations.
- **Roslyn Analyzer DP0018 & Code Fix: ValueObject collection immutability enforcement**
  - Added diagnostic analyzer DP0018 that flags array (`T[]`) or mutable collection properties (`List<T>`, `Dictionary<TKey, TValue>`, `HashSet<T>`) within `[ValueObject]` definitions to ensure deep immutability, with automated code fixes to `ImmutableArray<T>` ([adr-045](docs/adr/adr-045-analyzer-dp0018-valueobject-collection-immutability.md), [rules/dp0018](docs/rules/dp0018.md)).
- **`AspNetCore`: DataAnnotations & `IValidatableObject` integration**
  - Added `DomainPrimitiveValidationAttribute<TPrimitive, TValue>` and static helper `DomainPrimitiveValidator.Validate<TPrimitive, TValue>()` enabling seamless participation in ASP.NET Core DataAnnotations and MVC model validation pipelines.
- **`Dapper.SourceGenerators`: Compile-time `JsonConverter<T>` generation**
  - Added `JsonConverterGenerator` generating reflection-free `System.Text.Json` converter classes and centralized `DomainPrimitivesJsonRegistration.RegisterAll(JsonSerializerOptions)` for discovered domain primitive and entity ID types.
- **Benchmark Regression Quality Gate CI Workflow**
  - Added `.github/workflows/benchmark-regression-gate.yml` and `scripts/verify-benchmark-gate.ps1` asserting 0 B heap allocation on hot paths and capping PR latency regressions at $\le 5\%$ against `benchmarks/results/baseline.json`.
- **14-Job Parallel Mutation Testing Matrix**
  - Re-architected `.github/workflows/mutation-testing.yml` to execute a 14-job parallel matrix covering all individual packages using dedicated `stryker-*.json` configuration profiles.
- **NativeAOT Smoke Test Pipeline**
  - Added dedicated standalone probe project `tests/EricksonLopez.DomainPrimitives.AotSmokeTest` and reusable workflow `.github/workflows/aot-smoke-test.yml` verifying standalone Linux-x64 compilation with `PublishAot=true` and runtime execution without IL3050/IL2026 warnings.
- **Adversarial, Fuzzing & Property-Based Contract Tests**
  - Added `AdversarialSecurityTests.cs`, `ConcurrencyAndFuzzingTests.cs`, and `PropertyBasedContractTests.cs` to the test suite to validate thread-safety, immutability, and boundary conditions.

### Changed

- **`DapperTypeHandlerGenerator`: Unified syntax and referenced assembly ValueObject convention discovery**
  - Unified candidate inspection pipeline between current compilation syntax trees and referenced assembly metadata symbols to ensure consistent TypeHandler emission across multi-assembly solution topologies.

### Fixed

- **`DapperTypeHandlerGenerator`: Extended assembly deny-list to prevent generation for third-party library types**
  - **Problem:** During a `--no-incremental` build, the generator walked `Npgsql.Internal` and generated
    a `SizeTypeHandler` that referenced `Npgsql.Internal.Size` (an experimental API). This caused 4 build
    errors with diagnostic `NPG9001`. The deny-list only excluded `System`, `Microsoft`, `netstandard`,
    `mscorlib`, `Dapper`, and `EricksonLopez` prefixes.
  - **Fix:** Added the following prefixes to the assembly deny-list:
    `Npgsql`, `Polly`, `FluentValidation`, `Serilog`, `OpenTelemetry`, `Azure`, `AWSSDK`,
    `StackExchange`, `Newtonsoft`, `AutoMapper`, `MediatR`, `Mapster`, `Bogus`,
    `xunit`, `NUnit`, `Moq`, `NSubstitute`.
  - **Impact:** No change to generated handler count for domain types. The generator continues to produce
    738 Entity ID and Value Object handlers for OpusHydra. Handlers for Npgsql internal types are no
    longer generated.
  - **Related:** CAP-003, `GetReferencedAssemblyPrimitives()` in `DapperTypeHandlerGenerator.cs`.

## [2.0.0] — 2026-08-24

### Breaking Changes

- **BC-001: Removed `EricksonLopez.DomainPrimitives.Mapster` and `EricksonLopez.DomainPrimitives.Mapster.SourceGenerators` packages**
  - **What changed:** The dedicated Mapster integration package and its companion source generator have been removed from the repository.
  - **Previous behavior:** Projects could reference `EricksonLopez.DomainPrimitives.Mapster` to generate `IRegister` TypeAdapterConfigs automatically.
  - **Current behavior:** The packages are discontinued and deleted.
  - **Affected consumers:** Any project referencing `EricksonLopez.DomainPrimitives.Mapster` or `EricksonLopez.DomainPrimitives.Mapster.SourceGenerators`.
  - **Migration:** Mapster natively resolves the `explicit operator` generated on all scalar domain primitives without extra packages. Remove package references and rely on standard Mapster type mapping or manual configurations for composite types ([adr-017](docs/adr/adr-017-mapster-integration-rationale.md), [adr-030](docs/adr/adr-030-reject-automapper-integration.md)).

- **BC-002: Removed transitive testing dependencies (`NSubstitute`, `xunit`, `xunit.core`, `xunit.assert`, `xunit.extensibility.core`) from `EricksonLopez.DomainPrimitives.Testing`**
  - **What changed:** Removed transitive test framework and mocking packages from `EricksonLopez.DomainPrimitives.Testing.csproj`.
  - **Previous behavior:** Referencing the testing package implicitly provided `NSubstitute` and `xunit` assertion APIs.
  - **Current behavior:** Transitive dependencies were stripped to prevent dependency pollution. Only `AwesomeAssertions` and `Verify.Xunit` remain referenced.
  - **Affected consumers:** Downstream test projects that relied on transitive `NSubstitute` or `xunit` package imports.
  - **Migration:** Add explicit `<PackageReference Include="NSubstitute" />` and `<PackageReference Include="xunit" />` to test projects.

- **BC-003: Removed transitive `Swashbuckle.AspNetCore` dependency from `EricksonLopez.DomainPrimitives.AspNetCore`**
  - **What changed:** Removed `Swashbuckle.AspNetCore` package reference from `EricksonLopez.DomainPrimitives.AspNetCore.csproj`.
  - **Previous behavior:** Installing `AspNetCore` package transitively brought in Swashbuckle OpenAPI tooling.
  - **Current behavior:** The dependency was removed to keep `AspNetCore` Native AOT trim-safe and decoupled.
  - **Affected consumers:** Projects relying on transitive Swashbuckle imports from `EricksonLopez.DomainPrimitives.AspNetCore`.
  - **Migration:** Install `EricksonLopez.DomainPrimitives.OpenApi` or reference `Swashbuckle.AspNetCore` directly.

- **BC-004: Strict domain invariant validation enforced during `System.Text.Json` deserialization for `[ValueObject]`**
  - **What changed:** `ValueObjectJsonConverter` now constructs instances via `Create(...)`, actively executing the `Validate` partial hook during JSON deserialization.
  - **Previous behavior:** Deserialization bypassed `Create(...)` and invariant validation.
  - **Current behavior:** Invalid payloads that fail composite value object validation throw `DomainPrimitiveValidationException` (or configured custom exception) during `JsonSerializer.Deserialize<T>()`.
  - **Affected consumers:** Systems receiving and deserializing payloads with invalid composite value object state.
  - **Migration:** Ensure incoming JSON payloads conform to value object domain validation invariants, or catch validation exceptions during deserialization.

- **BC-005: Configurable validation exception throw sites via `[assembly: DomainPrimitivesDefaults(ExceptionType = ...)]`**
  - **What changed:** When `ExceptionType` is set at the assembly level, all generated `Create()` methods throw the configured exception instead of `DomainPrimitiveValidationException`.
  - **Previous behavior:** Generated validation failure throw sites always emitted `DomainPrimitiveValidationException` (or `ArgumentException` for SmartEnum).
  - **Current behavior:** Generated throw sites instantiate and throw the user-specified exception type.
  - **Affected consumers:** Codebases that opt into assembly-level `ExceptionType`.
  - **Migration:** Catch blocks expecting `DomainPrimitiveValidationException` must be updated to catch the specified custom exception type or `System.Exception`.

- **BC-006: Removed all root-level static fake data properties from `DomainPrimitiveFakeFactory`**
  - **What changed:** All root-level static properties (`ValidEmails`, `InvalidEmails`, `ValidPhones`, `ValidUrls`, `ValidSlugs`, `ValidCountryCodes`, `ValidGuids`, `ValidMoneyAmounts`, `ValidAges`, `ValidLatitudes`, `ValidLongitudes`, `ValidPercentages`, `ValidWeights`, `ValidHeights`, `ValidDistances`, `ValidTemperatures`, `ValidScores`, `ValidQuantities`, `ValidPrices`, `ValidTaxRates`, `ValidDiscounts`, `ValidCurrencyCodes`, `ValidIBANs`, `ValidISBNs`, `ValidVINs`, `ValidHexColors`, `ValidRatings`, `Today`, `ValidBirthDate`, `PastDate`, `FutureDate`, `ValidExpirationDates`, `ValidBusinessDates`, `ValidFiscalYears`) were removed from `DomainPrimitiveFakeFactory`.
  - **Previous behavior:** Test suites accessed fake data directly through `DomainPrimitiveFakeFactory.<PropertyName>`.
  - **Current behavior:** Fake data is strictly organized under categorized nested classes: `Strings`, `Numerics`, `Identifiers`, `Dates`, and `Shortcuts`.
  - **Affected consumers:** Any downstream test projects referencing root-level `DomainPrimitiveFakeFactory` static properties.
  - **Migration:** Update member access to use domain-specific nested classes (e.g. `DomainPrimitiveFakeFactory.Strings.ValidEmails`, `DomainPrimitiveFakeFactory.Numerics.ValidPrices`, `DomainPrimitiveFakeFactory.Identifiers.ValidGuids`, `DomainPrimitiveFakeFactory.Dates.ValidBirthDate`, `DomainPrimitiveFakeFactory.Shortcuts.ValidIBANs`).

- **BC-007: SmartEnum `FromName` binary signature modified with optional parameter**
  - **What changed:** Generated `FromName(string name)` method signature now includes `bool ignoreCase = false`.
  - **Previous behavior:** Emitted method signature was `public static T FromName(string name)`.
  - **Current behavior:** Emitted method signature is `public static T FromName(string name, bool ignoreCase = false)`.
  - **Affected consumers:** External pre-compiled binaries referencing SmartEnum types compiled against v1.0.0 without recompilation.
  - **Migration:** Recompile consumer projects against v2.0.0. For source code consumers, no changes are required as the call syntax remains backward-compatible at compile time.

- **BC-008: Removed deprecated `PrimitiveBuilder<TPrimitive, TValue>.BuildResult()` method**
  - **What changed:** The deprecated `BuildResult()` method on `PrimitiveBuilder<TPrimitive, TValue>` was removed from `EricksonLopez.DomainPrimitives.Abstractions`.
  - **Previous behavior:** In v1.0.0, `BuildResult()` was restored as an `[Obsolete(error: false)]` stub returning `object` (throwing `NotSupportedException` when called) intended for removal in v3.0.
  - **Current behavior:** The method is completely removed from the assembly public API surface.
  - **Affected consumers:** Code compiled against v1.0.0 referencing `PrimitiveBuilder.BuildResult()`.
  - **Migration:** Migrate to `Build(out TPrimitive result)` for non-throwing instantiation or `BuildOrThrow()` to throw `DomainPrimitiveValidationException` on invalid values.

### Added

- **`EricksonLopez.DomainPrimitives.NewtonsoftJson` package** (NOW-002 / GAP-002): `DomainPrimitiveNewtonsoftJsonConverter<TPrimitive, TValue>`, non-generic universal converter, `DomainPrimitivesContractResolver`, and `AddDomainPrimitives()` extension. `[RequiresDynamicCode]` annotated — intentionally not AOT-compatible. [adr-026](docs/adr/adr-026-newtonsoft-json-gap-plan.md).
- **`[assembly: DomainPrimitivesDefaults]` global configuration** (NEXT-001 / GAP-011): `DomainPrimitivesDefaultsAttribute` provides assembly-level defaults for `Trim`, `NotEmpty`, `MaxLength`, and `ExceptionType`. All 5 generators read assembly-level defaults; per-type attribute takes precedence. [adr-033](docs/adr/adr-033-global-assembly-configuration.md).
- **Configurable exception type + analyzer DP0017** (NEXT-002 / GAP-003): `ExceptionType` property on defaults attribute. Generator emits custom throw site. DP0017 validates the type at compile time. [adr-034](docs/adr/adr-034-configurable-exception-type.md).
- **SmartEnum exhaustive `Match<TResult>`, `Map<TResult>`, `Switch` methods** (NEXT-003 / GAP-006): Zero-allocation compiler-enforced exhaustive matching. [adr-035](docs/adr/adr-035-smartenum-exhaustive-switch-map.md).
- **SmartEnum case-insensitive `TryFromName(string, bool, out T)` / `FromName(string, bool)`** (NEXT-004 / GAP-007). [adr-036](docs/adr/adr-036-smartenum-case-insensitive-parsing.md).
- **ValueObject BCL parsing & formatting interfaces**: `[ValueObject]` generated types now implement `IParsable<T>`, `ISpanParsable<T>`, `IUtf8SpanParsable<T>`, `IUtf8SpanFormattable`, `IFormattable`, `ISpanFormattable`, and `IDomainPrimitive<T>`.
- **ASP.NET Core Model Binding**: `DomainPrimitivesMvcBuilderExtensions.AddDomainPrimitivesModelBinding()` and `DomainPrimitiveModelBinder<T>` for dynamic MVC model binding.
- Migration guides: `docs/migration/from-vogen.md` and `docs/migration/from-stronglytypedid.md` (NOW-003 / GAP-010).

### Documentation

- Tech-debt, ROADMAP, feature-gaps, positioning, competitive-analysis, competitive-evidence, differentiation all updated to reflect post-consolidation implementation state.

## [1.0.0] — 2026-08-10


### Initial Release

This is the initial 1.0.0 release of the `EricksonLopez.DomainPrimitives` ecosystem. 

### Important Design Decisions (Pre-release Refactoring)

> Note: These changes reflect final design decisions and refactorings made prior to the stable 1.0.0 release.

- **[rfc-0003]** `Parse()` now throws `System.FormatException` instead of `DomainPrimitiveFormatException` to align with BCL standards. `DomainPrimitiveFormatException` is deprecated with `[Obsolete(error: false)]`.
- **[rfc-0004]** Removed `EricksonLopez.DomainPrimitives.FluentValidation` integration package.
- **[rfc-0002]** `StrongIdAttribute.RejectEmpty` is now `true` by default. `Guid.Empty` is rejected by `Create()` unless opted out.
- **[rfc-0001]** Renamed `StrongId<T>` factory methods `New()` and `From()` to `Create()`. `New()` and `From()` are restored as `[Obsolete(error: false)]` bridges.
- `ArithmeticPolicy` enum and `Policy` property on `NumericPrimitiveAttribute<T>` restored as `[Obsolete(error: false)]` aliases for `NumericOperations` and `Operations` respectively. Aliases will be removed in v3.0.
- **[Diagnostics move — adr-015]** `DomainPrimitivesMetrics`, `DomainPrimitivesDiagnostics`, `DomainPrimitiveEventSource` moved from `Abstractions.dll` to `Core.dll`. 

### Added

- Core abstractions and marker attributes for Domain Primitives.
- Source Generators for Strongly-Typed IDs and String Primitives.
- ASP.NET Core integrations (Model Binders, JSON configs).
- Entity Framework Core Value Converters integration.
- Dapper Type Handlers integration.
- Mapster integration for automatic mapping.
- OpenAPI/Swagger support for Domain Primitives.
- Bogus and AutoFixture testing helpers.
- Roslyn Analyzers to enforce strict usage of Domain Primitives.
- `LengthAttribute.ErrorCode` and `LengthAttribute.ErrorMessage` properties for consistent error customization.
- `PrimitiveError.Create(string code, string message)` static factory method for clarity.
- `PrimitiveBuilder<T,V>.For()` static factory. `BuildOrThrow()` now explicitly throws `DomainPrimitiveValidationException`. `Build()` now returns `bool` with `out` parameter.
- `PrimitiveBuilder<T,V>.BuildResult()` restored as deprecated `[Obsolete]` stub for binary compatibility.
- Created RFC 0001 for `StrongId<T>` factory naming standardization.
- Nested `DebugView` class and `[DebuggerTypeProxy]` attribute in generated types for better debugging.
- All 6 source generators migrated from `CreateSyntaxProvider` + `IsCandidateRecordStruct` to `ForAttributeWithMetadataName`. FQNs centralized in `GeneratorShared`. `IsReadonlyRecordStruct` predicate is O(1). Multi-FQN generators use Collect+SelectMany deduplication to prevent duplicate hintNames. Closes R02 in `planning-risks.md`.
- Dedicated CI step "API Surface Budget Gate (P2 Gate)" added to `dotnet-build-test.yml` filtering on `[Trait("Category", "ApiSurfaceBudget")]`. Closes R06 in `planning-risks.md`.
- `DatePrimitive_ApiSurface_IsWithinBudget` test added for `RegistrationTimestamp` (budget ≤ 37). `DatePrimitive` added to the API surface census inventory.
- Filed rfc-0006 (`docs/rfcs/rfc-0006-valueobject-iparsable.md`) defining the design for `ValueObject IParsable<T>` implementation in v2.0.0.
- All 14 previously-unshipped API members promoted to `PublicAPI.Shipped.txt`. `PublicAPI.Unshipped.txt` is now empty. `PackageValidationBaselineVersion` bumped to 1.2.0.

### Changed

- `TryParse(ReadOnlySpan<char>)` fast path (no case normalization) now NFC-normalizes before calling `TryValidateSpan` per SEC-004.
- `TryParse(ReadOnlySpan<char>)` with case normalization now uses `MemoryExtensions.ToLowerInvariant/ToUpperInvariant` in-place, reducing intermediate allocations from 3 to 1.
- `EmbedUntrackedSources` is now unconditionally `true` in `Directory.Build.props` (was CI-conditional, violating determinism spec).
- Generator `LangVersion` pinned to `14` (was `latest` — not reproducible across SDK versions).
- Generator no longer suppresses CS8600–CS8625 nullable warnings. Generator code is now fully null-safe.
- Normalization attribute matching in generator now guards by `EricksonLopez.DomainPrimitives` namespace to prevent false matches from user-defined attributes with the same names.
- `PrimitiveError.Code` and `PrimitiveError.Message` now declared as `string?` to accurately reflect that `PrimitiveError.None` has `null` Code and Message.
- `System.Diagnostics.DiagnosticSource` removed from `Abstractions.csproj` netstandard2.0 target. Diagnostics types were already moved to Core.
- `PublicAPI.Shipped.txt` regenerated to fix mojibake encoding (garbled Spanish locale entries), correct `IStrongId.New()` → `IStrongId.Create()`, and add correct `PrimitiveBuilder` API surface.
- URL validation dead code `#if NET10_0_OR_GREATER` block removed (both branches were identical).
- Unified `COMPATIBILITY_MATRIX.md` framework targets.
- Improved `IsCandidateRecordStruct` string matching performance (now superseded by `ForAttributeWithMetadataName` migration).
- Optimized `EquatableArray<T>.GetHashCode()` to avoid boxing.
- Restructured string-backed ID `TryParse` to remove dead code.

### Fixed

- Fixed pre-existing `ReadOnlySpan<TValue>` build failure on `netstandard2.0` in `PrimitiveCollectionExtensions.cs` by wrapping the span overload in `#if NET7_0_OR_GREATER`.
- Fixed bug where `IsDefault` was true for `Guid.Empty` even if it was explicitly created.
- Added `ArgumentNullException.ThrowIfNull(value)` in `Create()` and `TryCreate()` to prevent NRE.
- Analyzers now use syntax filter before semantic model lookup for performance.
- Removed duplicated Sonar Analysis step in CI.
