# ADR 032: Exclude Source Generators from Mutation Testing

## Status
Accepted

## Context
As part of our commitment to high code quality, we employ Stryker.NET for mutation testing to evaluate the effectiveness of our test suites. Our quality gates require mutation scores between 95% and 100%.

However, when attempting to run mutation testing against the `EricksonLopez.DomainPrimitives.Generators` project (the Roslyn Source Generators themselves), the process consistently failed.

The issues identified were:
1. **Massive Number of Mutants:** The generator project logic is highly complex, yielding over 3,600 mutants in a single run.
2. **Snapshot Testing (Verify) I/O Contention:** The generator tests are heavily reliant on `Verify` for snapshot testing to check the output of syntax trees. Running these tests concurrently caused file I/O contention (e.g., `System.ObjectDisposedException: Cannot access a closed Stream` within the VsTest framework) when reading/writing baseline `.verified.` and `.received.` files.
3. **Extreme Computational Overhead:** Even when restricting the Stryker concurrency to 1 (`--concurrency 1`) to avoid I/O collisions, each of the 3,600+ mutant evaluations required instantiating the Roslyn compiler in memory, generating the source code, and running the `Verify` assertions. This massive computational requirement caused the Stryker process to eventually crash or exceed typical CI/CD and developer workstation memory/time constraints.

## Decision
We will **exclude the Source Generator logic (`EricksonLopez.DomainPrimitives.Generators`) from mutation testing**, relying solely on standard test coverage metrics (`xUnit` + `Verify`) for this specific project. 

*Note: This decision applies to mutating the **generator's logic**, not the generated code. Code that is dynamically generated into consumer projects by the generators is already ignored by Stryker natively, which aligns with standard .NET practices.*

## Consequences
- **Positive:** CI/CD pipeline and local developer workflows remain fast and stable without OOM exceptions or infinite retries caused by test runner crashes.
- **Negative:** We lose the strict mutation score guarantee for the generator's internal logic.
- **Mitigation:** We maintain a strict Line Coverage baseline (currently ~87.6%) via rigorous snapshot testing, ensuring all execution paths of the generator are exercised and baselined, even if they aren't mutation-tested.
