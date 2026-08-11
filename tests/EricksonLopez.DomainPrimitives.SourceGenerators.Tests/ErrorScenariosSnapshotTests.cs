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
    public class ErrorScenariosSnapshotTests
    {
        [Fact]
        public Task ErrorScenarios_TriggerDiagnostics()
        {
            string source = @"
using EricksonLopez.DomainPrimitives;
using EricksonLopez.DomainPrimitives.Normalization;

[StringPrimitive]
public readonly record struct GlobalNamespaceString;

namespace TestNamespace
{
    [StringPrimitive]
    public readonly record struct MissingPartial;

    [StringPrimitive]
    public class NotAStruct {}

    [StringPrimitive]
    [LowerCase]
    [UpperCase]
    public readonly partial record struct ConflictingNormalization;
}

namespace EricksonLopez.DomainPrimitives
{
    public class StringPrimitiveAttribute : System.Attribute {}
}
namespace EricksonLopez.DomainPrimitives.Normalization
{
    public class LowerCaseAttribute : System.Attribute {}
    public class UpperCaseAttribute : System.Attribute {}
}
";

            var syntaxTree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.CSharp11));
            var compilation = CSharpCompilation.Create(
                assemblyName: "Tests",
                syntaxTrees: new[] { syntaxTree },
                references: Basic.Reference.Assemblies.Net80.References.All,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var generator = new StringPrimitiveGenerator();
            var driver = (GeneratorDriver)CSharpGeneratorDriver.Create(generator);
            driver = driver.RunGenerators(compilation);

            return Verifier.Verify(driver).UseDirectory("Snapshots");
        }
    }
}

