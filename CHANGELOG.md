# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.0.0] — 2026-08-10

### Initial Release

This is the initial 1.0.0 release of the `EricksonLopez.DomainPrimitives` ecosystem. 

### Important Design Decisions (Pre-release Refactoring)

> Note: These changes reflect final design decisions and refactorings made prior to the stable 1.0.0 release.

- **[RFC-0003]** `Parse()` now throws `System.FormatException` instead of `DomainPrimitiveFormatException` to align with BCL standards. `DomainPrimitiveFormatException` is deprecated with `[Obsolete(error: false)]`.
- **[RFC-0004]** Removed `EricksonLopez.DomainPrimitives.FluentValidation` integration package.
- **[RFC-0002]** `StrongIdAttribute.RejectEmpty` is now `true` by default. `Guid.Empty` is rejected by `Create()` unless opted out.
- **[RFC-0001]** Renamed `StrongId<T>` factory methods `New()` and `From()` to `Create()`. `New()` and `From()` are restored as `[Obsolete(error: false)]` bridges.
- `ArithmeticPolicy` enum and `Policy` property on `NumericPrimitiveAttribute<T>` restored as `[Obsolete(error: false)]` aliases for `NumericOperations` and `Operations` respectively. Aliases will be removed in v3.0.
- **[Diagnostics move — ADR-015]** `DomainPrimitivesMetrics`, `DomainPrimitivesDiagnostics`, `DomainPrimitiveEventSource` moved from `Abstractions.dll` to `Core.dll`. 

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
- Filed RFC-0006 (`docs/rfcs/RFC-0006-valueobject-iparsable.md`) defining the design for `ValueObject IParsable<T>` implementation in v2.0.0.
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
