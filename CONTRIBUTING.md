# Contributing to EricksonLopez.DomainPrimitives

First off, thank you for considering contributing to `EricksonLopez.DomainPrimitives`. We welcome your input, whether it is a bug report, a feature request, or a pull request.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (pinned to `10.0.302` via [`global.json`](global.json); `rollForward: latestFeature` allows newer patches).
- Java 17 (required only if you want to run SonarScanner locally).

## Build Process

To build the solution locally:

```bash
# Restore dependencies
dotnet restore EricksonLopez.DomainPrimitives.slnx

# Build the entire solution in Release configuration
dotnet build EricksonLopez.DomainPrimitives.slnx --configuration Release --no-restore
```

## Testing and Quality

We maintain a high standard of quality. Please ensure your code is well-tested.

### Unit & Integration Tests with Code Coverage

```bash
dotnet test EricksonLopez.DomainPrimitives.slnx --configuration Release --no-build --verbosity normal --collect:"XPlat Code Coverage"
```

### Security Gate Tests

```bash
dotnet test EricksonLopez.DomainPrimitives.slnx --configuration Release --no-build --filter "Category=Security" --verbosity normal
```

### API Surface Budget Gate Tests

```bash
dotnet test tests/EricksonLopez.DomainPrimitives.UnitTests/EricksonLopez.DomainPrimitives.UnitTests.csproj --configuration Release --no-build --filter "Category=ApiSurfaceBudget" --verbosity normal
```

### Mutation Testing (Stryker)

Mutation testing verifies that your tests are meaningful. The project uses [Stryker.NET](https://stryker-mutator.io/docs/stryker-net/introduction/) with a break threshold of **95%** and high threshold of **100%**.

```bash
# Install Stryker (if not already installed)
dotnet tool install -g dotnet-stryker

# Run from the repository root using stryker-config.json
dotnet-stryker
```

Configuration is in [`stryker-config.json`](stryker-config.json) at the repository root. The mutation solution `Stryker.slnx` targets `net8.0` and covers 8 test projects: `Abstractions.UnitTests`, `UnitTests`, `Testing.UnitTests`, `Dapper.IntegrationTests`, `EFCore.UnitTests`, `AspNetCore.UnitTests`, `OpenApi.Tests`, and `NewtonsoftJson.Tests`.

### Benchmarks

Performance is critical. If your change touches the generator core or hot paths (`TryCreate`, `Parse`, `TryFormat`), run benchmarks:

```bash
dotnet run --project benchmarks/EricksonLopez.DomainPrimitives.Benchmarks/EricksonLopez.DomainPrimitives.Benchmarks.csproj \
  --configuration Release --framework net10.0 -- --filter "*" --job short --runtimes net8.0 net9.0 net10.0
```

Results are written to `benchmarks/results/`. See [docs/benchmark-plan.md](docs/benchmark-plan.md) for the expected scenarios and [docs/benchmark-results.md](docs/benchmark-results.md) for the latest run results.

## Branch Naming Convention

We recommend using the following convention for your branches:
- `feature/your-feature-name`
- `bugfix/issue-description`
- `docs/what-you-updated`

## Commit Convention

We follow the [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/) specification.
Examples:
- `feat: add support for XYZ validation`
- `fix: resolve JSON serialization issue`
- `docs: update public API documentation`

## Pull Request Process

1. Fork the repository and create your branch from `main` or `develop`.
2. Ensure you have added tests that cover your changes.
3. Ensure all tests pass.
4. Update documentation if you are changing public APIs or architecture (`README.md`, `/docs/*.md`, `docs/architecture.md`, `docs/boundary.md`, XML comments).
5. Update `CHANGELOG.md` under `[Unreleased]`.
6. Create the PR and ensure the CI checks pass:
   - Build (Release, `TreatWarningsAsErrors`)
   - Test (all unit + integration tests with code coverage)
   - API Compatibility Check (package validation against baseline)
   - SonarQube static analysis
   - Codecov coverage upload
   - Stryker mutation testing (break threshold: 95%)
   - Native AOT compatibility verification
   - Benchmark validation
7. Fill out the [PR template](.github/PULL_REQUEST_TEMPLATE.md) completely.

## Governance and RFCs

Major architectural changes and API additions must go through the Request for Comments (RFC) process. 
Please read our [Governance Policy](GOVERNANCE.md) to understand how decisions are made, who approves them, and how to submit a proposal in the [RFCs directory](docs/rfcs/).

## Code of Conduct

Please note that this project is released with a [Contributor Code of Conduct](CODE_OF_CONDUCT.md). By participating in this project you agree to abide by its terms.
