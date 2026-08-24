// Copyright © Erickson Lopez. MIT License.
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.DomainPrimitives.Dapper.SourceGenerators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VerifyXunit;
using Xunit;

namespace EricksonLopez.DomainPrimitives.Dapper.Tests
{
    public class DapperTypeHandlerGeneratorSnapshotTests
    {
        [Fact]
        public Task GeneratesTypeHandlerCorrectly()
        {
            // The source code to test
            string source = @"
using EricksonLopez.DomainPrimitives;

namespace TestNamespace
{
    [StrongId<Guid>]
    public readonly partial record struct OrderId;
}

namespace EricksonLopez.DomainPrimitives
{
    public class StrongIdAttribute<TValue> : System.Attribute {}
    public class DapperAttribute : System.Attribute {}
}
";

            // Parse the provided string into a C# syntax tree
            var syntaxTree = CSharpSyntaxTree.ParseText(source);

            // Create a Roslyn compilation for the syntax tree.
            var compilation = CSharpCompilation.Create(
                assemblyName: "Tests",
                syntaxTrees: new[] { syntaxTree },
                references: Basic.Reference.Assemblies.Net80.References.All,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            // Create an instance of our DapperTypeHandlerGenerator incremental source generator
            var generator = new DapperTypeHandlerGenerator();

            // The GeneratorDriver is used to run our generator against a compilation
            var driver = (GeneratorDriver)CSharpGeneratorDriver.Create(generator);

            // Run the source generator!
            driver = driver.RunGenerators(compilation);

            // Use verify to snapshot test the source generator output!
            return Verifier.Verify(driver).UseDirectory("Snapshots");
        }
    }
}



