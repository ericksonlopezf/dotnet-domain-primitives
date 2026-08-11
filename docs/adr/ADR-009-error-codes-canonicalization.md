# ADR 008: Error Codes Canonicalization

## Status
Accepted

## Context
Across different domain primitive generators (`String`, `Numeric`, `Date`, etc.), error codes for validation failures were historically inconsistent and often hardcoded per primitive (e.g., using the primitive name instead of a generic violation type).

## Decision
We canonicalize all internal string error codes across all primitive generators to exactly match the v4.0 Specification:
- `NULL_INPUT`: Value is unexpectedly null.
- `EMPTY`: The primitive value is empty.
- `LENGTH`: Length bounds violated (too long or too short).
- `FORMAT`: General parsing or regex validation failure.
- `RANGE`: Value falls out of acceptable scalar bounds.
- `TEMPORAL`: Value falls outside temporal constraints (e.g., Past/Future).
- `INVARIANT`: A custom `Must()` rule validation failed.

## Consequences
- Test assertions can robustly target generic error types instead of primitive-specific strings.
- Exception filtering based on `PrimitiveError.Code` is now consistent and safe for clients handling polymorphic domain primitives.
- Code generation complexity is slightly streamlined.
