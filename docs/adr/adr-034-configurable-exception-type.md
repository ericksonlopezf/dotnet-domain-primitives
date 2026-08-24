# adr-034: Configurable Validation Exception Type and Roslyn Analyzer DP0017

## Status
Accepted

## Context
By default, failing a `Create()` validation check throws `DomainPrimitiveValidationException`. However, many organizations and Clean Architecture projects maintain their own base domain exception types (e.g., `DomainException` or `ValidationException`) that integrate directly with global exception filters, ProblemDetails factories, or logging middlewares.

## Decision
1. Add `ExceptionType` property to `DomainPrimitivesDefaultsAttribute`:
   ```csharp
   [assembly: DomainPrimitivesDefaults(ExceptionType = typeof(CustomDomainException))]
   ```
2. When specified, source generators emit `throw new CustomDomainException(error.Message);` instead of `DomainPrimitiveValidationException`.
3. Introduce Roslyn Analyzer **DP0017**:
   - Validates at compile-time that the configured `ExceptionType` inherits from `System.Exception`.
   - Validates that the exception type provides a public constructor taking a single `string` message parameter.
   - Emits a compile-time Error if either condition is not met.

## Consequences
### Positive
- Seamless integration with enterprise Clean Architecture exception handling hierarchies.
- Zero runtime overhead and compile-time validation via Roslyn analyzer DP0017.

### Negative
- Custom exceptions must provide a constructor accepting a string message.

### Known Limitation — Incompatibility with DomainPrimitiveValidationException
`DomainPrimitiveValidationException` itself **cannot** be used as `ExceptionType` because:
- `ExceptionType` contract requires a `public ctor(string message)` constructor (validated by DP0017).
- `DomainPrimitiveValidationException`'s constructor is `(PrimitiveError error, string paramName)` — not `(string message)`.

The generator emits `throw new CustomDomainException(error.Message)` which requires the `(string)` constructor.
`DomainPrimitiveValidationException` is the **default** exception type built into the generator's hot path and is not intended to be used as a configurable `ExceptionType`. Only custom exception types with the standard `(string message)` signature should be used with this feature.

```csharp
// ✅ Valid — custom exception with (string message) ctor
[assembly: DomainPrimitivesDefaults(ExceptionType = typeof(MyDomainException))]

// ❌ Invalid — DomainPrimitiveValidationException does not have (string message) ctor
// [assembly: DomainPrimitivesDefaults(ExceptionType = typeof(DomainPrimitiveValidationException))]
// → DP0017 compile-time error
```
