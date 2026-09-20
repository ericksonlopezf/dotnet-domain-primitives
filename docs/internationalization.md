# Internationalization & Culture Invariance (i18n)

This document defines the internationalization, culture invariance, and Unicode normalization architecture implemented across `EricksonLopez.DomainPrimitives`.

---

## Design Principles

Domain primitives represent immutable business values that traverse serialization boundaries, storage engines, distributed network hops, and presentation tiers. Consequently, formatting and parsing must adhere to strict determinism:

1. **Deterministic Storage & Serialization:** Invariant culture (`CultureInfo.InvariantCulture`) is the default across all parse and format operations to guarantee round-trip fidelity regardless of server or client regional locale settings.
2. **Canonical Unicode Representation (SEC-004):** String inputs are normalized to **Unicode Normalization Form C (NFC)** prior to validation to neutralize visual spoofing, homoglyph attacks, and combining character variations.
3. **BCL First-Class Interface Alignment:** Primitives implement standard modern .NET formatting and parsing abstractions, enabling zero-overhead interop with string interpolation, span processing, and UTF-8 pipelines.

---

## Standard BCL Parsing & Formatting Interfaces

All generated primitives implement the standard modern .NET parsing and formatting hierarchy:

| Interface | Method Signature | Allocation | Purpose |
|:---|:---|:---:|:---|
| `IParsable<TSelf>` | `Parse(string, IFormatProvider?)` / `TryParse(...)` | 0 B (success) | Standard string parsing across .NET |
| `ISpanParsable<TSelf>` | `Parse(ReadOnlySpan<char>, IFormatProvider?)` / `TryParse(...)` | 0 B | High-throughput stack-allocated span parsing |
| `IUtf8SpanParsable<TSelf>` | `Parse(ReadOnlySpan<byte>, IFormatProvider?)` / `TryParse(...)` | 0 B (≤ 256 B) | Direct HTTP request body and network buffer parsing |
| `IFormattable` | `ToString(string?, IFormatProvider?)` | String alloc | Standard localized or custom formatting |
| `ISpanFormattable` | `TryFormat(Span<char>, out int, ReadOnlySpan<char>, IFormatProvider?)` | **0 B** | Zero-allocation buffer writing |
| `IUtf8SpanFormattable` | `TryFormat(Span<byte>, out int, ReadOnlySpan<char>, IFormatProvider?)` | **0 B** | Direct UTF-8 JSON and socket buffer writing |

---

## Culture Invariance by Default

When `IFormatProvider` is omitted or passed as `null`, generated methods explicitly default to `CultureInfo.InvariantCulture`:

```csharp
// Generated implementation pattern
public static TSelf Parse(string s, IFormatProvider? provider = null)
{
    provider ??= CultureInfo.InvariantCulture;
    // Parsing logic executes with guaranteed invariant semantics
}
```

### Why InvariantCulture is Mandatory for Primitives
- **Decimal Separators:** A monetary amount `100.50` must never parse as `10050` on European locales (where comma `,` is the decimal separator).
- **Date Representations:** Dates like `2026-08-24` or ISO 8601 timestamps must parse identically across US (`MM/dd/yyyy`) and international (`dd/MM/yyyy`) server runtimes.
- **Identifier Case Folding:** Strong identifiers and alphanumeric slugs use ordinal case comparisons (`StringComparison.OrdinalIgnoreCase`) rather than linguistic casing (avoiding Turkish "dotless i" anomalies).

---

## Unicode NFC Normalization (SEC-004)

In Unicode, visually identical characters can have distinct binary representations (e.g. `é` can be represented as precomposed `\u00E9` or decomposed `e + \u0301`).

To enforce consistent domain validation:
- All string primitives automatically execute `value.Normalize(NormalizationForm.FormC)` before executing length, regex, or custom validators.
- Prevents security bypasses where length limits or pattern matches are evaded through decomposed character sequences.
- Ensures hash code and equality parity across all inputs representing the same logical string.

```csharp
// Emitted validation preamble (SEC-004)
string normalized = value.IsNormalized(NormalizationForm.FormC)
    ? value
    : value.Normalize(NormalizationForm.FormC);
```

---

## Error Message Localization

While internal domain validation logic is invariant, user-facing error messages must support internationalization for end users.

### 1. Canonical Error Codes
Validation errors expose language-neutral, machine-readable canonical codes via `PrimitiveError.Code` (e.g., `EMAIL_INVALID_FORMAT`, `STRING_TOO_LONG`, `NUMBER_BELOW_MINIMUM`):

```csharp
if (!EmailAddress.TryCreate(input, out var email, out var error))
{
    // Retrieve localized string from application resource file using error.Code
    string localizedMessage = localizer[error.Code];
}
```

### 2. Custom Error Codes per Attribute
Developers can override the canonical code at the declaration site to align with downstream localization dictionaries:

```csharp
[StringPrimitive]
[MaxLength(50, ErrorCode = "ERR_USER_NAME_TOO_LONG")]
public readonly partial record struct UserName;
```
