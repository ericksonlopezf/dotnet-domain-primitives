# adr-041: PublicAPI.Shipped.txt Maintenance Policy

**Date:** 2026-08-24  
**Status:** Accepted  
**Authors:** Core maintainers  
**Related audit items:** F-001, PH-001, PH-002, PH-003 (audit-report)

## Context

The `PublicAPI.Shipped.txt` file in `EricksonLopez.DomainPrimitives.Abstractions` contains entries for several types that no longer exist in the current codebase:

- `DomainPrimitiveException` — renamed to `DomainPrimitiveValidationException` (now inherits from `ArgumentException`, not `Exception`). The old type had constructors `(string message)` and `(string primitiveName, string message)`.
- `ArithmeticPolicy` — renamed to `NumericOperations` in v1.0.0.
- `DomainRangeAttribute` / `RangeAttribute` — renamed to `PrimitiveRangeAttribute` to avoid collision with `System.ComponentModel.DataAnnotations.RangeAttribute`.

These "orphan entries" were preserved in `PublicAPI.Shipped.txt` without removal because:
1. Removing them from `PublicAPI.Shipped.txt` requires the RS0017 analyzer to agree that the API was intentionally removed.
2. Binary compatibility analysis (CRIT-003 CI gate) would flag their removal as a breaking change without a proper deprecation cycle.

## Decision

We **preserve** orphan entries in `PublicAPI.Shipped.txt` as historical record. We do **not** delete them silently. The policy is:

1. **Rename or delete with deprecation alias**: When a public type is renamed or removed, add an `[Obsolete(error: true, "Use X instead")]` alias in code.
2. **Update PublicAPI only via the Roslyn analyzer**: Never manually delete entries from `PublicAPI.Shipped.txt`.
3. **Breaking changes require a version bump**: Any type removal that is not accompanied by a deprecation alias constitutes a breaking change reserved for v2.0.0+.
4. **Document the reason**: Add a comment block above orphan entries explaining why they remain.

## Historical entries as of audit (2026-08-24)

| Entry | Former type | Current type | Reason retained |
|:------|:-----------|:-------------|:----------------|
| `DomainPrimitiveException.*` | `DomainPrimitiveException` | `DomainPrimitiveValidationException` | Renamed in v1.0.0 without alias |
| `ArithmeticPolicy.*` | `ArithmeticPolicy` | `NumericOperations` | Renamed in v1.0.0 |
| `DomainRangeAttribute.*` | `DomainRangeAttribute` | `PrimitiveRangeAttribute` | Renamed to avoid BCL collision |

## Consequences

### Positive
- `PublicAPI.Shipped.txt` serves as an explicit, auditable history of API surface changes.

### Negative
- Orphan entries can cause confusion during audits (as discovered in the 2026-08-24 audit).

### Action Required for v2.0.0+
Before the v2.0.0 release, remove orphan entries from `PublicAPI.Shipped.txt` using the RS0017 analyzer workflow or `[Obsolete(error: true)]` aliases.
