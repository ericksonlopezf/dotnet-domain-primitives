// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives.Generators.Shared;

internal static class ComparisonTemplate
{
    public static void GenerateComparison(SourceBuilder sb, string typeName, bool isStringBacked)
    {
        sb.AppendLine("// ─── Comparison ──────────────────────────────────────────────────");
        sb.AppendLine();

        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// Compares this instance with another. Default instances order before non-default instances.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine($"public int CompareTo({typeName} other)");
        sb.OpenBrace();
        sb.AppendLine("if (IsDefault && other.IsDefault) return 0;");
        sb.AppendLine("if (IsDefault) return -1;");
        sb.AppendLine("if (other.IsDefault) return 1;");
        if (isStringBacked)
            sb.AppendLine("return string.Compare(_value, other._value, StringComparison.Ordinal);");
        else
            sb.AppendLine("return _value.CompareTo(other._value);");
        sb.CloseBrace();
        sb.AppendLine();

        sb.AppendLine("public int CompareTo(object? obj) => obj switch");
        sb.OpenBrace();
        sb.AppendLine("null => 1,");
        sb.AppendLine($"{typeName} other => CompareTo(other),");
        sb.AppendLine($"_ => throw new ArgumentException($\"Object must be of type {typeName}.\")");
        sb.CloseBrace(";");
        sb.AppendLine();

        sb.AppendLine($"public static bool operator <({typeName} left, {typeName} right) => left.CompareTo(right) < 0;");
        sb.AppendLine($"public static bool operator <=({typeName} left, {typeName} right) => left.CompareTo(right) <= 0;");
        sb.AppendLine($"public static bool operator >({typeName} left, {typeName} right) => left.CompareTo(right) > 0;");
        sb.AppendLine($"public static bool operator >=({typeName} left, {typeName} right) => left.CompareTo(right) >= 0;");
        sb.AppendLine();
    }
}

