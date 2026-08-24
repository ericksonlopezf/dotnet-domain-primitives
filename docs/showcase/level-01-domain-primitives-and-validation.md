# Level 01 — Domain Primitives & Result-First Validation

In Level 01, we define custom scalar domain primitives with functional validation.

---

## 1. Defining a Domain Primitive

```csharp
using EricksonLopez.DomainPrimitives;
using EricksonLopez.Result;

[DomainPrimitive<string>]
public readonly partial struct EmailAddress
{
    private static Result<string, ValidationError> Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<string, ValidationError>(new ValidationError("Email cannot be empty."));

        if (!value.Contains('@') || !value.Contains('.'))
            return Result.Failure<string, ValidationError>(new ValidationError("Email format is invalid."));

        return Result.Success<string, ValidationError>(value.Trim().ToLowerInvariant());
    }
}
```

---

## 2. Parsing & Instantiation

```csharp
Result<EmailAddress, ValidationError> result = EmailAddress.Create("user@domain.com");

if (result.IsSuccess)
{
    EmailAddress email = result.Value;
    Console.WriteLine(email.Value);
}
```
