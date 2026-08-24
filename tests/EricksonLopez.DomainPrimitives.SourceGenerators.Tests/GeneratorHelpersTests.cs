// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Immutable;
using System.Linq;
using EricksonLopez.DomainPrimitives.Generators;
using EricksonLopez.DomainPrimitives.Generators.Models;
using AwesomeAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace EricksonLopez.DomainPrimitives.SourceGenerators.Tests;

public class GeneratorHelpersTests
{
    [Fact]
    public void ResolveSpecialType_ResolvesAllPrimitiveSpecialTypes()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"
namespace MyNamespace
{
    public class CustomClass { }
}
");
        var compilation = CSharpCompilation.Create(
            "HelperTests",
            new[] { syntaxTree },
            Basic.Reference.Assemblies.Net80.References.All,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        GeneratorHelpers.ResolveSpecialType(compilation.GetSpecialType(SpecialType.System_String)).Should().Be("string");
        GeneratorHelpers.ResolveSpecialType(compilation.GetSpecialType(SpecialType.System_Int32)).Should().Be("int");
        GeneratorHelpers.ResolveSpecialType(compilation.GetSpecialType(SpecialType.System_Int64)).Should().Be("long");
        GeneratorHelpers.ResolveSpecialType(compilation.GetSpecialType(SpecialType.System_Int16)).Should().Be("short");
        GeneratorHelpers.ResolveSpecialType(compilation.GetSpecialType(SpecialType.System_Byte)).Should().Be("byte");
        GeneratorHelpers.ResolveSpecialType(compilation.GetSpecialType(SpecialType.System_UInt32)).Should().Be("uint");
        GeneratorHelpers.ResolveSpecialType(compilation.GetSpecialType(SpecialType.System_UInt64)).Should().Be("ulong");
        GeneratorHelpers.ResolveSpecialType(compilation.GetSpecialType(SpecialType.System_UInt16)).Should().Be("ushort");
        GeneratorHelpers.ResolveSpecialType(compilation.GetSpecialType(SpecialType.System_SByte)).Should().Be("sbyte");
        GeneratorHelpers.ResolveSpecialType(compilation.GetSpecialType(SpecialType.System_Single)).Should().Be("float");
        GeneratorHelpers.ResolveSpecialType(compilation.GetSpecialType(SpecialType.System_Double)).Should().Be("double");
        GeneratorHelpers.ResolveSpecialType(compilation.GetSpecialType(SpecialType.System_Decimal)).Should().Be("decimal");
        GeneratorHelpers.ResolveSpecialType(compilation.GetSpecialType(SpecialType.System_Boolean)).Should().Be("bool");
        GeneratorHelpers.ResolveSpecialType(compilation.GetSpecialType(SpecialType.System_Char)).Should().Be("char");
        GeneratorHelpers.ResolveSpecialType(compilation.GetSpecialType(SpecialType.System_Object)).Should().Be("object");

        var customType = compilation.GetTypeByMetadataName("MyNamespace.CustomClass");
        customType.Should().NotBeNull();
        GeneratorHelpers.ResolveSpecialType(customType!).Should().Be("MyNamespace.CustomClass");
    }

    [Fact]
    public void ExtractAssemblyDefaults_ExtractsAllAttributesProperly()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"
using System;
using EricksonLopez.DomainPrimitives;

[assembly: System.Reflection.AssemblyTitle(""MyAssembly"")]
[assembly: DomainPrimitivesDefaults(Trim = true, NotEmpty = true, MaxLength = 128, ExceptionType = typeof(ArgumentException))]

namespace EricksonLopez.DomainPrimitives
{
    public class DomainPrimitivesDefaultsAttribute : Attribute
    {
        public bool Trim { get; set; }
        public bool NotEmpty { get; set; }
        public int MaxLength { get; set; }
        public Type ExceptionType { get; set; }
    }
}
");
        var compilation = CSharpCompilation.Create(
            "DefaultsTests",
            new[] { syntaxTree },
            Basic.Reference.Assemblies.Net80.References.All,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var defaults = GeneratorHelpers.ExtractAssemblyDefaults(compilation);
        defaults.Trim.Should().BeTrue();
        defaults.NotEmpty.Should().BeTrue();
        defaults.MaxLength.Should().Be(128);
        defaults.ExceptionTypeFullName.Should().Contain("ArgumentException");
    }

    [Fact]
    public void ExtractAssemblyDefaults_WithSuffixlessAttributeInDifferentNamespace_IsIgnored()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"
using System;
using OtherNamespace;

[assembly: DomainPrimitivesDefaults(Trim = true)]

namespace OtherNamespace
{
    public class DomainPrimitivesDefaultsAttribute : Attribute
    {
        public bool Trim { get; set; }
    }
}
");
        var compilation = CSharpCompilation.Create(
            "OtherDefaultsTests",
            new[] { syntaxTree },
            Basic.Reference.Assemblies.Net80.References.All,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var defaults = GeneratorHelpers.ExtractAssemblyDefaults(compilation);
        defaults.Trim.Should().BeFalse();
    }

    [Fact]
    public void ExtractAssemblyDefaults_WithSuffixlessAttributeName_Matches()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"
using System;
using EricksonLopez.DomainPrimitives;

[assembly: DomainPrimitivesDefaults(Trim = true)]

namespace EricksonLopez.DomainPrimitives
{
    public class DomainPrimitivesDefaults : Attribute
    {
        public bool Trim { get; set; }
    }
}
");
        var compilation = CSharpCompilation.Create(
            "SuffixlessDefaultsTests",
            new[] { syntaxTree },
            Basic.Reference.Assemblies.Net80.References.All,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var defaults = GeneratorHelpers.ExtractAssemblyDefaults(compilation);
        defaults.Trim.Should().BeTrue();
    }

    [Fact]
    public void ExtractAssemblyDefaults_WhenNoAttribute_ReturnsDefaultValues()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText("namespace EmptyNamespace { }");
        var compilation = CSharpCompilation.Create(
            "EmptyTests",
            new[] { syntaxTree },
            Basic.Reference.Assemblies.Net80.References.All,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var defaults = GeneratorHelpers.ExtractAssemblyDefaults(compilation);
        defaults.Trim.Should().BeFalse();
        defaults.NotEmpty.Should().BeFalse();
        defaults.MaxLength.Should().BeNull();
        defaults.ExceptionTypeFullName.Should().BeNull();
    }

    [Theory]
    [InlineData("string", "writer.WriteStringValue(value.Value);")]
    [InlineData("int", "writer.WriteNumberValue(value.Value);")]
    [InlineData("long", "throw new global::System.Text.Json.JsonException($\"Invalid TestPrimitive: expected long.\");")]
    [InlineData("decimal", "throw new global::System.Text.Json.JsonException($\"Invalid TestPrimitive: expected decimal.\");")]
    [InlineData("float", "writer.WriteNumberValue(value.Value);")]
    [InlineData("double", "writer.WriteNumberValue(value.Value);")]
    [InlineData("bool", "writer.WriteBooleanValue(value.Value);")]
    [InlineData("System.Guid", "throw new global::System.Text.Json.JsonException($\"Invalid TestPrimitive: expected Guid.\");")]
    [InlineData("global::System.Guid", "throw new global::System.Text.Json.JsonException($\"Invalid TestPrimitive: expected Guid.\");")]
    [InlineData("System.DateTime", "throw new global::System.Text.Json.JsonException($\"Invalid TestPrimitive: expected DateTime.\");")]
    [InlineData("global::System.DateTime", "throw new global::System.Text.Json.JsonException($\"Invalid TestPrimitive: expected DateTime.\");")]
    [InlineData("System.DateTimeOffset", "throw new global::System.Text.Json.JsonException($\"Invalid TestPrimitive: expected DateTimeOffset.\");")]
    [InlineData("global::System.DateTimeOffset", "throw new global::System.Text.Json.JsonException($\"Invalid TestPrimitive: expected DateTimeOffset.\");")]
    [InlineData("CustomBackingType", "global::System.Text.Json.JsonSerializer.Serialize(writer, value.Value, options);")]
    public void GenerateJsonConverter_ProducesValidConverterForType(string backingType, string expectedContent)
    {
        var sb = new SourceBuilder();
        GeneratorHelpers.GenerateJsonConverter(sb, "TestPrimitive", backingType);
        var source = sb.ToString();

        source.Should().Contain("TestPrimitiveJsonConverter");
        source.Should().Contain($"global::System.Text.Json.Serialization.JsonConverter<TestPrimitive>");
        source.Should().Contain("public override TestPrimitive Read");
        source.Should().Contain("public override void Write");
        source.Should().Contain(expectedContent);
        if (backingType != "string")
        {
            source.Should().Contain("if (TestPrimitive.TryCreate(value, out var result, out var err)) return result;");
            source.Should().Contain("throw new global::System.Text.Json.JsonException($\"Invalid TestPrimitive: {err.Message}\");");
        }
    }

    [Fact]
    public void TypeConverterTemplate_GeneratesStringAndSystemStringAndOtherTypes()
    {
        var sbStr = new SourceBuilder();
        EricksonLopez.DomainPrimitives.Generators.Shared.TypeConverterTemplate.GenerateTypeConverter(sbStr, "MyPrim", "string");
        var strOut = sbStr.ToString();
        strOut.Should().Contain("sourceType == typeof(string) || base.CanConvertFrom");
        strOut.Should().Contain("if (value is string s) return MyPrim.Create(s);");
        strOut.Should().Contain("destinationType == typeof(string) || base.CanConvertTo");
        strOut.Should().Contain("if (destinationType == typeof(string)) return id.Value;");

        var sbSysStr = new SourceBuilder();
        EricksonLopez.DomainPrimitives.Generators.Shared.TypeConverterTemplate.GenerateTypeConverter(sbSysStr, "MyPrim", "System.String");
        var sysStrOut = sbSysStr.ToString();
        sysStrOut.Should().Contain("sourceType == typeof(string) || base.CanConvertFrom");
        sysStrOut.Should().Contain("if (value is string s) return MyPrim.Create(s);");
        sysStrOut.Should().Contain("destinationType == typeof(string) || base.CanConvertTo");
        sysStrOut.Should().Contain("if (destinationType == typeof(string)) return id.Value;");

        var sbInt = new SourceBuilder();
        EricksonLopez.DomainPrimitives.Generators.Shared.TypeConverterTemplate.GenerateTypeConverter(sbInt, "MyPrim", "int");
        var intOut = sbInt.ToString();
        intOut.Should().Contain("sourceType == typeof(int)");
        intOut.Should().Contain("if (value is int v) return MyPrim.Create(v);");
        intOut.Should().Contain("destinationType == typeof(int)");
        intOut.Should().Contain("if (destinationType == typeof(int)) return id.Value;");
    }

    [Fact]
    public void ExtractAssemblyDefaults_WhenAttributeWithoutKnownClass_HandledSafely()
    {
        // Attribute with syntax errors or unresolvable type has null or error AttributeClass
        var syntaxTree = CSharpSyntaxTree.ParseText(@"[assembly: NonExistentAttributeNamespace.UnknownAttribute]");
        var compilation = CSharpCompilation.Create(
            "NullAttrClassTest",
            new[] { syntaxTree },
            Basic.Reference.Assemblies.Net80.References.All,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var defaults = GeneratorHelpers.ExtractAssemblyDefaults(compilation);
        defaults.Trim.Should().BeFalse();
    }

    [Fact]
    public void GeneratorShared_IsReadonlyRecordStruct_ChecksProperly()
    {
        var classTree = CSharpSyntaxTree.ParseText("readonly record class RecordClass;");
        var classNode = classTree.GetRoot().DescendantNodes().First();
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.IsReadonlyRecordStruct(classNode, default).Should().BeFalse();

        var structTree = CSharpSyntaxTree.ParseText("readonly record struct RecordStruct;");
        var structNode = structTree.GetRoot().DescendantNodes().First();
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.IsReadonlyRecordStruct(structNode, default).Should().BeTrue();

        var nonReadonlyTree = CSharpSyntaxTree.ParseText("record struct NonRoStruct;");
        var nonRoNode = nonReadonlyTree.GetRoot().DescendantNodes().First();
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.IsReadonlyRecordStruct(nonRoNode, default).Should().BeFalse();

        var normalClassTree = CSharpSyntaxTree.ParseText("class NormalClass;");
        var normalClassNode = normalClassTree.GetRoot().DescendantNodes().First();
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.IsReadonlyRecordStruct(normalClassNode, default).Should().BeFalse();
    }

    [Fact]
    public void StrongIdTypeInfo_Flags_AreAccurate()
    {
        var guidId = new StrongIdTypeInfo("NS", "OrderId", "Guid", "System.Guid", "public", new EquatableArray<string>([]), true);
        guidId.IsGuidBacked.Should().BeTrue();
        guidId.IsStringBacked.Should().BeFalse();
        guidId.IsIntegerBacked.Should().BeFalse();

        var stringId = new StrongIdTypeInfo("NS", "CodeId", "string", "string", "public", new EquatableArray<string>([]), true);
        stringId.IsGuidBacked.Should().BeFalse();
        stringId.IsStringBacked.Should().BeTrue();
        stringId.IsIntegerBacked.Should().BeFalse();

        var intId = new StrongIdTypeInfo("NS", "UserId", "int", "System.Int32", "public", new EquatableArray<string>([]), true);
        intId.IsGuidBacked.Should().BeFalse();
        intId.IsStringBacked.Should().BeFalse();
        intId.IsIntegerBacked.Should().BeTrue();

        var longId = new StrongIdTypeInfo("NS", "BigId", "long", "long", "public", new EquatableArray<string>([]), true);
        longId.IsIntegerBacked.Should().BeTrue();
    }

    [Fact]
    public void SmartEnumTypeInfo_FullName_FormatsCorrectly()
    {
        var enumWithNs = new SmartEnumTypeInfo("App.Domain", "Status", "int", new EquatableArray<string>(["Active", "Inactive"]), false);
        enumWithNs.FullName.Should().Be("App.Domain.Status");

        var enumNoNs = new SmartEnumTypeInfo("", "Status", "int", new EquatableArray<string>(["Active"]), false);
        enumNoNs.FullName.Should().Be("Status");
    }

    [Fact]
    public void ExtractTypeInfo_WhenSymbolIsNull_ReturnsNull()
    {
        var tree = CSharpSyntaxTree.ParseText("public record struct;");
        var compilation = CSharpCompilation.Create("Comp", new[] { tree }, Basic.Reference.Assemblies.Net80.References.All);
        var semanticModel = compilation.GetSemanticModel(tree);
        var recordSyntax = tree.GetRoot().DescendantNodes().OfType<Microsoft.CodeAnalysis.CSharp.Syntax.RecordDeclarationSyntax>().FirstOrDefault();
        if (recordSyntax is not null)
        {
            DatePrimitiveGenerator.ExtractTypeInfo(semanticModel, recordSyntax, System.Threading.CancellationToken.None).Should().BeNull();
            NumericPrimitiveGenerator.ExtractTypeInfo(semanticModel, recordSyntax, System.Threading.CancellationToken.None).Should().BeNull();
            SmartEnumGenerator.ExtractTypeInfo(semanticModel, recordSyntax, System.Threading.CancellationToken.None).Should().BeNull();
            StrongIdGenerator.ExtractTypeInfo(semanticModel, recordSyntax, System.Threading.CancellationToken.None).Should().BeNull();
            ValueObjectGenerator.ExtractTypeInfo(semanticModel, recordSyntax, System.Threading.CancellationToken.None).Should().BeNull();
        }
    }
}

