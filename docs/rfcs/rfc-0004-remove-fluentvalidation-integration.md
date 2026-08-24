# rfc-0004: Removal of FluentValidation Integration Package

> **Status:** Approved (post-hoc — ratified 2026-08-10)
> **Authors:** Erickson Lopez
> **Created:** 2026-07-25
> **Approved:** 2026-08-10
> **Implemented In:** v1.2.0 (Unreleased)

---

## Problem Statement

The `EricksonLopez.DomainPrimitives.FluentValidation` integration package was added in v1.0 to
allow consumers using FluentValidation to validate domain primitives within their FluentValidation
rule chains. However, this violated several core design principles:

1. **Scope Creep:** Domain primitives are self-validating. The engineering spec states: *"No fluent
   validation, no aggregation patterns, no repository patterns."* FluentValidation is an
   application-layer concern, not a domain primitive concern.

2. **Dependency on application framework:** The integration took a hard dependency on
   `FluentValidation` (>= 11.0), pulling 3 transitive packages into consumers' dependency graphs
   even if they don't use FluentValidation.

3. **Maintenance burden:** FluentValidation evolves independently and breaks binary compatibility
   between major versions. This created an implicit coupling the library had to maintain indefinitely.

4. **Semantic confusion:** `AbstractValidator<OrderId>` implies OrderId needs external validation.
   It doesn't — `OrderId.TryCreate()` IS the validation. The integration was enabling an anti-pattern.

## Decision

Remove `EricksonLopez.DomainPrimitives.FluentValidation` integration package entirely.

## Migration Guide

```csharp
// Before: using the FluentValidation integration
public class OrderValidator : AbstractValidator<Order>
{
    public OrderValidator()
    {
        RuleFor(x => x.CustomerId)
            .SetValidator(new DomainPrimitiveValidator<CustomerId>());
    }
}

// After: validate at the boundary (controller / command handler), not in FluentValidation
public class CreateOrderCommandHandler
{
    public async Task<Result> Handle(CreateOrderCommand command)
    {
        if (!CustomerId.TryCreate(command.RawCustomerId, out var customerId, out var error))
            return Result.Failure(error.Message);

        // customerId is now guaranteed valid
        var order = Order.Create(customerId, ...);
    }
}
```

## Breaking Change Classification

| Type | Level |
|------|-------|
| Source compatibility | ❌ Breaking (package removal) |
| Binary compatibility | ❌ Breaking (package removal) |
| Behavioral | N/A |

## Mitigation

- The `[FluentValidation]` attribute stub in `Abstractions` was retained with `[Obsolete]` to allow
  consuming projects to compile during migration.
- Consuming code should be migrated to call `TryCreate()` at the application boundary.

## Votes

| Maintainer | Decision | Rationale |
|------------|----------|-----------|
| Erickson Lopez | +1 | Scope creep removal, spec compliance |

*Note: This RFC was ratified post-implementation as part of the audit process.*
