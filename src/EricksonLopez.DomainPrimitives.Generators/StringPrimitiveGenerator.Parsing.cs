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
    private static void GenerateParsing(SourceBuilder sb, StringPrimitiveTypeInfo info)
    {
        sb.AppendLine("// ─── Parsing (IParsable, ISpanParsable, IUtf8SpanParsable) ───────");
        sb.AppendLine();

        // Parse(string)
        sb.AppendLine($"public static {info.TypeName} Parse(string s, IFormatProvider? provider)");
        sb.OpenBrace();
        sb.AppendLine("if (TryParse(s, provider, out var result)) return result;");
        sb.AppendLine($"throw new System.FormatException($\"The value '{{s}}' is not valid for {info.TypeName}.\");");
        sb.CloseBrace();
        sb.AppendLine();

        // TryParse(string)
        sb.AppendLine($"public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out {info.TypeName} result)");
        sb.OpenBrace();
        sb.AppendLine("if (s is not null)");
        sb.OpenBrace();
        sb.AppendLine($"if (TryCreate(s, out result, out _)) return true;");
        sb.CloseBrace();
        sb.AppendLine("result = default;");
        sb.AppendLine("return false;");
        sb.CloseBrace();
        sb.AppendLine();

        // Parse(ReadOnlySpan<char>)
        sb.AppendLine($"public static {info.TypeName} Parse(ReadOnlySpan<char> s, IFormatProvider? provider)");
        sb.OpenBrace();
        if (!info.LowerCase && !info.UpperCase && !info.NormalizeWhitespace)
        {
            if (info.Trim) sb.AppendLine("s = s.Trim();");
            // SEC-004: NFC-normalize before validation.
            sb.AppendLine("var normalized = s.ToString().Normalize(System.Text.NormalizationForm.FormC);");
            sb.AppendLine("var error = TryValidateSpan(normalized.AsSpan());");
            sb.AppendLine($"if (error.IsError) throw new System.FormatException(error.Message);");
            sb.AppendLine($"return new {info.TypeName}(normalized);");
        }
        else
        {
            sb.AppendLine($"if (TryParse(s, provider, out var result)) return result;");
            sb.AppendLine($"throw new System.FormatException(\"The span value is not valid.\");");
        }
        sb.CloseBrace();
        sb.AppendLine();

        // TryParse(ReadOnlySpan<char>)
        sb.AppendLine($"public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out {info.TypeName} result)");
        sb.OpenBrace();
        if (!info.LowerCase && !info.UpperCase && !info.NormalizeWhitespace)
        {
            if (info.Trim) sb.AppendLine("s = s.Trim();");
            // SEC-004: Apply NFC normalization before validation. NFC normalization can change the
            // character count (combining chars → composed), so we must produce a string first.
            // This is the single unavoidable allocation on this path; the stored value must be a
            // System.String and must be in NFC form per the security gate SEC-004.
            sb.AppendLine("var normalized = s.ToString().Normalize(System.Text.NormalizationForm.FormC);");
            sb.AppendLine("var error = TryValidateSpan(normalized.AsSpan());");
            sb.AppendLine("if (error.IsError)");
            sb.OpenBrace();
            sb.AppendLine("result = default;");
            sb.AppendLine("return false;");
            sb.CloseBrace();
            sb.AppendLine($"result = new {info.TypeName}(normalized);");
            sb.AppendLine("return true;");
        }
        else
        {
            // CRIT-003 fix: Use MemoryExtensions.ToLowerInvariant / ToUpperInvariant which operate
            // in-place on a Span<char> without allocating an intermediate string.
            // The only allocation is the final .Normalize(FormC) + .ToString() at the very end,
            // which is unavoidable because: (a) the stored value must be a System.String,
            // and (b) Unicode NFC normalization can change the char count so we must produce a string.
            // This reduces intermediate allocations from 3 (stackalloc→string, ToLower, Normalize)
            // to 1 (Normalize→string at storage time). NormalizeWhitespace still requires a Regex
            // pass which allocates, but that code path falls back to TryCreate.
            sb.AppendLine("#if NET8_0_OR_GREATER");

            if (info.NormalizeWhitespace)
            {
                // NormalizeWhitespace requires regex which returns a new string — fall through to TryCreate
                sb.AppendLine("// NormalizeWhitespace requires a regex pass; delegate to TryCreate.");
                if (info.Trim) sb.AppendLine("var trimmed = s.ToString().Trim();");
                else sb.AppendLine("var trimmed = s.ToString();");
                sb.AppendLine($"return TryCreate(trimmed, out result, out _);");
            }
            else
            {
                if (info.Trim) sb.AppendLine("s = s.Trim();");
                sb.AppendLine("// CRIT-003: Use in-place MemoryExtensions normalization to avoid intermediate string allocations.");
                sb.AppendLine("// Stackalloc limit: 256 chars = 512 bytes on stack — safe for typical domain values.");
                sb.AppendLine("if (s.Length <= 256)");
                sb.OpenBrace();
                sb.AppendLine("Span<char> buf = stackalloc char[s.Length];");
                sb.AppendLine("s.CopyTo(buf);");
                if (info.LowerCase)
                {
                    // MemoryExtensions.ToLowerInvariant(ReadOnlySpan<char>, Span<char>) — in-place, no allocation
                    sb.AppendLine("MemoryExtensions.ToLowerInvariant(s, buf);");
                }
                else
                {
                    sb.AppendLine("MemoryExtensions.ToUpperInvariant(s, buf);");
                }
                // NFC normalization: .Normalize(FormC) may change length — we MUST go to string here.
                // This is the single unavoidable allocation: the final stored string value.
                sb.AppendLine("var normalized = buf.ToString().Normalize(System.Text.NormalizationForm.FormC);");
                sb.AppendLine("var spanError = TryValidate(normalized);");
                sb.AppendLine("if (spanError.IsError) { result = default; return false; }");
                sb.AppendLine($"result = new {info.TypeName}(normalized);");
                sb.AppendLine("return true;");
                sb.CloseBrace();
                sb.AppendLine("else");
                sb.OpenBrace();
                sb.AppendLine("var rented = System.Buffers.ArrayPool<char>.Shared.Rent(s.Length);");
                sb.AppendLine("try");
                sb.OpenBrace();
                sb.AppendLine("var rentedSpan = rented.AsSpan(0, s.Length);");
                if (info.LowerCase)
                    sb.AppendLine("MemoryExtensions.ToLowerInvariant(s, rentedSpan);");
                else
                    sb.AppendLine("MemoryExtensions.ToUpperInvariant(s, rentedSpan);");
                // Single unavoidable allocation at storage time
                sb.AppendLine("var normalized = rentedSpan.ToString().Normalize(System.Text.NormalizationForm.FormC);");
                sb.AppendLine("var spanError = TryValidate(normalized);");
                sb.AppendLine("if (spanError.IsError) { result = default; return false; }");
                sb.AppendLine($"result = new {info.TypeName}(normalized);");
                sb.AppendLine("return true;");
                sb.CloseBrace();
                sb.AppendLine("finally");
                sb.OpenBrace();
                sb.AppendLine("System.Buffers.ArrayPool<char>.Shared.Return(rented);");
                sb.CloseBrace();
                sb.CloseBrace();
            }

            sb.AppendLine("#else");
            sb.AppendLine("// Fallback for older TFMs: allocates a string.");
            sb.AppendLine($"return TryParse(s.ToString(), provider, out result);");
            sb.AppendLine("#endif");
        }
        sb.CloseBrace();
        sb.AppendLine();

        // Parse(ReadOnlySpan<byte>) — UTF-8
        sb.AppendLine("#if NET8_0_OR_GREATER");
        sb.AppendLine($"public static {info.TypeName} Parse(ReadOnlySpan<byte> utf8, IFormatProvider? provider)");
        sb.OpenBrace();
        sb.AppendLine("// TD-003: Use GetMaxCharCount (O(1)) for the size guard — no traversal needed.");
        sb.AppendLine("// The real char count comes from GetChars() which returns the exact decoded length.");
        sb.AppendLine("int maxCount = System.Text.Encoding.UTF8.GetMaxCharCount(utf8.Length);");
        sb.AppendLine("// Stackalloc limit is 256 chars = 512 bytes on stack — safe for typical domain values.");
        sb.AppendLine("if (maxCount <= 256)");
        sb.OpenBrace();
        sb.AppendLine("Span<char> chars = stackalloc char[maxCount];");
        sb.AppendLine("int count = System.Text.Encoding.UTF8.GetChars(utf8, chars);");
        sb.AppendLine($"return Parse(chars.Slice(0, count), provider);");
        sb.CloseBrace();
        sb.AppendLine("else");
        sb.OpenBrace();
        sb.AppendLine("var rented = System.Buffers.ArrayPool<char>.Shared.Rent(maxCount);");
        sb.AppendLine("try");
        sb.OpenBrace();
        sb.AppendLine("int count = System.Text.Encoding.UTF8.GetChars(utf8, rented);");
        sb.AppendLine($"return Parse(rented.AsSpan(0, count), provider);");
        sb.CloseBrace();
        sb.AppendLine("finally");
        sb.OpenBrace();
        sb.AppendLine("System.Buffers.ArrayPool<char>.Shared.Return(rented);");
        sb.CloseBrace();
        sb.CloseBrace();
        sb.CloseBrace();
        sb.AppendLine();

        // TryParse(ReadOnlySpan<byte>)
        sb.AppendLine($"public static bool TryParse(ReadOnlySpan<byte> utf8, IFormatProvider? provider, out {info.TypeName} result)");
        sb.OpenBrace();
        sb.AppendLine("int maxCount = System.Text.Encoding.UTF8.GetMaxCharCount(utf8.Length);");
        sb.AppendLine("if (maxCount <= 256)");
        sb.OpenBrace();
        sb.AppendLine("Span<char> chars = stackalloc char[maxCount];");
        sb.AppendLine("int count = System.Text.Encoding.UTF8.GetChars(utf8, chars);");
        sb.AppendLine($"return TryParse(chars.Slice(0, count), provider, out result);");
        sb.CloseBrace();
        sb.AppendLine("else");
        sb.OpenBrace();
        sb.AppendLine("var rented = System.Buffers.ArrayPool<char>.Shared.Rent(maxCount);");
        sb.AppendLine("try");
        sb.OpenBrace();
        sb.AppendLine("int count = System.Text.Encoding.UTF8.GetChars(utf8, rented);");
        sb.AppendLine($"return TryParse(rented.AsSpan(0, count), provider, out result);");
        sb.CloseBrace();
        sb.AppendLine("finally");
        sb.OpenBrace();
        sb.AppendLine("System.Buffers.ArrayPool<char>.Shared.Return(rented);");
        sb.CloseBrace();
        sb.CloseBrace();
        sb.CloseBrace();
        sb.AppendLine("#endif");
        sb.AppendLine();
    }

}




