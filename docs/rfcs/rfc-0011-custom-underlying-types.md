# rfc-0011: Custom Underlying Types Support

**Status:** Proposed / Research  
**Author:** Core Maintainers  
**Date:** 2026-08-19  
**Target:** v2.0.0  
**Related:** GAP-01 (AUDITORIA_PARIDAD_FUNCIONAL.md §11, §20, §23)  

---

## Problem Statement

Currently, `EricksonLopez.DomainPrimitives` supports specific backing primitive types:
- `StringPrimitive`: `System.String`
- `NumericPrimitive`: `int`, `long`, `short`, `byte`, `decimal`, `double`, `float`
- `StrongId`: `Guid`, `int`, `long`, `string`
- `DatePrimitive`: `DateOnly`, `DateTime`
- `SmartEnum`: `int`, `long`, `string`, `short`, `byte`, `Guid`

Competitors like `Vogen` allow arbitrary generic backing types (e.g. `[ValueObject<NodaTime.LocalDate>]`, `[ValueObject<Ulid>]`, `[ValueObject<MongoDB.Bson.ObjectId>]`). Users with non-BCL primitive types cannot currently use DomainPrimitives without wrapping them inside a multi-property `[ValueObject]`.

---

## Proposed Design

Introduce generic underlying type support via `[DomainPrimitive<TValue>]` or extending existing attributes:

```csharp
// Example with third-party primitive types:
[StrongId<Ulid>]
public readonly partial record struct OrderId;

[DomainPrimitive<NodaTime.LocalDate>]
public readonly partial record struct BillingPeriodStart;
```

### Generic Constraints and Capabilities Pipeline

To preserve BCL parity and zero-allocation requirements, the generator uses Roslyn semantic model inspections:

1. **Equality & Hashing:**
   - If `TValue` implements `IEquatable<TValue>`, delegate to `Value.Equals(...)`.
   - Default record struct structural equality as fallback.

2. **Parsing Pipeline:**
   - If `TValue` implements `IParsable<TValue>`, generate `IParsable<TSelf>` delegating to `TValue.Parse / TryParse`.
   - If `TValue` implements `ISpanParsable<TValue>`, generate `ISpanParsable<TSelf>`.
   - If `TValue` implements `IUtf8SpanParsable<TValue>`, generate `IUtf8SpanParsable<TSelf>`.

3. **Formatting Pipeline:**
   - If `TValue` implements `ISpanFormattable`, delegate `TryFormat(Span<char>, ...)`.
   - If `TValue` implements `IUtf8SpanFormattable`, delegate `TryFormat(Span<byte>, ...)`.

4. **Serialization (System.Text.Json):**
   - Automatically delegate to `JsonSerializerOptions.GetConverter(typeof(TValue))` or inline serializer calls.

---

## Breaking Change Evaluation

- Adding generic overloads or generic attributes is purely **additive** (non-breaking).
- Fully compatible with existing `[StrongId<Guid>]`, `[StringPrimitive]`, etc.

---

## Implementation Roadmap

- **Phase 1 (v1.2.0):** Publish rfc-0011 and prototype generator inspection logic for `IParsable<TValue>`.
- **Phase 2 (v2.0.0):** Ship `[StrongId<TValue>]` and `[DomainPrimitive<TValue>]` with full BCL conditional generator pipelines.
