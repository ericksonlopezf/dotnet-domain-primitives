# RFC-0003: FormatException Standardization (Deprecation of DomainPrimitiveFormatException)

> **Status:** Approved (post-hoc — ratified 2026-08-10)
> **Authors:** Erickson Lopez
> **Created:** 2026-07-22
> **Approved:** 2026-08-10
> **Implemented In:** v1.2.0 (Unreleased)
> **Relates To:** CRIT-001, ADR-008

---

## Problem Statement

The library originally threw `DomainPrimitiveFormatException` (a custom exception type) from
`Parse()` methods. This was a deliberate design choice to provide additional context
(`PrimitiveName` property). However, it introduced two problems:

1. **BCL Inconsistency:** The .NET BCL throws `FormatException` from all `Parse()` methods
   (`int.Parse()`, `Guid.Parse()`, `DateTime.Parse()`, etc.). Consumers who write:
   ```csharp
   try { var id = MyId.Parse(input); }
   catch (FormatException) { ... }
   ```
   would silently MISS the exception in v1.1.0 because `DomainPrimitiveFormatException`
   derives from `FormatException` BUT catch clauses check the exact runtime type by default in
   some frameworks. More importantly, the non-BCL name violated the discoverability principle.

2. **Type proliferation:** A custom exception type adds to the API surface budget unnecessarily.
   The `PrimitiveName` context can be included in the exception `Message` instead.

## Decision

1. `Parse()` now throws **`System.FormatException`** directly.
2. The `Message` includes the primitive type name for context: `"The value '...' is not valid for {TypeName}."`.
3. `DomainPrimitiveFormatException` is **deprecated** with `[Obsolete(..., error: false)]` and
   `[EditorBrowsable(EditorBrowsableState.Never)]`. It will emit a compile-time warning when used.
4. `DomainPrimitiveFormatException` will be **removed** (made error-level obsolete) in v2.0,
   and **deleted** in v3.0.

## Deprecation Timeline

| Version | Action |
|---------|--------|
| v1.2.0 | `[Obsolete(error: false)]` + `[EditorBrowsable(Never)]` — compile warning only |
| v2.0.0 | `[Obsolete(error: true)]` — compile error for any code still catching it |
| v3.0.0 | Type deleted from assembly |

## Migration Guide

```csharp
// Before (v1.1.0)
try
{
    var primitive = MyPrimitive.Parse(input);
}
catch (DomainPrimitiveFormatException ex)
{
    // Handle: ex.PrimitiveName, ex.Message
}

// After (v1.2.0+)
try
{
    var primitive = MyPrimitive.Parse(input);
}
catch (FormatException ex)
{
    // ex.Message now contains: "The value '...' is not valid for MyPrimitive."
    // Use TryParse for non-exception control flow (preferred)
}

// Preferred: Use TryParse to avoid exception-based control flow entirely
if (!MyPrimitive.TryParse(input, null, out var result))
{
    // Handle invalid input
}
```

## Breaking Change Classification

| Type | Level |
|------|-------|
| Source compatibility | ⚠️ Warning-breaking (existing `catch (DomainPrimitiveFormatException)` still compiles but warns) |
| Binary compatibility | ✅ Compatible (deprecated type still exists in assembly) |
| Behavioral | ⚠️ Partially breaking (catch (FormatException) now correctly catches, catch (DomainPrimitiveFormatException) no longer fires) |

## Votes

| Maintainer | Decision | Rationale |
|------------|----------|-----------|
| Erickson Lopez | +1 | BCL consistency — Parse should throw FormatException |

*Note: This RFC was ratified post-implementation as part of the audit process.*
