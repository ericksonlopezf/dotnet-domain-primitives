# adr-027: Positioning Correction — "Zero-Allocation" Claim

**Date:** 2026-08-10
**Status:** Accepted
**Authors:** Core maintainers
**Related audit items:** differentiation.md §2 "False Differentiators", competitive-evidence.md §6,
                         positioning.md "Claims to Remove Immediately"

---

## Context

The original README contained the claim:

> "A high-performance, **zero-allocation**, Source Generator-driven library for building a
> rich, immutable, and strictly valid domain model in .NET."

The audit (2026-08-10) conducted a path-by-path allocation analysis of the source generator
output (`StringPrimitiveGenerator.Parsing.cs`) and found:

| Path | Allocation reality |
|------|--------------------|
| `TryCreate(string)` success — no normalization | ✅ 0 allocations |
| `TryCreate(string)` success — with `[Trim]`/`[LowerCase]` | ⚠️ 1 string alloc |
| `TryCreate(string)` — SEC-004 NFC normalization | ⚠️ 1 string alloc (unavoidable) |
| `TryParse(ReadOnlySpan<char>)` ≤ 256 chars | ⚠️ 1 string alloc (NFC) |
| `TryParse(ReadOnlySpan<byte>)` ≤ 256 chars | ⚠️ 1 string alloc (NFC) |
| JSON deserialize via `Utf8JsonReader.ValueSpan` | ⚠️ 1 string alloc (NFC+storage) |
| `TryFormat(Span<char>)` | ✅ 0 allocations |
| EF Core struct materialization | ✅ 0 allocations |

**Root cause:** SEC-004 (NFC Unicode normalization) is applied on ALL string paths and
produces a `System.String` from `.Normalize(FormC).ToString()`. Unicode normalization can
change the character count (combining characters → composed form), so the result cannot be
stored as a `Span<char>`. The allocation is unavoidable while NFC normalization is active.

The unqualified claim "zero-allocation" was therefore **partially false** and required correction.

---

## Decision

### Claims removed from all marketing material

| Claim | Status | Replacement |
|-------|--------|-------------|
| `"zero-allocation"` (unqualified) | ❌ Removed | `"allocation-minimized hot paths"` |
| `"Source Generator-driven"` (as primary differentiator) | ❌ Removed as differentiator | Kept as technical description; replaced with `"BCL-native"` as differentiator |
| `"15-Year Horizon"` (as guarantee) | ❌ Removed | `"Built for .NET 8+ and AOT-first architectures with a long-term design horizon"` |
| `"strictly valid domain model"` (as differentiator) | ❌ Removed as differentiator | Kept as factual description; all competitors validate on construction |
| `"Deep Ecosystem Integrations"` (unqualified) | ❌ Softened | `"Auto-discovered integrations"` |

### Corrected claim wording (approved for use)

```
Allocation model:
  "Allocation-minimized hot paths.
   Zero heap allocations on the success path without normalization.
   One unavoidable allocation per normalized value (NFC Unicode requirement — SEC-004).
   No Result<T> wrapper allocations."
```

### Claims verified as accurate (retain as-is)

| Claim | Evidence |
|-------|----------|
| `IUtf8SpanParsable<T>` generated | `StringPrimitiveGenerator.Parsing.cs:152-206` |
| `ISpanFormattable` + `IUtf8SpanFormattable` generated | `capability-matrix.md` |
| ReDoS-resistant regex (SEC-002 + SEC-003) | `StringPrimitiveGenerator.Regex.cs` |
| NFC Unicode normalization (SEC-004) | `StringPrimitiveGenerator.Parsing.cs:39` |
| 4096-char default limit (SEC-001) | `StringPrimitiveGenerator.Validation.cs:66-69` |
| Auto-discovery EF Core + Dapper | `EFCore`/`Dapper` package architecture |
| 39 semantic domain shortcuts | `StringShortcutAttributes.cs` (15) + `NumericShortcutAttributes.cs` (15) + `TemporalShortcutAttributes.cs` (9) |

---

## Actions Taken

1. ✅ README updated: "zero-allocation" → "allocation-minimized hot paths" with per-path table.
2. ✅ README updated: "Source Generator-driven" removed as primary differentiator; "BCL-native"
   added.
3. ✅ README updated: "15-Year Horizon" softened to architectural statement.
4. ✅ `docs/differentiation.md` §6 recommendations applied.
5. ✅ `docs/positioning.md` "Claims to Remove Immediately" section reflects this decision.
6. ✅ Benchmark results page (`docs/benchmark-results.md`) updated to show honest per-path
   allocation table.
7. ✅ (2026-08-24 audit) `docs/benchmark-results.md` corrected: `EmailAddress.Create(string)` row
   was incorrectly showing `0 B / ✅ VERIFIED` which contradicted this ADR's established model.
   Corrected to `48 B / ⚠️ 1 alloc (NFC normalization per ADR-027)`.

---

## Consequences

- **Positive:** All marketing claims are now verifiable and defensible.
- **Positive:** The honest allocation model builds trust — developers can rely on the accuracy
  of the documentation.
- **Positive:** Frees us to focus on genuinely unique differentiators (BCL interfaces,
  security gates).
- **Negative:** Some users who were attracted by the "zero-allocation" claim may feel the
  library is less differentiated. Mitigation: the correct claim ("allocation-minimized") is
  still meaningfully better than alternatives, and the security gate story is unique.
- **Note:** The 1 unavoidable NFC allocation is architecturally correct — it prevents Unicode
  homoglyph attacks. The trade-off is intentional and documented.
