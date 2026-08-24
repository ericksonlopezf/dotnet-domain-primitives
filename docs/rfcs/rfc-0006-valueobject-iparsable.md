# rfc-0006: ValueObject `IParsable<T>`, `ISpanParsable<T>`, `IUtf8SpanParsable<T>`, and `IUtf8SpanFormattable` Support

**Status:** Approved / Implemented  
**Author:** Audit v4.0  
**Date:** 2026-08-10 (Approved: 2026-08-19)  
**Target:** v2.0.0 (Pre-implemented in v1.2.0 for BCL parity)  
**Related:** TD-006, TD-013, AUDITORIA_PARIDAD_FUNCIONAL.md §19  

---

## Problem

`ValueObject` was the only domain primitive category that did not implement `IParsable<T>`, `ISpanParsable<T>`, `IUtf8SpanParsable<T>`, or `IUtf8SpanFormattable`. All other categories implemented the full BCL interface stack. This:

1. Made `ValueObject` incompatible with generic `where T : IParsable<T>` constraints.
2. Prevented use in ASP.NET Core route/query parameter model binding without custom converters.
3. Created an inconsistency in BCL coverage compared to StringPrimitive, NumericPrimitive, and StrongId.

---

## Design

`ValueObjectGenerator` generates complete, zero-reflection implementations:

1. **Interfaces Declared:**
   - `IParsable<TSelf>`
   - `ISpanParsable<TSelf>`
   - `IUtf8SpanParsable<TSelf>` (under `#if NET8_0_OR_GREATER`)
   - `IUtf8SpanFormattable` (under `#if NET8_0_OR_GREATER`)

2. **Parsing Behavior:**
   - Standard JSON-based parsing via `JsonSerializer.Deserialize` / `Utf8JsonReader` which matches composite properties accurately.
   - Non-throwing `TryParse` variants (string, `ReadOnlySpan<char>`, `ReadOnlySpan<byte>`) that return `bool` with zero uncaught exceptions.
   - Validation hook invocation (`Validate`) post-deserialization to guarantee cross-property domain invariants.

3. **Formatting Behavior:**
   - `ISpanFormattable.TryFormat(Span<char>, ...)`
   - `IUtf8SpanFormattable.TryFormat(Span<byte>, ...)` (NET8+)

---

## Acceptance Criteria

- [x] `[ValueObject]` types declare `IParsable<TSelf>`, `ISpanParsable<TSelf>`, `IUtf8SpanParsable<TSelf>`, `IUtf8SpanFormattable`.
- [x] Unit tests verify string, Span<char>, and UTF-8 byte parsing and formatting on composite value objects (`Address`).
- [x] ApiSurfaceBudgetTests updated to account for new BCL methods.

