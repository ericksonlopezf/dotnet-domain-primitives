# ADR-006: Use ValidationError over Result for Domain Primitives

**Date:** 2026-08-09  
**Status:** Accepted  

## Context
Previously (as noted in ADR-003), we considered returning `Result<T>` from factory methods (like `TryCreate`) in our domain primitives to enable Railway-Oriented Programming and transport detailed validation errors. 

However, using `Result<T>` within the core of the domain primitive generators introduces a significant architectural drawback: it creates tight coupling. If a domain primitive generator returns a specific `Result<T>` implementation, any consuming application or library is forced to take a dependency on that specific `Result` type. This violates the principle of loose coupling, as consumers may prefer different control flow mechanisms (like exceptions), or other result libraries (e.g., `FluentResults`, `LanguageExt`, or their own custom implementations).

Furthermore, the single responsibility of a Domain Primitive is to encapsulate a value and guarantee its validity according to domain rules. It is not its responsibility to dictate the application's architectural control flow.

## Decision
We will **not** use `Result<T>` in the generated domain primitives.

Instead, we will use `ValidationError` (or a simple `Error` struct) to represent validation failures, and rely on standard .NET BCL patterns for control flow:
- `Create(TValue value)`: Returns the domain primitive or throws a `DomainPrimitiveException` (or similar).
- `TryCreate(TValue value, out TSelf result, out ValidationError error)`: Returns a boolean indicating success, and uses `out` parameters to provide either the successfully created instance or the specific validation error.

## Consequences
- **Positive (Loose Coupling):** The domain primitives library remains entirely agnostic to the application's control flow mechanism. Consuming applications can easily wrap these primitives into their own `Result<T>` types if desired, without facing dependency conflicts or being forced to adopt our `Result` implementation.
- **Positive (Single Responsibility):** The primitives focus solely on data encapsulation and enforcing validation rules. `ValidationError` acts purely as a descriptive data structure of what went wrong, rather than a control flow monad.
- **Positive (BCL Consistency):** The generated code strictly follows standard C# / .NET idioms (the Try-Parse pattern).
- **Negative:** Developers using Railway-Oriented Programming must map the `TryCreate` boolean pattern to their preferred `Result` type in the application layer, slightly increasing boilerplate at the boundary where primitives are constructed.
