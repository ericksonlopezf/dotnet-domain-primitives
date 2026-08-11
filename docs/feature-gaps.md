# Feature Gaps & Recommendations

> **Audit Version:** 2.0 | **Date:** 2026-08-10  
> **Evidence policy:** Gaps identified by competitive analysis.  
> **Priority:** P0=Critical · P1=Important · P2=Useful · P3=Optional · Reject=Do not implement

---

## Competitive Gaps Summary

```
Critical gaps   → 2
High-value gaps → 4
Medium gaps     → 5
Low-value gaps  → 3
Intentional gaps → 5
```

---

## Missing Features (Gaps)

---

### GAP-001: Discriminated Unions

```
Feature:               Discriminated Union / Algebraic Data Type
Category:              Core Domain Primitive Model
Competitors:           Thinktecture.Runtime.Extensions (only one with this)
Business value:        Very High — model domain states as "one of" without invalid states
DDD value:             Critical — eliminates invalid states at compile time
Performance value:     High — struct-based DUs avoid polymorphic dispatch
Implementation:        High — requires Match/Switch exhaustiveness generation
Priority:              P1 — Important
```

**Recommendation:** Implement a `[DiscriminatedUnion]` attribute. Generator produces:
- A partial record struct with named case properties
- `Match<TResult>(case1Func, case2Func, ...)` — exhaustive, compile-time checked
- `Switch(case1Action, ...)` — void variant
- STJ JSON converter (tag-based discriminator)
- EF Core owned entity pattern

**Why not P0:** DomainPrimitives can position as "Value Object library" without DUs. But Thinktecture uses DUs to justify "full DDD primitive story". This gap costs competitive parity on DDD positioning.

---

### GAP-002: Newtonsoft.Json Support

```
Feature:               Newtonsoft.Json converters (generated)
Category:              Serialization
Competitors:           Vogen (Conversions.NewtonsoftJson), StronglyTypedId (StronglyTypedIdConverter.NewtonsoftJson)
Business value:        High — large enterprise codebase still on Newtonsoft
DDD value:             None (infrastructure concern)
Performance value:     Neutral
Implementation:        Low — generator emits JsonConverter<T> for Newtonsoft
Priority:              P1 — Important
```

**Recommendation:** Add `EricksonLopez.DomainPrimitives.NewtonsoftJson` package. Generator discovers types and emits `Newtonsoft.Json.JsonConverter<T>` alongside the STJ converter. Mirror the Vogen `Conversions` flag pattern.

**Note:** This is the **only integration gap** where both Vogen AND StronglyTypedId outperform DP. Not having it removes DP from consideration in enterprise projects with Newtonsoft dependencies.

---

### GAP-003: Configurable Global Exception Type

```
Feature:               Assembly-level custom exception type for Create() failures
Category:              Developer Experience / Validation
Competitors:           Vogen (VogenDefaults with custom exception)
Business value:        High — enterprise patterns require domain-specific exceptions
DDD value:             High — DomainException hierarchy is standard DDD pattern
Performance value:     None
Implementation:        Low — assembly attribute; generator checks for override
Priority:              P1 — Important
```

**Recommendation:** Add `[assembly: DomainPrimitivesDefaults(ExceptionType = typeof(MyDomainException))]`. Generator uses this type instead of `DomainPrimitiveValidationException`. Requires the custom exception to inherit from `Exception`.

---

### GAP-004: INumber<T> for NumericPrimitive

```
Feature:               INumber<T>, IMinMaxValue<T> generic math interfaces
Category:              Operators / BCL Integration
Competitors:           Vogen (partial, via operator hoisting)
Business value:        High for mathematical domains (pricing, measurements)
DDD value:             Medium — enables generic algorithms on domain types
Performance value:     High — SIMD and generic math algorithms
Implementation:        Medium — requires conditional compilation (#if NET7_0_OR_GREATER)
Priority:              P2 — Useful
Status:                NOT YET IMPLEMENTED — opt-in feature planned for v2.0
```

**Clarification (2026-08-10, HIGH-V4-002):** `INumber<T>` is **opt-in only** via `EnableGenericMath = true` flag on the attribute:
```csharp
[NumericPrimitive<decimal>(EnableGenericMath = true)] // opt-in
public readonly partial record struct Price;

[NumericPrimitive<int>] // default — no INumber<T>
public readonly partial record struct Count;
```

The API surface budget for `NumericPrimitive` (≤38 base, ≤42 with Operations) does **NOT** include `INumber<T>` members. When `EnableGenericMath = true` is used, the budget increases by the number of `INumber<T>` members that are NOT hidden with `[EditorBrowsable(Never)]`. An RFC must be filed before implementation.

**Implementation recommendation:** For `[NumericPrimitive<T>]` where `T : INumber<T>`, optionally generate `INumber<T>` implementation. Start with `IMinMaxValue<T>` and `IComparable<T>` only, then expand.

**Warning:** `INumber<T>` has a large interface surface. Most members should be hidden with `[EditorBrowsable(Never)]` — the user-facing API should remain consistent with the base budget. Expose only the members that are truly user-facing (`+`, `-`, `*`, `/`, `Abs`, `Min`, `Max`).

---

### GAP-005: TrimStart / TrimEnd Attributes

```
Feature:               Separate [TrimStart] and [TrimEnd] attributes
Category:              Normalization
Competitors:           None (all lack this too)
Business value:        Low-medium
DDD value:             Low
Performance value:     Zero-alloc on span path
Implementation:        Very Low
Priority:              P3 — Optional
```

**Recommendation:** Add [TrimStart] and [TrimEnd] to the normalization attribute set. Generates `s.TrimStart()` / `s.TrimEnd()` calls. Low priority — existing [Trim] covers most cases.

---

### GAP-006: Smart Enum Switch/Map Exhaustiveness

```
Feature:               Exhaustive Switch<TResult> and Map<TResult> methods
Category:              Smart Enum
Competitors:           Thinktecture.Runtime.Extensions (only one)
Business value:        High — eliminates missed enum cases at compile time
DDD value:             High — exhaustive handling enforced by compiler
Performance value:     Medium
Implementation:        Medium — generator emits overloaded method with all cases as params
Priority:              P2 — Useful
```

**Recommendation:** For `[SmartEnum<T>]`, generate:
```csharp
public TResult Switch<TResult>(
    Func<OrderStatus, TResult> onPending,
    Func<OrderStatus, TResult> onShipped,
    Func<OrderStatus, TResult> onDelivered)
```
where all enum members are covered. Compiler error if a case is missing.

---

### GAP-007: Case-Insensitive Smart Enum Parsing

```
Feature:               Case-insensitive TryFromName("pending") for SmartEnum
Category:              Smart Enum
Competitors:           Ardalis.SmartEnum (explicit feature), THK (supported)
Business value:        Medium — HTTP/JSON often sends lowercase or inconsistent casing
DDD value:             Low
Performance value:     Neutral
Implementation:        Low — generate overload with StringComparison.OrdinalIgnoreCase
Priority:              P2 — Useful
```

**Recommendation:** Add `TryFromName(string name, bool ignoreCase, out T result)` overload to SmartEnum generated code.

---

### GAP-008: Exact Length Validation Attribute

```
Feature:               [ExactLength(n)] — validates string has exactly n characters
Category:              Validation
Competitors:           None (all require MinLength+MaxLength workaround)
Business value:        Medium — common for codes (ISBN, VIN, country code)
DDD value:             Low (already enforced via semantic shortcuts)
Performance value:     None
Implementation:        Trivial
Priority:              P3 — Optional
```

**Note:** Already partly covered by domain shortcuts ([ISBN], [VIN], [CountryCode]) which have built-in length rules. [ExactLength] is only needed for custom StringPrimitive types.

---

### GAP-009: Public Benchmark Results

```
Feature:               Published BenchmarkDotNet results in README / docs
Category:              Developer Experience / Documentation
Competitors:           Vogen (has benchmark table in README), others
Business value:        Critical for adoption — developers won't trust "zero-allocation" without proof
DDD value:             None
Performance value:     N/A
Implementation:        Low — run benchmark suite and commit results
Priority:              P0 — CRITICAL (for marketing validity)
```

**Recommendation:** Add a `benchmarks/results/` directory with:
- `StringPrimitive_Create.md`
- `StringPrimitive_TryParse.md`  
- `StrongId_EFCore.md`
- `SmartEnum_Lookup.md`

Embed summary table in README. Without this, ALL performance claims are unsupported marketing.

---

### GAP-010: Migration Guide

```
Feature:               "Migrating from Vogen/StronglyTypedId to DomainPrimitives" guide
Category:              Developer Experience / Documentation
Competitors:           Vogen, Thinktecture (have migration docs)
Business value:        High — lowers switching cost for new adopters
DDD value:             None
Performance value:     None
Implementation:        Low (documentation only)
Priority:              P1 — Important
```

**Recommendation:** Create `docs/migration/from-vogen.md` and `docs/migration/from-stronglytypedid.md`. Include attribute equivalents table, API differences, and step-by-step walkthrough.

---

### GAP-011: Global Configuration ([assembly: DomainPrimitivesDefaults])

```
Feature:               Assembly-level defaults for all generated types
Category:              Developer Experience
Competitors:           Vogen (VogenDefaults attribute), Thinktecture (global config)
Business value:        High — avoid repeating Trim=true on 50 types
DDD value:             None
Performance value:     None
Implementation:        Medium — generator reads assembly attributes
Priority:              P2 — Useful
```

**Recommendation:** `[assembly: DomainPrimitivesDefaults(Trim = true, NotEmpty = true, MaxLength = 256)]`. Sensible defaults override per-attribute. Per-type attribute takes precedence.

---

## Features to Explicitly Reject

---

### REJECT-001: Reflection-Based GetAll() for SmartEnum

**Competitor:** Ardalis.SmartEnum  
**Reason:** Breaks Native AOT. Trimmer cannot statically analyze reflection over derived types. DP's generator already builds a static readonly array at compile time — this is the correct approach. **Never revert this to reflection.**

---

### REJECT-002: Async Validation / Async Factories

**Competitor:** Custom manual implementations, CSharpFunctionalExtensions  
**Reason:** Constructors and `TryCreate` must be fast and synchronous. Async implies I/O, which violates zero-allocation hot path philosophy. Domain validation is by definition intrinsic — it never needs a database lookup. Async validation = Infrastructure concern = Domain Service / Command Handler. **Reject permanently.**

---

### REJECT-003: Mutable Primitives

**Competitor:** Legacy ValueOf library  
**Reason:** DDD Value Objects must be immutable. Mutability breaks hash codes when used as dictionary keys, introduces side effects, and violates thread safety. All DP types are readonly record structs. **Never add mutation support.**

---

### REJECT-004: XML Serialization Support

**Competitor:** Legacy frameworks  
**Reason:** XML is legacy. System.Text.Json is the .NET 8+ standard. Adding XML serialization bloats the generator surface and creates maintenance burden. The only valid XML scenario (SOAP services) requires a full infrastructure overhaul that DP should not own. **Reject.**

---

### REJECT-005: Dynamic/Runtime Code Generation (Reflection.Emit / IL)

**Competitor:** AutoMapper (historical), older ORMs  
**Reason:** Completely breaks Native AOT. All integrations must remain 100% source-generated. **Reject permanently.**

---

### REJECT-006: Result<T> as Primary Error Pattern

**Competitor:** CSharpFunctionalExtensions, LanguageExt  
**Reason:** `Result<T>` is a heap-allocated wrapper object. DP's `out`-based TryCreate is zero-allocation on the success path AND the failure path (when using struct errors). Adding a Result<T> overload as the primary API would:
1. Allocate on every validation
2. Create an inconsistent API surface
3. Pull in functional programming dependencies

If users want `Result<T>`, they can wrap `TryCreate` themselves. **Reject as primary API.**

---

### REJECT-007: Implicit Conversions from Primitive

**Competitor:** Some libraries allow `MyId id = 42`  
**Reason:** Implicit conversion defeats the entire purpose of a strongly typed ID. It allows `void Do(OrderId id)` to be called with a raw `int`, silently. This is precisely the primitive obsession DP exists to prevent. **Reject permanently.**

---

### REJECT-008: Aggregate / Entity Support

**Reason:** DP is a Value Object / Primitive library. Aggregates and Entities require:
- Identity that survives mutations (Entities have ID, VOs do not)
- Domain events
- Repository pattern support
- Lifecycle management

These concerns belong in a separate library (MediatR, Wolverine, Duende) or framework. Adding them to DP would violate the Single Responsibility Principle at the library level. **Reject permanently.**

---

### REJECT-009: AutoMapper Generated Configuration

**Reason:** AutoMapper is reflection-based and has poor AOT compatibility. DP already provides Mapster (source-generated). Adding AutoMapper would:
1. Require reflection
2. Conflict with AOT stance
3. Duplicate what Mapster already provides

**Reject. Use Mapperly (source-generated) as the third mapping option instead of AutoMapper.**

---

### REJECT-010: Per-Property Validation Attributes on ValueObject

**Example:** `[ValueObject] public record struct Money([MaxLength(10)] string Currency, [Range(0, 1M)] decimal Amount)`  
**Reason:** This design blurs the line between DP's compile-time validation pipeline and System.ComponentModel.DataAnnotations. It creates ambiguity about when validation fires, who owns the contract, and how errors aggregate. The correct pattern is: validation is defined in the `[ValueObject]`'s partial implementation — not scattered across parameter attributes. **Reject. Use partial methods for cross-property validation instead.**

---

## Competitive Gaps by Competitor

### What Thinktecture has that DP should consider:
1. **Discriminated Unions** (GAP-001) — P1
2. **Switch/Map exhaustiveness** (GAP-006) — P2
3. **Class support** (REJECT-adjacent) — Reject (breaks struct-first design)

### What Vogen has that DP should consider:
1. **Global defaults (VogenDefaults)** (GAP-011) — P2
2. **Configurable exception type** (GAP-003) — P1
3. **Newtonsoft.Json converters** (GAP-002) — P1
4. **class partial type** — Reject (breaks readonly struct invariant)

### What StronglyTypedId has that DP should consider:
1. **Newtonsoft.Json converters** (GAP-002) — P1
2. Nothing else significant — STI is narrowly focused

### What Ardalis.SmartEnum has that DP should consider:
1. **Case-insensitive parsing** (GAP-007) — P2
2. **SmartFlagEnum** — P3 (consider after GAP-006)
3. **Reflection-based GetAll()** — REJECT (AOT risk)

### What DomainPrimitives has that NONE of the competitors have:
1. IUtf8SpanParsable<T> generated (NET8+)
2. ISpanFormattable / IUtf8SpanFormattable generated
3. SEC-001 through SEC-006 security gates
4. Declarative normalization attributes ([Trim], [LowerCase], [UpperCase])
5. NFC Unicode normalization (SEC-004) on all string paths
6. 15 semantic string shortcuts + 15 numeric shortcuts
7. Date primitive ([DatePrimitive])
8. Auto-discovery integrations (EF Core, Dapper — no per-type config)
9. Mapster source-generated integration
10. Dedicated OpenAPI package
11. Architecture Decision Records (docs/adr/)
