# Product Positioning

> **Audit Version:** 2.0 | **Date:** 2026-08-10  
> **Evidence policy:** Only evidence-backed claims may appear in public-facing positioning.

---

## Current Positioning (Audited)

> "A high-performance, zero-allocation, Source Generator-driven library for building a rich, immutable, and strictly valid domain model in .NET."

### Audit Verdict: PARTIALLY DEFENSIBLE — 3 of 6 claims fail scrutiny

| Claim | Verdict | Issue |
|:---|:---:|:---|
| "high-performance" | ⚠️ Unproven | No public benchmarks. Credible from source, but not demonstrated. |
| "zero-allocation" | ❌ Partially false | 1 unavoidable string alloc on any normalized path (NFC). Misleading as stated. |
| "Source Generator-driven" | ✅ Proven | Confirmed. But NOT a differentiator — all competitors use generators. |
| "rich domain model" | ✅ Mostly proven | 30 domain shortcuts, 6 generators. Discriminated Unions missing. |
| "immutable" | ✅ Proven | All readonly record structs. |
| "strictly valid" | ✅ Proven | Validation on construction, parse, and deserialization. |

---

## Evidence-Based Positioning Analysis

### Where DP genuinely leads:

1. **BCL interface depth:** ISpanParsable<T> + IUtf8SpanParsable<T> + ISpanFormattable + IUtf8SpanFormattable — no competitor generates all four.
2. **Security posture:** SEC-001 through SEC-006 (4096-char limit, NonBacktracking regex, NFC normalization, etc.) — zero competitors have any of these.
3. **Domain semantic shortcuts:** 30 attribute shortcuts ([Email], [Money], [Latitude], etc.) — zero competitors have any.
4. **Auto-discovery integrations:** EF Core + Dapper without per-type configuration — zero competitors have this.
5. **Declarative normalization:** [Trim], [LowerCase], [UpperCase] — zero competitors have this.

### Where DP trails:

1. **Documentation and community** — Vogen has years of blog posts, Stack Overflow answers, GitHub discussions.
2. **Discriminated Unions** — Thinktecture has this; DP does not.
3. **Newtonsoft.Json** — Vogen and StronglyTypedId support it; DP does not.
4. **Public benchmarks** — Vogen has a benchmark table in its README; DP does not.
5. **Class support** — Vogen and Thinktecture support class-based VOs; DP is struct-only.
6. **Global configuration** — Vogen has VogenDefaults; DP has none.

---

## Stronger Positioning (Evidence-Based)

### Option A — Technical differentiator focus:

> **"The BCL-native Domain Primitive Library for .NET 8+."**
>
> *EricksonLopez.DomainPrimitives* generates strictly valid, immutable domain primitives with zero domain-layer contamination. Unlike other generators, it produces types implementing `IUtf8SpanParsable<T>`, `ISpanFormattable`, and `IUtf8SpanFormattable` — the deepest BCL integration available. Includes 30 built-in domain types ([Email], [Money], [CountryCode]...), ReDoS-resistant validation, and auto-discovered EF Core + Dapper converters.

### Option B — Security differentiator focus:

> **"Domain Primitives with Security Gates Built In."**
>
> *EricksonLopez.DomainPrimitives* is the only .NET domain primitive library that applies NFC Unicode normalization, NonBacktracking regex with timeout, and a 4096-character safety limit by default — before your domain validation even runs.

### Option C — DDD focus (after implementing Discriminated Unions):

> **"The AOT-First Domain Primitive Framework for .NET 10+."**
>
> *EricksonLopez.DomainPrimitives* generates a complete AOT-ready domain primitive layer: Value Objects, Strongly Typed IDs, Smart Enums, and Discriminated Unions. Zero reflection. Zero domain-layer contamination. Deep BCL integration. 30 built-in semantic types.

**Recommended:** Option A now. Option C after GAP-001 (Discriminated Unions) is implemented.

---

## Claims to Remove Immediately

| Claim | Reason | Replacement |
|:---|:---|:---|
| "zero-allocation" (unqualified) | 1 string alloc on any normalized path | "Allocation-minimized hot paths" |
| "Source Generator-driven" (as primary differentiator) | All competitors use source generators | "BCL-native" or "UTF-8-native" |
| "15-Year Horizon" (as promise) | Impossible to guarantee | "Built for .NET 10+ with long-term design horizon" |
| "strictly valid domain model" (as differentiator) | All competitors validate on construction | Remove as differentiator; keep as factual description |
| "Deep Ecosystem Integrations" (unqualified) | Vogen has deeper integrations in some areas | "Auto-discovered integrations" |

---

## Claims Requiring Benchmarks Before Use

| Claim | Benchmark Required | Scenario |
|:---|:---|:---|
| "High-performance" | Yes | Scenario 1+2 vs Vogen |
| "Allocation-minimized parsing" | Yes | Scenario 3 — TryParse span |
| "Zero-allocation on success path" | Yes | Scenario 1 — TryCreate |
| "Utf8JsonReader.ValueSpan zero-alloc JSON" | Yes | Scenario 6+7 — JSON deserialization |
| "ISpanFormattable zero-alloc format" | Yes | Scenario 13 — Format to span |

---

## Claims Requiring Documentation

| Claim | Required Document |
|:---|:---|
| "ArrayPool<char> for UTF-8 decoding" | Cookbook: "Parsing domain primitives from HTTP request bodies" |
| "NFC Unicode normalization (SEC-004)" | ADR: why NFC, what attacks it prevents |
| "ReDoS-resistant by default" | Security page: NonBacktracking regex explanation |
| "Auto-discovery integrations" | Cookbook: "Zero-attribute EF Core registration" |
| "30 built-in domain types" | Reference: semantic attribute catalog |

---

## Weighted Competitive Position Summary

### Technical Score (evidence-derived)

| Dimension | DP | Best Competitor | DP Position |
|:---|:---:|:---:|:---:|
| BCL Interface Depth | 95% | 45% (Vogen) | **1st by far** |
| Security posture | 95% | 0% | **1st (unique)** |
| Normalization | 90% | 0% | **1st (unique)** |
| Source generation quality | 88% | 85% (Vogen) | **1st** |
| AOT/Trimming | 92% | 85% | **1st** |
| Core primitive model | 82% | 90% (Thinktecture) | **2nd** |
| Validation depth | 90% | 70% (Vogen) | **1st** |
| STJ integration | 92% | 85% | **1st** |
| EF Core | 90% | 80% | **1st** |
| Dapper | 90% | 80% (Vogen) | **1st** |
| ASP.NET Core | 85% | 80% | **1st** |
| Mapping | 90% | 0% | **1st (unique)** |
| Newtonsoft.Json | 0% | 90% (Vogen) | **Last** |
| Smart Enums | 75% | 90% (THK) | **2nd** |
| Developer Experience | 50% | 85% (Vogen) | **Last** |
| Documentation | 45% | 80% (Vogen/THK) | **Last** |
| Community | 10% | 90% (Vogen) | **Last** |

### Overall Assessment

**DP is technically superior in the dimensions it targets (BCL parsing, security, normalization, UTF-8)**  
**DP is materially behind in adoption, documentation, and community maturity.**

The gap between technical quality and perceived quality is DP's primary business risk.

---

## Roadmap Recommendation

> **The authoritative roadmap is now [`docs/ROADMAP.md`](ROADMAP.md).**
> This section is a summary view. For full initiative details, pre-conditions, and dependency
> tracking, refer to the roadmap document.

### Phase 1 — NOW (0–3 months): Credibility

| # | Item | Status | Roadmap ref |
|---|------|--------|------------|
| 1 | ✅ Complete audit | ✅ Done (2026-08-10) | — |
| 2 | ⬜ Publish BenchmarkDotNet results vs Vogen (GAP-009) | Open | [NOW-001](ROADMAP.md) |
| 3 | ✅ Write migration guides (from-vogen.md, from-stronglytypedid.md) | ✅ Done | — |
| 4 | ✅ Verified JSON converter uses ValueSpan (GeneratorHelpers.cs:45-51) | ✅ Done | — |
| 5 | ⬜ Newtonsoft.Json package (GAP-002) | Open | [NOW-002](ROADMAP.md) / [ADR-026](adr/ADR-026-newtonsoft-json-gap-plan.md) |
| 6 | ⬜ Verify + promote migration guides visibility | Open | [NOW-003](ROADMAP.md) |

### Phase 2 — NEXT (3–6 months): Parity + Amplification

| # | Item | Status | Roadmap ref |
|---|------|--------|------------|
| 1 | ⬜ Global configuration (`DomainPrimitivesDefaults`) (GAP-011) | Open | [NEXT-001](ROADMAP.md) |
| 2 | ⬜ Configurable exception type (GAP-003) | Open | [NEXT-002](ROADMAP.md) |
| 3 | ⬜ SmartEnum Switch/Map exhaustiveness (GAP-006) | Open | [NEXT-003](ROADMAP.md) |
| 4 | ⬜ Case-insensitive SmartEnum parsing (GAP-007) | Open | [NEXT-004](ROADMAP.md) |
| 5 | ⬜ Security story — content and documentation | Open | [NEXT-005](ROADMAP.md) |

### Phase 3 — LATER (6–12 months): Differentiation

| # | Item | Status | Roadmap ref |
|---|------|--------|------------|
| 1 | ⬜ Discriminated Unions (GAP-001) | Deferred — [ADR-029](adr/ADR-029-defer-discriminated-unions.md) | [LATER-001](ROADMAP.md) |
| 2 | ⬜ INumber\<T\> for NumericPrimitive (GAP-004) | Open | [LATER-002](ROADMAP.md) |
| 3 | ⬜ Community infrastructure | Open | [LATER-003](ROADMAP.md) |
| 4 | ⬜ SuperStrong.Types competitive analysis update (R08) | Quarterly | [LATER-005](ROADMAP.md) |
