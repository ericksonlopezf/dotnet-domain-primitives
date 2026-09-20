# Migration and Version Evolution Guide

This guide details the steps required to migrate traditional DDD manual boilerplate or earlier iterations to the compile-time Source Generator architecture of `EricksonLopez.DomainPrimitives`.

---

## 1. Migrating from Manual DDD Classes to Source-Generated Structs

### Before: Verbose Manual DDD Class Implementation
```csharp
// ❌ Class incurring heap allocations, GC pressure, and extensive boilerplate
public sealed class CustomerEmail : IEquatable<CustomerEmail>
{
    public string Value { get; }

    private CustomerEmail(string value) => Value = value;

    public static CustomerEmail Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Email is empty");
        if (!value.Contains("@")) throw new ArgumentException("Invalid email");
        return new CustomerEmail(value.Trim().ToLowerInvariant());
    }

    public bool Equals(CustomerEmail? other) => other is not null && Value == other.Value;
    public override bool Equals(object? obj) => obj is CustomerEmail other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value;
}
```

### After: Compile-Time Generated Domain Primitive
```csharp
// ✅ readonly partial record struct: stack allocated, zero GC allocations, verified immutability
using EricksonLopez.DomainPrimitives;

[Email]
public readonly partial record struct CustomerEmail;
```

**Immediate Benefits:**
- Replaces 30+ lines of repetitive code with a single declarative declaration.
- Eliminates heap allocation entirely on creation.
- Automatically applies RFC 5321 syntax validation and normalization (`[Trim]`, `[LowerCase]`).
- Emits BCL interfaces (`IParsable<T>`, `ISpanParsable<T>`, `IUtf8SpanParsable<T>`), equality operators, and JSON converters at compile time.

---

## 2. Deprecated Features and Replacements

| Legacy Component / Attribute ⚠️ | Status | Official Replacement | Architectural Rationale |
|:---|:---|:---|:---|
| `[FluentValidation]` | **Removed** | `TryCreate` at application boundary | Decoupling domain invariants from third-party validation libraries (RFC-0004). Invariants are intrinsic to the primitive. |
| `EricksonLopez.DomainPrimitives.Mapster` | **Removed** | Native `explicit operator` | Scalar primitives emit explicit conversion operators. Object mappers (Mapster, Mapperly) map them natively without extra packages ([adr-043](adr/adr-043-discontinue-mapster-package.md)). |
| `[EFCore]` per-type attribute | **Deprecated** | `configurationBuilder.ConfigureDomainPrimitives()` | Centralized convention discovery in `ConfigureConventions` replaces per-type decorations. |
| `[Dapper]` per-type attribute | **Deprecated** | `DapperDomainPrimitivesRegistration.RegisterAll()` | Centralized, thread-safe batch registration at application startup in `Program.cs`. |
| `[AspNetCore]` per-type attribute | **Deprecated** | `services.AddDomainPrimitivesModelBinding()` | Global model binder provider in MVC and Minimal APIs. |
| `[OpenApi]` per-type attribute | **Deprecated** | `options.SchemaFilter<DomainPrimitivesSchemaFilter>()` | Compile-time unified schema filter for Swagger UI. |
| `DomainPrimitiveFormatException` | **Deprecated** | `System.FormatException` | Alignment with standard .NET BCL parsing interfaces (`IParsable<T>`, `ISpanParsable<T>`) per RFC-0003. |
| `ArithmeticPolicy` | **Deprecated** | `NumericOperations` | Standardized bitwise flags for numeric operator generation (AUDIT-CRIT-004). |

---

## 3. Handling Nulls in Dapper Persistence

When migrating from legacy persistence layers to Dapper with domain primitives:
- If a database column contains `NULL` but the corresponding C# property is non-nullable (`TPrimitive`), Dapper throws a `DataException`.
- **Action Required:** For optional database columns, explicitly declare the entity property as nullable:

```csharp
// If the database column allows NULL:
public CustomerId? CustomerId { get; set; }
```
