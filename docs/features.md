# Features Catalog & Specifications

---

## 1. Package Inventory & Core Types

### 1. `EricksonLopez.DomainPrimitives`
- `[DomainPrimitive<T>]`: Roslyn marker attribute for scalar value types.
- `SmartEnum<TEnum, TValue>`: Base class for type-safe polymorphic enums.
- `SmartFlagEnum<TEnum, TValue>`: Base class for bitwise flag smart enums.

### 2. Satellite Packages
- `AspNetCore`: Minimal APIs and model binding integration.
- `EFCore`: Entity Framework Core value conversions.
- `Dapper`: Dapper type handlers.
- `OpenApi`: OpenAPI schema filters and generators.
- `Testing`: Fluent testing assertion extensions.
- `NewtonsoftJson`: Legacy JSON converters.
