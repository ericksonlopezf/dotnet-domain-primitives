# adr-042: ValueObject Dual Paradigm — Abstract Base Class vs Source-Generated Attribute

**Date:** 2026-08-24  
**Status:** Accepted  
**Authors:** Core maintainers  
**Related audit items:** F-015, MIS-003, ADR-040 (audit-report)

## Context

The library provides **two distinct mechanisms** both named `ValueObject`, which can cause confusion:

1. **`ValueObject` abstract base class** (`abstract record class ValueObject`) — for multi-property value objects that are **reference types** (heap-allocated, C# record inheritance).
2. **`[ValueObject]` attribute** (`SmartEnumAttribute`-style marker) — triggers source generation of a `readonly partial record struct` that is an **allocation-free, AOT-safe value object**.

This duality was established in ADR-040 (Dual Paradigm) but the specific distinction was not documented in the API reference.

## Decision

Both paradigms are intentionally retained. The decision matrix for which to use:

| Criterion | Use `ValueObject` base class | Use `[ValueObject]` attribute |
|:----------|:-------------------------------|:--------------------------------|
| Memory model | Reference type (heap, GC) | Value type (stack, zero-alloc) |
| Inheritance | Required (`record` inheritance) | Not allowed (struct) |
| AOT/Trimming | Yes | Yes |
| Nullable support | Reference semantics | Struct default = uninitialized |
| Use case | Multi-property DDD value objects (`Money`, `Address`) that need polymorphism | Domain primitives that are single or simple multi-property structs |

## Examples

`csharp
// Option A: ValueObject base class (reference type)
// Use when: the value object has multiple properties and may be polymorphic
public sealed record Money(decimal Amount, string Currency) : ValueObject;
public sealed record Address(string Street, string City, string PostalCode) : ValueObject;

// Option B: [ValueObject] attribute (source-generated struct)  
// Use when: AOT safety, zero allocation, and struct semantics are required
[ValueObject]
public readonly partial record struct GeoCoordinate;
`

## Consequences

### Positive
- Developers have a clear mental model for when to use each paradigm.
- Both paradigms are documented in `docs/api-reference.md` with examples.

### Negative
- Two paradigms add conceptual surface area. The naming overlap (`ValueObject` class vs `[ValueObject]` attribute) is intentional but requires documentation.

### Documentation
The distinction is captured in `docs/api-reference.md` §ValueObject (Abstract Base Class) section.
