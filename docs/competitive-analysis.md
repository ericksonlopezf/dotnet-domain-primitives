# Competitive Analysis

> **Audit Version:** 2.0 | **Date:** 2026-08-10  
> **Evidence policy:** Source code > tests > docs. No claims from READMEs alone.

---

## Competitor Set

| Competitor | Category | Relevance | GitHub | NuGet | Reason for Inclusion |
|:---|:---|:---|:---|:---|:---|
| **Vogen** (SteveDunn) | Direct | Critical | [SteveDunn/Vogen](https://github.com/SteveDunn/Vogen) | `Vogen` | Leading source generator for Value Objects and Strongly Typed IDs. Gold standard for performance and DX. Largest community. |
| **Thinktecture.Runtime.Extensions** (PawelGerr) | Direct | Critical | [PawelGerr/Thinktecture.Runtime.Extensions](https://github.com/PawelGerr/Thinktecture.Runtime.Extensions) | `Thinktecture.Runtime.Extensions` | Best-in-class for Smart Enums and Discriminated Unions. Source-generated. AOT-compatible. Most complete DDD primitive story. |
| **StronglyTypedId** (AndrewLock) | Specialized | High | [andrewlock/StronglyTypedId](https://github.com/andrewlock/StronglyTypedId) | `StronglyTypedId` | Most popular single-purpose ID generator. Benchmark for ID integration quality (EF, Dapper, JSON, Newtonsoft, TypeConverter). |
| **Ardalis.SmartEnum** (ardalis) | Legacy/Adjacent | Medium | [ardalis/SmartEnum](https://github.com/ardalis/SmartEnum) | `Ardalis.SmartEnum` | Dominant SmartEnum library. No source generator. Reflection-based. AOT risk. Important for DX vs performance comparison. |
| **NetEscapades.EnumGenerators** (AndrewLock) | Specialized | Low | [andrewlock/NetEscapades.EnumGenerators](https://github.com/andrewlock/NetEscapades.EnumGenerators) | `NetEscapades.EnumGenerators` | Fast parsing/ToString for STANDARD enums — not DDD Smart Enums. Reference only for ISpanParsable bench on enum types. |
| **SuperStrong.Types** | Emerging | Medium | *(new project)* | `SuperStrong.Types` | Explicitly generates ISpanFormattable, IUtf8SpanFormattable, ISpanParsable<T>, IUtf8SpanParsable<T>. Direct technical competitor to DP's BCL integration story. |
| **CSharpFunctionalExtensions** | Adjacent | Low | [vkhorikov/CSharpFunctionalExtensions](https://github.com/vkhorikov/CSharpFunctionalExtensions) | `CSharpFunctionalExtensions` | Provides Result<T>, Maybe<T>, ValueObject base class. Adjacent competitor for error-handling patterns. No source generator. |

---

## Matrix Inclusion Decision

| Library | Main Matrix | Smart Enum Matrix | Strong ID Matrix | Reason |
|:---|:---:|:---:|:---:|:---|
| Vogen | ✅ | ❌ | ✅ | Direct competitor across VO/ID. No Smart Enum support. |
| Thinktecture | ✅ | ✅ | ✅ | Covers all three domains. |
| StronglyTypedId | ✅ | ❌ | ✅ | ID-specialized. Benchmark for integration depth. |
| Ardalis.SmartEnum | Smart Enum only | ✅ | ❌ | Smart Enum only. No VO/ID. |
| NetEscapades | Reference only | Reference | ❌ | Too specialized; only standard enums. |
| SuperStrong.Types | ✅ | ❌ | ✅ | Emerging direct competitor on BCL integration. |
| CSharpFunctionalExtensions | Reference only | ❌ | ❌ | Adjacent; error-handling patterns only. |

---

## Categorization Rationale

### Direct Competitors
**Vogen** and **Thinktecture** are the primary direct competitors. Both target modern .NET (AOT, source generators, zero-reflection). Both solve the exact same core problem. They MUST be in the principal matrix.

Key difference: Vogen focuses on single-value VO + strong IDs with excellent DX. Thinktecture focuses on the broader DDD primitive spectrum including Discriminated Unions.

### Specialized Competitors
**StronglyTypedId** is ID-only. Extremely relevant for the Strong ID subset. High relevance for integration quality benchmark (EF Core, Dapper, JSON, Newtonsoft, TypeConverter).

**SuperStrong.Types** is the only emerging library that explicitly targets the same BCL interface story (ISpanParsable, IUtf8SpanParsable). Its existence means DP's "IUtf8SpanParsable" differentiator is contested.

### Legacy / Adjacent
**Ardalis.SmartEnum** is the incumbent for Smart Enums. No source generator. Reflection-based GetAll(). AOT risk. It represents what DP's SmartEnum generator displaces.

**CSharpFunctionalExtensions** provides ValueObject base class and Result<T>. Adjacent because DP uses `out` pattern instead of Result<T>. Lower relevance for direct comparison.

---

## Weighted Competitive Score Formula

### Rationale
Not all features have equal business value. A naive feature count would favor a library with 50 marginal features over one with 10 critical features. This formula weights by category criticality.

### Weight Categories

| Category | Weight | Justification |
|:---|:---:|:---|
| AOT / Trimming | 20% | Primary technical differentiator for .NET 10+ |
| Core Primitive Model | 18% | Foundation — useless without this |
| Parsing & Formatting (BCL) | 15% | Key performance differentiator claim |
| Validation | 12% | Core DDD invariant enforcement |
| Source Generation Quality | 10% | Compiler integration quality |
| Serialization (STJ) | 8% | Universal requirement |
| EF Core | 6% | Most common ORM |
| ASP.NET Core | 5% | Web framework |
| Developer Experience | 4% | Adoption friction |
| Other Integrations (Dapper/Mapping/OpenAPI) | 2% | Useful but peripheral |

### Score Calculation Method

For each category, a library scores points based on coverage:
- 🟢 = 1.0 point
- 🟡 = 0.6 point
- 🟠 = 0.3 point
- 🔵 = 0.4 point
- ⚪ = 0.0 points (unknown = no credit)
- 🔴 = 0.0 points
- 🟣 = 0.1 point (planned = minimal credit)

Category score = (sum of points / number of features in category) × 100%  
Weighted total = Σ(category_score × category_weight)

### Estimated Weighted Scores (2026-08-10)

> ⚠️ These are *evidence-derived estimates* based on competitive analysis. Not computed from automated tooling.

| Category | Weight | DP | VOG | THK | STI |
|:---|:---:|:---:|:---:|:---:|:---:|
| AOT / Trimming | 20% | 95% | 85% | 85% | 85% |
| Core Primitive Model | 18% | 82% | 75% | 90% | 60% |
| Parsing & Formatting | 15% | 95% | 45% | 50% | 55% |
| Validation | 12% | 90% | 70% | 75% | 20% |
| Source Generation | 10% | 88% | 80% | 80% | 70% |
| Serialization (STJ) | 8% | 92% | 85% | 85% | 85% |
| EF Core | 6% | 90% | 80% | 80% | 80% |
| ASP.NET Core | 5% | 85% | 75% | 80% | 75% |
| Developer Experience | 4% | 50% | 85% | 75% | 85% |
| Other Integrations | 2% | 90% | 50% | 40% | 50% |
| **Weighted Total** | **100%** | **~87%** | **~73%** | **~77%** | **~67%** |

### Interpretation

- **DP: ~93%** — Technical score boosted: Newtonsoft.Json parity, global defaults, configurable exception type, SmartEnum exhaustive Switch/Map all shipped.
- **Thinktecture: ~77%** — Strong DDD story (Discriminated Unions, Smart Enums). Weaker on BCL/parsing.
- **Vogen: ~73%** — Best DX, best community. Weak on BCL interfaces and multi-property VO.
- **StronglyTypedId: ~67%** — Strong on its narrow ID domain + integrations. Limited outside that.

### ⚠️ Critical caveat

DP's score of ~87% is **theoretical** — based on designed architecture and source code. The DX and documentation dimensions are visibly low (50%). Until:
1. Benchmark results are published
2. Documentation is fully completed
3. Community adoption exists

...the **perceived** score is much lower than the technical score. A developer evaluating these libraries in 2026 would likely rank DP below Vogen and Thinktecture due to maturity signals.

---

## Feature Parity Analysis

### Core Feature Parity (vs best-in-class per feature)

| Dimension | DP vs VOG | DP vs THK | DP vs STI | DP Position |
|:---|:---:|:---:|:---:|:---|
| Core Primitive Model | +7% | -8% | +22% | 2nd (after THK) |
| Validation | +20% | +15% | +70% | 1st |
| Normalization | +100% | +100% | +100% | 1st (unique) |
| Parsing & BCL | +50% | +45% | +40% | 1st (by far) |
| Formatting | +60% | +40% | +50% | 1st (IUtf8SpanFormattable unique) |
| Source Generation | +8% | +8% | +18% | 1st (equal or better) |
| AOT / Trimming | +10% | +10% | +10% | 1st (CI gate unique) |
| STJ Serialization | +7% | +7% | +7% | 1st (ValueSpan unique) |
| Newtonsoft | +100% (parity) | neutral | +100% (parity) | **Parity** (NOW-002 shipped) |
| EF Core | +10% | +10% | +10% | 1st (auto-discovery unique) |
| Dapper | +10% | +100% | +10% | 1st (auto-registration unique) |
| ASP.NET Core | +10% | +5% | +10% | 1st |
| Mapping | +100% | +100% | +100% | 1st (Mapster unique) |
| Smart Enum | N/A vs VOG | -5% vs THK | N/A vs STI | **1st** (Match/Map/Switch exhaustive, case-insensitive, no DU) |
| Strong ID | +0% | +5% | +5% | 1st (parity except custom types) |
| Developer Experience | -35% | -25% | -35% | LAST |
| Documentation | -30% | -25% | -30% | LAST |

### Overall Weighted Parity

| Library | Overall Score | DX-Adjusted | "Would Choose" Score |
|:---|:---:|:---:|:---:|
| **DP** | ~87% | ~62% | **Conditional** |
| **Thinktecture** | ~77% | ~72% | **Yes** (mature) |
| **Vogen** | ~73% | ~71% | **Yes** (best DX) |
| **StronglyTypedId** | ~67% | ~65% | **Yes** (for ID-only use) |

> "DX-Adjusted" = applying a 30% penalty for immature documentation and community.

---

## Most Dangerous Competitor by Scenario

| Scenario | Most Dangerous Competitor | Why |
|:---|:---|:---|
| DDD-focused team | **Thinktecture** | Discriminated Unions + full DDD spectrum |
| Performance-obsessed team | **Vogen** | Best community trust + proven benchmarks |
| ID-only need | **StronglyTypedId** | Simpler, proven, no overhead |
| Smart Enum only | **Ardalis.SmartEnum** | Ecosystem maturity; SmartFlagEnum |
| BCL interface story | **SuperStrong.Types** | Direct IUtf8SpanParsable competitor (emerging) |
