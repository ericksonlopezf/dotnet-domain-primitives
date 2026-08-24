# Product Roadmap — EricksonLopez.DomainPrimitives

> **Version:** 1.0 (post-audit)
> **Last Updated:** 2026-08-24
> **Horizon:** 12 months (NOW / NEXT / LATER)
> **Strategy:** Balanced — close credibility gaps first, then amplify differentiators.
>
> This roadmap is derived from the 2026-08-10 competitive audit and product strategy analysis.
> All items are ordered by business impact, not by implementation ease.
> Items in the LATER horizon require an RFC before implementation begins.

---

## Governing Principles

1. **Credibility before features.** Unverified claims hurt more than missing features.
2. **Differentiators over parity.** Only add parities that are adoption blockers.
3. **AOT-first always.** No feature ships if it breaks the `PublishAot=true` CI gate.
4. **API surface discipline.** Every new member requires passing the Feature Gate test.
5. **RFC before implementation.** Any LATER item requires an accepted RFC before coding starts.

---

## Horizon Definitions

| Horizon | Timeframe | Theme |
|---------|-----------|-------|
| **NOW** | 0–3 months | Credibility — make claims verifiable, close adoption blockers |
| **NEXT** | 3–6 months | Parity + Amplification — close DX gaps, strengthen differentiators |
| **LATER** | 6–12 months | Differentiation — expand TAM, build structural advantages |

---

## 🔴 NOW — 0–3 Months

### NOW-001: Publish Comparative Benchmark Suite

**Problem solved:** All performance claims are unverified. A developer evaluating the library
cannot confirm "allocation-minimized hot paths" without data. This makes every performance
claim indistinguishable from marketing.

**Feature/Initiative:**
- Execute `ComparativeBenchmarks.cs` (41 benchmarks already written in `benchmarks/`).
- Produce full results across: `Create`, `TryCreate`, `TryParse(string)`,
  `TryParse(ReadOnlySpan<char>)`, `TryParse(ReadOnlySpan<byte>)`, JSON serialize/deserialize,
  EF Core convert, Dapper SetValue/Parse, SmartEnum lookup.
- Compare against Vogen, StronglyTypedId, and raw types (baseline).
- Publish in `benchmarks/results/` + embed summary table in README.
- Update `docs/benchmark-results.md` with full results.

**Strategic reason:** Unlocks all performance differentiators. Without this, claims of
"allocation-minimized" and "BCL-native" are unverifiable assertions.

**Dependencies:** None. `ComparativeBenchmarks.cs` exists.

**Risks:** If results show DP is slower than Vogen in any path, the honest approach is to
document the trade-off (e.g., NFC normalization adds 1 alloc = security cost) rather than
suppress the result.

**Success metric:** Benchmark table in README. Zero "show me the benchmarks" issues.

**Status:** ✅ **Implemented** — `ComparativeBenchmarks.cs` exports published in `benchmarks/results/` and summary table in README.

**ADR:** N/A — this is an execution item, not an architectural decision.

---

### NOW-002: Newtonsoft.Json Package

**Problem solved:** DP is excluded from evaluation in any enterprise .NET project with a
Newtonsoft.Json dependency. Both Vogen and StronglyTypedId have this package. It is the only
integration gap where two competitors simultaneously outperform DP.

**Feature/Initiative:**
- Create `EricksonLopez.DomainPrimitives.NewtonsoftJson` NuGet package.
- Extend source generator to emit `Newtonsoft.Json.JsonConverter<T>` for all generated types
  when the package is referenced.
- Implement auto-registration via `AddDomainPrimitivesNewtonsoft()`.
- Add roundtrip integration tests.
- Update README competition table.

**Strategic reason:** Adoption blocker removal. Expands the addressable market from
"greenfield .NET 8+ projects only" to "any .NET 8+ project."

**Dependencies:** `EricksonLopez.DomainPrimitives.Core` (no changes needed there).

**Risks:** Newtonsoft.Json is not AOT-compatible. The package must carry `[RequiresDynamicCode]`
and be excluded from the AOT CI gate. This is the correct behavior — Newtonsoft.Json users are
by definition not on AOT paths.

**Success metric:** Zero "does it support Newtonsoft?" issues after release. Package appears in
competitive matrix as 🟢.

**Status:** ✅ **Implemented** in `EricksonLopez.DomainPrimitives.NewtonsoftJson`.

**ADR:** [adr-026](adr/adr-026-newtonsoft-json-gap-plan.md)

---

### NOW-003: Verify and Promote Migration Guides

**Problem solved:** Developers evaluating a switch from Vogen or StronglyTypedId have no clear
path. Migration friction = no switch.

**Feature/Initiative:**
- Audit completeness of `docs/migration/from-vogen.md` and `docs/migration/from-stronglytypedid.md`.
- Ensure both include: attribute equivalence table, API differences table, step-by-step walkthrough,
  and known behavioral differences.
- Add prominent links in README (already referenced — verify link works and content is complete).
- Add migration link in the comparison table footnote.

**Strategic reason:** Migration guides are the conversion funnel for the largest existing pool
of potential adopters (current Vogen/STI users).

**Dependencies:** None.

**Risks:** None significant.

**Success metric:** Issues asking "how do I migrate?" drop to zero. Migration guide pages have
measurable traffic.

---

## 🟠 NEXT — 3–6 Months

### NEXT-001: Global Configuration (`[assembly: DomainPrimitivesDefaults]`)

**Problem solved:** In projects with 20+ domain primitives, repeating `Trim = true, MaxLength = 256`
on every attribute is tedious and error-prone. Vogen has `VogenDefaults`; Thinktecture has global config.

**Feature/Initiative:**
- Add `[assembly: DomainPrimitivesDefaults(Trim = true, NotEmpty = true, MaxLength = 256)]`
  assembly attribute.
- Extend all generators to read assembly-level defaults when per-type attribute is not specified.
- Per-type attribute takes precedence over assembly defaults.
- Add tests for default inheritance and override behavior.

**Strategic reason:** DX parity with Vogen. Reduces friction for large projects.

**Dependencies:** None architectural. Requires generator update for all 6 generators.

**Risks:** Assembly attribute discovery in the incremental generator pipeline must be tested
carefully — assembly attributes are not part of the SyntaxProvider predicate chain.

**Success metric:** Feature adopted in sample projects. Issues requesting this feature stop.

**Status:** ✅ **Implemented** via `[assembly: DomainPrimitivesDefaults]`.

**ADR:** [adr-033](adr/adr-033-global-assembly-configuration.md)

---

### NEXT-002: Configurable Exception Type (`DomainPrimitivesDefaults.ExceptionType`)

**Problem solved:** Enterprise projects use custom exception hierarchies (e.g., `DomainException`
inheriting from a base class). The generated `Create()` method currently always throws
`DomainPrimitiveValidationException`. Teams must wrap every `Create()` call.

**Feature/Initiative:**
- Extend `[assembly: DomainPrimitivesDefaults]` with `ExceptionType = typeof(MyDomainException)`.
- Generator emits `throw new {ExceptionType}(error.Message)` instead of the default exception.
- Custom exception must inherit from `Exception` (enforced by analyzer DP0017).

**Strategic reason:** DX parity with Vogen (configurable exception type). Removes a common
wrapper boilerplate pattern in enterprise projects.

**Dependencies:** NEXT-001 (DomainPrimitivesDefaults attribute).

**Risks:** Low. Generator change is localized to the exception throw site.

**Success metric:** Teams with custom exception hierarchies can adopt DP without wrapper code.

**Status:** ✅ **Implemented** via `ExceptionType` and analyzer **DP0017**.

**ADR:** [adr-034](adr/adr-034-configurable-exception-type.md)

---

### NEXT-003: Smart Enum Switch/Map Exhaustiveness

**Problem solved:** Thinktecture is the only library with exhaustive `Switch<TResult>` and
`Map<TResult>` for SmartEnums — a compile-time check that all cases are handled. DP's SmartEnum
is missing this feature.

**Feature/Initiative:**
- For `[SmartEnum<T>]`, generate:
  ```csharp
  public TResult Switch<TResult>(
      Func<OrderStatus, TResult> onPending,
      Func<OrderStatus, TResult> onShipped,
      Func<OrderStatus, TResult> onDelivered
  ) => this == Pending ? onPending(this)
     : this == Shipped ? onShipped(this)
     : onDelivered(this);
  ```
- Generates one parameter per declared member — compiler error if a case is missing.
- Also generate void `Switch(Action<OrderStatus> onPending, ...)` variant.

**Strategic reason:** Closes the most visible Smart Enum gap vs Thinktecture. Reinforces
DP's position as a complete Smart Enum generator.

**Dependencies:** None. SmartEnumGenerator.cs is self-contained.

**Risks:** Generated method signature grows with the number of enum members. Budget gate
must be re-evaluated for SmartEnums with >10 members.

**Success metric:** DP SmartEnum comparison table shows 🟢 for Switch/Map exhaustiveness.

**Status:** ✅ **Implemented** in `SmartEnumGenerator.cs`.

**ADR:** [adr-035](adr/adr-035-smartenum-exhaustive-switch-map.md)

---

### NEXT-004: Case-Insensitive SmartEnum Parsing

**Problem solved:** HTTP/JSON payloads often use lowercase or inconsistent casing for enum
values. `TryFromName("pending")` fails when the member is `Pending`.

**Feature/Initiative:**
- Generate `TryFromName(string name, bool ignoreCase, out T result)` overload.
- Uses `StringComparison.OrdinalIgnoreCase` when `ignoreCase = true`.

**Strategic reason:** Minor DX improvement. Low effort, high frequency request.

**Dependencies:** None.

**Risks:** None significant.

**Success metric:** Feature adopted without follow-up issues.

**Status:** ✅ **Implemented** in `SmartEnumGenerator.cs`.

**ADR:** [adr-036](adr/adr-036-smartenum-case-insensitive-parsing.md)

---

### NEXT-005: Security Story — Content & Documentation

**Problem solved:** The Security Gates (SEC-001..006) are the library's most unique and defensible
differentiator, but they are invisible to developers who haven't read the docs. No competitor has
these protections. The story is not told.

**Feature/Initiative:**
- Publish `docs/security.md` as a prominent top-level doc (already exists — promote it).
- Write and publish: "How DomainPrimitives prevents ReDoS attacks by default."
- Write and publish: "Unicode homoglyph attacks and why NFC normalization matters."
- Add "Security Posture" row to the README comparison table.
- Submit to OWASP .NET security resources page.

**Strategic reason:** Content marketing for the Security-by-Default pillar. This is the most
unique claim — zero competitors have it. Content drives organic discovery.

**Dependencies:** None code-related.

**Risks:** None.

**Success metric:** DP cited in .NET security discussions online. "Security" appears in user
testimonials and migration motivations.

**Status:** ✅ **Implemented** via `docs/security.md` and feature comparison matrix in README.

---

## 🟡 LATER — 6–12 Months

> All LATER items require an accepted RFC before implementation begins.
> Pre-conditions for starting each item are listed explicitly.

---

### LATER-001: Discriminated Unions

**Problem solved:** Teams practicing strict DDD need to model domain states as "one of" — an
`OrderState` that is exactly one of `(Pending | Confirmed | Shipped | Cancelled)` with
compiler-enforced exhaustiveness. Only Thinktecture supports this today.

**Feature/Initiative:** `[DiscriminatedUnion]` attribute generating `Switch<TResult>`, `Map<TResult>`,
STJ JSON converter, and EF Core owned entity mapping.

**Pre-conditions (must ALL be met before starting):**
1. NuGet downloads ≥ 1,000/month for 3 consecutive months.
2. GAP-002 (Newtonsoft.Json) shipped.
3. GAP-009 (public benchmarks) published.
4. rfc-0007 filed and accepted.
5. 3+ real user requests with documented use cases in GitHub Issues.

**ADR:** [adr-029](adr/adr-029-defer-discriminated-unions.md)

**Estimated effort:** 40–80 hours.

---

### LATER-002: INumber\<T\> for NumericPrimitive (opt-in)

**Problem solved:** Generic math algorithms (`Min`, `Max`, `Abs`, arithmetic) cannot be applied
to domain numeric types without explicit casting to the backing type. `INumber<T>` support would
allow domain types to participate in generic math contexts.

**Feature/Initiative:** `[NumericPrimitive<T>(EnableGenericMath = true)]` generates `INumber<T>`
implementation with non-user-facing members hidden via `[EditorBrowsable(Never)]`.

**Pre-conditions:**
1. API surface budget impact measured and approved via RFC.
2. RFC filed specifying exactly which `INumber<T>` members are user-facing vs hidden.
3. No impact on non-opt-in types.

**ADR:** Linked to GAP-004 and planning-risks.md R07.

**Estimated effort:** 16–24 hours.

---

### LATER-003: Community Infrastructure

**Problem solved:** "No community" is a primary reason developers choose Vogen over DP in
evaluations where technical criteria are equal.

**Feature/Initiative:**
- Enable GitHub Discussions.
- Publish GitHub Pages documentation site.
- Blog post series: "Why IUtf8SpanParsable\<T\> matters", "Security gates in domain primitives",
  "Auto-discovery: the missing piece in DDD persistence."
- NuGet download tracking + monthly download badge in README.
- Conference talk submission (dotnetconf, NDC, local .NET user groups).

**Pre-conditions:**
1. Benchmarks published (NOW-001) — need credibility before community building.
2. Newtonsoft.Json shipped (NOW-002) — need complete product before promoting.

**Estimated effort:** 4–8 hours/month ongoing.

---

### LATER-004: net10.0 Explicit Feature Targeting

**Problem solved:** NET 10 introduces new BCL APIs that may improve performance or allow
simplification of generated code. Currently the library targets `net8.0;net9.0;net10.0` but
does not use NET10-specific APIs.

**Feature/Initiative:** Evaluate and implement NET10-specific improvements:
- `System.Text.Unicode` improvements.
- `ReadOnlySpan<T>` API additions.
- Potential reduction in conditional compilation blocks.

**Pre-conditions:** NET 10 reaches GA and LTS designation.

**Estimated effort:** 4–8 hours.

---

### LATER-005: SuperStrong.Types Competitive Analysis Update

**Problem solved:** SuperStrong.Types is the only emerging library explicitly targeting
`IUtf8SpanParsable<T>` and related BCL interfaces — the same differentiation story as DP.
If it matures, DP's BCL-Native pillar becomes contested.

**Feature/Initiative:**
- Quarterly competitive review of SuperStrong.Types feature completeness.
- Update `docs/competitive-analysis.md` with evidence-based comparison.
- If SuperStrong.Types achieves feature parity in BCL interfaces, identify next differentiation.

**Pre-conditions:** Recurring — every 3 months.

---

## Backlog — Not Scheduled

Items that are valid requests but do not have sufficient priority for scheduling:

| Item | Reason not scheduled | Re-evaluate when |
|------|---------------------|------------------|
| `[TrimStart]` / `[TrimEnd]` attributes | Low impact — `[Trim]` covers 95% of cases | Community requests ≥ 5 |
| `[ExactLength(n)]` attribute | Covered by semantic shortcuts for common cases | Community requests ≥ 5 |
| Mapperly generated config | AutoMapper (rejected) + Mapperly explicit operators work | Community requests ≥ 10 |
| Error message localization | No current request; complex design | Community requests ≥ 20 |
| Sequential Guid generation | Application-level concern, not domain primitive | Never — scope creep |
| `SmartFlagEnum` (bit flags) | Low demand; complex generator; Ardalis.SmartEnum has it | Community requests ≥ 10 |

---

## Rejected Items (Permanent)

These items will never be implemented. See `docs/rejected-features.md` for full justification.

| Item | ADR |
|------|-----|
| Class-based primitives | [adr-018](adr/adr-018-reject-class-based-primitives.md) |
| Implicit conversions from primitive | [adr-019](adr/adr-019-reject-implicit-conversions.md) |
| Async validation / async factories | [adr-020](adr/adr-020-reject-async-validation.md) |
| Reflection-based GetAll() | [adr-021](adr/adr-021-reject-reflection-getall.md) |
| Aggregate / Entity support | [adr-022](adr/adr-022-reject-aggregate-entity-support.md) |
| XML serialization | [adr-023](adr/adr-023-reject-xml-serialization.md) |
| Mutable primitives | [adr-024](adr/adr-024-reject-mutable-primitives.md) |
| `Result<T>` as primary API | [adr-025](adr/adr-025-reject-result-as-primary-api.md) |
| AutoMapper generated config | [adr-030](adr/adr-030-reject-automapper-integration.md) |
| Per-property validation on ValueObject | [adr-031](adr/adr-031-reject-per-property-validation-on-valueobject.md) |

---

## Roadmap Summary Table

| ID | Item | Horizon | Priority | Effort | ADR/RFC | Status |
|----|------|---------|---------|--------|---------|--------|
| NOW-001 | Publish comparative benchmarks | NOW | 🔴 Must | Low | — | ✅ Done |
| NOW-002 | Newtonsoft.Json package | NOW | 🔴 Must | Low-Med | [adr-026](adr/adr-026-newtonsoft-json-gap-plan.md) | ✅ Done |
| NOW-003 | Verify + promote migration guides | NOW | 🔴 Must | Low | — | ✅ Done |
| NEXT-001 | Global configuration (`DomainPrimitivesDefaults`) | NEXT | 🟠 Should | Med | [adr-033](adr/adr-033-global-assembly-configuration.md) | ✅ Done |
| NEXT-002 | Configurable exception type | NEXT | 🟠 Should | Low | [adr-034](adr/adr-034-configurable-exception-type.md) | ✅ Done |
| NEXT-003 | SmartEnum Switch/Map exhaustiveness | NEXT | 🟠 Should | Med | [adr-035](adr/adr-035-smartenum-exhaustive-switch-map.md) | ✅ Done |
| NEXT-004 | Case-insensitive SmartEnum parsing | NEXT | 🟡 Could | Low | [adr-036](adr/adr-036-smartenum-case-insensitive-parsing.md) | ✅ Done |
| NEXT-005 | Security story content | NEXT | 🟠 Should | Low | — | ✅ Done |
| LATER-001 | Discriminated Unions | LATER | 🟡 Could | High | adr-029, rfc-0007 | ⏳ Planned (v2.x) |
| LATER-002 | INumber\<T\> for NumericPrimitive | LATER | 🟡 Could | Med | RFC needed | ⏳ Planned |
| LATER-003 | Community infrastructure | LATER | 🟠 Should | Ongoing | — | ⏳ Planned |
| LATER-004 | NET 10 feature targeting | LATER | 🟡 Could | Low | — | ⏳ Planned |
| LATER-005 | SuperStrong.Types competitive review | LATER | 🟡 Could | Ongoing | — | ⏳ Planned |

---

*For the full feature gap list, see [`docs/feature-gaps.md`](feature-gaps.md).*
*For rejected features, see [`docs/rejected-features.md`](rejected-features.md).*
*For competitive positioning, see [`docs/positioning.md`](positioning.md).*
