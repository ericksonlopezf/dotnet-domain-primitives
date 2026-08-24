# Architectural Boundary Specification: EricksonLopez.DomainPrimitives.Abstractions

> This document defines the architectural contract of the `Abstractions` package — the zero-dependency foundational layer of the entire ecosystem.

---

## 1. Purpose

`EricksonLopez.DomainPrimitives.Abstractions` defines the zero-dependency foundational contracts and attributes for strongly-typed domain primitives in .NET. It provides the marker interfaces, type declaration attributes, validation attributes, and normalization attributes that all other packages in the ecosystem depend on.

**Target Frameworks:** `netstandard2.0`, `net8.0`, `net9.0`, `net10.0`  
**NativeAOT Compatible:** `true` (on .NET 8+)  
**Trimmable:** `true` (on .NET 8+)  
**External Dependencies:** **None** (pure BCL)

---

## 2. Owns

### Marker Interfaces

| Type | Namespace | Purpose |
|------|-----------|---------|
| `IDomainPrimitive<TSelf>` | `EricksonLopez.DomainPrimitives` | Base interface for all domain primitives (CRTP) |
| `IDomainPrimitive<TSelf, TValue>` | `EricksonLopez.DomainPrimitives` | Value-bearing domain primitive interface |
| `IStrongId` | `EricksonLopez.DomainPrimitives` | Non-generic marker for all strong IDs |
| `IStrongId<TSelf, TValue>` | `EricksonLopez.DomainPrimitives` | Generic strong ID with factory methods |

### Value Object Base

| Type | Namespace | Purpose |
|------|-----------|---------|
| `ValueObject` | `EricksonLopez.DomainPrimitives` | Abstract record base class for structural equality in composite value objects |

### Type Declaration Attributes

| Attribute | Namespace | Marks |
|-----------|-----------|-------|
| `[StrongId]` | `EricksonLopez.DomainPrimitives` | Strongly-typed identifier |
| `[StringPrimitive]` | `EricksonLopez.DomainPrimitives` | String-backed domain primitive |
| `[NumericPrimitive<T>]` | `EricksonLopez.DomainPrimitives` | Numeric-backed domain primitive |
| `[DatePrimitive]` | `EricksonLopez.DomainPrimitives` | Date/time-backed domain primitive |
| `[SmartEnum]` | `EricksonLopez.DomainPrimitives` | Named enumeration with BCL parsing |
| `[SmartFlagEnum]` | `EricksonLopez.DomainPrimitives` | Flags-style smart enum |
| `[ValueObject]` | `EricksonLopez.DomainPrimitives` | Composite value object |

### Validation & Error Types

| Type | Namespace | Purpose |
|------|-----------|---------|
| `PrimitiveError` | `EricksonLopez.DomainPrimitives.Validation` | Zero-allocation error model (struct) |
| `DomainPrimitiveValidationException` | `EricksonLopez.DomainPrimitives` | Thrown by `Create()` on validation failure |
| `DomainPrimitivesDefaultsAttribute` | `EricksonLopez.DomainPrimitives` | Assembly-level defaults for Trim, NotEmpty, MaxLength, ExceptionType |
| `ICustomValidator<TSelf>` | `EricksonLopez.DomainPrimitives.Validation` | Optional user-defined validation hook |

### Validation Attributes (applied to primitive declarations)

- `[Required]`, `[NotEmpty]`, `[MaxLength]`, `[MinLength]`, `[Length]`
- `[Regex]`, `[Pattern]`
- `[Min<T>]`, `[Max<T>]`, `[Range<T>]`

### Normalization Attributes

- `[Trim]`, `[ToUpperCase]`, `[ToLowerCase]`, `[ToTitleCase]`
- `[Normalize]` (NFC Unicode)

### Semantic Shortcut Attributes (30 built-in)

Pre-composed attributes combining validation and normalization:  
`[Email]`, `[Url]`, `[Phone]`, `[CountryCode]`, `[CurrencyCode]`, `[IBAN]`, `[BIC]`, `[PostalCode]`, `[Slug]`, `[HexColor]`, `[IPv4]`, `[IPv6]`, `[MAC]`, `[Uuid]`, `[Money]`, `[Age]`, `[Percentage]`, `[Latitude]`, `[Longitude]`, and others.

### Utility Types

| Type | Purpose |
|------|---------|
| `PrimitiveBuilder<T, V>` | Fluent builder for test data construction |
| `PrimitiveCollectionExtensions` | LINQ-style extension methods for collections of domain primitives |
| `DatePrimitiveKind` | Enum distinguishing `DateOnly`, `DateTime`, `DateTimeOffset` |

---

## 3. Does Not Own

| Concern | Responsible Package |
|---------|-------------------|
| Source generator implementations | `EricksonLopez.DomainPrimitives.Generators` |
| Roslyn analyzer implementations | `EricksonLopez.DomainPrimitives.Analyzers` |
| Diagnostics (`EventSource`, `Metrics`) | `EricksonLopez.DomainPrimitives` (Core) |
| Dapper type handlers | `EricksonLopez.DomainPrimitives.Dapper` |
| EF Core value converters | `EricksonLopez.DomainPrimitives.EFCore` |
| ASP.NET Core model binders | `EricksonLopez.DomainPrimitives.AspNetCore` |
| OpenAPI schema filters | `EricksonLopez.DomainPrimitives.OpenApi` |
| Newtonsoft.Json converters | `EricksonLopez.DomainPrimitives.NewtonsoftJson` |

---

## 4. Allowed Dependencies

- **.NET BCL only** (`System.*`, `Microsoft.CSharp.*` polyfills for netstandard2.0)
- **Zero** `EricksonLopez.*` package references

---

## 5. Forbidden Dependencies

- `EricksonLopez.Result`, `EricksonLopez.Events.*`, `EricksonLopez.SharedKernel`
- `Dapper`, `Microsoft.EntityFrameworkCore`, `Newtonsoft.Json`
- `Microsoft.AspNetCore.*`
- `System.Diagnostics.DiagnosticSource` (moved to Core per [ADR-015](adr/adr-015-meta-package-target-frameworks.md))

---

## 6. Reverse Dependency Matrix

Packages that may take a dependency on `Abstractions`:

| Package | Dependency Justification |
|---------|--------------------------|
| `EricksonLopez.DomainPrimitives` (Core) | L1 — meta-package bundles Abstractions |
| `EricksonLopez.DomainPrimitives.EFCore` | Needs marker interfaces for type detection only |
| `EricksonLopez.DomainPrimitives.OpenApi` | Needs marker interfaces for schema filter detection |
| `EricksonLopez.DomainPrimitives.NewtonsoftJson` | Needs marker interfaces for converter detection |
| Any downstream domain library | Can reference Abstractions for `netstandard2.0` shared contract libraries |

> [!NOTE]
> `AspNetCore`, `Dapper`, and `Testing` depend on the **meta-package** (not Abstractions directly) because they require runtime-generated code from the generators.

---

## 7. API Design Rules

- All `static abstract` members must be trim-safe and AOT-compliant
- No boxing of strongly-typed ID value types in any interface member
- `PrimitiveError` must remain a `readonly struct` — no heap allocation on the error path

---

## 8. AOT & Trimming Expectations

| TFM | IsAotCompatible | IsTrimmable |
|-----|----------------|-------------|
| `net8.0` | `true` | `true` |
| `net9.0` | `true` | `true` |
| `net10.0` | `true` | `true` |
| `netstandard2.0` | `false` | `false` |

> `netstandard2.0` explicitly disables AOT and trimming analyzers. The `_ResetPublishAotOnNetStandard` target in the `.csproj` ensures `PublishAot=false` is enforced unconditionally during multi-targeting builds.

---

## 9. Testing Isolation

- Test doubles and fixtures live in `EricksonLopez.DomainPrimitives.Testing`
- Architecture conformance is verified by `EricksonLopez.DomainPrimitives.ArchitectureTests` using `NetArchTest.Rules`
