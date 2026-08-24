# ADR 032: Exclude Source Generators from Mutation Testing

## Status
Accepted

## Context
As part of our commitment to high code quality, we employ Stryker.NET for mutation testing to evaluate the effectiveness of our test suites. Our quality gates require mutation scores between 95% and 100%.

However, when attempting to run mutation testing against Roslyn Source Generator projects (`EricksonLopez.DomainPrimitives.Generators` and satellite integration generators), the process consistently failed.

The issues identified were:
1. **Massive Number of Mutants:** The incremental generator project logic is highly complex, yielding over 3,600 mutants in a single run for core generators alone.
2. **Snapshot Testing (Verify) I/O Contention:** The generator tests are heavily reliant on `Verify.Xunit` for snapshot baseline testing to check the exact syntax trees generated. Running these tests concurrently caused file I/O contention (e.g., `System.ObjectDisposedException: Cannot access a closed Stream` within the VsTest runner) when reading/writing baseline `.verified.cs` and `.received.cs` files.
3. **Extreme Computational Overhead:** Even when restricting the Stryker concurrency to 1 (`--concurrency 1`) to avoid I/O collisions, each mutant evaluation required instantiating the Roslyn compiler in memory, executing the incremental pipeline, generating source code, and running `Verify` assertions. This massive computational requirement caused the Stryker process to eventually crash or exceed CI/CD timeout and memory constraints.

## Decision
We will **exclude all Roslyn Source Generator projects from mutation testing**, relying solely on standard test coverage metrics (`xUnit` + `Verify.Xunit` snapshot testing) for:
- `EricksonLopez.DomainPrimitives.Generators` (Core domain primitive generators)
- `EricksonLopez.DomainPrimitives.AspNetCore.SourceGenerators` (ModelBinderProvider generator)
- `EricksonLopez.DomainPrimitives.EFCore.SourceGenerators` (ValueConverter generator)
- `EricksonLopez.DomainPrimitives.Dapper.SourceGenerators` (TypeHandler generator)
- `EricksonLopez.DomainPrimitives.OpenApi.SourceGenerators` (SchemaFilter generator)
- `EricksonLopez.DomainPrimitives.Mapster.SourceGenerators` (Register generator)

*Note: This decision applies to mutating the **generator's internal Roslyn transformation logic**, not the generated code. Code dynamically generated into consumer assemblies is tested via the runtime integration and unit test projects included in `stryker-config.json`.*

## Consequences
- **Positive:** CI/CD pipelines and local developer workflows remain fast, deterministic, and stable without OOM exceptions or runner crashes.
- **Negative:** We lose the strict mutation score guarantee for the generator's internal AST generation logic.
- **Mitigation:** We maintain a strict Line Coverage baseline via rigorous snapshot testing with `Verify.Xunit`, error scenario testing, and Roslyn driver verification across all generator projects, ensuring all incremental pipeline branches are exercised and verified.

