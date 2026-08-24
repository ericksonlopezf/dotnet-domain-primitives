# adr-016: Target Runtime — Primary vs Minimum Supported

**Status:** Accepted  
**Date:** 2026-08-10  
**Authors:** Core maintainers  
**Linked audit item:** CRIT-V4-001 (Audit v4.0)

---

## Context

The Engineering Specification v4.0 (AUDIT.md) declares:

> **Target runtime:** .NET 10 LTS (C# 14) with forward-compat to .NET 11 (C# 15)

It also states in §TECHNICAL PRINCIPLES:

> `EricksonLopez.DomainPrimitives.Core (attributes + PrimitiveError): net10.0;netstandard2.0`

However, the README states:

> **Minimum runtime: net8.0.** `IUtf8SpanParsable<T>`, `RegexOptions.NonBacktracking`,
> `System.Buffers.ArrayPool<T>`, and `MemoryExtensions.ToLowerInvariant` all require .NET 8+.

The audit (2026-08-10 CRIT-V4-001) identified this as a `CONTRADICTED` claim requiring resolution.

---

## Problem Statement

Two interpretations existed simultaneously:

1. **Strict reading of spec v4.0**: `net10.0` is the minimum TFM. Users on NET 8 or NET 9 cannot use the library.
2. **README reading**: `net8.0` is the minimum TFM. NET 10 is the primary development and testing target.

Interpretation 1 would remove the library from consideration for all NET 8 and NET 9 projects — an unjustifiable restriction given that `net8.0` is itself an LTS. This contradicts the mission to maximize adoption.

---

## Decision

**The library supports .NET 8, .NET 9, and .NET 10+ (minimum: net8.0). The primary development and CI verification target is .NET 10 LTS.**

These two concepts are explicitly separated:

| Concept | Value | Meaning |
|---------|-------|---------|
| **Minimum supported TFM** | `net8.0` | Earliest .NET version that provides all required BCL APIs |
| **Primary target** | `net10.0` | The .NET version used for benchmarks, CI primary matrix, and new feature development |
| **Forward compat target** | `net11.0` (preview) | Forward-compatibility testing performed against next preview SDK |
| **Attributes/Abstractions** | `netstandard2.0;net8.0;net9.0;net10.0` | Consumed by projects targeting older TFMs |

The spec v4.0 wording "Target runtime: .NET 10 LTS" means "the primary reference implementation is NET 10" — not "users must be on NET 10." This interpretation is consistent with how the .NET team describes their own "target TFM" for libraries.

### Why net8.0 is the correct minimum:

| API | Minimum TFM | Usage |
|-----|------------|-------|
| `IUtf8SpanParsable<T>` | NET 8 | Zero-allocation UTF-8 parse path |
| `IUtf8SpanFormattable` | NET 8 | Zero-allocation UTF-8 format path |
| `RegexOptions.NonBacktracking` | NET 7 | SEC-002 |
| `INumber<T>` | NET 7 | Generic Math (opt-in) |
| `System.Buffers.ArrayPool<T>` | NET 6 | SEC-006 |
| `MemoryExtensions.ToLowerInvariant` | NET 5 | NFC normalization without allocation |

`IUtf8SpanParsable<T>` (NET 8) is the highest-version BCL dependency in the core hot path. Dropping to NET 7 would require removing it. Since it is central to the library's differentiation claim (unique interface coverage per README), NET 8 is the correct floor.

---

## Correction to Spec v4.0

The Engineering Specification §TECHNICAL PRINCIPLES Multi-TFM strategy wording is updated to read:

```
EricksonLopez.DomainPrimitives.Core (attributes + PrimitiveError):
  - Minimum: net8.0
  - Multi-TFM: net8.0;net9.0;net10.0 + netstandard2.0 for Abstractions only
  - Primary development/benchmark target: net10.0

EricksonLopez.DomainPrimitives.Generators (Source Generator):
  - netstandard2.0 — required by Roslyn

Integration packages (EF Core, Dapper, ASP.NET Core):
  - net8.0 minimum (follows their own dependency requirements)
```

The AUDIT.md §TECHNICAL PRINCIPLES is updated as part of this ADR acceptance.

---

## Alternatives Considered

| Alternative | Rejected because |
|-------------|-----------------|
| NET 10 as hard minimum | Removes 100% of NET 8 and NET 9 users. Unjustifiable for a library at v1.x. |
| NET 9 as minimum | NET 9 is STS (18-month support). NET 8 is LTS (3 years). Better floor is LTS. |
| No declared minimum — "latest only" | Violates the 15-year design horizon. Users need stability guarantees. |

---

## Consequences

- **Positive**: No change to source code — the library already targets `net8.0;net9.0;net10.0`.
- **Positive**: Eliminates the `CONTRADICTED` claim in README vs spec.
- **Positive**: Aligns with how the .NET ecosystem describes "target" vs "minimum."
- **Negative**: The spec v4.0 wording requires a corrigendum (applied here).
- **Note**: If NET 8 reaches end of support (May 2026), the minimum may be elevated to NET 9 in a future minor version, per §LONG-TERM MAINTAINABILITY rule 1.
