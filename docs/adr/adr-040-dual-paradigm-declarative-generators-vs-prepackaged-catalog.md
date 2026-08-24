# adr-040: Dual Paradigm Strategy: Declarative Source Generators vs. Enterprise Pre-Packaged Domain Catalog

## Status
Accepted — August 2026

## Context
Eliminating Primitive Obsession and modeling Value Objects across the `EricksonLopez.*` ecosystem follows two distinct architectural patterns:
- **Declarative Generation**: Generating custom domain primitives on demand via Roslyn attributes (`[StrongId]`, `[SmartEnum]`).
- **Enterprise Catalog**: Consuming a rich, pre-built library of mature Value Objects with complex business operations and multi-country fiscal support.

The ecosystem contains both `EricksonLopez.DomainPrimitives` and `EricksonLopez.ValueObjects`.

## Decision
Retain both packages as deliberate, high-value alternative paradigms:

1. **`EricksonLopez.DomainPrimitives` (Generator Paradigm)**:
   - Best for custom, greenfield domain models where teams define bespoke identifiers, codes, and smart enums via `[StrongId]` and `[SmartEnum]`.
   - Produces lightweight, zero-allocation structs emitted at compile-time.

2. **`EricksonLopez.ValueObjects` (Catalog Paradigm)**:
   - Best for enterprise applications requiring immediate, production-grade implementations of complex composite types (`Money`, `Address`, `CurrencyCode`, `ExchangeRate`, `DateRange`) and international fiscal modules (`Fiscal.DominicanRepublic`, `Fiscal.Mexico`, `Fiscal.Colombia`, `Fiscal.Peru`, `Fiscal.Chile`, `Fiscal.Argentina`).
   - Completely independent package with dedicated test suites, benchmarks, and persistence type handlers.

## Consequences
- Prevents unnecessary code duplication: Developers do not rewrite complex currency or fiscal arithmetic.
- Clean positioning: Each package serves distinct project requirements without boundary erosion.
