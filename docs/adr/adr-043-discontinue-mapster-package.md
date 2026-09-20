# adr-043: Discontinuation and Removal of Dedicated Mapster Integration Packages

## Status
Accepted (Implemented in v2.0.0 as BC-001)

## Date
2026-08-24

## Context
In v1.x, the repository published two dedicated packages: `EricksonLopez.DomainPrimitives.Mapster` and `EricksonLopez.DomainPrimitives.Mapster.SourceGenerators` (documented in ADR-014 and ADR-017). These packages automatically emitted Mapster `IRegister` implementations for mapping domain primitives.

However, modern .NET object mappers (such as Mapster and Mapperly) natively resolve the source-generated `explicit operator` conversions emitted on all scalar domain primitives (`[StringPrimitive]`, `[StrongId]`, `[NumericPrimitive]`, `[DatePrimitive]`). Maintaining dedicated packages and Roslyn generator pipelines solely for mapping was adding disproportionate maintenance and CI overhead without delivering substantial value over native mapper capabilities.

## Decision
1. Remove `EricksonLopez.DomainPrimitives.Mapster` and `EricksonLopez.DomainPrimitives.Mapster.SourceGenerators` from the repository in v2.0.0 (breaking change BC-001).
2. Direct consumers to rely on native `explicit operator` conversion operators emitted on scalar primitives, which Mapster resolves without extra integration libraries.
3. For composite `[ValueObject]` types, recommend standard user-defined `TypeAdapterConfig` or constructor mappings when required.

## Consequences
### Positive
- Eliminates 2 packages from the build, packaging, signing, and publish pipelines.
- Reduces dependencies, CI wall-clock time, and mutation testing matrix footprint.
- Decouples domain primitive core from specific third-party object mapping library version lifecycles.

### Negative
- Downstream projects upgrading to v2.0.0 that referenced `EricksonLopez.DomainPrimitives.Mapster` must remove the package reference and rely on standard Mapster configuration.

## See Also
- [ADR-017: Mapster Integration Rationale](adr-017-mapster-integration-rationale.md)
- [ADR-030: Rejection of AutoMapper Integration](adr-030-reject-automapper-integration.md)
- [CHANGELOG.md: v2.0.0 Breaking Changes (BC-001)](../../CHANGELOG.md)
