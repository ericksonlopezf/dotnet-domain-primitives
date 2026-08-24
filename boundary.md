# Architectural Boundary Specification: EricksonLopez.DomainPrimitives.Abstractions

## 1. Purpose
`EricksonLopez.DomainPrimitives.Abstractions` defines the zero-dependency foundational contracts and attributes for strongly-typed domain primitives, IDs, and base value objects in .NET 8 / 9 / 10.

## 2. Owns
- `IDomainPrimitive<TSelf>`, `IDomainPrimitive<TSelf, TValue>`.
- `IStrongId<TSelf, TValue>`, `IStrongId`.
- `ValueObject` abstract record base class for structural equality.
- Domain primitive code generation attributes (`[StrongId]`, `[StringPrimitive]`, `[NumericPrimitive]`, `[GuidPrimitive]`).
- `PrimitiveError` and `ICustomValidator` contracts.

## 3. Does Not Own
- Source generator implementations (`EricksonLopez.DomainPrimitives.Generators`).
- Rich domain value objects or fiscal models (`EricksonLopez.ValueObjects`).
- Aggregate roots or entities (`EricksonLopez.SharedKernel`).
- Dapper type handlers (`EricksonLopez.DomainPrimitives.Dapper`).
- EF Core value converters (`EricksonLopez.DomainPrimitives.EFCore`).

## 4. Allowed Dependencies
- **.NET BCL only**.
- **Zero** `EricksonLopez.*` package references.

## 5. Forbidden Dependencies
- `EricksonLopez.Result`, `EricksonLopez.Events.*`, `EricksonLopez.SharedKernel`.
- `Dapper`, `Microsoft.EntityFrameworkCore`, `Newtonsoft.Json`.
- `Microsoft.AspNetCore.*`.

## 6. Who Can Depend On It
- `EricksonLopez.DomainPrimitives` (L1).
- `EricksonLopez.ValueObjects` (L1).
- `EricksonLopez.SharedKernel` (L1).
- Adapters (`EricksonLopez.DomainPrimitives.Dapper`, `EricksonLopez.DomainPrimitives.EFCore`).

## 7. Public API Rules
- Static abstract interface members (CRTP pattern) must be trim-safe and AOT-compliant.
- Zero boxing of strongly-typed ID value types.

## 8. AOT Expectations
- `IsAotCompatible=true`.
- Zero dynamic code emission (`System.Reflection.Emit`).

## 9. Trimming Expectations
- `IsTrimmable=true`.

## 10. Provider Isolation
- 100% database-agnostic.

## 11. Testing Isolation
- Testing doubles and fixtures live in `EricksonLopez.DomainPrimitives.Testing`.
