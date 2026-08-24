// Copyright © Erickson Lopez. MIT License.
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VerifyXunit;
using Xunit;
using EricksonLopez.DomainPrimitives.Generators;
using System.Linq;

namespace EricksonLopez.DomainPrimitives.Generators.Tests;

public class StrongIdGeneratorSnapshotTests
{
    [Fact]
    public Task GeneratesStrongIdCorrectly()
    {
        var source = @"
                    using System;
                    using EricksonLopez.DomainPrimitives;

                    namespace TestNamespace
                    {
                        [StrongId<string>]
                        public readonly partial record struct UserId;
                    }

                    namespace EricksonLopez.DomainPrimitives
                    {
                        public class StrongIdAttribute<TValue> : System.Attribute {}
                    }
                    ";

        // Parse the provided string into a C# syntax tree
        var syntaxTree = CSharpSyntaxTree.ParseText(EricksonLopez.DomainPrimitives.SourceGenerators.Tests.TestCompilationHelper.EnsureUsings(source), new CSharpParseOptions(LanguageVersion.CSharp11));

        // Create a Roslyn compilation for the syntax tree.
        var compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: new[] { syntaxTree },
            references: Basic.Reference.Assemblies.Net80.References.All,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        // Create an instance of our EnumGenerator incremental source generator
        var generator = new StrongIdGenerator();

        // The GeneratorDriver is used to run our generator against a compilation
        var driver = (GeneratorDriver)CSharpGeneratorDriver.Create(generator);

        // Run the source generator!
        driver = driver.RunGenerators(compilation);

        // Use verify to snapshot test the source generator output!
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
            [StrongId<int>]
            public readonly partial record struct NestedPrimitive;
        }
    }
}
namespace EricksonLopez.DomainPrimitives
{
    public class StrongIdAttribute<T> : System.Attribute {}
}
";
        var syntaxTree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(EricksonLopez.DomainPrimitives.SourceGenerators.Tests.TestCompilationHelper.EnsureUsings(source), new CSharpParseOptions(LanguageVersion.CSharp11));
        var compilation = Microsoft.CodeAnalysis.CSharp.CSharpCompilation.Create("Tests", new[] { syntaxTree }, Basic.Reference.Assemblies.Net80.References.All, new Microsoft.CodeAnalysis.CSharp.CSharpCompilationOptions(Microsoft.CodeAnalysis.OutputKind.DynamicallyLinkedLibrary));
        var generator = new EricksonLopez.DomainPrimitives.Generators.StrongIdGenerator();
        Microsoft.CodeAnalysis.GeneratorDriver driver = Microsoft.CodeAnalysis.CSharp.CSharpGeneratorDriver.Create(generator);
        driver = driver.RunGenerators(compilation);
        return VerifyXunit.Verifier.Verify(driver).UseDirectory("Snapshots");
    }
}


