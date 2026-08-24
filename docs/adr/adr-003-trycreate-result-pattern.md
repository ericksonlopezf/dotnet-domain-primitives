# adr-003: TryCreate Result Pattern and BCL Consistency

**Date:** 2026-08-06  
**Status:** Superseded by [adr-006](adr-006-use-validation-error-instead-of-result.md) (2026-08-09)

> **Historical note:** This ADR documented the decision to retain a `Result<T> TryCreate(TValue)` overload alongside a BCL-compliant `bool TryCreate(...)` overload. ADR-006 superseded this decision — the `Result<T>` approach was rejected in favor of `PrimitiveError` + `out` parameter to avoid a `Result<T>` dependency. The code implements ADR-006's resolution exclusively. This document is preserved as architectural history.

## Context
The v3.0 audit specification strictly requires consistency with BCL conventions. The standard BCL `TryX` pattern uses a boolean return type with an `out` parameter (e.g., `bool TryParse(string value, out T result)`). 

However, the `TryCreate` factory method for domain primitives currently returns a `Result<T>` instead. 

Returning `Result<T>` has distinct advantages:
- It supports Railway-Oriented Programming, allowing validation to be easily composed in LINQ or async chains.
- It carries detailed error diagnostics (why the validation failed) without the boxing overhead of an `out string error` parameter.
- It avoids exceptions on the hot path entirely.

The conflict is that this deviation breaks the "pit of success" for a .NET developer who intuitively expects `bool TryCreate(TValue, out TSelf)`.

## Decision
We will **retain** the `Result<T> TryCreate(TValue)` method to preserve the benefits of Railway-Oriented Programming and detailed validation errors.

To achieve BCL compliance, we will **add** the standard BCL overload:
`static abstract bool TryCreate(TValue value, out TSelf result)`

Both factory methods will be generated for all domain primitives.

## Consequences
- **Positive:** Developers expecting standard BCL behavior can use the `out` parameter overload seamlessly.
- **Positive:** Advanced developers building composable pipelines can continue using the `Result<T>` overload.
- **Negative:** Slightly increases the API surface area of generated types (1 additional visible member). However, this is within the Simplicity Budget.
- **No breaking changes:** Adding the overload is an additive, binary-compatible change.
