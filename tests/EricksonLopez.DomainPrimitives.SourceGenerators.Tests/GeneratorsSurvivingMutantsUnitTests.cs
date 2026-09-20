// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Immutable;
using System.Linq;
using AwesomeAssertions;
using EricksonLopez.DomainPrimitives.Generators;
using EricksonLopez.DomainPrimitives.Generators.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace EricksonLopez.DomainPrimitives.SourceGenerators.Tests;

public class GeneratorsSurvivingMutantsUnitTests
{
    private static string Normalize(string code) =>
        string.Join("\n", code.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
            .Select(l => l.TrimEnd()));

    #region GeneratorHelpers.GenerateJsonConverter Tests

    [Theory]
    [InlineData("Guid")]
    [InlineData("global::Guid")]
    [InlineData("System.Guid")]
    [InlineData("global::System.Guid")]
    public void GeneratorHelpers_GenerateJsonConverter_GuidVariants_EmitsExpectedCode(string backing)
    {
        var sb = new SourceBuilder();
        GeneratorHelpers.GenerateJsonConverter(sb, "TestId", backing);
        var code = sb.ToString();

        code.Should().Contain("if (!reader.TryGetGuid(out var value)) throw new global::System.Text.Json.JsonException($\"Invalid TestId: expected Guid.\");");
        code.Should().Contain("#if NET8_0_OR_GREATER");
        code.Should().Contain("writer.WriteStringValue(value.Value);");
        code.Should().Contain("#else");
        code.Should().Contain("writer.WriteStringValue(value.Value.ToString(\"D\"));");
        code.Should().Contain("#endif");
    }

    [Theory]
    [InlineData("DateTime")]
    [InlineData("global::DateTime")]
    [InlineData("System.DateTime")]
    [InlineData("global::System.DateTime")]
    public void GeneratorHelpers_GenerateJsonConverter_DateTimeVariants_EmitsExpectedCode(string backing)
    {
        var sb = new SourceBuilder();
        GeneratorHelpers.GenerateJsonConverter(sb, "TestDt", backing);
        var code = sb.ToString();

        code.Should().Contain("if (!reader.TryGetDateTime(out var value)) throw new global::System.Text.Json.JsonException($\"Invalid TestDt: expected DateTime.\");");
        code.Should().Contain("writer.WriteStringValue(value.Value);");
    }

    [Theory]
    [InlineData("DateTimeOffset")]
    [InlineData("global::DateTimeOffset")]
    [InlineData("System.DateTimeOffset")]
    [InlineData("global::System.DateTimeOffset")]
    public void GeneratorHelpers_GenerateJsonConverter_DateTimeOffsetVariants_EmitsExpectedCode(string backing)
    {
        var sb = new SourceBuilder();
        GeneratorHelpers.GenerateJsonConverter(sb, "TestDto", backing);
        var code = sb.ToString();

        code.Should().Contain("if (!reader.TryGetDateTimeOffset(out var value)) throw new global::System.Text.Json.JsonException($\"Invalid TestDto: expected DateTimeOffset.\");");
        code.Should().Contain("writer.WriteStringValue(value.Value);");
    }

    [Theory]
    [InlineData("DateOnly")]
    [InlineData("global::DateOnly")]
    [InlineData("System.DateOnly")]
    [InlineData("global::System.DateOnly")]
    public void GeneratorHelpers_GenerateJsonConverter_DateOnlyVariants_EmitsExpectedCode(string backing)
    {
        var sb = new SourceBuilder();
        GeneratorHelpers.GenerateJsonConverter(sb, "TestDo", backing);
        var code = sb.ToString();

        code.Should().Contain("#if NET8_0_OR_GREATER");
        code.Should().Contain("if (stringValue is null || !global::System.DateOnly.TryParse(stringValue, global::System.Globalization.CultureInfo.InvariantCulture, global::System.Globalization.DateTimeStyles.None, out var value)) throw new global::System.Text.Json.JsonException($\"Invalid TestDo: expected valid DateOnly ISO-8601 string.\");");
        code.Should().Contain("writer.WriteStringValue(value.Value.ToString(\"O\", global::System.Globalization.CultureInfo.InvariantCulture));");
    }

    [Theory]
    [InlineData("TimeOnly")]
    [InlineData("global::TimeOnly")]
    [InlineData("System.TimeOnly")]
    [InlineData("global::System.TimeOnly")]
    public void GeneratorHelpers_GenerateJsonConverter_TimeOnlyVariants_EmitsExpectedCode(string backing)
    {
        var sb = new SourceBuilder();
        GeneratorHelpers.GenerateJsonConverter(sb, "TestTo", backing);
        var code = sb.ToString();

        code.Should().Contain("#if NET8_0_OR_GREATER");
        code.Should().Contain("if (stringValue is null || !global::System.TimeOnly.TryParse(stringValue, global::System.Globalization.CultureInfo.InvariantCulture, global::System.Globalization.DateTimeStyles.None, out var value)) throw new global::System.Text.Json.JsonException($\"Invalid TestTo: expected valid TimeOnly ISO-8601 string.\");");
        code.Should().Contain("writer.WriteStringValue(value.Value.ToString(\"O\", global::System.Globalization.CultureInfo.InvariantCulture));");
    }

    [Theory]
    [InlineData("byte")]
    [InlineData("sbyte")]
    [InlineData("short")]
    [InlineData("ushort")]
    [InlineData("uint")]
    [InlineData("ulong")]
    public void GeneratorHelpers_GenerateJsonConverter_NumericTypes_EmitsNumberValue(string backing)
    {
        var sb = new SourceBuilder();
        GeneratorHelpers.GenerateJsonConverter(sb, "TestNum", backing);
        var code = sb.ToString();

        code.Should().Contain("writer.WriteNumberValue(value.Value);");
    }

    [Fact]
    public void GeneratorHelpers_GenerateJsonConverter_StringVariants_SupportsUtf8SpanAndRejectsNull()
    {
        // supportsUtf8Span = false, rejectsNull = true
        var sb1 = new SourceBuilder();
        GeneratorHelpers.GenerateJsonConverter(sb1, "TestStr1", "string", supportsUtf8Span: false, rejectsNull: true);
        var code1 = sb1.ToString();
        code1.Should().Contain("if (stringValue is null) throw new global::System.Text.Json.JsonException($\"Cannot deserialize null into non-nullable domain primitive 'TestStr1'.\");");

        // supportsUtf8Span = false, rejectsNull = false
        var sb2 = new SourceBuilder();
        GeneratorHelpers.GenerateJsonConverter(sb2, "TestStr2", "string", supportsUtf8Span: false, rejectsNull: false);
        var code2 = sb2.ToString();
        code2.Should().Contain("if (stringValue is null) return default;");

        // Fallback for custom type
        var sb3 = new SourceBuilder();
        GeneratorHelpers.GenerateJsonConverter(sb3, "TestCustom", "MyCustomType");
        var code3 = sb3.ToString();
        code3.Should().Contain("var value = global::System.Text.Json.JsonSerializer.Deserialize<MyCustomType>(ref reader, options);");
    }

    #endregion

    #region DatePrimitiveGenerator Tests

    [Theory]
    [InlineData("None", "System.DateTime")]
    [InlineData("None", "DateTime")]
    [InlineData("DateTime", "System.DateTime")]
    [InlineData("DateTime", "DateTime")]
    public void DatePrimitiveGenerator_DateTimeVariants_EmitsExactSwitchBlockInAllFactories(string kind, string backing)
    {
        var info = new DatePrimitiveTypeInfo(
            Namespace: "TestNamespace",
            TypeName: "OrderDate",
            BackingTypeName: backing,
            Accessibility: "public",
            ContainingTypes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            Kind: kind,
            PastOnly: false,
            FutureOnly: false,
            MaxAge: null,
            DomainShortcut: null,
            CustomExceptionType: null);

        var code = DatePrimitiveGenerator.GenerateDatePrimitive(info);

        code.Should().Contain("value = value.Kind switch");
        code.Should().Contain("DateTimeKind.Unspecified => DateTime.SpecifyKind(value, DateTimeKind.Utc),");
        code.Should().Contain("DateTimeKind.Local => value.ToUniversalTime(),");
        code.Should().Contain("_ => value");
    }

    [Fact]
    public void DatePrimitiveGenerator_WithCustomExceptionType_EmitsExactExceptionThrow()
    {
        var info = new DatePrimitiveTypeInfo(
            Namespace: "TestNamespace",
            TypeName: "BirthDate",
            BackingTypeName: "System.DateOnly",
            Accessibility: "public",
            ContainingTypes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            Kind: "DateOnly",
            PastOnly: true,
            FutureOnly: false,
            MaxAge: null,
            DomainShortcut: null,
            CustomExceptionType: "MyDomainException");

        var code = DatePrimitiveGenerator.GenerateDatePrimitive(info);

        code.Should().Contain("if (error.IsError) throw new MyDomainException(error.Message);");
    }

    [Fact]
    public void DatePrimitiveGenerator_TimeOnlyAndDateTimeOffset_EmitsTimeProviderCall()
    {
        var infoTime = new DatePrimitiveTypeInfo(
            Namespace: "TestNamespace",
            TypeName: "ShiftTime",
            BackingTypeName: "System.TimeOnly",
            Accessibility: "public",
            ContainingTypes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            Kind: "TimeOnly",
            PastOnly: true,
            FutureOnly: false,
            MaxAge: null,
            DomainShortcut: null,
            CustomExceptionType: null);

        var codeTime = DatePrimitiveGenerator.GenerateDatePrimitive(infoTime);
        codeTime.Should().Contain("TimeOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime)");

        var infoDto = new DatePrimitiveTypeInfo(
            Namespace: "TestNamespace",
            TypeName: "EventTime",
            BackingTypeName: "System.DateTimeOffset",
            Accessibility: "public",
            ContainingTypes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            Kind: "DateTimeOffset",
            PastOnly: true,
            FutureOnly: false,
            MaxAge: null,
            DomainShortcut: null,
            CustomExceptionType: null);

        var codeDto = DatePrimitiveGenerator.GenerateDatePrimitive(infoDto);
        codeDto.Should().Contain("timeProvider.GetUtcNow()");
    }

    [Fact]
    public void DatePrimitiveGenerator_FiscalYear_WithMinYear_EmitsValidation()
    {
        var info = new DatePrimitiveTypeInfo(
            Namespace: "TestNamespace",
            TypeName: "FiscalYearVo",
            BackingTypeName: "System.DateOnly",
            Accessibility: "public",
            ContainingTypes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            Kind: "DateOnly",
            PastOnly: false,
            FutureOnly: false,
            MaxAge: null,
            DomainShortcut: "FiscalYear",
            CustomExceptionType: null,
            MinYear: 1900);

        var code = DatePrimitiveGenerator.GenerateDatePrimitive(info);
        code.Should().Contain("value.Year < 1900");
    }

    #endregion

    #region NumericPrimitiveGenerator Tests

    [Fact]
    public void NumericPrimitiveGenerator_Float_EmitsNaNAndInfinityCheck()
    {
        var info = new NumericPrimitiveTypeInfo(
            Namespace: "TestNamespace",
            TypeName: "Temperature",
            BackingTypeName: "float",
            Accessibility: "public",
            ContainingTypes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            AllowAddition: true,
            AllowSubtraction: true,
            AllowScalarMultiplication: true,
            AllowScalarDivision: true,
            AllowNegation: true,
            RangeMin: null,
            RangeMax: null,
            RangeMinExclusive: false,
            RangeMaxExclusive: false,
            DomainShortcut: null,
            CustomExceptionType: null);

        var code = NumericPrimitiveGenerator.GenerateNumericPrimitive(info);

        code.Should().Contain("if (float.IsNaN(value) || float.IsInfinity(value))");
        code.Should().Contain("return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"RANGE\", $\"Temperature cannot be NaN or Infinity.\");");
    }

    [Theory]
    [InlineData("int", "checked((int)(-value.Value))")]
    [InlineData("double", "(double)(-value.Value)")]
    public void NumericPrimitiveGenerator_UnaryNegation_EmitsCheckedOrUnchecked(string backing, string expected)
    {
        var info = new NumericPrimitiveTypeInfo(
            Namespace: "TestNamespace",
            TypeName: "Amount",
            BackingTypeName: backing,
            Accessibility: "public",
            ContainingTypes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            AllowAddition: true,
            AllowSubtraction: true,
            AllowScalarMultiplication: true,
            AllowScalarDivision: true,
            AllowNegation: true,
            RangeMin: null,
            RangeMax: null,
            RangeMinExclusive: false,
            RangeMaxExclusive: false,
            DomainShortcut: null,
            CustomExceptionType: null);

        var code = NumericPrimitiveGenerator.GenerateNumericPrimitive(info);

        code.Should().Contain(expected);
    }

    #endregion

    #region StringPrimitiveGenerator Tests

    [Fact]
    public void StringPrimitiveGenerator_WithExactLengthLarge_CalculatesMaxBuffer()
    {
        var info = new StringPrimitiveTypeInfo(
            Namespace: "TestNamespace",
            TypeName: "FixedCode",
            Accessibility: "public",
            ContainingTypes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            Trim: true,
            TrimStart: false,
            TrimEnd: false,
            LowerCase: false,
            UpperCase: false,
            NormalizeWhitespace: false,
            MinLength: null,
            MaxLength: null,
            ExactLength: 3000,
            NotEmpty: false,
            RegexPatterns: new EquatableArray<RegexInfo>(ImmutableArray<RegexInfo>.Empty),
            DomainShortcut: null,
            HasCustomValidator: false,
            CustomExceptionType: "global::MyException");

        var code = StringPrimitiveGenerator.GenerateStringPrimitive(info);

        // 3000 * 2 = 6000 > 4096
        code.Should().Contain("6000");
        code.Should().Contain("global::MyException");
    }

    [Fact]
    public void StringPrimitiveGenerator_WithUnprefixedExceptionType_PrependsGlobal()
    {
        var info = new StringPrimitiveTypeInfo(
            Namespace: "TestNamespace",
            TypeName: "FixedCode2",
            Accessibility: "public",
            ContainingTypes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            Trim: true,
            TrimStart: false,
            TrimEnd: false,
            LowerCase: false,
            UpperCase: false,
            NormalizeWhitespace: false,
            MinLength: null,
            MaxLength: null,
            ExactLength: null,
            NotEmpty: false,
            RegexPatterns: new EquatableArray<RegexInfo>(ImmutableArray<RegexInfo>.Empty),
            DomainShortcut: null,
            HasCustomValidator: false,
            CustomExceptionType: "MyCompany.Exceptions.MyStringException");

        var code = StringPrimitiveGenerator.GenerateStringPrimitive(info);

        code.Should().Contain("global::MyCompany.Exceptions.MyStringException");
    }

    #endregion

    #region StrongIdGenerator & ValueObjectGenerator Tests

    [Fact]
    public void StrongIdGenerator_GuidBacked_EmitsSpanFormattable()
    {
        var info = new StrongIdTypeInfo(
            Namespace: "TestNamespace",
            TypeName: "OrderId",
            BackingTypeName: "Guid",
            BackingTypeFullName: "System.Guid",
            Accessibility: "public",
            ContainingTypes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            RejectEmpty: true);

        var code = StrongIdGenerator.GenerateStrongId(info);

        code.Should().Contain("((ISpanFormattable)_value).TryFormat(destination, out charsWritten, format, provider);");
        code.Should().Contain("((IUtf8SpanFormattable)_value).TryFormat(utf8Destination, out bytesWritten, format, provider);");
    }

    [Fact]
    public void ValueObjectGenerator_GlobalNamespaceAndNullable_EmitsProperly()
    {
        var prop = new ValueObjectPropertyInfo("Name", "string?", "name", IsValueType: false, IsNullable: true);
        var info = new ValueObjectTypeInfo(
            Namespace: "",
            TypeName: "GlobalVO",
            Accessibility: "public",
            ContainingTypes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            Properties: new EquatableArray<ValueObjectPropertyInfo>(ImmutableArray.Create(prop)),
            CustomExceptionType: null);

        var code = ValueObjectGenerator.GenerateValueObject(info);

        code.Should().Contain("readonly partial record struct GlobalVO");
        code.Should().Contain("Create(string? name)");
        code.Should().Contain("(Name?.ToString() ?? \"null\")");
    }

    #endregion
}
