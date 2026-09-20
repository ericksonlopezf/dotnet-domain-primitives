# Level 4 — Advanced Integration

This level demonstrates deep integration of domain primitives with core .NET frameworks: ASP.NET Core, Entity Framework Core, and MediatR.

---

## 1. ASP.NET Core Integration

The `EricksonLopez.DomainPrimitives.AspNetCore` package provides compile-time model binders and declarative validation attributes.

### Enabling Model Binding
```csharp
var builder = WebApplication.CreateBuilder(args);

// Enable the generated ModelBinderProvider
builder.Services.AddDomainPrimitivesModelBinding();
```

### Route and Query Parameter Binding
```csharp
app.MapGet("/users/{id}", (UserId id) => 
{
    return Results.Ok(new { Id = id.Value });
});
```

### Declarative DTO Validation
```csharp
using EricksonLopez.DomainPrimitives.AspNetCore;

public class CreateCustomerRequest
{
    [DomainPrimitiveValidation<CustomerEmail, string>]
    public string Email { get; set; } = string.Empty;

    [DomainPrimitiveValidation<OrderAmount, decimal>]
    public decimal CreditLimit { get; set; }
}
```

---

## 2. Persistence with Entity Framework Core

The `EricksonLopez.DomainPrimitives.EFCore` package automatically emits compile-time `ValueConverter<T, TValue>` implementations for each primitive without requiring verbose manual mappings in `OnModelCreating`.

### Centralized Convention Configuration
```csharp
using EricksonLopez.DomainPrimitives.EFCore.Generated;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Customer> Customers => Set<Customer>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Auto-configures all ValueConverters, column max lengths, and decimal precisions
        configurationBuilder.ConfigureDomainPrimitives();
    }
}
```

Calling `ConfigureDomainPrimitives()` instructs EF Core to apply:
- The exact scalar `ValueConverter` for database persistence.
- Maximum length constraints (`HasMaxLength`) if the primitive has `[MaxLength(n)]`.
- Decimal precision and scale configurations for monetary types (`[Money]`, `[Percentage]`).

---

## 3. CQRS Pipelines with MediatR

Commands and queries express business intent using domain primitives rather than raw scalars:

```csharp
using MediatR;

public record RegisterCustomerCommand(
    CustomerId Id,
    CustomerEmail Email,
    CustomerName Name
) : IRequest<Result<CustomerId>>;

public class RegisterCustomerHandler : IRequestHandler<RegisterCustomerCommand, Result<CustomerId>>
{
    public Task<Result<CustomerId>> Handle(RegisterCustomerCommand command, CancellationToken ct)
    {
        // The invariant validity of command.Email and command.Id is guaranteed by the type
        return Task.FromResult(Result<CustomerId>.Success(command.Id));
    }
}
```

---

## Showcase Reference
- Executable projects:
  - [`15-AspNetCoreIntegration`](file:///d:/DevData/ericksonlopez.dev/dotnet-domain-primitives/samples/OfficialSample/15-AspNetCoreIntegration/Program.cs)
  - [`16-EFCoreIntegration`](file:///d:/DevData/ericksonlopez.dev/dotnet-domain-primitives/samples/OfficialSample/16-EFCoreIntegration/Program.cs)
  - [`17-MediatRIntegration`](file:///d:/DevData/ericksonlopez.dev/dotnet-domain-primitives/samples/OfficialSample/17-MediatRIntegration/Program.cs)
