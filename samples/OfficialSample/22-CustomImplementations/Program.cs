// Copyright © Erickson Lopez. MIT License.
// ============================================================================
// CHAPTER 22: CUSTOM IMPLEMENTATIONS
// ============================================================================
// In this chapter you will learn to extend the library with your own
// custom validators, normalizers, and advanced builder patterns.
//
// COVERED APIS:
// 1. ICustomValidator<T>          — Reusable validation logic as a class.
// 2. [CustomValidator<TValidator>]— Applies a custom validator to a primitive.
// 3. INormalizer<T>               — Custom normalization strategy.
// 4. [Normalize<TNormalizer>]     — Applies a custom normalizer.
// 5. PrimitiveBuilder<TPrimitive, TValue> — Fluent builder with extra rules.
// 6. [assembly: DomainPrimitivesDefaults] — Assembly-level global defaults.
// ============================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Chapter22;
using EricksonLopez.DomainPrimitives;
using EricksonLopez.DomainPrimitives.Advanced;
using EricksonLopez.DomainPrimitives.Validation;

// Assembly-level global defaults (Section 4 demo).
// This attribute configures defaults for all string primitives in this assembly.
// Individual per-type attributes override these values.
// NOTE: Must appear after 'using' directives but before any code or type declarations.
[assembly: DomainPrimitivesDefaults(Trim = true, NotEmpty = false, MaxLength = 4096)]

Console.WriteLine("=========================================================");
Console.WriteLine(" 📘 CHAPTER 22: CUSTOM IMPLEMENTATIONS (VALIDATORS & NORMALIZERS)");
Console.WriteLine("=========================================================\n");

// ─────────────────────────────────────────────────────────────────────────────
// SECTION 1: ICustomValidator<T> + [CustomValidator<TValidator>]
// ─────────────────────────────────────────────────────────────────────────────
Console.WriteLine("--- 🔍 SECTION 1: CUSTOM VALIDATORS ---");

// LuhnCreditCardNumber uses [CustomValidator<LuhnCardValidator>].
// The validator is called after the built-in [StringPrimitive] + [MinLength]/[MaxLength] rules.

Console.WriteLine("\n[LuhnCreditCardNumber] — Custom Luhn Algorithm Validator");

// Valid Luhn number (4532015112830366 passes the Luhn check)
bool isValid = LuhnCreditCardNumber.TryCreate("4532015112830366", out var card, out var cardError);
Console.WriteLine($"  Create('4532015112830366'): {(isValid ? "✅ Valid — " + card.Value : "❌ " + cardError.Message)}");

// Invalid Luhn number (digit changed to fail the checksum)
bool isInvalid = LuhnCreditCardNumber.TryCreate("4532015112830367", out var badCard, out var badError);
Console.WriteLine($"  Create('4532015112830367'): {(isInvalid ? "✅ Valid" : "❌ " + badError.Message)}");

// Empty string fails the built-in MinLength rule before reaching the custom validator
bool isEmpty = LuhnCreditCardNumber.TryCreate("", out _, out var emptyError);
Console.WriteLine($"  Create(''): {(isEmpty ? "✅" : "❌ " + emptyError.Message)}");

Console.WriteLine();

// ─────────────────────────────────────────────────────────────────────────────
// SECTION 2: INormalizer<T> + [Normalize<TNormalizer>]
// ─────────────────────────────────────────────────────────────────────────────
Console.WriteLine("--- 🔄 SECTION 2: CUSTOM NORMALIZERS ---");

// ProductCode uses [Normalize<RemoveHyphensNormalizer>] which strips hyphens
// before the built-in validation runs. After normalization, [NotEmpty] and
// [MaxLength(20)] are verified against the normalized value.
Console.WriteLine("\n[ProductCode] — Custom RemoveHyphensNormalizer");

bool isCode = ProductCode.TryCreate("PROD-2026-XYZ", out var code, out var codeError);
Console.WriteLine($"  Create('PROD-2026-XYZ') (hyphens stripped by normalizer):");
Console.WriteLine($"    Success: {isCode} | Stored Value: '{(isCode ? code.Value : codeError.Message)}'");
// Expected: stored value is "PROD2026XYZ" (hyphens removed)

bool isLongCode = ProductCode.TryCreate("AAAAABBBBBCCCCCDDDDDEEEE", out _, out var longCodeError);
Console.WriteLine($"  Create('AAAAABBBBBCCCCCDDDDDEEEE') (>20 after normalization): {(!isLongCode ? "❌ " + longCodeError.Message : "✅ Unexpected success")}");

Console.WriteLine();

// ─────────────────────────────────────────────────────────────────────────────
// SECTION 3: PrimitiveBuilder<TPrimitive, TValue> — Fluent Builder
// ─────────────────────────────────────────────────────────────────────────────
Console.WriteLine("--- 🏗️ SECTION 3: PrimitiveBuilder<TPrimitive, TValue> ---");
Console.WriteLine();
Console.WriteLine("PrimitiveBuilder is designed for complex assembly scenarios where additional");
Console.WriteLine("runtime rules go beyond what attributes can express at compile time.");
Console.WriteLine();

// 3a. For() + WithValue() + Must() + BuildOrThrow()
Console.WriteLine("[3a] BuildOrThrow() — throws DomainPrimitiveValidationException on failure");

try
{
    var validScore = PrimitiveBuilder<CustomerScore, int>
        .For()
        .WithValue(85)
        .Must(v => v % 5 == 0, "SCORE.NOT_MULTIPLE", "Score must be a multiple of 5.")
        .BuildOrThrow();

    Console.WriteLine($"  BuildOrThrow(85, must be multiple of 5): ✅ Value = {validScore.Value}");
}
catch (DomainPrimitiveValidationException ex)
{
    Console.WriteLine($"  BuildOrThrow failed: [{ex.Error.Code}] {ex.Error.Message}");
}

try
{
    var invalidScore = PrimitiveBuilder<CustomerScore, int>
        .For()
        .WithValue(83)
        .Must(v => v % 5 == 0, "SCORE.NOT_MULTIPLE", "Score must be a multiple of 5.")
        .BuildOrThrow();

    Console.WriteLine($"  BuildOrThrow(83, must be multiple of 5): unexpected success ({invalidScore.Value})");
}
catch (DomainPrimitiveValidationException ex)
{
    Console.WriteLine($"  BuildOrThrow(83, must be multiple of 5): ❌ [{ex.Error.Code}] {ex.Error.Message}");
}

Console.WriteLine();

// 3b. Build() — non-throwing variant that returns bool
Console.WriteLine("[3b] Build(out result) — non-throwing try-style pattern");

bool built = PrimitiveBuilder<CustomerScore, int>
    .For()
    .WithValue(90)
    .Must(v => v % 5 == 0, "SCORE.NOT_MULTIPLE", "Score must be a multiple of 5.")
    .Build(out var builtScore);

Console.WriteLine($"  Build(90): {(built ? "✅ Value = " + builtScore.Value : "❌ Failed")}");

bool notBuilt = PrimitiveBuilder<CustomerScore, int>
    .For()
    .WithValue(77)
    .Must(v => v % 5 == 0, "SCORE.NOT_MULTIPLE", "Score must be a multiple of 5.")
    .Build(out var _);

Console.WriteLine($"  Build(77): {(notBuilt ? "✅ Unexpected" : "❌ Correctly rejected")}");

Console.WriteLine();

// 3c. Missing value — null guard
Console.WriteLine("[3c] BuildOrThrow() with no value set — NULL_INPUT guard");

try
{
    PrimitiveBuilder<CustomerScore, int>.For().BuildOrThrow();
    Console.WriteLine("  Unexpected success");
}
catch (DomainPrimitiveValidationException ex)
{
    Console.WriteLine($"  BuildOrThrow() with no value: ❌ [{ex.Error.Code}] {ex.Error.Message}");
}

Console.WriteLine();

// 3d. Multiple Must() rules — first failing rule wins
Console.WriteLine("[3d] Multiple Must() rules — first failure wins");

bool multipleRules = PrimitiveBuilder<CustomerScore, int>
    .For()
    .WithValue(0)
    .Must(v => v >= 0, "SCORE.NEGATIVE", "Score cannot be negative.")
    .Must(v => v <= 100, "SCORE.OVERFLOW", "Score cannot exceed 100.")
    .Must(v => v % 5 == 0, "SCORE.NOT_MULTIPLE", "Score must be a multiple of 5.")
    .Build(out var multipleResult);

Console.WriteLine($"  Build(0, >=0, <=100, %5==0): {(multipleRules ? "✅ Value = " + multipleResult.Value : "❌ Unexpected failure")}");

Console.WriteLine();

// ─────────────────────────────────────────────────────────────────────────────
// SECTION 4: [assembly: DomainPrimitivesDefaults(...)]
// ─────────────────────────────────────────────────────────────────────────────
Console.WriteLine("--- ⚙️  SECTION 4: [assembly: DomainPrimitivesDefaults] ---");
Console.WriteLine();
Console.WriteLine("DomainPrimitivesDefaultsAttribute is declared at assembly level and");
Console.WriteLine("configures global defaults for all string primitives in the assembly.");
Console.WriteLine();
Console.WriteLine("Example declaration (top of your AssemblyInfo.cs or any .cs file):");
Console.WriteLine("  [assembly: DomainPrimitivesDefaults(Trim = true, NotEmpty = true, MaxLength = 1024)]");
Console.WriteLine();
Console.WriteLine("Available properties:");
Console.WriteLine("  • Trim      (bool)  — auto-trim all string primitives");
Console.WriteLine("  • NotEmpty  (bool)  — auto-reject empty/whitespace-only strings");
Console.WriteLine("  • MaxLength (int)   — default max length (security limit). Default: 4096");
Console.WriteLine("  • ExceptionType (Type?) — custom exception type for Create() failures");
Console.WriteLine();
Console.WriteLine("Per-type attributes always take precedence over assembly defaults.");
Console.WriteLine();

// Demonstrate that a primitive decorated with assembly defaults (set in this file)
// automatically picks up the Trim = true default.
// The [assembly: DomainPrimitivesDefaults] attribute is declared below.
bool trimmedDefault = TrimmedByDefault.TryCreate("  hello  ", out var trimmed, out _);
Console.WriteLine($"  TrimmedByDefault.Create('  hello  ') with Trim=true default:");
Console.WriteLine($"    Stored value: '{(trimmedDefault ? trimmed.Value : "(failed)")}' (leading/trailing whitespace removed ✅)");

Console.WriteLine("\nCHAPTER 22 COMPLETED SUCCESSFULLY.\n");


// ============================================================================
// DOMAIN TYPE DEFINITIONS
// ============================================================================

namespace Chapter22
{
    // ─── Section 1: Custom Validator ────────────────────────────────────────

    /// <summary>
    /// Validates a credit card number using the Luhn algorithm.
    /// Applied via [CustomValidator&lt;LuhnCardValidator&gt;] on <see cref="LuhnCreditCardNumber"/>.
    /// </summary>
    public sealed class LuhnCardValidator : ICustomValidator<string>
    {
#if NET7_0_OR_GREATER
        /// <summary>
        /// Runs the Luhn checksum algorithm and returns an error if the value fails.
        /// </summary>
        public static PrimitiveError Validate(string value)
        {
            // Strip any spaces or dashes that may appear in formatted card numbers
            var digits = value.Replace(" ", "").Replace("-", "");

            if (!digits.All(char.IsDigit) || digits.Length < 12 || digits.Length > 19)
                return new PrimitiveError("CARD.LUHN_FORMAT", "Value is not a valid credit card number format.");

            // Luhn algorithm
            int sum = 0;
            bool doubleIt = false;
            for (int i = digits.Length - 1; i >= 0; i--)
            {
                int digit = digits[i] - '0';
                if (doubleIt)
                {
                    digit *= 2;
                    if (digit > 9) digit -= 9;
                }
                sum += digit;
                doubleIt = !doubleIt;
            }

            return sum % 10 == 0
                ? PrimitiveError.None
                : new PrimitiveError("CARD.LUHN_INVALID", "Value has an invalid Luhn checksum.");
        }
#endif
    }

    /// <summary>
    /// A credit card number validated using the Luhn algorithm.
    /// Combines built-in string constraints with a custom Luhn validator.
    /// </summary>
    [StringPrimitive]
    [MinLength(12)]
    [MaxLength(19)]
    [CustomValidator<LuhnCardValidator>]
    public readonly partial record struct LuhnCreditCardNumber;


    // ─── Section 2: Custom Normalizer ───────────────────────────────────────

    /// <summary>
    /// Removes all hyphen characters from a string before validation.
    /// Applied via [Normalize&lt;RemoveHyphensNormalizer&gt;] on <see cref="ProductCode"/>.
    /// </summary>
    public sealed class RemoveHyphensNormalizer : INormalizer<string>
    {
#if NET7_0_OR_GREATER
        /// <summary>
        /// Returns the value with all hyphen ('-') characters removed.
        /// </summary>
        public static string Normalize(string value) => value.Replace("-", string.Empty);
#endif
    }

    /// <summary>
    /// A product code where hyphens are stripped before built-in validation.
    /// e.g., "PROD-2026-XYZ" is normalized to "PROD2026XYZ".
    /// </summary>
    [StringPrimitive]
    [Trim]
    [NotEmpty]
    [MaxLength(20)]
    [Normalize<RemoveHyphensNormalizer>]
    public readonly partial record struct ProductCode;


    // ─── Section 3: PrimitiveBuilder usage primitive ─────────────────────────

    /// <summary>
    /// A customer satisfaction score in the 0–100 range (int).
    /// Uses explicit <c>[NumericPrimitive&lt;int&gt;]</c> + <c>[PrimitiveRange(0, 100)]</c> instead of
    /// the <c>[Score]</c> shortcut so that <see cref="PrimitiveBuilder{TPrimitive,TValue}"/>
    /// can satisfy its <c>IDomainPrimitive&lt;CustomerScore, int&gt;</c> generic constraint.
    /// (The <c>[Score]</c> shortcut generates a different internal backing — use
    /// <c>[NumericPrimitive&lt;int&gt;]</c> when you need PrimitiveBuilder compatibility.)
    /// </summary>
    [NumericPrimitive<int>]
    [PrimitiveRange(0, 100)]
    public readonly partial record struct CustomerScore;


    // ─── Section 4: TrimmedByDefault — demonstrates DomainPrimitivesDefaults ─

    /// <summary>
    /// A simple string primitive that relies on the assembly-level
    /// <see cref="DomainPrimitivesDefaultsAttribute"/> for automatic trimming.
    /// </summary>
    [StringPrimitive]
    [Trim]
    public readonly partial record struct TrimmedByDefault;
}
