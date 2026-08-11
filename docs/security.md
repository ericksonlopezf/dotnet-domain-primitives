# Security Gates — SEC-001 through SEC-006

> **Version:** 1.0.0 | **Date:** 2026-08-10  
> **Audience:** Library users, security reviewers, penetration testers

DomainPrimitives is the only .NET domain primitive library with built-in security gates applied automatically to **all** string-backed primitives. These gates run before your custom validation logic, ensuring a baseline of security even if you forget to define explicit rules.

---

## Overview

```
Input string
    │
    ▼
SEC-006: ArrayPool / stackalloc gate ──────────── blocks stack overflow
    │
    ▼
SEC-004: NFC Unicode normalization ────────────── eliminates homoglyph attacks
    │
    ▼
[Your normalization: Trim, LowerCase, etc.]
    │
    ▼
SEC-001: 4096-character limit ─────────────────── blocks memory exhaustion
    │
    ▼
SEC-005: NotEmpty guard (on sensitive types) ──── prevents empty PII
    │
    ▼
SEC-002 + SEC-003: Regex with NonBacktracking ─── eliminates ReDoS
    │
    ▼
SEC-005: No PII echoed in error messages ──────── prevents information leakage
    │
    ▼
[Your custom validator, if defined]
    │
    ▼
Domain primitive created ✅
```

---

## SEC-001: Default 4096-Character Limit

### Problem

Without an explicit `MaxLength`, a string value can be arbitrarily long. An attacker can send a 10MB string to a JSON endpoint, which:
1. Deserializes successfully
2. Passes through validation (no length check)
3. Gets stored in memory, logged, or sent to a database
4. Triggers allocation spikes and potential denial of service

### Solution

DomainPrimitives applies a **4096-character hard limit** to every `[StringPrimitive]` that does not have an explicit `MaxLength` or `ExactLength`:

```csharp
// Generated code (when no MaxLength is specified):
if (value.Length > 4096)
    return new PrimitiveError("LENGTH", $"{TypeName} must be at most 4096 character(s) (security limit). Got {value.Length}.");
```

### Override

If you need a longer value, explicitly declare your limit:

```csharp
[StringPrimitive]
[MaxLength(65536)] // SEC-001 waived; explicit 64KB limit replaces default
public readonly partial record struct LargeTextValue;
```

### Error code: `"LENGTH"`

---

## SEC-002: NonBacktracking Regex

### Problem

Regular expression engines using backtracking can be exploited by crafting inputs that cause exponential time complexity (ReDoS — Regular Expression Denial of Service). Example: the regex `^(a+)+$` applied to `"aaaaaaaaaaaaaaaaaab"` can take seconds or minutes with a backtracking engine.

### Solution

On .NET 7+, DomainPrimitives uses `RegexOptions.NonBacktracking` for all generated regex validators:

```csharp
// Generated code (on .NET 7+):
[GeneratedRegex(@"^[a-zA-Z0-9.!...]+@...$",
    RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.NonBacktracking)]
private static partial Regex ValidationRegex();
```

`NonBacktracking` uses a deterministic finite automaton (DFA) that runs in O(n) time for ALL inputs, regardless of the pattern. This eliminates the entire class of ReDoS vulnerabilities.

### Fallback on .NET 6 and earlier

`RegexOptions.NonBacktracking` requires .NET 7+. For earlier TFMs, DomainPrimitives falls back to `RegexOptions.Compiled` with SEC-003 (timeout).

---

## SEC-003: 100ms Regex Timeout

### Problem

On .NET 6 and earlier (or on custom patterns that NonBacktracking cannot optimize), regex evaluation can still be slow.

### Solution

DomainPrimitives injects a 100ms timeout on all regex validators for TFMs that do not support `NonBacktracking`:

```csharp
// Generated code (fallback for older TFMs):
private static readonly Regex ValidationRegex = new Regex(
    @"^[a-zA-Z0-9...]+@...$",
    RegexOptions.Compiled | RegexOptions.CultureInvariant,
    matchTimeout: TimeSpan.FromMilliseconds(100));
```

This caps the worst-case regex evaluation time at 100ms per validation call, preventing thread exhaustion under adversarial input.

### Error behavior

If the timeout fires, `Regex.IsMatch()` throws `RegexMatchTimeoutException`. DomainPrimitives catches this and returns a `"FORMAT"` error with the message `"Validation timed out."`.

---

## SEC-004: NFC Unicode Normalization

### Problem

Unicode allows multiple byte sequences to represent visually identical characters:
- `é` can be encoded as U+00E9 (precomposed) OR as `e` + U+0301 (combining accent)
- An attacker can use the combining form to bypass regex validation

This is a **Unicode homoglyph attack**:
```
"admin"    // U+0061 U+0064 U+006D U+0069 U+006E — normal ASCII
"аdmin"   // U+0430 (Cyrillic а) + ASCII "dmin" — visually identical
```

If your validation regex checks for `^[a-z]+$`, the Cyrillic version passes the regex but is a different string than `"admin"`.

### Solution

DomainPrimitives applies **NFC (Canonical Decomposition followed by Canonical Composition)** normalization to ALL string inputs before any validation runs:

```csharp
// Generated code (in Normalize method, applied to all string paths):
value = value.Normalize(System.Text.NormalizationForm.FormC);
```

NFC normalization:
1. Decomposes characters into their canonical forms
2. Recomposes them into the canonical composed form (precomposed)
3. Ensures that `é` is always U+00E9, never e + combining accent

After NFC normalization, your regex `^[a-z]+$` will correctly reject Cyrillic characters.

### Performance cost

NFC normalization requires allocating a new `System.String` (the .NET BCL `string.Normalize()` method cannot normalize in-place). This is the **one unavoidable heap allocation** in all DomainPrimitives string hot paths. See the allocation model in [README.md](../README.md#allocation-model).

---

## SEC-005: No PII in Error Messages

### Problem

Error messages that echo user-provided input can leak Personally Identifiable Information (PII) in logs, error responses, or monitoring systems.

**Bad pattern:**
```csharp
throw new ValidationException($"Email '{userEmail}' is invalid");
// → "Email 'user@internal-corp.com' is invalid" — now in your logs
```

### Solution

DomainPrimitives **never echoes the user-provided value** in error messages. Error messages only describe what was wrong, not what the value was:

```
"Email must not be empty."
"CountryCode must be exactly 2 character(s). Got 5."  ← echoes length, not content
"Phone must match the required format."               ← no value echoed
```

For length errors, only the character count is included — not the value itself. This prevents PII leakage while still providing actionable diagnostic information.

---

## SEC-006: Bounded Allocations for Large Inputs

### Problem

When processing a large string (>256 characters) through normalization (TrimStart, ToLowerInvariant, ToUpperInvariant), naïve code would allocate a new intermediate string for each step, creating allocation pressure.

More critically, a stack-allocated buffer (`stackalloc`) for a very large input can cause a stack overflow.

### Solution

DomainPrimitives uses a two-tier allocation strategy for normalization:

**For inputs ≤ 256 characters:**
```csharp
Span<char> buf = stackalloc char[s.Length]; // Stack allocation — no heap
MemoryExtensions.ToLowerInvariant(s, buf);  // In-place normalization
```

**For inputs > 256 characters:**
```csharp
char[] rented = ArrayPool<char>.Shared.Rent(s.Length);
try
{
    MemoryExtensions.ToLowerInvariant(s, rented);
    // ... use the buffer
}
finally
{
    ArrayPool<char>.Shared.Return(rented); // Always returned to pool
}
```

The 256-character threshold prevents stack overflows from large inputs. The `ArrayPool<char>` rental avoids new heap allocations for the normalization buffer — only the final NFC-normalized string (SEC-004) is a new heap object.

---

## Security Comparison vs Competitors

| Gate | DomainPrimitives | Vogen | Thinktecture | StronglyTypedId |
|:---|:---:|:---:|:---:|:---:|
| Default length limit (SEC-001) | ✅ 4096 chars | ❌ | ❌ | ❌ |
| NonBacktracking regex (SEC-002) | ✅ | ❌ | ❌ | ❌ |
| Regex timeout (SEC-003) | ✅ 100ms | ❌ | ❌ | ❌ |
| Unicode NFC normalization (SEC-004) | ✅ | ❌ | ❌ | ❌ |
| No PII in error messages (SEC-005) | ✅ | — | — | ❌ |
| Bounded allocations (SEC-006) | ✅ | ❌ | ❌ | ❌ |

**All six gates are enabled by default.** No opt-in required.

---

## Opting Out

If a specific security gate conflicts with a legitimate domain requirement:

### Override SEC-001 (4096-char limit):
```csharp
[StringPrimitive]
[MaxLength(1_000_000)] // Explicitly allow up to 1MB
public readonly partial record struct LargeBase64Payload;
```

### Override SEC-002/SEC-003 (regex timeout/NonBacktracking):
Not directly configurable. If your regex pattern does not compile with `NonBacktracking`, open an issue. The generated regex is protected by the timeout as a fallback.

### Override SEC-004 (NFC normalization):
Not configurable — NFC is always applied. This is by design. Disabling NFC would reintroduce homoglyph vulnerabilities. If you need a raw (non-normalized) string, use `System.String` directly.

---

## Reporting Security Issues

Please report security vulnerabilities via GitHub private security advisories, not via public issues.
