# Best Practices and Production Guidelines

Official design, performance, and architectural guidelines for building mission-critical systems with `EricksonLopez.DomainPrimitives`.

---

## 1. Domain Primitive Declaration

### 1.1. Always use `readonly partial record struct`
Domain primitives model immutable scalar concepts without distinct lifecycle identity. Declaring them as readonly structs guarantees:
- Stack allocation (0 bytes on the heap during local method execution).
- Zero garbage collection overhead on the hot path.
- Native value equality semantics without runtime boxing.

```csharp
// ✅ Correct: immutable partial record struct
[Email]
public readonly partial record struct CustomerEmail;

// ❌ Incorrect: classes incur heap allocations and GC overhead
public class CustomerEmail { ... }
```

### 1.2. Keep Validation Pure and Free of Side-Effects
- **Never** perform database queries, HTTP calls, or asynchronous I/O inside `ICustomValidator<T>`.
- Invariant validation must remain strictly deterministic and memory-bound (e.g. check-digit algorithms, regex evaluation, range and length constraints).
- Contextual validations (e.g. "does this customer exist in the database?") belong in **Application Handlers** or **Domain Services**, not inside the primitive.

---

## 2. Ingestion at System Boundaries

### 2.1. Use `TryCreate` at Untrusted Ingestion Boundaries
In HTTP endpoints, message queue consumers, external data parsers, and public APIs:
- Use `TryCreate` to validate inputs without paying the performance penalty of throwing exceptions.
- Leverage the stack-allocated `PrimitiveError` struct to construct `400 Bad Request` or `ProblemDetails` responses.

```csharp
// ✅ Correct: at untrusted perimeter
if (!CustomerEmail.TryCreate(rawInput, out var email, out var error))
{
    return Results.BadRequest(new { error.Code, error.Message });
}
```

### 2.2. Use `Create` Within Trusted Domain Core
Inside aggregates, entities, and trusted domain logic:
- Use `Create()` when inputs are already sanitized or loaded from trusted data stores.
- Failing an invariant at this layer represents a programming error, making a fast-fail `DomainPrimitiveValidationException` appropriate.

---

## 3. Persistence and ORM Integration

### 3.1. Entity Framework Core
- Invoke `configurationBuilder.ConfigureDomainPrimitives()` inside `ConfigureConventions` in your `DbContext` class.
- Avoid redundant manual value converter configurations in `OnModelCreating`.
- Ensure column lengths match `[MaxLength(n)]` attributes specified on the domain primitive.

### 3.2. Dapper Micro-ORM
- Call `DapperDomainPrimitivesRegistration.RegisterAll()` once during application startup in `Program.cs`.
- If a database column is nullable, declare the corresponding C# model property as nullable (`CustomerId?`), preventing Dapper from throwing a `DataException` upon encountering `DBNull.Value`.

---

## 4. Observability and Telemetry

- Subscribe to `DomainPrimitiveEventSource.OnValidationFailed` during application initialization to audit anomalies or malformed payload patterns.
- Register the `EricksonLopez.DomainPrimitives` meter in your OpenTelemetry pipeline to track creation throughput and validation failure rates in production.
