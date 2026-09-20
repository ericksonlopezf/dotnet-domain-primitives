// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives.Generators.Models;

namespace EricksonLopez.DomainPrimitives.Generators;

internal sealed partial class StringPrimitiveGenerator
{
    private static void GenerateNormalize(SourceBuilder sb, StringPrimitiveTypeInfo info)
    {

        sb.AppendLine("// ─── Normalization ───────────────────────────────────────────────");
        sb.AppendLine();

        sb.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine("private static string Normalize(string value)");
        sb.OpenBrace();

        if (info.Trim)
            sb.AppendLine("value = value.Trim();");
        if (info.TrimStart)
            sb.AppendLine("value = value.TrimStart();");
        if (info.TrimEnd)
            sb.AppendLine("value = value.TrimEnd();");
        if (info.NormalizeWhitespace)
        {
            sb.AppendLine("// Collapse consecutive whitespace into single space");
            sb.AppendLine("value = WhitespaceRegex.Replace(value, \" \");");
        }
        if (info.LowerCase)
            sb.AppendLine("value = value.ToLowerInvariant();");
        if (info.UpperCase)
            sb.AppendLine("value = value.ToUpperInvariant();");
        sb.AppendLine("try");
        sb.OpenBrace();
        sb.AppendLine("value = value.Normalize(System.Text.NormalizationForm.FormC);");
        sb.CloseBrace();
        // HIGH-01: Use named 'ex' only in the branch where it is re-thrown as inner exception.
        // When CustomExceptionType is set, the original ArgumentException is not wrapped, so we
        // use a discard-style catch (without a named variable) to avoid CS0168 in the generated
        // project when TreatWarningsAsErrors=true.
        if (!string.IsNullOrEmpty(info.CustomExceptionType))
        {
            sb.AppendLine("catch (ArgumentException)");
            sb.OpenBrace();
            sb.AppendLine($"throw new {info.CustomExceptionType}(\"Value contains invalid or unpaired Unicode code points.\");");
            sb.CloseBrace();
        }
        else
        {
            sb.AppendLine("catch (ArgumentException ex)");
            sb.OpenBrace();
            sb.AppendLine("throw new global::EricksonLopez.DomainPrimitives.DomainPrimitiveValidationException(new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"INVALID_UNICODE\", \"Value contains invalid or unpaired Unicode code points.\"), ex);");
            sb.CloseBrace();
        }
        sb.AppendLine("return value;");

        sb.CloseBrace();
        sb.AppendLine();
    }

}


