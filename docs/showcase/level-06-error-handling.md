# Level 6 — Error Handling

This level covers the immutable, zero-allocation error model (`PrimitiveError`), standardized error codes, and seamless integration with Railway-Oriented Programming (ROP).

---

## 1. The Immutable `PrimitiveError` Struct

`PrimitiveError` is a stack-allocated readonly struct (`readonly record struct`) designed to prevent heap allocations when returning validation failures:

```csharp
using EricksonLopez.DomainPrimitives.Validation;

// Standardized error codes emitted by validators:
// - "EMPTY": Value is null, empty, or whitespace-only.
// - "LENGTH": Length violates [MinLength], [MaxLength], or [ExactLength].
// - "FORMAT": Format violates [Regex] or semantic shortcut RFC rules.
// - "RANGE": Value breaches bounds in [PrimitiveRange] or [DateRange].
// - "CUSTOM": Error produced by an ICustomValidator<T> implementation.
```

---

## 2. Integration with Railway-Oriented Programming (`Result<T>`)

Domain primitives integrate seamlessly with monadic Result libraries (e.g. `EricksonLopez.Result`):

```csharp
using EricksonLopez.Result;

public static Result<CustomerEmail> ValidateEmail(string raw)
{
    return CustomerEmail.TryCreate(raw, out var email, out var error)
        ? Result<CustomerEmail>.Success(email)
        : Result<CustomerEmail>.Failure(Error.Validation(error.Code ?? "VALIDATION", error.Message ?? "Invalid email"));
}
```

---

## 3. Unit Testing Failure Scenarios (`DomainPrimitiveTestBuilder`)

The `EricksonLopez.DomainPrimitives.Testing` package allows asserting validation failures without manual `try/catch` boilerplate:

```csharp
using EricksonLopez.DomainPrimitives.Testing;
using Xunit;

public class EmailTests
{
    [Fact]
    public void Should_Fail_When_Email_Is_Invalid()
    {
        // AssertCreationFails asserts that the validation exception is thrown and returns it for inspection
        var ex = DomainPrimitiveTestBuilder.AssertCreationFails<CustomerEmail, string>("invalid-email");
        
        Assert.Equal("FORMAT", ex.Error.Code);
    }

    [Theory]
    [MemberData(nameof(DomainPrimitiveScenarios.InvalidEmailInputs), MemberType = typeof(DomainPrimitiveScenarios))]
    public void Should_Fail_For_All_Invalid_Scenarios(string invalidInput)
    {
        Assert.False(CustomerEmail.TryCreate(invalidInput, out _, out var error));
        Assert.True(error.IsError);
    }
}
```

---

## Showcase Reference
- Executable projects:
  - [`02-FirstResult`](file:///d:/DevData/ericksonlopez.dev/dotnet-domain-primitives/samples/OfficialSample/02-FirstResult/Program.cs)
  - [`03-Errors`](file:///d:/DevData/ericksonlopez.dev/dotnet-domain-primitives/samples/OfficialSample/03-Errors/Program.cs)
  - [`19-UnitTesting`](file:///d:/DevData/ericksonlopez.dev/dotnet-domain-primitives/samples/OfficialSample/19-UnitTesting/Program.cs)
