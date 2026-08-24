# Level 06 — Entity Framework Core & Dapper Persistence

In Level 06, we map domain primitives to relational databases using EF Core ValueConverters and Dapper TypeHandlers.

---

## 1. EF Core Value Conversion

```csharp
using Microsoft.EntityFrameworkCore;
using EricksonLopez.DomainPrimitives.EFCore;

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Customer>()
        .Property(c => c.Email)
        .HasDomainPrimitiveConversion();
}
```

---

## 2. Dapper Type Handlers

`EricksonLopez.DomainPrimitives.Dapper` automatically registers type handlers at application startup without reflection.
