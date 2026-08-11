# RFC 0001: Auto-Discovery for Infrastructure Integrations

## Summary
Deprecate domain-side integration attributes (`[EFCore]`, `[Json]`, `[Dapper]`, etc.) in favor of auto-discovery triggered directly from the infrastructure boundary.

## Motivation
Currently, consumers must pollute their Domain Layer with integration attributes like `[EFCoreAttribute]` to generate ValueConverters. This strictly violates Domain-Driven Design (DDD) principles where the Domain should have zero knowledge of persistence or serialization infrastructure. 

## Proposed Design
1. Obsolete all attributes in `EricksonLopez.DomainPrimitives.Abstractions.Attributes.IntegrationAttributes`.
2. Update the source generators in the respective integration projects (e.g., `EricksonLopez.DomainPrimitives.EFCore.SourceGenerators`) to automatically scan for *all* types implementing `IDomainPrimitive<T>`.
3. The integrations will generate converters for any valid primitive discovered in the assembly, without requiring an opt-in attribute on the primitive itself.

## Drawbacks
- Generates converters for *all* primitives, even those not used in the database. Given the minimal size of a ValueConverter class, the impact on assembly size is negligible.

## Unresolved Questions
- Should we provide an opt-out mechanism (e.g., `[IgnoreEFCore]`) for edge cases where auto-discovery is explicitly unwanted? (Decision: Defer until requested by users).
