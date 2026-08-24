# rfc-0002: StrongId RejectEmpty Default Change

> **Status:** Approved (post-hoc — ratified 2026-08-10)
> **Authors:** Erickson Lopez
> **Created:** 2026-07-20
> **Approved:** 2026-08-10
> **Implemented In:** v1.2.0 (Unreleased)
> **Relates To:** TD-004

---

## Problem Statement

`StrongIdAttribute<Guid>` previously defaulted to `RejectEmpty = false`, meaning `Guid.Empty` was
a valid StrongId value. This caused subtle bugs in production:

1. `default(OrderId)` would compare equal to an explicitly set `OrderId` with `Guid.Empty` value.
2. Guid.Empty as an ID is almost never intentional — it indicates a coding error (uninitialized ID).
3. EF Core and other ORMs can insert `Guid.Empty` into a non-nullable ID column if the primitive
   is not properly constructed, leading to silent data integrity issues.

## Decision

Change the default for `RejectEmpty` from `false` to **`true`**.

Any `StrongId<Guid>` without an explicit `RejectEmpty = false` will now **reject `Guid.Empty`**
at creation time with error code `EMPTY_ID`.

## Migration Guide

```csharp
// Before: Guid.Empty was accepted silently
var id = OrderId.From(Guid.Empty);  // OK in v1.1.0

// After: Guid.Empty is rejected by default
var id = OrderId.Create(Guid.Empty);  // Throws DomainPrimitiveValidationException

// Opt-out if Guid.Empty is legitimate in your domain (rare)
[StrongId<Guid>(RejectEmpty = false)]
public readonly partial record struct NullableOrderId;
```

## Breaking Change Classification

| Type | Level |
|------|-------|
| Source compatibility | ✅ Compatible (same attribute, changed default) |
| Binary compatibility | ✅ Compatible (default argument — resolved at compile time) |
| Behavioral | ❌ Breaking (Guid.Empty now rejected where it was previously accepted) |

## Risks and Mitigations

- **Risk:** Existing data with `Guid.Empty` IDs will fail deserialization.
  **Mitigation:** Analyzer `DP007_AvoidDefaultConstructor` will warn at compile time if `Guid.Empty`
  is explicitly used as a `StrongId` value.
- **Risk:** Unit tests that relied on `Guid.Empty` as a valid test ID will fail.
  **Mitigation:** Update tests to use `Create()` or opt-out via `RejectEmpty = false`.

## Votes

| Maintainer | Decision | Rationale |
|------------|----------|-----------|
| Erickson Lopez | +1 | Prevents silent data integrity bugs |

*Note: This RFC was ratified post-implementation as part of the audit process.*
