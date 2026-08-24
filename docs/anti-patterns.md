# Anti-Patterns & Common Pitfalls

---

## 1. Prohibited Anti-Patterns in Domain Primitives

### ❌ Anti-Pattern 1: Throwing Exceptions in Domain Primitive Creation
```csharp
// BAD: Throwing exceptions for business validation
public static EmailAddress Create(string value)
{
    if (!value.Contains('@')) throw new ArgumentException("Invalid email");
    return new EmailAddress(value);
}

// GOOD: Use Result Pattern / TryCreate
public static Result<EmailAddress, ValidationError> Create(string value) { ... }
```

### ❌ Anti-Pattern 2: Class-Based Primitives Causing Heap Churn
```csharp
// BAD: Class wrappers allocate 24-32B per entity ID on heap
public class CustomerId { public Guid Value { get; set; } }

// GOOD: readonly partial struct with zero allocations
[DomainPrimitive<Guid>]
public readonly partial struct CustomerId;
```
