using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using FluentAssertions;
using Xunit;
using EricksonLopez.DomainPrimitives.Dapper.SourceGenerators;

namespace EricksonLopez.DomainPrimitives.Dapper.SourceGenerators.Tests;

public class DapperTypeHandlerGeneratorTests
{
    private static Compilation CreateCompilation(string source)
    {
        string dummyAttributes = @"
namespace EricksonLopez.DomainPrimitives
{
    public class DapperAttribute : System.Attribute { }
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
[EricksonLopez.DomainPrimitives.StrongIdAttribute<System.Guid>]
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
";
        var compilation = CreateCompilation(source);
        var generator = new DapperTypeHandlerGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("global::System.DateTime");
    }
}




