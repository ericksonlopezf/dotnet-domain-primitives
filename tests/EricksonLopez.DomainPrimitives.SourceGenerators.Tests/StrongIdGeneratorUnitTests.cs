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

public class StrongIdGeneratorUnitTests
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
    public void ExtractTypeInfo_NullWhenNotStrongIdOrUnsupportedType()
    {
        var source = @"
using System;
using EricksonLopez.DomainPrimitives;

namespace TestNamespace
{
    public record struct PlainStruct;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public record struct IgnoredStruct;

    [StrongId<double>]
    public readonly partial record struct DoubleId;

    [StrongId<short>]
    public readonly partial record struct ShortId;

    [StrongId<byte>]
    public readonly partial record struct ByteId;
}

namespace EricksonLopez.DomainPrimitives
{
    public class StrongIdAttribute<T> : System.Attribute {}
}";
        var model = CreateSemanticModel(source);
        var records = model.SyntaxTree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().ToList();

        var plain = StrongIdGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "PlainStruct"), CancellationToken.None);
        plain.Should().BeNull();

        var obs = StrongIdGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "IgnoredStruct"), CancellationToken.None);
        obs.Should().BeNull();

        var dbl = StrongIdGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "DoubleId"), CancellationToken.None);
        dbl.Should().BeNull();

        var shrt = StrongIdGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "ShortId"), CancellationToken.None);
        shrt.Should().BeNull();

        var bte = StrongIdGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "ByteId"), CancellationToken.None);
        bte.Should().BeNull();
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

        var act = () => StrongIdGenerator.ExtractTypeInfo(model, syntax, cts.Token);
        act.Should().Throw<OperationCanceledException>();
    }

    [Fact]
    public void ExtractTypeInfo_MultiAttribute_FirstGenericStrongIdMatchesAndBreaks()
    {
        var source = @"
using System;
using EricksonLopez.DomainPrimitives;

namespace TestNamespace
{
    [StrongId<Guid>]
    [StrongId<double>]
    public readonly partial record struct MultiAttrId;

    [StrongId<Guid>(OtherProp = ""ignore"", RejectEmpty = true)]
    public readonly partial record struct NamedPropsId;
}

namespace EricksonLopez.DomainPrimitives
{
    public class StrongIdAttribute<T> : System.Attribute 
    { 
        public bool RejectEmpty { get; set; } 
        public string OtherProp { get; set; } 
    }
}";
        var model = CreateSemanticModel(source);
        var syntax = model.SyntaxTree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().First();
        var info = StrongIdGenerator.ExtractTypeInfo(model, syntax, CancellationToken.None);
        info.Should().NotBeNull();
        info!.TypeName.Should().Be("MultiAttrId");
        info.BackingTypeName.Should().Be("Guid");
        info.BackingTypeFullName.Should().Be("System.Guid");

        var namedSyntax = model.SyntaxTree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().First(r => r.Identifier.Text == "NamedPropsId");
        var namedInfo = StrongIdGenerator.ExtractTypeInfo(model, namedSyntax, CancellationToken.None);
        namedInfo.Should().NotBeNull();
        namedInfo!.RejectEmpty.Should().BeTrue();
    }

    [Fact]
    public void ExtractTypeInfo_GuidIntLongStringAndAccessibility_Extracted()
    {
        var source = @"
using System;
using EricksonLopez.DomainPrimitives;

[assembly: DomainPrimitivesDefaults(ExceptionType = typeof(System.ArgumentException))]

namespace TestNamespace
{
    [StrongId<Guid>]
    public readonly partial record struct CustomerId;

    [StrongId<int>(RejectEmpty = false)]
    internal readonly partial record struct OrderId;

    [StrongId<long>]
    protected readonly partial record struct SequenceId;

    [StrongId<string>]
    private readonly partial record struct CodeId;

    public class OuterClass
    {
        [StrongId<Guid>]
        protected internal readonly partial record struct ProtIntNestedId;

        [StrongId<Guid>]
        private protected readonly partial record struct PrivProtNestedId;
    }
}

namespace EricksonLopez.DomainPrimitives
{
    public class DomainPrimitivesDefaultsAttribute : System.Attribute { public System.Type ExceptionType { get; set; } }
    public class StrongIdAttribute<T> : System.Attribute { public bool RejectEmpty { get; set; } }
}";
        var model = CreateSemanticModel(source);
        var records = model.SyntaxTree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().ToList();

        var guidId = StrongIdGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "CustomerId"), CancellationToken.None);
        guidId!.BackingTypeFullName.Should().Be("System.Guid");
        guidId.RejectEmpty.Should().BeTrue();
        guidId.CustomExceptionType.Should().Be("global::System.ArgumentException");
        guidId.Accessibility.Should().Be("public");

        var intId = StrongIdGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "OrderId"), CancellationToken.None);
        intId!.BackingTypeFullName.Should().Be("int");
        intId.RejectEmpty.Should().BeFalse();
        intId.Accessibility.Should().Be("internal");

        var longId = StrongIdGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "SequenceId"), CancellationToken.None);
        longId!.BackingTypeFullName.Should().Be("long");
        longId.Accessibility.Should().Be("protected");

        var strId = StrongIdGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "CodeId"), CancellationToken.None);
        strId!.BackingTypeFullName.Should().Be("string");
        strId.Accessibility.Should().Be("private");

        var protInt = StrongIdGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "ProtIntNestedId"), CancellationToken.None);
        protInt!.Accessibility.Should().Be("protected internal");
        protInt.ContainingTypes.Values.Should().Equal("OuterClass");

        var privProt = StrongIdGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "PrivProtNestedId"), CancellationToken.None);
        privProt!.Accessibility.Should().Be("private protected");
    }

    [Fact]
    public void GenerateStrongId_GuidBackedWithDefaults_Generated()
    {
        var info = new StrongIdTypeInfo(
            Namespace: "TestNamespace",
            TypeName: "GuidStrongId",
            BackingTypeName: "Guid",
            BackingTypeFullName: "System.Guid",
            Accessibility: "public",
            ContainingTypes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            RejectEmpty: true,
            CustomExceptionType: "global::System.InvalidOperationException");

        var code = StrongIdGenerator.GenerateStrongId(info).Replace("\r\n", "\n");

        code.Should().Contain("public readonly partial record struct GuidStrongId :");
        
        var expectedCreate =
            "    /// <summary>Creates a new, unique identifier.</summary>\n" +
            "    [MethodImpl(MethodImplOptions.AggressiveInlining)]\n" +
            "    public static GuidStrongId Create()\n" +
            "    {\n" +
            "        return new(Guid.NewGuid());\n" +
            "    }\n";
        code.Should().Contain(expectedCreate);

        code.Should().Contain("if (value == Guid.Empty)\n            return new PrimitiveError(\"EMPTY\", \"GuidStrongId must not be empty.\");");
        code.Should().Contain("throw new global::System.InvalidOperationException(error.Message);");
        
        var expectedTryParseString =
            "    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out GuidStrongId result)\n" +
            "    {\n" +
            "        if (Guid.TryParse(s, out var parsed))\n" +
            "        {\n" +
            "            result = new GuidStrongId(parsed);\n" +
            "            return true;\n" +
            "        }\n" +
            "        result = default;\n" +
            "        return false;\n" +
            "    }\n";
        code.Should().Contain(expectedTryParseString);

        var expectedTryParseSpan =
            "    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out GuidStrongId result)\n" +
            "    {\n" +
            "        if (Guid.TryParse(s, out var parsed))\n" +
            "        {\n" +
            "            result = new GuidStrongId(parsed);\n" +
            "            return true;\n" +
            "        }\n" +
            "        result = default;\n" +
            "        return false;\n" +
            "    }\n";
        code.Should().Contain(expectedTryParseSpan);

        var expectedTryParseUtf8 =
            "    public static bool TryParse(ReadOnlySpan<byte> utf8, IFormatProvider? provider, out GuidStrongId result)\n" +
            "    {\n" +
            "        if (System.Buffers.Text.Utf8Parser.TryParse(utf8, out Guid parsed, out _))\n" +
            "        {\n" +
            "            result = new GuidStrongId(parsed);\n" +
            "            return true;\n" +
            "        }\n" +
            "        result = default;\n" +
            "        return false;\n" +
            "    }\n";
        code.Should().Contain(expectedTryParseUtf8);

        code.Should().Contain("public string ToString(string? format, IFormatProvider? formatProvider)\n        => _value.ToString(format, formatProvider);\n");
        code.Should().Contain("=> ((ISpanFormattable)_value).TryFormat(destination, out charsWritten, format, provider);");
        code.Should().Contain("=> ((IUtf8SpanFormattable)_value).TryFormat(utf8Destination, out bytesWritten, format, provider);");
        code.Should().Contain("public static explicit operator Guid(GuidStrongId id) => id._value;");
        code.Should().Contain("public static explicit operator GuidStrongId(Guid value) => new(value);");
        code.Should().Contain("private sealed class GuidStrongIdDebugView");
    }

    [Fact]
    public void GenerateStrongId_IntegerBacked_Generated()
    {
        var info = new StrongIdTypeInfo(
            Namespace: "TestNamespace",
            TypeName: "IntStrongId",
            BackingTypeName: "int",
            BackingTypeFullName: "int",
            Accessibility: "internal",
            ContainingTypes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            RejectEmpty: true,
            CustomExceptionType: null);

        var code = StrongIdGenerator.GenerateStrongId(info).Replace("\r\n", "\n");

        code.Should().Contain("internal readonly partial record struct IntStrongId :");
        
        var expectedCreate =
            "    /// <summary>Not supported for integer-backed IDs. Use Create(value) with a known value.</summary>\n" +
            "    public static IntStrongId Create() => throw new NotSupportedException(\"Cannot generate a new IntStrongId. Integer-backed IDs must be assigned by the persistence layer.\");\n";
        code.Should().Contain(expectedCreate);

        code.Should().Contain("if (value == 0)\n            return new PrimitiveError(\"EMPTY\", \"IntStrongId must not be empty.\");");
        code.Should().Contain("throw new DomainPrimitiveValidationException(error);");
        
        var expectedTryParseString =
            "    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out IntStrongId result)\n" +
            "    {\n" +
            "        if (int.TryParse(s, provider, out var parsed))\n" +
            "        {\n" +
            "            result = new IntStrongId(parsed);\n" +
            "            return true;\n" +
            "        }\n" +
            "        result = default;\n" +
            "        return false;\n" +
            "    }\n";
        code.Should().Contain(expectedTryParseString);

        var expectedTryParseSpan =
            "    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out IntStrongId result)\n" +
            "    {\n" +
            "        if (int.TryParse(s, provider, out var parsed))\n" +
            "        {\n" +
            "            result = new IntStrongId(parsed);\n" +
            "            return true;\n" +
            "        }\n" +
            "        result = default;\n" +
            "        return false;\n" +
            "    }\n";
        code.Should().Contain(expectedTryParseSpan);

        var expectedTryParseUtf8 =
            "    public static bool TryParse(ReadOnlySpan<byte> utf8, IFormatProvider? provider, out IntStrongId result)\n" +
            "    {\n" +
            "        if (int.TryParse(utf8, provider, out var parsed))\n" +
            "        {\n" +
            "            result = new IntStrongId(parsed);\n" +
            "            return true;\n" +
            "        }\n" +
            "        result = default;\n" +
            "        return false;\n" +
            "    }\n";
        code.Should().Contain(expectedTryParseUtf8);

        code.Should().Contain("public string ToString(string? format, IFormatProvider? formatProvider)\n        => _value.ToString(format, formatProvider);\n");
        code.Should().Contain("=> _value.TryFormat(destination, out charsWritten, format, provider);");
        code.Should().Contain("=> _value.TryFormat(utf8Destination, out bytesWritten, format, provider);");
    }

    [Fact]
    public void GenerateStrongId_LongBacked_Generated()
    {
        var info = new StrongIdTypeInfo(
            Namespace: "TestNamespace",
            TypeName: "LongStrongId",
            BackingTypeName: "long",
            BackingTypeFullName: "long",
            Accessibility: "public",
            ContainingTypes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            RejectEmpty: true,
            CustomExceptionType: null);

        var code = StrongIdGenerator.GenerateStrongId(info).Replace("\r\n", "\n");

        code.Should().Contain("public string ToString(string? format, IFormatProvider? formatProvider)\n        => _value.ToString(format, formatProvider);\n");
        code.Should().Contain("if (long.TryParse(s, provider, out var parsed))");
        code.Should().Contain("if (long.TryParse(s, provider, out var parsed))");
        code.Should().Contain("if (long.TryParse(utf8, provider, out var parsed))");
    }

    [Fact]
    public void GenerateStrongId_StringBackedWithRejectEmptyFalse_Generated()
    {
        var info = new StrongIdTypeInfo(
            Namespace: "TestNamespace",
            TypeName: "StringStrongId",
            BackingTypeName: "string",
            BackingTypeFullName: "string",
            Accessibility: "public",
            ContainingTypes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            RejectEmpty: false,
            CustomExceptionType: null);

        var code = StrongIdGenerator.GenerateStrongId(info).Replace("\r\n", "\n");

        code.Should().Contain("public static StringStrongId Create() => throw new NotSupportedException(\"Cannot generate a new StringStrongId. String-backed IDs must be assigned explicitly.\");");
        code.Should().Contain("ArgumentNullException.ThrowIfNull(value);");
        code.Should().Contain("validationError = PrimitiveError.None;");
        code.Should().Contain("public static StringStrongId Empty { get => new(string.Empty); }");
        code.Should().Contain("if (s is not null)");
        code.Should().Contain("var parsed = s.ToString();\n        if (parsed.Length > 0)");
        code.Should().Contain("int count = System.Text.Encoding.UTF8.GetCharCount(utf8);");
        code.Should().Contain("public string ToString(string? format, IFormatProvider? formatProvider)\n        => _value.ToString();\n");
        code.Should().Contain("if (_value is null) { charsWritten = 0; return false; }");
        code.Should().Contain("id._value ?? throw new InvalidOperationException(\"Cannot convert a default StringStrongId to string. Check IsDefault before casting.\");");
    }

    [Fact]
    public void GenerateStrongId_StringBackedWithRejectEmptyTrue_Generated()
    {
        var info = new StrongIdTypeInfo(
            Namespace: "TestNamespace",
            TypeName: "StrictStringStrongId",
            BackingTypeName: "string",
            BackingTypeFullName: "string",
            Accessibility: "public",
            ContainingTypes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            RejectEmpty: true,
            CustomExceptionType: null);

        var code = StrongIdGenerator.GenerateStrongId(info).Replace("\r\n", "\n");

        code.Should().Contain("if (string.IsNullOrWhiteSpace(value))\n            return new PrimitiveError(\"EMPTY\", \"StrictStringStrongId must not be empty.\");");
        code.Should().Contain("public static StrictStringStrongId Empty { get => default; }");
    }
}


