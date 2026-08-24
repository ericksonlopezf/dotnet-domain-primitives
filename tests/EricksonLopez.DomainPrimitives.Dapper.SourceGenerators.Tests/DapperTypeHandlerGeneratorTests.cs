// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using AwesomeAssertions;
using EricksonLopez.DomainPrimitives.Dapper.SourceGenerators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Threading.Tasks;
using Xunit;

namespace EricksonLopez.DomainPrimitives.Dapper.SourceGenerators.Tests;

public class DapperTypeHandlerGeneratorTests
{
    private static Compilation CreateCompilation(string source)
    {
        string dummyAttributes = @"
namespace EricksonLopez.DomainPrimitives
{
    public class DapperAttribute : System.Attribute { }
    public class DomainPrimitivesDefaultsAttribute : System.Attribute { }
    public class ValueObjectAttribute : System.Attribute { }
    public class StringPrimitiveAttribute : System.Attribute { }
    public class StrongIdAttribute<T> : System.Attribute { }
    public class DatePrimitiveAttribute : System.Attribute { public int Kind { get; set; } }
    public class NumericPrimitiveAttribute<T> : System.Attribute { }
    public class PercentageAttribute : System.Attribute { }
    public class SmartEnumAttribute : System.Attribute { }
    public class SmartEnumAttribute<T> : System.Attribute { }
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
[EricksonLopez.DomainPrimitives.DapperAttribute]
[EricksonLopez.DomainPrimitives.StringPrimitiveAttribute]
public readonly partial struct NameId { }
";
        var compilation = CreateCompilation(source);
        var generator = new DapperTypeHandlerGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("class NameIdTypeHandler");
        generatedSource.Should().Contain("NameId.Create(s)");
    }
    
    [Fact]
    public void Generator_WithGuidBackedPrimitive_ShouldGenerateCode()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.DapperAttribute]
[EricksonLopez.DomainPrimitives.StrongIdAttribute<Guid>]
public readonly partial struct UserGuid { }
";
        var compilation = CreateCompilation(source);
        var generator = new DapperTypeHandlerGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("if (value is Guid g)");
    }

    [Fact]
    public void Generator_WithDateOnlyBackedPrimitive_ShouldGenerateCode()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.DapperAttribute]
[EricksonLopez.DomainPrimitives.DatePrimitiveAttribute]
public readonly partial struct BirthDate { }
";
        var compilation = CreateCompilation(source);
        var generator = new DapperTypeHandlerGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("DateOnly.FromDateTime");
    }

    [Fact]
    public void Generator_WithNumericBackedPrimitive_ShouldGenerateCode()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.DapperAttribute]
[EricksonLopez.DomainPrimitives.NumericPrimitiveAttribute<int>]
public readonly partial struct Age { }
";
        var compilation = CreateCompilation(source);
        var generator = new DapperTypeHandlerGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("Convert.ChangeType");
    }
    
    [Fact]
    public void Generator_WithPercentageAttribute_ShouldGenerateDecimal()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.DapperAttribute]
[EricksonLopez.DomainPrimitives.PercentageAttribute]
public readonly partial struct Discount { }
";
        var compilation = CreateCompilation(source);
        var generator = new DapperTypeHandlerGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("(decimal)Convert.ChangeType");
    }
    
    [Fact]
    public void Generator_WithSmartEnumAttribute_ShouldGenerateCode()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.DapperAttribute]
[EricksonLopez.DomainPrimitives.SmartEnumAttribute<string>]
public readonly partial struct Status { }
";
        var compilation = CreateCompilation(source);
        var generator = new DapperTypeHandlerGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("Status.FromValue");
    }
    
    [Fact]
    public void Generator_WithNonGenericSmartEnumAttribute_ShouldGenerateInt()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.DapperAttribute]
[EricksonLopez.DomainPrimitives.SmartEnumAttribute]
public readonly partial struct TypeCode { }
";
        var compilation = CreateCompilation(source);
        var generator = new DapperTypeHandlerGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("(int)Convert.ChangeType");
    }
    
    [Fact]
    public void Generator_WithDatePrimitiveKind_ShouldGenerateCorrectBackingType()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.DapperAttribute]
[EricksonLopez.DomainPrimitives.DatePrimitiveAttribute(Kind = 1)]
public readonly partial struct CreatedAt { }

[EricksonLopez.DomainPrimitives.DatePrimitiveAttribute(Kind = 2)]
public readonly partial struct LogTime { }

[EricksonLopez.DomainPrimitives.DatePrimitiveAttribute(Kind = 3)]
public readonly partial struct ScheduledAt { }
";
        var compilation = CreateCompilation(source);
        var generator = new DapperTypeHandlerGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("(global::DateTime)Convert.ChangeType");
        generatedSource.Should().Contain("(global::System.TimeOnly)Convert.ChangeType");
        generatedSource.Should().Contain("(global::System.DateTimeOffset)Convert.ChangeType");
    }

    [Fact]
    public void Generator_WithSmartEnumGeneric_ShouldGenerateLongBackingType()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.SmartEnumAttribute<long>]
public readonly partial struct StatusLong { }
";
        var compilation = CreateCompilation(source);
        var generator = new DapperTypeHandlerGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("(long)Convert.ChangeType");
    }

    [Fact]
    public void Generator_WithUnresolvedAttribute_DoesNotGenerateTypeHandler()
    {
        string source = @"
namespace TestNamespace;
[UnknownUnresolvedAttribute]
public readonly partial struct UnresolvedStruct { }
";
        var syntaxTrees = new[] { CSharpSyntaxTree.ParseText(source) };
        var compilation = CSharpCompilation.Create("compilation", syntaxTrees, null, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var generator = new DapperTypeHandlerGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        outputCompilation.SyntaxTrees.Count().Should().Be(1);
    }

    [Fact]
    public void GenerateTypeHandler_WithShortGuidAndDateOnly_GeneratesSpecificParsers()
    {
        var guidInfo = new PrimitiveInfo("MyCompany.Domain", "ShortGuidId", "Guid", isSmartEnum: false);
        var guidCode = DapperTypeHandlerGenerator.GenerateTypeHandler(guidInfo);
        guidCode.Should().Contain("if (value is Guid g)");

        var dateInfo = new PrimitiveInfo("MyCompany.Domain", "ShortDate", "DateOnly", isSmartEnum: false);
        var dateCode = DapperTypeHandlerGenerator.GenerateTypeHandler(dateInfo);
        dateCode.Should().Contain("DateOnly.FromDateTime(dt)");
    }

    [Fact]
    public void Generator_WithMoneyAttribute_ShouldGenerateDecimal()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.MoneyAttribute]
public readonly partial struct Price { }
";
        var compilation = CreateCompilation(source);
        var generator = new DapperTypeHandlerGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("(decimal)Convert.ChangeType");
    }

    [Fact]
    public void Generator_WithClass_OrValueObject_OrNonPrimitive_ShouldNotGenerateTypeHandler()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.StringPrimitiveAttribute]
public class ClassNotStruct { }

[EricksonLopez.DomainPrimitives.ValueObjectAttribute]
public readonly partial record struct VoStruct { public int A { get; } }

public readonly partial struct NormalStruct { }

[System.Serializable]
public readonly partial struct OtherAttrStruct { }
";
        var compilation = CreateCompilation(source);
        var generator = new DapperTypeHandlerGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedTrees = outputCompilation.SyntaxTrees.Skip(2).ToList();
        generatedTrees.Should().BeEmpty();
    }

    [Theory]
    [InlineData("EmailAttribute")]
    [InlineData("PhoneAttribute")]
    [InlineData("UrlAttribute")]
    [InlineData("SlugAttribute")]
    [InlineData("CountryCodeAttribute")]
    [InlineData("LanguageCodeAttribute")]
    [InlineData("CurrencyCodeAttribute")]
    [InlineData("UsernameAttribute")]
    [InlineData("PasswordHashAttribute")]
    [InlineData("HexColorAttribute")]
    [InlineData("IPAddressAttribute")]
    [InlineData("MacAddressAttribute")]
    [InlineData("IBANAttribute")]
    [InlineData("ISBNAttribute")]
    [InlineData("VINAttribute")]
    [InlineData("LatitudeAttribute")]
    [InlineData("LongitudeAttribute")]
    [InlineData("AgeAttribute")]
    [InlineData("WeightAttribute")]
    [InlineData("HeightAttribute")]
    [InlineData("DistanceAttribute")]
    [InlineData("TemperatureAttribute")]
    [InlineData("ScoreAttribute")]
    [InlineData("QuantityAttribute")]
    [InlineData("PriceAttribute")]
    [InlineData("TaxRateAttribute")]
    [InlineData("DiscountAttribute")]
    [InlineData("RatingAttribute")]
    [InlineData("BirthDateAttribute")]
    [InlineData("ExpirationDateAttribute")]
    [InlineData("BusinessDateAttribute")]
    [InlineData("FiscalYearAttribute")]
    [InlineData("MonthAttribute")]
    [InlineData("QuarterAttribute")]
    [InlineData("WeekAttribute")]
    [InlineData("DateRangeAttribute")]
    [InlineData("TimeRangeAttribute")]
    public void Generator_WithShortcutAttributes_GeneratesTypeHandler(string attributeName)
    {
        string source = $@"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.{attributeName}]
public readonly partial struct TestPrimitive {{ }}
";
        var compilation = CreateCompilation(source);
        var generator = new DapperTypeHandlerGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("class TestPrimitiveTypeHandler");
    }

    [Fact]
    public void IsDomainPrimitiveAttribute_ValidatesAttributesCorrectly()
    {
        string source = @"
namespace EricksonLopez.DomainPrimitives
{
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
        var dapperAttr = attrs.First(a => a.AttributeClass?.Name == "DapperAttribute");
        var efCoreAttr = attrs.First(a => a.AttributeClass?.Name == "EFCoreAttribute");
        var defaultsAttr = attrs.First(a => a.AttributeClass?.Name == "DomainPrimitivesDefaultsAttribute");
        var aspNetCoreAttr = attrs.First(a => a.AttributeClass?.Name == "AspNetCoreAttribute");
        var valueObjAttr = attrs.First(a => a.AttributeClass?.Name == "ValueObjectAttribute");
        var customAttr = attrs.First(a => a.AttributeClass?.ContainingNamespace.Name == "CustomNamespace");
        var serializableAttr = attrs.First(a => a.AttributeClass?.Name == "SerializableAttribute");

        DapperTypeHandlerGenerator.IsDomainPrimitiveAttribute(stringPrimAttr).Should().BeTrue();
        DapperTypeHandlerGenerator.IsDomainPrimitiveAttribute(dapperAttr).Should().BeFalse();
        DapperTypeHandlerGenerator.IsDomainPrimitiveAttribute(efCoreAttr).Should().BeFalse();
        DapperTypeHandlerGenerator.IsDomainPrimitiveAttribute(defaultsAttr).Should().BeFalse();
        DapperTypeHandlerGenerator.IsDomainPrimitiveAttribute(aspNetCoreAttr).Should().BeFalse();
        DapperTypeHandlerGenerator.IsDomainPrimitiveAttribute(valueObjAttr).Should().BeFalse();
        DapperTypeHandlerGenerator.IsDomainPrimitiveAttribute(customAttr).Should().BeFalse();
        DapperTypeHandlerGenerator.IsDomainPrimitiveAttribute(serializableAttr).Should().BeFalse();
    }
}







