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
    private static void GenerateFormatting(SourceBuilder sb, StringPrimitiveTypeInfo info)
    {
        sb.AppendLine("// ─── Formatting (IFormattable, ISpanFormattable, IUtf8SpanFormattable) ───");
        sb.AppendLine();

        sb.AppendLine("public void Deconstruct(out string value) => value = _value;");
        sb.AppendLine();
        sb.AppendLine("public override string ToString() => _value ?? string.Empty;");
        sb.AppendLine();

        sb.AppendLine("public string ToString(string? format, IFormatProvider? formatProvider)");
        sb.AppendLine("    => _value ?? string.Empty;");
        sb.AppendLine();

        // ISpanFormattable
        sb.AppendLine("public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)");
        sb.OpenBrace();
        sb.AppendLine("if (_value is null) { charsWritten = 0; return false; }");
        sb.AppendLine("var v = _value.AsSpan();");
        sb.AppendLine("if (v.TryCopyTo(destination))");
        sb.OpenBrace();
        sb.AppendLine("charsWritten = v.Length;");
        sb.AppendLine("return true;");
        sb.CloseBrace();
        sb.AppendLine("charsWritten = 0;");
        sb.AppendLine("return false;");
        sb.CloseBrace();
        sb.AppendLine();

        // IUtf8SpanFormattable
        sb.AppendLine("#if NET8_0_OR_GREATER");
        sb.AppendLine("public bool TryFormat(Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)");
        sb.OpenBrace();
        sb.AppendLine("if (_value is null) { bytesWritten = 0; return false; }");
        sb.AppendLine("return System.Text.Encoding.UTF8.TryGetBytes(_value.AsSpan(), utf8Destination, out bytesWritten);");
        sb.CloseBrace();
        sb.AppendLine("#endif");
        sb.AppendLine();
    }

}
