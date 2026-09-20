# Official Cookbook: Architecture and Domain Recipes

Collection of production technical recipes for the `EricksonLopez.DomainPrimitives` ecosystem.
Every recipe derives directly from the verified public API surface and adheres to the mandatory 6-section structure:

1. **Problem**
2. **Solution**
3. **Full Code**
4. **Explanation**
5. **Best Practices**
6. **Common Pitfalls**

---

## Recipe Index

1. [Preventing Primitive Obsession with Strongly-Typed IDs](#recipe-1--preventing-primitive-obsession-with-strongly-typed-ids)
2. [Automated Email Normalization and Validation](#recipe-2--automated-email-normalization-and-validation)
3. [Financial Modeling and Safe Arithmetic Operations](#recipe-3--financial-modeling-and-safe-arithmetic-operations)
4. [Composite Value Objects with Structural Equality](#recipe-4--composite-value-objects-with-structural-equality)
5. [NativeAOT-Safe Smart Enums](#recipe-5--nativeaot-safe-smart-enums)
6. [High-Performance Bulk Operations and Collection Mapping](#recipe-6--high-performance-bulk-operations-and-collection-mapping)
7. [Custom Algorithmic Validation with ICustomValidator](#recipe-7--custom-algorithmic-validation-with-icustomvalidator)
8. [String Normalization Pipelines with INormalizer](#recipe-8--string-normalization-pipelines-with-inormalizer)
9. [Dynamic Construction and Fluent Validation with PrimitiveBuilder](#recipe-9--dynamic-construction-and-fluent-validation-with-primitivebuilder)
10. [Zero-Configuration Persistence in Entity Framework Core](#recipe-10--zero-configuration-persistence-in-entity-framework-core)
11. [Automated TypeHandler Registration in Dapper](#recipe-11--automated-typehandler-registration-in-dapper)
12. [Seamless Model Binding in ASP.NET Core](#recipe-12--seamless-model-binding-in-aspnet-core)
13. [Accurate Schema Generation in Swagger / OpenAPI](#recipe-13--accurate-schema-generation-in-swagger--openapi)
14. [Legacy Migration and Serialization with Newtonsoft.Json](#recipe-14--legacy-migration-and-serialization-with-newtonsoftjson)
15. [Deterministic Unit Testing and Reusable Scenarios](#recipe-15--deterministic-unit-testing-and-reusable-scenarios)
16. [Observability, Diagnostics, and OpenTelemetry Metrics](#recipe-16--observability-diagnostics-and-opentelemetry-metrics)
17. [Integration with Railway-Oriented Programming (Result Pattern)](#recipe-17--integration-with-railway-oriented-programming-result-pattern)
18. [Concurrent Background Processing with Channels](#recipe-18--concurrent-background-processing-with-channels)

---

## Recipe 1 — Preventing Primitive Obsession with Strongly-Typed IDs

### 1. Problem
Prevent accidental transposition of primitive identifier types (`Guid`, `int`, `long`) across different domain entities (e.g. passing a `CustomerId` into an argument expecting `OrderId`), causing silent database corruption.

### 2. Solution
Declare strongly-typed identifiers using `[StrongId<T>]` over immutable partial structs.

### 3. Full Code
```csharp
using System;
using EricksonLopez.DomainPrimitives;

[StrongId<Guid>]
public readonly partial record struct CustomerId;

[StrongId<Guid>]
public readonly partial record struct OrderId;

public class OrderService
{
    public void CancelOrder(CustomerId customerId, OrderId orderId)
    {
        // Swapping parameters is impossible:
        // CancelOrder(orderId, customerId); -> Compiler Error CS1503
        Console.WriteLine($"Canceling order {orderId.Value} for customer {customerId.Value}");
    }
}
```

### 4. Explanation
The Roslyn generator emits private constructors, factory methods (`New()`, `Create()`, `TryCreate()`), parsing methods (`Parse`, `TryParse`), and explicit conversions. Because `CustomerId` and `OrderId` are distinct nominal types in C#, the compiler prevents cross-assignment.

### 5. Best Practices
- Use `CustomerId.New()` to generate new unique identifiers when creating aggregate roots.
- Use `TryCreate` when parsing identifiers received from external APIs.

### 6. Common Pitfalls
- Declaring the type as `public partial class` instead of `public readonly partial record struct`, forfeiting stack allocation and zero GC pressure.

---

## Recipe 2 — Automated Email Normalization and Validation

### 1. Problem
Ensure email addresses stored throughout the application adhere to RFC 5321/5322 syntax, have accidental leading/trailing whitespace removed, and are canonicalized to lowercase before persistence or authentication lookups.

### 2. Solution
Use the built-in `[Email]` semantic shortcut attribute.

### 3. Full Code
```csharp
using System;
using EricksonLopez.DomainPrimitives;

[Email]
public readonly partial record struct CustomerEmail;

public class AccountRegistration
{
    public static bool Register(string rawInput, out CustomerEmail email)
    {
        if (CustomerEmail.TryCreate(rawInput, out email, out var error))
        {
            return true;
        }

        Console.WriteLine($"Validation Error [{error.Code}]: {error.Message}");
        return false;
    }
}
```

### 4. Explanation
The `[Email]` attribute expands at compile time into:
1. `[Trim]`: Strips leading and trailing whitespace.
2. `[LowerCase]`: Converts characters to invariant lowercase.
3. NativeAOT-compatible RFC regex pattern validation.

### 5. Best Practices
- Persist and compare `CustomerEmail.Value` with confidence that it is in canonical, normalized Form C format.

### 6. Common Pitfalls
- Applying manual `rawInput.Trim().ToLowerInvariant()` calls before invoking `TryCreate`; the primitive performs this automatically.

---

## Recipe 3 — Financial Modeling and Safe Arithmetic Operations

### 1. Problem
Financial calculations often encounter floating-point inaccuracies, negative values in asset balances, or unauthorized arithmetic combinations.

### 2. Solution
Use `[Money]` or `[NumericPrimitive<decimal>]` configured with `NumericOperations`.

### 3. Full Code
```csharp
using System;
using EricksonLopez.DomainPrimitives;

[NumericPrimitive<decimal>(Operations = NumericOperations.Additive)]
[PrimitiveRange(0, 10_000_000)]
public readonly partial record struct WalletBalance;

public class WalletService
{
    public WalletBalance TopUp(WalletBalance current, WalletBalance amount)
    {
        // Operator '+' is inlined and automatically bounds-checked
        return current + amount;
    }
}
```

### 4. Explanation
`NumericOperations.Additive` emits strongly-typed overloads for `+` and `-`. If an arithmetic operation produces a result outside `[PrimitiveRange]`, a `DomainPrimitiveValidationException` is thrown.

### 5. Best Practices
- Restrict enabled arithmetic operations to those strictly required by business invariants (`Additive`, `Multiplicative`, etc.).

### 6. Common Pitfalls
- Using `double` or `float` instead of `decimal` for monetary values, introducing floating-point precision loss.

---

## Recipe 4 — Composite Value Objects with Structural Equality

### 1. Problem
Encapsulate a group of cohesive domain primitives that lack individual identity and whose equality depends strictly on their combined values (e.g. a shipping address).

### 2. Solution
Apply the `[ValueObject]` attribute over an immutable partial record struct with multiple properties.

### 3. Full Code
```csharp
using EricksonLopez.DomainPrimitives;

[ValueObject]
public readonly partial record struct ShippingAddress(
    string Street,
    string City,
    CountryCode Country,
    string PostalCode
);
```

### 4. Explanation
The source generator emits structural equality logic (`IEquatable<T>`), deterministic `GetHashCode`, BCL parsing and formatting interfaces, and equality operators `==` and `!=`.

### 5. Best Practices
- Compose Value Objects out of smaller domain primitives (e.g. `CountryCode` rather than raw `string`).
- Ensure all collection properties use immutable types (`ImmutableArray<T>`) to comply with analyzer rule **DP0018**.

### 6. Common Pitfalls
- Declaring mutable auto-properties with public `set` accessors or mutable collections (`List<T>`).

---

## Recipe 5 — NativeAOT-Safe Smart Enums

### 1. Problem
Native C# enums allow undefined integral casts (`(OrderStatus)999`), lack domain behavior, and traditional reflection-based smart enum libraries fail under NativeAOT compilation.

### 2. Solution
Declare domain enumerations with the `[SmartEnum<TValue>]` attribute.

### 3. Full Code
```csharp
using System;
using EricksonLopez.DomainPrimitives;

[SmartEnum<int>]
public readonly partial record struct OrderStatus
{
    public static readonly OrderStatus Placed = new(1, nameof(Placed));
    public static readonly OrderStatus Paid = new(2, nameof(Paid));
    public static readonly OrderStatus Shipped = new(3, nameof(Shipped));

    public bool CanCancel() => this == Placed;
}
```

### 4. Explanation
The generator inspects public static fields at compile time and emits `All`, `FromValue`, `FromName`, `TryFromValue`, and `Match`, enabling O(1) lookups without runtime reflection.

### 5. Best Practices
- Attach domain behavioral methods directly onto the Smart Enum partial struct.

### 6. Common Pitfalls
- Forgetting the `partial` keyword, preventing the generator from emitting lookup catalogues and pattern matching methods.

---

## Recipe 6 — High-Performance Bulk Operations and Collection Mapping

### 1. Problem
Convert bulk lists or arrays of raw scalar strings/values into immutable collections of validated domain primitives without verbose manual iteration.

### 2. Solution
Use `PrimitiveCollectionExtensions`.

### 3. Full Code
```csharp
using System;
using System.Collections.Generic;
using EricksonLopez.DomainPrimitives;

public class BulkNotificationService
{
    public void NotifyCustomers(IEnumerable<string> rawRecipients)
    {
        IReadOnlyList<CustomerEmail> validatedRecipients = 
            rawRecipients.ToDomainPrimitiveList<CustomerEmail, string>();

        foreach (var recipient in validatedRecipients)
        {
            Console.WriteLine($"Sending notification to {recipient.Value}");
        }
    }
}
```

### 4. Explanation
`ToDomainPrimitiveList` and `ToDomainPrimitiveArray` offer overloads for `IEnumerable<T>` and `ReadOnlySpan<T>`, pre-allocating exact destination capacity to maximize performance.

### 5. Best Practices
- Use the `ReadOnlySpan<T>` overloads when working with contiguous buffers to eliminate intermediate heap allocations.

### 6. Common Pitfalls
- Overlooking that any invalid element in the sequence will cause `DomainPrimitiveValidationException` to be thrown.

---

## Recipe 7 — Custom Algorithmic Validation with ICustomValidator

### 1. Problem
Enforce domain rules requiring complex algorithms (e.g. Luhn check-digit validation or fiscal ID checksum verification) that cannot be expressed via standard attributes.

### 2. Solution
Implement `ICustomValidator<T>` and link it to the primitive via `[CustomValidator<T>]`.

### 3. Full Code
```csharp
using EricksonLopez.DomainPrimitives;
using EricksonLopez.DomainPrimitives.Validation;

public sealed class Modulo11Validator : ICustomValidator<string>
{
    public PrimitiveError Validate(string value)
    {
        if (value.Length != 9)
            return PrimitiveError.Create("LENGTH", "Document must be exactly 9 digits.");

        bool isValid = true; // Algorithmic check

        return isValid
            ? PrimitiveError.None
            : PrimitiveError.Create("CHECKSUM", "Check digit is invalid.");
    }
}

[StringPrimitive]
[CustomValidator<Modulo11Validator>]
public readonly partial record struct DocumentId;
```

### 4. Explanation
The generated validation pipeline runs built-in constraints first and then invokes `ICustomValidator.Validate()`. If it returns `PrimitiveError.None`, instantiation succeeds.

### 5. Best Practices
- Mark validator classes as `sealed` and stateless to guarantee deterministic and thread-safe execution.

### 6. Common Pitfalls
- Throwing exceptions inside `Validate()` rather than returning a structured `PrimitiveError`.

---

## Recipe 8 — String Normalization Pipelines with INormalizer

### 1. Problem
Strip inconsistent characters (e.g. hyphens, dots, or whitespace) prior to invariant validation.

### 2. Solution
Implement `INormalizer<T>` and decorate the primitive with `[Normalize<T>]`.

### 3. Full Code
```csharp
using EricksonLopez.DomainPrimitives;

public sealed class CleanTaxNumberNormalizer : INormalizer<string>
{
    public string Normalize(string value) => value.Replace("-", "").Replace(".", "").Trim();
}

[StringPrimitive]
[Normalize<CleanTaxNumberNormalizer>]
[ExactLength(10)]
public readonly partial record struct TaxNumber;
```

### 4. Explanation
The normalizer executes before evaluating validation attributes (`[ExactLength]`), ensuring the verified length applies to the sanitized value.

### 5. Best Practices
- Ensure `Normalize()` is pure, deterministic, and idempotent.

### 6. Common Pitfalls
- Attempting validation inside `Normalize()`; normalization transforms values, validation enforces rules.

---

## Recipe 9 — Dynamic Construction and Fluent Validation with PrimitiveBuilder

### 1. Problem
Execute dynamic runtime validation checks in test fixtures or administrative migration tools.

### 2. Solution
Use `PrimitiveBuilder<TPrimitive, TValue>`.

### 3. Full Code
```csharp
using System;
using EricksonLopez.DomainPrimitives.Advanced;

public class DynamicOrderFactory
{
    public static CustomerEmail CreateCorporateEmail(string rawEmail)
    {
        return PrimitiveBuilder<CustomerEmail, string>.For()
            .WithValue(rawEmail)
            .Must(e => e.EndsWith("@acme-corp.com"), "DOMAIN", "Email must belong to acme-corp.com")
            .BuildOrThrow();
    }
}
```

### 4. Explanation
`PrimitiveBuilder` provides a fluent API evaluating chained predicates, returning the valid instance or throwing `DomainPrimitiveValidationException`.

### 5. Best Practices
- Use `Build(out var result)` in production paths to avoid throwing exceptions on untrusted inputs.

### 6. Common Pitfalls
- Using `PrimitiveBuilder` inside high-throughput loops instead of direct `TryCreate`, as the builder allocates a small heap instance.

---

## Recipe 10 — Zero-Configuration Persistence in Entity Framework Core

### 1. Problem
Eliminate manual boilerplate when registering dozens of `ValueConverter` configurations in EF Core's `OnModelCreating`.

### 2. Solution
Reference `EricksonLopez.DomainPrimitives.EFCore` and invoke `ConfigureDomainPrimitives()` inside `ConfigureConventions`.

### 3. Full Code
```csharp
using EricksonLopez.DomainPrimitives.EFCore.Generated;
using Microsoft.EntityFrameworkCore;

public class StoreDbContext : DbContext
{
    public DbSet<Product> Products => Set<Product>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Automatically discovers and maps all ValueConverters, MaxLengths, and precisions
        configurationBuilder.ConfigureDomainPrimitives();
    }
}
```

### 4. Explanation
The generator produces compile-time sealed value converters for every primitive and registers them in bulk via `configurationBuilder.Properties<T>()`.

### 5. Best Practices
- Call `ConfigureDomainPrimitives()` inside `ConfigureConventions` rather than `OnModelCreating` so conventions apply uniformly across the model.

### 6. Common Pitfalls
- Using legacy per-type `[EFCore]` attributes; convention configuration is fully automated at the project level.

---

## Recipe 11 — Automated TypeHandler Registration in Dapper

### 1. Problem
Dapper does not recognize custom structs by default, resulting in conversion errors when mapping SQL query results to domain models.

### 2. Solution
Reference `EricksonLopez.DomainPrimitives.Dapper` and call `DapperDomainPrimitivesRegistration.RegisterAll()` at application startup.

### 3. Full Code
```csharp
using Dapper;
using EricksonLopez.DomainPrimitives.Dapper.Generated;

// Call once during application host bootstrapping
DapperDomainPrimitivesRegistration.RegisterAll();

// Native query execution with automatic mapping
var order = await connection.QuerySingleAsync<OrderRecord>(
    "SELECT Id, CustomerEmail, Amount FROM Orders WHERE Id = @Id",
    new { Id = orderId.Value });
```

### 4. Explanation
`RegisterAll()` registers generated `SqlMapper.TypeHandler<T>` instances for every primitive in the compilation and referenced assemblies. It is idempotent and thread-safe.

### 5. Best Practices
- Call registration in `Program.cs` before opening SQL connections.

### 6. Common Pitfalls
- Expecting Dapper to handle `DBNull` into a non-nullable primitive struct; if the SQL column is nullable, declare the C# property as nullable (`CustomerId?`).

---

## Recipe 12 — Seamless Model Binding in ASP.NET Core

### 1. Problem
Allow Minimal APIs and MVC Controllers to bind route and query parameters directly into strongly-typed primitives without manual parsing boilerplate.

### 2. Solution
Register `AddDomainPrimitivesModelBinding()` in `IServiceCollection`.

### 3. Full Code
```csharp
using EricksonLopez.DomainPrimitives.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDomainPrimitivesModelBinding();

var app = builder.Build();

app.MapGet("/invoices/{invoiceId}", (InvoiceNumber invoiceId) =>
{
    return Results.Ok(new { Invoice = invoiceId.Value });
});

app.Run();
```

### 4. Explanation
The model binder provider intercepts route and query parameters, binding them via `T.Parse()` or `T.TryCreate()`. If validation fails, ASP.NET Core returns a standard `400 Bad Request`.

### 5. Best Practices
- Combine with Minimal APIs to expose clean, strongly-typed endpoint signatures.

### 6. Common Pitfalls
- Forgetting to invoke `AddDomainPrimitivesModelBinding()`, causing ASP.NET Core to attempt complex model binding on the struct.

---

## Recipe 13 — Accurate Schema Generation in Swagger / OpenAPI

### 1. Problem
Swagger generates complex nested object schemas with a `Value` property for domain primitives, whereas they serialize to JSON as simple scalar values.

### 2. Solution
Register `DomainPrimitivesSchemaFilter` in the SwaggerGen options.

### 3. Full Code
```csharp
using EricksonLopez.DomainPrimitives.OpenApi.Generated;
using Microsoft.Extensions.DependencyInjection;

builder.Services.AddSwaggerGen(options =>
{
    options.SchemaFilter<DomainPrimitivesSchemaFilter>();
});
```

### 4. Explanation
The generated `ISchemaFilter` rewrites OpenAPI schemas for primitives to their underlying scalar representation (`string`, `number`, `format: uuid`).

### 5. Best Practices
- Combine with Smart Enums so Swagger enumerates valid enum options in documentation.

### 6. Common Pitfalls
- Adding conflicting custom schema filters that overwrite the generated scalar schema.

---

## Recipe 14 — Legacy Migration and Serialization with Newtonsoft.Json

### 1. Problem
Integrate domain primitives into legacy projects requiring `Newtonsoft.Json` (Json.NET).

### 2. Solution
Use the `AddDomainPrimitives()` extension method.

### 3. Full Code
```csharp
using EricksonLopez.DomainPrimitives.NewtonsoftJson;
using Newtonsoft.Json;

var settings = new JsonSerializerSettings()
    .AddDomainPrimitives();

string json = JsonConvert.SerializeObject(new OrderDto(OrderId.New()), settings);
OrderDto? order = JsonConvert.DeserializeObject<OrderDto>(json, settings);
```

### 4. Explanation
The package registers `DomainPrimitiveUniversalNewtonsoftJsonConverter`, serializing and deserializing domain primitives transparently.

### 5. Best Practices
- Configure the resolver once via `JsonConvert.DefaultSettings`.

### 6. Common Pitfalls
- Expecting NativeAOT compatibility with `Newtonsoft.Json`; Newtonsoft relies on runtime reflection. For NativeAOT, use `System.Text.Json` (per ADR-026).

---

## Recipe 15 — Deterministic Unit Testing and Reusable Scenarios

### 1. Problem
Avoid duplicating valid and invalid test input sets across multiple unit test projects.

### 2. Solution
Use `DomainPrimitiveFakeFactory` and `DomainPrimitiveScenarios`.

### 3. Full Code
```csharp
using EricksonLopez.DomainPrimitives.Testing;
using Xunit;

public class CustomerPrimitiveTests
{
    [Theory]
    [MemberData(nameof(DomainPrimitiveScenarios.ValidEmailInputs), MemberType = typeof(DomainPrimitiveScenarios))]
    public void ValidEmails_ShouldPassValidation(string rawEmail)
    {
        Assert.True(CustomerEmail.TryCreate(rawEmail, out var email, out _));
        Assert.NotNull(email.Value);
    }

    [Fact]
    public void TestBuilder_AssertFailure()
    {
        var ex = DomainPrimitiveTestBuilder.AssertCreationFails<CustomerEmail, string>("bad-email");
        Assert.Equal("FORMAT", ex.Error.Code);
    }
}
```

### 4. Explanation
`DomainPrimitiveFakeFactory` provides static, deterministic collections of test inputs for strings, numbers, dates, and semantic shortcuts.

### 5. Best Practices
- Use `[MemberData]` with `DomainPrimitiveScenarios` for boundary test coverage in parameterized test theories.

### 6. Common Pitfalls
- Relying on non-deterministic random data generators (e.g. Bogus) in invariant unit tests, causing sporadic and irreproducible test failures.

---

## Recipe 16 — Observability, Diagnostics, and OpenTelemetry Metrics

### 1. Problem
Monitor creation throughput and validation failure rates in production to detect security anomalies or client integration defects.

### 2. Solution
Subscribe to `DomainPrimitiveEventSource` or export metrics via `DomainPrimitivesMetrics`.

### 3. Full Code
```csharp
using System;
using EricksonLopez.DomainPrimitives.Diagnostics;

// 1. Subscribe to validation failures without DI coupling
DomainPrimitiveEventSource.OnValidationFailed += (sender, args) =>
{
    Console.WriteLine($"[ALERT] Validation failed for {args.PrimitiveName}: [{args.ErrorType}] {args.ErrorMessage}");
};

// 2. Record creation telemetry
DomainPrimitivesMetrics.RecordCreation("CustomerEmail");
```

### 4. Explanation
DomainPrimitives includes an OpenTelemetry `Meter` (`EricksonLopez.DomainPrimitives`) and a `DiagnosticSource` that emit success and failure events with zero performance overhead when no subscribers are attached.

### 5. Best Practices
- Add `EricksonLopez.DomainPrimitives` to the listened meters in `AddOpenTelemetry().WithMetrics(...)`.

### 6. Common Pitfalls
- Throwing exceptions inside the `OnValidationFailed` callback, which disrupts core application execution.

---

## Recipe 17 — Integration with Railway-Oriented Programming (Result Pattern)

### 1. Problem
Model functional pipelines without throwing control-flow exceptions when primitive creation fails at application boundaries.

### 2. Solution
Integrate `TryCreate` with a `Result<T>` pattern.

### 3. Full Code
```csharp
using EricksonLopez.DomainPrimitives;
using EricksonLopez.Result;

public class CustomerService
{
    public Result<CustomerEmail> ParseEmail(string raw)
    {
        return CustomerEmail.TryCreate(raw, out var email, out var error)
            ? Result<CustomerEmail>.Success(email)
            : Result<CustomerEmail>.Failure(Error.Validation(error.Code ?? "VALIDATION", error.Message ?? "Invalid email"));
    }
}
```

### 4. Explanation
`TryCreate` returns a boolean and outputs a stack-allocated `PrimitiveError`, making it the ideal building block for monadic `Bind` and `Map` operations without heap allocations.

### 5. Best Practices
- Map `PrimitiveError.Code` directly into your application's structured error taxonomy.

### 6. Common Pitfalls
- Wrapping calls to `Create()` in `try/catch` blocks to construct a `Result`; always use `TryCreate`.

---

## Recipe 18 — Concurrent Background Processing with Channels

### 1. Problem
Process high-volume asynchronous message queues and guarantee that malformed or corrupted messages never compromise domain state.

### 2. Solution
Reconstruct and validate primitives using `TryCreate` at the consumer boundary of `Channel<T>`.

### 3. Full Code
```csharp
using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using EricksonLopez.DomainPrimitives;

public record OrderQueueMessage(string OrderId, string Email, decimal Amount);

public class OrderQueueProcessor
{
    public async Task ProcessQueueAsync(ChannelReader<OrderQueueMessage> reader, CancellationToken ct)
    {
        await foreach (var msg in reader.ReadAllAsync(ct))
        {
            if (!OrderId.TryCreate(Guid.Parse(msg.OrderId), out var orderId, out var idError))
            {
                // Handle corrupted payload / Dead-letter
                continue;
            }

            if (!CustomerEmail.TryCreate(msg.Email, out var email, out var emailError))
            {
                continue;
            }

            // Safe domain execution
            Console.WriteLine($"Processing order {orderId.Value} for {email.Value}");
        }
    }
}
```

### 4. Explanation
Re-validating with `TryCreate` at the point of worker ingestion protects the domain core regardless of the message origin or transport mechanism.

### 5. Best Practices
- Forward invalid messages to a Dead-Letter Queue along with their structured error code for auditing.

### 6. Common Pitfalls
- Assuming queued messages are already valid; domain invariants must be re-asserted when crossing process boundaries.
