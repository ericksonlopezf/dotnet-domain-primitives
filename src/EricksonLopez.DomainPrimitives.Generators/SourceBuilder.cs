using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace EricksonLopez.DomainPrimitives.Generators;

/// <summary>
/// Helper for generating well-formatted C# source code with proper indentation.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class SourceBuilder
{
    private readonly StringBuilder _sb = new(4096);
    private int _indent;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SourceBuilder AppendLine() { _sb.AppendLine(); return this; }

    public SourceBuilder AppendLine(string line)
    {
        if (line.Length == 0) { _sb.AppendLine(); return this; }
        _sb.Append(new string(' ', _indent * 4));
        _sb.AppendLine(line);
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SourceBuilder Append(string text)
    {
        _sb.Append(text);
        return this;
    }

    public SourceBuilder AppendIndented(string text)
    {
        _sb.Append(new string(' ', _indent * 4));
        _sb.Append(text);
        return this;
    }

    public SourceBuilder OpenBrace()
    {
        AppendLine("{");
        _indent++;
        return this;
    }

    public SourceBuilder CloseBrace(string suffix = "")
    {
        _indent = System.Math.Max(0, _indent - 1);
        AppendLine("}" + suffix);
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SourceBuilder IncreaseIndent() { _indent++; return this; }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SourceBuilder DecreaseIndent() { _indent = System.Math.Max(0, _indent - 1); return this; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => _sb.ToString();
}


