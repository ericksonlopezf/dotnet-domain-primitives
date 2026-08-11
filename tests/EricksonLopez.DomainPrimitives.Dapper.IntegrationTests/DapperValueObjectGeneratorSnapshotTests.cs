using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VerifyXunit;
using Xunit;
using EricksonLopez.DomainPrimitives.Dapper.SourceGenerators;

namespace EricksonLopez.DomainPrimitives.Dapper.Tests
{
    [UsesVerify]
    public class DapperValueObjectGeneratorSnapshotTests
    {
        [Fact]
        public Task GeneratesCompoundValueObjectCorrectly()
        {
            // The source code to test
            string source = @"
using System;
using EricksonLopez.DomainPrimitives;

namespace TestNamespace
{
    [ValueObject]
    public readonly partial record struct Address(string Street, string City, string ZipCode);
}

namespace EricksonLopez.DomainPrimitives
{
    public class ValueObjectAttribute : System.Attribute {}
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

            // Create an instance of our DapperValueObjectGenerator incremental source generator
            var generator = new DapperValueObjectGenerator();

            // The GeneratorDriver is used to run our generator against a compilation
            var driver = (GeneratorDriver)CSharpGeneratorDriver.Create(generator);

            // Run the source generator!
            driver = driver.RunGenerators(compilation);

            

            // Use verify to snapshot test the source generator output!
            return Verifier.Verify(driver).UseDirectory("Snapshots");
        }
    }
}
