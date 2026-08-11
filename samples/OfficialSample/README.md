# DomainPrimitives Showcase (Reference Implementation) - Version 1.0.0

This project (`OfficialSample.slnx`) constitutes the official reference implementation and executable documentation of the `EricksonLopez.DomainPrimitives` public API.

It has been pedagogically structured in **11 progressive levels**, guaranteeing step-by-step learning from fundamental concepts to enterprise architectures.

## Learning Structure (Level 0 to 10)

| Level | Dimension | Projects / Resources | Description |
|---|---|---|---|
| **Level 0** | Conceptual | [README.md](./README.md) | What is the library? Why use Domain Primitives? Advantages and base architecture. |
| **Level 1** | Quick Start | `01-GettingStarted` | Installation, basic DI, and first functional use of a primitive (e.g. `StringPrimitive`). |
| **Level 2** | Full Configuration | `08-SerializationAndMapping`, `09-SourceGenerators`, `11-SmartEnums` | Builder options, JSON Serialization configuration, and automatic code generation. |
| **Level 3** | Real Use Cases | `04-ValueObjects`, `05-StronglyTypedIds`, `13-DomainCollections` | Modeling real business concepts using the public inventory (e.g., `Age`, `Money`, Database IDs). |
| **Level 4** | Advanced Integration | `15-AspNetCoreIntegration`, `16-EFCoreIntegration`, `17-MediatRIntegration` | ASP.NET Registration, Entity Framework Type Converters, and CQRS pipeline with MediatR. |
| **Level 5** | Processing | `21-BackgroundProcessing` | Integration with Background Services, Workers, or concurrency (asynchronous and queued processing). |
| **Level 6** | Error Handling | `02-FirstResult`, `03-Errors`, `19-UnitTesting` | Capturing `DomainPrimitiveException`, using the `Result` pattern, and defensive validation. |
| **Level 7** | Scalability | `14-Performance` | Performance considerations and optimizations using generated Structs. |
| **Level 8** | Customization | `22-CustomImplementations` | Custom implementations of validators (`ICustomValidator<T>`) or normalizers (`INormalizer<T>`). |
| **Level 9** | Extensions | `18-Observability` | Use of `DomainPrimitivesMetrics` and native OpenTelemetry instrumentation of the library. |
| **Level 10** | Enterprise Architecture | `06-EntitiesAndAggregates`, `07-DomainEvents`, `12-Specifications`, `20-EndToEndApplication` | Full tactical DDD. Use of primitives inside Aggregates, dispatching Domain Events, and validating with Specifications. |

> [!NOTE]  
> None of these examples use dummy or simulated code. Absolutely every line reflects an explicit, tested, and supported capability of the library, contrasted with the official `PUBLIC_API.md` of the solution.
