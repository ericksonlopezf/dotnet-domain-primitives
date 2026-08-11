using System;
using System.Linq;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;
using EricksonLopez.DomainPrimitives.AspNetCore.SourceGenerators;

namespace EricksonLopez.DomainPrimitives.AspNetCore.SourceGenerators.Tests;

public class ModelBinderProviderGeneratorTests
{
    private static Compilation CreateCompilation(string source)
    {
        string dummyAttributes = @"
namespace EricksonLopez.DomainPrimitives
{
    public class AspNetCoreAttribute : System.Attribute { }
    public class StringPrimitiveAttribute : System.Attribute { }
    public class StrongIdAttribute<T> : System.Attribute { }
    public class ValueObjectAttribute : System.Attribute { }
    public class EmailAttribute : System.Attribute { }
}
namespace System
{
    public struct Guid {}
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
    public void Generator_WithAspNetCoreAttributeAndStringPrimitive_ShouldGenerateModelBinder()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.AspNetCoreAttribute]
[EricksonLopez.DomainPrimitives.StringPrimitiveAttribute]
public readonly partial struct NameId { }
";
        var compilation = CreateCompilation(source);
        var generator = new ModelBinderProviderGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("new DomainPrimitiveModelBinder<TestNamespace.NameId>()");
        generatedSource.Should().Contain("class GeneratedDomainPrimitiveModelBinderProvider : IModelBinderProvider");
        generatedSource.Should().Contain("if (_binders.TryGetValue(context.Metadata.ModelType, out var binder))");
    }

    [Fact]
    public void Generator_WithStrongId_ShouldGenerateModelBinder()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.AspNetCoreAttribute]
[EricksonLopez.DomainPrimitives.StrongIdAttribute<System.Guid>]
public readonly partial struct UserGuid { }
";
        var compilation = CreateCompilation(source);
        var generator = new ModelBinderProviderGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("new DomainPrimitiveModelBinder<TestNamespace.UserGuid>()");
    }

    [Fact]
    public void Generator_WithValueObject_ShouldGenerateModelBinder()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.AspNetCoreAttribute]
[EricksonLopez.DomainPrimitives.ValueObjectAttribute]
public readonly partial struct Address { }
";
        var compilation = CreateCompilation(source);
        var generator = new ModelBinderProviderGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("new DomainPrimitiveModelBinder<TestNamespace.Address>()");
    }

    [Fact]
    public void Generator_WithShortcutAttribute_ShouldGenerateModelBinder()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.AspNetCoreAttribute]
[EricksonLopez.DomainPrimitives.EmailAttribute]
public readonly partial struct UserEmail { }
";
        var compilation = CreateCompilation(source);
        var generator = new ModelBinderProviderGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("new DomainPrimitiveModelBinder<TestNamespace.UserEmail>()");
    }
}

