# adr-035: Exhaustive Switch, Map, and Match Pattern for `[SmartEnum<T>]`

## Status
Accepted

## Context
When working with Smart Enums in domain modeling, developers need exhaustive pattern matching capabilities to replace `switch` statements, preventing runtime errors when new enum members are added to the codebase.

## Decision
Generate strongly-typed, compile-time exhaustive matching methods on all `[SmartEnum<T>]` structs:
1. `Match<TResult>(Func<TResult> whenMember1, ...)`: Zero-argument lambda variant for mapping.
2. `Map<TResult>(Func<TSelf, TResult> whenMember1, ...)`: Lambda variant receiving the enum member instance.
3. `Switch(Action whenMember1, ...)`: Action variant for side effects.

All methods are generated at compile time with exact parameter lists matching the defined enum members, guaranteeing compile-time errors if a caller fails to handle a member.

## Consequences
### Positive
- Compile-time safety: Adding a new enum member breaks compilation until all `Switch`/`Map`/`Match` invocations are updated.
- Zero reflection and 100% Native AOT compatibility.
- Zero allocations.
