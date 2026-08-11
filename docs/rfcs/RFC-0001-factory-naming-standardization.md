# RFC-0001: Factory Method Naming Standardization

> **Status:** Approved (post-hoc — ratified 2026-08-10)
> **Authors:** Erickson Lopez
> **Created:** 2026-07-15
> **Approved:** 2026-08-10
> **Implemented In:** v1.2.0 (Unreleased)
> **Replaces:** Previous `New()` and `From()` factory methods on StrongId<T>

---

## Problem Statement

The `StrongId<T>` type originally exposed two factory methods:

- `New()` — creates a new ID with a freshly generated value (e.g., `Guid.NewGuid()`)
- `From(T value)` — creates an ID from an existing value

This naming was inconsistent with:
1. **BCL conventions** — `int.Parse()`, `Guid.Parse()`, `DateTimeOffset.FromUnixTimeSeconds()` use `Parse` for conversion.
2. **The rest of the library** — `StringPrimitive`, `NumericPrimitive`, and `DatePrimitive` all use `Create()`.
3. **Discoverability** — `New()` was ambiguous ("new what?"). `From()` had no parallel in BCL.

## Decision

Rename all factory methods to follow the **Create/TryCreate/Parse/TryParse** pattern, per the
Engineering Specification v4.0 §API Surface Budget:

| Old Name | New Name | Notes |
|----------|----------|-------|
| `New()` | `Create()` | For Guid-backed IDs: generates a new Guid internally |
| `From(T value)` | `Create(T value)` | For int/long/string/Guid-backed IDs |
| *(no try variant)* | `TryCreate(T value, out TSelf, out PrimitiveError)` | New — aligns with TryCreate on all primitives |

## Migration Guide

```csharp
// Before (v1.1.0 and earlier)
var id1 = OrderId.New();           // Guid.NewGuid() based
var id2 = UserId.From(rawGuid);    // From existing value

// After (v1.2.0+)
var id1 = OrderId.Create();        // Same semantics, standardized name
var id2 = UserId.Create(rawGuid);  // Same semantics, standardized name

// New: TryCreate (avoids exception)
if (UserId.TryCreate(rawGuid, out var userId, out var error))
    // use userId
```

## Breaking Change Classification

| Type | Level |
|------|-------|
| Source compatibility | ❌ Breaking (compile error) |
| Binary compatibility | ❌ Breaking (method not found) |
| Behavioral | ✅ Identical |

## Mitigation

- A `[Obsolete]` shim for `New()` and `From()` was NOT added because the method signatures
  of `Create()` are identical in behavior — a find-and-replace migration is sufficient.
- The `[DP0016_InvalidFactoryMethodName]` analyzer was updated to flag any `New()` or `From()`
  methods on domain primitive types in consuming code.

## Votes

| Maintainer | Decision | Rationale |
|------------|----------|-----------|
| Erickson Lopez | +1 | Spec alignment |

*Note: This RFC was ratified post-implementation as part of the audit process. Future breaking changes
must complete RFC approval before implementation begins.*
