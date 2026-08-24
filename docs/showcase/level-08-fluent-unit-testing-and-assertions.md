# Level 08 — Fluent Testing & Quality Assertions

In Level 08, we write isolated unit tests using `EricksonLopez.DomainPrimitives.Testing`.

---

## 1. Domain Primitive Assertions

```csharp
using EricksonLopez.DomainPrimitives.Testing;
using Xunit;

public class EmailAddressTests
{
    [Fact]
    public void Create_ValidEmail_ShouldSucceed()
    {
        var result = EmailAddress.Create("valid@domain.com");

        result.ShouldBeSuccess()
              .WithValue("valid@domain.com");
    }

    [Fact]
    public void Create_InvalidEmail_ShouldFail()
    {
        var result = EmailAddress.Create("invalid-email");

        result.ShouldBeFailure()
              .WithError("Email format is invalid.");
    }
}
```
