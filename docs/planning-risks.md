# Planning Risks & Mitigation

> **Version:** 1.2.0-unreleased  
> **Last Updated:** 2026-08-10  
> **Required by:** AUDIT.md §11.11  
> **Format updated:** 2026-08-10 (HIGH-V4-003) — added structured fields per §11.11

---

## Risk Register Format

Each risk must specify all §11.11 required fields:

| Field | Description |
|-------|-------------|
| **Risk ID** | Unique identifier (R01, R02, ...) |
| **Description** | What could go wrong |
| **Probability** | L (Low <30%) / M (Medium 30-60%) / H (High >60%) |
| **Impact** | L (Low) / M (Medium) / H (High) / C (Critical — blocks release) |
| **Risk Score** | Probability × Impact matrix value |
| **Affected phase** | Which part of the development lifecycle this affects |
| **Mitigation** | How we prevent or reduce the risk |
| **Trigger** | Observable event that indicates the risk is materializing |
| **Owner** | Who is responsible for monitoring and mitigation |
| **Target resolution** | Version by which the risk must be resolved or accepted |
| **Status** | Active / Mitigated / Accepted / Resolved |

---

## Risk Matrix

| R ID | Description | Prob | Impact | Score | Status |
|------|-------------|------|--------|-------|--------|
| R01 | NativeAOT Dependency Drift | L | C | **L·C = High** | Mitigated |
| R02 | Roslyn Generator Performance (IDE Freeze) | M | M | **M·M = Medium** | ✅ Resolved (v1.2.0) |
| R03 | C# Feature Evolution | L | M | **L·M = Low** | Monitored |
| R04 | Cognitive Complexity Escalation | L | M | **L·M = Low** | Compliant |
| R05 | Allocation Creep in Hot Paths | M | H | **M·H = High** | Monitored |
| R06 | API Surface Budget Creep | M | M | **M·M = Medium** | ✅ Resolved (v1.2.0) |
| R07 | INumber\<T\> Interface Debt | L | L | **L·L = Low** | Accepted |
| R08 | SuperStrong.Types BCL Differentiation Threat | L | M | **L·M = Low** | 🔵 Monitored |

---

## Active Risks (Detailed)

---

### R01: NativeAOT Dependency Drift

| Field | Value |
|-------|-------|
| **Probability** | L (Low) |
| **Impact** | C (Critical — blocks release) |
| **Risk Score** | High |
| **Affected phase** | Integration, Release |
| **Trigger** | `dotnet publish -p:PublishAot=true` produces IL3050/IL2026 warnings or test failures |
| **Owner** | Core maintainers |
| **Target resolution** | Ongoing — re-verified on every release |
| **Status** | ✅ Mitigated |

**Description:** New integrations (like EF Core or Dapper updates) may introduce runtime reflection dependencies that break `PublishAot`.

**Mitigation:** CI gate that executes `dotnet publish -p:PublishAot=true` against the full test suite on every PR. Any new dependency must pass AOT probe before merge.

**Evidence of mitigation:** NativeAOT CI job added to `.github/workflows/aot-smoke-test.yml`. AOT probe app in `tests/EricksonLopez.DomainPrimitives.AotProbe/` compiles and passes on every CI run.

---

### R02: Roslyn Generator Performance (IDE Freeze)

| Field | Value |
|-------|-------|
| **Probability** | M (Medium — only in large solutions with 50+ primitives) |
| **Impact** | M (Medium — UX degradation, no correctness impact) |
| **Risk Score** | Medium |
| **Affected phase** | Development experience (IDE) |
| **Trigger** | Editing a non-generator-related file triggers re-execution of a generator; IDE response time >2s on a solution with 20+ generated types |
| **Owner** | Generator maintainer |
| **Target resolution** | v1.3.0 (TD-014) |
| **Status** | ✅ **Resolved — v1.2.0** |

**Description:** Complex generators iterating over syntax nodes can cause Visual Studio/Rider to freeze if they don't efficiently use `ForAttributeWithMetadataName` and incremental caching.

**Resolution (v1.2.0 — TD-014):** All 6 generators (String, Numeric, Date, StrongId, ValueObject, SmartEnum) migrated from `CreateSyntaxProvider` + `IsCandidateRecordStruct` to `ForAttributeWithMetadataName`. FQNs centralized in `GeneratorShared`. `IsReadonlyRecordStruct` predicate is O(1) and allocation-free. Deduplication applied to all multi-FQN generators via Collect+SelectMany pattern.

---

### R03: C# Feature Evolution (e.g. Primary Constructors)

| Field | Value |
|-------|-------|
| **Probability** | L (Low — C# 14 is stable; C# 15 features are preview) |
| **Impact** | M (Medium — generated code may emit warnings; no behavioral impact) |
| **Risk Score** | Low |
| **Affected phase** | Development, Release |
| **Trigger** | SDK upgrade produces new CS warnings in generated code or snapshot test failures |
| **Owner** | Generator maintainer |
| **Target resolution** | Evaluated on each SDK upgrade |
| **Status** | 🔵 Monitored |

**Description:** Emitted code may trigger warnings in newer C# versions (e.g., C# 14/15) if language idioms change (e.g., `field` keyword, `params collections`).

**Mitigation:** Use strict `#nullable enable` and avoid bleeding-edge syntax in generated output unless guarded by `#if NET...` compiler directives. C# 14 features tracked in TD-017.

---

### R04: Cognitive Complexity Escalation

| Field | Value |
|-------|-------|
| **Probability** | L (Low — Rule of 25 enforced via API surface budget tests) |
| **Impact** | M (Medium — onboarding friction, maintenance burden) |
| **Risk Score** | Low |
| **Affected phase** | Design, Implementation |
| **Trigger** | API surface budget test fails; or `docs/api-surface-budget.md` shows >10% budget consumption increase between releases |
| **Owner** | All contributors |
| **Target resolution** | Ongoing — budget tests run on every PR |
| **Status** | ✅ Compliant — budget gate active |

**Description:** Generated types become "god objects" implementing too many interfaces, making the API surface overwhelming.

**Mitigation:** `ApiSurfaceBudgetTests` enforce per-category limits. Any increase requires an RFC (per §PUBLIC API GOVERNANCE). Current max measured surface: 37 members (Distance, NumericPrimitive+Operations).

---

### R05: Allocation Creep in Hot Paths

| Field | Value |
|-------|-------|
| **Probability** | M (Medium — contributors unfamiliar with allocation rules are likely) |
| **Impact** | H (High — violates P3 zero-cost abstraction; breaks marketing claim) |
| **Risk Score** | High |
| **Affected phase** | Implementation, Review |
| **Trigger** | BenchmarkDotNet `[MemoryDiagnoser]` shows `Allocated > 0B` in `Create`, `TryCreate`, `Parse`, `TryParse`, `Format`, `Equals`, `GetHashCode`, or `CompareTo` |
| **Owner** | All contributors |
| **Target resolution** | Ongoing — BDN benchmarks run on every release |
| **Status** | 🔵 Monitored (GAP-009 — public results not yet published) |

**Description:** Future contributors might accidentally allocate closures or heap objects in `TryCreate` or `TryParse`.

**Mitigation:** Use `ArrayPool`, `ReadOnlySpan<T>`, and mutation testing to detect allocations. Stryker mutation suite covers hot paths. BenchmarkDotNet suite must show `Allocated: 0B` before release.

**Open risk:** Benchmark results are not yet published (GAP-009). Without public evidence, the "zero-allocation" claim is unverified by external observers.

---

### R06: API Surface Budget Creep

| Field | Value |
|-------|-------|
| **Probability** | M (Medium — as features are added, surface grows) |
| **Impact** | M (Medium — cognitive overload for users; maintenance burden) |
| **Risk Score** | Medium |
| **Affected phase** | Design, Implementation |
| **Trigger** | `ApiSurfaceBudgetTests` CI fails |
| **Owner** | Core maintainers |
| **Target resolution** | v1.2.0 (TD-015 — CI step to surface budget failures clearly) |
| **Status** | 🟡 Active — budget gate in unit tests; dedicated CI step missing (TD-015) |

**Description:** New features add members to generated types; without a gate, budget creep is invisible.

**Mitigation:** `ApiSurfaceBudgetTests` (9 tests) run on every CI build. Dedicated CI step with `ApiSurfaceBudget` trait needed (TD-015).

---

### R07: INumber\<T\> Interface Debt

| Field | Value |
|-------|-------|
| **Probability** | L (Low — currently opt-in only; implementation not started) |
| **Impact** | L (Low — only affects users explicitly opting in) |
| **Risk Score** | Low |
| **Affected phase** | Implementation (future, v2.0) |
| **Trigger** | GAP-004 RFC filed and assigned to v2.0 milestone |
| **Owner** | Feature owner (to be assigned at RFC time) |
| **Target resolution** | v2.0.0 |
| **Status** | 🔵 Accepted — deferred by design |

**Description:** `INumber<T>` implementation for `NumericPrimitive` is planned as opt-in but the large interface surface creates risk of budget violation and API confusion.

**Mitigation:** GAP-004 clarified that `INumber<T>` is opt-in only. Most members must be hidden with `[EditorBrowsable(Never)]`. RFC required before implementation. Budget impact must be measured.

---

### R08: SuperStrong.Types BCL Differentiation Threat

| Field | Value |
|-------|-------|
| **Probability** | L (Low — library is early-stage with thin evidence of completeness) |
| **Impact** | M (Medium — if it matures, DP's primary BCL-Native differentiator becomes contested) |
| **Risk Score** | Low |
| **Affected phase** | Positioning, Differentiation |
| **Trigger** | SuperStrong.Types reaches 500+ NuGet downloads/month OR publishes verified benchmark comparison claiming IUtf8SpanParsable<T> parity |
| **Owner** | Core maintainers |
| **Target resolution** | Quarterly competitive review — LATER-005 in roadmap |
| **Status** | 🔵 Monitored |

**Description:** `SuperStrong.Types` is the only emerging library explicitly targeting
`ISpanFormattable`, `IUtf8SpanFormattable`, `ISpanParsable<T>`, and `IUtf8SpanParsable<T>` —
the same BCL interface story that is DP's Pillar 1 differentiator (adr-028). If it achieves
feature parity and publishes credible benchmarks before DP does, the "unique BCL integration"
claim is weakened.

**Mitigation:**
1. Publish DP's comparative benchmarks first (NOW-001 in roadmap) — establish the evidence
   baseline before competitors can contest it.
2. Conduct quarterly evidence-based review of SuperStrong.Types maturity.
3. If parity is reached, identify next differentiation dimension (likely: security gates +
   auto-discovery — neither of which SuperStrong.Types targets).

**Evidence of current status:** SuperStrong.Types mentions ISpanFormattable/IUtf8SpanFormattable
in its README, but evidence of completeness (tests, benchmarks, NuGet downloads) is thin as of
2026-08-10. Risk is Low now; re-evaluate at 2026-11-10.

---

## Resolved / Historical Risks

| R ID | Description | Resolution | Version |
|------|-------------|-----------|---------|
| R-AOT-v1 | NativeAOT CI gate missing | CI job added | v1.2.0 |
| R-COMPAT-v1 | `EmbedUntrackedSources` was CI-conditional (CRIT-006) | Now unconditionally `true` | v1.2.0 |
| R-REFLECT-v1 | Reflection in generator's namespace check | Fixed — namespace check is compile-time only | v1.2.0 |
