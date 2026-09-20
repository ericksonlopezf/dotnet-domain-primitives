# Level 8 — Customization

This level demonstrates extending the library with custom algorithmic validators, sanitizing normalizers, and dynamic programmatic construction using `PrimitiveBuilder`.

---

## 1. Custom Invariant Validators (`ICustomValidator<T>`)

When a business rule requires custom algorithmic verification (e.g. Luhn checksum algorithm, fiscal check-digit computation), implement `ICustomValidator<T>`:

```csharp
using EricksonLopez.DomainPrimitives.Validation;

public sealed class LuhnAlgorithmValidator : ICustomValidator<string>
{
    public PrimitiveError Validate(string value)
    {
        int sum = 0;
        bool alternate = false;
        for (int i = value.Length - 1; i >= 0; i--)
        {
            if (!char.IsDigit(value[i]))
                return PrimitiveError.Create("INVALID_DIGIT", "Input contains non-digit characters.");

            int n = value[i] - '0';
            if (alternate)
            {
                n *= 2;
                if (n > 9) n -= 9;
            }
            sum += n;
            alternate = !alternate;
        }

        return (sum % 10 == 0)
            ? PrimitiveError.None
            : PrimitiveError.Create("LUHN_CHECKSUM", "Value failed Luhn checksum verification.");
    }
}

// Declarative association on the primitive:
[StringPrimitive]
[CustomValidator<LuhnAlgorithmValidator>]
public readonly partial record struct CreditCardNumber;
```

---

## 2. Custom Normalizers (`INormalizer<T>`)

To transform raw input before invariant validation occurs (e.g. stripping hyphens or invisible formatting characters):

```csharp
using EricksonLopez.DomainPrimitives;

public sealed class StripHyphensNormalizer : INormalizer<string>
{
    public string Normalize(string value) => value.Replace("-", "").Trim();
}

[StringPrimitive]
[Normalize<StripHyphensNormalizer>]
[ExactLength(10)]
public readonly partial record struct NationalTaxId;
```

---

## 3. Dynamic Programmatic Construction (`PrimitiveBuilder`)

For scenarios where rules must be composed dynamically at runtime (e.g. test factories or dynamic data import pipelines):

```csharp
using EricksonLopez.DomainPrimitives.Advanced;

var builder = PrimitiveBuilder<CustomerEmail, string>.For()
    .WithValue("user@domain.com")
    .Must(val => val.EndsWith("@trusted.org"), "UNTRUSTED_DOMAIN", "Domain must be trusted.org")
    .Must(val => !val.Contains("admin"), "FORBIDDEN_KEYWORD", "Email cannot contain admin");

if (builder.Build(out var email))
{
    Console.WriteLine($"Dynamically validated email: {email.Value}");
}
```

---

## Showcase Reference
- Executable project: [`22-CustomImplementations`](file:///d:/DevData/ericksonlopez.dev/dotnet-domain-primitives/samples/OfficialSample/22-CustomImplementations/Program.cs)
