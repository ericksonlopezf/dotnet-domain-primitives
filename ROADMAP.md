# Roadmap — EricksonLopez.DomainPrimitives

> **Version:** 1.0 (post-audit)  
> **Last Updated:** 2026-08-24  
> **Horizon:** 12 months (NOW / NEXT / LATER)  
> **Strategy:** Balanced — close credibility gaps first, then amplify differentiators.
>
> This roadmap is derived from the competitive audit and product strategy analysis.  
> All items are ordered by business impact, not by implementation ease.  
> Items in the LATER horizon require an RFC before implementation begins.

For the full roadmap with detailed rationale, pre-conditions, and strategic context, see **[docs/roadmap.md](docs/roadmap.md)**.

---

## Summary Table

| ID | Item | Horizon | Priority | Effort | ADR/RFC | Status |
|----|------|---------|---------|--------|---------|:---:|
| NOW-001 | Publish comparative benchmarks | NOW | 🔴 Must | Low | — | ✅ Done |
| NOW-002 | Newtonsoft.Json package | NOW | 🔴 Must | Low-Med | [adr-026](docs/adr/adr-026-newtonsoft-json-gap-plan.md) | ✅ Done |
| NOW-003 | Verify + promote migration guides | NOW | 🔴 Must | Low | — | ✅ Done |
| NEXT-001 | Global configuration (`DomainPrimitivesDefaults`) | NEXT | 🟠 Should | Med | [adr-033](docs/adr/adr-033-global-assembly-configuration.md) | ✅ Done |
| NEXT-002 | Configurable exception type & analyzer DP0017 | NEXT | 🟠 Should | Low | [adr-034](docs/adr/adr-034-configurable-exception-type.md) | ✅ Done |
| NEXT-003 | SmartEnum Switch/Map exhaustiveness | NEXT | 🟠 Should | Med | [adr-035](docs/adr/adr-035-smartenum-exhaustive-switch-map.md) | ✅ Done |
| NEXT-004 | Case-insensitive SmartEnum parsing | NEXT | 🟡 Could | Low | [adr-036](docs/adr/adr-036-smartenum-case-insensitive-parsing.md) | ✅ Done |
| NEXT-005 | Security story content & promotion | NEXT | 🟠 Should | Low | — | ✅ Done |
| LATER-001 | Discriminated Unions | LATER | 🟡 Could | High | adr-029, rfc-0007 | ⏳ Planned |
| LATER-002 | `INumber<T>` for NumericPrimitive | LATER | 🟡 Could | Med | RFC needed | ⏳ Planned |
| LATER-003 | Community infrastructure | LATER | 🟠 Should | Ongoing | — | ⏳ Planned |
| LATER-004 | .NET 10 explicit feature targeting | LATER | 🟡 Could | Low | — | ⏳ Planned |
| LATER-005 | SuperStrong.Types competitive review | LATER | 🟡 Could | Ongoing | — | ⏳ Planned |

---

## Horizon Definitions

| Horizon | Timeframe | Theme |
|---------|-----------|-------|
| **NOW** | 0–3 months | Credibility — make claims verifiable, close adoption blockers |
| **NEXT** | 3–6 months | Parity + Amplification — close DX gaps, strengthen differentiators |
| **LATER** | 6–12 months | Differentiation — expand TAM, build structural advantages |

---

## Governing Principles

1. **Credibility before features.** Unverified claims hurt more than missing features.
2. **Differentiators over parity.** Only add parities that are adoption blockers.
3. **AOT-first always.** No feature ships if it breaks the `PublishAot=true` CI gate.
4. **API surface discipline.** Every new member requires passing the Feature Gate test.
5. **RFC before implementation.** Any LATER item requires an accepted RFC before coding starts.

---

## Permanently Rejected Items

These items will never be implemented. See [`docs/rejected-features.md`](docs/rejected-features.md) for full justification and the corresponding ADRs.

| Item | ADR |
|------|-----|
| Class-based primitives | [adr-018](docs/adr/adr-018-reject-class-based-primitives.md) |
| Implicit conversions | [adr-019](docs/adr/adr-019-reject-implicit-conversions.md) |
| Async validation | [adr-020](docs/adr/adr-020-reject-async-validation.md) |
| Reflection-based GetAll() | [adr-021](docs/adr/adr-021-reject-reflection-getall.md) |
| Aggregate / Entity support | [adr-022](docs/adr/adr-022-reject-aggregate-entity-support.md) |
| XML serialization | [adr-023](docs/adr/adr-023-reject-xml-serialization.md) |
| Mutable primitives | [adr-024](docs/adr/adr-024-reject-mutable-primitives.md) |
| `Result<T>` as primary API | [adr-025](docs/adr/adr-025-reject-result-as-primary-api.md) |
| AutoMapper generated config | [adr-030](docs/adr/adr-030-reject-automapper-integration.md) |
| Per-property validation on ValueObject | [adr-031](docs/adr/adr-031-reject-per-property-validation-on-valueobject.md) |

---

*For detailed item descriptions, risk analysis, and pre-conditions, see [`docs/roadmap.md`](docs/roadmap.md).*  
*For the full feature gap list, see [`docs/feature-gaps.md`](docs/feature-gaps.md).*  
*For competitive positioning, see [`docs/positioning.md`](docs/positioning.md).*
