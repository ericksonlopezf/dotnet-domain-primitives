# adr-029: Discriminated Unions — Deferred to v2.x

**Date:** 2026-08-10 (Revised: 2026-08-19)
**Status:** Deferred
**Authors:** Core maintainers
**Related audit items:** GAP-001 (feature-gaps.md), AUDITORIA_PARIDAD_FUNCIONAL.md §19

---

## Context

`Thinktecture.Runtime.Extensions` is the only competitor that supports Discriminated Unions (DUs)
as first-class generated types. The feature enables modeling domain states as "one of":

```csharp
// Thinktecture pattern:
[DiscriminatedUnion]
public abstract partial class PaymentResult
{
    public sealed partial class Success(decimal amount);
    public sealed partial class Declined(string reason);
    public sealed partial class Pending(Guid transactionId);
}

// Usage (exhaustive):
result.Switch(
    success: s => ProcessSuccess(s.Amount),
    declined: d => HandleDecline(d.Reason),
    pending: p => AwaitTransaction(p.TransactionId));
```

The functional parity audit (§19) confirmed that while DU support is valuable, establishing an unachievable pre-condition (e.g. 2,000 GitHub stars) creates a circular dependency: without DUs, some teams don't adopt; without adoption, the threshold is never met.

---

## Decision

**Discriminated Union support remains deferred to v2.x**, but with calibrated, realistic triggers.

In the interim:
1. Document `[SmartEnum]` with exhaustive `Match<TResult>` / `Map<TResult>` / `Switch` as the recommended lightweight alternative for state-machine style single-value unions.
2. The triggers to unblock formal DU design are adjusted to reflect active community engagement rather than inflated vanity metrics.

---

## Interim Workaround Pattern: SmartEnum with Match

For state machines and tagged status outcomes where cases do not carry independent multi-property payloads, `[SmartEnum<int>]` or `[SmartEnum<string>]` provides full compile-time exhaustiveness:

```csharp
[SmartEnum<string>]
public readonly partial record struct OrderStatus
{
    public static readonly OrderStatus Placed = new("PLACED");
    public static readonly OrderStatus Paid = new("PAID");
    public static readonly OrderStatus Cancelled = new("CANCELLED");
}

// Exhaustive compile-time pattern matching with 0 allocations:
var message = status.Match(
    whenPlaced: () => "Order waiting for payment.",
    whenPaid: () => "Order paid and being fulfilled.",
    whenCancelled: () => "Order was cancelled.");
```

---

## Rationale

### 1. Implementation cost is high
DU support requires complex nested partial declarations, compile-time exhaustiveness generators, polymorphic JSON discriminator converters, and EF Core owned entity mapping (40–80h effort).

### 2. Design stability
C# language proposals for native Discriminated Unions / Type Unions continue to evolve in the .NET design team. Rushing a library-specific syntax risks obsolescence once Roslyn standardizes language-level unions.

---

## Pre-conditions for Resuming (Calibrated)

The following will trigger the drafting and implementation of the DU generator:

- [ ] NuGet downloads ≥ 1,000/month or GitHub stars ≥ 500.
- [ ] At least 3 documented user issues / architectural RFC submissions requesting complex payload DUs.
- [ ] RFC for Discriminated Union generator approved.

---

## Consequences

- **Positive:** Engineering efforts remain focused on core BCL depth, AOT perfection, and DX cookbook.
- **Positive:** Clear, non-blocking path forward for users via `[SmartEnum]` exhaustive matching.
- **Negative:** Full payload-carrying unions (e.g. `Success(Amount)` vs `Declined(Reason, ErrorCode)`) require custom records or Thinktecture until v2.x.

