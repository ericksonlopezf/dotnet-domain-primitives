# ADR-004: Factory Method Naming for Strong IDs

**Date:** 2026-08-06  
**Status:** Accepted  

## Context
The v3.0 audit specification strictly requires consistency with BCL conventions. According to the Framework Design Guidelines, factory methods should use the verb `Create` (which throws on validation failure) or `TryCreate` (which returns a boolean or result type). The verb `From` is only acceptable if there is no validation (a pure conversion).

Previously, `StrongId<T>` generated the factory method `New()`. While `New()` is occasionally used in the BCL (e.g., `Guid.NewGuid()`), it is not standard practice for type construction (which generally prefers `Create`).

## Decision
We will rename the `New()` factory method to `Create()` for all domain primitives, including `StrongId<T>`.

## Consequences
- **Positive:** Consistency with BCL patterns and with other domain primitives (which already use `Create()`).
- **Negative:** This is a binary-breaking change for consumers of `StrongId<T>` who are currently using `New()`. However, since the library is still in a pre-GA phase (v1.x), making the breaking change now is acceptable to ensure long-term stability and API consistency.
