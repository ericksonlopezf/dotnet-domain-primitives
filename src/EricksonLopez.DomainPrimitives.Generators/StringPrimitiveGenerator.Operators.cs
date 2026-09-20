// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives.Generators.Models;

namespace EricksonLopez.DomainPrimitives.Generators;

internal sealed partial class StringPrimitiveGenerator
{
    private static void GenerateOperators(SourceBuilder sb, StringPrimitiveTypeInfo info)
    {
        sb.AppendLine("// ─── Operators ────────────────────────────────────────────────────");
        sb.AppendLine();

        sb.AppendLine("/// <summary>Explicit conversion to string.</summary>");
        sb.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine($"public static explicit operator string({info.TypeName} value) =>");
        sb.AppendLine($"    value._value ?? throw new InvalidOperationException(\"Cannot convert a default {info.TypeName} to string. Check IsDefault before casting.\");");
        sb.AppendLine();

        sb.AppendLine("/// <summary>Explicit conversion from string. Applies normalization and validation.</summary>");
        sb.AppendLine($"public static explicit operator {info.TypeName}(string value) => Create(value);");
        sb.AppendLine();
    }

}


