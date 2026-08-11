# ADR-010: System.Text.Json Integration via Inline Source Generation

## Status
Accepted

## Context
The v4.0 Specification stipulates that `EricksonLopez.DomainPrimitives.Json` must not exist as a separate package. STJ is part of the Base Class Library, meaning domain primitives can natively integrate STJ serialization attributes and converters without external dependencies.

## Decision
We have deleted the `EricksonLopez.DomainPrimitives.Json` package. The Core Source Generators have been updated to emit a private, inline `JsonConverter<T>` inside the generated file for each primitive type, and the primitive itself is decorated with `[JsonConverter(typeof(TJsonConverter))]`.

## Consequences
- **Positive**: Consumers get zero-configuration JSON serialization/deserialization for all primitives out-of-the-box.
- **Positive**: We reduce the package ecosystem surface area by one, centralizing generation logic to a single analyzer.
- **Positive**: Performance is enhanced as serializers do not need to rely on runtime reflection to dynamically resolve generic converters.
