# ADR 009: Strict BCL Alignment

## Status
Accepted

## Context
Our primitives aim to act as transparent, native types within the C# ecosystem. Some APIs deviated from standard BCL expectations, causing friction for adoption and unexpected side-effects (e.g. relying on an external JSON library or default `IFormatProvider`).

## Decision
- Serialize automatically via `System.Text.Json.Serialization.JsonConverter<T>` injected directly into the generated primitive namespace, completely eliminating `EricksonLopez.DomainPrimitives.Json`.
- Strip the default `IFormatProvider? provider = null` from all `StringPrimitiveGenerator` parsing methods to mirror standard .NET primitives. Users must provide culture explicitly or rely on standard overloads.
- Tag utility types such as `PrimitiveBuilder<T>` with `[EditorBrowsable(EditorBrowsableState.Never)]` to hide them from normal IntelliSense unless the developer is fully aware.
- Maintain a netstandard2.0 target on the `Abstractions` package to ensure broad compatibility.

## Consequences
- Better ergonomics and native feeling for end users.
- Reduced external dependencies.
- Potentially breaking for users who implicitly relied on the default format provider injection or the external JSON package.
