# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

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
