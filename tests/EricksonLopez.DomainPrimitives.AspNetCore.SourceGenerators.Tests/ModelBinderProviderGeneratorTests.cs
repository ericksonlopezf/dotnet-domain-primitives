// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using AwesomeAssertions;
using EricksonLopez.DomainPrimitives.AspNetCore.SourceGenerators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

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
[EricksonLopez.DomainPrimitives.StrongIdAttribute<Guid>]
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

    [Fact]
    public void Generator_WithGlobalNamespaceStruct_GeneratesModelBinderWithoutNamespace()
    {
        string source = @"
[EricksonLopez.DomainPrimitives.StringPrimitiveAttribute]
public readonly partial struct GlobalId { }
";
        var compilation = CreateCompilation(source);
        var generator = new ModelBinderProviderGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("{ typeof(GlobalId), new DomainPrimitiveModelBinder<GlobalId>() },");
        generatedSource.Should().Contain("options.ModelBinderProviders.Insert(0, new GeneratedDomainPrimitiveModelBinderProvider());");
    }

    [Fact]
    public void Generator_WithRecordStruct_ShouldGenerateModelBinder()
    {
        string source = @"
namespace EricksonLopez.DomainPrimitives
{
    public class NumericPrimitiveAttribute : System.Attribute { }
}
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.NumericPrimitiveAttribute]
public readonly partial record struct ItemScore { }
";
        var compilation = CreateCompilation(source);
        var generator = new ModelBinderProviderGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("new DomainPrimitiveModelBinder<TestNamespace.ItemScore>()");
    }

    [Fact]
    public void Generator_WithCodeAttribute_ShouldGenerateModelBinder()
    {
        string source = @"
namespace EricksonLopez.DomainPrimitives
{
    public class CountryCodeAttribute : System.Attribute { }
}
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.CountryCodeAttribute]
public readonly partial record struct CountryCode { }
";
        var compilation = CreateCompilation(source);
        var generator = new ModelBinderProviderGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("new DomainPrimitiveModelBinder<TestNamespace.CountryCode>()");
    }

    [Fact]
    public void Generator_WithClass_ShouldNotGenerateModelBinder()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.StringPrimitiveAttribute]
public class NotAStruct { }
";
        var compilation = CreateCompilation(source);
        var generator = new ModelBinderProviderGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        outputCompilation.SyntaxTrees.Count().Should().Be(2);
    }

    [Fact]
    public void Generator_WithRecordClass_ShouldNotGenerateModelBinder()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.StringPrimitiveAttribute]
public record NotAStructRecord { }
";
        var compilation = CreateCompilation(source);
        var generator = new ModelBinderProviderGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        outputCompilation.SyntaxTrees.Count().Should().Be(2);
    }

    [Fact]
    public void Generator_WithoutPrimitives_ShouldNotGenerateSource()
    {
        string source = @"
namespace TestNamespace;
public struct PlainStruct { }
";
        var compilation = CreateCompilation(source);
        var generator = new ModelBinderProviderGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        outputCompilation.SyntaxTrees.Count().Should().Be(2);
    }

    [Fact]
    public void Generator_WithDuplicatePrimitives_GeneratesDistinctEntries()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.StringPrimitiveAttribute]
public readonly partial record struct DupId { }
";
        var syntaxTrees = new[] { 
            CSharpSyntaxTree.ParseText(source),
            CSharpSyntaxTree.ParseText(source),
            CSharpSyntaxTree.ParseText(@"
namespace EricksonLopez.DomainPrimitives
{
    public class StringPrimitiveAttribute : System.Attribute { }
}")
        };
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .Cast<MetadataReference>()
            .ToArray();

        var compilation = CSharpCompilation.Create("compilation", syntaxTrees, references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var generator = new ModelBinderProviderGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(3).Select(t => t.ToString()));
        int count = generatedSource.Split(new[] { "new DomainPrimitiveModelBinder<TestNamespace.DupId>()" }, StringSplitOptions.None).Length - 1;
        count.Should().Be(1);
    }

    [Fact]
    public void GenerateSourceCode_WithPrimitives_ProducesExactExpectedOutput()
    {
        var code = ModelBinderProviderGenerator.GenerateSourceCode(new[] { new PrimitiveInfo("TestNs", "MyPrim") });
        var expected = @"// <auto-generated/>
#nullable enable
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using EricksonLopez.DomainPrimitives.AspNetCore;

namespace EricksonLopez.DomainPrimitives.AspNetCore.Generated;

/// <summary>
/// A generated ModelBinderProvider that maps domain primitives without reflection.
/// </summary>
public sealed class GeneratedDomainPrimitiveModelBinderProvider : IModelBinderProvider
{
    private readonly Dictionary<Type, IModelBinder> _binders = new()
    {
        { typeof(TestNs.MyPrim), new DomainPrimitiveModelBinder<TestNs.MyPrim>() },
    };

    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        if (_binders.TryGetValue(context.Metadata.ModelType, out var binder))
        {
            return binder;
        }
        return null;
    }
}

/// <summary>
/// Extension methods to register generated domain primitive model binders.
/// </summary>
public static class GeneratedDomainPrimitivesServiceCollectionExtensions
{
    public static IServiceCollection AddGeneratedDomainPrimitivesModelBinding(this IServiceCollection services)
    {
        services.Configure<MvcOptions>(options =>
        {
            options.ModelBinderProviders.Insert(0, new GeneratedDomainPrimitiveModelBinderProvider());
        });
        return services;
    }
}
";
        code.Replace("\r\n", "\n").Trim().Should().Be(expected.Replace("\r\n", "\n").Trim());
    }

    [Fact]
    public void Generator_WithOnlyAspNetCoreAttribute_DoesNotGenerateModelBinder()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.AspNetCoreAttribute]
public readonly partial struct OnlyAspNetCore { }
";
        var compilation = CreateCompilation(source);
        var generator = new ModelBinderProviderGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        outputCompilation.SyntaxTrees.Count().Should().Be(2);
    }

    [Fact]
    public void Generator_WithOnlyDomainPrimitivesDefaultsAttribute_DoesNotGenerateModelBinder()
    {
        string source = @"
namespace EricksonLopez.DomainPrimitives
{
    public class DomainPrimitivesDefaultsAttribute : System.Attribute { }
}
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.DomainPrimitivesDefaultsAttribute]
public readonly partial struct OnlyDefaults { }
";
        var compilation = CreateCompilation(source);
        var generator = new ModelBinderProviderGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        outputCompilation.SyntaxTrees.Count().Should().Be(2);
    }

    [Fact]
    public void Generator_WithAttributeInDifferentNamespace_DoesNotGenerateModelBinder()
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
        var generator = new ModelBinderProviderGenerator();
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
    public class AspNetCoreAttribute : System.Attribute { }
    public class DomainPrimitivesDefaultsAttribute : System.Attribute { }
    public class StringPrimitiveAttribute : System.Attribute { }
}
namespace CustomNamespace
{
    public class StringPrimitiveAttribute : System.Attribute { }
}

[EricksonLopez.DomainPrimitives.StringPrimitive]
[EricksonLopez.DomainPrimitives.AspNetCore]
[EricksonLopez.DomainPrimitives.DomainPrimitivesDefaults]
[CustomNamespace.StringPrimitive]
[System.Serializable]
public struct TestAttributesType { }
";
        var compilation = CreateCompilation(source);
        var typeSymbol = compilation.GetTypeByMetadataName("TestAttributesType")!;
        var attrs = typeSymbol.GetAttributes();

        var stringPrimAttr = attrs.First(a => a.AttributeClass?.Name == "StringPrimitiveAttribute" && a.AttributeClass.ContainingNamespace.Name == "DomainPrimitives");
        var aspNetCoreAttr = attrs.First(a => a.AttributeClass?.Name == "AspNetCoreAttribute");
        var defaultsAttr = attrs.First(a => a.AttributeClass?.Name == "DomainPrimitivesDefaultsAttribute");
        var customAttr = attrs.First(a => a.AttributeClass?.ContainingNamespace.Name == "CustomNamespace");
        var serializableAttr = attrs.First(a => a.AttributeClass?.Name == "SerializableAttribute");

        ModelBinderProviderGenerator.IsDomainPrimitiveAttribute(stringPrimAttr).Should().BeTrue();
        ModelBinderProviderGenerator.IsDomainPrimitiveAttribute(aspNetCoreAttr).Should().BeFalse();
        ModelBinderProviderGenerator.IsDomainPrimitiveAttribute(defaultsAttr).Should().BeFalse();
        ModelBinderProviderGenerator.IsDomainPrimitiveAttribute(customAttr).Should().BeFalse();
        ModelBinderProviderGenerator.IsDomainPrimitiveAttribute(serializableAttr).Should().BeFalse();
    }

    [Fact]
    public void Generator_WithUnresolvedAttribute_DoesNotGenerateModelBinder()
    {
        string source = @"
namespace TestNamespace;
[UnresolvedUnknownAttribute]
public readonly partial struct UnresolvedStruct { }
";
        var syntaxTrees = new[] { CSharpSyntaxTree.ParseText(source) };
        var compilation = CSharpCompilation.Create("compilation", syntaxTrees, null, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var typeSymbol = compilation.GetTypeByMetadataName("TestNamespace.UnresolvedStruct")!;
        var unresolvedAttr = typeSymbol.GetAttributes().First();

        ModelBinderProviderGenerator.IsDomainPrimitiveAttribute(unresolvedAttr).Should().BeFalse();

        var generator = new ModelBinderProviderGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        outputCompilation.SyntaxTrees.Count().Should().Be(1);
    }
}




