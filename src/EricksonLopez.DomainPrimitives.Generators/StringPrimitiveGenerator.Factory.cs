// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives.Generators.Models;

namespace EricksonLopez.DomainPrimitives.Generators;

internal sealed partial class StringPrimitiveGenerator
{
    private static void GenerateFactoryMethods(SourceBuilder sb, StringPrimitiveTypeInfo info)
    {
        sb.AppendLine("// ─── Factory Methods ─────────────────────────────────────────────");
        sb.AppendLine();

        // Create(string) — normalize → validate → wrap
        sb.AppendLine("/// <summary>Creates a valid instance. Normalizes, then validates. Throws on invalid input.</summary>");
        sb.AppendLine("/// <exception cref=\"ArgumentNullException\">Thrown when the value is null.</exception>");
        sb.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine($"public static {info.TypeName} Create(string value)");
        sb.OpenBrace();
        sb.AppendLine("ArgumentNullException.ThrowIfNull(value);");
        var rawLimit = info.MaxLength.HasValue
            ? Math.Max(info.MaxLength.Value * 2, 4096)
            : (info.ExactLength.HasValue ? Math.Max(info.ExactLength.Value * 2, 4096) : 4096);
        // HIGH-02/DoS guard: Reject inputs that far exceed the configured limit BEFORE running
        // the expensive NFC normalization. This is not a semantic validation — it's a safety ceiling
        // to prevent O(n) normalization on attacker-controlled gigantic strings.
        // rawLimit = max(maxLength * 2, 4096) gives a 2x safety margin above the configured limit.
        // A string > rawLimit chars can NEVER pass maxLength validation after normalization,
        // so we call Validate() here which will throw/return the length error immediately.
        sb.AppendLine($"if (value.Length > {rawLimit})");
        sb.OpenBrace();
        sb.AppendLine("// DoS guard — reject here before expensive NFC normalization.");
        sb.AppendLine("Validate(value);");
        sb.CloseBrace();
        sb.AppendLine("value = Normalize(value);");
        sb.AppendLine("Validate(value);");
        sb.AppendLine($"return new {info.TypeName}(value);");
        sb.CloseBrace();
        sb.AppendLine();


        sb.AppendLine("/// <summary>Tries to create a valid instance. Returns a boolean indicating success.</summary>");
        sb.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine($"public static bool TryCreate(string value, out {info.TypeName} result, out global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError validationError)");
        sb.OpenBrace();
        sb.AppendLine("if (value is null)");
        sb.OpenBrace();
        sb.AppendLine("result = default;");
        sb.AppendLine($"validationError = new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"NULL_INPUT\", \"Value cannot be null.\");");
        sb.AppendLine("return false;");
        sb.CloseBrace();
        sb.AppendLine($"if (value.Length > {rawLimit})");
        sb.OpenBrace();
        sb.AppendLine("// DoS guard — reject before expensive NFC normalization.");
        sb.AppendLine("result = default;");
        sb.AppendLine("validationError = TryValidate(value);");
        sb.AppendLine("return false;");
        sb.CloseBrace();
        sb.AppendLine("try");
        sb.OpenBrace();
        sb.AppendLine("value = Normalize(value);");
        sb.CloseBrace();
        // MED-04: Narrow the catch to specific exception types rather than using a broad
        // 'catch (Exception) when (...)' filter. Normalize() can only throw ArgumentException
        // (from string.Normalize() on invalid Unicode) or the configured custom/validation exception.
        // Catching the base Exception class — even with a filter — risks silently swallowing
        // unexpected bugs inside Normalize(), violating fail-fast principles.
        var customEx = !string.IsNullOrEmpty(info.CustomExceptionType)
            ? (info.CustomExceptionType!.StartsWith("global::", StringComparison.Ordinal) ? info.CustomExceptionType : $"global::{info.CustomExceptionType}")
            : null;
        sb.AppendLine("catch (global::EricksonLopez.DomainPrimitives.DomainPrimitiveValidationException ex)");
        sb.OpenBrace();
        sb.AppendLine("result = default;");
        sb.AppendLine("validationError = ex.Error;");
        sb.AppendLine("return false;");
        sb.CloseBrace();
        if (customEx != null)
        {
            sb.AppendLine($"catch ({customEx} ex)");
            sb.OpenBrace();
            sb.AppendLine("result = default;");
            sb.AppendLine("validationError = new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"INVALID_UNICODE\", ex.Message);");
            sb.AppendLine("return false;");
            sb.CloseBrace();
        }
        sb.AppendLine("catch (ArgumentException)");
        sb.OpenBrace();
        sb.AppendLine("result = default;");
        sb.AppendLine("validationError = new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"INVALID_UNICODE\", \"Value contains invalid or unpaired Unicode code points.\");");
        sb.AppendLine("return false;");
        sb.CloseBrace();
        sb.AppendLine("validationError = TryValidate(value);");
        sb.AppendLine("if (validationError.IsError)");
        sb.OpenBrace();
        sb.AppendLine("result = default;");
        sb.AppendLine("return false;");
        sb.CloseBrace();
        sb.AppendLine($"result = new {info.TypeName}(value);");
        sb.AppendLine("return true;");
        sb.CloseBrace();
        sb.AppendLine();
    }

}
