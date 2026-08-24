# adr-019: Reject Implicit Conversions from Primitive Type

**Date:** 2026-08-10
**Status:** Accepted
**Authors:** Core maintainers
**Related audit items:** REJECT-007 (feature-gaps.md)

---

## Context

Several competing libraries (notably Vogen via the optional `Conversions.Implicit` flag) allow
generating an implicit conversion from the underlying primitive type to the domain primitive:

```csharp
// Pattern that some libraries support:
OrderId id = 42; // int implicitly converts to OrderId
```

The `EricksonLopez.DomainPrimitives` generators intentionally do not generate this operator.

The competitive analysis (2026-08-10) classified this as REJECT-007: "Reject permanently."

---

## Decision

**`EricksonLopez.DomainPrimitives` will never generate `implicit operator` from the backing
primitive type to the domain type.**

Explicit conversion in the opposite direction (`explicit operator string(EmailAddress e)`) is
generated and supported.

---

## Rationale

### 1. Implicit conversion defeats the purpose of a strongly typed ID

The primary use case for strongly typed IDs is to prevent callers from confusing parameters:

```csharp
void Process(OrderId orderId, CustomerId customerId);

// Without implicit conversion — this is a compile error (correct):
Process(customerId, orderId); // ❌ CS1503 — argument type mismatch

// With implicit conversion — this silently compiles (wrong):
int raw = 42;
Process(raw, raw); // ✅ compiles — but semantically wrong
```

An implicit conversion from `int` to `OrderId` would allow any `int` to be passed wherever an
`OrderId` is expected, eliminating the type-safety guarantee.

### 2. Validation is bypassed

A generated implicit conversion would need to either:
- **Always succeed** (bypass validation, creating an invalid primitive in a "valid" type).
- **Throw on failure** (which is semantically wrong for an implicit conversion — the C# spec
  expects implicit conversions to always succeed and be side-effect free).

Neither option is correct. `Create()` and `TryCreate()` exist precisely because construction
requires validation.

### 3. Breaks the "never unbox" contract

If `implicit operator` is present, the compiler will silently use it in generic contexts,
LINQ queries, and pattern matching — often in ways the developer does not expect. Debugging
"why did my `int` become an `OrderId` here?" is expensive.

### 4. The explicit conversion is the right design

`explicit operator string(EmailAddress e)` allows:
```csharp
string raw = (string)email; // explicit, visible, intentional
```

This is the correct direction — converting OUT of the domain type to its backing representation,
for infrastructure boundaries. It is always explicit, never silent.

---

## Alternatives Considered

| Alternative | Rejected because |
|-------------|-----------------|
| Generate implicit conversion with a compile-time warning | C# warnings for implicit conversions cannot be attached at the call site; the generator owns the operator definition, not the usage site. |
| Opt-in via `[StringPrimitive(AllowImplicit = true)]` | Providing the escape hatch is itself the bug. Adding the flag signals that the design has a "safe" mode and an "unsafe" mode — which erodes confidence. |
| Only allow implicit conversion for value types | The problem exists equally for value types (`int`, `Guid`). Struct layout similarity does not confer semantic equivalence. |

---

## Consequences

- **Positive:** Type safety invariant is guaranteed at the language level.
- **Positive:** No silent validation bypass.
- **Positive:** Code review can rely on the type system — any `(OrderId)42` is explicit and
  intentional.
- **Negative:** More verbose at infrastructure boundaries where raw types are needed (e.g.,
  passing to a legacy API). Mitigated by the generated `.Value` property.
- **Documentation action:** Documented in `docs/rejected-features.md`.
