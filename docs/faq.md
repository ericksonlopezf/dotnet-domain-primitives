# Frequently Asked Questions (FAQ)

---

### 1. Why must domain primitives be declared as `readonly partial record struct`?
Because this declaration combines three essential technical characteristics:
1. **`readonly`**: Guarantees absolute mathematical immutability after creation.
2. **`partial`**: Allows the Roslyn Source Generator to emit constructors, validators, operators, formatters, and type converters at compile time.
3. **`record struct`**: Provides native value equality semantics and allocates directly on the stack, ensuring zero heap allocations and zero garbage collection pressure on the hot path.

---

### 2. How does the library validate business rules without reflection?
The Roslyn Source Generator inspects declarations and validation attributes (`[NotEmpty]`, `[MaxLength]`, `[Email]`, `[Range]`, etc.) at design/build time and emits pure, imperative C# code directly into the static `TryCreate` method and private constructors. At runtime, only direct scalar checks execute, with zero calls to `Type.GetCustomAttributes()` or runtime reflection.

---

### 3. Is it compatible with NativeAOT in .NET 8, .NET 9, and .NET 10?
**Yes, 100%.** All generated domain primitives, inline `System.Text.Json` converters, `EFCore` value converters, `Dapper` type handlers, and `OpenApi` schema filters are engineered specifically to comply with IL trimming and Ahead-Of-Time (AOT) compilation rules without emitting dynamic code warnings (`IL2026`, `IL2087`).
*(Note: The optional `NewtonsoftJson` package relies on Newtonsoft's internal reflection by design, so `System.Text.Json` must be chosen for NativeAOT targets per ADR-026).*

---

### 4. What if validation requires querying a database or an external service?
**Domain primitives must never perform asynchronous operations or I/O.**
A domain primitive encapsulates **intrinsic invariants** (e.g. "an email must follow valid RFC syntax", "a monetary amount cannot be negative").
Contextual or extrinsic validations (e.g. "is this email already registered in our database?", "does the customer have sufficient account balance?") are the responsibility of **Domain Services** or **Application Handlers**.

---

### 5. How are primitives serialized in JSON? Do they appear as `{ "Value": "..." }` objects?
**No.** The source-generated `System.Text.Json` converters serialize primitives directly as their underlying scalar value:
```json
// Customer record containing CustomerId and CustomerEmail serialized:
{
  "id": "c1f7a29e-29df-419b-a320-b4845564c781",
  "email": "customer@example.com"
}
```

---

### 6. Why was direct integration with FluentValidation discontinued (RFC-0004)?
FluentValidation is designed for validating mutable application DTOs and command payloads. Coupling domain primitives to FluentValidation introduced heavy external dependencies, heap allocations, and violated the core DDD principle that domain invariants must be self-contained and autonomous within the type. The recommended pattern is using `TryCreate` at the application boundary to parse raw input without external dependencies.

---

### 7. How should a primitive be instantiated when loading trusted data from a database?
Use the standard factory method:
```csharp
var customerId = CustomerId.Create(dbGuid);
```
If you need to construct unvalidated instances specifically in unit test fixtures with legacy or malformed test data, use `DomainPrimitiveTestBuilder.CreateUnvalidated<TPrimitive, TValue>(rawValue)`.

---

### 8. How does `TryCreate` integrate with the Result pattern and Railway-Oriented Programming?
`TryCreate` integrates cleanly without allocations:
```csharp
Result<CustomerEmail> result = CustomerEmail.TryCreate(raw, out var email, out var error)
    ? Result<CustomerEmail>.Success(email)
    : Result<CustomerEmail>.Failure(Error.Validation(error.Code, error.Message));
```
The stack-allocated `PrimitiveError` struct provides `Code` and `Message` properties to populate your custom `Result` error payload with zero heap overhead.
