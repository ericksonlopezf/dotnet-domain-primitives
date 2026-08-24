# Governance

> **Version:** 2.1  
> **Last Updated:** 2026-08-23  
> **Required by:** Engineering Specification v4.0 §11.3

---

## Core Committee

The Core Committee is responsible for reviewing RFCs, approving breaking changes, and
maintaining the quality and direction of the `EricksonLopez.DomainPrimitives` library.

| Member | Role | Contact |
|--------|------|---------|
| Erickson Lopez | Lead Maintainer | [@ericksonlopezf](https://github.com/ericksonlopezf) ([ericksonlopezf@gmail.com](mailto:ericksonlopezf@gmail.com)) |

> **Quorum requirement:** For a committee of 1, 1 approval is required for all decisions.
> As the committee grows, the voting rules below apply.

---

## Design Principles (Non-Negotiable)

These constraints are enforced by the engineering specification and may not be overridden by RFC:

1. **API Surface Budget ≤ 25 members per generated struct.** A type with more than 25 public
   members is rejected at the Roslyn analyzer level (DP0014).
2. **Zero reflection in hot paths.** All code in `Create()`, `TryCreate()`, `Parse()`, and
   `TryParse()` must be reflection-free and NativeAOT-compatible.
3. **Zero allocation in parsing and validation hot paths.** Verified by benchmarks and mutation tests.
4. **Strict BCL semantics.** `ArgumentNullException` with `paramName`, `FormatException` from
   `Parse()`, `ArgumentException` for range violations — no deviations.
5. **Scope:** Domain Primitives only. No ORMs, no aggregates, no repositories, no application-layer patterns.

---

## RFC Process

All changes that meet the criteria below **MUST** go through the RFC process before implementation begins.

### When an RFC is Required

| Change Type | RFC Required? | Examples |
|-------------|--------------|---------|
| New primitive category | ✅ Required | Adding `[NetworkPrimitive]`, `[CollectionPrimitive]` |
| New integration package | ✅ Required | Adding `MediatR`, `MassTransit` integration |
| Breaking source change | ✅ Required | Renaming public methods, changing method signatures |
| Breaking binary change | ✅ Required | Removing public members, changing types |
| Behavioral breaking change | ✅ Required | Changing validation defaults, error semantics |
| New generator capability | ✅ Required | Adding new attributes, changing generated code structure |
| Governance changes | ✅ Required | Changes to this document |
| Additive (non-breaking) API | ❌ Optional | Adding optional parameters, new overloads |
| Bugfix | ❌ Not Required | Fixing incorrect behavior that is clearly a bug |
| Documentation | ❌ Not Required | README, ADR, XML doc improvements |
| Tooling / infra | ❌ Not Required | CI scripts, benchmark changes, gitignore |

### Definition of "Trivial" (RFC bypass eligible)

A change is **trivial** if ALL of the following are true:
- Does not touch any public API surface (no changes to generated method signatures or names)
- Does not change validation semantics (no changes to error codes, messages, or conditions)
- Has ≤ 50 lines of net change in source files (excluding tests and docs)
- Has complete test coverage (mutation score ≥ 95%)
- Does not introduce new dependencies

### RFC Process Steps

1. **Open a GitHub Issue** with the label `rfc` and the prefix `[RFC] Title`.
2. **Write the RFC document** in `docs/rfcs/RFC-NNNN-<kebab-title>.md` using the template below.
3. **Request review** by posting in the issue thread.
4. **Wait for the Discussion Period**: minimum 48 hours, no maximum.
5. **Collect votes** (see Voting Rules below).
6. **Implement** only after RFC reaches `Approved` state.
7. **Reference RFC in commit message**: `feat: implement rfc-0005 — NetworkPrimitive support`.

### RFC Document Template

```markdown
# RFC-NNNN: Short Title

> **Status:** Draft | Under Review | Approved | Rejected | Withdrawn
> **Authors:** Your Name
> **Created:** YYYY-MM-DD

## Problem Statement
## Decision
## Migration Guide
## Breaking Change Classification
## Risks and Mitigations
## Votes
```

---

## Voting Rules

- **Approval** requires at least **1 +1 vote** from a core committee member (current single-maintainer model).
- As committee grows: **3 +1 votes** required, **1 -1 vote constitutes a veto**.
- A **veto** (`-1`) must be accompanied by:
  - A detailed technical justification (minimum 3 sentences)
  - A concrete alternative direction forward
  - Without these, the veto is invalid and is treated as abstention
- **Trivial changes** require **1 approval** only, without full RFC.
- **Response SLA:** Committee members must respond to RFC proposals within **48 hours** (business days).
  Failure to respond within 48h is counted as abstention, not veto.

---

## Breaking Change Policy

All breaking changes must follow this lifecycle:

| Change Level | Deprecation Period | Method |
|-------------|--------------------|--------|
| Source-breaking | 2 minor versions | `[Obsolete(error: false)]` → `[Obsolete(error: true)]` → delete |
| Binary-breaking | 2 minor versions | Keep old member with `[Obsolete]`, add new member |
| Behavioral-breaking | 1 minor version | Document in BREAKING_CHANGES.md, emit DP analyzer warning |

---

## Security Policy

Security vulnerabilities must be reported by emailing the lead maintainer directly (not as
a public GitHub issue). Critical security fixes bypass the RFC process and may be merged
immediately with a single approval.

---

## Roadmap and Milestones

Major milestones are tracked in [ROADMAP.md](ROADMAP.md). The RFC label `milestone:vX.Y.Z`
associates an RFC with its target milestone.

---

## Amendment History

| Date | Version | Change |
|------|---------|--------|
| 2026-08-23 | 2.1 | Fixed `roadmap.md` → `ROADMAP.md` reference (SCREAMING_CASE root convention). Documentation audit corrections. |
| 2026-08-10 | 2.0 | Expanded from stub. Added committee, voting SLA, objective trivial criteria, breaking change lifecycle |
| 2026-07-01 | 1.0 | Initial governance stub |
