# Migrating from Vogen to DomainPrimitives

> **Version:** 2.0 | **Date:** 2026-08-10  
> **Difficulty:** Low–Medium  
> **Time estimate:** 1–4 hours per project depending on size

---

## Should You Migrate?

### Migrate if you need:
- `IUtf8SpanParsable<T>` / `ISpanFormattable` / `IUtf8SpanFormattable` (not in Vogen)
- Declarative normalization (`[Trim]`, `[LowerCase]`, `[UpperCase]`) generated for you
- ReDoS-resistant regex validation (NonBacktracking + 100ms timeout)
- Unicode NFC normalization as a security default
- 30 semantic domain type shortcuts (`[Email]`, `[Money]`, `[CountryCode]`...)
- Auto-discovered EF Core / Dapper registrations without per-type annotations

### Keep Vogen if you need:
- Newtonsoft.Json converters (not yet in DomainPrimitives)
- Class-based value objects (DomainPrimitives is struct-only)
- Custom underlying types beyond `string/int/long/Guid` (Vogen supports `[ValueObject<T>]` with any T)
- A large community and extensive examples

---

## Attribute Equivalents

| Vogen | DomainPrimitives | Notes |
|:---|:---|:---|
| `[ValueObject]` | `[StringPrimitive]` or `[NumericPrimitive<T>]` | Pick the appropriate primitive type |
| `[ValueObject<int>]` | `[NumericPrimitive<int>]` | |
| `[ValueObject<string>]` | `[StringPrimitive]` | Adds normalization + validation pipeline |
| `[ValueObject<Guid>]` | `[StrongId<Guid>]` | Use `[StrongId]` for IDs |
| `[ValueObject<int>]` (for IDs) | `[StrongId<int>]` | |
| `VogenDefaults` | N/A | Global config not yet available (on roadmap) |
| `[Instance("Name", value)]` | Field declaration | `public static readonly MyEnum Member = new(...);` |
| `[Validate]` static method | Partial method | `private static PrimitiveError? CustomValidate(string value)` |

---

## Code Migration Examples

### Simple String Value Object

**Vogen:**
```csharp
[ValueObject<string>]
public partial struct Name;
```

**DomainPrimitives:**
```csharp
[StringPrimitive]
[NotEmpty]
[MaxLength(100)]
public readonly partial record struct Name;
```

---

### Email Value Object

**Vogen:**
```csharp
[ValueObject<string>]
public partial struct Email
{
    private static Validation Validate(string value)
    {
        if (!Regex.IsMatch(value, @"^[^@]+@[^@]+$"))
            return Validation.Invalid("Invalid email format");
        return Validation.Ok;
    }
}
```

**DomainPrimitives:**
```csharp
[Email] // Includes Trim + LowerCase + MaxLength(320) + RFC5322 regex
public readonly partial record struct Email;
```

> **Note:** DomainPrimitives validates with a NonBacktracking RFC 5322 regex automatically. The regex is ReDoS-safe.

---

### Numeric Value Object

**Vogen:**
```csharp
[ValueObject<decimal>]
public partial struct Price
{
    private static Validation Validate(decimal value)
    {
        if (value < 0) return Validation.Invalid("Price cannot be negative");
        return Validation.Ok;
    }
}
```

**DomainPrimitives:**
```csharp
[Money] // Or: [NumericPrimitive<decimal>] [PrimitiveRange(0, double.MaxValue)]
public readonly partial record struct Price;
```

---

### Strongly Typed ID

**Vogen:**
```csharp
[ValueObject<Guid>]
public partial struct OrderId;
```

**DomainPrimitives:**
```csharp
[StrongId<Guid>]
public readonly partial record struct OrderId;
```

---

### Validation Pattern

**Vogen:**
```csharp
var result = Email.TryFrom("test@example.com");
if (result.IsSuccess)
    var email = result.Value;
else
    var error = result.Error;
```

**DomainPrimitives:**
```csharp
if (Email.TryCreate("test@example.com", out var email, out var error))
    // use email — zero heap allocation on success
else
    Console.WriteLine($"Error [{error.Code}]: {error.Message}");
```

> **Key difference:** Vogen returns `ValueObjectOrError<T>` (a heap-allocated wrapper). DomainPrimitives uses `out` parameters — zero allocation on the success path.

---

### Smart Enum (Vogen has no native SmartEnum support → use DomainPrimitives directly)

**Previous (Ardalis.SmartEnum or custom):**
```csharp
public sealed class OrderStatus : SmartEnum<OrderStatus>
{
    public static readonly OrderStatus Pending = new(nameof(Pending), 1);
    public static readonly OrderStatus Shipped = new(nameof(Shipped), 2);
    private OrderStatus(string name, int value) : base(name, value) { }
}
```

**DomainPrimitives:**
```csharp
[SmartEnum<int>]
public readonly partial record struct OrderStatus
{
    public static readonly OrderStatus Pending = new(1, nameof(Pending));
    public static readonly OrderStatus Shipped = new(2, nameof(Shipped));
}
```

Generated: `All`, `FromValue()`, `TryFromValue()`, `FromName()`, `TryFromName()`, `TryFromName(ignoreCase)`, `Match<TResult>()`, `Map<TResult>()`, `Switch()`, STJ converter, TypeConverter. AOT-safe.

---

## EF Core Migration

### Vogen EF Core (per-type annotation required):
```csharp
[ValueObject<Guid>(conversions: Conversions.EfCore)]
public partial struct OrderId;

// In DbContext:
modelBuilder.Entity<Order>()
    .Property(o => o.Id)
    .HasConversion<OrderIdEfCoreValueConverter>();
```

### DomainPrimitives EF Core (auto-discovery, no annotation):
```csharp
[StrongId<Guid>]
public readonly partial record struct OrderId;

// In DbContext (one-time setup — discovers ALL domain types):
protected override void ConfigureConventions(ModelConfigurationBuilder configBuilder)
    => configBuilder.AddDomainPrimitivesConventions();
```

---

## Dapper Migration

### Vogen Dapper (per-type annotation):
```csharp
[ValueObject<Guid>(conversions: Conversions.Dapper)]
public partial struct OrderId;

// Registration:
SqlMapper.AddTypeHandler(new OrderIdTypeHandler());
```

### DomainPrimitives Dapper (auto-registration):
```csharp
// At app startup — discovers and registers ALL domain type handlers:
DomainPrimitivesDapperSetup.RegisterAll(Assembly.GetExecutingAssembly());
```

---

## Search-and-Replace Cheatsheet

Run these in order:

```
1.  [ValueObject<string>]           →  [StringPrimitive]
2.  [ValueObject<Guid>]             →  [StrongId<Guid>]
3.  [ValueObject<int>]              →  [StrongId<int>] or [NumericPrimitive<int>]
4.  [ValueObject<decimal>]          →  [NumericPrimitive<decimal>]
5.  partial struct                  →  readonly partial record struct
6.  .TryFrom(                       →  .TryCreate(  (adjust return handling)
7.  result.IsSuccess                →  TryCreate(..., out var result, out var error)
8.  result.Value                    →  result  (result IS the value object)
9.  Conversions.EfCore              →  (remove — auto-discovered)
10. Conversions.Dapper              →  (remove — auto-registered)
```

---

## Known Differences After Migration

| Behavior | Vogen | DomainPrimitives |
|:---|:---|:---|
| Error model | `ValueObjectOrError<T>` (heap) | `out PrimitiveError` (struct, stack) |
| Newtonsoft.Json | ✅ | ❌ (not yet supported) |
| Class-based VOs | ✅ | ❌ (struct only) |
| String normalization | Manual in Validate() | Declarative [Trim], [LowerCase], etc. |
| Regex | Manual | Declarative [Regex("pattern")] |
| NFC normalization | Never | Always (SEC-004) |
| Default MaxLength | None | 4096 (SEC-001) |
| JSON hot path | GetString() | ValueSpan (zero extra alloc) |
