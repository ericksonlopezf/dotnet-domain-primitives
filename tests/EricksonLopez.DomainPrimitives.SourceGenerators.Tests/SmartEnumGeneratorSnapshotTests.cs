// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.DomainPrimitives.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VerifyXunit;
using Xunit;

namespace EricksonLopez.DomainPrimitives.Generators.Tests
{
    public class SmartEnumGeneratorSnapshotTests
    {
        [Fact]
        public Task GeneratesSmartEnumCorrectly()
        {
            string source = @"
using EricksonLopez.DomainPrimitives;

namespace TestNamespace
{
    [SmartEnum<int>]
    public readonly partial record struct OrderStatus
    {
        public OrderStatus(int value, string name) { }
        public static readonly OrderStatus Pending = new(1, nameof(Pending));
        public static readonly OrderStatus Processing = new(2, nameof(Processing));
    }
}

namespace EricksonLopez.DomainPrimitives
{
    public class SmartEnumAttribute<TValue> : System.Attribute { }
}
";

            var syntaxTree = CSharpSyntaxTree.ParseText(EricksonLopez.DomainPrimitives.SourceGenerators.Tests.TestCompilationHelper.EnsureUsings(source), new CSharpParseOptions(LanguageVersion.CSharp11));
            var compilation = CSharpCompilation.Create(
                assemblyName: "Tests",
                syntaxTrees: new[] { syntaxTree },
                references: Basic.Reference.Assemblies.Net80.References.All,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var compDiagnostics = compilation.GetDiagnostics();
            Assert.Empty(compDiagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));

            var generator = new SmartEnumGenerator();
            var driver = (GeneratorDriver)CSharpGeneratorDriver.Create(generator);
            driver = driver.RunGenerators(compilation);
            
            var runResult = driver.GetRunResult();
            var diagnostics = runResult.Diagnostics;
            Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
            Assert.NotEmpty(runResult.GeneratedTrees);

            return Verifier.Verify(driver).UseDirectory("Snapshots");
        }

        [Fact]
        public Task GeneratesSmartEnum_StringValueCorrectly()
        {
            string source = @"
namespace TestNamespace
{
    [EricksonLopez.DomainPrimitives.SmartEnum<string>]
    public readonly partial record struct Role
    {
        public Role(string value, string name) { }
        public static readonly Role Admin = new(""admin"", nameof(Admin));
        public static readonly Role User = new(""user"", nameof(User));
    }
}

namespace EricksonLopez.DomainPrimitives
{
    public class SmartEnumAttribute<TValue> : System.Attribute { }
}
";

            var syntaxTree = CSharpSyntaxTree.ParseText(EricksonLopez.DomainPrimitives.SourceGenerators.Tests.TestCompilationHelper.EnsureUsings(source), new CSharpParseOptions(LanguageVersion.CSharp11));
            var compilation = CSharpCompilation.Create(
                assemblyName: "Tests",
                syntaxTrees: new[] { syntaxTree },
                references: Basic.Reference.Assemblies.Net80.References.All,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var generator = new SmartEnumGenerator();
            var driver = (GeneratorDriver)CSharpGeneratorDriver.Create(generator);
            driver = driver.RunGenerators(compilation);
            var runResult = driver.GetRunResult();
            var diagnostics = runResult.Diagnostics;
            Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
            Assert.NotEmpty(runResult.GeneratedTrees);

            return Verifier.Verify(driver).UseDirectory("Snapshots");
        }
        [Fact]
        public Task GeneratesNestedTypeCorrectly()
        {
            string source = @"
namespace TestNamespace
{
    public partial class OuterClass
    {
        public partial class InnerClass
        {
            [EricksonLopez.DomainPrimitives.SmartEnum<int>]
            public readonly partial record struct NestedPrimitive;
        }
    }
}
namespace EricksonLopez.DomainPrimitives
{
    public class SmartEnumAttribute<T> : System.Attribute {}
}
";
            var syntaxTree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(EricksonLopez.DomainPrimitives.SourceGenerators.Tests.TestCompilationHelper.EnsureUsings(source), new CSharpParseOptions(LanguageVersion.CSharp11));
            var compilation = Microsoft.CodeAnalysis.CSharp.CSharpCompilation.Create("Tests", new[] { syntaxTree }, Basic.Reference.Assemblies.Net80.References.All, new Microsoft.CodeAnalysis.CSharp.CSharpCompilationOptions(Microsoft.CodeAnalysis.OutputKind.DynamicallyLinkedLibrary));
            var compDiagnostics = compilation.GetDiagnostics();
            Assert.Empty(compDiagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
            var generator = new EricksonLopez.DomainPrimitives.Generators.SmartEnumGenerator();
            Microsoft.CodeAnalysis.GeneratorDriver driver = Microsoft.CodeAnalysis.CSharp.CSharpGeneratorDriver.Create(generator);
            driver = driver.RunGenerators(compilation);
            var runResult = driver.GetRunResult();
            Assert.NotEmpty(runResult.GeneratedTrees);
            return VerifyXunit.Verifier.Verify(driver).UseDirectory("Snapshots");
        }
    }
}




