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

public class NumericPrimitiveGeneratorUnitTests
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
    public void ExtractTypeInfo_NullWhenNotNumericPrimitive()
    {
        var source = @"
namespace TestNamespace
{
    public record struct PlainStruct;
}";
        var model = CreateSemanticModel(source);
        var syntax = model.SyntaxTree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().First();
        var info = NumericPrimitiveGenerator.ExtractTypeInfo(model, syntax, CancellationToken.None);
        info.Should().BeNull();
    }

    [Fact]
    public void ExtractTypeInfo_AllSpecialTypes_AndOperationsFlags_Extracted()
    {
        var source = @"
using EricksonLopez.DomainPrimitives;

namespace TestNamespace
{
    [NumericPrimitive<int>(Operations = 31)]
    public readonly partial record struct IntPrim;

    [NumericPrimitive<long>(Operations = 3)]
    public readonly partial record struct LongPrim;

    [NumericPrimitive<double>(Operations = 4)]
    public readonly partial record struct DoublePrim;

    [NumericPrimitive<decimal>(Operations = 8)]
    public readonly partial record struct DecimalPrim;

    [NumericPrimitive<float>(Operations = 16)]
    public readonly partial record struct FloatPrim;

    [NumericPrimitive<short>]
    public readonly partial record struct ShortPrim;

    [NumericPrimitive<byte>]
    public readonly partial record struct BytePrim;
}

namespace EricksonLopez.DomainPrimitives
{
    public class NumericPrimitiveAttribute<T> : System.Attribute { public int Operations { get; set; } }
}";
        var model = CreateSemanticModel(source);
        var records = model.SyntaxTree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().ToList();

        var intPrim = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "IntPrim"), CancellationToken.None);
        intPrim!.BackingTypeName.Should().Be("int");
        intPrim.AllowAddition.Should().BeTrue();
        intPrim.AllowSubtraction.Should().BeTrue();
        intPrim.AllowScalarMultiplication.Should().BeTrue();
        intPrim.AllowScalarDivision.Should().BeTrue();
        intPrim.AllowNegation.Should().BeTrue();

        var longPrim = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "LongPrim"), CancellationToken.None);
        longPrim!.BackingTypeName.Should().Be("long");
        longPrim.AllowAddition.Should().BeTrue();
        longPrim.AllowSubtraction.Should().BeTrue();
        longPrim.AllowScalarMultiplication.Should().BeFalse();
        longPrim.AllowScalarDivision.Should().BeFalse();
        longPrim.AllowNegation.Should().BeFalse();

        var doublePrim = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "DoublePrim"), CancellationToken.None);
        doublePrim!.BackingTypeName.Should().Be("double");
        doublePrim.AllowScalarMultiplication.Should().BeTrue();

        var decPrim = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "DecimalPrim"), CancellationToken.None);
        decPrim!.BackingTypeName.Should().Be("decimal");
        decPrim.AllowScalarDivision.Should().BeTrue();

        var floatPrim = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "FloatPrim"), CancellationToken.None);
        floatPrim!.BackingTypeName.Should().Be("float");
        floatPrim.AllowNegation.Should().BeTrue();

        var shortPrim = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "ShortPrim"), CancellationToken.None);
        shortPrim!.BackingTypeName.Should().Be("short");

        var bytePrim = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "BytePrim"), CancellationToken.None);
        bytePrim!.BackingTypeName.Should().Be("byte");
    }

    [Fact]
    public void ExtractTypeInfo_RangeAttributes_Extracted()
    {
        var source = @"
using System;
using EricksonLopez.DomainPrimitives;

namespace TestNamespace
{
    [NumericPrimitive<int>]
    [Range(10, 100, MinExclusive = true, MaxExclusive = true)]
    public readonly partial record struct NumericRangePrim;

    [NumericPrimitive<decimal>]
    [PrimitiveRange(""10.5"", ""99.9"", StringMin = ""12.0"", StringMax = ""90.0"")]
    public readonly partial record struct StringRangePrim;
}

namespace EricksonLopez.DomainPrimitives
{
    public class NumericPrimitiveAttribute<T> : System.Attribute {}
    public class RangeAttribute : System.Attribute
    {
        public RangeAttribute(object min, object max) {}
        public bool MinExclusive { get; set; }
        public bool MaxExclusive { get; set; }
    }
    public class PrimitiveRangeAttribute : System.Attribute
    {
        public PrimitiveRangeAttribute(string min, string max) {}
        public string StringMin { get; set; }
        public string StringMax { get; set; }
    }
}";
        var model = CreateSemanticModel(source);
        var records = model.SyntaxTree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().ToList();

        var numRange = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "NumericRangePrim"), CancellationToken.None);
        numRange!.RangeMin.Should().Be(10);
        numRange.RangeMax.Should().Be(100);
        numRange.RangeMinExclusive.Should().BeTrue();
        numRange.RangeMaxExclusive.Should().BeTrue();

        var strRange = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "StringRangePrim"), CancellationToken.None);
        strRange!.RangeStringMin.Should().Be("12.0");
        strRange.RangeStringMax.Should().Be("90.0");
    }

    [Fact]
    public void ExtractTypeInfo_AllDomainShortcuts_Extracted()
    {
        var source = @"
using EricksonLopez.DomainPrimitives;

namespace TestNamespace
{
    [Money(null, 15.0, 5000.0)] public readonly partial record struct MoneyCtorPrim;
    [Money(Min = 20.0, Max = 10000.0)] public readonly partial record struct MoneyNamedPrim;
    [Percentage(Min = 5.0, Max = 95.0)] public readonly partial record struct PercentagePrim;
    [Latitude] public readonly partial record struct LatPrim;
    [Longitude] public readonly partial record struct LongPrim;
    [Age] public readonly partial record struct AgePrim;
    [Rating(Min = 1.0, Max = 10.0, Scale = 2)] public readonly partial record struct RatingPrim;
    [Weight] public readonly partial record struct WeightPrim;
    [Height] public readonly partial record struct HeightPrim;
    [Distance] public readonly partial record struct DistancePrim;
    [Temperature] public readonly partial record struct TempPrim;
    [Score] public readonly partial record struct ScorePrim;
    [Quantity] public readonly partial record struct QuantityPrim;
    [Price] public readonly partial record struct PricePrim;
    [TaxRate] public readonly partial record struct TaxRatePrim;
    [Discount] public readonly partial record struct DiscountPrim;
}

namespace EricksonLopez.DomainPrimitives
{
    public class MoneyAttribute : System.Attribute
    {
        public MoneyAttribute() {}
        public MoneyAttribute(string cur = null, double min = 0, double max = double.MaxValue) {}
        public double Min { get; set; }
        public double Max { get; set; }
    }
    public class PercentageAttribute : System.Attribute { public double Min { get; set; } public double Max { get; set; } }
    public class LatitudeAttribute : System.Attribute {}
    public class LongitudeAttribute : System.Attribute {}
    public class AgeAttribute : System.Attribute {}
    public class RatingAttribute : System.Attribute { public double Min { get; set; } public double Max { get; set; } public int Scale { get; set; } }
    public class WeightAttribute : System.Attribute {}
    public class HeightAttribute : System.Attribute {}
    public class DistanceAttribute : System.Attribute {}
    public class TemperatureAttribute : System.Attribute {}
    public class ScoreAttribute : System.Attribute {}
    public class QuantityAttribute : System.Attribute {}
    public class PriceAttribute : System.Attribute {}
    public class TaxRateAttribute : System.Attribute {}
    public class DiscountAttribute : System.Attribute {}
}";
        var model = CreateSemanticModel(source);
        var records = model.SyntaxTree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().ToList();

        var moneyCtor = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "MoneyCtorPrim"), CancellationToken.None);
        moneyCtor!.DomainShortcut.Should().Be("Money");
        moneyCtor.BackingTypeName.Should().Be("decimal");
        moneyCtor.RangeMin.Should().Be(15.0);
        moneyCtor.RangeMax.Should().Be(5000.0);
        moneyCtor.AllowAddition.Should().BeTrue();

        var moneyNamed = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "MoneyNamedPrim"), CancellationToken.None);
        moneyNamed!.RangeMin.Should().Be(20.0);
        moneyNamed.RangeMax.Should().Be(10000.0);

        var pct = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "PercentagePrim"), CancellationToken.None);
        pct!.DomainShortcut.Should().Be("Percentage");
        pct.BackingTypeName.Should().Be("decimal");
        pct.RangeMin.Should().Be(5.0);
        pct.RangeMax.Should().Be(95.0);

        var lat = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "LatPrim"), CancellationToken.None);
        lat!.DomainShortcut.Should().Be("Latitude");
        lat.BackingTypeName.Should().Be("double");
        lat.RangeMin.Should().Be(-90);
        lat.RangeMax.Should().Be(90);

        var lon = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "LongPrim"), CancellationToken.None);
        lon!.DomainShortcut.Should().Be("Longitude");
        lon.BackingTypeName.Should().Be("double");
        lon.RangeMin.Should().Be(-180);
        lon.RangeMax.Should().Be(180);

        var age = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "AgePrim"), CancellationToken.None);
        age!.DomainShortcut.Should().Be("Age");
        age.BackingTypeName.Should().Be("int");
        age.RangeMin.Should().Be(0);
        age.RangeMax.Should().Be(150);

        var rating = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "RatingPrim"), CancellationToken.None);
        rating!.DomainShortcut.Should().Be("Rating");
        rating.BackingTypeName.Should().Be("decimal");
        rating.RangeMin.Should().Be(1.0);
        rating.RangeMax.Should().Be(10.0);
        rating.Scale.Should().Be(2);

        var weight = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "WeightPrim"), CancellationToken.None);
        weight!.BackingTypeName.Should().Be("double");

        var price = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "PricePrim"), CancellationToken.None);
        price!.DomainShortcut.Should().Be("Price");
        price.BackingTypeName.Should().Be("decimal");

        var height = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "HeightPrim"), CancellationToken.None);
        height!.DomainShortcut.Should().Be("Height");
        height.BackingTypeName.Should().Be("double");

        var dist = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "DistancePrim"), CancellationToken.None);
        dist!.DomainShortcut.Should().Be("Distance");
        dist.BackingTypeName.Should().Be("double");

        var temp = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "TempPrim"), CancellationToken.None);
        temp!.DomainShortcut.Should().Be("Temperature");
        temp.BackingTypeName.Should().Be("double");

        var score = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "ScorePrim"), CancellationToken.None);
        score!.DomainShortcut.Should().Be("Score");
        score.BackingTypeName.Should().Be("double");

        var qty = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "QuantityPrim"), CancellationToken.None);
        qty!.DomainShortcut.Should().Be("Quantity");
        qty.BackingTypeName.Should().Be("double");

        var tax = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "TaxRatePrim"), CancellationToken.None);
        tax!.DomainShortcut.Should().Be("TaxRate");
        tax.BackingTypeName.Should().Be("decimal");

        var disc = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "DiscountPrim"), CancellationToken.None);
        disc!.DomainShortcut.Should().Be("Discount");
        disc.BackingTypeName.Should().Be("decimal");
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

        var act = () => NumericPrimitiveGenerator.ExtractTypeInfo(model, syntax, cts.Token);
        act.Should().Throw<OperationCanceledException>();
    }

    [Fact]
    public void ExtractTypeInfo_DomainShortcutsWithOverriddenTypesAndRanges_Preserved()
    {
        var source = @"
using EricksonLopez.DomainPrimitives;

namespace TestNamespace
{
    [NumericPrimitive<double>] [Money] [Range(100.0, 200.0)] public readonly partial record struct CustomMoneyPrim;
    [NumericPrimitive<int>] [Percentage] [Range(10.0, 50.0)] public readonly partial record struct CustomPctPrim;
    [NumericPrimitive<float>] [Latitude] [Range(-10.0, 10.0)] public readonly partial record struct CustomLatPrim;
    [NumericPrimitive<float>] [Longitude] [Range(-20.0, 20.0)] public readonly partial record struct CustomLonPrim;
    [NumericPrimitive<byte>] [Age] [Range(18.0, 65.0)] public readonly partial record struct CustomAgePrim;
    [NumericPrimitive<double>] [Rating] [Range(1.0, 10.0)] public readonly partial record struct CustomRatingPrim;
    [NumericPrimitive<float>] [Weight] public readonly partial record struct CustomWeightPrim;
    [NumericPrimitive<double>] [Price] public readonly partial record struct CustomPricePrim;
}

namespace EricksonLopez.DomainPrimitives
{
    public class NumericPrimitiveAttribute<T> : System.Attribute {}
    public class MoneyAttribute : System.Attribute {}
    public class PercentageAttribute : System.Attribute {}
    public class LatitudeAttribute : System.Attribute {}
    public class LongitudeAttribute : System.Attribute {}
    public class AgeAttribute : System.Attribute {}
    public class RatingAttribute : System.Attribute {}
    public class WeightAttribute : System.Attribute {}
    public class PriceAttribute : System.Attribute {}
    public class RangeAttribute : System.Attribute { public RangeAttribute(object min, object max) {} }
}";
        var model = CreateSemanticModel(source);
        var records = model.SyntaxTree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().ToList();

        var money = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "CustomMoneyPrim"), CancellationToken.None);
        money!.BackingTypeName.Should().Be("double");
        money.RangeMin.Should().Be(100.0);
        money.RangeMax.Should().Be(200.0);

        var pct = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "CustomPctPrim"), CancellationToken.None);
        pct!.BackingTypeName.Should().Be("int");
        pct.RangeMin.Should().Be(10.0);
        pct.RangeMax.Should().Be(50.0);

        var lat = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "CustomLatPrim"), CancellationToken.None);
        lat!.BackingTypeName.Should().Be("float");
        lat.RangeMin.Should().Be(-10.0);
        lat.RangeMax.Should().Be(10.0);

        var lon = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "CustomLonPrim"), CancellationToken.None);
        lon!.BackingTypeName.Should().Be("float");
        lon.RangeMin.Should().Be(-20.0);
        lon.RangeMax.Should().Be(20.0);

        var age = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "CustomAgePrim"), CancellationToken.None);
        age!.BackingTypeName.Should().Be("byte");
        age.RangeMin.Should().Be(18.0);
        age.RangeMax.Should().Be(65.0);

        var rating = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "CustomRatingPrim"), CancellationToken.None);
        rating!.BackingTypeName.Should().Be("double");
        rating.RangeMin.Should().Be(1.0);
        rating.RangeMax.Should().Be(10.0);

        var weight = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "CustomWeightPrim"), CancellationToken.None);
        weight!.BackingTypeName.Should().Be("float");

        var price = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "CustomPricePrim"), CancellationToken.None);
        price!.BackingTypeName.Should().Be("double");
    }

    [Fact]
    public void ExtractTypeInfo_Operations_ZeroFlags()
    {
        var source = @"
using EricksonLopez.DomainPrimitives;

namespace TestNamespace
{
    [NumericPrimitive<int>(Operations = 0)]
    public readonly partial record struct ZeroOpsPrim;
}

namespace EricksonLopez.DomainPrimitives
{
    public class NumericPrimitiveAttribute<T> : System.Attribute { public int Operations { get; set; } }
}";
        var model = CreateSemanticModel(source);
        var syntax = model.SyntaxTree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().First();
        var info = NumericPrimitiveGenerator.ExtractTypeInfo(model, syntax, CancellationToken.None);

        info.Should().NotBeNull();
        info!.AllowAddition.Should().BeFalse();
        info.AllowSubtraction.Should().BeFalse();
        info.AllowScalarMultiplication.Should().BeFalse();
        info.AllowScalarDivision.Should().BeFalse();
        info.AllowNegation.Should().BeFalse();
    }

    [Fact]
    public void ExtractTypeInfo_MoneyConstructor_TwoArgs_Extracted()
    {
        var source = @"
using EricksonLopez.DomainPrimitives;

namespace TestNamespace
{
    [Money(""USD"", 50.0)]
    public readonly partial record struct MoneyTwoArgsPrim;
}

namespace EricksonLopez.DomainPrimitives
{
    public class MoneyAttribute : System.Attribute
    {
        public MoneyAttribute(string cur, double min) {}
    }
}";
        var model = CreateSemanticModel(source);
        var syntax = model.SyntaxTree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().First();
        var info = NumericPrimitiveGenerator.ExtractTypeInfo(model, syntax, CancellationToken.None);

        info.Should().NotBeNull();
        info!.RangeMin.Should().Be(50.0);
        info.RangeMax.Should().Be(double.MaxValue);
    }

    [Fact]
    public void ExtractTypeInfo_PrimitiveRangeWithoutNamedArguments_StringMinMaxExtracted()
    {
        var source = @"
using EricksonLopez.DomainPrimitives;

namespace TestNamespace
{
    [NumericPrimitive<decimal>]
    [PrimitiveRange(""10"", ""100"", OtherProp = true)]
    public readonly partial record struct StringMinMaxPrim;
}

namespace EricksonLopez.DomainPrimitives
{
    public class NumericPrimitiveAttribute<T> : System.Attribute {}
    public class PrimitiveRangeAttribute : System.Attribute
    {
        public PrimitiveRangeAttribute(string min, string max) {}
        public bool OtherProp { get; set; }
    }
}";
        var model = CreateSemanticModel(source);
        var syntax = model.SyntaxTree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().First();
        var info = NumericPrimitiveGenerator.ExtractTypeInfo(model, syntax, CancellationToken.None);

        info.Should().NotBeNull();
        info!.RangeStringMin.Should().Be("10");
        info.RangeStringMax.Should().Be("100");
        info.RangeMin.Should().BeNull();
        info.RangeMax.Should().BeNull();
        info.RangeMinExclusive.Should().BeFalse();
        info.RangeMaxExclusive.Should().BeFalse();
    }

    [Fact]
    public void ExtractTypeInfo_RangeNamedKeysFiltering()
    {
        var source = @"
using EricksonLopez.DomainPrimitives;

namespace TestNamespace
{
    [NumericPrimitive<int>]
    [Range(0, 10, OtherProp = ""ignored"")]
    public readonly partial record struct RangeFilteringPrim;
}

namespace EricksonLopez.DomainPrimitives
{
    public class NumericPrimitiveAttribute<T> : System.Attribute {}
    public class RangeAttribute : System.Attribute
    {
        public RangeAttribute(object min, object max) {}
        public string OtherProp { get; set; }
    }
}";
        var model = CreateSemanticModel(source);
        var syntax = model.SyntaxTree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().First();
        var info = NumericPrimitiveGenerator.ExtractTypeInfo(model, syntax, CancellationToken.None);

        info.Should().NotBeNull();
        info!.RangeMinExclusive.Should().BeFalse();
        info.RangeMaxExclusive.Should().BeFalse();
    }

    [Fact]
    public void GenerateNumericPrimitive_DoubleWithExclusiveNumericRanges_Generated()
    {
        var info = new NumericPrimitiveTypeInfo(
            Namespace: "TestNamespace",
            TypeName: "ExclusiveDoublePrim",
            BackingTypeName: "double",
            Accessibility: "public",
            ContainingTypes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            AllowAddition: false,
            AllowSubtraction: false,
            AllowScalarMultiplication: false,
            AllowScalarDivision: false,
            AllowNegation: true,
            RangeMin: 10.0,
            RangeMax: 100.0,
            RangeMinExclusive: true,
            RangeMaxExclusive: true,
            DomainShortcut: null,
            Scale: 1,
            RangeStringMin: null,
            RangeStringMax: null,
            CustomExceptionType: null);

        var code = NumericPrimitiveGenerator.GenerateNumericPrimitive(info).Replace("\r\n", "\n");

        code.Should().Contain("System.Numerics.IUnaryNegationOperators<ExclusiveDoublePrim, ExclusiveDoublePrim>");
        
        var expectedValidation =
            "    private static global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError TryValidate(double value)\n" +
            "    {\n" +
            "        if (value <= (double)10)\n" +
            "            return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"RANGE\", $\"ExclusiveDoublePrim must be greater than 10. Got {value}.\");\n" +
            "        if (value >= (double)100)\n" +
            "            return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"RANGE\", $\"ExclusiveDoublePrim must be less than 100. Got {value}.\");\n" +
            "        if (Math.Round((double)value, 1) != (double)value)\n" +
            "            return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"FORMAT\", $\"ExclusiveDoublePrim must have at most 1 decimal place(s).\");\n" +
            "        return global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError.None;\n" +
            "    }\n";

        code.Should().Contain(expectedValidation);
        code.Should().Contain("    public static ExclusiveDoublePrim operator -(ExclusiveDoublePrim value) => Create((double)(-value.Value));\n\n    // ─── Comparison ──────────────────────────────────────────────────\n");
    }

    [Fact]
    public void GenerateNumericPrimitive_DoubleWithInclusiveStringRanges_Generated()
    {
        var info = new NumericPrimitiveTypeInfo(
            Namespace: "TestNamespace",
            TypeName: "InclusiveStringDoublePrim",
            BackingTypeName: "double",
            Accessibility: "public",
            ContainingTypes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            AllowAddition: false,
            AllowSubtraction: false,
            AllowScalarMultiplication: false,
            AllowScalarDivision: false,
            AllowNegation: false,
            RangeMin: null,
            RangeMax: null,
            RangeMinExclusive: false,
            RangeMaxExclusive: false,
            DomainShortcut: null,
            Scale: null,
            RangeStringMin: "5.5",
            RangeStringMax: "95.5",
            CustomExceptionType: null);

        var code = NumericPrimitiveGenerator.GenerateNumericPrimitive(info).Replace("\r\n", "\n");

        var expectedValidation =
            "    private static global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError TryValidate(double value)\n" +
            "    {\n" +
            "        if (value < 5.5)\n" +
            "            return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"RANGE\", $\"InclusiveStringDoublePrim must be at least 5.5. Got {value}.\");\n" +
            "        if (value > 95.5)\n" +
            "            return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"RANGE\", $\"InclusiveStringDoublePrim must be at most 95.5. Got {value}.\");\n" +
            "        return global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError.None;\n" +
            "    }\n";

        code.Should().Contain(expectedValidation);
    }

    [Fact]
    public void ExtractTypeInfo_NestedAndAccessibilityLevels_Extracted()
    {
        var source = @"
using EricksonLopez.DomainPrimitives;

namespace TestNamespace
{
    public class OuterClass
    {
        internal readonly partial record struct NestedPrim;
        [NumericPrimitive<int>] public readonly partial record struct PublicNestedPrim;
        [NumericPrimitive<int>] internal readonly partial record struct InternalNestedPrim;
        [NumericPrimitive<int>] protected readonly partial record struct ProtectedNestedPrim;
        [NumericPrimitive<int>] private readonly partial record struct PrivateNestedPrim;
        [NumericPrimitive<int>] protected internal readonly partial record struct ProtIntNestedPrim;
        [NumericPrimitive<int>] private protected readonly partial record struct PrivProtNestedPrim;
    }
}

namespace EricksonLopez.DomainPrimitives
{
    public class NumericPrimitiveAttribute<T> : System.Attribute {}
}";
        var model = CreateSemanticModel(source);
        var records = model.SyntaxTree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().ToList();

        var missingAttr = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "NestedPrim"), CancellationToken.None);
        missingAttr.Should().BeNull();

        var pub = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "PublicNestedPrim"), CancellationToken.None);
        pub!.Accessibility.Should().Be("public");
        pub.ContainingTypes.Values.Should().Equal("OuterClass");

        var intern = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "InternalNestedPrim"), CancellationToken.None);
        intern!.Accessibility.Should().Be("internal");

        var prot = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "ProtectedNestedPrim"), CancellationToken.None);
        prot!.Accessibility.Should().Be("protected");

        var priv = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "PrivateNestedPrim"), CancellationToken.None);
        priv!.Accessibility.Should().Be("private");

        var protInt = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "ProtIntNestedPrim"), CancellationToken.None);
        protInt!.Accessibility.Should().Be("protected internal");

        var privProt = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "PrivProtNestedPrim"), CancellationToken.None);
        privProt!.Accessibility.Should().Be("private protected");
    }

    [Fact]
    public void ExtractTypeInfo_AssemblyDefaults_Extracted()
    {
        var source = @"
using EricksonLopez.DomainPrimitives;

[assembly: DomainPrimitivesDefaults(ExceptionType = typeof(System.InvalidOperationException))]

namespace TestNamespace
{
    [NumericPrimitive<int>]
    public readonly partial record struct CustomExPrim;
}

namespace EricksonLopez.DomainPrimitives
{
    public class DomainPrimitivesDefaultsAttribute : System.Attribute { public System.Type ExceptionType { get; set; } }
    public class NumericPrimitiveAttribute<T> : System.Attribute {}
}";
        var model = CreateSemanticModel(source);
        var syntax = model.SyntaxTree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().First();
        var info = NumericPrimitiveGenerator.ExtractTypeInfo(model, syntax, CancellationToken.None);

        info.Should().NotBeNull();
        info!.CustomExceptionType.Should().Be("global::System.InvalidOperationException");
    }

    [Fact]
    public void ExtractTypeInfo_RangeAttribute_InvalidConversionFallback()
    {
        var source = @"
using EricksonLopez.DomainPrimitives;

namespace TestNamespace
{
    [NumericPrimitive<int>]
    [Range(null, null)]
    public readonly partial record struct NullRangePrim;

    [NumericPrimitive<int>]
    [Range(typeof(int), typeof(double))]
    public readonly partial record struct TypeRangePrim;
}

namespace EricksonLopez.DomainPrimitives
{
    public class NumericPrimitiveAttribute<T> : System.Attribute {}
    public class RangeAttribute : System.Attribute { public RangeAttribute(object min, object max) {} }
}";
        var model = CreateSemanticModel(source);
        var records = model.SyntaxTree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().ToList();
        
        var nullInfo = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "NullRangePrim"), CancellationToken.None);
        nullInfo.Should().NotBeNull();
        nullInfo!.RangeMin.Should().BeNull();
        nullInfo.RangeMax.Should().BeNull();

        var typeInfo = NumericPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "TypeRangePrim"), CancellationToken.None);
        typeInfo.Should().NotBeNull();
        typeInfo!.RangeMin.Should().BeNull();
        typeInfo.RangeMax.Should().BeNull();
    }

    [Fact]
    public void GenerateNumericPrimitive_DecimalWithScaleAndStringRange_Generated()
    {
        var info = new NumericPrimitiveTypeInfo(
            Namespace: "TestNamespace",
            TypeName: "DecimalPrice",
            BackingTypeName: "decimal",
            Accessibility: "public",
            ContainingTypes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            AllowAddition: true,
            AllowSubtraction: true,
            AllowScalarMultiplication: true,
            AllowScalarDivision: true,
            AllowNegation: true,
            RangeMin: null,
            RangeMax: null,
            RangeMinExclusive: true,
            RangeMaxExclusive: true,
            DomainShortcut: "Price",
            Scale: 2,
            RangeStringMin: "0.01",
            RangeStringMax: "99999.99",
            CustomExceptionType: "global::System.ArgumentOutOfRangeException");

        var code = NumericPrimitiveGenerator.GenerateNumericPrimitive(info);

        code.Should().Contain("public readonly partial record struct DecimalPrice :");
        code.Should().Contain("throw new global::System.ArgumentOutOfRangeException(error.Message);");
        code.Should().Contain("if (value <= 0.01m)");
        code.Should().Contain("        return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"RANGE\", $\"DecimalPrice must be greater than 0.01. Got {value}.\");");
        code.Should().Contain("if (value >= 99999.99m)");
        code.Should().Contain("        return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"RANGE\", $\"DecimalPrice must be less than 99999.99. Got {value}.\");");
        code.Should().Contain("if (Math.Round((double)value, 2) != (double)value)");
        code.Should().Contain("        return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"FORMAT\", $\"DecimalPrice must have at most 2 decimal place(s).\");");
        code.Should().Contain("public static DecimalPrice operator +(DecimalPrice left, DecimalPrice right)");
        code.Should().Contain("public static DecimalPrice operator -(DecimalPrice left, DecimalPrice right)");
        code.Should().Contain("public static DecimalPrice operator *(DecimalPrice left, decimal right)");
        code.Should().Contain("public static DecimalPrice operator /(DecimalPrice left, decimal right)");
        code.Should().Contain("public static DecimalPrice operator -(DecimalPrice value)");
    }

    [Fact]
    public void GenerateNumericPrimitive_NumericMinMax_Generated()
    {
        var info = new NumericPrimitiveTypeInfo(
            Namespace: "TestNamespace",
            TypeName: "IntAge",
            BackingTypeName: "int",
            Accessibility: "internal",
            ContainingTypes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            AllowAddition: false,
            AllowSubtraction: false,
            AllowScalarMultiplication: false,
            AllowScalarDivision: false,
            AllowNegation: false,
            RangeMin: 0,
            RangeMax: 150,
            RangeMinExclusive: false,
            RangeMaxExclusive: false,
            DomainShortcut: "Age",
            Scale: null,
            RangeStringMin: null,
            RangeStringMax: null,
            CustomExceptionType: null);

        var code = NumericPrimitiveGenerator.GenerateNumericPrimitive(info);

        code.Should().Contain("internal readonly partial record struct IntAge :");
        code.Should().Contain("throw new DomainPrimitiveValidationException(error);");
        code.Should().Contain("if (value < (int)0)");
        code.Should().Contain("        return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"RANGE\", $\"IntAge must be at least 0. Got {value}.\");");
        code.Should().Contain("if (value > (int)150)");
        code.Should().Contain("        return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"RANGE\", $\"IntAge must be at most 150. Got {value}.\");");
        code.Should().NotContain("public static IntAge operator +(");
    }

    [Fact]
    public void GenerateNumericPrimitive_SingleBoundMinAndMax_AndNoValidation_Generated()
    {
        var minOnly = new NumericPrimitiveTypeInfo(
            Namespace: "TestNamespace",
            TypeName: "MinOnlyPrim",
            BackingTypeName: "double",
            Accessibility: "public",
            ContainingTypes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            AllowAddition: false,
            AllowSubtraction: false,
            AllowScalarMultiplication: false,
            AllowScalarDivision: false,
            AllowNegation: false,
            RangeMin: 10.0,
            RangeMax: null,
            RangeMinExclusive: false,
            RangeMaxExclusive: false,
            DomainShortcut: null,
            Scale: null,
            RangeStringMin: null,
            RangeStringMax: null,
            CustomExceptionType: null);

        var minCode = NumericPrimitiveGenerator.GenerateNumericPrimitive(minOnly);
        minCode.Should().Contain("if (value < (double)10)");
        minCode.Should().NotContain("must be at most");

        var maxOnly = new NumericPrimitiveTypeInfo(
            Namespace: "TestNamespace",
            TypeName: "MaxOnlyPrim",
            BackingTypeName: "double",
            Accessibility: "public",
            ContainingTypes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            AllowAddition: false,
            AllowSubtraction: false,
            AllowScalarMultiplication: false,
            AllowScalarDivision: false,
            AllowNegation: false,
            RangeMin: null,
            RangeMax: 100.0,
            RangeMinExclusive: false,
            RangeMaxExclusive: false,
            DomainShortcut: null,
            Scale: null,
            RangeStringMin: null,
            RangeStringMax: null,
            CustomExceptionType: null);

        var maxCode = NumericPrimitiveGenerator.GenerateNumericPrimitive(maxOnly);
        maxCode.Should().Contain("if (value > (double)100)");
        maxCode.Should().NotContain("must be at least");

        var strMinOnly = new NumericPrimitiveTypeInfo(
            Namespace: "TestNamespace",
            TypeName: "StrMinOnlyPrim",
            BackingTypeName: "decimal",
            Accessibility: "public",
            ContainingTypes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            AllowAddition: false,
            AllowSubtraction: false,
            AllowScalarMultiplication: false,
            AllowScalarDivision: false,
            AllowNegation: false,
            RangeMin: null,
            RangeMax: null,
            RangeMinExclusive: false,
            RangeMaxExclusive: false,
            DomainShortcut: null,
            Scale: null,
            RangeStringMin: "10.5",
            RangeStringMax: null,
            CustomExceptionType: null);

        var strMinCode = NumericPrimitiveGenerator.GenerateNumericPrimitive(strMinOnly);
        strMinCode.Should().Contain("if (value < 10.5m)");

        var strMaxOnly = new NumericPrimitiveTypeInfo(
            Namespace: "TestNamespace",
            TypeName: "StrMaxOnlyPrim",
            BackingTypeName: "decimal",
            Accessibility: "public",
            ContainingTypes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            AllowAddition: false,
            AllowSubtraction: false,
            AllowScalarMultiplication: false,
            AllowScalarDivision: false,
            AllowNegation: false,
            RangeMin: null,
            RangeMax: null,
            RangeMinExclusive: false,
            RangeMaxExclusive: false,
            DomainShortcut: null,
            Scale: null,
            RangeStringMin: null,
            RangeStringMax: "99.5",
            CustomExceptionType: null);

        var strMaxCode = NumericPrimitiveGenerator.GenerateNumericPrimitive(strMaxOnly);
        strMaxCode.Should().Contain("if (value > 99.5m)");

        var noVal = new NumericPrimitiveTypeInfo(
            Namespace: "TestNamespace",
            TypeName: "NoValPrim",
            BackingTypeName: "int",
            Accessibility: "public",
            ContainingTypes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            AllowAddition: false,
            AllowSubtraction: false,
            AllowScalarMultiplication: false,
            AllowScalarDivision: false,
            AllowNegation: false,
            RangeMin: null,
            RangeMax: null,
            RangeMinExclusive: false,
            RangeMaxExclusive: false,
            DomainShortcut: null,
            Scale: null,
            RangeStringMin: null,
            RangeStringMax: null,
            CustomExceptionType: null);

        var noValCode = NumericPrimitiveGenerator.GenerateNumericPrimitive(noVal);
        noValCode.Should().Contain("return global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError.None;");
    }
}


