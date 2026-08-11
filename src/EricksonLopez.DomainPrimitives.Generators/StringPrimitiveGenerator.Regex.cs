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
    private static void GenerateRegexFields(SourceBuilder sb, StringPrimitiveTypeInfo info)
    {
        if (info.RegexPatterns.Length == 0 && !info.NormalizeWhitespace) return;

        sb.AppendLine("// ─── Regex Patterns (source-generated, NativeAOT-safe) ────────────");
        sb.AppendLine();

        for (int i = 0; i < info.RegexPatterns.Length; i++)
        {
            var regex = info.RegexPatterns[i];
            var methodName = info.RegexPatterns.Length == 1 ? "ValidationRegex" : $"ValidationRegex{i + 1}";
            // Source generators cannot emit [GeneratedRegex] because the RegexGenerator won't run on generated code.
            // Using a static readonly instance instead with Compiled and timeout for ReDoS mitigation.
            sb.AppendLine("#if NET7_0_OR_GREATER");
            sb.AppendLine($"private static readonly System.Text.RegularExpressions.Regex {methodName} = new System.Text.RegularExpressions.Regex(@\"{EscapeVerbatimString(regex.Pattern)}\", System.Text.RegularExpressions.RegexOptions.CultureInvariant | System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.NonBacktracking, TimeSpan.FromMilliseconds(100));");
            sb.AppendLine("#else");
            sb.AppendLine($"private static readonly System.Text.RegularExpressions.Regex {methodName} = new System.Text.RegularExpressions.Regex(@\"{EscapeVerbatimString(regex.Pattern)}\", System.Text.RegularExpressions.RegexOptions.CultureInvariant | System.Text.RegularExpressions.RegexOptions.Compiled, TimeSpan.FromMilliseconds(100));");
            sb.AppendLine("#endif");
        }

        if (info.NormalizeWhitespace)
        {
            sb.AppendLine("#if NET7_0_OR_GREATER");
            sb.AppendLine($"private static readonly System.Text.RegularExpressions.Regex WhitespaceRegex = new System.Text.RegularExpressions.Regex(@\"\\s+\", System.Text.RegularExpressions.RegexOptions.CultureInvariant | System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.NonBacktracking, TimeSpan.FromMilliseconds(100));");
            sb.AppendLine("#else");
            sb.AppendLine($"private static readonly System.Text.RegularExpressions.Regex WhitespaceRegex = new System.Text.RegularExpressions.Regex(@\"\\s+\", System.Text.RegularExpressions.RegexOptions.CultureInvariant | System.Text.RegularExpressions.RegexOptions.Compiled, TimeSpan.FromMilliseconds(100));");
            sb.AppendLine("#endif");
        }
        sb.AppendLine();
    }

}
