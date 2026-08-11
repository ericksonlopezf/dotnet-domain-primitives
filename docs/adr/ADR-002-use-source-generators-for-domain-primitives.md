# 2. Use Source Generators for Domain Primitives

Date: 2026-07-23

## Status

Accepted

## Context

Domain-Driven Design (DDD) encourages the use of Value Objects and Domain Primitives (e.g., `EmailAddress`, `UserId`) to encapsulate validation and business rules, preventing "Primitive Obsession".

Historically in .NET, implementing these required significant boilerplate:
- Implementing `IEquatable<T>`
- Implementing `IComparable<T>`
- Writing custom JSON converters.
- Writing EF Core Value Converters.
- Writing Dapper TypeHandlers.

To reduce boilerplate, some libraries use Reflection at runtime or base classes. However, base classes introduce heap allocations (if they are `class`) and Reflection introduces performance hits during serialization or database mapping.

## Decision

We will build the library entirely around **C# Source Generators** and `partial record struct`. 
The core package (`EricksonLopez.DomainPrimitives`) will only contain marker attributes (e.g., `[StrongId<T>]`).
A companion Source Generator package will emit the implementation of `IParsable<T>`, `IUtf8SpanFormattable`, JSON Converters, and validation logic at compile time.

## Consequences

* **Positive:** Zero runtime Reflection. Peak performance and zero heap allocations for the primitives (using `struct`).
* **Positive:** The domain model remains completely pure, free from infrastructure logic.
* **Negative:** Source Generators are notoriously difficult to debug and maintain. The barrier to entry for contributing to the core generator is high.
* **Negative:** The user's IDE experience is highly dependent on the Roslyn compiler cache. Sometimes developers will need to restart their IDE to see the generated code. We mitigate this by documenting the DX caveats in `CONTRIBUTING.md`.
