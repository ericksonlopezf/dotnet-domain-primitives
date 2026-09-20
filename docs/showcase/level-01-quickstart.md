# Level 1 — Quick Start

This level demonstrates minimal configuration, declaration of initial primitives, and core API consumption as demonstrated in the [`01-GettingStarted`](file:///d:/DevData/ericksonlopez.dev/dotnet-domain-primitives/samples/OfficialSample/01-GettingStarted/Program.cs) project.

---

## 1. Installation

Install the official meta-package from NuGet:

```bash
dotnet add package EricksonLopez.DomainPrimitives
```

The meta-package automatically includes abstractions (`Abstractions`), incremental code generators (`Generators`), and static Roslyn analyzers (`Analyzers`).

---

## 2. Declaring Domain Primitives

Declare primitives as `readonly partial record struct` decorated with the corresponding attributes:

```csharp
using EricksonLopez.DomainPrimitives;

// String-backed primitive with automated normalization and RFC validation
[Email]
public readonly partial record struct CustomerEmail;

// Strongly-typed identifier backed by Guid
[StrongId<Guid>]
public readonly partial record struct CustomerId;
```

---

## 3. Creation and Invariant Validation: `TryCreate` vs `Create`

The library emits two primary static factory methods:

### Safe Perimeter Ingestion Pattern (`TryCreate`)
Use `TryCreate` at untrusted boundaries (HTTP controllers, JSON deserialization, message queues). It returns a boolean and an immutable `PrimitiveError` struct with zero heap allocations:

```csharp
string rawEmail = "  User.Name@Example.COM  ";

if (CustomerEmail.TryCreate(rawEmail, out var email, out var error))
{
    // email is automatically normalized: "user.name@example.com"
    Console.WriteLine($"Valid email created: {email.Value}");
}
else
{
    Console.WriteLine($"Validation failed [{error.Code}]: {error.Message}");
}
```

### Strict Pattern for Trusted Internals (`Create`)
Use `Create` when data originates from trusted storage or has already passed boundary sanitization. It throws a structured `DomainPrimitiveValidationException` if invariants are violated:

```csharp
// Direct creation with new unique GUID
var customerId = CustomerId.New();

// Instantiation from existing GUID
var existingId = CustomerId.Create(Guid.NewGuid());
```

---

## 4. ASP.NET Core Integration

Domain primitives are immutable value types that do not require IoC container service registration. To enable parameter binding in ASP.NET Core:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Enable model binding for all domain primitives
builder.Services.AddDomainPrimitivesModelBinding();

var app = builder.Build();

app.MapGet("/customers/{id}", (CustomerId id) => Results.Ok(new { CustomerId = id.Value }));

app.Run();
```

---

## Showcase Reference
- Executable project: [`samples/OfficialSample/01-GettingStarted`](file:///d:/DevData/ericksonlopez.dev/dotnet-domain-primitives/samples/OfficialSample/01-GettingStarted/Program.cs)
