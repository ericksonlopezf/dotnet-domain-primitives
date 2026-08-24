// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.DomainPrimitives.Generators.Models;

namespace EricksonLopez.DomainPrimitives.Generators;

internal sealed partial class StringPrimitiveGenerator
{
    private static void GenerateValidation(SourceBuilder sb, StringPrimitiveTypeInfo info)
    {
        sb.AppendLine("// ─── Validation ─────────────────────────────────────────────────");
        sb.AppendLine();

        // Validate — throws on failure
        sb.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine($"private static void Validate(string value)");
        sb.OpenBrace();
        sb.AppendLine($"var error = TryValidate(value);");
        sb.AppendLine($"if (error.IsError)");
        sb.OpenBrace();
        if (!string.IsNullOrEmpty(info.CustomExceptionType))
        {
            sb.AppendLine($"throw new {info.CustomExceptionType}(error.Message);");
        }
        else
        {
            sb.AppendLine($"throw new DomainPrimitiveValidationException(error);");
        }
        sb.CloseBrace();
        sb.CloseBrace();
        sb.AppendLine();

        // TryValidate — returns global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError (null = valid)
        sb.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine("private static global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError TryValidate(string value)");
        sb.OpenBrace();

        if (info.NotEmpty)
        {
            sb.AppendLine("if (string.IsNullOrWhiteSpace(value))");
            sb.IncreaseIndent();
            sb.AppendLine($"return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"EMPTY\", \"{info.TypeName} must not be empty.\");");
            sb.DecreaseIndent();
        }

            if (info.ExactLength.HasValue)
            {
                sb.AppendLine($"if (value.Length != {info.ExactLength.Value})");
                sb.IncreaseIndent();
                sb.AppendLine($"return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"LENGTH\", $\"{info.TypeName} must be exactly {info.ExactLength.Value} character(s). Got {{value.Length}}.\");");
                sb.DecreaseIndent();
            }
            else
            {
                if (info.MinLength.HasValue)
                {
                    sb.AppendLine($"if (value.Length < {info.MinLength.Value})");
                    sb.IncreaseIndent();
                    sb.AppendLine($"return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"LENGTH\", $\"{info.TypeName} must be at least {info.MinLength.Value} character(s). Got {{value.Length}}.\");");
                    sb.DecreaseIndent();
                }

                if (info.MaxLength.HasValue)
                {
                    sb.AppendLine($"if (value.Length > {info.MaxLength.Value})");
                    sb.IncreaseIndent();
                    sb.AppendLine($"return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"LENGTH\", $\"{info.TypeName} must be at most {info.MaxLength.Value} character(s). Got {{value.Length}}.\");");
                    sb.DecreaseIndent();
                }
                else
                {
                    sb.AppendLine($"if (value.Length > 4096)");
                    sb.IncreaseIndent();
                    sb.AppendLine($"return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"LENGTH\", $\"{info.TypeName} must be at most 4096 character(s) (security limit). Got {{value.Length}}.\");");
                    sb.DecreaseIndent();
                }
            }


            // URL validation via Uri.TryCreate
            if (info.DomainShortcut == "Url")
            {
                var schemes = new string[info.AllowedSchemes.Length];
                for (int i = 0; i < info.AllowedSchemes.Length; i++)
                {
                    schemes[i] = $"uri.Scheme != \"{info.AllowedSchemes[i]}\"";
                }
                
                var condition = string.Join(" && ", schemes);
                var isDefaultHttp = info.AllowedSchemes.Length == 2 && info.AllowedSchemes[0] == "https" && info.AllowedSchemes[1] == "http";
                var schemeNames = isDefaultHttp
                    ? "HTTP(S)"
                    : string.Join("/", info.AllowedSchemes.Values).ToUpperInvariant();
                sb.AppendLine($"if (!Uri.TryCreate(value.ToString(), UriKind.Absolute, out var uri) || ({condition}))");
                sb.IncreaseIndent();
                sb.AppendLine($"return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"FORMAT\", \"{info.TypeName} must be a valid absolute {schemeNames} URL.\");");
                sb.DecreaseIndent();
            }

            // Regex validation
            for (int i = 0; i < info.RegexPatterns.Length; i++)
            {
                var regex = info.RegexPatterns[i];
                var fieldName = info.RegexPatterns.Length == 1 ? "ValidationRegex" : $"ValidationRegex{i + 1}";
                var errorMsg = regex.ErrorMessage ?? $"{info.TypeName} has an invalid format.";
                sb.AppendLine($"if (!{fieldName}.IsMatch(value))");
                sb.IncreaseIndent();
                sb.AppendLine($"return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"FORMAT\", \"{EscapeString(errorMsg)}\");");
                sb.DecreaseIndent();
            }

            sb.AppendLine("return global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError.None;");
        sb.CloseBrace();
        sb.AppendLine();
    }

    private static void GenerateSpanValidation(SourceBuilder sb, StringPrimitiveTypeInfo info)
    {
        // SEC-004: TryValidateSpan must only be called on a NFC-normalized span.
        // The calling site (TryParse fast path for types without case normalization)
        // must ensure the span has been normalized to FormC before calling this method.
        // For types WITH case normalization the span path delegates to TryValidate(string)
        // which is called after normalization.
        sb.AppendLine("/// <summary>Validates a pre-NFC-normalized span. Caller must ensure the span is already in NFC form.</summary>");
        sb.AppendLine("private static global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError TryValidateSpan(ReadOnlySpan<char> value)");
        sb.OpenBrace();

        if (info.NotEmpty)
        {
            sb.AppendLine("#if NET7_0_OR_GREATER");
            sb.AppendLine("if (value.IsWhiteSpace())");
            sb.AppendLine("#else");
            sb.AppendLine("if (value.Trim().Length == 0)");
            sb.AppendLine("#endif");
            sb.IncreaseIndent();
            sb.AppendLine($"return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"EMPTY\", \"{info.TypeName} must not be empty.\");");
            sb.DecreaseIndent();
        }

        if (info.ExactLength.HasValue)
        {
            sb.AppendLine($"if (value.Length != {info.ExactLength.Value})");
            sb.IncreaseIndent();
            sb.AppendLine($"return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"LENGTH\", $\"{info.TypeName} must be exactly {info.ExactLength.Value} character(s). Got {{value.Length}}.\");");
            sb.DecreaseIndent();
        }
        else
        {
            if (info.MinLength.HasValue)
            {
                sb.AppendLine($"if (value.Length < {info.MinLength.Value})");
                sb.IncreaseIndent();
                sb.AppendLine($"return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"LENGTH\", $\"{info.TypeName} must be at least {info.MinLength.Value} character(s). Got {{value.Length}}.\");");
                sb.DecreaseIndent();
            }

            if (info.MaxLength.HasValue)
            {
                sb.AppendLine($"if (value.Length > {info.MaxLength.Value})");
                sb.IncreaseIndent();
                sb.AppendLine($"return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"LENGTH\", $\"{info.TypeName} must be at most {info.MaxLength.Value} character(s). Got {{value.Length}}.\");");
                sb.DecreaseIndent();
            }
            else
            {
                sb.AppendLine($"if (value.Length > 4096)");
                sb.IncreaseIndent();
                sb.AppendLine($"return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"LENGTH\", $\"{info.TypeName} must be at most 4096 character(s) (security limit). Got {{value.Length}}.\");");
                sb.DecreaseIndent();
            }
        }

        // URL validation via Uri.TryCreate (Fallback to string)
        if (info.DomainShortcut == "Url")
        {
            var schemes = new string[info.AllowedSchemes.Length];
            for (int i = 0; i < info.AllowedSchemes.Length; i++)
            {
                schemes[i] = $"uri.Scheme != \"{info.AllowedSchemes[i]}\"";
            }
            
            var condition = string.Join(" && ", schemes);
            var isDefaultHttp = info.AllowedSchemes.Length == 2 && info.AllowedSchemes[0] == "https" && info.AllowedSchemes[1] == "http";
            var schemeNames = isDefaultHttp
                ? "HTTP(S)"
                : string.Join("/", info.AllowedSchemes.Values).ToUpperInvariant();
            // MED-004 fix: Removed dead #if NET10_0_OR_GREATER block (both branches were identical).
            // Future: when a span-based Uri.TryCreate overload is available in .NET, add it here.
            sb.AppendLine($"if (!Uri.TryCreate(value.ToString(), UriKind.Absolute, out var uri) || ({condition}))");
            sb.IncreaseIndent();
            sb.AppendLine($"return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"FORMAT\", \"{info.TypeName} must be a valid absolute {schemeNames} URL.\");");
            sb.DecreaseIndent();
        }

        // Regex validation
        for (int i = 0; i < info.RegexPatterns.Length; i++)
        {
            var regex = info.RegexPatterns[i];
            var fieldName = info.RegexPatterns.Length == 1 ? "ValidationRegex" : $"ValidationRegex{i + 1}";
            var errorMsg = regex.ErrorMessage ?? $"{info.TypeName} has an invalid format.";
            sb.AppendLine($"if (!{fieldName}.IsMatch(value))");
            sb.IncreaseIndent();
            sb.AppendLine($"return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"FORMAT\", \"{EscapeString(errorMsg)}\");");
            sb.DecreaseIndent();
        }

        sb.AppendLine("return global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError.None;");
        sb.CloseBrace();
        sb.AppendLine();
    }
}




