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

public class DatePrimitiveGeneratorUnitTests
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
    public void ExtractTypeInfo_NullWhenNotDatePrimitive()
    {
        var source = @"
namespace TestNamespace
{
    public record struct PlainStruct;
}";
        var model = CreateSemanticModel(source);
        var syntax = model.SyntaxTree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().First();
        var info = DatePrimitiveGenerator.ExtractTypeInfo(model, syntax, CancellationToken.None);
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

        var act = () => DatePrimitiveGenerator.ExtractTypeInfo(model, syntax, cts.Token);
        act.Should().Throw<OperationCanceledException>();
    }

    [Fact]
    public void ExtractTypeInfo_AllKindsAndFlags_Extracted()
    {
        var source = @"
using EricksonLopez.DomainPrimitives;

namespace TestNamespace
{
    [DatePrimitive(Kind = 0, PastOnly = true)]
    public readonly partial record struct DateOnlyPastPrim;

    [DatePrimitive(Kind = 1, FutureOnly = true)]
    public readonly partial record struct DateTimeFuturePrim;

    [DatePrimitive(Kind = 2)]
    public readonly partial record struct DateTimeOffsetPrim;

    [DatePrimitive(Kind = 3)]
    public readonly partial record struct TimeOnlyPrim;

    [DatePrimitive]
    public readonly partial record struct PlainDatePrimitive;

    [DatePrimitive(Kind = 99)]
    public readonly partial record struct DefaultKindPrim;
}

namespace EricksonLopez.DomainPrimitives
{
    public class DatePrimitiveAttribute : System.Attribute
    {
        public int Kind { get; set; }
        public bool PastOnly { get; set; }
        public bool FutureOnly { get; set; }
    }
}";
        var model = CreateSemanticModel(source);
        var records = model.SyntaxTree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().ToList();

        var plain = DatePrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "PlainDatePrimitive"), CancellationToken.None);
        plain!.Kind.Should().Be("DateOnly");
        plain.BackingTypeName.Should().Be("System.DateOnly");

        var dOnly = DatePrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "DateOnlyPastPrim"), CancellationToken.None);
        dOnly!.Kind.Should().Be("DateOnly");
        dOnly.BackingTypeName.Should().Be("System.DateOnly");
        dOnly.PastOnly.Should().BeTrue();
        dOnly.FutureOnly.Should().BeFalse();

        var dTime = DatePrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "DateTimeFuturePrim"), CancellationToken.None);
        dTime!.Kind.Should().Be("DateTime");
        dTime.BackingTypeName.Should().Be("System.DateTime");
        dTime.PastOnly.Should().BeFalse();
        dTime.FutureOnly.Should().BeTrue();

        var dOffset = DatePrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "DateTimeOffsetPrim"), CancellationToken.None);
        dOffset!.Kind.Should().Be("DateTimeOffset");
        dOffset.BackingTypeName.Should().Be("System.DateTimeOffset");

        var tOnly = DatePrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "TimeOnlyPrim"), CancellationToken.None);
        tOnly!.Kind.Should().Be("TimeOnly");
        tOnly.BackingTypeName.Should().Be("System.TimeOnly");

        var def = DatePrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "DefaultKindPrim"), CancellationToken.None);
        def!.Kind.Should().Be("DateOnly");
        def.BackingTypeName.Should().Be("System.DateOnly");
    }

    [Fact]
    public void ExtractTypeInfo_AllDomainShortcuts_Extracted()
    {
        var source = @"
using EricksonLopez.DomainPrimitives;

namespace TestNamespace
{
    [BirthDate(MaxAge = 120)] public readonly partial record struct CustomBirthDatePrim;
    [BirthDate(OtherParam = ""ignored"", MaxAge = 95)] public readonly partial record struct NamedParamBirthDatePrim;
    [BirthDate] public readonly partial record struct DefaultBirthDatePrim;
    [ExpirationDate] public readonly partial record struct ExpirationDatePrim;
    [BusinessDate] public readonly partial record struct BusinessDatePrim;
    [FiscalYear] public readonly partial record struct FiscalYearPrim;
    [Month] public readonly partial record struct MonthPrim;
    [Quarter] public readonly partial record struct QuarterPrim;
    [Week] public readonly partial record struct WeekPrim;
    [DateRange] public readonly partial record struct DateRangePrim;
    [TimeRange] public readonly partial record struct TimeRangePrim;
}

namespace EricksonLopez.DomainPrimitives
{
    public class BirthDateAttribute : System.Attribute { public int MaxAge { get; set; } public string OtherParam { get; set; } }
    public class ExpirationDateAttribute : System.Attribute {}
    public class BusinessDateAttribute : System.Attribute {}
    public class FiscalYearAttribute : System.Attribute {}
    public class MonthAttribute : System.Attribute {}
    public class QuarterAttribute : System.Attribute {}
    public class WeekAttribute : System.Attribute {}
    public class DateRangeAttribute : System.Attribute {}
    public class TimeRangeAttribute : System.Attribute {}
}";
        var model = CreateSemanticModel(source);
        var records = model.SyntaxTree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().ToList();

        var customBirth = DatePrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "CustomBirthDatePrim"), CancellationToken.None);
        customBirth!.DomainShortcut.Should().Be("BirthDate");
        customBirth.Kind.Should().Be("DateOnly");
        customBirth.BackingTypeName.Should().Be("System.DateOnly");
        customBirth.PastOnly.Should().BeTrue();
        customBirth.MaxAge.Should().Be(120);

        var namedBirth = DatePrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "NamedParamBirthDatePrim"), CancellationToken.None);
        namedBirth!.DomainShortcut.Should().Be("BirthDate");
        namedBirth.MaxAge.Should().Be(95);

        var defBirth = DatePrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "DefaultBirthDatePrim"), CancellationToken.None);
        defBirth!.DomainShortcut.Should().Be("BirthDate");
        defBirth.MaxAge.Should().Be(150);

        var exp = DatePrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "ExpirationDatePrim"), CancellationToken.None);
        exp!.DomainShortcut.Should().Be("ExpirationDate");
        exp.Kind.Should().Be("DateOnly");
        exp.FutureOnly.Should().BeTrue();

        var biz = DatePrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "BusinessDatePrim"), CancellationToken.None);
        biz!.DomainShortcut.Should().Be("BusinessDate");

        var fy = DatePrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "FiscalYearPrim"), CancellationToken.None);
        fy!.DomainShortcut.Should().Be("FiscalYear");

        var month = DatePrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "MonthPrim"), CancellationToken.None);
        month!.DomainShortcut.Should().Be("Month");

        var quarter = DatePrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "QuarterPrim"), CancellationToken.None);
        quarter!.DomainShortcut.Should().Be("Quarter");

        var week = DatePrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "WeekPrim"), CancellationToken.None);
        week!.DomainShortcut.Should().Be("Week");

        var dRange = DatePrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "DateRangePrim"), CancellationToken.None);
        dRange!.DomainShortcut.Should().Be("DateRange");

        var tRange = DatePrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "TimeRangePrim"), CancellationToken.None);
        tRange!.DomainShortcut.Should().Be("TimeRange");
        tRange.Kind.Should().Be("TimeOnly");
        tRange.BackingTypeName.Should().Be("System.TimeOnly");
    }

    [Fact]
    public void ExtractTypeInfo_NestedAndAccessibilityLevels_Extracted()
    {
        var source = @"
using EricksonLopez.DomainPrimitives;

[assembly: DomainPrimitivesDefaults(ExceptionType = typeof(System.ArgumentException))]

namespace TestNamespace
{
    public class OuterClass
    {
        internal readonly partial record struct NestedPrim;
        [DatePrimitive] public readonly partial record struct PublicNestedPrim;
        [DatePrimitive] internal readonly partial record struct InternalNestedPrim;
        [DatePrimitive] protected readonly partial record struct ProtectedNestedPrim;
        [DatePrimitive] private readonly partial record struct PrivateNestedPrim;
        [DatePrimitive] protected internal readonly partial record struct ProtIntNestedPrim;
        [DatePrimitive] private protected readonly partial record struct PrivProtNestedPrim;
    }
}

namespace EricksonLopez.DomainPrimitives
{
    public class DomainPrimitivesDefaultsAttribute : System.Attribute { public System.Type ExceptionType { get; set; } }
    public class DatePrimitiveAttribute : System.Attribute {}
}";
        var model = CreateSemanticModel(source);
        var records = model.SyntaxTree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().ToList();

        var missingAttr = DatePrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "NestedPrim"), CancellationToken.None);
        missingAttr.Should().BeNull();

        var pub = DatePrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "PublicNestedPrim"), CancellationToken.None);
        pub!.Accessibility.Should().Be("public");
        pub.ContainingTypes.Values.Should().Equal("OuterClass");
        pub.CustomExceptionType.Should().Be("global::System.ArgumentException");

        var intern = DatePrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "InternalNestedPrim"), CancellationToken.None);
        intern!.Accessibility.Should().Be("internal");

        var prot = DatePrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "ProtectedNestedPrim"), CancellationToken.None);
        prot!.Accessibility.Should().Be("protected");

        var priv = DatePrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "PrivateNestedPrim"), CancellationToken.None);
        priv!.Accessibility.Should().Be("private");

        var protInt = DatePrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "ProtIntNestedPrim"), CancellationToken.None);
        protInt!.Accessibility.Should().Be("protected internal");

        var privProt = DatePrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "PrivProtNestedPrim"), CancellationToken.None);
        privProt!.Accessibility.Should().Be("private protected");
    }

    [Fact]
    public void GenerateDatePrimitive_BirthDateWithMaxAge_Generated()
    {
        var info = new DatePrimitiveTypeInfo(
            Namespace: "TestNamespace",
            TypeName: "BirthDatePrim",
            BackingTypeName: "System.DateOnly",
            Accessibility: "public",
            ContainingTypes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            Kind: "DateOnly",
            PastOnly: true,
            FutureOnly: false,
            MaxAge: 100,
            DomainShortcut: "BirthDate",
            CustomExceptionType: "global::System.InvalidOperationException");

        var code = DatePrimitiveGenerator.GenerateDatePrimitive(info).Replace("\r\n", "\n");

        code.Should().Contain("public readonly partial record struct BirthDatePrim :");
        code.Should().Contain("throw new global::System.InvalidOperationException(error.Message);");
        
        var expectedValidation =
            "    private static global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError TryValidate(System.DateOnly value)\n" +
            "    {\n" +
            "        var now = DateOnly.FromDateTime(DateTime.UtcNow);\n" +
            "        if (value >= now)\n" +
            "            return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"TEMPORAL\", \"BirthDatePrim must be in the past.\");\n" +
            "        var age = now.Year - value.Year;\n" +
            "        if (value > now.AddYears(-age)) age--;\n" +
            "        if (age > 100)\n" +
            "            return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"TEMPORAL\", $\"BirthDatePrim exceeds maximum age of 100.\");\n" +
            "        return global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError.None;\n" +
            "    }\n";

        code.Should().Contain(expectedValidation);

        var expectedHelpers =
            "    // ─── BirthDate Helpers ───────────────────────────────────────────\n\n" +
            "    public int Age\n" +
            "    {\n" +
            "        get\n" +
            "        {\n" +
            "            var today = DateOnly.FromDateTime(DateTime.UtcNow);\n" +
            "            var age = today.Year - _value.Year;\n" +
            "            if (_value > today.AddYears(-age)) age--;\n" +
            "            return age;\n" +
            "        }\n" +
            "    }\n\n" +
            "    // ─── JSON Serialization ───────────────────────────────────────────────\n\n" +
            "    private sealed class BirthDatePrimJsonConverter : global::System.Text.Json.Serialization.JsonConverter<BirthDatePrim>\n";

        code.Should().Contain(expectedHelpers);
    }

    [Fact]
    public void GenerateDatePrimitive_ExpirationDate_Generated()
    {
        var info = new DatePrimitiveTypeInfo(
            Namespace: "TestNamespace",
            TypeName: "ExpDatePrim",
            BackingTypeName: "System.DateOnly",
            Accessibility: "internal",
            ContainingTypes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            Kind: "DateOnly",
            PastOnly: false,
            FutureOnly: true,
            MaxAge: null,
            DomainShortcut: "ExpirationDate",
            CustomExceptionType: null);

        var code = DatePrimitiveGenerator.GenerateDatePrimitive(info).Replace("\r\n", "\n");

        code.Should().Contain("internal readonly partial record struct ExpDatePrim :");
        code.Should().Contain("throw new DomainPrimitiveValidationException(error);");
        
        var expectedValidation =
            "    private static global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError TryValidate(System.DateOnly value)\n" +
            "    {\n" +
            "        var now = DateOnly.FromDateTime(DateTime.UtcNow);\n" +
            "        if (value <= now)\n" +
            "            return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"TEMPORAL\", \"ExpDatePrim must be in the future.\");\n" +
            "        return global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError.None;\n" +
            "    }\n";

        code.Should().Contain(expectedValidation);

        var expectedHelpers =
            "    // ─── Expiration Helpers ──────────────────────────────────────────\n\n" +
            "    public bool IsExpired()\n" +
            "    {\n" +
            "        return DateOnly.FromDateTime(DateTime.UtcNow) > _value;\n" +
            "    }\n\n" +
            "    public int DaysUntilExpiration()\n" +
            "    {\n" +
            "        var today = DateOnly.FromDateTime(DateTime.UtcNow);\n" +
            "        return _value.DayNumber - today.DayNumber;\n" +
            "    }\n\n" +
            "    // ─── JSON Serialization ───────────────────────────────────────────────\n\n" +
            "    private sealed class ExpDatePrimJsonConverter : global::System.Text.Json.Serialization.JsonConverter<ExpDatePrim>\n";

        code.Should().Contain(expectedHelpers);
    }

    [Fact]
    public void GenerateDatePrimitive_BirthDateWithoutMaxAge_AndNonBirthDateWithMaxAge_DoesNotEmitAgeValidation()
    {
        var birthWithoutMaxAge = new DatePrimitiveTypeInfo(
            Namespace: "TestNamespace",
            TypeName: "BirthNoMaxAge",
            BackingTypeName: "System.DateOnly",
            Accessibility: "public",
            ContainingTypes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            Kind: "DateOnly",
            PastOnly: true,
            FutureOnly: false,
            MaxAge: null,
            DomainShortcut: "BirthDate",
            CustomExceptionType: null);

        var code1 = DatePrimitiveGenerator.GenerateDatePrimitive(birthWithoutMaxAge);
        code1.Should().NotContain("exceeds maximum age");

        var nonBirthWithMaxAge = new DatePrimitiveTypeInfo(
            Namespace: "TestNamespace",
            TypeName: "NonBirthMaxAge",
            BackingTypeName: "System.DateOnly",
            Accessibility: "public",
            ContainingTypes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            Kind: "DateOnly",
            PastOnly: true,
            FutureOnly: false,
            MaxAge: 100,
            DomainShortcut: null,
            CustomExceptionType: null);

        var code2 = DatePrimitiveGenerator.GenerateDatePrimitive(nonBirthWithMaxAge);
        code2.Should().NotContain("exceeds maximum age");
    }

    [Fact]
    public void GenerateDatePrimitive_DateTimeAndOffsetAndTimeOnly_NowExpr_Generated()
    {
        var dtInfo = new DatePrimitiveTypeInfo("TestNamespace", "DtPrim", "System.DateTime", "public", new EquatableArray<string>(ImmutableArray<string>.Empty), "DateTime", true, false, null, null);
        var dtCode = DatePrimitiveGenerator.GenerateDatePrimitive(dtInfo);
        dtCode.Should().Contain("var now = DateTime.UtcNow;");

        var dtoInfo = new DatePrimitiveTypeInfo("TestNamespace", "DtoPrim", "System.DateTimeOffset", "public", new EquatableArray<string>(ImmutableArray<string>.Empty), "DateTimeOffset", true, false, null, null);
        var dtoCode = DatePrimitiveGenerator.GenerateDatePrimitive(dtoInfo);
        dtoCode.Should().Contain("var now = DateTimeOffset.UtcNow;");

        var toInfo = new DatePrimitiveTypeInfo("TestNamespace", "ToPrim", "System.TimeOnly", "public", new EquatableArray<string>(ImmutableArray<string>.Empty), "TimeOnly", true, false, null, null);
        var toCode = DatePrimitiveGenerator.GenerateDatePrimitive(toInfo);
        toCode.Should().Contain("var now = TimeOnly.FromDateTime(DateTime.UtcNow);");

        var unkInfo = new DatePrimitiveTypeInfo("TestNamespace", "UnkPrim", "System.DateTime", "public", new EquatableArray<string>(ImmutableArray<string>.Empty), "CustomKind", true, false, null, null);
        var unkCode = DatePrimitiveGenerator.GenerateDatePrimitive(unkInfo);
        unkCode.Should().Contain("var now = DateTime.UtcNow;");
    }

    [Fact]
    public void GenerateDatePrimitive_NoValidation_EmptyTryValidate()
    {
        var info = new DatePrimitiveTypeInfo(
            Namespace: "TestNamespace",
            TypeName: "PlainDatePrim",
            BackingTypeName: "System.DateOnly",
            Accessibility: "public",
            ContainingTypes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            Kind: "DateOnly",
            PastOnly: false,
            FutureOnly: false,
            MaxAge: null,
            DomainShortcut: null,
            CustomExceptionType: null);

        var code = DatePrimitiveGenerator.GenerateDatePrimitive(info).Replace("\r\n", "\n");

        var expectedValidation =
            "    private static global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError TryValidate(System.DateOnly value)\n" +
            "    {\n" +
            "        return global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError.None;\n" +
            "    }\n";

        code.Should().Contain(expectedValidation);
    }
}


