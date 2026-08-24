# Package Reference & Dependency Hierarchy

---

## 1. NuGet Packages

| Package | Description | Dependencies |
|---|---|---|
| [`EricksonLopez.DomainPrimitives`](https://nuget.org/packages/EricksonLopez.DomainPrimitives) | Core domain primitives, SmartEnum, and attributes | `Abstractions` |
| [`EricksonLopez.DomainPrimitives.Abstractions`](https://nuget.org/packages/EricksonLopez.DomainPrimitives.Abstractions) | Zero-dependency contracts and interfaces | None (BCL Only) |
| [`EricksonLopez.DomainPrimitives.Generators`](https://nuget.org/packages/EricksonLopez.DomainPrimitives.Generators) | Incremental Roslyn source generator | Roslyn 4.8 |
| [`EricksonLopez.DomainPrimitives.Analyzers`](https://nuget.org/packages/EricksonLopez.DomainPrimitives.Analyzers) | Roslyn diagnostic analyzers | Roslyn 4.8 |
| [`EricksonLopez.DomainPrimitives.AspNetCore`](https://nuget.org/packages/EricksonLopez.DomainPrimitives.AspNetCore) | ASP.NET Core Minimal APIs model binding | `DomainPrimitives` |
| [`EricksonLopez.DomainPrimitives.EFCore`](https://nuget.org/packages/EricksonLopez.DomainPrimitives.EFCore) | EF Core value converters | `DomainPrimitives`, `EF Core` |
| [`EricksonLopez.DomainPrimitives.Dapper`](https://nuget.org/packages/EricksonLopez.DomainPrimitives.Dapper) | Dapper type handlers | `DomainPrimitives`, `Dapper` |
| [`EricksonLopez.DomainPrimitives.OpenApi`](https://nuget.org/packages/EricksonLopez.DomainPrimitives.OpenApi) | OpenAPI / Swagger schema filters | `DomainPrimitives` |
| [`EricksonLopez.DomainPrimitives.Testing`](https://nuget.org/packages/EricksonLopez.DomainPrimitives.Testing) | Fluent assertions for unit tests | `DomainPrimitives` |
| [`EricksonLopez.DomainPrimitives.NewtonsoftJson`](https://nuget.org/packages/EricksonLopez.DomainPrimitives.NewtonsoftJson) | Newtonsoft.Json converters | `DomainPrimitives`, `Newtonsoft.Json` |
