// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives.SourceGenerators.Tests;

internal static class TestCompilationHelper
{
    public static string EnsureUsings(string source)
    {
        var usings = "";
        if (!source.Contains("using System;")) usings += "using System;\n";
        if (!source.Contains("using EricksonLopez.DomainPrimitives;")) usings += "using EricksonLopez.DomainPrimitives;\n";
        return usings.Length > 0 ? usings + source : source;
    }
}
