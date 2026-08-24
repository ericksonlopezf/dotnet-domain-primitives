# Rejected Features Register

> **Version:** 1.0
> **Date:** 2026-08-10
> **Governing document:** Each entry links to an Architecture Decision Record (ADR) in `docs/adr/`.
> **Policy:** A feature listed here has been deliberately decided against. It is not "not yet
> implemented" — it is **permanently out of scope** unless an ADR explicitly reverses the decision.
>
> Before filing a GitHub Issue requesting a feature listed here, read the linked ADR.
> If you have new evidence that was not considered at decision time, a new ADR can be proposed.

---

## Summary Table

| ID | Feature | Decision | Supersedes | ADR |
|----|---------|----------|-----------|-----|
| RF-001 | Class-based primitives (`partial class`) | Rejected — permanent | — | [adr-018](adr/adr-018-reject-class-based-primitives.md) |
| RF-002 | Implicit conversions from primitive type | Rejected — permanent | — | [adr-019](adr/adr-019-reject-implicit-conversions.md) |
| RF-003 | Async validation / async factories | Rejected — permanent | — | [adr-020](adr/adr-020-reject-async-validation.md) |
| RF-004 | Reflection-based `GetAll()` for SmartEnum | Rejected — permanent | — | [adr-021](adr/adr-021-reject-reflection-getall.md) |
| RF-005 | Aggregate and Entity support | Rejected — permanent | — | [adr-022](adr/adr-022-reject-aggregate-entity-support.md) |
| RF-006 | XML serialization (`System.Xml`, SOAP, WCF) | Rejected — permanent | — | [adr-023](adr/adr-023-reject-xml-serialization.md) |
| RF-007 | Mutable domain primitives | Rejected — permanent | — | [adr-024](adr/adr-024-reject-mutable-primitives.md) |
| RF-008 | `Result<T>` as primary error API | Rejected — permanent | adr-003, adr-006 | [adr-025](adr/adr-025-reject-result-as-primary-api.md) |
| RF-009 | AutoMapper generated configuration | Rejected — permanent | — | [adr-030](adr/adr-030-reject-automapper-integration.md) |
| RF-010 | Per-property validation on `[ValueObject]` | Rejected — permanent | — | [adr-031](adr/adr-031-reject-per-property-validation-on-valueobject.md) |
| RF-011 | Discriminated Unions (v1.x) | Deferred — not rejected | — | [adr-029](adr/adr-029-defer-discriminated-unions.md) |

---

## Detailed Entries

---

### RF-001: Class-Based Primitives (`partial class`)

**What was requested:**
Support for generating `partial class` Value Objects in addition to `readonly partial record struct`.
Motivation: compatibility with Vogen's class mode, EF Core scenarios requiring reference types.

**Why rejected:**

1. **Immutability cannot be guaranteed at language level for classes.** A `readonly record struct`
   is immutable by the compiler. A `partial class` with only `init` setters can still be mutated
   via reflection, EF Core proxy generation, or serializer `Activator.CreateInstance` paths.

2. **Allocation model is incompatible.** Every `class` instance allocates on the managed heap.
   The library's allocation-minimized guarantee applies to struct types only. Adding class support
   would create a two-tier model with different allocation semantics.

3. **AOT and trimmer analysis is simpler for structs.** `readonly record struct` has no virtual
   dispatch chain to analyze. Class types with inheritance introduce polymorphic dispatch and
   trimmer complexity.

4. **Superior alternatives exist.** Users requiring class-based VOs: Vogen (class mode),
   Thinktecture (both modes), CSharpFunctionalExtensions `ValueObject<T>`.

**Decision:** Permanent rejection. See [adr-018](adr/adr-018-reject-class-based-primitives.md).

**Correct pattern:**
```csharp
// ✅ Use readonly partial record struct (the only supported form):
[StringPrimitive]
public readonly partial record struct EmailAddress;
```

---

### RF-002: Implicit Conversions from Primitive Type

**What was requested:**
Generate `implicit operator EmailAddress(string s)` so that string values can be assigned
directly: `EmailAddress email = "user@example.com";`

**Why rejected:**

1. **Defeats the purpose of a strongly typed primitive.** If `string` implicitly converts to
   `EmailAddress`, then `void Send(EmailAddress to)` can be called with any raw `string`. The
   type safety guarantee is eliminated.

2. **Bypasses validation.** An implicit conversion must always succeed (C# spec). It cannot
   call `Create()` (which throws) or `TryCreate()` (which requires an `out` parameter).
   Any implicit conversion either bypasses validation or throws — both are wrong.

3. **Creates silent runtime failures.** Code like `EmailAddress e = someString;` looks valid
   to the compiler but may throw at runtime if `someString` is invalid — with no compile-time
   indication of the risk.

**Decision:** Permanent rejection. See [adr-019](adr/adr-019-reject-implicit-conversions.md).

**Correct pattern:**
```csharp
// ✅ Explicit construction with validation:
var email = EmailAddress.Create("user@example.com");

// ✅ Or with error handling:
if (EmailAddress.TryCreate("user@example.com", out var email, out var error))
    // use email
```

---

### RF-003: Async Validation / Async Factories

**What was requested:**
`EmailAddress.CreateAsync(raw, cancellationToken)` or async hooks in `TryCreate` for database
lookups (e.g., check email uniqueness).

**Why rejected:**

1. **Domain validation must be intrinsic.** Checking whether a string is a valid email format
   is domain logic. Checking whether an email is already in the database is *application* logic.
   The latter belongs in a Command Handler or Application Service, not in the domain type.

2. **Async allocates.** `Task<T>` and `ValueTask<T>` both allocate on the heap. The zero-alloc
   success path guarantee is eliminated if any factory method returns a Task.

3. **Source generators cannot emit useful async hooks.** The delegate type for an async
   validator is unknown at generation time. Any emitted async method would require application
   infrastructure injected into the domain layer.

**Decision:** Permanent rejection. See [adr-020](adr/adr-020-reject-async-validation.md).

**Correct pattern:**
```csharp
// ✅ Synchronous domain validation (format/rules):
if (!EmailAddress.TryCreate(raw, out var email, out var error))
    return ValidationProblem(error);

// ✅ Async application validation (uniqueness/business rules):
if (await _userRepo.EmailExistsAsync(email, ct))
    return Conflict("Email already registered");
```

---

### RF-004: Reflection-Based `GetAll()` for SmartEnum

**What was requested:**
Dynamic `GetAll()` implementation using reflection to discover all `static` members of a SmartEnum
type at runtime (the Ardalis.SmartEnum pattern).

**Why rejected:**

1. **Breaks Native AOT.** `GetFields(BindingFlags.Static | BindingFlags.DeclaredOnly)` triggers
   `IL3050` (RequiresDynamicCode) and `IL2072` trimmer warnings. The `PublishAot=true` CI gate
   would fail.

2. **Worse performance.** Reflection-based discovery creates a new collection on every call
   (or requires user-side caching). The source-generated `static readonly IReadOnlyList<T> All`
   is O(1) with zero allocation.

3. **Not verifiable at compile time.** A reflection-based list can change if new members are
   added to a type in a different assembly at runtime. The static array is determined at
   compile time and is always correct.

**Decision:** Permanent rejection. See [adr-021](adr/adr-021-reject-reflection-getall.md).

**Correct pattern (generated automatically):**
```csharp
// ✅ Generated by SmartEnumGenerator.cs:
public static readonly IReadOnlyList<OrderStatus> All =
    new[] { Pending, Shipped, Delivered };

public static IReadOnlyList<OrderStatus> GetAll() => All;
```

---

### RF-005: Aggregate and Entity Support

**What was requested:**
Source generators for Aggregates (`[Aggregate]`) and Entities (`[Entity]`), including identity
management, domain events, and lifecycle support.

**Why rejected:**

1. **Categorically different problem.** Value Objects are immutable and defined by value.
   Entities have persistent identity and are mutable over time. They require different design
   patterns, lifecycle management, and infrastructure integration.

2. **Competing with frameworks, not libraries.** Aggregate support would require competing
   with MediatR, Wolverine, Duende, EventStore, and Marten — products serving a different
   market with different design constraints.

3. **Scope violation.** The library's purpose is "strictly valid, immutable domain primitives."
   Aggregates are neither strictly immutable nor primitive.

4. **Generator surface would grow unmanageably.** Current surface: ~100KB of generator code.
   Full Aggregate support would at minimum triple this.

**Decision:** Permanent rejection. See [adr-022](adr/adr-022-reject-aggregate-entity-support.md).

**Recommendation:** `EricksonLopez.DomainPrimitives` for Value Objects + `Ardalis.SharedKernel`
or similar DDD base library for Aggregates/Entities.

---

### RF-006: XML Serialization

**What was requested:**
Support for `System.Xml.Serialization`, `DataContractSerializer`, or SOAP/WCF XML encoding.

**Why rejected:**

1. **Not a .NET 8+ standard.** `System.Text.Json` is the official .NET 8+ serializer.
   XML/SOAP is a legacy technology explicitly deprecated in favor of gRPC and JSON-based APIs.

2. **AOT incompatibility.** `XmlSerializer` uses `Reflection.Emit` — incompatible with
   `PublishAot=true`. Cannot be added without breaking the CI gate.

3. **Disproportionate maintenance cost.** XML namespaces, attributes vs elements, SOAP
   envelope handling — each requires generator-level support and a test matrix.

**Decision:** Permanent rejection. See [adr-023](adr/adr-023-reject-xml-serialization.md).

---

### RF-007: Mutable Domain Primitives

**What was requested:**
Domain primitives with public setters or mutation methods, to support EF Core change tracking
or patterns that require post-construction updates.

**Why rejected:**

1. **Definition violation.** A mutable "Value Object" is an oxymoron. By definition, Value
   Objects are immutable. If identity-preserving mutation is needed, the type is an Entity.

2. **Mutable structs are a documented .NET anti-pattern.** Mutation after construction breaks
   hash codes when the struct is used as a dictionary key or in LINQ groupings.

3. **EF Core works correctly with immutable types.** The generated `ValueConverter<TModel, TProvider>`
   handles EF Core persistence correctly without requiring mutable primitives.

**Decision:** Permanent rejection. See [adr-024](adr/adr-024-reject-mutable-primitives.md).

---

### RF-008: `Result<T>` as Primary Error API

**What was requested:**
`TryCreate` or a new method returning `Result<T, PrimitiveError>`, `ValueObjectOrError<T>`,
or a similar Railway-Oriented Programming monad.

**Why rejected:**

1. **`Result<T>` allocates.** Any `Result<T>` type that is a `class` (or contains a class
   reference) allocates on the heap. The current `out`-based TryCreate is zero-allocation
   on the success path.

2. **No standard `Result<T>` in the BCL.** Every library defines its own. Generating one
   would force a specific Result library on all consumers.

3. **The `out`-pattern is the BCL standard** (`int.TryParse`, `DateTime.TryParse`, etc.).
   No learning curve for .NET developers.

4. **Railway-Oriented Programming is the Application Layer's responsibility.** A thin wrapper
   around `TryCreate` can produce any `Result<T>` type without generator support.

**Decision:** Permanent rejection. See [adr-025](adr/adr-025-reject-result-as-primary-api.md).

**Correct pattern (user-side adapter):**
```csharp
// 3-line adapter — no library support needed:
public static ErrorOr<EmailAddress> ParseEmail(string raw) =>
    EmailAddress.TryCreate(raw, out var email, out var err)
        ? email
        : Error.Validation(err.Code, err.Message);
```

---

### RF-009: AutoMapper Generated Configuration

**What was requested:**
`EricksonLopez.DomainPrimitives.AutoMapper` package generating `Profile` subclasses with
`CreateMap<T, TDto>()` configuration for all generated types.

**Why rejected:**

1. **AutoMapper is reflection-based and AOT-incompatible.** `Expression.Compile()` requires
   `RequiresDynamicCode`. Adding AutoMapper support would break the AOT CI gate.

2. **The problem is already solved.** `EricksonLopez.DomainPrimitives.Mapster` provides
   source-generated, AOT-safe mapping for composite `[ValueObject]` types. For scalar primitives,
   the generated `explicit operator` suffices for any mapper.

3. **AutoMapper's own direction is toward source generation.** If AutoMapper adopts full source
   generation, the gap disappears without any work from this library.

**Decision:** Permanent rejection. See [adr-030](adr/adr-030-reject-automapper-integration.md).

**Recommendation:** Use Mapster (with `EricksonLopez.DomainPrimitives.Mapster` for ValueObjects)
or Mapperly (source-generated, AOT-safe, works with explicit operators automatically).

---

### RF-010: Per-Property Validation Attributes on `[ValueObject]`

**What was requested:**
DataAnnotations-style attributes on `[ValueObject]` record properties:
```csharp
[ValueObject]
public readonly partial record struct Address(
    [MaxLength(100)] string Street,
    [Length(2, 2)] string CountryCode
);
```

**Why rejected:**

1. **Ambiguous validation semantics.** When does validation run? What error type is produced?
   Who aggregates errors from multiple properties? No obvious answers.

2. **Dual pipeline conflict.** DP's pipeline produces `PrimitiveError` structs. DataAnnotations
   produces `ValidationResult` objects. Mixing both creates inconsistency.

3. **AOT incompatibility.** `Validator.TryValidateObject` uses reflection — incompatible with
   `PublishAot=true`.

4. **The correct pattern eliminates the need.** Using domain primitives as property types means
   each property is validated when its value is created — no per-property annotations needed.

**Decision:** Permanent rejection. See [adr-031](adr/adr-031-reject-per-property-validation-on-valueobject.md).

**Correct pattern:**
```csharp
// ✅ Each property type carries its own validation:
[StringPrimitive(MaxLength = 100)]
public readonly partial record struct Street;

[CountryCode] // Implies Length(2,2) + UpperCase + Trim
public readonly partial record struct CountryIsoCode;

[ValueObject]
public readonly partial record struct Address(Street Street, CountryIsoCode Country);
// Address cannot be created with invalid Street or Country — type system enforces it.
```

---

### RF-011: Discriminated Unions in v1.x (Deferred, Not Rejected)

> ⚠️ This entry is **deferred**, not permanently rejected. It will be reconsidered for v2.x.

**What was requested:**
`[DiscriminatedUnion]` attribute generating exhaustive `Switch<TResult>` / `Map<TResult>`
methods, STJ converter, and EF Core owned entity mapping. (Thinktecture has this.)

**Why deferred for v1.x:**

1. **High implementation cost** (40–80 hours) relative to current adoption base.
2. **Design risk without users.** Getting the API wrong before users validate it = breaking
   change in a forced v2.0.
3. **Credibility gaps must close first.** Without public benchmarks and Newtonsoft.Json parity,
   DP loses evaluations on different criteria even if DUs are added.
4. **Competitive gap is not widening.** Thinktecture has had DUs for 2+ years with no new
   entrant. The urgency is lower than NOW/NEXT horizon items.

**Pre-conditions for resuming:**
- [ ] NuGet downloads ≥ 1,000/month for 3 consecutive months.
- [ ] GAP-002 (Newtonsoft.Json) shipped.
- [ ] GAP-009 (public benchmarks) published.
- [ ] rfc-0007 filed and accepted.
- [ ] 3+ real user requests with documented use cases.

**ADR:** [adr-029](adr/adr-029-defer-discriminated-unions.md)

**Current recommendation:** Use Thinktecture.Runtime.Extensions for Discriminated Unions.

---

## How to Propose a Reversal

If you have evidence that was not considered at the time of an ADR decision, you may propose
a reversal by:

1. Filing a GitHub Issue titled `[ADR Reversal Proposal] RF-00X: <Feature Name>`.
2. Providing specific new evidence not covered in the original ADR.
3. Filing a new ADR (following `docs/adr/adr-000-use-markdown-anywhere-architecture-decision-records.md`)
   that explicitly supersedes the original.
4. Getting maintainer approval on the new ADR before any implementation begins.

Reversals are rare but not impossible. The evidence bar is high precisely because these
decisions protect the library's architectural coherence over the long term.
