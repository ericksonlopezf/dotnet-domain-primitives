# ADR 007: Zero-Allocation Error Model

## Status
Accepted

## Context
The previous implementation of the library relied heavily on the `EricksonLopez.Result` package and allocated objects on the heap for validation errors in `TryCreate` workflows, creating unacceptable overhead in high-performance or hot-path scenarios.

## Decision
We will completely migrate away from generic exceptions and the `EricksonLopez.Result` package in the parsing pipelines.
- Use a strictly struct-based error model via `PrimitiveError` (readonly record struct).
- Generators will implement `bool TryCreate(TValue value, out TPrimitive result, out PrimitiveError validationError)`.
- Generic exceptions have been replaced with `DomainPrimitiveValidationException`, strictly accepting a `PrimitiveError` to enforce strong typing.

## Consequences
- Significant performance improvements and elimination of allocations on the happy path.
- The external dependency on `EricksonLopez.Result` is removed from `EricksonLopez.DomainPrimitives.Abstractions`.
- A breaking change for users expecting `Result<T>` from `TryCreate`.
