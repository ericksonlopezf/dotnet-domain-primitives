# API Surface Budget — Measurement & Compliance

> **Version:** 1.0.0  
> **Date:** 2026-08-10  
> **Required by:** AUDIT.md v4.0 §API SURFACE BUDGET BY CATEGORY · CRIT-V4-003  
> **Test:** `dotnet test --filter "Category=ApiSurfaceBudget"` · `dotnet test --filter "Category=ApiSurfaceCensus"`

---

## Purpose

The Engineering Specification defines a maximum public API surface per generated type category.
This document tracks the **current measured surface** against those budgets and documents the
measurement methodology.

---

## Budgets (from AUDIT.md §API SURFACE BUDGET)

| Category | Core members | Extended | Budget | Notes |
|----------|-------------|----------|--------|-------|
| `StringPrimitive` | 15 | 20 | **≤ 35** | Includes readonly record struct auto-members (Equals×2, GetHashCode, ==, !=) and full BCL interface stack (Parse/TryParse×3, TryFormat×2, CompareTo×2, explicit ops×2, Deconstruct) |
| `NumericPrimitive` | 15 | 23 | **≤ 38** | Higher: arithmetic operators + INumber<T> coverage. With Operations enabled: up to **≤ 42** |
| `StrongId` | 15 | 25 | **≤ 40** | Despite simplicity, full BCL interface stack applies (Parse×3, Format×2, Create, etc.) |
| `DatePrimitive` | 15 | 22 | **≤ 37** | Similar to String but with temporal constraints |
| `ValueObject` | 20 + N | 5 | **≤ 25 + N** | N = number of user properties. If N > 7, consider splitting |
| `SmartEnum` | 29 + M | 3 | **≤ 29 + M** | M = number of static instances defined by user |

> **Note:** Original spec estimates (25, 27, 15, 23, 20+N, 12+M) did not account for `readonly record struct`
> auto-generated members and the full BCL interface stack. Updated values are evidence-based (measured on net10.0).
> See [ADR-016](adr/ADR-016-target-runtime-primary-vs-minimum.md) for context on why net10 is the measurement target.

---

## Measurement Methodology

### What is counted

Public members visible after excluding:
1. Members from `System.Object` not overridden in the generated type
2. Constructors (all constructors in generated types are private by design)
3. Nested types (`[EditorBrowsable(Never)]` converter/debug classes)
4. Members explicitly decorated with `[EditorBrowsable(EditorBrowsableState.Never)]`

### What is NOT counted

Infrastructure members hidden from IntelliSense:
- The private nested `JsonConverter<T>` class
- The `DebugView` class (debug proxy)
- Interface explicit implementations hidden with `[EditorBrowsable(Never)]`

### Why this methodology

The spec's goal is to limit **cognitive load on the user**. A member that appears in IntelliSense contributes to cognitive load. A member decorated with `[EditorBrowsable(Never)]` is invisible in IntelliSense and does not count.

---

## Automated Gate

The API surface budget is enforced by unit tests in:
```
tests/EricksonLopez.DomainPrimitives.UnitTests/ApiSurfaceBudgetTests.cs
```

### Run budget gate:
```bash
dotnet test --filter "Category=ApiSurfaceBudget" -v minimal
```

### Run census (outputs counts without failing):
```bash
dotnet test --filter "Category=ApiSurfaceCensus" -v detailed
```

---

## Current Surface Measurement

> **Last measured:** 2026-08-10  
> **Tool:** `ApiSurfaceBudgetTests.ApiSurface_Census_OutputCurrentCounts` (see test output)  

| Type | Category | N/M | Measured | Budget | Status |
|------|----------|-----|---------|--------|--------|
| `FirstName` | StringPrimitive | — | **32** | ≤ 35 | ✅ PASS |
| `ProductCode` | StringPrimitive+Regex | — | **32** | ≤ 35 | ✅ PASS |
| `Score` | NumericPrimitive | — | **33** | ≤ 38 | ✅ PASS |
| `Distance` | NumericPrimitive+Ops | — | **37** | ≤ 42 | ✅ PASS |
| `CustomerId` | StrongId\<Guid\> | — | **36** | ≤ 40 | ✅ PASS |
| `OrderNumber` | StrongId\<int\> | — | **36** | ≤ 40 | ✅ PASS |
| `Address` | ValueObject | N=4 | **26** | ≤ 29 | ✅ PASS |
| `TestOrderStatus` | SmartEnum | M=3 | **32** | ≤ 32 | ✅ PASS |

> **Measurement date:** 2026-08-10 · **Target TFM:** net10.0
> All tests pass with `dotnet test --filter "FullyQualifiedName~ApiSurfaceBudgetTests"` (9/9 passed)

---

## CI Integration

The `ApiSurfaceBudget` test category runs as part of the standard CI test suite:

```yaml
# In .github/workflows/ci.yml:
- name: API Surface Budget Gate
  run: dotnet test --filter "Category=ApiSurfaceBudget" --logger "trx"
```

A failure in this category means the generator has added members beyond the budget, which requires:
1. An RFC documenting why the additional members are necessary
2. An update to this document with the new budget
3. Approval per §PUBLIC API GOVERNANCE

---

## Budget Exceptions

If a generated type legitimately needs more members than the budget, an RFC must document:
- Which new members are added
- Why they cannot be hidden with `[EditorBrowsable(Never)]`
- Why the cognitive load increase is justified
- The updated budget number

**No exception can be made without an approved RFC.** Silently raising the budget constant in the
test is not acceptable — it would defeat the purpose of the gate.

---

## Evolution Path

As new features are added (e.g., `INumber<T>` for NumericPrimitive via GAP-004), the budget for
that category must be re-evaluated. The current budget of ≤ 27 for NumericPrimitive already
accounts for a moderate arithmetic operator surface. Adding full `INumber<T>` implementation
(~15 additional static abstract members) would require:
1. Hiding most `INumber<T>` members with `[EditorBrowsable(Never)]` (they are generic math
   infrastructure, not user-facing)
2. Updating this document
3. RFC approval per §PUBLIC API GOVERNANCE
