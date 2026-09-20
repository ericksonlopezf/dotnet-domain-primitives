# Level 0 — Conceptual Overview

## What is EricksonLopez.DomainPrimitives?

It is a framework powered by Roslyn **Source Generators** to declare **strongly-typed, immutable, and self-validating domain primitives** in .NET with **zero runtime reflection**.

A **Domain Primitive** is a scalar value object that guarantees that any instantiated instance in memory demonstrably adheres to all its declared invariants and business rules.

---

## What Problem Does It Solve?

Conventional software architectures frequently suffer from **Primitive Obsession**: using raw BCL types such as `string`, `int`, `decimal`, or `Guid` to model rich business concepts.

```csharp
// ❌ Primitive Obsession: High defect probability and fragmented validation
void ProcessOrder(string email, decimal price, Guid customerId, Guid orderId)
{
    // Is the email valid? Is the price negative? Were customerId and orderId transposed?
}

// ✅ Domain Primitives: Types guarantee compile-time and runtime correctness
void ProcessOrder(CustomerEmail email, OrderPrice price, CustomerId customerId, OrderId orderId)
{
    // Transposing customerId and orderId causes a compiler error CS1503.
    // If email and price arrived here, they are demonstrably and mathematically valid.
}
```

| Problem | DomainPrimitives Solution |
|---|---|
| Accidentally swapping `OrderId` and `CustomerId` | Prevented at compile time; they are distinct nominal types. |
| Invalid or un-normalized emails in storage | `[Email]` normalizes casing/whitespace and validates RFC 5321 prior to instantiation. |
| Negative monetary amounts | `[Money]` / `[Price]` strictly reject negative values. |
| Fragmented validation rules across layers | Invariants live within the type itself; zero duplication in controllers, services, or DB scripts. |
| Memory overhead and Garbage Collection (GC) churn | Primitives are `readonly record struct` instances allocating 0 bytes on the heap. |

---

## Why Does It Exist?

1. **Alignment with Domain-Driven Design (DDD)**: Simplifies creating clean Value Objects without writing hundreds of lines of repetitive boilerplate for every domain concept.
2. **Zero-Reflection Code Generation (NativeAOT)**: Traditional alternatives rely on reflection, dynamic serializers, or IL emission incompatible with NativeAOT. `EricksonLopez.DomainPrimitives` generates all serialization, conversion, and binding code at compile time.
3. **Total Safety at System Perimeters**: Rebuilding types at application boundaries (HTTP, messaging, persistence) utilizes `TryCreate` without throwing exceptions, maximizing throughput and reliability.

---

## Advantages and Trade-offs

### Advantages
- ✅ **Zero Heap Allocations**: Implemented using `readonly partial record struct`.
- ✅ **100% NativeAOT Compatible**: Zero calls to `System.Reflection` or runtime dynamic code emission.
- ✅ **Automated Ecosystem Integration**: Dedicated source generators for EF Core, Dapper, ASP.NET Core, Swagger OpenAPI, and Newtonsoft.Json.
- ✅ **IDE Design Diagnostics**: Roslyn Analyzers (DP0001–DP0018) flag architectural errors in real time.

### Trade-offs
- ⚠️ Requires partial type syntax (`readonly partial record struct`).
- ⚠️ Requires C# source generator compilation (.NET 8.0+).
- ⚠️ Stateful, evolving entities with mutable identity must be modeled as Aggregate Roots, not value primitives.

---

## Comparison with Alternatives

| Feature | EricksonLopez.DomainPrimitives | Vogen | StronglyTypedId | Manual DDD Implementation |
|---|---|---|---|---|
| **Underlying Type** | `readonly record struct` | `struct` / `class` | `struct` | `class` / `record` |
| **Mathematical Operator Generation** | Yes (`NumericOperations`) | No | No | Manual |
| **Pre-Validation Normalization** | Yes (`[Trim]`, `[LowerCase]`, `INormalizer`) | No | No | Manual |
| **Semantic Shortcuts (Email, Phone, IBAN)** | Yes (38+ shortcuts) | No | No | Manual |
| **Zero Allocation on Error (`PrimitiveError`)** | Yes (immutable struct) | Throws exception | N/A | Manual |
| **EF Core / Dapper / OpenAPI Generators** | Yes (Automated Source Generators) | Partial | No | Manual |
| **NativeAOT Ready** | 100% Verified | Yes | Yes | Variable |
