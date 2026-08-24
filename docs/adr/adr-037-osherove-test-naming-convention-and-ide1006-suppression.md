# ADR 037: Adopt Osherove Test Naming Pattern and Suppress IDE1006 in Test Projects

## Status
Accepted

## Context
In standard .NET and Microsoft C# coding guidelines, methods are required to adhere to `PascalCase` without underscores. This rule is enforced by the Roslyn analyzer rule `IDE1006: Naming rule violation`. In production assemblies (`src/`), this convention ensures consistency, clean API surfaces, and seamless interoperability.

However, unit and integration tests fulfill a profoundly different architectural purpose than production classes:
1. **Living Documentation & Executable Specifications:** Tests are not general-purpose APIs; they represent formal requirements and executable specifications of system behavior under concrete domain conditions.
2. **Immediate CI/CD Triage:** When a test fails in continuous integration pipelines (GitHub Actions, Azure DevOps, Test Explorer, console test logs), the failure output reports only the class and method name. Engineers must be able to immediately discern three distinct pieces of information without needing to open the source code:
   - **UnitOfWork / Method Under Test:** What function or component is being exercised (e.g., `Create`, `TryValidate`, `Parse`, `AddParameters`).
   - **StateUnderTest / Scenario:** What specific condition, input boundary, or edge case is being simulated (e.g., `WhenEmailExceedsMaxLength`, `WithLeapYearDate`, `WhenSecretIsSensitive`).
   - **ExpectedBehavior / Result:** What the guaranteed contract or outcome should be (e.g., `ThrowsValidationException`, `ReturnsFailureResult`, `DoesNotExposeSensitiveValue`).

The standard naming pattern introduced by Roy Osherove (`UnitOfWork_StateUnderTest_ExpectedBehavior` / `Method_Scenario_Result`) uses underscores deliberately as semantic delimiters to segment these three concepts.

When `IDE1006` is enforced strictly across test projects, developers are forced to write run-on PascalCase test names (e.g., `CreateWhenEmailExceedsMaxLengthThrowsValidationException`), which severely degrades readability in terminal outputs, test report summaries, and pull request CI annotations.

## Decision
1. We institutionalize the **Roy Osherove test naming pattern (`Method_Scenario_Result` / `UnitOfWork_StateUnderTest_ExpectedBehavior`)** as the mandatory naming standard for all test suites across `tests/`.
2. We **locally disable the Roslyn analyzer rule `IDE1006` for all test projects** via `.editorconfig` (scoped to `[tests/**/*.cs]`).
3. We maintain strict `IDE1006` enforcement with zero tolerance (`severity = error/warning`) for all production source code under `src/`.

### Canonical Test Naming Anatomy
```csharp
// 1. Standard Osherove Pattern (Mandatory Default): [MethodUnderTest]_[ScenarioUnderTest]_[ExpectedResult]
[Fact]
public void Create_WhenInputExceedsMaxLength_ThrowsRangeError() { ... }

[Fact]
public void TryCreate_WithValidEmail_ReturnsSuccessAndNormalizedPrimitive() { ... }

[Fact]
public void Parse_WhenInputIsInvalidGuid_ThrowsFormatException() { ... }

// 2. Security Gate Compliance Variant: SEC###_[Component]_[Scenario]_[ExpectedOutcome]
[Fact, Trait("Category", "Security")]
public async Task SEC002_EmailAttribute_RegexUsesNonBacktracking_DoesNotHangOnAdversarialInput() { ... }

// 3. Domain Invariant & Contract Variant: [PropertyOrConcept]_[Condition]_[ExpectedBehavior]
[Fact]
public void Equal_Ids_Are_Equal() { ... }

[Fact]
public void Empty_Returns_DefaultInstance_And_Throws_On_Value_Access() { ... }
```

### Approved Taxonomy Exceptions
- **Security Gates:** Tests formally validating compliance against SEC-001 through SEC-006 are prefixed with the security gate identifier `SEC###_` for automated compliance audit reporting and triage.
- **Domain Invariant Tests:** Tests verifying fundamental framework behavior (e.g., Equal_Ids_Are_Equal, New_Creates_UniqueId) may omit the unit of work to focus purely on the behavioral contract.
- **Satellite Test Projects:** Standalone satellite integration projects (e.g. `NewtonsoftJson.Tests`, `OpenApi.Tests`) maintain a unified `.Tests` suffix encompassing both unit serializer logic and integration roundtrip verifications.

## Consequences
- **Positive:**
  - **Living Specifications:** Test methods read as clear, natural-language behavioral specifications in CI logs and IDE Test Explorers.
  - **Rapid Incident Triage:** Failed tests in CI immediately communicate *what failed*, *under what premise*, and *what was expected*.
  - **Architectural Pragmatism:** Prioritizes QA clarity and test maintainability over dogmatic application of production API naming rules to test methods.
  - **Documented Consistency:** Eliminates ambiguity regarding approved naming exceptions across domain invariants and security gates.
- **Negative:**
  - Method names in `tests/` differ in convention from method names in `src/`.
- **Mitigation:**
  - The suppression of `IDE1006` is strictly localized to the `tests/` folder hierarchy in `.editorconfig`, preventing any underscore naming leaks into production packages.

