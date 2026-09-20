# adr-044: Dapper Source Generator Referenced Assembly Discovery and Third-Party Assembly Deny-List

## Status
Accepted

## Date
2026-08-31

## Context
In enterprise Clean Architecture solutions, domain primitives and strongly-typed IDs frequently reside in a domain assembly (e.g., `MyApp.Domain`), while database persistence configurations and Dapper integrations reside in an infrastructure or presentation assembly (e.g., `MyApp.Infrastructure` or `MyApp.Api`).

When `DapperTypeHandlerGenerator` was enhanced to discover primitives across referenced project assemblies, full compilation traversal walked all referenced assemblies, including third-party transitives. In particular, the generator processed `Npgsql.Internal.Size`, attempting to emit a `SizeTypeHandler` that triggered compiler diagnostic `NPG9001` (usage of experimental/internal API). The existing prefix exclusions only skipped `System`, `Microsoft`, `netstandard`, `mscorlib`, `Dapper`, and `EricksonLopez`.

## Decision
1. Unify syntax tree inspection and referenced assembly symbol traversal into a single cohesive candidate discovery pipeline in `DapperTypeHandlerGenerator`.
2. Expand the assembly deny-list to exclude known third-party library and framework prefixes:
   `Npgsql`, `Polly`, `FluentValidation`, `Serilog`, `OpenTelemetry`, `Azure`, `AWSSDK`, `StackExchange`, `Newtonsoft`, `AutoMapper`, `MediatR`, `Mapster`, `Bogus`, `xunit`, `NUnit`, `Moq`, `NSubstitute`.
3. Filter candidate types strictly to domain primitives implementing `IDomainPrimitive<TSelf, TValue>`, `IStrongId`, or recognized composite `[ValueObject]` conventions.

## Consequences
### Positive
- Prevents compilation errors caused by third-party internal or experimental types (such as `Npgsql.Internal`).
- Enables seamless cross-assembly registration of hundreds of domain type handlers in consumer solutions without manual annotations.
- Deterministic, zero-allocation handler generation for multi-project solution architectures.

### Negative
- Domain assemblies must not use names starting with any of the blacklisted prefixes (a standard best practice in .NET development).

## See Also
- [CHANGELOG.md: Unreleased entries](../../CHANGELOG.md)
- [ADR-002: Roslyn Source Generators for Domain Primitives](adr-002-use-source-generators-for-domain-primitives.md)
