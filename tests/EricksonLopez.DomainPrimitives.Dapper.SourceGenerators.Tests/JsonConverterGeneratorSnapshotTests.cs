// Copyright © Erickson Lopez. MIT License.
using System.Threading.Tasks;
using EricksonLopez.DomainPrimitives.Dapper.SourceGenerators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VerifyXunit;
using Xunit;

namespace EricksonLopez.DomainPrimitives.Dapper.Tests;

/// <summary>
/// Snapshot tests for <see cref="JsonConverterGenerator"/>.
/// Captures the exact generated output so regressions are caught immediately.
/// To update snapshots: delete the .verified.txt files and re-run the tests.
/// </summary>
public sealed class JsonConverterGeneratorSnapshotTests
{
    [Fact]
    public Task GeneratesGuidBackedJsonConverterCorrectly()
    {
        const string source = @"
using EricksonLopez.DomainPrimitives;

namespace TestNamespace
{
    public readonly record struct OrderId(System.Guid Value)
        : IEntityId<OrderId>
    {
        public static OrderId Create(System.Guid value) => new(value);
        public bool IsEmpty => Value == System.Guid.Empty;
    }
}

namespace EricksonLopez.DomainPrimitives
{
    public interface IEntityId<TSelf>
    {
        System.Guid Value { get; }
        static abstract TSelf Create(System.Guid value);
        bool IsEmpty { get; }
    }
}";

        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        var compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: new[] { syntaxTree },
            references: Basic.Reference.Assemblies.Net80.References.All,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var generator = new JsonConverterGenerator();
        var driver = (GeneratorDriver)CSharpGeneratorDriver.Create(generator);
        driver = driver.RunGenerators(compilation);

        return Verifier.Verify(driver).UseDirectory("Snapshots");
    }

    [Fact]
    public Task GeneratesMultiTypeRegistrationCorrectly()
    {
        const string source = @"
namespace TestNamespace
{
    public readonly record struct SaleId(System.Guid Value)
    {
        public static SaleId Create(System.Guid value) => new(value);
    }
    public readonly record struct InvoiceId(System.Guid Value)
    {
        public static InvoiceId Create(System.Guid value) => new(value);
    }
}";

        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        var compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: new[] { syntaxTree },
            references: Basic.Reference.Assemblies.Net80.References.All,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var generator = new JsonConverterGenerator();
        var driver = (GeneratorDriver)CSharpGeneratorDriver.Create(generator);
        driver = driver.RunGenerators(compilation);

        return Verifier.Verify(driver).UseDirectory("Snapshots");
    }
}
