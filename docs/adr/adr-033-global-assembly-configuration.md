# adr-033: Global Assembly Configuration via `[assembly: DomainPrimitivesDefaults]`

## Status
Accepted

## Context
In large enterprise codebases, developers often want uniform normalization and validation rules across all domain primitives in an assembly (such as always trimming string inputs, enforcing non-empty strings, or setting a default maximum length). Requiring explicit decoration on every single primitive struct creates boilerplate and risks accidental omissions.

## Decision
Introduce `[assembly: DomainPrimitivesDefaults]` attribute in `EricksonLopez.DomainPrimitives.Abstractions`:
- Allows configuring assembly-level defaults for `Trim`, `NotEmpty`, `MaxLength`, and `ExceptionType`.
- Incremental source generators inspect the compilation's assembly attributes and apply these defaults to any primitive that does not have an explicit per-type attribute override.
- Per-type explicit attributes take precedence over global defaults.

## Consequences
### Positive
- Drastically reduces boilerplate across microservices and domain libraries.
- Guarantees uniform quality and validation constraints across all domain primitives.
- Completely reflection-free and Native AOT compliant via compile-time source generation.

### Negative
- Developers must be aware of assembly-level attributes when reading a domain primitive declaration.
