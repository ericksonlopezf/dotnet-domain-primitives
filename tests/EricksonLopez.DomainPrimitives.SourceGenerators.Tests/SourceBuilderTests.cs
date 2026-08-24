// Copyright © Erickson Lopez. MIT License.
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
using AwesomeAssertions;
using EricksonLopez.DomainPrimitives.Generators;

namespace EricksonLopez.DomainPrimitives.Generators.Tests
{
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

        [Fact]
        public void SourceBuilder_AdditionalMethods_WorkCorrectly()
        {
            var sbEmpty = new SourceBuilder();
            sbEmpty.AppendLine("");
            sbEmpty.ToString().Should().Be(Environment.NewLine);

            var sb = new SourceBuilder();
            sb.AppendLine();
            sb.AppendLine("");
            sb.Append("text");
            sb.IncreaseIndent();
            sb.AppendIndented("indented");
            sb.CloseBrace(";");

            var output = sb.ToString();
            output.Should().Contain("text");
            output.Should().Contain("    indented");
            output.Should().Contain("};");
        }
    }
}


