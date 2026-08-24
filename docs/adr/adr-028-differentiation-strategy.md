# adr-028: Differentiation Strategy — BCL-Native + Security-by-Default

**Date:** 2026-08-10
**Status:** Accepted
**Authors:** Core maintainers
**Related documents:** product_strategy.md (2026-08-10), positioning.md, differentiation.md

---

## Context

The audit (2026-08-10) identified that `EricksonLopez.DomainPrimitives` had accumulated
multiple positioning claims that were either:

1. **Commodities** — true but shared with all competitors (Source Generator-driven, Native AOT).
2. **False** — the "zero-allocation" claim (corrected in adr-027).
3. **Genuine differentiators** — unique capabilities not found in any competitor.

Without a deliberate differentiation strategy, marketing material mixed all three categories,
diluting the impact of the genuine differentiators and exposing the library to credibility risks
when the false claims were questioned.

---

## Decision

**`EricksonLopez.DomainPrimitives` will differentiate on exactly three pillars:**

### Pillar 1: BCL-Native (Deepest BCL Interface Coverage)

**Claim (approved):**
> "The only .NET domain primitive generator that produces `IUtf8SpanParsable<T>`,
> `ISpanFormattable`, and `IUtf8SpanFormattable` — the deepest BCL interface coverage
> in the .NET domain primitive ecosystem."

**Evidence:**
- `IUtf8SpanParsable<T>`: `StringPrimitiveGenerator.Parsing.cs:152-206` — NET8+ guard confirmed.
- `ISpanFormattable`: `capability-matrix.md` — ✅ for all types.
- `IUtf8SpanFormattable`: `capability-matrix.md` — ✅ for all types except ValueObject (v2.0).
- Competitive verification: Vogen ❌, Thinktecture ❌, StronglyTypedId ❌ for both UTF-8 interfaces.

**Why it matters:**
In high-throughput .NET services (gRPC, Kafka consumers, ASP.NET Core minimal APIs), parsing
from UTF-8 bytes without an intermediate string allocation is a measurable performance advantage
at scale. No other domain primitive generator provides this BCL integration out of the box.

**Monitoring threat:** SuperStrong.Types — the only emerging library explicitly targeting the
same BCL interfaces. Re-evaluate competitiveness every 3 months.

---

### Pillar 2: Security-by-Default

**Claim (approved):**
> "The only .NET domain primitive library with built-in security gates: NFC Unicode normalization
> (SEC-004), NonBacktracking regex with 100ms timeout (SEC-002+SEC-003), and a 4096-character
> default limit (SEC-001) — applied automatically before your domain validation runs."

**Evidence:**
- SEC-001: `StringPrimitiveGenerator.Validation.cs:66-69` — hardcoded 4096 guard.
- SEC-002: `StringPrimitiveGenerator.Regex.cs` — `RegexOptions.NonBacktracking` on NET7+.
- SEC-003: `StringPrimitiveGenerator.Regex.cs` — `TimeSpan.FromMilliseconds(100)`.
- SEC-004: `StringPrimitiveGenerator.Parsing.cs:39` — `.Normalize(NormalizationForm.FormC)`.
- Competitive: Zero competitors have any of these protections.

**Why it matters:**
ReDoS (Regular Expression Denial of Service) is a documented attack vector in .NET applications
using user-supplied regex patterns. Unicode homoglyph attacks (e.g., "admin" vs "аdmin" with
Cyrillic 'a') are used in phishing and authorization bypass. No other domain primitive library
addresses either attack by default.

**Monitoring threat:** No current competitor. Potential: if Vogen adds NonBacktracking regex
as a default option, this pillar is contested.

---

### Pillar 3: Zero Domain Contamination (Auto-Discovery)

**Claim (approved):**
> "Auto-discovered EF Core and Dapper converters — no per-type annotations needed in your
> domain layer. The domain stays pure."

**Evidence:**
- EF Core: `EricksonLopez.DomainPrimitives.EFCore` package architecture — auto-discovers all
  generated types without `[ValueObject(conversions: Conversions.EfCore)]` or equivalent.
- Dapper: `DapperDomainPrimitivesRegistration.RegisterAll()` — single call, zero per-type
  configuration.
- Competitive: Vogen requires `Conversions.EfCore` flag; STI requires explicit conversion
  declaration; THK requires per-type EF config.

**Why it matters:**
In DDD, the domain layer must not have knowledge of infrastructure concerns. Annotating domain
types with EF Core or Dapper conversion flags contaminates the domain with persistence concerns.
DP's auto-discovery preserves the architectural boundary.

**Monitoring threat:** Moderate — Vogen could add auto-discovery as a new `VogenDefaults`
option with medium implementation effort.

---

## Claims NOT to Compete On

| Claim | Reason not to use |
|-------|-------------------|
| `"Source Generator-driven"` | All major competitors (Vogen, THK, STI) use IIncrementalGenerator. Not a differentiator. |
| `"Native AOT compatible"` | Vogen, THK, STI are all AOT-compatible. Not a differentiator. |
| `"Strictly valid domain model"` | All competitors validate on construction. Table stakes. |
| `"Zero-allocation"` (unqualified) | Corrected in adr-027. Use "allocation-minimized" with per-path nuance. |
| `"15-Year Horizon"` | Unverifiable promise. Replaced with architectural statement. |

---

## Approved Positioning Statement

> For .NET 8+ developers who need domain primitives that are correct, secure, and allocation-
> minimized, `EricksonLopez.DomainPrimitives` is the BCL-native domain primitive generator
> that produces `IUtf8SpanParsable<T>`, applies 6 security gates by default, and auto-discovers
> EF Core and Dapper converters — unlike Vogen, Thinktecture, and StronglyTypedId, which
> require manual configuration, have no security gates, and do not support UTF-8-native parsing.

---

## Consequences

- **Positive:** Marketing material is now coherent, defensible, and focused.
- **Positive:** Differentiators are documented with source code evidence — not marketing claims.
- **Positive:** The three pillars provide a clear filter for feature prioritization: "Does this
  feature strengthen BCL-Native, Security-by-Default, or Zero Domain Contamination?"
- **Negative:** Narrowing the differentiation means some users for whom these pillars are not
  priorities will choose competitors. This is correct — the library is not for everyone.
- **Documentation action:** README Why DomainPrimitives? table updated to lead with the three
  pillar differentiators. Commodity claims demoted or removed.
