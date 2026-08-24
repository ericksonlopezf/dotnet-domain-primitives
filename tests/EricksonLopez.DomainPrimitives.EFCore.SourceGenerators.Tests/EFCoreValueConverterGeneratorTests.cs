// Copyright © Erickson Lopez. MIT License.
using System;
using System.Linq;
using AwesomeAssertions;
using EricksonLopez.DomainPrimitives.EFCore.SourceGenerators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace EricksonLopez.DomainPrimitives.EFCore.SourceGenerators.Tests;

public class EFCoreValueConverterGeneratorTests
{
    private static Compilation CreateCompilation(string source)
    {
        string dummyAttributes = @"
namespace EricksonLopez.DomainPrimitives
{
    public class EFCoreAttribute : System.Attribute { }
    public class DomainPrimitivesDefaultsAttribute : System.Attribute { }
    public class ValueObjectAttribute : System.Attribute { }
    public class StringPrimitiveAttribute : System.Attribute { }
    public class StrongIdAttribute<T> : System.Attribute { }
    public class DatePrimitiveAttribute : System.Attribute { public int Kind { get; set; } }
    public class NumericPrimitiveAttribute<T> : System.Attribute { }
    public class PercentageAttribute : System.Attribute { }
    public class SmartEnumAttribute : System.Attribute { }
    public class SmartEnumAttribute<T> : System.Attribute { }
    public class MoneyAttribute : System.Attribute { }
}
namespace EricksonLopez.DomainPrimitives.Validation
{
    public class MaxLengthAttribute : System.Attribute { public MaxLengthAttribute() {} public MaxLengthAttribute(int max) {} }
    public class LengthAttribute : System.Attribute { public LengthAttribute(int min, int max) {} }
}
namespace System
{
    public struct Guid {}
    public struct DateOnly { public static DateOnly FromDateTime(DateTime dt) => default; }
    public struct DateTime {}
    public struct TimeOnly {}
    public struct DateTimeOffset {}
}
";
        var syntaxTrees = new[] { 
            CSharpSyntaxTree.ParseText(source),
            CSharpSyntaxTree.ParseText(dummyAttributes)
        };
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .Cast<MetadataReference>()
            .ToArray();

        return CSharpCompilation.Create("compilation",
            syntaxTrees,
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }

    [Fact]
    public void Generator_WithValidPrimitive_ShouldGenerateCode()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.EFCoreAttribute]
[EricksonLopez.DomainPrimitives.StringPrimitiveAttribute]
public readonly partial struct NameId { }
";
        var compilation = CreateCompilation(source);
        var generator = new EFCoreValueConverterGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("class NameIdValueConverter : ValueConverter<NameId, string>");
        generatedSource.Should().Contain("NameId.Create(provider)");
    }
    
    [Fact]
    public void Generator_WithGuidBackedPrimitive_ShouldGenerateCode()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.EFCoreAttribute]
[EricksonLopez.DomainPrimitives.StrongIdAttribute<Guid>]
public readonly partial struct UserGuid { }
";
        var compilation = CreateCompilation(source);
        var generator = new EFCoreValueConverterGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("class UserGuidValueConverter : ValueConverter<UserGuid, Guid>");
    }

    [Fact]
    public void Generator_WithDatePrimitive_ShouldGenerateCode()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.EFCoreAttribute]
[EricksonLopez.DomainPrimitives.DatePrimitiveAttribute]
public readonly partial struct BirthDate { }
";
        var compilation = CreateCompilation(source);
        var generator = new EFCoreValueConverterGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("class BirthDateValueConverter : ValueConverter<BirthDate, global::System.DateOnly>");
    }

    [Theory]
    [InlineData(1, "global::DateTime")]
    [InlineData(2, "global::System.TimeOnly")]
    [InlineData(3, "global::System.DateTimeOffset")]
    public void Generator_WithDatePrimitiveKinds_ShouldGenerateCorrectBackingType(int kind, string expectedType)
    {
        string source = $@"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.EFCoreAttribute]
[EricksonLopez.DomainPrimitives.DatePrimitiveAttribute(Kind = {kind})]
public readonly partial struct CustomDate {{ }}
";
        var compilation = CreateCompilation(source);
        var generator = new EFCoreValueConverterGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain($"class CustomDateValueConverter : ValueConverter<CustomDate, {expectedType}>");
    }
    
    [Fact]
    public void Generator_WithSmartEnum_ShouldGenerateCode()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.EFCoreAttribute]
[EricksonLopez.DomainPrimitives.SmartEnumAttribute<string>]
public readonly partial struct Status { }
";
        var compilation = CreateCompilation(source);
        var generator = new EFCoreValueConverterGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("class StatusValueConverter : ValueConverter<Status, string>");
        generatedSource.Should().Contain("Status.FromValue(provider)");
    }

    [Fact]
    public void Generator_WithNonGenericSmartEnum_ShouldGenerateIntConverter()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.EFCoreAttribute]
[EricksonLopez.DomainPrimitives.SmartEnumAttribute]
public readonly partial struct TypeCode { }
";
        var compilation = CreateCompilation(source);
        var generator = new EFCoreValueConverterGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("class TypeCodeValueConverter : ValueConverter<TypeCode, int>");
    }

    [Fact]
    public void Generator_WithMaxLength_ShouldGenerateConfig()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.EFCoreAttribute]
[EricksonLopez.DomainPrimitives.StringPrimitiveAttribute]
[EricksonLopez.DomainPrimitives.Validation.MaxLength(50)]
public readonly partial struct Name { }
";
        var compilation = CreateCompilation(source);
        var generator = new EFCoreValueConverterGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("HaveMaxLength(50)");
    }

    [Fact]
    public void Generator_WithLength_ShouldGenerateConfig()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.EFCoreAttribute]
[EricksonLopez.DomainPrimitives.StringPrimitiveAttribute]
[EricksonLopez.DomainPrimitives.Validation.Length(10, 100)]
public readonly partial struct Summary { }
";
        var compilation = CreateCompilation(source);
        var generator = new EFCoreValueConverterGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("HaveMaxLength(100)");
    }

    [Fact]
    public void Generator_WithMoney_ShouldGeneratePrecisionAndScale()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.EFCoreAttribute]
[EricksonLopez.DomainPrimitives.MoneyAttribute]
public readonly partial struct Price { }
";
        var compilation = CreateCompilation(source);
        var generator = new EFCoreValueConverterGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("class PriceValueConverter : ValueConverter<Price, decimal>");
        generatedSource.Should().Contain("HavePrecision(18, 4)");
    }

    [Fact]
    public void Generator_WithPercentage_ShouldGeneratePrecisionAndScale()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.EFCoreAttribute]
[EricksonLopez.DomainPrimitives.PercentageAttribute]
public readonly partial struct Discount { }
";
        var compilation = CreateCompilation(source);
        var generator = new EFCoreValueConverterGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("class DiscountValueConverter : ValueConverter<Discount, decimal>");
        generatedSource.Should().Contain("HavePrecision(5, 2)");
    }

    [Fact]
    public void Generator_WithNumericPrimitiveGeneric_ShouldGenerateValueConverter()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.NumericPrimitiveAttribute<long>]
public readonly partial struct ItemCount { }
";
        var compilation = CreateCompilation(source);
        var generator = new EFCoreValueConverterGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("class ItemCountValueConverter : ValueConverter<ItemCount, long>");
    }

    [Fact]
    public void Generator_WithSmartEnumGeneric_ShouldGenerateBackingType()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.SmartEnumAttribute<long>]
public readonly partial struct LongEnum { }
";
        var compilation = CreateCompilation(source);
        var generator = new EFCoreValueConverterGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("class LongEnumValueConverter : ValueConverter<LongEnum, long>");
    }

    [Fact]
    public void Generator_WithUnparameterizedMaxLength_DoesNotThrow()
    {
        string source = @"
namespace EricksonLopez.DomainPrimitives.Validation
{
    public class ParameterlessMaxLengthAttribute : System.Attribute { }
    public class CustomTwoArgAttribute : System.Attribute { public CustomTwoArgAttribute(int a, int b) {} }
}
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.StringPrimitive]
[EricksonLopez.DomainPrimitives.Validation.ParameterlessMaxLength]
[EricksonLopez.DomainPrimitives.Validation.CustomTwoArg(1, 2)]
public readonly partial struct CustomType { }
";
        var compilation = CreateCompilation(source);
        var generator = new EFCoreValueConverterGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("class CustomTypeValueConverter : ValueConverter<CustomType, string>");
        generatedSource.Should().NotContain("HaveMaxLength");
    }

    [Fact]
    public void Generator_WithParameterlessMaxLength_DoesNotThrowAndDoesNotSetMaxLength()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.StringPrimitive]
[EricksonLopez.DomainPrimitives.Validation.MaxLength]
public readonly partial struct NoArgMaxLengthString { }
";
        var compilation = CreateCompilation(source);
        var generator = new EFCoreValueConverterGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("class NoArgMaxLengthStringValueConverter : ValueConverter<NoArgMaxLengthString, string>");
        generatedSource.Should().NotContain("HaveMaxLength");
    }

    [Fact]
    public void GenerateExtensionsSource_WithPartialPrecisionOrScale_DoesNotEmitHavePrecision()
    {
        var primitives = new[]
        {
            new PrimitiveInfo("TestNamespace", "OnlyPrec", "decimal", false, null, 10, null),
            new PrimitiveInfo("TestNamespace", "OnlyScale", "decimal", false, null, null, 2)
        };
        var source = EFCoreValueConverterGenerator.GenerateExtensionsSource(primitives);
        source.Should().NotContain("HavePrecision");
    }

    [Fact]
    public void GenerateConverterSource_WithPrimitive_ProducesExactExpectedOutput()
    {
        var primitive = new PrimitiveInfo("TestNamespace", "CustomerId", "string", false, 50, null, null);
        var source = EFCoreValueConverterGenerator.GenerateConverterSource(primitive);
        var expected = @"// <auto-generated/>
#nullable enable
using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TestNamespace;

namespace EricksonLopez.DomainPrimitives.EFCore.Generated;

/// <summary>
/// EF Core ValueConverter for the CustomerId domain primitive.
/// </summary>
public sealed class CustomerIdValueConverter : ValueConverter<CustomerId, string>
{
    public CustomerIdValueConverter() : this(null)
    {
    }

    public CustomerIdValueConverter(ConverterMappingHints? mappingHints)
        : base(
            model => model.Value,
            provider => CustomerId.Create(provider),
            mappingHints)
    {
    }
}
";
        source.Replace("\r\n", "\n").Trim().Should().Be(expected.Replace("\r\n", "\n").Trim());
    }

    [Fact]
    public void GenerateConverterSource_WithSmartEnum_ProducesFromValueFactory()
    {
        var primitive = new PrimitiveInfo("TestNamespace", "UserRole", "int", true, null, null, null);
        var source = EFCoreValueConverterGenerator.GenerateConverterSource(primitive);
        source.Should().Contain("UserRole.FromValue(provider)");
    }

    [Fact]
    public void GenerateExtensionsSource_WithAllConstraints_ProducesExactExpectedOutput()
    {
        var primitives = new[]
        {
            new PrimitiveInfo("TestNamespace", "Name", "string", false, 100, null, null),
            new PrimitiveInfo("TestNamespace", "Price", "decimal", false, null, 18, 4),
            new PrimitiveInfo("<global namespace>", "GlobalId", "string", false, null, null, null)
        };
        var source = EFCoreValueConverterGenerator.GenerateExtensionsSource(primitives);
        var expected = @"// <auto-generated/>
#nullable enable
using Microsoft.EntityFrameworkCore;

namespace EricksonLopez.DomainPrimitives.EFCore.Generated;

/// <summary>
/// Extension methods to register Domain Primitives ValueConverters in EF Core.
/// </summary>
public static class DomainPrimitivesEFCoreExtensions
{
    /// <summary>
    /// Configures all generated ValueConverters in the provided ModelConfigurationBuilder.
    /// Call this inside your DbContext.ConfigureConventions method.
    /// </summary>
    public static void ConfigureDomainPrimitives(this ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<TestNamespace.Name>()
            .HaveConversion<NameValueConverter>()
            .HaveMaxLength(100)
            ;
        configurationBuilder.Properties<TestNamespace.Price>()
            .HaveConversion<PriceValueConverter>()
            .HavePrecision(18, 4)
            ;
        configurationBuilder.Properties<GlobalId>()
            .HaveConversion<GlobalIdValueConverter>()
            ;
    }
}
";
        source.Replace("\r\n", "\n").Trim().Should().Be(expected.Replace("\r\n", "\n").Trim());
    }

    [Fact]
    public void Generator_WithOnlyEFCoreAttribute_DoesNotGenerateConverters()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.EFCoreAttribute]
public readonly partial struct OnlyEFCore { }
";
        var compilation = CreateCompilation(source);
        var generator = new EFCoreValueConverterGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        outputCompilation.SyntaxTrees.Count().Should().Be(2);
    }

    [Fact]
    public void Generator_WithOnlyDomainPrimitivesDefaultsAttribute_DoesNotGenerateConverters()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.DomainPrimitivesDefaultsAttribute]
public readonly partial struct OnlyDefaults { }
";
        var compilation = CreateCompilation(source);
        var generator = new EFCoreValueConverterGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        outputCompilation.SyntaxTrees.Count().Should().Be(2);
    }

    [Fact]
    public void Generator_WithValueObjectAttribute_DoesNotGenerateConverters()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.ValueObjectAttribute]
public readonly partial struct FullAddress { }
";
        var compilation = CreateCompilation(source);
        var generator = new EFCoreValueConverterGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        outputCompilation.SyntaxTrees.Count().Should().Be(2);
    }

    [Fact]
    public void Generator_WithClass_DoesNotGenerateConverters()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.StringPrimitiveAttribute]
public class ClassPrimitive { }
";
        var compilation = CreateCompilation(source);
        var generator = new EFCoreValueConverterGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        outputCompilation.SyntaxTrees.Count().Should().Be(2);
    }

    [Fact]
    public void Generator_WithRecordClass_DoesNotGenerateConverters()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.StringPrimitiveAttribute]
public record RecordClassPrimitive { }
";
        var compilation = CreateCompilation(source);
        var generator = new EFCoreValueConverterGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        outputCompilation.SyntaxTrees.Count().Should().Be(2);
    }

    [Fact]
    public void Generator_WithUnresolvedAttribute_DoesNotGenerateConverters()
    {
        string source = @"
namespace TestNamespace;
[UnknownUnresolvedAttribute]
public readonly partial struct UnresolvedStruct { }
";
        var syntaxTrees = new[] { CSharpSyntaxTree.ParseText(source) };
        var compilation = CSharpCompilation.Create("compilation", syntaxTrees, null, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var generator = new EFCoreValueConverterGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        outputCompilation.SyntaxTrees.Count().Should().Be(1);
    }

    [Fact]
    public void Generator_WithAttributeInDifferentNamespace_DoesNotGenerateConverters()
    {
        string source = @"
namespace OtherNamespace
{
    public class StringPrimitiveAttribute : System.Attribute { }
}
namespace TestNamespace;
[OtherNamespace.StringPrimitiveAttribute]
public readonly partial struct OtherNsStruct { }
";
        var compilation = CreateCompilation(source);
        var generator = new EFCoreValueConverterGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        outputCompilation.SyntaxTrees.Count().Should().Be(2);
    }

    [Fact]
    public void IsDomainPrimitiveAttribute_ValidatesAttributesCorrectly()
    {
        string source = @"
namespace EricksonLopez.DomainPrimitives
{
    public class EFCoreAttribute : System.Attribute { }
    public class DomainPrimitivesDefaultsAttribute : System.Attribute { }
    public class AspNetCoreAttribute : System.Attribute { }
    public class ValueObjectAttribute : System.Attribute { }
    public class StringPrimitiveAttribute : System.Attribute { }
}
namespace CustomNamespace
{
    public class StringPrimitiveAttribute : System.Attribute { }
}

[EricksonLopez.DomainPrimitives.StringPrimitive]
[EricksonLopez.DomainPrimitives.EFCore]
[EricksonLopez.DomainPrimitives.DomainPrimitivesDefaults]
[EricksonLopez.DomainPrimitives.AspNetCore]
[EricksonLopez.DomainPrimitives.ValueObject]
[CustomNamespace.StringPrimitive]
[System.Serializable]
public struct TestAttributesType { }
";
        var compilation = CreateCompilation(source);
        var typeSymbol = compilation.GetTypeByMetadataName("TestAttributesType")!;
        var attrs = typeSymbol.GetAttributes();

        var stringPrimAttr = attrs.First(a => a.AttributeClass?.Name == "StringPrimitiveAttribute" && a.AttributeClass.ContainingNamespace.Name == "DomainPrimitives");
        var efCoreAttr = attrs.First(a => a.AttributeClass?.Name == "EFCoreAttribute");
        var defaultsAttr = attrs.First(a => a.AttributeClass?.Name == "DomainPrimitivesDefaultsAttribute");
        var aspNetCoreAttr = attrs.First(a => a.AttributeClass?.Name == "AspNetCoreAttribute");
        var valueObjAttr = attrs.First(a => a.AttributeClass?.Name == "ValueObjectAttribute");
        var customAttr = attrs.First(a => a.AttributeClass?.ContainingNamespace.Name == "CustomNamespace");
        var serializableAttr = attrs.First(a => a.AttributeClass?.Name == "SerializableAttribute");

        EFCoreValueConverterGenerator.IsDomainPrimitiveAttribute(stringPrimAttr).Should().BeTrue();
        EFCoreValueConverterGenerator.IsDomainPrimitiveAttribute(efCoreAttr).Should().BeFalse();
        EFCoreValueConverterGenerator.IsDomainPrimitiveAttribute(defaultsAttr).Should().BeFalse();
        EFCoreValueConverterGenerator.IsDomainPrimitiveAttribute(aspNetCoreAttr).Should().BeFalse();
        EFCoreValueConverterGenerator.IsDomainPrimitiveAttribute(valueObjAttr).Should().BeFalse();
        EFCoreValueConverterGenerator.IsDomainPrimitiveAttribute(customAttr).Should().BeFalse();
        EFCoreValueConverterGenerator.IsDomainPrimitiveAttribute(serializableAttr).Should().BeFalse();
    }

    [Fact]
    public void Generator_WithDatePrimitiveKinds_GeneratesCorrectBackingTypes()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.DatePrimitive(Kind = 1)]
public readonly partial struct DtType { }

[EricksonLopez.DomainPrimitives.DatePrimitive(Kind = 2)]
public readonly partial struct ToType { }

[EricksonLopez.DomainPrimitives.DatePrimitive(Kind = 3)]
public readonly partial struct DtoType { }
";
        var compilation = CreateCompilation(source);
        var generator = new EFCoreValueConverterGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("ValueConverter<DtType, global::DateTime>");
        generatedSource.Should().Contain("ValueConverter<ToType, global::System.TimeOnly>");
        generatedSource.Should().Contain("ValueConverter<DtoType, global::System.DateTimeOffset>");
    }

    [Fact]
    public void Generator_WithLengthAttribute_CapturesMaxLength()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.StringPrimitive]
[EricksonLopez.DomainPrimitives.Validation.Length(5, 50)]
public readonly partial struct LengthBoundedString { }
";
        var compilation = CreateCompilation(source);
        var generator = new EFCoreValueConverterGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("HaveMaxLength(50)");
    }
}




