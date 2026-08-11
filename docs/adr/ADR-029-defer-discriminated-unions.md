# ADR-029: Discriminated Unions — Deferred to v2.x

**Date:** 2026-08-10
**Status:** Deferred
**Authors:** Core maintainers
**Related audit items:** GAP-001 (feature-gaps.md)

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

The gap analysis (GAP-001) rated this as P1 — Important but not P0 Critical. The product
strategy analysis (2026-08-10) further determined:

> "The effort is high, the adoption base to validate the design is inexistente, and the
> gaps of credibility (benchmarks, Newtonsoft.Json, migration guides) must close first."

---

## Decision

**Discriminated Union support is deferred to v2.x.**

The feature will not be implemented until:

1. The library has meaningful NuGet adoption (minimum threshold: 1,000 downloads/month).
2. GAP-002 (Newtonsoft.Json) and GAP-009 (public benchmarks) are closed.
3. An RFC is filed and accepted specifying the exact generator design.
4. At least 3 real-world use cases from actual library users are documented.

---

## Rationale

### 1. Implementation cost is high

DU support requires:
- A new `[DiscriminatedUnion]` attribute.
- A generator capable of reading nested partial class declarations (the "cases").
- `Switch<TResult>` / `Map<TResult>` methods with one parameter per case — compile-time
  exhaustiveness.
- STJ JSON converter with discriminator tag.
- EF Core owned entity pattern for persistence.

Estimated effort: 40–80 hours. This is 5–10x the cost of any single integration added so far.

### 2. Design risk without users

The DU API surface is non-trivial. Getting it wrong — wrong case naming, wrong exhaustiveness
pattern, wrong JSON discriminator convention — and then discovering the mistake after users
have adopted it means a breaking change in a v2.0 major version.

Without users to validate the design before release, the risk of shipping the wrong API is high.

### 3. Credibility gaps must close first

A developer evaluating `EricksonLopez.DomainPrimitives` who needs DUs today will choose
Thinktecture. That is the correct choice given the current state.

Adding DUs without closing the credibility gap (no benchmarks, no Newtonsoft.Json) would
not change the evaluation outcome for performance-conscious teams. The library would still
lose evaluations on different criteria.

### 4. The competitive landscape for DUs is not urgent

Thinktecture has DUs. Vogen does not and has shown no signs of adding them. StronglyTypedId
does not. The gap exists but is not widening — Thinktecture has had this feature for 2+ years
with no new entrant.

---

## Pre-conditions for Resuming

The following must be true before implementing DUs:

- [ ] NuGet downloads ≥ 1,000/month for 3 consecutive months.
- [ ] GAP-002 (Newtonsoft.Json package) shipped.
- [ ] GAP-009 (public benchmark comparison vs Vogen) published.
- [ ] RFC-0007 (Discriminated Union design) filed and approved.
- [ ] At least 3 real user requests with documented use cases in GitHub Issues.

---

## Consequences

- **Positive:** Resources focus on credibility gaps (benchmarks, Newtonsoft.Json) which have
  higher ROI for adoption than DU support.
- **Positive:** DU design can be validated against real user feedback before committing to an API.
- **Negative:** Teams that need DUs today must use Thinktecture. This is explicitly documented
  in the README as a known gap.
- **Documentation action:** Gap remains in `docs/feature-gaps.md` with P1 priority and this
  ADR as the formal decision record. README known gaps section references this ADR.
