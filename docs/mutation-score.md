# Mutation Score Report and Stryker.NET Quality Gates

This document defines the mutation testing architecture, matrix execution, and quality thresholds for `EricksonLopez.DomainPrimitives`. All mutation testing is powered by [Stryker.NET](https://stryker-mutator.io/docs/stryker-net/introduction/).

---

## 1. Quality Threshold Policy

Mutation testing enforces high test suite fidelity by systematically injecting mutants (faults) into source code to ensure that test suites detect and fail on regressions.

Thresholds are centrally configured in `stryker-*.json` profiles and strictly enforced by the CI/CD pipeline:

| Level | Threshold | Status | Description | Pipeline Action |
|-------|:---------:|:------:|-------------|-----------------|
| **High** | **≥ 100.0%** | `✅ HIGH` | Ideal coverage; all generated mutants killed | Release Approved |
| **Low** | **≥ 98.0%** | `🟡 LOW` | Minor surviving mutants in non-critical paths | Pass |
| **Warning** | **≥ 95.0%** | `🟠 WARNING` | Approaching minimum safety threshold | Pass with alert |
| **Break** | **< 95.0%** | `❌ FAILED` | Quality gate breached | **Hard Gate Fail** (blocks CI & releases) |

---

## 2. 14-Package Matrix Breakdown

Mutation testing is executed via [`.github/workflows/mutation-testing.yml`](../.github/workflows/mutation-testing.yml) across all 14 packages in the ecosystem.

| Package / Component | Target Config | Output Directory | Artifact Name | Baseline Score | Gate Status |
|---------------------|---------------|------------------|---------------|:--------------:|:-----------:|
| `EricksonLopez.DomainPrimitives` | `stryker-config.json` | `StrykerOutput/core` | `stryker-report-core` | **100.0%** | `✅ HIGH` |
| `EricksonLopez.DomainPrimitives.Abstractions` | `stryker-abstractions-config.json` | `StrykerOutput/abstractions` | `stryker-report-abstractions` | **100.0%** | `✅ HIGH` |
| `EricksonLopez.DomainPrimitives.Generators` | `stryker-generators-config.json` | `StrykerOutput/generators` | `stryker-report-generators` | **100.0%** | `✅ HIGH` |
| `EricksonLopez.DomainPrimitives.Analyzers` | `stryker-analyzers-config.json` | `StrykerOutput/analyzers` | `stryker-report-analyzers` | **100.0%** | `✅ HIGH` |
| `EricksonLopez.DomainPrimitives.AspNetCore` | `stryker-aspnetcore-config.json` | `StrykerOutput/aspnetcore` | `stryker-report-aspnetcore` | **100.0%** | `✅ HIGH` |
| `EricksonLopez.DomainPrimitives.AspNetCore.SourceGenerators` | `stryker-aspnetcore-sourcegen-config.json` | `StrykerOutput/aspnetcore-sourcegen` | `stryker-report-aspnetcore-sourcegen` | **100.0%** | `✅ HIGH` |
| `EricksonLopez.DomainPrimitives.EFCore` | `stryker-efcore-config.json` | `StrykerOutput/efcore` | `stryker-report-efcore` | **100.0%** | `✅ HIGH` |
| `EricksonLopez.DomainPrimitives.EFCore.SourceGenerators` | `stryker-efcore-sourcegen-config.json` | `StrykerOutput/efcore-sourcegen` | `stryker-report-efcore-sourcegen` | **100.0%** | `✅ HIGH` |
| `EricksonLopez.DomainPrimitives.Dapper` | `stryker-dapper-config.json` | `StrykerOutput/dapper` | `stryker-report-dapper` | **100.0%** | `✅ HIGH` |
| `EricksonLopez.DomainPrimitives.Dapper.SourceGenerators` | `stryker-dapper-sourcegen-config.json` | `StrykerOutput/dapper-sourcegen` | `stryker-report-dapper-sourcegen` | **100.0%** | `✅ HIGH` |
| `EricksonLopez.DomainPrimitives.OpenApi` | `stryker-openapi-config.json` | `StrykerOutput/openapi` | `stryker-report-openapi` | **100.0%** | `✅ HIGH` |
| `EricksonLopez.DomainPrimitives.OpenApi.SourceGenerators` | `stryker-openapi-sourcegen-config.json` | `StrykerOutput/openapi-sourcegen` | `stryker-report-openapi-sourcegen` | **100.0%** | `✅ HIGH` |
| `EricksonLopez.DomainPrimitives.NewtonsoftJson` | `stryker-newtonsoftjson-config.json` | `StrykerOutput/newtonsoftjson` | `stryker-report-newtonsoftjson` | **100.0%** | `✅ HIGH` |
| `EricksonLopez.DomainPrimitives.Testing` | `stryker-testing-config.json` | `StrykerOutput/testing` | `stryker-report-testing` | **100.0%** | `✅ HIGH` |
| **Ecosystem Aggregate** | — | — | — | **100.0%** | `✅ HIGH` |

---

## 3. Architecture: Deferred Quality Gate

Due to the computational intensity of full AST mutation testing across 14 packages and over 20 test assemblies:

1. **Decoupled from PRs:** Fast CI (`ci.yml` invoking `dotnet-build-test.yml`) executes standard compilation, unit tests, code coverage (Coverlet), and SonarCloud analysis within ~3 minutes.
2. **Weekly Scheduled Execution:** `mutation-testing.yml` runs every Monday at 04:00 UTC across all 14 matrix jobs.
3. **Commit Status Reporting:** Upon run completion, the workflow registers a commit status check `stryker/mutation-gate` against the target commit SHA.
4. **Publish-Time Validation:** When publishing a release to NuGet.org via `publish.yml`, the workflow invokes `scripts/verify-mutation-gate.js` using `actions/github-script@v7`. If the release commit does not have a verified passing mutation score (≥ 95%), deployment is immediately halted.

---

## 4. Local Mutation Execution

To run mutation testing locally for a specific package, install the global tool and invoke Stryker using its dedicated configuration:

```bash
# Install Stryker.NET global tool
dotnet tool install --global dotnet-stryker

# Run mutation testing on the Core library
dotnet stryker --config-file stryker-config.json

# Run mutation testing on Dapper integration
dotnet stryker --config-file stryker-dapper-config.json

# Run mutation testing on ASP.NET Core Source Generators
dotnet stryker --config-file stryker-aspnetcore-sourcegen-config.json
```

HTML reports are generated in the `StrykerOutput/` folder and can be opened in any web browser for mutation diff analysis.
