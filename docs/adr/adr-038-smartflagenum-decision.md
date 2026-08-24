# adr-038: SmartFlagEnum (Bitfield Enums) Decision

**Date:** 2026-08-19  
**Status:** Deferred / Rejected for v1.x (Subject to Community RFC for v2.x)  
**Authors:** Core maintainers  
**Related audit items:** GAP-05 (AUDITORIA_PARIDAD_FUNCIONAL.md §11, §19)  

---

## Context

`Ardalis.SmartEnum` provides a `SmartFlagEnum` variant that implements bitfield (`[Flags]`-like) semantics over smart enums. This allows bitwise operations (combining, masking, checking `HasFlag`) on enum members.

In `EricksonLopez.DomainPrimitives`, all primitives including `[SmartEnum<TValue>]` are implemented as immutable `readonly record struct` types. Bitfield operations on record structs with custom metadata properties introduce several architectural dilemmas:

1. **Composite Values vs Fixed Constants:** A standard SmartEnum represents a closed set of known instances declared as static fields. A bitfield combination (e.g. `Read | Write = 3`) produces a composite value that is *not* one of the pre-declared static instances.
2. **Metadata Resolution:** If individual flags carry custom properties (e.g. `DisplayName`, `RiskLevel`), what should the composite instance return for those properties?
3. **Allocation & Value Semantics:** Standard enum flags in C# are lightweight primitives. Modeling composite SmartEnums without heap allocations or ambiguous member resolution requires complex generator logic.

---

## Decision

**`SmartFlagEnum` is deferred for v1.x and will not be implemented without a formal community-driven RFC for v2.x.**

### Options Evaluated:

- **Option A (Implement via new generator `[SmartFlagEnum<TValue>]`):**
  - Requires generating bitwise operators (`|`, `&`, `^`, `~`) and `HasFlag(TSelf)`.
  - *Verdict:* Deferred. High risk of ambiguous semantics for custom metadata properties.

- **Option B (Permanent Rejection):**
  - Bitfield enums are considered an anti-pattern in rich domain modeling where separate boolean capabilities or role value objects are preferred.
  - *Verdict:* Rejected as too rigid; specific authorization and telemetry scenarios genuinely benefit from bitfields.

- **Option C (Defer to v2.x with RFC requirements) — ACCEPTED:**
  - Defer until a clear design solves the metadata composition problem for composite bitmask instances.

---

## Interim Workaround

For domain models requiring permission flags or multi-select capabilities, the recommended DDD pattern is to model permissions as an explicit `IReadOnlySet<PermissionEnum>` or a composite `ValueObject` (e.g. `UserPermissions` struct wrapping explicit booleans or a collection).

---

## Consequences

- **Positive:** Keeps `SmartEnumGenerator` focused on high-performance, O(1), AOT-safe single-value enums.
- **Positive:** Avoids shipping an unstable or confusing API for composite bitmask instances.
- **Negative:** Users migrating directly from `Ardalis.SmartFlagEnum` must refactor flag-based bitwise checks to set collections or composite value objects.
