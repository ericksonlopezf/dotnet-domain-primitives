# Progressive Showcase — Official Learning Guide

The Showcase project (`samples/OfficialSample/OfficialSample.slnx`) is the **official reference implementation and executable documentation** for `EricksonLopez.DomainPrimitives`.

Every example:
- Uses **only** verified public APIs from the official inventory.
- Is fully compilable and testable (targets: `net8.0`, `net9.0`, `net10.0`).
- Belongs to an explicit pedagogical level.
- Provides a single authoritative reference implementation per scenario.

---

## Pedagogical Matrix (Levels 0 to 10)

| Level | Dimension | Associated Showcase Projects | Detailed Guide | Description |
|---|---|---|---|---|
| **Level 0** | **Conceptual** | `README.md` | [Level 00 — Conceptual](level-00-conceptual.md) | What is the library? What problem does it solve? Advantages, trade-offs, and comparison. |
| **Level 1** | **Quick Start** | `01-GettingStarted` | [Level 01 — Quick Start](level-01-quickstart.md) | Minimal setup, first primitive with `[Email]` and `[StrongId<Guid>]`, using `TryCreate`. |
| **Level 2** | **Full Configuration** | `08-SerializationAndMapping`, `09-SourceGenerators`, `11-SmartEnums` | [Level 02 — Full Configuration](level-02-full-configuration.md) | `[assembly: DomainPrimitivesDefaults]`, System.Text.Json + Newtonsoft.Json, Smart Enums (`[SmartEnum<T>]`). |
| **Level 3** | **Real Use Cases** | `04-ValueObjects`, `05-StronglyTypedIds`, `13-DomainCollections` | [Level 03 — Real Use Cases](level-03-real-use-cases.md) | 38+ semantic shortcuts (Strings, Numerics, Dates), composite Value Objects, LINQ and Span extensions. |
| **Level 4** | **Advanced Integration** | `15-AspNetCoreIntegration`, `16-EFCoreIntegration`, `17-MediatRIntegration` | [Level 04 — Advanced Integration](level-04-advanced-integration.md) | ASP.NET Core Model Binding, EF Core `ConfigureDomainPrimitives()`, CQRS pipelines with MediatR. |
| **Level 5** | **Processing** | `21-BackgroundProcessing` | [Level 05 — Processing](level-05-processing.md) | Concurrency with `Channel<T>`, safe boundary reconstruction with `TryCreate` in Background Workers. |
| **Level 6** | **Error Handling** | `02-FirstResult`, `03-Errors`, `19-UnitTesting` | [Level 06 — Error Handling](level-06-error-handling.md) | `PrimitiveError`, error codes, Railway-Oriented Programming and Result pattern integration. |
| **Level 7** | **Scalability** | `14-Performance` | [Level 07 — Scalability](level-07-scalability.md) | Stack-allocated immutable structs, zero GC allocations on success, span parsing with `ISpanParsable<T>`. |
| **Level 8** | **Customization** | `22-CustomImplementations` | [Level 08 — Customization](level-08-customization.md) | Custom validators (`ICustomValidator<T>`), normalizers (`INormalizer<T>`), and `PrimitiveBuilder<T, V>`. |
| **Level 9** | **Extensions** | `18-Observability`, `23-DapperIntegration`, `24-OpenApiIntegration` | [Level 09 — Extensions](level-09-extensions.md) | OpenTelemetry metrics, DI-free event sources, Dapper `RegisterAll()`, OpenAPI Swagger filters. |
| **Level 10** | **Enterprise Architecture** | `06-EntitiesAndAggregates`, `07-DomainEvents`, `12-Specifications`, `20-EndToEndApplication` | [Level 10 — Enterprise Architecture](level-10-enterprise-architecture.md) | Full Tactical DDD: Aggregate Roots, Domain Events, Specifications, and Clean Architecture. |

---

## Running the Showcase

To build and verify all Showcase projects:

```bash
# Build the entire Showcase solution
dotnet build samples/OfficialSample/OfficialSample.slnx

# Execute the Showcase test suite
dotnet test samples/OfficialSample/OfficialSample.Tests/OfficialSample.Tests.csproj
```
