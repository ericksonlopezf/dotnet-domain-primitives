# Architecture & Design Philosophy

> **See also:** [system-overview.md](system-overview.md) for the system-level overview and dependency diagrams.  
> [boundary.md](boundary.md) for the `Abstractions` package boundary specification.  
> [docs/adr/](adr/) for all 41 Architecture Decision Records.

---

## Design Priorities

The following priority stack governs all design decisions. When priorities conflict, the higher priority wins:

| Priority | Principle | Enforcement |
|----------|-----------|-------------|
| **P0** | Semantic Correctness | Domain invariants must always hold |
| **P1** | NativeAOT & Trimming Compatibility | Zero IL3050/IL2026 warnings; zero `Activator`/reflection in hot paths |
| **P2** | Zero Allocation on Hot Paths | `TryCreate`, `Parse`, comparisons incur 0 bytes heap allocation |
| **P3** | Performance | Verified by BenchmarkDotNet |
| **P4** | API Ergonomics | BCL alignment, discoverable, minimal surface |

**Rule:** If optimizing P3 breaks P0, P0 wins — always.

---

## Forward-Compatible Design

`EricksonLopez.DomainPrimitives` targets `.NET 8+` (C# 14) and is designed for forward compatibility. All generated code avoids deprecated APIs. The library's dependency on stable BCL interfaces (`IParsable<T>`, `ISpanFormattable`, `IUtf8SpanParsable<T>`) ensures long-term viability as the .NET runtime evolves.

---

## Zero-Allocation Policy (P2)

The library enforces a strict zero-allocation policy across the generator core and all generated primitives:

1. **`TryCreate` out Pattern:** Validation methods return `bool` with the primitive via `out` parameter — no heap-allocating `Result<T>` wrappers on hot paths. (See [ADR-003](adr/adr-003-trycreate-result-pattern.md) and [ADR-006](adr/adr-006-use-validation-error-instead-of-result.md).)
2. **UTF-8 Parsing:** `ArrayPool<char>` is used when decoding `ReadOnlySpan<byte>` above the 256-character threshold (SEC-006), avoiding large `char[]` heap allocations. (See [ADR-007](adr/adr-007-zero-allocation-error-model.md).)
3. **Struct-Based Primitives:** Domain primitives are `readonly record struct` instances — stack-allocated. (See [ADR-001](adr/adr-001-record-structs-for-domain-primitives.md).)

> **One unavoidable allocation:** Unicode NFC normalization (SEC-004) requires producing a `System.String` because normalization can change the character count. This is the minimum allocation on any validated string path.

---

## Semantic Correctness (P0)

**"If optimizing performance (P3) breaks semantic correctness (P0), P0 wins."**

A domain primitive must never misrepresent its validated state. Validation rules are absolute, and invariants cannot be bypassed through any public API. The generator enforces this by making the constructor `private` and exposing creation exclusively through `Create()` and `TryCreate()`.

---

## Exception Philosophy

| Factory | Exception Behavior | BCL Alignment |
|---------|-------------------|---------------|
| `Create(TValue)` | Throws `DomainPrimitiveValidationException` on validation failure | Throw on expected invalid input |
| `Parse(string)` | Throws `System.FormatException` | Aligns with `IParsable<T>` contract ([rfc-0003](rfcs/rfc-0003-format-exception-standardization.md)) |
| `TryCreate(TValue, out T, out PrimitiveError)` | Returns `false`; never throws | Exception-free validation |
| `TryParse(string, out T)` | Returns `false`; never throws | Aligns with `IParsable<T>` |

> **Deprecation:** `DomainPrimitiveFormatException` was deprecated with `[Obsolete(error: false)]` in v1.0.0 (standardized via rfc-0003) and will be removed in v3.0.

---

## Source Generators & Integration Auto-Discovery

The library relies on Roslyn Incremental Source Generators (`IIncrementalGenerator`) to emit zero-overhead integration code. All generators use `ForAttributeWithMetadataName` for precise, semantics-aware filtering. ([ADR-002](adr/adr-002-use-source-generators-for-domain-primitives.md), [ADR-014](adr/adr-014-mapster-integration.md), [ADR-032](adr/adr-032-exclude-source-generators-mutation-testing.md).)

**Integration auto-discovery principle:** Developers do not need to annotate domain models with integration-specific attributes. The integration generators (EFCore, Dapper, AspNetCore, OpenAPI) discover all types implementing `IDomainPrimitive<TSelf, TValue>` at compile time.

---

## Primitive Type Taxonomy

The library generates six primitive categories:

| Category | Attribute | Underlying Type | Key Interfaces Generated |
|----------|-----------|-----------------|--------------------------|
| Strong ID | `[StrongId]` | `Guid`, `int`, `long`, `string` | `IDomainPrimitive<TSelf, TValue>`, `IComparable<T>`, `IParsable<T>`, `ISpanParsable<T>` |
| String Primitive | `[StringPrimitive]` | `string` | `IDomainPrimitive<TSelf, string>`, `IParsable<T>`, `ISpanParsable<T>`, `IUtf8SpanParsable<T>`, `IFormattable`, `ISpanFormattable`, `IUtf8SpanFormattable` |
| Numeric Primitive | `[NumericPrimitive<T>]` | `decimal`, `double`, `float`, `int`, `long`, `short` | `IDomainPrimitive<TSelf, T>`, `IComparable<T>`, arithmetic operators |
| Date Primitive | `[DatePrimitive]` | `DateOnly`, `DateTime`, `DateTimeOffset` | `IDomainPrimitive<TSelf, TDate>`, `IComparable<T>` |
| Smart Enum | `[SmartEnum]` | `string` (name-based) | `IDomainPrimitive<TSelf, string>`, exhaustive `Match<TResult>`, `Map<TResult>`, `Switch` |
| Value Object | `[ValueObject]` | composite | `IDomainPrimitive<TSelf>`, `IParsable<T>`, `ISpanParsable<T>`, `IUtf8SpanParsable<T>` |

---

## Security Architecture (Automated Gates)

All string-backed primitives automatically enforce six security gates at generation time. These cannot be disabled without explicit opt-out:

| Gate ID | Protection | Mechanism |
|---------|-----------|-----------|
| SEC-001 | 4096-character hard ceiling | Generated `MaxLength` validation |
| SEC-002 | ReDoS prevention on .NET 7+ | `RegexOptions.NonBacktracking` |
| SEC-003 | ReDoS timeout on older TFMs | `TimeSpan.FromMilliseconds(100)` timeout |
| SEC-004 | Unicode homoglyph prevention | NFC Unicode normalization before validation |
| SEC-005 | No PII in error messages | Input values never echoed in `PrimitiveError` |
| SEC-006 | Bounded buffer allocation | `stackalloc` ≤ 256 chars; `ArrayPool<char>` above |

> See [docs/security.md](security.md) for the full specification.

---

## Static Abstract Interfaces (CRTP Pattern)

The library uses C# 11+ `static abstract` interface members extensively to enable generic constraints with compile-time factory method access. This pattern (documented in [ADR-013](adr/adr-013-static-abstract-interfaces.md)) is what makes `IDomainPrimitive<TSelf>.Create(value)` possible without reflection.

```csharp
// CRTP enables this generic usage — no reflection, no boxing:
T primitive = T.Create(rawValue);
bool success = T.TryCreate(rawValue, out T result, out PrimitiveError error);
string name = T.PrimitiveName;
```

---

## API Surface Budget

Every generated struct is governed by an API surface budget enforced by Roslyn Analyzer DP0014 and verified by `ApiSurfaceBudgetTests` in CI:

| Primitive Category | Member Budget |
|--------------------|--------------|
| StringPrimitive | ≤ 35 members |
| NumericPrimitive | ≤ 38 members |
| StrongId (Guid) | ≤ 40 members |
| DatePrimitive | ≤ 37 members |

Exceeding the budget fails the build. New members require justification via the Feature Gate process. ([ADR-012](adr/adr-012-security-gates.md), GOVERNANCE.md §Design Principles.)

---

## NativeAOT & Trimming Compatibility

- `Abstractions` and `Generators` packages: `IsAotCompatible=true`, `IsTrimmable=true`
- Generated code: zero `Type.GetMethod()`, `Activator.CreateInstance()`, or runtime expression compilation
- CI gate: `aot-smoke-test.yml` publishes the `AotProbe` project with `PublishAot=true` and executes the native binary on every push/PR

---

## Design Decisions

All significant architectural decisions are formally documented as Architecture Decision Records (ADRs):

- [docs/adr/](adr/) — 41 ADRs covering primitives design, generator architecture, integration decisions, and rejected features
- [docs/rfcs/](rfcs/) — 12 RFCs covering API design changes (factory naming, exception standardization, multi-TFM strategy, etc.)

Key decisions:
- [ADR-001](adr/adr-001-record-structs-for-domain-primitives.md) — `readonly record struct` over `class`
- [ADR-002](adr/adr-002-use-source-generators-for-domain-primitives.md) — Source Generators over T4/reflection
- [ADR-006](adr/adr-006-use-validation-error-instead-of-result.md) — `PrimitiveError` + `out` over `Result<T>`
- [ADR-018](adr/adr-018-reject-class-based-primitives.md) — Reject class-based primitives (permanently)
- [ADR-025](adr/adr-025-reject-result-as-primary-api.md) — Reject `Result<T>` as primary API
- [ADR-040](adr/adr-040-dual-paradigm-declarative-generators-vs-prepackaged-catalog.md) — Dual paradigm (declarative generators + shortcut catalog)
