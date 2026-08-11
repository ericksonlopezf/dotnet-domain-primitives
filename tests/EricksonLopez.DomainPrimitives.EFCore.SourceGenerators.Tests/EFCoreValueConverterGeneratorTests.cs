using System;
using System.Linq;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;
using EricksonLopez.DomainPrimitives.EFCore.SourceGenerators;

namespace EricksonLopez.DomainPrimitives.EFCore.SourceGenerators.Tests;

public class EFCoreValueConverterGeneratorTests
{
    private static Compilation CreateCompilation(string source)
    {
        string dummyAttributes = @"
namespace EricksonLopez.DomainPrimitives
{
    public class EFCoreAttribute : System.Attribute { }
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
    public class MaxLengthAttribute : System.Attribute { public MaxLengthAttribute(int max) {} }
    public class LengthAttribute : System.Attribute { public LengthAttribute(int min, int max) {} }
}
namespace System
{
    public struct Guid {}
    public struct DateOnly { public static DateOnly FromDateTime(DateTime dt) => default; }
    public struct DateTime {}
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
[EricksonLopez.DomainPrimitives.StrongIdAttribute<System.Guid>]
public readonly partial struct UserGuid { }
";
        var compilation = CreateCompilation(source);
        var generator = new EFCoreValueConverterGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("class UserGuidValueConverter : ValueConverter<UserGuid, global::System.Guid>");
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
    [InlineData(1, "global::System.DateTime")]
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
        generatedSource.Should().Contain("HavePrecision(5, 2)");
    }
}

