# ADR-030: Reject AutoMapper Generated Configuration

**Date:** 2026-08-10
**Status:** Accepted
**Authors:** Core maintainers
**Related audit items:** REJECT-009 (feature-gaps.md)

---

## Context

AutoMapper is the most widely-used object-to-object mapping library in the .NET ecosystem.
Users mapping domain primitives to DTOs occasionally request that `EricksonLopez.DomainPrimitives`
generate AutoMapper type map configurations.

The library already provides:
- **Mapster** source-generated integration (`EricksonLopez.DomainPrimitives.Mapster` — unique,
  no competitor has this).
- Implicit workaround via explicit conversion operators for most mapping scenarios.

---

## Decision

**`EricksonLopez.DomainPrimitives` will not add AutoMapper generated configuration.**

---

## Rationale

### 1. AutoMapper is reflection-based and incompatible with Native AOT

AutoMapper uses `Expression<Func<T, TResult>>` compilation and `Reflection.Emit` internally
for its mapping engine. This is fundamentally incompatible with `PublishAot=true`:

- `Expression.Compile()` requires `RequiresDynamicCode`.
- Type mapping discovery via reflection triggers `IL2072` trimmer warnings.
- AutoMapper's `IMapper` interface relies on `Activator.CreateInstance` for DTO creation.

Adding AutoMapper support would require the library to either:
- Emit `[RequiresDynamicCode]` annotations, explicitly marking the feature as non-AOT-safe.
- Or document a two-class system (AOT-safe mappings via Mapster, non-AOT via AutoMapper).

Neither option is acceptable given the library's AOT-first commitment.

### 2. The problem is already solved by a superior alternative

`EricksonLopez.DomainPrimitives.Mapster` provides source-generated, AOT-safe mapping for
composite `[ValueObject]` types. Mapster's source generation mode produces the equivalent of
what AutoMapper would configure at runtime, at compile time.

For scalar primitives (`[StringPrimitive]`, `[StrongId]`, `[NumericPrimitive<T>]`), the
generated `explicit operator` is sufficient for Mapster, Mapperly, and — in most scenarios —
for AutoMapper itself without any generator support.

### 3. AutoMapper's own trajectory is toward source generation

AutoMapper v13.0+ introduced source generation support as an experimental feature. If AutoMapper
moves to full source generation, the scenario where `EricksonLopez.DomainPrimitives` would need
to generate AutoMapper config becomes moot — Roslyn would handle both generators without
coordination.

### 4. Maintenance cost is disproportionate

Supporting AutoMapper configuration would require:
- Tracking AutoMapper's configuration API (which has had breaking changes across major versions).
- Testing against multiple AutoMapper versions.
- Handling the `IMapper`, `Profile`, `MapperConfiguration` API surface.

This duplicates the Mapster package maintenance without providing AOT compatibility.

---

## Alternatives Considered

| Alternative | Rejected because |
|-------------|-----------------|
| `EricksonLopez.DomainPrimitives.AutoMapper` optional package | Non-AOT-compatible. Contradicts the library's technical identity. |
| Generate `Profile` subclass with `CreateMap<T, TDto>()` | AutoMapper's Profile API changes between major versions. Also reflection-based. |
| Document a manual AutoMapper Profile pattern | Acceptable as documentation (cookbook). No generator needed. |

---

## Recommendation for Users

For users who need AutoMapper:

```csharp
// Manual AutoMapper profile — no generator needed for scalar primitives:
public class DomainPrimitivesProfile : Profile
{
    public DomainPrimitivesProfile()
    {
        // Explicit operator covers scalar primitives automatically:
        CreateMap<string, EmailAddress>().ConvertUsing(s => EmailAddress.Create(s));
        CreateMap<EmailAddress, string>().ConvertUsing(e => (string)e);
    }
}
```

For composite `[ValueObject]` types, use `EricksonLopez.DomainPrimitives.Mapster` with Mapster
instead of AutoMapper.

---

## Consequences

- **Positive:** AOT guarantee is preserved.
- **Positive:** No AutoMapper version coupling.
- **Negative:** Users who prefer AutoMapper must write a thin Profile (documented in cookbook).
- **Documentation action:** Documented in `docs/REJECTED-FEATURES.md` with the manual Profile
  pattern as the recommended alternative.
