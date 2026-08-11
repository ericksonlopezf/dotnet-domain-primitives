# Capability Traceability Matrix

> **Version:** 1.3.0-draft  
> **Last Updated:** 2026-08-10  
> **Required by:** AUDIT.md §11.2  
> **Audit:** Applied corrections from Engineering Spec v4.0 Audit (2026-08-10) — added Release + AC columns (MED-V4-005)

## Legend

| Symbol | Meaning |
|--------|---------|
| ✅ | Fully implemented & verified |
| ⚠️ | Partially implemented or unverified |
| ❌ | Missing or not implemented |
| 🔧 | Fixed in audit 2026-08-10 |
| N/A | Not applicable |

## Core Primitives

| Capability | README | Design Doc | Implementation | Unit Tests | Mutation Test | Benchmark | CI Gate | Sample | Release | AC Doc |
|---|---|---|---|---|---|---|---|---|---|---|
| `[StringPrimitive]` | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | v1.0 | [AC-SP-01..10](acceptance-criteria.md#stringprimitive) |
| `[NumericPrimitive<T>]` | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ⚠️ | v1.0 | [AC-NP-01..03](acceptance-criteria.md#numericprimitive) |
| `[DatePrimitive]` | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ⚠️ | v1.0 | [AC-DP-01..02](acceptance-criteria.md#dateprimitive) |
| `[StrongId]` | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | v1.0 | [AC-SI-01..04](acceptance-criteria.md#strongid) |
| `[ValueObject]` | ✅ | ✅ | 🔧✅ | ✅ | ✅ | ✅ | ✅ | ✅ | v1.0 | [AC-VO-01..03](acceptance-criteria.md#valueobject) |
| `[SmartEnum]` | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | v1.0 | [AC-SE-01..05](acceptance-criteria.md#smartenum) |

## Domain Shortcut Attributes

| Capability | README | Design Doc | Implementation | Unit Tests | CI Gate |
|---|---|---|---|---|---|
| `[Email]` | ✅ | ✅ | ✅ | ✅ | ✅ |
| `[Phone]` | ✅ | ✅ | ✅ | ✅ | ✅ |
| `[Url]` | ✅ | ✅ | ✅ | ✅ | ✅ |
| `[Slug]` | ✅ | ✅ | ✅ | ✅ | ✅ |
| `[CountryCode]` | ✅ | ✅ | ✅ | ✅ | ✅ |
| `[LanguageCode]` | ✅ | ✅ | ✅ | ✅ | ✅ |
| `[CurrencyCode]` | ✅ | ✅ | ✅ | ✅ | ✅ |
| `[Username]` | ✅ | ✅ | ✅ | ✅ | ✅ |
| `[PasswordHash]` | ✅ | ✅ | ✅ | ✅ | ✅ |
| `[HexColor]` | ✅ | ✅ | ✅ | ✅ | ✅ |
| `[IPAddress]` | ✅ | ✅ | ✅ | ✅ | ✅ |
| `[MacAddress]` | ✅ | ✅ | ✅ | ✅ | ✅ |
| `[IBAN]` | ✅ | ✅ | ✅ | ✅ | ✅ |
| `[ISBN]` | ✅ | ✅ | ✅ | ✅ | ✅ |
| `[VIN]` | ✅ | ✅ | ✅ | ✅ | ✅ |

## BCL Interface Coverage (Generated Code)

| Interface | String | Numeric | Date | StrongId | ValueObject |
|---|---|---|---|---|---|
| `IDomainPrimitive<T>` | ✅ | ✅ | ✅ | ✅ (IStrongId) | 🔧✅ |
| `IParsable<T>` | ✅ | ✅ | ✅ | ✅ | ❌ (planned v2.0) |
| `ISpanParsable<T>` | ✅ | ✅ | ✅ | ✅ | ❌ (planned v2.0) |
| `IUtf8SpanParsable<T>` | ✅ | ✅ | ✅ | ✅ | ❌ (planned v2.0) |
| `IFormattable` | ✅ | ✅ | ✅ | ✅ | 🔧✅ |
| `ISpanFormattable` | ✅ | ✅ | ✅ | ✅ | 🔧✅ |
| `IUtf8SpanFormattable` | ✅ | ✅ | ✅ | ✅ | ❌ (planned v2.0) |
| `IComparable<T>` | ✅ | ✅ | ✅ | ✅ | ❌ (N/A — composite types have no natural order) |
| `IEqualityOperators` | ✅ | ✅ | ✅ | ✅ | 🔧✅ |
| `IComparisonOperators` | ✅ | ✅ | ✅ | ✅ | ❌ (N/A — no natural order for composite types) |
| `TypeConverter` | ✅ | ✅ | ✅ | ✅ | 🔧✅ |
| `IsDefault` | ✅ | ✅ | ✅ | ✅ | 🔧✅ |
| `STJ JsonConverter` | ✅ | ✅ | ✅ | ✅ | 🔧✅ |

> **Note:** `IComparable<T>` and `IComparisonOperators` are intentionally NOT implemented for
> `ValueObject` because composite value objects have no canonical ordering unless the domain
> explicitly defines one. Users who need ordering should implement `IComparable<T>` in their
> partial struct definition.

## Integration Packages

| Integration | Package | SourceGen | Unit Tests | Sample | AC |
|---|---|---|---|---|---|
| System.Text.Json | ✅ (Inline, Core) | ✅ | ✅ | ⚠️ | Covered in AC-SP-08 |
| EF Core | ✅ | ✅ | ✅ | ⚠️ | AC-SI-04 |
| Dapper | ✅ | ✅ | ✅ | ✅ ([23-DapperIntegration](../samples/OfficialSample/23-DapperIntegration/)) | AC-SI-04 analogue |
| ASP.NET Core | ✅ | ✅ | ✅ | ⚠️ | — |
| Mapster | ✅ | ✅ | ✅ | ❌ (TD-015) | — |
| OpenAPI | ✅ | ✅ | ✅ | ✅ ([24-OpenApiIntegration](../samples/OfficialSample/24-OpenApiIntegration/)) | — |

## Security Gates

| Gate | Specification | Status | Test |
|------|--------------|--------|------|
| SEC-001: 4096 char limit | All string primitives without MaxLength reject inputs > 4096 chars | 🔧✅ | `SEC001_*` tests |
| SEC-002: NonBacktracking regex | `RegexOptions.NonBacktracking` on NET7+, timeout on older TFMs | ✅ | `SEC002_*` tests |
| SEC-003: 100ms regex timeout | `TimeSpan.FromMilliseconds(100)` injected by generator | ✅ | `SEC003_*` tests |
| SEC-004: NFC normalization | All string inputs normalized to FormC before validation (🔧 fast path fixed in audit) | 🔧✅ | `SEC004_*` tests |
| SEC-005: No PII in errors | Sensitive type errors do not expose input values | 🔧✅ | `SEC005_*` tests |
| SEC-006: ArrayPool limits | Stackalloc ≤ 256 chars (512 bytes) on char span path; ≤ 256 bytes on UTF-8 byte path; ArrayPool with try/finally for larger inputs | 🔧✅ | `SEC006_*` tests |

## Infrastructure

| Capability | Status | Notes |
|---|---|---|
| Roslyn Analyzers (DP0001–DP0016) | ✅ | 16 diagnostics implemented |
| Diagnostic ID collision avoidance | ✅ | TD-010 resolved |
| Strong naming | ✅ | Conditional on .snk presence |
| SourceLink | ✅ | Requires git remote |
| NuGet package validation | ✅ | Baseline v1.1.0 |
| Deterministic builds | 🔧✅ | CRIT-006 fixed: `EmbedUntrackedSources` is now unconditional (was CI-only) |
| NativeAOT compatibility | ✅ | `IsAotCompatible=true` + `dotnet publish` CI gate |
| Mutation testing (Stryker) | ✅ | Configured and running in CI |
| Benchmark execution | ✅ | `dotnet run` benchmark gate in CI |
| API compatibility tracking | 🔧✅ | ApiCompat step added to CI (CRIT-003) |
| Benchmark regression gate | 🔧✅ | `check-regression.sh` comparing vs baseline (MED-005) |
| Security gate CI step | 🔧✅ | Dedicated `Security` trait test filter step (HIGH-002) |

## README Claims Audit (§11.1)

This section audits every public claim in the README against implementation reality.

| README Claim | Status | Evidence |
|-------------|--------|---------|
| "Allocation-minimized hot paths" | ✅ ACCURATE (README rewritten 2026-08-10) | README now shows per-path allocation table: 0 allocs success (no normalization), 1 alloc success (NFC), 1 alloc failure, 1 alloc TryParse(span). JSON ValueSpan path VERIFIED (GeneratorHelpers.cs:45-51, NET8+). Old claim "zero-allocation" was inaccurate — removed. |
| "NativeAOT-ready" | ✅ IMPLEMENTED | `IsAotCompatible=true`, `[DynamicDependency]` not used, zero reflection in generators. CI gate validates publish. |
| "Zero reflection" | ✅ IMPLEMENTED | All generation is compile-time. No `Type.GetMethod()`, `Expression<>`, or Activator calls in hot paths. |
| "BCL-conventional API" | ⚠️ PARTIALLY_IMPLEMENTED | FormatException standardization complete (RFC-0003). ValueObject interfaces added (CRIT-004). ValueObject still missing `IComparable<T>` — intentionally N/A for composites. |
| "Source-generated — no runtime overhead" | ✅ IMPLEMENTED | All types are generated at compile time with `IIncrementalGenerator`. |
| "Roslyn Analyzers enforce correct usage" | ✅ IMPLEMENTED | DP0001–DP0016 active. DP0016 covers factory naming. |
| "15-year design horizon (2026–2041)" | ⚠️ UNVERIFIED | Planning risks documented in `planning-risks.md`. Spec v4.0 is the governing document. No forward-compat tests yet beyond NET10. |
| "STJ integration is inline (no extra package)" | ✅ IMPLEMENTED | Generated converter is a private nested class. No `EricksonLopez.DomainPrimitives.Json` package. |
| "Supports netstandard2.0" | ⚠️ PARTIALLY_IMPLEMENTED | `Abstractions` targets `netstandard2.0`. `Core`/generators target `net8.0+` only. See MED-006. |
