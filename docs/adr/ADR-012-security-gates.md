# ADR 010: Security Gates & Validation Performance

## Status
Accepted

## Context
String parsing and validation present vectors for performance degradation or DoS attacks (e.g. runaway regex, large string allocations, or stack explosion).

## Decision
We enforce strict "Security Gates" (SEC-001 through SEC-006) on all generated string primitives:
- **SEC-001**: Apply an implicit maximum length of 4096 characters to any `[StringPrimitive]` if `[Length]` or `[MaxLength]` are not provided.
- **SEC-002**: Force `RegexOptions.NonBacktracking` on .NET 7+ for all regex validations.
- **SEC-003**: Reduce regex validation timeout limits from 1000ms to a much tighter 100ms.
- **SEC-004**: Enforce `NormalizationForm.FormC` on any string primitive that mutates input via trimming or casing.
- **SEC-006**: Reduce `stackalloc` limits during `Parse(ReadOnlySpan<byte>)` parsing from 512 characters to 128 to prevent runaway stack growth.

## Consequences
- Significantly hardens domain primitives against DoS and untrusted input.
- Applications depending on primitives >4096 characters without explicitly configuring it via attributes will experience new validation errors.
