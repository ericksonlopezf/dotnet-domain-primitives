# RFC-0006: ValueObject `IParsable<T>` and `IUtf8SpanParsable<T>` Support

**Status:** Draft  
**Author:** Audit v4.0  
**Date:** 2026-08-10  
**Target:** v2.0.0  
**Related:** TD-006, TD-013

---

## Problem

`ValueObject` is the only domain primitive category that does not implement `IParsable<T>` or
`IUtf8SpanParsable<T>`. All other categories implement the full BCL interface stack. This:

1. Makes `ValueObject` incompatible with generic `where T : IParsable<T>` constraints.
2. Prevents use in ASP.NET Core route/query parameter model binding without a custom converter.
3. Creates an inconsistency that surprises users familiar with the other categories.

---

## Design Constraints

- Zero-allocation rule applies: `TryParse` must not allocate on the success path.
- Composite types have no single canonical string representation.
- The generator cannot know the user's intended serialization format.

---

## Options Evaluated

### Option A: First-property parse (generated)
Delegates to the first property's `IParsable<T>.TryParse`.
**Verdict:** Rejected — property ordering not stable across `partial` definitions.

### Option B: JSON round-trip (generated)
Calls `JsonSerializer.Deserialize<TSelf>(s)`.
**Verdict:** Rejected — always allocates, violating zero-allocation constraint.

### Option C: User partial (scaffold only) — RECOMMENDED
Generator emits `IParsable<T>` on the implements clause + a scaffold comment requiring user to provide implementation. An optional `[ParseStrategy]` attribute can enable auto-generation in v2.1.0.

---

## Recommended Design (v2.0.0)

1. `[ValueObject]` generator adds `IParsable<TSelf>` to the implements clause.
2. Emits a scaffold comment in the generated file.
3. New Roslyn analyzer warns if user partial is missing the implementation.
4. Opt-in `ParseStrategy` attribute deferred to v2.1.0.

---

## Acceptance Criteria

- [ ] `[ValueObject]` types declare `IParsable<T>`.
- [ ] Analyzer warns if implementation is missing.
- [ ] ApiSurfaceBudgetTests updated (budget +2: Parse + TryParse).
