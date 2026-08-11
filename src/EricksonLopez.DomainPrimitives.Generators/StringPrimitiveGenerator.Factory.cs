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
        sb.AppendLine("value = Normalize(value);");
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
