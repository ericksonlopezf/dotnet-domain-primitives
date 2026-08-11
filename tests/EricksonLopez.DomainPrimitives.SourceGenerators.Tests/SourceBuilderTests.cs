using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VerifyXunit;
using Xunit;
using EricksonLopez.DomainPrimitives.Generators;

namespace EricksonLopez.DomainPrimitives.Generators.Tests
{
    [UsesVerify]
    public class SourceBuilderTests
    {
        [Fact]
        public Task SourceBuilder_FormatCorrectly()
        {
            var sb = new SourceBuilder();
            sb.AppendLine("public class Test");
            sb.OpenBrace();
            sb.AppendLine("public void Run()");
            sb.OpenBrace();
            sb.AppendLine("System.Console.WriteLine(\"Hello\");");
            sb.CloseBrace();
            sb.DecreaseIndent(); // Test explicit DecreaseIndent when not needed just to cover branch
            sb.DecreaseIndent(); // Test underflow
            sb.DecreaseIndent();
            sb.IncreaseIndent();
            sb.AppendLine("// end");
            sb.CloseBrace(); // Test close brace after underflow
            
            return Verifier.Verify(sb.ToString());
        }
    }
}

