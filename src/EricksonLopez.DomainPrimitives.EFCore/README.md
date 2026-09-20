# EricksonLopez.DomainPrimitives.EntityFrameworkCore

Entity Framework Core compile-time integration for `EricksonLopez.DomainPrimitives`.

## Overview

This package operates as a **Roslyn Source Generator analyzer package**. It contains zero runtime DLL overhead and emits EF Core `ValueConverter` registrations and `ModelBuilder` extension methods directly at compile time.

## Key Features

- **Compile-time Value Converter Generation**: Automatically discovers all types implementing `IDomainPrimitive<TSelf, TValue>` and generates EF Core `ValueConverter<TPrimitive, TValue>` implementations.
- **Zero Runtime Reflection**: Does not perform runtime type scanning or dynamic assembly inspection, ensuring full compatibility with Native AOT and Trimming.
- **Fluent Configuration**: Generates `modelBuilder.ApplyDomainPrimitiveValueConverters()` to register all domain primitive converters in a single call.

## Usage

```csharp
public class AppDbContext : DbContext
{
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyDomainPrimitiveValueConverters();
    }
}
```
