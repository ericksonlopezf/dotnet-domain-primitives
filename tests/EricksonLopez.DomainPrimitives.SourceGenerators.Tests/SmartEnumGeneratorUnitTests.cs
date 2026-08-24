// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using AwesomeAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;
using EricksonLopez.DomainPrimitives.Generators.Models;

namespace EricksonLopez.DomainPrimitives.Generators.Tests;

public class SmartEnumGeneratorUnitTests
{
    private static SemanticModel CreateSemanticModel(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(EricksonLopez.DomainPrimitives.SourceGenerators.Tests.TestCompilationHelper.EnsureUsings(source), new CSharpParseOptions(LanguageVersion.CSharp11));
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            new[] { syntaxTree },
            Basic.Reference.Assemblies.Net80.References.All,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        return compilation.GetSemanticModel(syntaxTree);
    }

    [Fact]
    public void ExtractTypeInfo_NullWhenNotSmartEnum()
    {
        var source = @"
using System;

namespace TestNamespace
{
    public record struct PlainStruct;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public record struct IgnoredStruct;
}";
        var model = CreateSemanticModel(source);
        var records = model.SyntaxTree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().ToList();
        var plain = SmartEnumGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "PlainStruct"), CancellationToken.None);
        plain.Should().BeNull();

        var obs = SmartEnumGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "IgnoredStruct"), CancellationToken.None);
        obs.Should().BeNull();
    }

    [Fact]
    public void ExtractTypeInfo_MultiAttribute_FirstGenericSmartEnumMatchesAndBreaks()
    {
        var source = @"
namespace TestNamespace
{
    [EricksonLopez.DomainPrimitives.SmartEnum<int>]
    [NonGenericAttr.SmartEnum]
    public readonly partial record struct MultiAttrEnum;

    public class SmartEnumAttribute : System.Attribute {}
}

namespace EricksonLopez.DomainPrimitives
{
    public class SmartEnumAttribute<T> : System.Attribute {}
}

namespace NonGenericAttr
{
    public class SmartEnumAttribute : System.Attribute {}
}";
        var model = CreateSemanticModel(source);
        var syntax = model.SyntaxTree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().First();
        var info = SmartEnumGenerator.ExtractTypeInfo(model, syntax, CancellationToken.None);
        info.Should().NotBeNull();
        info!.TypeName.Should().Be("MultiAttrEnum");
        info.BackingTypeName.Should().Be("int");
    }

    [Fact]
    public void ExtractTypeInfo_AttributeWithoutAttributeSuffix_Matches()
    {
        var source = @"
namespace TestNamespace
{
    [CustomNs.SmartEnum<string>]
    public readonly partial record struct SuffixlessEnum;
}

namespace CustomNs
{
    public class SmartEnum<T> : System.Attribute {}
}";
        var model = CreateSemanticModel(source);
        var syntax = model.SyntaxTree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().First();
        var info = SmartEnumGenerator.ExtractTypeInfo(model, syntax, CancellationToken.None);
        info.Should().NotBeNull();
        info!.TypeName.Should().Be("SuffixlessEnum");
        info.BackingTypeName.Should().Be("string");
    }

    [Fact]
    public void ExtractTypeInfo_CancellationRequested_Throws()
    {
        var source = @"
namespace TestNamespace
{
    public record struct PlainStruct;
}";
        var model = CreateSemanticModel(source);
        var syntax = model.SyntaxTree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().First();
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var act = () => SmartEnumGenerator.ExtractTypeInfo(model, syntax, cts.Token);
        act.Should().Throw<OperationCanceledException>();
    }

    [Fact]
    public void ExtractTypeInfo_ValueTypeAndRefTypeAndGlobalNamespace_Extracted()
    {
        var source = @"
using EricksonLopez.DomainPrimitives;

[assembly: DomainPrimitivesDefaults(ExceptionType = typeof(System.InvalidOperationException))]

[SmartEnum<int>]
public readonly partial record struct GlobalEnum
{
    public static readonly GlobalEnum First = new(1, nameof(First));
    public static GlobalEnum Second { get; } = new(2, nameof(Second));
    public static readonly int OtherField = 10;
    public static string OtherProp => ""test"";
    public readonly int InstanceField = 5;
}

namespace TestNamespace
{
    [SmartEnum<string>]
    public readonly partial record struct StringSmartEnum
    {
        public static readonly StringSmartEnum Active = new(""ACT"", nameof(Active));
        public static readonly StringSmartEnum Inactive = new(""INA"", nameof(Inactive));
    }
}

namespace EricksonLopez.DomainPrimitives
{
    public class DomainPrimitivesDefaultsAttribute : System.Attribute { public System.Type ExceptionType { get; set; } }
    public class SmartEnumAttribute<T> : System.Attribute {}
}";
        var model = CreateSemanticModel(source);
        var records = model.SyntaxTree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().ToList();

        var globalEnum = SmartEnumGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "GlobalEnum"), CancellationToken.None);
        globalEnum.Should().NotBeNull();
        globalEnum!.Namespace.Should().BeEmpty();
        globalEnum.TypeName.Should().Be("GlobalEnum");
        globalEnum.BackingTypeName.Should().Be("int");
        globalEnum.IsReferenceType.Should().BeFalse();
        globalEnum.MemberNames.Values.Should().Equal("First", "Second");
        globalEnum.CustomExceptionType.Should().Be("global::System.InvalidOperationException");

        var strEnum = SmartEnumGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "StringSmartEnum"), CancellationToken.None);
        strEnum.Should().NotBeNull();
        strEnum!.Namespace.Should().Be("TestNamespace");
        strEnum.TypeName.Should().Be("StringSmartEnum");
        strEnum.BackingTypeName.Should().Be("string");
        strEnum.IsReferenceType.Should().BeTrue();
        strEnum.MemberNames.Values.Should().Equal("Active", "Inactive");
    }

    [Fact]
    public void ExtractTypeInfo_NonGenericSmartEnumAttribute_ReturnsNull()
    {
        var source = @"
namespace TestNamespace
{
    [SmartEnum]
    public record struct NonGenericSmartEnum;

    public class SmartEnumAttribute : System.Attribute {}
}";
        var model = CreateSemanticModel(source);
        var syntax = model.SyntaxTree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().First();
        var info = SmartEnumGenerator.ExtractTypeInfo(model, syntax, CancellationToken.None);
        info.Should().BeNull();
    }

    [Fact]
    public void GenerateSmartEnum_WithValueTypeAndMembers_Generated()
    {
        var info = new SmartEnumTypeInfo(
            Namespace: "TestNamespace",
            TypeName: "StatusEnum",
            BackingTypeName: "int",
            MemberNames: new EquatableArray<string>(ImmutableArray.Create("Pending", "Active", "Completed")),
            IsReferenceType: false,
            CustomExceptionType: "global::System.InvalidOperationException");

        var code = SmartEnumGenerator.GenerateSmartEnum(info).Replace("\r\n", "\n");

        code.Should().Contain("namespace TestNamespace;\n");
        code.Should().Contain("readonly partial record struct StatusEnum :");
        code.Should().Contain("public static readonly IReadOnlyList<StatusEnum> All = new StatusEnum[] { Pending, Active, Completed };");
        code.Should().Contain("if (EqualityComparer<int>.Default.Equals(item.Value, value))");
        code.Should().Contain("throw new global::System.InvalidOperationException($\"No StatusEnum found with value {value}\");");
        code.Should().Contain("throw new global::System.InvalidOperationException($\"No StatusEnum found with name '{name}'\");");

        // Pattern matching
        code.Should().Contain("public TResult Match<TResult>(Func<TResult> whenPending, Func<TResult> whenActive, Func<TResult> whenCompleted)");
        code.Should().Contain("public TResult Map<TResult>(Func<StatusEnum, TResult> whenPending, Func<StatusEnum, TResult> whenActive, Func<StatusEnum, TResult> whenCompleted)");
        code.Should().Contain("public void Switch(Action whenPending, Action whenActive, Action whenCompleted)");

        // TryFormat
        code.Should().Contain("var nameSpan = Name.AsSpan();");
        code.Should().Contain("if (nameSpan.Length <= destination.Length)");

        code.Should().EndWith("}\n\n");
    }

    [Fact]
    public void GenerateSmartEnum_WithReferenceTypeAndNoMembers_Generated()
    {
        var info = new SmartEnumTypeInfo(
            Namespace: "",
            TypeName: "EmptyRefEnum",
            BackingTypeName: "string",
            MemberNames: new EquatableArray<string>(ImmutableArray<string>.Empty),
            IsReferenceType: true,
            CustomExceptionType: null);

        var code = SmartEnumGenerator.GenerateSmartEnum(info).Replace("\r\n", "\n");

        code.Should().NotContain("namespace ;");
        code.Should().Contain("public static readonly IReadOnlyList<EmptyRefEnum> All = Array.Empty<EmptyRefEnum>();");
        code.Should().Contain("if (item.Value is not null && item.Value.Equals(value))");
        code.Should().Contain("throw new ArgumentException($\"No EmptyRefEnum found with value {value}\", nameof(value));");
        code.Should().Contain("throw new ArgumentException($\"No EmptyRefEnum found with name {name}\", nameof(name));");

        // No pattern matching when no members
        code.Should().NotContain("public TResult Match");
        code.Should().NotContain("public TResult Map");
        code.Should().NotContain("public void Switch");

        // Equality for reference type
        code.Should().Contain("return Value is not null ? Value.Equals(other.Value) : other.Value is null;");
        code.Should().Contain("return Value is not null ? Value.GetHashCode() : 0;");

        code.Should().EndWith("}\n\n");
    }
}


