// Copyright © Erickson Lopez. MIT License.
using System;
using System.Linq;
using AwesomeAssertions;
using EricksonLopez.DomainPrimitives.Dapper.SourceGenerators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace EricksonLopez.DomainPrimitives.Dapper.SourceGenerators.Tests;

/// <summary>
/// Unit tests for <see cref="JsonConverterGenerator"/>.
/// Verifies that the generator emits the correct JsonConverter and registration code
/// for each supported backing type and discovery mechanism.
/// </summary>
public sealed class JsonConverterGeneratorTests
{
    private static Compilation CreateCompilation(string source)
    {
        const string dummyInfrastructure = @"
namespace EricksonLopez.DomainPrimitives
{
    public interface IEntityId
    {
        System.Guid Value { get; }
    }
    public interface IEntityId<TSelf> where TSelf : IEntityId<TSelf>
    {
        System.Guid Value { get; }
        static abstract TSelf Create(System.Guid value);
    }
    public interface IStrongId<TSelf, TValue>
    {
        TValue Value { get; }
        static abstract TSelf Create(TValue value);
    }
    public class StrongIdAttribute<T> : System.Attribute { }
    public class DomainPrimitivesDefaultsAttribute : System.Attribute { }
}";

        var syntaxTrees = new[]
        {
            CSharpSyntaxTree.ParseText(source),
            CSharpSyntaxTree.ParseText(dummyInfrastructure),
        };

        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .Cast<MetadataReference>()
            .ToArray();

        return CSharpCompilation.Create(
            "compilation",
            syntaxTrees,
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }

    private static string RunGenerator(string source)
    {
        var compilation = CreateCompilation(source);
        var generator = new JsonConverterGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        return string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
    }

    [Fact]
    public void Generator_WithGuidBackedIEntityId_ShouldGenerateJsonConverter()
    {
        const string source = @"
namespace TestNamespace;
public readonly record struct OrderId(System.Guid Value)
    : EricksonLopez.DomainPrimitives.IEntityId<OrderId>
{
    public static OrderId Create(System.Guid value) => new(value);
    public bool IsEmpty => Value == System.Guid.Empty;
}";

        var generatedSource = RunGenerator(source);

        generatedSource.Should().Contain("OrderIdJsonConverter");
        generatedSource.Should().Contain("JsonConverter<global::TestNamespace.OrderId>");
        generatedSource.Should().Contain("reader.GetGuid()");
        generatedSource.Should().Contain("writer.WriteStringValue(value.Value)");
        generatedSource.Should().Contain("global::TestNamespace.OrderId.Create(");
    }

    [Fact]
    public void Generator_WithConventionBasedGuidId_ShouldGenerateJsonConverter()
    {
        const string source = @"
namespace TestNamespace;
public readonly record struct CustomerId(System.Guid Value)
{
    public static CustomerId Create(System.Guid value) => new(value);
}";

        var generatedSource = RunGenerator(source);

        generatedSource.Should().Contain("CustomerIdJsonConverter");
        generatedSource.Should().Contain("reader.GetGuid()");
    }

    [Fact]
    public void Generator_WithStringBackedId_ShouldEmitGetStringRead()
    {
        // Convention-based: must end in 'Id', have a Value property, and static Create(string)
        const string source = @"
namespace TestNamespace;
public readonly record struct CountryCodeId(string Value)
{
    public static CountryCodeId Create(string value) => new(value);
}";

        var generatedSource = RunGenerator(source);

        generatedSource.Should().Contain("CountryCodeIdJsonConverter");
        generatedSource.Should().Contain("reader.GetString()");
        generatedSource.Should().Contain("writer.WriteStringValue(value.Value)");
    }

    [Fact]
    public void Generator_WithIntBackedId_ShouldEmitGetInt32Read()
    {
        const string source = @"
namespace TestNamespace;
public readonly record struct SequenceId(int Value)
{
    public static SequenceId Create(int value) => new(value);
}";

        var generatedSource = RunGenerator(source);

        generatedSource.Should().Contain("SequenceIdJsonConverter");
        generatedSource.Should().Contain("reader.GetInt32()");
        generatedSource.Should().Contain("writer.WriteNumberValue(value.Value)");
    }

    [Fact]
    public void Generator_WithLongBackedId_ShouldEmitGetInt64Read()
    {
        const string source = @"
namespace TestNamespace;
public readonly record struct TraceId(long Value)
{
    public static TraceId Create(long value) => new(value);
}";

        var generatedSource = RunGenerator(source);

        generatedSource.Should().Contain("TraceIdJsonConverter");
        generatedSource.Should().Contain("reader.GetInt64()");
        generatedSource.Should().Contain("writer.WriteNumberValue(value.Value)");
    }

    [Fact]
    public void Generator_WithDecimalBackedId_ShouldEmitGetDecimalRead()
    {
        const string source = @"
namespace TestNamespace;
public readonly record struct AmountId(decimal Value)
{
    public static AmountId Create(decimal value) => new(value);
}";

        var generatedSource = RunGenerator(source);

        generatedSource.Should().Contain("AmountIdJsonConverter");
        generatedSource.Should().Contain("reader.GetDecimal()");
        generatedSource.Should().Contain("writer.WriteNumberValue(value.Value)");
    }

    [Fact]
    public void Generator_WithMultipleIds_ShouldEmitRegistrationClass()
    {
        const string source = @"
namespace TestNamespace;
public readonly record struct SaleId(System.Guid Value)
{
    public static SaleId Create(System.Guid value) => new(value);
}
public readonly record struct InvoiceId(System.Guid Value)
{
    public static InvoiceId Create(System.Guid value) => new(value);
}";

        var generatedSource = RunGenerator(source);

        generatedSource.Should().Contain("DomainPrimitivesJsonRegistration");
        generatedSource.Should().Contain("RegisterAll");
        generatedSource.Should().Contain("new SaleIdJsonConverter()");
        generatedSource.Should().Contain("new InvoiceIdJsonConverter()");
    }

    [Fact]
    public void Generator_GeneratedConverter_ShouldNotContainReflectionCalls()
    {
        const string source = @"
namespace TestNamespace;
public readonly record struct ProductId(System.Guid Value)
{
    public static ProductId Create(System.Guid value) => new(value);
}";

        var generatedSource = RunGenerator(source);

        generatedSource.Should().NotContain("GetType()");
        generatedSource.Should().NotContain("Activator.CreateInstance");
        generatedSource.Should().NotContain("BindingFlags");
    }

    [Fact]
    public void Generator_WithGuidBackedId_ShouldEmitNullCheck()
    {
        const string source = @"
namespace TestNamespace;
public readonly record struct TenantId(System.Guid Value)
{
    public static TenantId Create(System.Guid value) => new(value);
}";

        var generatedSource = RunGenerator(source);

        generatedSource.Should().Contain("JsonTokenType.Null");
        generatedSource.Should().Contain("throw new JsonException");
    }

    [Fact]
    public void Generator_WithNonIdConventionType_ShouldNotGenerate()
    {
        const string source = @"
namespace TestNamespace;
public readonly record struct Money(decimal Amount)
{
    public static Money Create(decimal value) => new(value);
}";

        var generatedSource = RunGenerator(source);

        generatedSource.Should().NotContain("MoneyJsonConverter");
    }

    [Fact]
    public void Generator_GeneratedClasses_ShouldBeDecoratedWithExcludeFromCodeCoverage()
    {
        const string source = @"
namespace TestNamespace;
public readonly record struct CompanyId(System.Guid Value)
{
    public static CompanyId Create(System.Guid value) => new(value);
}";

        var generatedSource = RunGenerator(source);

        generatedSource.Should().Contain("ExcludeFromCodeCoverage");
    }

    [Fact]
    public void Generator_RegistrationClass_ShouldUseLockForThreadSafety()
    {
        const string source = @"
namespace TestNamespace;
public readonly record struct UserId(System.Guid Value)
{
    public static UserId Create(System.Guid value) => new(value);
}";

        var generatedSource = RunGenerator(source);

        generatedSource.Should().Contain("_lock");
        generatedSource.Should().Contain("lock (");
        generatedSource.Should().Contain("_registered");
    }

    [Fact]
    public void Generator_GeneratedCode_ShouldBeInCorrectNamespace()
    {
        const string source = @"
namespace TestNamespace;
public readonly record struct ItemId(System.Guid Value)
{
    public static ItemId Create(System.Guid value) => new(value);
}";

        var generatedSource = RunGenerator(source);

        generatedSource.Should().Contain("namespace EricksonLopez.DomainPrimitives.Dapper.Generated");
    }

    [Fact]
    public void Generator_GeneratedCode_ShouldHaveAutoGeneratedHeader()
    {
        const string source = @"
namespace TestNamespace;
public readonly record struct CatalogId(System.Guid Value)
{
    public static CatalogId Create(System.Guid value) => new(value);
}";

        var generatedSource = RunGenerator(source);

        generatedSource.Should().Contain("// <auto-generated/>");
    }

    [Fact]
    public void Generator_WithNoEligibleTypes_ShouldNotEmitAnyOutput()
    {
        const string source = @"
namespace TestNamespace;
public class MyService { }";

        var compilation = CreateCompilation(source);
        var generator = new JsonConverterGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        outputCompilation.SyntaxTrees.Count().Should().Be(2);
    }

    [Fact]
    public void Generator_WithNamespaceCollision_EmitsSafeNamespacePrefixedConverter()
    {
        const string source = @"
namespace NS1
{
    public readonly record struct ProductId(System.Guid Value)
    {
        public static ProductId Create(System.Guid value) => new(value);
    }
}
namespace NS2
{
    public readonly record struct ProductId(System.Guid Value)
    {
        public static ProductId Create(System.Guid value) => new(value);
    }
}";
        var generatedSource = RunGenerator(source);

        generatedSource.Should().Contain("class ProductIdJsonConverter");
        generatedSource.Should().Contain("class NS2_ProductIdJsonConverter");
    }

    [Fact]
    public void Generator_WithStrongIdInterface_EmitsCorrectConverter()
    {
        const string source = @"
namespace TestNamespace;
public readonly record struct LongId(long Value) : EricksonLopez.DomainPrimitives.IStrongId<LongId, long>
{
    public static LongId Create(long value) => new(value);
}";
        var generatedSource = RunGenerator(source);

        generatedSource.Should().Contain("LongIdJsonConverter");
        generatedSource.Should().Contain("reader.GetInt64()");
    }

    [Fact]
    public void Generator_WithNonGenericIEntityId_ShouldGenerateJsonConverter()
    {
        const string source = @"
namespace TestNamespace;
public readonly record struct LegacyId(System.Guid Value) : EricksonLopez.DomainPrimitives.IEntityId
{
    public static LegacyId Create(System.Guid value) => new(value);
}";
        var generatedSource = RunGenerator(source);

        generatedSource.Should().Contain("LegacyIdJsonConverter");
        generatedSource.Should().Contain("reader.GetGuid()");
    }

    [Fact]
    public void Generator_WithGlobalNamespacePrimitive_EmitsGlobalPrefix()
    {
        const string source = @"
[EricksonLopez.DomainPrimitives.StrongIdAttribute<System.Guid>]
public readonly record struct RootGlobalId(System.Guid Value)
{
    public static RootGlobalId Create(System.Guid value) => new(value);
}";
        var compilation = CreateCompilation(source);
        var generator = new JsonConverterGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedTrees = outputCompilation.SyntaxTrees.Skip(2).ToList();
        var generatedSource = string.Join(Environment.NewLine, generatedTrees.Select(t => t.ToString()));
        generatedSource.Should().NotContain("using <global namespace>;");
        generatedSource.Should().Contain("internal sealed class RootGlobalIdJsonConverter : JsonConverter<RootGlobalId>");
    }

    [Fact]
    public void Generator_WithDuplicateSymbolAcrossPartialDeclarations_HandlesDeduplication()
    {
        const string source = @"
namespace ModuleA
{
    public readonly partial record struct OrderId(System.Guid Value)
    {
        public static OrderId Create(System.Guid value) => new(value);
    }

    public readonly partial record struct OrderId
    {
    }
}";
        var compilation = CreateCompilation(source);
        var generator = new JsonConverterGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedTrees = outputCompilation.SyntaxTrees.Skip(2).ToList();
        generatedTrees.Count(t => t.ToString().Contains("class OrderIdJsonConverter", StringComparison.Ordinal)).Should().Be(1);
    }

    [Fact]
    public void Generator_WithClassInsteadOfStruct_IsIgnored()
    {
        const string source = @"
namespace TestNamespace;
public class ClassId : EricksonLopez.DomainPrimitives.IStrongId<ClassId, long>
{
    public long Value { get; set; }
    public static ClassId Create(long value) => new();
}";
        var generatedTrees = RunGenerator(source);
        generatedTrees.Should().NotContain("ClassIdJsonConverter");
    }

    [Fact]
    public void Generator_WithNonMatchingInterface_IsIgnored()
    {
        const string source = @"
namespace TestNamespace
{
    public interface IOtherInterface<T1, T2> { }
    public readonly record struct NonIdStruct(long Value) : IOtherInterface<NonIdStruct, long>
    {
    }
}";
        var generatedTrees = RunGenerator(source);
        generatedTrees.Should().NotContain("NonIdStructJsonConverter");
    }

    [Fact]
    public void Generator_WithIdEndingWithoutCreate_IsIgnored()
    {
        const string source = @"
namespace TestNamespace;
public readonly record struct NoCreateId(long Value);
";
        var generatedTrees = RunGenerator(source);
        generatedTrees.Should().NotContain("NoCreateIdJsonConverter");
    }
}
