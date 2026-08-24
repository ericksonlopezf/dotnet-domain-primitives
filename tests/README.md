# 🧪 DomainPrimitives Test Suite Architecture & Guide

Welcome to the test suite for **EricksonLopez.DomainPrimitives**. This suite is engineered for extreme execution speed, multi-framework determinism (.NET 8, 9, 10), and strict verification of domain invariants and security gates.

---

## 🏛️ Test Project Taxonomy

The suite is structured into 5 cohesive tiers across 19 test projects:

```
tests/
├── 1. Core & Abstractions
│   ├── EricksonLopez.DomainPrimitives.Abstractions.UnitTests/  → Attributes, exceptions, interfaces, and error model
│   └── EricksonLopez.DomainPrimitives.UnitTests/              → Invariants of generated types, Security Gates, and BCL
│
├── 2. Roslyn Generators & Analyzers
│   ├── EricksonLopez.DomainPrimitives.SourceGenerators.Tests/   → Incremental generators (String, Numeric, Date, Id, VO)
│   ├── EricksonLopez.DomainPrimitives.Analyzers.Tests/          → Roslyn Analyzers (DP0001–DP0017) & CodeFixes
│   ├── EricksonLopez.DomainPrimitives.AspNetCore.SourceGenerators.Tests/
│   ├── EricksonLopez.DomainPrimitives.EFCore.SourceGenerators.Tests/
│   ├── EricksonLopez.DomainPrimitives.Dapper.SourceGenerators.Tests/   → Snapshot baseline tests with Verify.Xunit
│   └── EricksonLopez.DomainPrimitives.OpenApi.SourceGenerators.Tests/
│
├── 3. Satellite Integration & Converter Tests
│   ├── EricksonLopez.DomainPrimitives.AspNetCore.UnitTests/     → ModelBinder & MvcOptions
│   ├── EricksonLopez.DomainPrimitives.EFCore.UnitTests/         → ValueConverter unit tests
│   ├── EricksonLopez.DomainPrimitives.EFCore.IntegrationTests/  → SQLite in-memory relational tests
│   ├── EricksonLopez.DomainPrimitives.Dapper.IntegrationTests/  → Dapper TypeHandler & SQLite tests
│   ├── EricksonLopez.DomainPrimitives.OpenApi.Tests/            → Swashbuckle / OpenAPI SchemaFilters
│   └── EricksonLopez.DomainPrimitives.NewtonsoftJson.Tests/    → Newtonsoft.Json JsonConverter & ContractResolver
│
├── 4. Testing SDK for Consumers
│   └── EricksonLopez.DomainPrimitives.Testing.UnitTests/        → AssertionsExtensions, TestBuilder, FakeFactory, Scenarios
│
└── 5. Architecture, Governance & End-to-End
    ├── EricksonLopez.DomainPrimitives.ArchitectureTests/        → NetArchTest dependency rules & clean boundaries
    ├── EricksonLopez.DomainPrimitives.IntegrationTests/         → Cross-package smoke tests
    ├── EricksonLopez.DomainPrimitives.EndToEndTests/            → Serialization & lifecycle roundtrip
    └── EricksonLopez.DomainPrimitives.AotProbe/                 → Native AOT compilation & trimming probe
```

---

## 🎯 Testing Principles & Conventions

1. **FIRST Principles**:
   - **Fast**: The entire multi-target test suite (>600 tests) executes in **< 3 seconds**.
   - **Independent**: No test depends on the execution order or shared state of other tests.
   - **Repeatable**: Deterministic dates and test fixtures provided by `DomainPrimitiveFakeFactory` and `DomainPrimitiveScenarios`.
   - **Self-validating**: Automated validation with binary exit codes.
   - **Timely**: Tests constructed in parallel with production features.

2. **Naming Convention: Osherove Pattern (`Method_Scenario_Result`)**:
   - Roy Osherove's standard (`UnitOfWork_StateUnderTest_ExpectedBehavior` / `Method_Scenario_Result`) is adopted institutionally across all test projects.
   - Tests act as **Living Documentation** readable in CI reports and Test Explorer.
   - Rule `IDE1006` (PascalCase violation) is disabled locally for `tests/**/*.cs` in `.editorconfig` to prioritize semantic clarity.
   - See [adr-037: Adopt Osherove Test Naming Pattern and Suppress IDE1006 in Test Projects](../docs/adr/adr-037-osherove-test-naming-convention-and-ide1006-suppression.md).

   ```csharp
   // Canonical Anatomy:
   [Fact]
   public void Create_WhenInputExceedsMaxLength_ThrowsRangeError() { ... }

   [Fact]
   public void TryCreate_WithValidEmail_ReturnsSuccessAndNormalizedPrimitive() { ... }

   [Fact]
   public void Parse_WhenInputIsInvalidGuid_ThrowsFormatException() { ... }
   ```

3. **Fluent Assertions**:
   - Uses **AwesomeAssertions (FluentAssertions)** in a unified style (`value.Should().Be(...)`, `act.Should().Throw<...>()`).
   - Exception assertions must explicitly verify the error code:
     ```csharp
     act.Should().Throw<DomainPrimitiveValidationException>()
        .Where(e => e.Error.Code == "RANGE");
     ```

4. **Snapshot Testing with Verify.Xunit**:
   - Incremental source generator tests reside in `*.SourceGenerators.Tests`.
   - Baseline `.verified.cs` files are located in dedicated `Snapshots/` folders.

---

## 🚀 Execution Commands

### Run entire test suite (all frameworks):
```bash
dotnet test EricksonLopez.DomainPrimitives.slnx --logger "console;verbosity=minimal"
```

### Run specific target framework (e.g. .NET 10 or .NET 8):
```bash
dotnet test EricksonLopez.DomainPrimitives.slnx -f net10.0
dotnet test EricksonLopez.DomainPrimitives.slnx -f net8.0
```

### Run Mutation Testing (Stryker.NET):
```bash
# Install dotnet-stryker globally
dotnet tool install -g dotnet-stryker

# Execute with centralized configuration (threshold break: 95%, high: 100%)
dotnet-stryker
```

### Stryker Configuration & Coverage Analysis Rationale
The root `stryker-config.json` sets `"coverage-analysis": "off"` by design:
- **Full Mutation Exhaustiveness:** Because the core unit and integration test suite runs in under 3 seconds, evaluating all tests against all mutants eliminates false escapes from heuristic test-filtering and guarantees 100% deterministic mutation scoring in scheduled quality gates.
- **PR Build Acceleration:** In high-frequency PR CI pipelines, `dotnet-stryker --coverage-analysis perTest` can be invoked for sub-minute differential runs without altering the master config.
- **Zero Suppression Policy:** The entire codebase enforces a strict zero `// Stryker disable` policy; all business invariants are genuinely covered and verified.

### 📦 Satellite Integration Taxonomy
Standalone satellite packages (such as `EricksonLopez.DomainPrimitives.NewtonsoftJson.Tests` and `EricksonLopez.DomainPrimitives.OpenApi.Tests`) use the unified `.Tests` naming suffix. This intentional architectural convention bundles unit-level serializer converters/filters and end-to-end serialization roundtrips into a single, cohesive test assembly per integration target.

### 🛠️ Testing SDK (`DomainPrimitiveFakeFactory`) Architecture
To prevent the fake test factory from becoming a monolithic god-object while preserving 100% backward compatibility with consumer code:
- `DomainPrimitiveFakeFactory` is physically partitioned across domain-specific partial files (`DomainPrimitiveFakeFactory.Dates.cs`, `DomainPrimitiveFakeFactory.Identifiers.cs`, `DomainPrimitiveFakeFactory.Numerics.cs`, `DomainPrimitiveFakeFactory.Shortcuts.cs`).
- All public static properties remain directly accessible on `DomainPrimitiveFakeFactory` (enforced by `PublicAPI.Shipped.txt`), ensuring optimal discoverability without breaking changes.

> [!NOTE]
> For architectural details on the justified exclusion of Roslyn Source Generators from Stryker.NET mutation testing, see [adr-032: Exclude Source Generators from Mutation Testing](../docs/adr/adr-032-exclude-source-generators-mutation-testing.md).
> For test naming conventions, see [adr-037: Adopt Osherove Test Naming Pattern and Suppress IDE1006 in Test Projects](../docs/adr/adr-037-osherove-test-naming-convention-and-ide1006-suppression.md).
