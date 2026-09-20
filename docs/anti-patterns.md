# Anti-Patterns and Common Pitfalls

This document outlines prohibited anti-patterns, design pitfalls, and best practices when designing and consuming domain primitives within `EricksonLopez.DomainPrimitives`.

---

## 1. Prohibited Anti-Patterns

### ❌ Anti-Pattern 1: Bypassing Compile-Time Analyzers with Class Wrappers

Using reference types (`class`) for IDs or scalar domain primitives induces unnecessary GC pressure and heap churn (24–32 bytes overhead per instance).

```csharp
// BAD: Class wrappers allocate memory on the heap and lose value semantics
public class CustomerId
{
    public Guid Value { get; set; }
}

// GOOD: Use readonly partial record struct with [StrongId] (0 bytes heap allocation)
[StrongId<Guid>]
public readonly partial struct CustomerId;
```

---

### ❌ Anti-Pattern 2: Public Constructors Bypassing Invariants

Domain primitives must never expose public constructors that allow callers to instantiate them without executing validation logic.

```csharp
// BAD: Public constructor bypasses factory invariants (Triggers DP0004)
[StringPrimitive(MinLength = 3, MaxLength = 50)]
public readonly partial struct Username
{
    public Username(string value) => Value = value; // Violates encapsulation
}

// GOOD: Allow the source generator to emit private constructors.
// Use Create() for fail-fast exceptions or TryCreate() for non-throwing validation.
var user = Username.Create("john_doe");
if (Username.TryCreate(input, out var validatedUser, out var error))
{
    // Consume validatedUser
}
```

---

### ❌ Anti-Pattern 3: Mutable State or Collections in Value Objects

Value objects must be immutable. Exposing mutable collection types like `List<T>`, `Dictionary<TKey, TValue>`, or arrays violates value semantics and triggers compiler diagnostic **DP0018**.

```csharp
// BAD: Mutable collection allows post-creation state mutation (Triggers DP0018)
[ValueObject]
public readonly partial record struct OrderLineItem
{
    public string Sku { get; init; }
    public List<string> Tags { get; init; } // DP0018: Mutable collection
}

// GOOD: Use ImmutableArray<T>, IReadOnlyList<T>, or immutable sequences
[ValueObject]
public readonly partial record struct OrderLineItem
{
    public string Sku { get; init; }
    public System.Collections.Immutable.ImmutableArray<string> Tags { get; init; }
}
```

---

### ❌ Anti-Pattern 4: Ignoring Uninitialized `default(T)` Structs

Structs in .NET can always be instantiated using `default(T)`. Consuming an uninitialized struct without verifying `.IsDefault` can lead to invalid domain state.

```csharp
// BAD: Assuming struct is always validly initialized
public void ProcessOrder(OrderId orderId, Amount amount)
{
    // If orderId is default(OrderId), it was never created through Create/TryCreate!
    _repository.Load(orderId);
}

// GOOD: Check .IsDefault if there is any chance of uninitialized struct passing
public void ProcessOrder(OrderId orderId, Amount amount)
{
    if (orderId.IsDefault)
        throw new InvalidOperationException("OrderId must be properly initialized.");

    _repository.Load(orderId);
}
```

---

### ❌ Anti-Pattern 5: Using Exceptions for Expected Control Flow

While `Create()` throws `ValidationException` for fail-fast scenarios, throwing and catching exceptions on hot API ingestion paths creates high CPU overhead.

```csharp
// BAD: Catching exceptions for expected user validation
try
{
    var email = EmailAddress.Create(userInput);
    return Results.Ok(email);
}
catch (ValidationException ex)
{
    return Results.BadRequest(ex.Message);
}

// GOOD: Use TryCreate for zero-allocation, non-throwing validation
if (!EmailAddress.TryCreate(userInput, out var email, out var error))
{
    return Results.BadRequest(new { error.Code, error.Message });
}
return Results.Ok(email);
```

---

### ❌ Anti-Pattern 6: Manual Regex Instantiation in Primitives

Instantiating `Regex` at runtime inside custom validation methods wastes memory and risks Regular Expression Denial of Service (ReDoS).

```csharp
// BAD: Manual regex without timeouts or source generation
public static bool Validate(string value)
{
    var regex = new Regex(@"^[a-z0-9]+$"); // Inefficient, prone to ReDoS
    return regex.IsMatch(value);
}

// GOOD: Leverage [StringPrimitive] pattern attributes
// The generator automatically injects RegexOptions.NonBacktracking (NET7+)
// and a strict 100ms timeout.
[StringPrimitive(Pattern = @"^[a-z0-9]+$")]
public readonly partial struct AlphaCode;
```

---

## 2. Summary of Roslyn Analyzer Guardrails (DP0001–DP0018)

| Diagnostic | Anti-Pattern Prevented |
|------------|------------------------|
| **DP0001** | Primitive must be declared as a `partial` type |
| **DP0002** | Primitive must be declared as `readonly` struct or `record struct` |
| **DP0004** | Primitive must not declare explicit public constructors |
| **DP0017** | Custom exception type in `DomainPrimitivesDefaults` must inherit from `Exception` |
| **DP0018** | ValueObject properties must not use mutable collection types (`List<T>`, arrays) |
