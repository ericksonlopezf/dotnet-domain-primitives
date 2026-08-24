# adr-039: Abstractions.Net8 Multi-Targeting Strategy

**Date:** 2026-08-19  
**Status:** Accepted (Implementation planned for v2.0.0 — package does not exist in v1.x)  
**Authors:** Core maintainers  
**Related audit items:** Section 13 (AUDITORIA_PARIDAD_FUNCIONAL.md §13, §19, §23)  

---

## Context

`EricksonLopez.DomainPrimitives.Abstractions` targets `netstandard2.0` to achieve maximum reach across .NET ecosystems without forcing minimum modern runtime dependencies onto consumer domain projects.

However, modern high-performance BCL interfaces introduced in .NET 8+:
- `IUtf8SpanParsable<TSelf>`
- `IUtf8SpanFormattable`

do not exist in `netstandard2.0`. Currently, these interfaces are generated directly onto the concrete domain primitive types by `EricksonLopez.DomainPrimitives.Generators` under `#if NET8_0_OR_GREATER` preprocessor directives.

### Tension
While the concrete types implement these interfaces at compile-time in .NET 8+ projects, generic abstractions such as:
```csharp
public void ProcessPrimitive<T>(T primitive) where T : IDomainPrimitive<T>
```
cannot express constraints like `where T : IUtf8SpanParsable<T>` through the `IDomainPrimitive` contract in `Abstractions` because `netstandard2.0` cannot reference those interfaces.

---

## Decision

We adopt a two-tier strategy:

1. **Keep Core `Abstractions` on `netstandard2.0` with Zero Dependencies:**
   - Preserves universal portability for pure domain models.
   - All standard contracts (`IDomainPrimitive<TSelf>`, `IDomainPrimitive<TSelf, TValue>`, `IStrongId<TSelf, TValue>`, `PrimitiveError`, validation attributes) remain in `EricksonLopez.DomainPrimitives.Abstractions`.

2. **Introduce `EricksonLopez.DomainPrimitives.Abstractions.Net8` (Planned for v2.0.0):**
   - Targets `net8.0;net9.0;net10.0`.
   - Defines extended interfaces:
     ```csharp
     namespace EricksonLopez.DomainPrimitives;

     public interface IDomainPrimitiveNet8<TSelf> : 
         IDomainPrimitive<TSelf>,
         IUtf8SpanParsable<TSelf>,
         IUtf8SpanFormattable
         where TSelf : IDomainPrimitiveNet8<TSelf>
     {
     }
     ```
   - Allows framework authors and high-throughput pipeline components to place generic constraints on UTF-8 span parsable primitives.

---

## Consequences

- **Positive:** Preserves backward compatibility and lightweight domain project references.
- **Positive:** Provides first-class generic typing for .NET 8+ ASP.NET Core Minimal APIs, HybridCache, and JSON source generators.
- **Negative:** Introduces an additional optional package for consumers requiring high-performance generic constraints.
