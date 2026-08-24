# adr-018: Reject Class-Based Primitive Support

**Date:** 2026-08-10
**Status:** Accepted
**Authors:** Core maintainers
**Related audit items:** REJECT-adjacent (feature-gaps.md), differentiation.md §1 "Weak Differentiators"

---

## Context

Vogen and Thinktecture.Runtime.Extensions both support generating `partial class` Value Objects in
addition to `readonly partial record struct`. Several user requests have asked whether
`EricksonLopez.DomainPrimitives` plans to support class-based primitives.

The current implementation exclusively accepts `readonly partial record struct` declarations. The
generator's predicate (`IsReadonlyRecordStruct`) rejects any `class` or `record class` keyword at
the syntax filter phase.

The audit (2026-08-10) classified class support as "REJECT-adjacent — Reject (breaks struct-first
design)."

---

## Decision

**We will not add class-based primitive support to `EricksonLopez.DomainPrimitives`.**

All generated types remain `readonly partial record struct`.

---

## Rationale

### 1. Structural invariant enforcement is stronger with structs

A `readonly record struct` enforces immutability at the language level. The compiler prevents
mutation of any field. A `class`-based Value Object must rely on convention (no public setters,
manual `readonly` enforcement) — the generator cannot guarantee the invariant.

### 2. Allocation model is incompatible with class support

The core performance promise of this library is an allocation-minimized hot path. A `class`
instance always allocates on the managed heap. A `struct` value lives on the stack or inline in
its parent structure. Adding class support would require either:

- A separate code path that violates the allocation model.
- Or documenting that class-based primitives have different allocation characteristics, creating
  a confusing two-tier API.

Neither option is acceptable.

### 3. AOT and trimmer compatibility is simpler with structs

`readonly record struct` types have no virtual dispatch, no inheritance chain to analyze, and no
`Activator.CreateInstance` fallback path. Trimmer analysis is trivially complete. Class-based
Value Objects with virtual factory methods or inheritance would require more complex trimmer
annotations.

### 4. Default constructor risk is already documented for structs

The known risk with `readonly record struct` is the zero-value `default(T)` bypass. This is
mitigated by Analyzer DP0001 (`DoNotUseDefaultConstructor`). Adding class support would introduce
*additional* bypass vectors (`Activator.CreateInstance`, JSON deserialization via reflection,
proxy generation) that cannot be detected by a compile-time analyzer.

### 5. The market already has class-based options

Users who specifically need class-based Value Objects have mature alternatives:

- Vogen — supports `partial class` with `VogenDefaults`
- Thinktecture — supports both struct and class
- Manual base class patterns (CSharpFunctionalExtensions `ValueObject<T>`)

Competing in this space does not differentiate `EricksonLopez.DomainPrimitives`. The library's
differentiation is in BCL interface depth, security gates, and normalization — none of which
require class support.

---

## Alternatives Considered

| Alternative | Rejected because |
|-------------|-----------------|
| Add `[ClassPrimitive]` attribute as an opt-in | Creates a two-tier allocation model. Doubles the maintenance surface of every generator. |
| Allow `partial class` with explicit AOT warnings | Users ignore warnings. Creates a library that is "AOT-compatible except when you use class mode." |
| Generate `abstract record` for inheritance scenarios | Inheritance violates Value Object semantics. VOs are not substitutable via polymorphism. |

---

## Consequences

- **Positive:** Generator remains simple — one code path, one allocation model.
- **Positive:** AOT story remains clean.
- **Positive:** Invariant enforcement is guaranteed at the language level.
- **Negative:** Users who require class-based VOs (e.g., for Entity Framework owned types with
  specific behavior) must use Vogen or Thinktecture.
- **Documentation action:** The feature gap is explicitly documented in `docs/rejected-features.md`
  with a migration pointer to Vogen.
