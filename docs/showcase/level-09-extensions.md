# Level 9 — Ecosystem Extensions

This level covers official ecosystem extensions for observability (OpenTelemetry), micro-ORMs (Dapper), and API documentation (Swagger / OpenAPI).

---

## 1. Observability and OpenTelemetry Metrics (`DomainPrimitivesDiagnostics`)

The Core package provides native instrumentation for OpenTelemetry and `DiagnosticSource` with zero third-party runtime dependencies:

```csharp
using EricksonLopez.DomainPrimitives.Diagnostics;

// 1. OpenTelemetry Metrics (Meters)
// Exposes counters:
// - "domain_primitive.validation.success"
// - "domain_primitive.validation.failure"
// - "domain_primitive.creation"
DomainPrimitivesMetrics.RecordCreation("CustomerEmail");

// 2. Static events without DI (ideal for auditing and security monitoring)
DomainPrimitiveEventSource.OnValidationFailed += (sender, args) =>
{
    Console.WriteLine($"[AUDIT] Validation failure in {args.PrimitiveName}: {args.ErrorType} - {args.ErrorMessage}");
};
```

---

## 2. Dapper Micro-ORM (`EricksonLopez.DomainPrimitives.Dapper`)

Dapper requires custom type handlers (`SqlMapper.TypeHandler<T>`). The source generator automatically emits and registers all required handlers:

```csharp
using EricksonLopez.DomainPrimitives.Dapper.Generated;

// One-line registration during application startup (Thread-safe and idempotent)
DapperDomainPrimitivesRegistration.RegisterAll();

// Native query execution with strongly-typed domain models
using var connection = new SqlConnection(connectionString);
var customer = await connection.QuerySingleOrDefaultAsync<CustomerEntity>(
    "SELECT Id, Email, Balance FROM Customers WHERE Id = @Id",
    new { Id = customerId.Value });
```

---

## 3. OpenAPI / Swagger Documentation (`EricksonLopez.DomainPrimitives.OpenApi`)

Emits Swagger schemas describing domain primitives as their scalar types rather than nested objects containing a `Value` property:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Register the SchemaFilter emitted by EricksonLopez.DomainPrimitives.OpenApi
    options.SchemaFilter<EricksonLopez.DomainPrimitives.OpenApi.Generated.DomainPrimitivesSchemaFilter>();
});
```

In the Swagger UI documentation:
- `CustomerEmail` renders as `string` with `format: email`.
- `OrderId` renders as `string` with `format: uuid`.
- `Price` renders as `number`.
- `OrderStatus` (`SmartEnum`) documents the exact list of valid states in its schema enum field.

---

## Showcase Reference
- Executable projects:
  - [`18-Observability`](file:///d:/DevData/ericksonlopez.dev/dotnet-domain-primitives/samples/OfficialSample/18-Observability/Program.cs)
  - [`23-DapperIntegration`](file:///d:/DevData/ericksonlopez.dev/dotnet-domain-primitives/samples/OfficialSample/23-DapperIntegration/Program.cs)
  - [`24-OpenApiIntegration`](file:///d:/DevData/ericksonlopez.dev/dotnet-domain-primitives/samples/OfficialSample/24-OpenApiIntegration/Program.cs)
