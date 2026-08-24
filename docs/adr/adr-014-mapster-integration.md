# adr-009: Mapster Integration via Separate Package

## Status
Accepted

## Context
The v4.0 Specification indicates that domain primitives do not require a separate Mapster integration package because they provide explicit and implicit cast operators to map between primitive types and their raw backing values seamlessly.

## Decision
We maintain `EricksonLopez.DomainPrimitives.Mapster` as a separate, optional integration package to provide explicit `IRegister` mapping rules for consumers who rely on global Mapster TypeAdapterConfigs.

## Consequences
- **Positive**: Consumers can use `TypeAdapterConfig.GlobalSettings.Scan(typeof(DomainPrimitivesRegister).Assembly)` to automatically configure their mappers to avoid mapping errors involving primitives.
- **Positive**: The core generators remain dependency-free.
- **Negative**: Adds a small amount of overhead to maintain the Mapster integrations package, but it serves power-users effectively.
