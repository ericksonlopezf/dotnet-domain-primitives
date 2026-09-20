# DevSecOps Quality Gates Specification

This document defines the formal automated Quality Gates enforced across the `EricksonLopez.DomainPrimitives` software development lifecycle. Every gate is automated in continuous integration workflows, ensuring that no pull request can be merged and no package can be published without satisfying strict architectural, reliability, and security invariants.

---

## Quality Gate Philosophy

Quality in `EricksonLopez.DomainPrimitives` is preventative rather than detective. By shifting quality enforcement to compile-time source generation, Roslyn analyzers, strict build flags, and zero-allocation assertions, regressions are caught at the earliest possible stage.

```mermaid
flowchart LR
    G1["Gate 1<br/>Build & Diagnostics"] --> G2["Gate 2<br/>Fast Tests"]
    G2 --> G3["Gate 3<br/>Coverage &ge;99%"]
    G3 --> G4["Gate 4<br/>SonarCloud"]
    G4 --> G5["Gate 5<br/>NativeAOT Smoke"]
    G5 --> G6["Gate 6<br/>Benchmark Gate"]
    G6 --> G7["Gate 7<br/>Stryker Mutation"]
    
    classDef gate fill:#512BD4,stroke:#fff,color:#fff;
    class G1,G2,G3,G4,G5,G6,G7 gate;
```

---

## Detailed Gate Specifications

### Gate 1: Strict Compilation & Compiler Diagnostics

| Property | Specification |
|:---|:---|
| **Enforcement** | MSBuild build pipeline (`Directory.Build.props`) |
| **Workflow** | `ci.yml` / `dotnet-build-test.yml`, `repo-compliance.yml` |
| **Command** | `dotnet build EricksonLopez.DomainPrimitives.slnx --configuration Release` |
| **Threshold** | Zero warnings (`TreatWarningsAsErrors=true`, `WarningLevel=5`) |
| **Scope** | All 14 production projects, 19 test projects, benchmarks, and samples |

- **Zero Tolerance Warnings:** Any compiler warning is treated as a fatal build error.
- **XML Documentation:** Public API XML comments (`GenerateDocumentationFile=true`) are mandatory on all public members; missing documentation generates CS1591 errors.
- **Code Style:** `<EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>` enforces .NET naming and formatting rules across every build.
- **Dependency Audit:** `<NuGetAudit>true</NuGetAudit>` with `<NuGetAuditLevel>low</NuGetAuditLevel>` scans dependencies during restore for vulnerabilities.

---

### Gate 2: Fast-Path Unit & Contract Tests

| Property | Specification |
|:---|:---|
| **Enforcement** | xUnit runner in CI pipeline |
| **Workflow** | `ci.yml` / `dotnet-build-test.yml` |
| **Command** | `dotnet test EricksonLopez.DomainPrimitives.slnx --no-build --configuration Release` |
| **Threshold** | 100% test pass rate across .NET 8.0, .NET 9.0, and .NET 10.0 |
| **Scope** | Unit, integration, architecture, concurrency, fuzzing, and property-based test suites |

- **Multi-TFM Execution:** All test suites run against all targeted .NET runtimes to prevent runtime-specific regressions.
- **Contract Integrity:** Property-based and fuzzing tests assert algebraic invariants (identity, idempotence, symmetry).
- **Architecture Validation:** NetArchTest rules verify layering boundaries and absence of forbidden dependencies in abstractions.

---

### Gate 3: Code Coverage Gate ($\ge 99\%$)

| Property | Specification |
|:---|:---|
| **Enforcement** | Coverlet + Codecov (`.codecov.yml`) |
| **Workflow** | `dotnet-build-test.yml`, `publish.yml` |
| **Command** | `dotnet test --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover,cobertura` |
| **Threshold** | Overall project coverage $\ge 99\%$ (1% tolerance), PR patch coverage $\ge 90\%$ (5% tolerance) |
| **Scope** | Production code (`src/**`), excluding test projects, samples, benchmarks, and generated artifacts |

- **Strict Precision:** Codecov tracks coverage to 2 decimal places with `require_ci_to_pass: true`.
- **Patch Policy:** Every pull request must have $\ge 90\%$ test coverage on modified lines.

---

### Gate 4: SonarCloud Static Code Analysis

| Property | Specification |
|:---|:---|
| **Enforcement** | SonarScanner for .NET (`sonarcloud.io`) |
| **Workflow** | `dotnet-build-test.yml` |
| **Threshold** | SonarCloud Quality Gate "A" (0 bugs, 0 vulnerabilities, 0 security hotspots) |
| **Scope** | All production C# source code |

- **Security Analysis:** Scans for injection flaws, weak cryptography, resource leaks, and AST anomalies.
- **Duplication Threshold:** Duplication strictly barred on domain primitives and value object structures.

---

### Gate 5: NativeAOT & Trimming Smoke Test

| Property | Specification |
|:---|:---|
| **Enforcement** | Standalone .NET 10 NativeAOT compiler (`PublishAot=true`) |
| **Workflow** | `aot-smoke-test.yml` (called on every push and PR) |
| **Command** | `dotnet publish tests/EricksonLopez.DomainPrimitives.AotSmokeTest --configuration Release --runtime linux-x64 --self-contained -p:PublishAot=true -p:TreatWarningsAsErrors=true` |
| **Threshold** | 0 IL2026/IL3050 trimmer warnings; exit code `0` on executed native binary |
| **Scope** | All core primitives, source generators, and model binding converters |

- **True AOT Validation:** Rather than merely checking attributes, a real Linux-x64 binary is compiled and executed in a clean container.
- **Reflection Elimination:** Any use of unannotated reflection or dynamic code generation halts the pipeline immediately.

---

### Gate 6: Benchmark Regression & Zero-Allocation Gate

| Property | Specification |
|:---|:---|
| **Enforcement** | BenchmarkDotNet + PowerShell Gate Script (`verify-benchmark-gate.ps1`) |
| **Workflow** | `benchmark-regression-gate.yml` (runs on PRs touching `src/**` or `benchmarks/**`) |
| **Command** | `dotnet run --project benchmarks/.../EricksonLopez.DomainPrimitives.Benchmarks.csproj -c Release -f net10.0 -- --filter "*" --job short --exporters json --memory` |
| **Threshold** | **0 B allocated** on hot paths (`TryCreate`, `Parse`, comparisons); latency regression $\le 5\%$ vs `baseline.json` |
| **Scope** | All primary operations across string, numeric, date, and smart enum primitives |

- **Heap Invariant:** Zero heap allocations on hot path combinators is non-negotiable. Any PR introducing 1+ bytes of GC allocation on hot paths is blocked.
- **Regression Detection:** Latency deviations greater than 5% relative to the checked-in baseline fail the pull request.

---

### Gate 7: Stryker Mutation Testing Quality Gate ($\ge 95\%$)

| Property | Specification |
|:---|:---|
| **Enforcement** | Stryker.NET 14-job parallel matrix (`mutation-testing.yml`) |
| **Workflow** | Weekly schedule (Monday 04:00 UTC) and pre-release validation gate (`publish.yml`) |
| **Threshold** | Mutation score $\ge 95\%$ break threshold (High target: $100\%$, Warning: $95\%$) |
| **Scope** | All 14 packages configured via dedicated `stryker-*.json` profiles |

- **Anti-Gaming Rules:** Mutation testing ensures that unit tests actively assert invariants rather than merely executing code for coverage.
- **Release Blocker:** `publish.yml` executes `verify-mutation-gate.js`, inspecting commit statuses to guarantee that published commits have verified Stryker mutation scores $\ge 95\%$.

---

## Quality Gate Execution Matrix

| Gate | Local Dev | PR to Develop/Main | Push to Main | Release Publish |
|:---|:---:|:---:|:---:|:---:|
| **Gate 1: Build & Diagnostics** | ✅ Manual | ✅ Mandatory (Blocking) | ✅ Mandatory (Blocking) | ✅ Mandatory (Blocking) |
| **Gate 2: Fast Tests** | ✅ Manual | ✅ Mandatory (Blocking) | ✅ Mandatory (Blocking) | ✅ Mandatory (Blocking) |
| **Gate 3: Code Coverage** | 🟡 Optional | ✅ Mandatory (Blocking) | ✅ Mandatory (Reporting) | ✅ Mandatory (Blocking) |
| **Gate 4: SonarCloud** | 🟡 Optional | ✅ Mandatory (Blocking) | ✅ Mandatory (Reporting) | — |
| **Gate 5: NativeAOT Smoke** | 🟡 Optional | ✅ Mandatory (Blocking) | ✅ Mandatory (Blocking) | — |
| **Gate 6: Benchmark Gate** | 🟡 Optional | ✅ Mandatory (PR paths) | — | — |
| **Gate 7: Stryker Mutation** | 🟡 On-demand | ⏳ Weekly / Pre-Release | ⏳ Scheduled | ✅ Release Gate Check |

---

## Failure Remediation Protocol

1. **Gate 1 Failures:** Inspect build output, correct CS warnings, and verify XML documentation on all new public APIs.
2. **Gate 2 Failures:** Run `dotnet test --filter "FullyQualifiedName~<TestName>"` locally to isolate regressions.
3. **Gate 3 Failures:** Check Codecov PR comment for uncovered branches and add targeted test cases.
4. **Gate 5 Failures:** Check ILC compiler warnings for unannotated reflection or dynamic code generation.
5. **Gate 6 Failures:** Run `verify-benchmark-gate.ps1` locally to identify which benchmark exceeded the 5% regression or 0 B allocation threshold.
6. **Gate 7 Failures:** Open the generated HTML mutation report in `StrykerOutput/`, locate survived mutants, and add assertions that kill them.
