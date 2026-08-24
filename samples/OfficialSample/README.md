# DomainPrimitives Showcase (Reference Implementation) — v1.0.0

This project (`OfficialSample.slnx`) is the **official reference implementation and executable documentation** of the `EricksonLopez.DomainPrimitives` public API.

Every example in this Showcase:
- Uses **only** verified public APIs from the library inventory
- Is fully compilable and runnable (net8.0 / net9.0 / net10.0)
- Represents a single authoritative implementation per scenario
- Serves as executable documentation, learning guide, integration reference, cookbook, and API demonstration simultaneously

---

## Learning Structure (Level 0 to 10)

| Level | Dimension | Project(s) | Description |
|---|---|---|---|
| **Level 0** | Conceptual | `README.md` (this file) | What is the library? Why use Domain Primitives? Advantages, trade-offs, comparison with alternatives. |
| **Level 1** | Quick Start | `01-GettingStarted` | Installation, DI registration, first functional primitive (`[Email]`, `[StrongId<Guid>]`), `TryCreate` pattern. |
| **Level 2** | Full Configuration | `08-SerializationAndMapping`, `09-SourceGenerators`, `11-SmartEnums` | Builder options, System.Text.Json + Newtonsoft.Json serialization, source generator internals, `[assembly: DomainPrimitivesDefaults]`, Smart Enums. |
| **Level 3** | Real Use Cases | `04-ValueObjects`, `05-StronglyTypedIds`, `13-DomainCollections` | All 40+ shortcut attributes, composite Value Objects, `PrimitiveCollectionExtensions`. |
| **Level 4** | Advanced Integration | `15-AspNetCoreIntegration`, `16-EFCoreIntegration`, `17-MediatRIntegration` | ASP.NET Core model binding (`AddDomainPrimitivesModelBinding`), EF Core `ConfigureDomainPrimitives`, CQRS pipeline with MediatR. |
| **Level 5** | Processing | `21-BackgroundProcessing` | Domain primitives in `Channel<T>` producer/consumer; safe `TryCreate` boundary reconstruction in background workers. |
| **Level 6** | Error Handling | `02-FirstResult`, `03-Errors`, `19-UnitTesting` | `PrimitiveError`, `DomainPrimitiveValidationException`, Result pattern, `DomainPrimitiveFakeFactory`, `DomainPrimitiveTestBuilder`, `DomainPrimitiveScenarios`. |
| **Level 7** | Scalability | `14-Performance` | Struct allocation profile, span-based parsing (`IUtf8SpanParsable<T>`), TypeConverter, zero-allocation success path. |
| **Level 8** | Customization | `22-CustomImplementations` | `ICustomValidator<T>` + `[CustomValidator<T>]`, `INormalizer<T>` + `[Normalize<T>]`, `PrimitiveBuilder<TPrimitive, TValue>` (all overloads). |
| **Level 9** | Extensions | `18-Observability`, `23-DapperIntegration`, `24-OpenApiIntegration` | `ILogger<T>` structured logging, `Activity`/OpenTelemetry tracing, Dapper `RegisterAll()`, OpenAPI schema generation. |
| **Level 10** | Enterprise Architecture | `06-EntitiesAndAggregates`, `07-DomainEvents`, `12-Specifications`, `20-EndToEndApplication` | Full tactical DDD with Aggregates, Domain Events, Specification pattern, end-to-end pipeline. |

> [!NOTE]
> None of these examples use dummy or simulated code. Every line reflects an explicit, tested, and supported capability of the library, verified against the official public API inventory.

---

## Level 0 — Conceptual Overview

### What is EricksonLopez.DomainPrimitives?

A **Source Generator–driven library** for creating **strongly-typed, validated, immutable domain primitives** in .NET with zero runtime reflection.

Instead of passing raw primitives like `string`, `int`, or `Guid` through your entire application (a pattern called _Primitive Obsession_), you declare semantic types that enforce their own invariants:

```csharp
// ❌ Primitive Obsession — what could go wrong?
void SendInvoice(string email, int amount, Guid orderId) { }

// ✅ Domain Primitives — types encode business rules
void SendInvoice(CustomerEmail email, OrderAmount amount, OrderId orderId) { }
```

### What problem does it solve?

| Problem | Domain Primitives Solution |
|---------|---------------------------|
| Passing an `OrderId` as a `CustomerId` | Compile-time type mismatch — impossible |
| Invalid email stored in DB | `[Email]` validates before construction |
| Negative money values | `[Money]` rejects via `[PrimitiveRange]` |
| Inconsistent normalization | `[Trim]`, `[LowerCase]` applied before validation |
| Boilerplate `TypeConverter`, JSON converters, EF mappings | Source-generated automatically |

### Why Source Generators?

- ✅ **Zero runtime reflection** — 100% AOT / NativeAOT compatible
- ✅ **Zero GC allocations on success path** — `TryCreate()` uses stack-allocated `PrimitiveError`
- ✅ **No registration required** — `ConfigureDomainPrimitives()` auto-discovers all types
- ✅ **IDE integration** — Roslyn Analyzers (DP0001–DP0017) enforce correct usage at design time

### Trade-offs

| Advantage | Trade-off |
|-----------|-----------|
| Compile-time safety | Requires `readonly partial record struct` syntax |
| Zero reflection | Source generators must run at build time |
| Auto-generated integration code | Additional NuGet packages per integration target |
| Stack-allocated value types | `PrimitiveBuilder<>` allocates on heap (by design) |

---

## Quick Start (2 minutes)

### 1. Install

```bash
dotnet add package EricksonLopez.DomainPrimitives
```

### 2. Declare your first primitive

```csharp
using EricksonLopez.DomainPrimitives;

[Email]
public readonly partial record struct CustomerEmail;

[StrongId<Guid>]
public readonly partial record struct CustomerId;

[Money]
public readonly partial record struct OrderAmount;
```

### 3. Use it

```csharp
// Safe non-throwing creation
if (CustomerEmail.TryCreate("  USER@example.com  ", out var email, out var error))
    Console.WriteLine(email.Value); // "user@example.com" (trimmed + lowercased)
else
    Console.WriteLine($"[{error.Code}] {error.Message}");

// Throwing creation (trusted sources)
var id = CustomerId.Create(); // generates new Guid
```

### 4. Register integrations (optional)

```csharp
// EF Core — auto-discovers all primitives
modelBuilder.ConfigureDomainPrimitives();

// Dapper — auto-registers TypeHandlers
DomainPrimitivesDapperExtensions.RegisterAll();

// ASP.NET Core — model binding
builder.Services.AddDomainPrimitivesModelBinding();

// Newtonsoft.Json
var settings = new JsonSerializerSettings();
settings.AddDomainPrimitives();
```

---

## Running the Showcase

Each chapter is an independent console application. To run a chapter:

```bash
cd samples/OfficialSample/01-GettingStarted
dotnet run
```

To build all chapters:

```bash
cd samples/OfficialSample
dotnet build OfficialSample.slnx
```

---

## API Coverage by Chapter

| Chapter | Key APIs Demonstrated |
|---------|----------------------|
| `01-GettingStarted` | `[Email]`, `[StrongId<Guid>]`, `TryCreate`, `Create`, `IParsable<T>` |
| `02-FirstResult` | `PrimitiveError`, Result pattern integration |
| `03-Errors` | `DomainPrimitiveValidationException`, `PrimitiveError.Code/Message` |
| `04-ValueObjects` | All 40+ shortcut attributes, `[ValueObject]`, `DatePrimitiveKind`, `NumericOperations` |
| `05-StronglyTypedIds` | `[StrongId<T>]`, `IStrongId<TSelf,TValue>`, `New()`, `Empty` |
| `06-EntitiesAndAggregates` | Primitives inside Aggregate Roots, entity modeling |
| `07-DomainEvents` | Domain Event dispatching with typed primitive payloads |
| `08-SerializationAndMapping` | `System.Text.Json` converters, `Newtonsoft.Json` `AddDomainPrimitives()` (both overloads) |
| `09-SourceGenerators` | Generator anatomy, `[assembly: DomainPrimitivesDefaults]` |
| `10-Analyzers` | Roslyn Analyzer diagnostics DP0001–DP0017 |
| `11-SmartEnums` | `[SmartEnum<T>]`, `GetAll()`, `FromName()`, `Match<TResult>()` |
| `12-Specifications` | Specification pattern with primitive-typed predicates |
| `13-DomainCollections` | `PrimitiveCollectionExtensions`: `ToDomainPrimitiveList<>`, `ToDomainPrimitiveArray<>` (IEnumerable + ReadOnlySpan) |
| `14-Performance` | Span parsing, zero-allocation, struct footprint |
| `15-AspNetCoreIntegration` | `AddDomainPrimitivesModelBinding()` (MvcOptions + IServiceCollection), route/query binding |
| `16-EFCoreIntegration` | `ConfigureDomainPrimitives()`, auto-generated `ValueConverter` |
| `17-MediatRIntegration` | CQRS with MediatR, typed command/query parameters |
| `18-Observability` | `ILogger<T>` structured logging, `Activity`/OpenTelemetry, `DiagnosticSource` |
| `19-UnitTesting` | `DomainPrimitiveFakeFactory`, `DomainPrimitiveTestBuilder`, `DomainPrimitiveScenarios`, `DomainPrimitiveVerifyExtensions` |
| `20-EndToEndApplication` | Full DDD application with all layers integrated |
| `21-BackgroundProcessing` | `Channel<T>` queue, `TryCreate` boundary reconstruction, invalid message rejection |
| `22-CustomImplementations` | `ICustomValidator<T>`, `[CustomValidator<T>]`, `INormalizer<T>`, `[Normalize<T>]`, `PrimitiveBuilder<T,V>` (all 4 patterns) |
| `23-DapperIntegration` | `DomainPrimitivesDapperExtensions.RegisterAll()`, auto-generated `TypeHandler` |
| `24-OpenApiIntegration` | OpenAPI schema filters, Swagger UI primitive representation |

---

## Further Reading

- [Quick Start](../../docs/quickstart.md) — 5-minute onboarding
- [Cookbook](../../docs/cookbook.md) — 15 recipes with complete code
- [API Reference](../../docs/api-reference.md) — detailed interface/method documentation
- [Functional Map](../../docs/functional-map.md) — architecture and component interactions
- [Diagrams](../../docs/diagrams.md) — Mermaid diagrams: pipeline, states, error flow, background processing
