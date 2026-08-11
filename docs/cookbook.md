# Cookbook: EricksonLopez.DomainPrimitives (Version 1.0.0)

This cookbook covers the most common needs derived from the public API inventory.

## 1. Create a Strongly Typed ID

**Problem:** Avoid "Primitive Obsession" and ensure that a `CustomerId` cannot be assigned to a `ProductId`.

**Solution:** Use `[StrongId]` along with the Source Generator.

**Code:**
```csharp
using EricksonLopez.DomainPrimitives;

[StrongId]
public readonly partial struct CustomerId;
```

**Explanation:**
The `[StrongId]` attribute instructs the Source Generator to automatically implement `IStrongId<CustomerId, Guid>` (by default the type is `Guid`), including validation for `Guid.Empty`, native JSON converters, and equality comparers.

## 2. Validate an Email Address

**Problem:** Validate the format of an email and normalize it to lowercase before being used.

**Solution:** Use `[Email]` on a primitive.

**Code:**
```csharp
using EricksonLopez.DomainPrimitives;

[Email]
public readonly partial struct EmailAddress;
```

**Explanation:**
`[Email]` is a shortcut attribute. Behind the scenes, it applies `[StringPrimitive]`, `[NotEmpty]`, `[Regex("...")]`, and `[LowerCase]`.
It is not possible to instantiate `EmailAddress` with an invalid email. If `EmailAddress.TryCreate(" TEST@example.com ", out var email)` is used, the normalized and trimmed result will be `"test@example.com"`.

## 3. Numeric Primitive with Range (Percentage)

**Problem:** Guarantee that a discount is always between 0 and 100 and be able to safely add discounts.

**Solution:** Use `[NumericPrimitive]`, define arithmetic policies and range validation.

**Code:**
```csharp
using EricksonLopez.DomainPrimitives;

[NumericPrimitive<decimal>(Policy = ArithmeticPolicy.Addition)]
[Range(0, 100)]
public readonly partial struct DiscountPercentage;
```

**Best Practices:**
Restrict `ArithmeticPolicy` only to what makes sense in the domain (a discount is not usually directly multiplied by another discount).

## 4. Using the Builder for silent validation

**Problem:** Instantiating a primitive will throw `DomainPrimitiveException` if it fails, which is inefficient in scenarios like model binding where multiple errors must be collected.

**Solution:** Use the `PrimitiveBuilder<TPrimitive, TValue>`.

**Code:**
```csharp
using EricksonLopez.DomainPrimitives;
using EricksonLopez.Result;

Result<EmailAddress> emailResult = PrimitiveBuilder<EmailAddress, string>
    .For()
    .Must(val => !val.EndsWith("@baddomain.com"), "Blacklist", "Domain not allowed")
    .TryCreate(inputString);

if (emailResult.IsSuccess)
{
    EmailAddress email = emailResult.Value;
}
```

## 5. Direct integration in EF Core

**Problem:** A `CustomerId` is required to be persisted in Entity Framework Core transparently, mapping directly to a scalar type in the DB (such as `uniqueidentifier`).

**Solution:** Decorate with `[EFCore]`.

**Code:**
```csharp
using EricksonLopez.DomainPrimitives;

[StrongId]
[EFCore]
public readonly partial struct CustomerId;
```

**Explanation:**
The Source Generator emits a `ValueConverter<CustomerId, Guid>` that EF Core discovers automatically. In the model, you only need to declare the property as `public CustomerId Id { get; set; }`.
