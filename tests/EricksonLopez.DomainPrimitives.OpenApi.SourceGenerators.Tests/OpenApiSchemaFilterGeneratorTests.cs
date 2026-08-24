// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using AwesomeAssertions;
using EricksonLopez.DomainPrimitives.OpenApi.SourceGenerators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace EricksonLopez.DomainPrimitives.OpenApi.SourceGenerators.Tests;

public class OpenApiSchemaFilterGeneratorTests
{
    private static CSharpParseOptions ParseOptions => new CSharpParseOptions(LanguageVersion.CSharp11);

    private static Compilation CreateCompilation(string source)
    {
        string dummyAttributes = @"
namespace EricksonLopez.DomainPrimitives
{
    public class JsonAttribute : System.Attribute { }
    public class MapsterAttribute : System.Attribute { }
    public class OpenApiAttribute : System.Attribute { }
    public class StringPrimitiveAttribute : System.Attribute { }
    public class ValueObjectAttribute : System.Attribute { }
    public class NumericPrimitiveAttribute<T> : System.Attribute { }

public class AgeAttribute : System.Attribute { }
public class BirthDateAttribute : System.Attribute { }
public class BusinessDateAttribute : System.Attribute { }
public class CountryCodeAttribute : System.Attribute { }
public class CurrencyCodeAttribute : System.Attribute { }
public class DatePrimitiveAttribute : System.Attribute { public int Kind { get; set; } }
public class DateRangeAttribute : System.Attribute { }
public class DiscountAttribute : System.Attribute { }
public class DistanceAttribute : System.Attribute { }
public class EmailAttribute : System.Attribute { }
public class ExpirationDateAttribute : System.Attribute { }
public class FiscalYearAttribute : System.Attribute { }
public class HeightAttribute : System.Attribute { }
public class HexColorAttribute : System.Attribute { }
public class LanguageCodeAttribute : System.Attribute { }
public class LatitudeAttribute : System.Attribute { }
public class LongitudeAttribute : System.Attribute { }
public class MoneyAttribute : System.Attribute { }
public class MonthAttribute : System.Attribute { }
public class PasswordHashAttribute : System.Attribute { }
public class PercentageAttribute : System.Attribute { }
public class PhoneAttribute : System.Attribute { }
public class PriceAttribute : System.Attribute { }
public class QuantityAttribute : System.Attribute { }
public class QuarterAttribute : System.Attribute { }
public class RatingAttribute : System.Attribute { }
public class ScoreAttribute : System.Attribute { }
public class SlugAttribute : System.Attribute { }
public class SmartEnumAttribute : System.Attribute { }
public class SmartEnumAttribute<T> : System.Attribute { }
public class StrongIdAttribute<T> : System.Attribute { }
public class TaxRateAttribute : System.Attribute { }
public class TemperatureAttribute : System.Attribute { }
public class TimeRangeAttribute : System.Attribute { }
public class UrlAttribute : System.Attribute { }
public class UsernameAttribute : System.Attribute { }
public class WeekAttribute : System.Attribute { }
public class WeightAttribute : System.Attribute { }

}
namespace System
{
    public struct Guid {}
    public struct DateTime {}
    public struct DateTimeOffset {}
    public struct TimeOnly {}
    public struct DateOnly { public static DateOnly FromDateTime(DateTime dt) => default; }
}
";
        var syntaxTrees = new[] { 
            CSharpSyntaxTree.ParseText(source, ParseOptions),
            CSharpSyntaxTree.ParseText(dummyAttributes, ParseOptions)
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
    public void Generator_WithNumericPrimitiveAndMoneyAndPercentage_GeneratesCorrectSchemas()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.NumericPrimitiveAttribute<decimal>]
public readonly partial struct CustomNumeric { }

[EricksonLopez.DomainPrimitives.MoneyAttribute]
public readonly partial struct CustomMoney { }

[EricksonLopez.DomainPrimitives.PercentageAttribute]
public readonly partial struct CustomPercentage { }
";
        var compilation = CreateCompilation(source);
        var generator = new OpenApiSchemaFilterGenerator();
        var driver = CSharpGeneratorDriver.Create(new[] { generator.AsSourceGenerator() }, parseOptions: ParseOptions);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        
        generatedSource.Should().Contain("typeof(TestNamespace.CustomNumeric), schema => \n            {\n                schema.Type = \"number\";\n            }".Replace("\n", Environment.NewLine));
        generatedSource.Should().Contain("typeof(TestNamespace.CustomMoney), schema => \n            {\n                schema.Type = \"number\";\n                schema.Format = \"double\";\n            }".Replace("\n", Environment.NewLine));
        generatedSource.Should().Contain("typeof(TestNamespace.CustomPercentage), schema => \n            {\n                schema.Type = \"number\";\n            }".Replace("\n", Environment.NewLine));
    }

    [Fact]
    public void Generator_WithDatePrimitives_GeneratesStringWithDateFormat()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.DatePrimitiveAttribute]
public readonly partial struct CustomDate { }

[EricksonLopez.DomainPrimitives.BirthDateAttribute]
public readonly partial struct CustomBirthDate { }

[EricksonLopez.DomainPrimitives.ExpirationDateAttribute]
public readonly partial struct CustomExpDate { }
";
        var compilation = CreateCompilation(source);
        var generator = new OpenApiSchemaFilterGenerator();
        var driver = CSharpGeneratorDriver.Create(new[] { generator.AsSourceGenerator() }, parseOptions: ParseOptions);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        
        generatedSource.Should().Contain("typeof(TestNamespace.CustomDate), schema => \n            {\n                schema.Type = \"string\";\n                schema.Format = \"date\";\n            }".Replace("\n", Environment.NewLine));
        generatedSource.Should().Contain("typeof(TestNamespace.CustomBirthDate), schema => \n            {\n                schema.Type = \"string\";\n                schema.Format = \"date\";\n            }".Replace("\n", Environment.NewLine));
        generatedSource.Should().Contain("typeof(TestNamespace.CustomExpDate), schema => \n            {\n                schema.Type = \"string\";\n                schema.Format = \"date\";\n            }".Replace("\n", Environment.NewLine));
    }

    [Fact]
    public void Generator_WithStrongIds_GeneratesUuidOrInteger()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.StrongIdAttribute<Guid>]
public readonly partial struct GuidId { }

[EricksonLopez.DomainPrimitives.StrongIdAttribute<int>]
public readonly partial struct IntId { }

[EricksonLopez.DomainPrimitives.StrongIdAttribute<long>]
public readonly partial struct LongId { }

[EricksonLopez.DomainPrimitives.StrongIdAttribute<string>]
public readonly partial struct StringId { }
";
        var compilation = CreateCompilation(source);
        var generator = new OpenApiSchemaFilterGenerator();
        var driver = CSharpGeneratorDriver.Create(new[] { generator.AsSourceGenerator() }, parseOptions: ParseOptions);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        
        generatedSource.Should().Contain("typeof(TestNamespace.GuidId), schema => \n            {\n                schema.Type = \"string\";\n                schema.Format = \"uuid\";\n            }".Replace("\n", Environment.NewLine));
        generatedSource.Should().Contain("typeof(TestNamespace.IntId), schema => \n            {\n                schema.Type = \"integer\";\n            }".Replace("\n", Environment.NewLine));
        generatedSource.Should().Contain("typeof(TestNamespace.LongId), schema => \n            {\n                schema.Type = \"integer\";\n            }".Replace("\n", Environment.NewLine));
        generatedSource.Should().Contain("typeof(TestNamespace.StringId), schema => \n            {\n                schema.Type = \"string\";\n            }".Replace("\n", Environment.NewLine));
    }

    [Fact]
    public void Generator_WithEmailAndUrl_GeneratesEmailAndUriFormats()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.EmailAttribute]
public readonly partial struct CustomEmail { }

[EricksonLopez.DomainPrimitives.UrlAttribute]
public readonly partial struct CustomUrl { }
";
        var compilation = CreateCompilation(source);
        var generator = new OpenApiSchemaFilterGenerator();
        var driver = CSharpGeneratorDriver.Create(new[] { generator.AsSourceGenerator() }, parseOptions: ParseOptions);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        
        generatedSource.Should().Contain("typeof(TestNamespace.CustomEmail), schema => \n            {\n                schema.Type = \"string\";\n                schema.Format = \"email\";\n            }".Replace("\n", Environment.NewLine));
        generatedSource.Should().Contain("typeof(TestNamespace.CustomUrl), schema => \n            {\n                schema.Type = \"string\";\n                schema.Format = \"uri\";\n            }".Replace("\n", Environment.NewLine));
    }

    [Fact]
    public void Generator_WithSmartEnums_GeneratesIntegerOrStringEnums()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.SmartEnumAttribute<int>]
public readonly partial struct IntSmartEnum { }

[EricksonLopez.DomainPrimitives.SmartEnumAttribute<long>]
public readonly partial struct LongSmartEnum { }

[EricksonLopez.DomainPrimitives.SmartEnumAttribute<string>]
public readonly partial struct StringSmartEnum { }

[EricksonLopez.DomainPrimitives.SmartEnumAttribute]
public readonly partial struct NonGenericSmartEnum { }
";
        var compilation = CreateCompilation(source);
        var generator = new OpenApiSchemaFilterGenerator();
        var driver = CSharpGeneratorDriver.Create(new[] { generator.AsSourceGenerator() }, parseOptions: ParseOptions);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        
        generatedSource.Should().Contain("typeof(TestNamespace.IntSmartEnum), schema => \n            {\n                schema.Type = \"integer\";\n                schema.Enum = new List<IOpenApiAny>();\n                foreach (var item in TestNamespace.IntSmartEnum.All)\n                {\n                    schema.Enum.Add(new OpenApiInteger((int)Convert.ChangeType(item.Value, typeof(int))));\n                }\n            }".Replace("\n", Environment.NewLine));
        generatedSource.Should().Contain("typeof(TestNamespace.LongSmartEnum), schema => \n            {\n                schema.Type = \"integer\";\n                schema.Enum = new List<IOpenApiAny>();\n                foreach (var item in TestNamespace.LongSmartEnum.All)\n                {\n                    schema.Enum.Add(new OpenApiInteger((int)Convert.ChangeType(item.Value, typeof(int))));\n                }\n            }".Replace("\n", Environment.NewLine));
        generatedSource.Should().Contain("typeof(TestNamespace.StringSmartEnum), schema => \n            {\n                schema.Type = \"string\";\n                schema.Enum = new List<IOpenApiAny>();\n                foreach (var item in TestNamespace.StringSmartEnum.All)\n                {\n                    schema.Enum.Add(new OpenApiString(item.Name));\n                }\n            }".Replace("\n", Environment.NewLine));
        generatedSource.Should().Contain("typeof(TestNamespace.NonGenericSmartEnum), schema => \n            {\n                schema.Type = \"string\";\n                schema.Enum = new List<IOpenApiAny>();\n                foreach (var item in TestNamespace.NonGenericSmartEnum.All)\n                {\n                    schema.Enum.Add(new OpenApiString(item.Name));\n                }\n            }".Replace("\n", Environment.NewLine));
    }

    [Fact]
    public void Generator_WithGuidStrongId_StringSmartEnum_AndGlobalNamespace_GeneratesExpectedOpenApiTypes()
    {
        string source = @"
[EricksonLopez.DomainPrimitives.StrongIdAttribute<Guid>]
public readonly partial struct GuidStrongId { }

[EricksonLopez.DomainPrimitives.SmartEnumAttribute<string>]
public readonly partial struct StringSmartEnum { }

[EricksonLopez.DomainPrimitives.StringPrimitiveAttribute]
public readonly partial struct GlobalStringPrimitive { }

[EricksonLopez.DomainPrimitives.StringPrimitiveAttribute]
public class ClassWithAttribute { }

public readonly partial struct PlainStructWithoutAttributes { }
";
        var compilation = CreateCompilation(source);
        var generator = new OpenApiSchemaFilterGenerator();
        var driver = CSharpGeneratorDriver.Create(new[] { generator.AsSourceGenerator() }, parseOptions: ParseOptions);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));

        generatedSource.Should().Contain("uuid");
        generatedSource.Should().Contain("OpenApiString(item.Name)");
        generatedSource.Should().Contain("typeof(GuidStrongId)");
        generatedSource.Should().Contain("typeof(StringSmartEnum)");
        generatedSource.Should().Contain("typeof(GlobalStringPrimitive)");
    }

    [Fact]
    public void Generator_WhenNoDomainPrimitivesFound_DoesNotGenerateSource()
    {
        string source = @"
namespace TestNamespace;
public class RegularClass { }
public struct RegularStruct { }
";
        var compilation = CreateCompilation(source);
        var generator = new OpenApiSchemaFilterGenerator();
        var driver = CSharpGeneratorDriver.Create(new[] { generator.AsSourceGenerator() }, parseOptions: ParseOptions);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedTrees = outputCompilation.SyntaxTrees.Skip(2).ToList();
        generatedTrees.Should().BeEmpty();
    }

    [Fact]
    public void GenerateSchemaFilterSource_WithPrimitivesAndSmartEnums_ProducesExactCode()
    {
        var primitives = new[]
        {
            new PrimitiveInfo("TestNamespace", "CustomerId", "string", "uuid", false),
            new PrimitiveInfo("TestNamespace", "Price", "number", "double", false),
            new PrimitiveInfo("TestNamespace", "CreatedAt", "string", "date", false),
            new PrimitiveInfo("TestNamespace", "IntStatus", "integer", "", true),
            new PrimitiveInfo("TestNamespace", "StringStatus", "string", "", true),
            new PrimitiveInfo("<global namespace>", "GlobalId", "string", "", false)
        };

        var source = OpenApiSchemaFilterGenerator.GenerateSchemaFilterSource(primitives);
        var expected = @"// <auto-generated/>
using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Any;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EricksonLopez.DomainPrimitives.OpenApi.Generated;

/// <summary>
/// Automatically maps Domain Primitives to their correct OpenAPI schema types without reflection.
/// </summary>
public class DomainPrimitivesSchemaFilter : ISchemaFilter
{
    private readonly Dictionary<Type, Action<OpenApiSchema>> _schemaConfigs = new()
    {
        { typeof(TestNamespace.CustomerId), schema => 
            {
                schema.Type = ""string"";
                schema.Format = ""uuid"";
            }
        },
        { typeof(TestNamespace.Price), schema => 
            {
                schema.Type = ""number"";
                schema.Format = ""double"";
            }
        },
        { typeof(TestNamespace.CreatedAt), schema => 
            {
                schema.Type = ""string"";
                schema.Format = ""date"";
            }
        },
        { typeof(TestNamespace.IntStatus), schema => 
            {
                schema.Type = ""integer"";
                schema.Enum = new List<IOpenApiAny>();
                foreach (var item in TestNamespace.IntStatus.All)
                {
                    schema.Enum.Add(new OpenApiInteger((int)Convert.ChangeType(item.Value, typeof(int))));
                }
            }
        },
        { typeof(TestNamespace.StringStatus), schema => 
            {
                schema.Type = ""string"";
                schema.Enum = new List<IOpenApiAny>();
                foreach (var item in TestNamespace.StringStatus.All)
                {
                    schema.Enum.Add(new OpenApiString(item.Name));
                }
            }
        },
        { typeof(GlobalId), schema => 
            {
                schema.Type = ""string"";
            }
        },
    };

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (_schemaConfigs.TryGetValue(context.Type, out var configAction))
        {
            configAction(schema);
        }
    }
}
";
        source.Replace("\r\n", "\n").Trim().Should().Be(expected.Replace("\r\n", "\n").Trim());
    }

    [Fact]
    public void IsDomainPrimitiveAttribute_ValidatesAttributesCorrectly()
    {
        string source = @"
namespace EricksonLopez.DomainPrimitives
{
    public class OpenApiAttribute : System.Attribute { }
    public class DapperAttribute : System.Attribute { }
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
[EricksonLopez.DomainPrimitives.OpenApi]
[EricksonLopez.DomainPrimitives.Dapper]
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
        var openApiAttr = attrs.First(a => a.AttributeClass?.Name == "OpenApiAttribute");
        var dapperAttr = attrs.First(a => a.AttributeClass?.Name == "DapperAttribute");
        var efCoreAttr = attrs.First(a => a.AttributeClass?.Name == "EFCoreAttribute");
        var defaultsAttr = attrs.First(a => a.AttributeClass?.Name == "DomainPrimitivesDefaultsAttribute");
        var aspNetCoreAttr = attrs.First(a => a.AttributeClass?.Name == "AspNetCoreAttribute");
        var valueObjAttr = attrs.First(a => a.AttributeClass?.Name == "ValueObjectAttribute");
        var customAttr = attrs.First(a => a.AttributeClass?.ContainingNamespace.Name == "CustomNamespace");
        var serializableAttr = attrs.First(a => a.AttributeClass?.Name == "SerializableAttribute");

        OpenApiSchemaFilterGenerator.IsDomainPrimitiveAttribute(stringPrimAttr).Should().BeTrue();
        OpenApiSchemaFilterGenerator.IsDomainPrimitiveAttribute(openApiAttr).Should().BeFalse();
        OpenApiSchemaFilterGenerator.IsDomainPrimitiveAttribute(dapperAttr).Should().BeFalse();
        OpenApiSchemaFilterGenerator.IsDomainPrimitiveAttribute(efCoreAttr).Should().BeFalse();
        OpenApiSchemaFilterGenerator.IsDomainPrimitiveAttribute(defaultsAttr).Should().BeFalse();
        OpenApiSchemaFilterGenerator.IsDomainPrimitiveAttribute(aspNetCoreAttr).Should().BeFalse();
        OpenApiSchemaFilterGenerator.IsDomainPrimitiveAttribute(valueObjAttr).Should().BeFalse();
        OpenApiSchemaFilterGenerator.IsDomainPrimitiveAttribute(customAttr).Should().BeFalse();
        OpenApiSchemaFilterGenerator.IsDomainPrimitiveAttribute(serializableAttr).Should().BeFalse();
    }

    [Fact]
    public void PrimitiveInfo_EqualityAndHashCode_Comprehensive()
    {
        var info1 = new PrimitiveInfo("NS", "T1", "string", "uuid", false);
        var info2 = new PrimitiveInfo("NS", "T1", "string", "uuid", false);
        var diffNs = new PrimitiveInfo("OtherNS", "T1", "string", "uuid", false);
        var diffName = new PrimitiveInfo("NS", "T2", "string", "uuid", false);
        var diffType = new PrimitiveInfo("NS", "T1", "number", "uuid", false);
        var diffFmt = new PrimitiveInfo("NS", "T1", "string", "date", false);
        var diffEnum = new PrimitiveInfo("NS", "T1", "string", "uuid", true);

        Assert.True(info1.Equals(info2));
        Assert.True(info1.Equals((object)info2));
        Assert.False(info1.Equals(diffNs));
        Assert.False(info1.Equals(diffName));
        Assert.False(info1.Equals(diffType));
        Assert.False(info1.Equals(diffFmt));
        Assert.False(info1.Equals(diffEnum));
        Assert.False(info1.Equals((object?)null));
        Assert.False(info1.Equals("not an info"));

        info1.GetHashCode().Should().Be(info2.GetHashCode());
        info1.GetHashCode().Should().NotBe(diffNs.GetHashCode());
        info1.GetHashCode().Should().NotBe(diffName.GetHashCode());
        info1.GetHashCode().Should().NotBe(diffType.GetHashCode());
        info1.GetHashCode().Should().NotBe(diffFmt.GetHashCode());
        info1.GetHashCode().Should().NotBe(diffEnum.GetHashCode());
    }

    [Fact]
    public void Generator_WithStrongIdInt32AndInt64_AndNonGenericSmartEnum_GeneratesCorrectTypes()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.StrongIdAttribute<int>]
public readonly partial struct Int32Id { }

[EricksonLopez.DomainPrimitives.StrongIdAttribute<long>]
public readonly partial struct Int64Id { }

[EricksonLopez.DomainPrimitives.SmartEnumAttribute]
public readonly partial struct NonGenericSmartEnum { }
";
        var compilation = CreateCompilation(source);
        var generator = new OpenApiSchemaFilterGenerator();
        var driver = CSharpGeneratorDriver.Create(new[] { generator.AsSourceGenerator() }, parseOptions: ParseOptions);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("typeof(TestNamespace.Int32Id)");
        generatedSource.Should().Contain("typeof(TestNamespace.Int64Id)");
        generatedSource.Should().Contain("typeof(TestNamespace.NonGenericSmartEnum)");
    }

    [Fact]
    public void Generator_WithUnresolvedAttribute_DoesNotGenerateFilter()
    {
        string source = @"
namespace TestNamespace;
[UnknownUnresolvedAttribute]
public readonly partial struct UnresolvedStruct { }
";
        var syntaxTrees = new[] { CSharpSyntaxTree.ParseText(source, ParseOptions) };
        var compilation = CSharpCompilation.Create("compilation", syntaxTrees, null, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var generator = new OpenApiSchemaFilterGenerator();
        var driver = CSharpGeneratorDriver.Create(new[] { generator.AsSourceGenerator() }, parseOptions: ParseOptions);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        outputCompilation.SyntaxTrees.Count().Should().Be(1);
    }
}





