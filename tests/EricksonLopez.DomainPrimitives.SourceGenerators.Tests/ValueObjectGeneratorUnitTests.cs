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

public class ValueObjectGeneratorUnitTests
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
    public void ExtractTypeInfo_NullWhenNotValueObject()
    {
        var source = @"
namespace TestNamespace
{
    public record struct PlainStruct;
}";
        var model = CreateSemanticModel(source);
        var syntax = model.SyntaxTree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().First();
        var info = ValueObjectGenerator.ExtractTypeInfo(model, syntax, CancellationToken.None);
        info.Should().BeNull();
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

        var act = () => ValueObjectGenerator.ExtractTypeInfo(model, syntax, cts.Token);
        act.Should().Throw<OperationCanceledException>();
    }

    [Fact]
    public void ExtractTypeInfo_PropertiesAndAccessibilityAndNesting_Extracted()
    {
        var source = @"
using EricksonLopez.DomainPrimitives;

namespace TestNamespace
{
    [ValueObject]
    public readonly partial record struct Coordinate
    {
        public double Latitude { get; init; }
        public double Longitude { get; init; }
        internal int InternalProp { get; init; }
        public static string StaticProp => ""static"";
    }

    public class OuterClass
    {
        [ValueObject]
        internal readonly partial record struct InternalNestedVo;

        [ValueObject]
        protected readonly partial record struct ProtectedNestedVo;

        [ValueObject]
        private readonly partial record struct PrivateNestedVo;

        [ValueObject]
        protected internal readonly partial record struct ProtIntNestedVo;

        [ValueObject]
        private protected readonly partial record struct PrivProtNestedVo;
    }
}

namespace EricksonLopez.DomainPrimitives
{
    public class ValueObjectAttribute : System.Attribute {}
}";
        var model = CreateSemanticModel(source);
        var records = model.SyntaxTree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().ToList();

        var coord = ValueObjectGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "Coordinate"), CancellationToken.None);
        coord!.Accessibility.Should().Be("public");
        coord.Properties.Length.Should().Be(2);
        coord.Properties.Values[0].Name.Should().Be("Latitude");
        coord.Properties.Values[0].CamelCaseName.Should().Be("latitude");
        coord.Properties.Values[1].Name.Should().Be("Longitude");
        coord.Properties.Values[1].CamelCaseName.Should().Be("longitude");

        var intern = ValueObjectGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "InternalNestedVo"), CancellationToken.None);
        intern!.Accessibility.Should().Be("internal");
        intern.ContainingTypes.Values.Should().Equal("OuterClass");

        var prot = ValueObjectGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "ProtectedNestedVo"), CancellationToken.None);
        prot!.Accessibility.Should().Be("protected");

        var priv = ValueObjectGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "PrivateNestedVo"), CancellationToken.None);
        priv!.Accessibility.Should().Be("private");

        var protInt = ValueObjectGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "ProtIntNestedVo"), CancellationToken.None);
        protInt!.Accessibility.Should().Be("protected internal");

        var privProt = ValueObjectGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "PrivProtNestedVo"), CancellationToken.None);
        privProt!.Accessibility.Should().Be("private protected");
    }

    [Fact]
    public void GenerateValueObject_WithSmallPropertiesList_Generated()
    {
        var props = ImmutableArray.Create(
            new ValueObjectPropertyInfo("Street", "string", "street"),
            new ValueObjectPropertyInfo("City", "string", "city"),
            new ValueObjectPropertyInfo("ZipCode", "int", "zipCode"));

        var info = new ValueObjectTypeInfo(
            Namespace: "TestNamespace",
            TypeName: "Address",
            Accessibility: "public",
            ContainingTypes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            Properties: new EquatableArray<ValueObjectPropertyInfo>(props));

        var code = ValueObjectGenerator.GenerateValueObject(info).Replace("\r\n", "\n");

        code.Should().Contain("public readonly partial record struct Address :");
        code.Should().Contain("public bool IsDefault => Street == default && City == default && ZipCode == default;");
        code.Should().Contain("static partial void Validate(ref Address value, ref global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError error);");

        var expectedCreate =
            "    public static Address Create(string street, string city, int zipCode)\n" +
            "    {\n" +
            "        var error = global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError.None;\n" +
            "        var instance = new Address { Street = street, City = city, ZipCode = zipCode };\n" +
            "        Validate(ref instance, ref error);\n" +
            "        if (error.IsError)\n" +
            "        {\n" +
            "            throw new DomainPrimitiveValidationException(error);\n" +
            "        }\n" +
            "        return instance;\n" +
            "    }\n";
        code.Should().Contain(expectedCreate);

        var expectedTryCreate =
            "    public static bool TryCreate(string street, string city, int zipCode, out Address result, out global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError validationError)\n" +
            "    {\n" +
            "        validationError = global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError.None;\n" +
            "        var instance = new Address { Street = street, City = city, ZipCode = zipCode };\n" +
            "        Validate(ref instance, ref validationError);\n" +
            "        if (validationError.IsError)\n" +
            "        {\n" +
            "            result = default;\n" +
            "            return false;\n" +
            "        }\n" +
            "        result = instance;\n" +
            "        return true;\n" +
            "    }\n";
        code.Should().Contain(expectedTryCreate);

        // <= 3 props uses inline concat
        code.Should().Contain("return \"Address { \" + \"Street = \" + (Street?.ToString() ?? \"null\") + \", \" + \"City = \" + (City?.ToString() ?? \"null\") + \", \" + \"ZipCode = \" + (ZipCode?.ToString() ?? \"null\") + \" }\";");
    }

    [Fact]
    public void GenerateValueObject_WithZeroProperties_Generated()
    {
        var info = new ValueObjectTypeInfo(
            Namespace: "TestNamespace",
            TypeName: "EmptyVo",
            Accessibility: "internal",
            ContainingTypes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            Properties: new EquatableArray<ValueObjectPropertyInfo>(ImmutableArray<ValueObjectPropertyInfo>.Empty));

        var code = ValueObjectGenerator.GenerateValueObject(info).Replace("\r\n", "\n");

        code.Should().Contain("public bool IsDefault => true;");
        code.Should().NotContain("public static EmptyVo Create(");
        code.Should().NotContain("public static bool TryCreate(");
        code.Should().Contain("return \"EmptyVo { }\";");
    }

    [Fact]
    public void GenerateValueObject_WithMoreThanThreeProperties_UsesStringBuilder()
    {
        var props = ImmutableArray.Create(
            new ValueObjectPropertyInfo("P1", "string", "p1"),
            new ValueObjectPropertyInfo("P2", "string", "p2"),
            new ValueObjectPropertyInfo("P3", "string", "p3"),
            new ValueObjectPropertyInfo("P4", "string", "p4"));

        var info = new ValueObjectTypeInfo(
            Namespace: "TestNamespace",
            TypeName: "LargeVo",
            Accessibility: "public",
            ContainingTypes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            Properties: new EquatableArray<ValueObjectPropertyInfo>(props));

        var code = ValueObjectGenerator.GenerateValueObject(info).Replace("\r\n", "\n");

        var expectedToString =
            "    public override string ToString()\n" +
            "    {\n" +
            "        var buf = new global::System.Text.StringBuilder();\n" +
            "        buf.Append(\"LargeVo { \");\n" +
            "        buf.Append(\"P1 = \");\n" +
            "        buf.Append(P1);\n" +
            "        buf.Append(\", \");\n" +
            "        buf.Append(\"P2 = \");\n" +
            "        buf.Append(P2);\n" +
            "        buf.Append(\", \");\n" +
            "        buf.Append(\"P3 = \");\n" +
            "        buf.Append(P3);\n" +
            "        buf.Append(\", \");\n" +
            "        buf.Append(\"P4 = \");\n" +
            "        buf.Append(P4);\n" +
            "        buf.Append(\" }\");\n" +
            "        return buf.ToString();\n" +
            "    }\n";
        code.Should().Contain(expectedToString);
    }

    [Fact]
    public void GenerateValueObject_WithCustomExceptionType_EmitsCustomExceptionThrow()
    {
        var props = ImmutableArray.Create(new ValueObjectPropertyInfo("Street", "string", "street"));

        var info = new ValueObjectTypeInfo(
            Namespace: "TestNamespace",
            TypeName: "AddressVo",
            Accessibility: "public",
            ContainingTypes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            Properties: new EquatableArray<ValueObjectPropertyInfo>(props),
            CustomExceptionType: "global::System.InvalidOperationException");

        var code = ValueObjectGenerator.GenerateValueObject(info).Replace("\r\n", "\n");

        code.Should().Contain("throw new global::System.InvalidOperationException(error.Message);");
        code.Should().NotContain("throw new DomainPrimitiveValidationException(error);");
    }

    [Fact]
    public void ExtractTypeInfo_WithAssemblyDefaultsCustomExceptionType_PopulatesCustomExceptionType()
    {
        var source = @"
using System;
using EricksonLopez.DomainPrimitives;

[assembly: DomainPrimitivesDefaults(ExceptionType = typeof(InvalidOperationException))]

namespace TestNamespace
{
    [ValueObject]
    public readonly partial record struct ConfiguredVo
    {
        public string Name { get; init; }
    }
}

namespace EricksonLopez.DomainPrimitives
{
    public class DomainPrimitivesDefaultsAttribute : System.Attribute { public Type ExceptionType { get; set; } }
    public class ValueObjectAttribute : System.Attribute {}
}";
        var model = CreateSemanticModel(source);
        var syntax = model.SyntaxTree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().First(r => r.Identifier.Text == "ConfiguredVo");
        var info = ValueObjectGenerator.ExtractTypeInfo(model, syntax, CancellationToken.None);

        info.Should().NotBeNull();
        info!.CustomExceptionType.Should().Contain("InvalidOperationException");
    }
}



