# adr-045: Roslyn Analyzer DP0018 for ValueObject Collection Immutability

## Status
Accepted

## Date
2026-09-04

## Context
In Domain-Driven Design (DDD), Value Objects must guarantee absolute immutability and value equality. While `[ValueObject]` types are generated as `readonly partial record struct` instances whose scalar properties require `init` accessors (enforced by DP0008), composite Value Objects that contain collections or arrays risk compromising their immutability if mutable types (such as `T[]`, `List<T>`, or `Dictionary<TKey, TValue>`) are used as properties.

Even if an array or list property is declared with `{ get; init; }`, the underlying elements can be modified in-place by external callers after construction (e.g., `valueObject.Items[0] = newItem;`), corrupting domain invariants without re-triggering validation.

## Decision
1. Introduce Roslyn Analyzer **DP0018** (`DP0018_ValueObjectMutableCollection`):
   - Category: Design
   - Severity: Warning
   - Enabled by Default: True
2. The analyzer inspects all properties of types decorated with `[ValueObject]`:
   - If a property type is an array (`T[]`) or a mutable collection type (`List<T>`, `HashSet<T>`, `Dictionary<K, V>`, `Collection<T>`), diagnostic DP0018 is reported.
3. Recommend and guide developers toward using deeply immutable collections such as `System.Collections.Immutable.ImmutableArray<T>`, `IReadOnlyList<T>`, or domain collection wrappers.

## Consequences
### Positive
- Enforces true deep immutability on complex composite value objects.
- Protects domain invariants from post-construction tampering or thread-safety hazards.
- Provides immediate, live IDE feedback and guidance during domain modeling.

### Negative
- Codebases modeling multi-item Value Objects must reference `System.Collections.Immutable` and adopt `ImmutableArray<T>`.

## See Also
- [DP0018 Rule Reference](../rules/dp0018.md)
- [DP0008 Rule Reference](../rules/dp0008.md)
- [ADR-001: Record Structs for Domain Primitives](adr-001-record-structs-for-domain-primitives.md)
