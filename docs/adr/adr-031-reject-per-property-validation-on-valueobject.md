# adr-031: Reject Per-Property Validation Attributes on ValueObject

**Date:** 2026-08-10
**Status:** Accepted
**Authors:** Core maintainers
**Related audit items:** REJECT-010 (feature-gaps.md)

---

## Context

Users familiar with `System.ComponentModel.DataAnnotations` sometimes request the ability to
place validation attributes directly on `[ValueObject]` properties:

```csharp
// Proposed (rejected) pattern:
[ValueObject]
public readonly partial record struct Address(
    [MaxLength(100)] string Street,
    [Length(2, 2)] string CountryCode,
    [Range(1, 99999)] int PostalCode
);
```

The `EricksonLopez.DomainPrimitives` `[ValueObject]` generator currently validates the composite
type as a whole via the user's partial `Validate()` method, or via individual property types
(where each property is itself a domain primitive with its own validation).

---

## Decision

**`EricksonLopez.DomainPrimitives` will not support per-property validation attributes on
`[ValueObject]` declarations.**

---

## Rationale

### 1. Validation semantics are ambiguous

If `[MaxLength(100)]` appears on a `string Street` property inside a `[ValueObject]`:

- Does validation run when the `Address` is constructed?
- Or only when explicitly invoked?
- What is the error model — is it the `PrimitiveError` struct or `DataAnnotationsValidationResult`?
- Who aggregates errors from multiple properties?
- Does the library need to implement its own `Validator.TryValidateObject` equivalent?

These questions have no obvious answers without significant design work, and every answer
creates a new API surface that must be versioned and maintained.

### 2. It creates a second, inconsistent validation pipeline

`EricksonLopez.DomainPrimitives` already has a well-defined validation pipeline for scalar
primitives: `notEmpty → minLength → maxLength → regex → custom`. This pipeline runs at
construction time, is source-generated, and produces `PrimitiveError` structs.

`DataAnnotations` attributes run at a different time (typically in model binding or explicit
`Validator.TryValidateObject` calls) and produce `ValidationResult` objects (heap-allocated,
not struct-based).

Mixing both pipelines in the same type creates:
- Confusion about which validation runs when.
- Inconsistent error models (`PrimitiveError` vs `ValidationResult`).
- Potential for the same field to be validated twice with different rules.

### 3. The correct pattern is to use domain primitives as properties

The idiomatic DomainPrimitives approach for a validated composite type is:

```csharp
// ✅ Correct: each property is itself a validated domain primitive
[StringPrimitive(MaxLength = 100)]
public readonly partial record struct Street;

[CountryCode]  // Implies Length(2, 2) + UpperCase + Trim
public readonly partial record struct CountryIsoCode;

[ValueObject]
public readonly partial record struct Address(
    Street Street,
    CountryIsoCode Country,
    PostalCode PostalCode  // [NumericPrimitive<int>] with Range validation
);
```

In this pattern:
- Each property is validated when its primitive is created.
- An `Address` cannot be created with invalid properties — the type system enforces it.
- No additional annotation layer is needed.
- The validation pipeline is consistent across all types.

### 4. Cross-property validation belongs in the partial `Validate()` method

If validation requires knowledge of multiple properties together (e.g., "Street cannot be
empty if CountryCode is non-EU"), that logic belongs in the user's partial `Validate()` method:

```csharp
[ValueObject]
public readonly partial record struct Address(Street Street, CountryIsoCode Country)
{
    private static partial PrimitiveError? Validate(Address value)
    {
        if (value.Country == CountryIsoCode.Create("US") && value.Street == default)
            return new PrimitiveError("ADDRESS_INCOMPLETE", "US addresses require a street.");
        return null;
    }
}
```

This pattern is explicit, testable, and does not require the generator to parse attribute
combinations.

---

## Alternatives Considered

| Alternative | Rejected because |
|-------------|-----------------|
| Generate validation from `DataAnnotations` attributes on properties | Mixed semantics, dual error model, ambiguous timing. |
| Generate a `Validate()` method that calls `Validator.TryValidateObject` | `Validator.TryValidateObject` uses reflection — breaks AOT. |
| Custom `[PropertyValidation]` attribute that wraps DataAnnotations in source gen | Doubles the attribute surface. Users would still need to learn a new attribute system. |

---

## Consequences

- **Positive:** Validation pipeline remains consistent — one model, one error type.
- **Positive:** AOT compatibility is not compromised.
- **Positive:** Generator surface does not expand to handle property-level attribute parsing.
- **Negative:** Users from DataAnnotations backgrounds may find the "primitive as property
  type" pattern unfamiliar. Migration is documented in the cookbook.
- **Documentation action:** Cookbook entry explaining the correct pattern. Documented in
  `docs/rejected-features.md` with the `ValueObject` + typed properties example.
