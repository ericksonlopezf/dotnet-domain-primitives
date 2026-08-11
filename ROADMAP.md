# Roadmap — EricksonLopez.DomainPrimitives

> **Version:** 1.0 (post-audit)  
> **Last Updated:** 2026-08-10  
> **Horizon:** 12 months (NOW / NEXT / LATER)  
> **Strategy:** Balanced — close credibility gaps first, then amplify differentiators.
>
> This roadmap is derived from the 2026-08-10 competitive audit and product strategy analysis.  
> All items are ordered by business impact, not by implementation ease.  
> Items in the LATER horizon require an RFC before implementation begins.

For the full roadmap with detailed rationale, pre-conditions, and strategic context, see **[docs/ROADMAP.md](docs/ROADMAP.md)**.

---

## Summary Table

| ID | Item | Horizon | Priority | Effort | ADR/RFC |
|----|------|---------|---------|--------|---------|
| NOW-001 | Publish comparative benchmarks | NOW | 🔴 Must | Low | — |
| NOW-002 | Newtonsoft.Json package | NOW | 🔴 Must | Low-Med | ADR-026 |
| NOW-003 | Verify + promote migration guides | NOW | 🔴 Must | Low | — |
| NEXT-001 | Global configuration (`DomainPrimitivesDefaults`) | NEXT | 🟠 Should | Med | RFC needed |
| NEXT-002 | Configurable exception type | NEXT | 🟠 Should | Low | RFC needed |
| NEXT-003 | SmartEnum Switch/Map exhaustiveness | NEXT | 🟠 Should | Med | RFC needed |
| NEXT-004 | Case-insensitive SmartEnum parsing | NEXT | 🟡 Could | Low | — |
| NEXT-005 | Security story content & promotion | NEXT | 🟠 Should | Low | — |
| LATER-001 | Discriminated Unions | LATER | 🟡 Could | High | ADR-029, RFC-0007 |
| LATER-002 | `INumber<T>` for NumericPrimitive | LATER | 🟡 Could | Med | RFC needed |
| LATER-003 | Community infrastructure | LATER | 🟠 Should | Ongoing | — |
| LATER-004 | .NET 10 explicit feature targeting | LATER | 🟡 Could | Low | — |
| LATER-005 | SuperStrong.Types competitive review | LATER | 🟡 Could | Ongoing | — |

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

These items will never be implemented. See [`docs/REJECTED-FEATURES.md`](docs/REJECTED-FEATURES.md) for full justification and the corresponding ADRs.

| Item | ADR |
|------|-----|
| Class-based primitives | [ADR-018](docs/adr/ADR-018-reject-class-based-primitives.md) |
| Implicit conversions | [ADR-019](docs/adr/ADR-019-reject-implicit-conversions.md) |
| Async validation | [ADR-020](docs/adr/ADR-020-reject-async-validation.md) |
| Reflection-based GetAll() | [ADR-021](docs/adr/ADR-021-reject-reflection-getall.md) |
| Aggregate / Entity support | [ADR-022](docs/adr/ADR-022-reject-aggregate-entity-support.md) |
| XML serialization | [ADR-023](docs/adr/ADR-023-reject-xml-serialization.md) |
| Mutable primitives | [ADR-024](docs/adr/ADR-024-reject-mutable-primitives.md) |
| `Result<T>` as primary API | [ADR-025](docs/adr/ADR-025-reject-result-as-primary-api.md) |
| AutoMapper generated config | [ADR-030](docs/adr/ADR-030-reject-automapper-integration.md) |
| Per-property validation on ValueObject | [ADR-031](docs/adr/ADR-031-reject-per-property-validation-on-valueobject.md) |

---

*For detailed item descriptions, risk analysis, and pre-conditions, see [`docs/ROADMAP.md`](docs/ROADMAP.md).*  
*For the full feature gap list, see [`docs/feature-gaps.md`](docs/feature-gaps.md).*  
*For competitive positioning, see [`docs/positioning.md`](docs/positioning.md).*
