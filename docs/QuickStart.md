# Quick Start

Welcome to `EricksonLopez.DomainPrimitives`! This framework lets you build **Domain Primitives** — strongly-typed types that encapsulate business rules — declaratively, with inherent validation and minimal allocations.

## 1. Installation

Add the core package to your domain project (typically a .NET 8+ class library). The core package includes abstractions, source generators, and analyzers.

```bash
dotnet add package EricksonLopez.DomainPrimitives
```

> **AOT Note:** The package is 100% reflection-free via Source Generators, making it natively compatible with **NativeAOT** and trimming.

## 2. Creating Your First Primitive

Imagine you have an application that handles user emails. Instead of representing it as a raw `string` (and validating its format everywhere), declare a **Domain Primitive**.

Declare a `readonly partial record struct` and apply the `[Email]` shortcut attribute:

```csharp
using EricksonLopez.DomainPrimitives;

namespace MyProject.Domain;

[Email] // Implies: [StringPrimitive], [Trim], [LowerCase], regex validation
public readonly partial record struct EmailAddress;
```

**That's it!** The Source Generator creates the rest — factory methods, validators, `IParsable<T>`, `ISpanFormattable`, `IUtf8SpanParsable<T>`, immutability, and security gates.

## 3. Basic Usage

The primitive exposes factory methods (`Create` and `TryCreate`) designed for safety and ergonomics.

### Happy Path

The instance is automatically normalized (e.g., `[Email]` applies `Trim()` and `ToLowerInvariant()` before validation):

```csharp
// Creates a valid instance. Internally applies trim, lowercase, and NFC normalization.
EmailAddress email = EmailAddress.Create("  USER@company.com   ");

Console.WriteLine(email.Value);
// Output: "user@company.com"
```

### Error Handling

If you pass an invalid value, `Create()` throws `DomainPrimitiveValidationException`. To avoid exceptions, use the `TryCreate` pattern with `out` parameters (zero-allocation on success):

```csharp
if (EmailAddress.TryCreate("not_an_email", out var email, out var error))
{
    Console.WriteLine($"Valid: {email.Value}");
}
else
{
    Console.WriteLine($"Error [{error.Code}]: {error.Message}");
    // Output: Error [FORMAT]: EmailAddress must match the required format.
}
```

### Span-Based Parsing

For high-performance scenarios, use `TryParse` with `ReadOnlySpan<char>` or `ReadOnlySpan<byte>`:

```csharp
// Parse from Span<char>
if (EmailAddress.TryParse("user@company.com".AsSpan(), null, out var parsed))
    Console.WriteLine($"Parsed: {parsed}");

// Parse from UTF-8 bytes (NET8+) — ideal for HTTP/gRPC/Kafka
ReadOnlySpan<byte> utf8 = "user@company.com"u8;
if (EmailAddress.TryParse(utf8, null, out var utf8Parsed))
    Console.WriteLine($"UTF-8 parsed: {utf8Parsed}");
```

## 4. Numeric Primitives

Need to represent a user's age? Use a numeric primitive:

```csharp
using EricksonLopez.DomainPrimitives;

namespace MyProject.Domain;

[Age] // Implies: [NumericPrimitive<int>], Min = 0, Max = 150
public readonly partial record struct UserAge;
```

The generator implements comparison and equality operators:

```csharp
var childAge = UserAge.Create(10);
var adultAge = UserAge.Create(21);

if (childAge < adultAge) // Operator overloads generated automatically
{
    Console.WriteLine("Is younger");
}
```

## 5. Integrations (EF Core, Dapper, ASP.NET Core)

The real power lies in effortless interoperability. Integrations are **auto-discovered** — no per-type attributes needed in your domain layer.

Simply install the integration package:

```bash
dotnet add package EricksonLopez.DomainPrimitives.EFCore
dotnet add package EricksonLopez.DomainPrimitives.AspNetCore
dotnet add package EricksonLopez.DomainPrimitives.Dapper
```

The source generators bundled in each package automatically detect your domain primitives and generate the appropriate converters (`ValueConverter` for EF Core, `TypeHandler` for Dapper, `ModelBinder` for ASP.NET Core).

> **No attributes required.** Your domain types remain clean — integration code is generated in the integration project, not in your domain.

## Summary

- Use **attributes** to declare design intent.
- **Source Generators** write the boilerplate.
- You get **minimal allocations**, full AOT support, and **type-safe domain boundaries**.

Explore the [Cookbook](cookbook.md) for more advanced use cases, including custom validators, Strong IDs, Value Objects, Smart Enums, and more!
