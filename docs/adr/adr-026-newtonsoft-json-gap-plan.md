# adr-026: Newtonsoft.Json Support — Gap Accepted, Package Planned

**Date:** 2026-08-10
**Status:** Accepted — Implementation Planned (v1.x)
**Authors:** Core maintainers
**Related audit items:** GAP-002 (feature-gaps.md)

---

## Context

`EricksonLopez.DomainPrimitives` currently generates only `System.Text.Json` converters.

The competitive analysis (2026-08-10) classified this as GAP-002 (Priority P1 — Important) and as the only
serialization gap where **both** Vogen AND StronglyTypedId outperform DomainPrimitives
simultaneously.

The competitive analysis notes:
> "This is the only integration gap where both Vogen AND StronglyTypedId outperform DP.
> Not having it removes DP from consideration in enterprise projects with Newtonsoft
> dependencies."

---

## Problem Statement

Enterprise .NET codebases that predate System.Text.Json (pre-.NET Core 3.0 or projects using
older NuGet packages that still take a Newtonsoft.Json dependency) cannot use
`EricksonLopez.DomainPrimitives` for their serialized types without writing manual converters.

The `Newtonsoft.Json.JsonConverter<T>` pattern is well-understood and low-effort to generate:

```csharp
// Generated converter structure (Newtonsoft.Json):
internal sealed class EmailAddressNewtonsoftJsonConverter : JsonConverter<EmailAddress>
{
    public override EmailAddress ReadJson(
        JsonReader reader, Type objectType,
        EmailAddress existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        var raw = reader.Value as string
            ?? throw new JsonSerializationException("Expected string.");
        return EmailAddress.Create(raw);
    }

    public override void WriteJson(
        JsonWriter writer, EmailAddress value, JsonSerializer serializer)
        => writer.WriteValue(value.Value);
}
```

---

## Decision

**A new optional NuGet package `EricksonLopez.DomainPrimitives.NewtonsoftJson` will be
created.**

The source generator will be extended to emit `Newtonsoft.Json.JsonConverter<T>` for each
generated primitive type when the package is referenced.

### Design constraints

1. **Optional dependency:** Newtonsoft.Json must remain an optional, separate-package dependency.
   The core package must not reference Newtonsoft.Json.

2. **Auto-registration:** Mirrors the EFCore/Dapper auto-discovery pattern — converters are
   registered once without per-type annotation.

3. **AOT note:** Newtonsoft.Json itself is **not AOT-compatible** due to its use of
   `Reflection.Emit`. The `EricksonLopez.DomainPrimitives.NewtonsoftJson` package will carry
   an explicit `[RequiresDynamicCode]` annotation and will be excluded from the AOT CI gate.
   This is acceptable because Newtonsoft.Json users are already not on AOT paths.

4. **Implementation effort:** Estimated 4–8 hours of generator work (mirroring the STJ
   generator, replacing `Utf8JsonReader`/`Utf8JsonWriter` with `JsonReader`/`JsonWriter`).

---

## Implementation Plan

| Step | Work item | Estimated effort |
|------|-----------|-----------------|
| 1 | Create `src/EricksonLopez.DomainPrimitives.NewtonsoftJson/` project | 1h |
| 2 | Extend generator to emit `JsonConverter<T>` for Newtonsoft when package is referenced | 4h |
| 3 | Add auto-registration via `AddDomainPrimitivesNewtonsoft()` extension | 1h |
| 4 | Add integration tests (serialize/deserialize roundtrip per type) | 2h |
| 5 | Update README and competitive matrix | 0.5h |

**Target version:** v1.x (post v1.2.0 release cycle)

---

## Consequences

- **Positive:** Removes the primary adoption blocker for enterprise .NET projects with
  Newtonsoft.Json dependencies.
- **Positive:** Competitive parity with Vogen and StronglyTypedId on serialization.
- **Positive:** Core package remains clean — no Newtonsoft.Json transitive dependency.
- **Negative:** Newtonsoft.Json package cannot be AOT-compatible (inherent limitation of
  Newtonsoft.Json itself, not of this library).
- **Negative:** Additional maintenance surface: one more package to update when Newtonsoft.Json
  releases new versions.
- **Documentation action:** Gap is acknowledged in README `> Known gap:` callout (already
  present). This ADR is the formal record of the plan.
