// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using EricksonLopez.DomainPrimitives.Generators.Models;
using System.Threading.Tasks;

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
        sb.AppendLine("value = value.Normalize(System.Text.NormalizationForm.FormC);");
        sb.AppendLine("return value;");

        sb.CloseBrace();
        sb.AppendLine();
    }

}


