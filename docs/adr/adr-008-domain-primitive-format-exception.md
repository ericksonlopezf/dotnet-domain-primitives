# adr-007: Parse() Throws DomainPrimitiveFormatException

## Status
Accepted

## Context
The v4.0 specification states that `Parse()` must throw a standard `FormatException` when parsing fails, to remain perfectly consistent with BCL types like `int.Parse()` and `Guid.Parse()`.
However, during implementation, we created a subclass `DomainPrimitiveFormatException : FormatException`. This subclass allows consumers to capture the `PrimitiveName` dynamically without parsing the message string.

## Decision
We will retain `DomainPrimitiveFormatException` and the source generators will continue to throw this subclass from `Parse()` methods.

## Consequences
- **Positive**: Consumers can use `catch (DomainPrimitiveFormatException ex)` to identify exactly which domain primitive failed parsing, thanks to the `PrimitiveName` property.
- **Positive**: Backward compatibility and BCL consistency are maintained because `catch (FormatException)` will still successfully intercept the subclass, meaning consumers expecting standard BCL behavior are unaffected.
- **Negative**: Slight deviation from the literal wording of the v4.0 spec, which we resolve via this ADR.
