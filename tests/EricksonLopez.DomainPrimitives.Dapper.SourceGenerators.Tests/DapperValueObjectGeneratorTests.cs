#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using FluentAssertions;
using Xunit;
using EricksonLopez.DomainPrimitives.Dapper.SourceGenerators;

namespace EricksonLopez.DomainPrimitives.Dapper.SourceGenerators.Tests;

public class DapperValueObjectGeneratorTests
{
    private static Compilation CreateCompilation(string source)
    {
        string dummyAttributes = @"
namespace EricksonLopez.DomainPrimitives
{
    public class DapperAttribute : System.Attribute { }
    public class ValueObjectAttribute : System.Attribute { }
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
    public void Generator_WithValueObject_ShouldGenerateCode()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.DapperAttribute]
[EricksonLopez.DomainPrimitives.ValueObjectAttribute]
public readonly partial record struct Money
{
    public decimal Amount { get; init; }
    public string Currency { get; init; }
    private int InternalState { get; init; } // Should be ignored
    public static int IgnoreStatic { get; } // Should be ignored
}
";
        var compilation = CreateCompilation(source);
        var generator = new DapperValueObjectGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("class MoneyDapperExtensions");
        generatedSource.Should().Contain("parameters.Add($\"{prefix}Amount\", value.Amount);");
        generatedSource.Should().Contain("parameters.Add($\"{prefix}Currency\", value.Currency);");
        generatedSource.Should().Contain("var idx_Amount = record.GetOrdinal($\"{prefix}Amount\");");
        generatedSource.Should().Contain("var val_Amount = record.IsDBNull(idx_Amount) ? default : (decimal)record.GetValue(idx_Amount);");
        generatedSource.Should().Contain("return Money.Create(val_Amount!, val_Currency!);");
        
        generatedSource.Should().NotContain("InternalState");
        generatedSource.Should().NotContain("IgnoreStatic");
    }
    
    [Fact]
    public void Generator_WithEmptyValueObject_ShouldGenerateEmptyCreate()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.DapperAttribute]
[EricksonLopez.DomainPrimitives.ValueObjectAttribute]
public readonly partial record struct EmptyObject { }
";
        var compilation = CreateCompilation(source);
        var generator = new DapperValueObjectGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("class EmptyObjectDapperExtensions");
        generatedSource.Should().Contain("return EmptyObject.Create();");
    }

    [Fact]
    public void ValueObjectProperty_Equals_GetHashCode_Tests()
    {
        // Using reflection to test internal struct, or simply creating instances if they were accessible... wait, it's internal.
        // I can test via semantic model by triggering multiple passes or I can invoke them using reflection since they are internal.
        // Actually, to get 100% on internal structs I will use reflection in the test.
        var type = typeof(DapperValueObjectGenerator).Assembly.GetType("EricksonLopez.DomainPrimitives.Dapper.SourceGenerators.ValueObjectProperty")!;
        var obj1 = Activator.CreateInstance(type, "Prop1", "string")!;
        var obj2 = Activator.CreateInstance(type, "Prop1", "string")!;
        var obj3 = Activator.CreateInstance(type, "Prop2", "string")!;
        var obj4 = Activator.CreateInstance(type, "Prop1", "int")!;

        obj1.Equals(obj2).Should().BeTrue();
        obj1.Equals(obj3).Should().BeFalse();
        obj1.Equals(obj4).Should().BeFalse();
        obj1.Equals(null).Should().BeFalse();
        
        obj1.GetHashCode().Should().Be(obj2.GetHashCode());
        obj1.GetHashCode().Should().NotBe(obj3.GetHashCode());
    }

    [Fact]
    public void ValueObjectInfo_Equals_GetHashCode_Tests()
    {
        var propType = typeof(DapperValueObjectGenerator).Assembly.GetType("EricksonLopez.DomainPrimitives.Dapper.SourceGenerators.ValueObjectProperty")!;
        var p1 = Activator.CreateInstance(propType, "Prop", "string")!;
        var propsArray = Array.CreateInstance(propType, 1);
        propsArray.SetValue(p1, 0);
        
        var equatableArrayType = typeof(DapperValueObjectGenerator).Assembly.GetType("EricksonLopez.DomainPrimitives.Dapper.SourceGenerators.EquatableArray`1")!.MakeGenericType(propType);
        
        var methodToImmutable = typeof(System.Collections.Immutable.ImmutableArray).GetMethods().First(m => m.Name == "ToImmutableArray" && m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType.Name == "IEnumerable`1").MakeGenericMethod(propType);
        var immutableArray = methodToImmutable.Invoke(null, new object[] { propsArray });
        
        var equatableArray = Activator.CreateInstance(equatableArrayType, immutableArray)!;

        var infoType = typeof(DapperValueObjectGenerator).Assembly.GetType("EricksonLopez.DomainPrimitives.Dapper.SourceGenerators.ValueObjectInfo")!;
        var info1 = Activator.CreateInstance(infoType, "Namespace", "Name", equatableArray)!;
        var info2 = Activator.CreateInstance(infoType, "Namespace", "Name", equatableArray)!;
        var info3 = Activator.CreateInstance(infoType, "Namespace2", "Name", equatableArray)!;
        
        info1.Equals(info2).Should().BeTrue();
        info1.Equals(info3).Should().BeFalse();
        info1.Equals(null).Should().BeFalse();
        
        info1.GetHashCode().Should().Be(info2.GetHashCode());
        info1.GetHashCode().Should().NotBe(info3.GetHashCode());
    }
    
    [Fact]
    public void EquatableArray_Equals_GetHashCode_Tests()
    {
        var type = typeof(DapperValueObjectGenerator).Assembly.GetType("EricksonLopez.DomainPrimitives.Dapper.SourceGenerators.EquatableArray`1")!.MakeGenericType(typeof(int));
        
        var arr1 = Activator.CreateInstance(type, new int[] { 1, 2, 3 })!;
        var arr2 = Activator.CreateInstance(type, new int[] { 1, 2, 3 })!;
        var arr3 = Activator.CreateInstance(type, new int[] { 1, 2, 4 })!;
        var arr4 = Activator.CreateInstance(type, new int[] { 1, 2 })!;
        
        arr1.Equals(arr2).Should().BeTrue();
        arr1.Equals(arr3).Should().BeFalse();
        arr1.Equals(arr4).Should().BeFalse();
        arr1.Equals(null).Should().BeFalse();
        
        arr1.GetHashCode().Should().Be(arr2.GetHashCode());
        arr1.GetHashCode().Should().NotBe(arr3.GetHashCode());
    }
}





