# adr-022: Reject Aggregate and Entity Support

**Date:** 2026-08-10
**Status:** Accepted
**Authors:** Core maintainers
**Related audit items:** REJECT-008 (feature-gaps.md)

---

## Context

Users practicing Domain-Driven Design sometimes request that `EricksonLopez.DomainPrimitives`
extend its scope to support Aggregates and Entities, in addition to Value Objects.

An Entity is a domain object with a persistent identity that survives mutations. An Aggregate is
a consistency boundary grouping one or more Entities (and Value Objects) behind a root.

The library currently generates: `[StringPrimitive]`, `[NumericPrimitive<T>]`, `[DatePrimitive]`,
`[StrongId]`, `[ValueObject]`, `[SmartEnum]` — all Value Object primitives.

---

## Decision

**`EricksonLopez.DomainPrimitives` will never add Aggregate or Entity generation support.**

The scope of this library is permanently bounded to **Value Objects and Domain Primitives**.

---

## Rationale

### 1. Aggregates and Entities are a categorically different problem

Value Objects are defined by their value — two `EmailAddress("a@b.com")` instances are equal
because they have the same value. Value Objects are immutable.

Entities are defined by their identity — two `User(id: 42)` instances are the same entity even
if their properties differ. Entities are mutable over time.

This difference is not superficial. It requires:

- **Lifecycle management:** Entities have a lifecycle (Created → Active → Deleted). Source-
  generated structs cannot model this.
- **Domain events:** Aggregates publish domain events on state transitions. This requires
  an event dispatcher, registration pattern, and integration with the application bus.
- **Optimistic concurrency:** Aggregates typically carry a version/concurrency token.
- **Repository pattern:** Aggregates are loaded and saved through repositories with unit-of-work
  semantics.

None of these concerns are expressible through source generation of `readonly record struct` types.

### 2. Competing in this space means competing with frameworks, not libraries

Adding Aggregate support would put `EricksonLopez.DomainPrimitives` in direct competition with:

- MediatR / Wolverine (event dispatching)
- EventSourcing frameworks (Marten, EventStore)
- DDD base class libraries (Ardalis.SharedKernel, DDDToolkit)

These are fundamentally different products targeting different decisions. The library would lose
its focused identity.

### 3. Scope creep violates the Single Responsibility Principle at the library level

The purpose of `EricksonLopez.DomainPrimitives` is:
> "Generate strictly valid, immutable, allocation-minimized domain primitives."

Aggregates are not immutable, not primitive, and not allocation-minimized. Adding them would
violate the stated purpose.

### 4. Generator surface would grow to an unmanageable size

Current generator surface: ~100KB of generator C# code across 6 generators. Aggregate support
would require:
- Entity base class generation
- Domain event publication hooks
- Repository interface generation
- Snapshot/versioning logic
- Concurrency token management

This would at minimum triple the generator surface, increasing maintenance risk, cognitive
complexity, and test burden beyond what a small maintainer team can manage sustainably.

---

## Alternatives Considered

| Alternative | Rejected because |
|-------------|-----------------|
| Provide `[Entity]` attribute as an experimental feature | Experimental features become supported features. The maintenance burden starts on day one. |
| Provide Aggregate base classes (non-generated) | This is the scope of Ardalis.SharedKernel, not a domain primitive library. |
| Generate only the `IAggregateRoot` marker interface | Adds no value — any `readonly record struct` can implement an interface. The generator is unnecessary. |

---

## Consequences

- **Positive:** Library scope remains focused on Value Objects.
- **Positive:** Generator surface stays bounded.
- **Negative:** Users who want both Value Objects and Aggregate scaffolding must use two
  libraries. Recommended pairing: `EricksonLopez.DomainPrimitives` + `Ardalis.SharedKernel`
  or any DDD base library.
- **Documentation action:** Documented in `docs/rejected-features.md`.
