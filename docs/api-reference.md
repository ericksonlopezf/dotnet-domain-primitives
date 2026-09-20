# API Reference — EricksonLopez.DomainPrimitives

Comprehensive technical documentation adhering to **Microsoft Learn** standards for the core types, methods, and extensibility points of the `EricksonLopez.DomainPrimitives` ecosystem.

---

## 1. `IDomainPrimitive<TSelf, TValue>.Create`

### Signature
```csharp
public static abstract TSelf Create(TValue value);
```

### Parameters
- `value` (`TValue`): The raw underlying scalar value to be evaluated, normalized, and encapsulated within the domain primitive.

### Returns
- `TSelf`: An immutable domain primitive instance with all invariants validated.

### Exceptions
- `DomainPrimitiveValidationException`: Thrown when `value` violates any declared constraint or invariant (e.g. `[NotEmpty]`, `[MaxLength]`, `[PrimitiveRange]`, or custom `ICustomValidator<T>` implementations).
- `ArgumentNullException`: Thrown when `value` is null in primitives backed by non-nullable reference types.

### Remarks
`Create` is the fail-fast factory method. It sequentially executes declared normalizers (`INormalizer<T>`, `[Trim]`, `[LowerCase]`), followed by built-in validation attributes, and finally custom validators (`ICustomValidator<T>`).

### Basic Example
```csharp
// Direct creation from a valid string
var email = CustomerEmail.Create("user@example.com");
Console.WriteLine(email.Value); // "user@example.com"
```

### Advanced Example
```csharp
// Fail-fast construction with structured error capture
try
{
    var price = OrderPrice.Create(-10.5m);
}
catch (DomainPrimitiveValidationException ex)
{
    Console.WriteLine($"Business rule violation [{ex.Error.Code}]: {ex.Error.Message}");
}
```

### Best Practices
- Use inside domain entity constructors, aggregates, and internal factories where data originates from trusted sources or persistence layers.

### Performance
Stack allocated (0 bytes on heap). Execution overhead is limited to invariant regex evaluation or scalar range comparisons.

### Common Pitfalls
- Using `Create` to ingest untrusted user input without a `try/catch` block, causing unhandled 500 errors instead of clean 400 Bad Request responses.

### When to Use
- Within the domain core, entities, or aggregates where inputs have already passed boundary validation.

### When Not to Use
- In HTTP controllers, Minimal APIs, or message queue consumers; use `TryCreate` instead.

---

## 2. `IDomainPrimitive<TSelf, TValue>.TryCreate`

### Signature
```csharp
public static abstract bool TryCreate(TValue value, out TSelf result, out PrimitiveError error);
```

### Parameters
- `value` (`TValue`): The unprocessed raw scalar value.
- `result` (`out TSelf`): Output parameter containing the instantiated primitive on success, or `default(TSelf)` on failure.
- `error` (`out PrimitiveError`): Output parameter containing failure details (`Code`, `Message`), or `PrimitiveError.None` on success.

### Returns
- `bool`: `true` if the value satisfies all invariants; otherwise, `false`.

### Exceptions
- **None**. Guarantees a zero-exception execution path.

### Remarks
Forms the foundation of Railway-Oriented Programming within the ecosystem. Executes the complete normalization and validation pipeline without incurring CLR exception-throwing overhead.

### Basic Example
```csharp
if (CustomerEmail.TryCreate("invalid-email", out var email, out var error))
{
    Console.WriteLine($"Valid email: {email.Value}");
}
else
{
    Console.WriteLine($"Rejected [{error.Code}]: {error.Message}");
}
```

### Advanced Example
```csharp
// Seamless integration with the Result pattern
public Result<OrderId> ResolveOrder(string input)
{
    return Guid.TryParse(input, out var guid) && OrderId.TryCreate(guid, out var orderId, out var error)
        ? Result<OrderId>.Success(orderId)
        : Result<OrderId>.Failure(Error.Validation(error.Code ?? "INVALID_ID", error.Message ?? "Invalid Order ID"));
}
```

### Best Practices
- Always verify the boolean return value before accessing `result.Value`.
- Avoid re-throwing exceptions when handling boundary input; map `PrimitiveError` directly into `Result.Failure` or `ProblemDetails`.

### Performance
**Zero heap allocations** on both success and failure paths (`PrimitiveError` is a stack-allocated struct).

### Common Pitfalls
- Discarding the `out error` parameter and returning generic error messages to consumers.

### When to Use
- At application ingestion perimeters (JSON deserialization, ASP.NET Core model binders, async queue consumers).

### When Not to Use
- Static initialization code where throwing an exception on contract violation is mandatory.

---

## 3. `IStrongId<TSelf, TValue>.New` / `Create`

### Signature
```csharp
public static abstract TSelf New();
public static abstract TSelf Create();
```

### Parameters
- None.

### Returns
- `TSelf`: A new strongly-typed identifier instance backed by a newly generated `Guid` (`Guid.NewGuid()`).

### Exceptions
- None.

### Remarks
Available exclusively on strongly-typed identifiers declared with `[StrongId<Guid>]` or `[StrongId]`.

### Basic Example
```csharp
CustomerId newCustomerId = CustomerId.New();
Console.WriteLine($"New customer: {newCustomerId.Value}");
```

### Advanced Example
```csharp
public class Order
{
    public OrderId Id { get; }

    public Order()
    {
        Id = OrderId.New();
    }
}
```

### Best Practices
- Use `New()` or parameterless `Create()` when instantiating new entities or aggregates in the domain.

### Performance
Zero heap allocations. Direct invocation of `Guid.NewGuid()`.

### Common Pitfalls
- Using `default(CustomerId)` instead of `CustomerId.New()`, producing an uninitialized `Guid.Empty`.

### When to Use
- When generating new entity instances prior to persisting them in a data store.

### When Not to Use
- When reconstructing existing entities from database records; use `CustomerId.Create(dbGuid)`.

---

## 4. `ISpanParsable<TSelf>.Parse` and `TryParse`

### Signature
```csharp
public static abstract TSelf Parse(ReadOnlySpan<char> s, IFormatProvider? provider);
public static abstract bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out TSelf result);
```

### Parameters
- `s` (`ReadOnlySpan<char>`): The character span to parse.
- `provider` (`IFormatProvider?`): Optional culture-specific format provider.
- `result` (`out TSelf`): Output parsed instance.

### Returns
- `TSelf` (for `Parse`) or `bool` (for `TryParse`).

### Exceptions
- `FormatException`: Thrown by `Parse` if the span fails format or invariant validation (per RFC-0003).

### Remarks
Enables high-throughput parsing of memory buffers and streams without intermediate string allocations.

### Basic Example
```csharp
ReadOnlySpan<char> slice = "user@domain.com".AsSpan();
if (CustomerEmail.TryParse(slice, null, out var email))
{
    Console.WriteLine(email.Value);
}
```

### Advanced Example
```csharp
// Parsing a comma-delimited network buffer
void ProcessBuffer(ReadOnlySpan<char> buffer)
{
    int separatorIndex = buffer.IndexOf(',');
    var emailSpan = buffer.Slice(0, separatorIndex);
    var email = CustomerEmail.Parse(emailSpan, null);
}
```

### Best Practices
- Use in high-throughput, low-latency ingestion pipelines.

### Performance
Zero heap allocations throughout the parsing pipeline.

### Common Pitfalls
- Calling `.ToString()` on a `ReadOnlySpan<char>` before calling `TryParse`, eliminating zero-allocation benefits.

### When to Use
- In custom deserializers, binary protocols, or delimited text processing.

### When Not to Use
- When an existing `string` instance is already available and no slicing is needed.

---

## 5. `PrimitiveCollectionExtensions.ToDomainPrimitiveList`

### Signature
```csharp
public static IReadOnlyList<TPrimitive> ToDomainPrimitiveList<TPrimitive, TValue>(
    this IEnumerable<TValue> source)
    where TPrimitive : struct, IDomainPrimitive<TPrimitive, TValue>
    where TValue : notnull;
```

### Parameters
- `source` (`IEnumerable<TValue>`): Raw scalar input collection.

### Returns
- `IReadOnlyList<TPrimitive>`: Read-only list of validated domain primitives.

### Exceptions
- `DomainPrimitiveValidationException`: If any element in the collection violates its invariants.
- `ArgumentNullException`: If `source` is null.

### Remarks
Pre-allocates list capacity when `source` implements `ICollection<T>` to avoid dynamic resizing.

### Basic Example
```csharp
var rawCodes = new[] { "US", "ES", "FR" };
IReadOnlyList<CountryCode> countryCodes = rawCodes.ToDomainPrimitiveList<CountryCode, string>();
```

### Advanced Example
```csharp
// Safe transformation from bulk DTO payloads
public void ImportCustomers(List<string> incomingEmails)
{
    var validatedList = incomingEmails.ToDomainPrimitiveList<CustomerEmail, string>();
    _repository.BulkInsert(validatedList);
}
```

### Best Practices
- Ensure source data is pre-validated or comes from trusted storage to avoid abrupt exceptions.

### Performance
Single list allocation with pre-computed capacity.

### Common Pitfalls
- Manually writing `.Select(x => T.Create(x)).ToList()` instead of using this optimized extension.

### When to Use
- Batch imports, batch mapping from DTO collections to domain models.

### When Not to Use
- Unbounded streaming sequences or reactive async streams.

---

## 6. `PrimitiveBuilder<TPrimitive, TValue>`

### Signature
```csharp
public sealed class PrimitiveBuilder<TPrimitive, TValue>
{
    public static PrimitiveBuilder<TPrimitive, TValue> For();
    public PrimitiveBuilder<TPrimitive, TValue> WithValue(TValue value);
    public PrimitiveBuilder<TPrimitive, TValue> Must(Func<TValue, bool> predicate, string errorCode, string errorMessage);
    public bool Build(out TPrimitive result);
    public TPrimitive BuildOrThrow();
}
```

### Parameters
- `predicate`: Delegate evaluating a domain condition.
- `errorCode`: Machine-readable error code.
- `errorMessage`: Human-readable error description.

### Returns
- Builder instance for fluent chaining, or the constructed primitive.

### Exceptions
- `DomainPrimitiveValidationException`: Thrown by `BuildOrThrow` if any predicate fails.

### Remarks
Allows injecting runtime ad-hoc validation rules that supplement compile-time declared attributes.

### Basic Example
```csharp
var email = PrimitiveBuilder<CustomerEmail, string>.For()
    .WithValue("admin@corp.com")
    .Must(e => !e.StartsWith("test"), "TEST_REJECTED", "Test emails are not allowed")
    .BuildOrThrow();
```

### Best Practices
- Use primarily in unit testing fixtures and dynamic data migration utilities.

### Performance
Allocates a lightweight builder instance on the heap. Avoid inside high-frequency tight loops.

### Common Pitfalls
- Forgetting to invoke `WithValue()` before calling `Build()`.

### When to Use
- Dynamic validation with rules configured at runtime.

### When Not to Use
- Core business logic where invariants are static and belong directly on the type definition.

---

## 7. `DomainPrimitivesEFCoreExtensions.ConfigureDomainPrimitives`

### Signature
```csharp
public static void ConfigureDomainPrimitives(this ModelConfigurationBuilder configurationBuilder);
```

### Parameters
- `configurationBuilder` (`ModelConfigurationBuilder`): EF Core model configuration builder.

### Returns
- `void`.

### Exceptions
- `ArgumentNullException`: If `configurationBuilder` is null.

### Remarks
Call inside the `ConfigureConventions` method of your `DbContext`. Automatically registers compile-time `ValueConverter` instances, column max lengths, and precision settings for all domain primitives in the model.

### Basic Example
```csharp
public class MyDbContext : DbContext
{
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.ConfigureDomainPrimitives();
    }
}
```

### Best Practices
- Avoid redundant manual `HasConversion` calls in `OnModelCreating` for types handled by this extension.

### Performance
Zero runtime overhead; converters and configurations are resolved at model build time.

### Common Pitfalls
- Invoking this method inside `OnModelCreating` instead of `ConfigureConventions`.

### When to Use
- In any relational persistence project utilizing EF Core.

### When Not to Use
- Projects utilizing micro-ORMs (e.g. Dapper) without Entity Framework Core.

---

## 8. `DapperDomainPrimitivesRegistration.RegisterAll`

### Signature
```csharp
public static void RegisterAll();
```

### Parameters
- None.

### Returns
- `void`.

### Exceptions
- None.

### Remarks
Idempotent and thread-safe. Registers all compile-time generated `SqlMapper.TypeHandler<T>` instances with Dapper.

### Basic Example
```csharp
// In Program.cs
DapperDomainPrimitivesRegistration.RegisterAll();
```

### Best Practices
- Call once during application bootstrapping before opening SQL connections.

### Performance
One-time in-memory registration; zero per-query overhead.

### Common Pitfalls
- Forgetting registration prior to executing the first Dapper query containing domain primitives.

### When to Use
- Applications and services persisting domain primitives via Dapper.

### When Not to Use
- Applications exclusively utilizing EF Core.

---

## 9. `DomainPrimitivesMvcBuilderExtensions.AddDomainPrimitivesModelBinding`

### Signature
```csharp
public static IServiceCollection AddDomainPrimitivesModelBinding(this IServiceCollection services);
public static MvcOptions AddDomainPrimitivesModelBinding(this MvcOptions options);
```

### Parameters
- `services` or `options`: Service collection or MVC options configuration.

### Returns
- The same instance for fluent chaining.

### Exceptions
- `ArgumentNullException`: If the extension target is null.

### Remarks
Registers the compile-time generated `IModelBinderProvider` that binds route parameters, query strings, and form values directly to domain primitives without reflection.

### Basic Example
```csharp
builder.Services.AddDomainPrimitivesModelBinding();
```

### Best Practices
- Use in combination with both Minimal APIs and MVC Controllers.

### Performance
Direct, reflection-free model binding on HTTP requests.

### Common Pitfalls
- Writing manual custom model binders for domain primitive types.

### When to Use
- In all ASP.NET Core Web API and Minimal API projects consuming domain primitives.

### When Not to Use
- Console applications, class libraries, or background worker services without web endpoints.
