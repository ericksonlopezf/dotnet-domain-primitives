# Public API (v1.1.1)

> **Version:** 1.1.1  
> **Last Updated:** 2026-08-09  

This document tracks the publicly exposed API surface of `EricksonLopez.DomainPrimitives`. This is required to ensure API compatibility tracking (TD-020).

## Core Interfaces

```csharp
namespace EricksonLopez.DomainPrimitives;

public interface IDomainPrimitive<TSelf>
    where TSelf : IDomainPrimitive<TSelf>;

public interface IDomainPrimitive<TSelf, TValue> : IDomainPrimitive<TSelf>
    where TSelf : IDomainPrimitive<TSelf, TValue>
    where TValue : notnull
{
    TValue Value { get; }
#if NET7_0_OR_GREATER
    static abstract TSelf Create(TValue value);
    static abstract bool TryCreate(TValue value, out TSelf result, out ValidationError validationError);
#endif
}

public interface IStrongId<TSelf, TValue> : IDomainPrimitive<TSelf, TValue>
    where TSelf : IStrongId<TSelf, TValue>
    where TValue : notnull
{
#if NET7_0_OR_GREATER
    static abstract TSelf Empty { get; }
#endif
}
```

## Validation Surface

```csharp
namespace EricksonLopez.DomainPrimitives.Validation;

public readonly record struct ValidationError(string Code, string Message);

public sealed class ValidationErrors
{
    public bool HasErrors { get; }
    public IReadOnlyList<ValidationError> Errors { get; }
    public void AddError(string code, string message);
    public static ValidationErrors Success();
}

public interface ICustomValidator<T>
{
    static abstract ValidationErrors Validate(T value);
}
```

## Attributes

```csharp
namespace EricksonLopez.DomainPrimitives;

[AttributeUsage(AttributeTargets.Struct, Inherited = false)]
public sealed class StringPrimitiveAttribute : Attribute
{
    public int MinLength { get; set; }
    public int MaxLength { get; set; }
    public string Pattern { get; set; }
}

// (Full list of other attributes like NumericPrimitive, DatePrimitive, Email, Phone, etc.)
```
