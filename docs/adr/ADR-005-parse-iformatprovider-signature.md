# ADR 005: Parse(string) with IFormatProvider? = null vs Separate Overloads

**Date:** 2026-08-09  
**Status:** Approved

## Context

The `IParsable<T>` interface in .NET 7+ requires the implementation of the static abstract method:
```csharp
static abstract T Parse(string s, IFormatProvider? provider);
```

To implement this in the Domain Primitives Source Generators, a public method with an optional parameter was generated:
```csharp
public static T Parse(string s, IFormatProvider? provider = null)
```

In the BCL-caliber design review (Audit v3.0, finding F-03), it was pointed out that the canonical framework (e.g., `int.Parse("123")`) separates these methods into distinct overloads instead of using default parameters, to maximize long-term binary compatibility and avoid ambiguities in overload resolution when calling the interface.

## Decision

The committee has decided to **keep the current signature using `provider = null`** for version 1.x. 

## Rationale

1. **DX (Developer Experience) Benefit:** The use of optional parameters allows IntelliSense to show a single clear signature for `Parse`, which reduces cognitive load for the developer (prioritizing the P2 pillar - Simplicity).
2. **Compatibility:** Newer types in the BCL (post-.NET 7) have started to adopt optional parameters in similar scenarios.
3. **Not an error:** It technically fulfills the `IParsable<T>` contract.

## Consequences

- A small deviation from the strict canonical BCL pattern is accepted.
- It avoids generating 2 additional methods per type in the public API (keeping the API Surface Budget under 25 members).
- The topic will be reviewed again in v2.0 if overload resolution issues are reported by the community.
