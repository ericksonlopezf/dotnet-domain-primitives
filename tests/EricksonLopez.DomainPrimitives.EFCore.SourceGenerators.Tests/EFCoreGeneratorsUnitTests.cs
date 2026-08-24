// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Immutable;
using System.Linq;
using AwesomeAssertions;
using EricksonLopez.DomainPrimitives.EFCore.SourceGenerators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace EricksonLopez.DomainPrimitives.EFCore.SourceGenerators.Tests;

public class EFCoreGeneratorsUnitTests
{
    private static Compilation CreateCompilation(string source)
    {
        string dummyAttributes = @"
namespace EricksonLopez.DomainPrimitives
{
    public class EFCoreAttribute : System.Attribute { }
    public class ValueObjectAttribute : System.Attribute { }
    public class StringPrimitiveAttribute : System.Attribute { }
    public class StrongIdAttribute<T> : System.Attribute { }
    public class DatePrimitiveAttribute : System.Attribute { public int Kind { get; set; } }
    public class NumericPrimitiveAttribute<T> : System.Attribute { }
    public class PercentageAttribute : System.Attribute { }
    public class SmartEnumAttribute : System.Attribute { }
    public class SmartEnumAttribute<T> : System.Attribute { }
    public class MoneyAttribute : System.Attribute { }
    public class EmailAttribute : System.Attribute { }
    public class PhoneAttribute : System.Attribute { }
    public class UrlAttribute : System.Attribute { }
    public class SlugAttribute : System.Attribute { }
    public class CountryCodeAttribute : System.Attribute { }
    public class LanguageCodeAttribute : System.Attribute { }
    public class CurrencyCodeAttribute : System.Attribute { }
    public class UsernameAttribute : System.Attribute { }
    public class PasswordHashAttribute : System.Attribute { }
    public class HexColorAttribute : System.Attribute { }
    public class IPAddressAttribute : System.Attribute { }
    public class MacAddressAttribute : System.Attribute { }
    public class IBANAttribute : System.Attribute { }
    public class ISBNAttribute : System.Attribute { }
    public class VINAttribute : System.Attribute { }
    public class LatitudeAttribute : System.Attribute { }
    public class LongitudeAttribute : System.Attribute { }
    public class AgeAttribute : System.Attribute { }
    public class WeightAttribute : System.Attribute { }
    public class HeightAttribute : System.Attribute { }
    public class DistanceAttribute : System.Attribute { }
    public class TemperatureAttribute : System.Attribute { }
    public class ScoreAttribute : System.Attribute { }
    public class QuantityAttribute : System.Attribute { }
    public class PriceAttribute : System.Attribute { }
    public class TaxRateAttribute : System.Attribute { }
    public class DiscountAttribute : System.Attribute { }
    public class RatingAttribute : System.Attribute { }
    public class BirthDateAttribute : System.Attribute { }
    public class ExpirationDateAttribute : System.Attribute { }
    public class BusinessDateAttribute : System.Attribute { }
    public class FiscalYearAttribute : System.Attribute { }
    public class MonthAttribute : System.Attribute { }
    public class QuarterAttribute : System.Attribute { }
    public class WeekAttribute : System.Attribute { }
    public class DateRangeAttribute : System.Attribute { }
    public class TimeRangeAttribute : System.Attribute { }
}
namespace EricksonLopez.DomainPrimitives.Validation
{
    public class MaxLengthAttribute : System.Attribute { public MaxLengthAttribute(int max) {} }
    public class LengthAttribute : System.Attribute { public LengthAttribute(int min, int max) {} }
}
namespace System
{
    public struct Guid {}
    public struct DateOnly { public static DateOnly FromDateTime(DateTime dt) => default; }
    public struct DateTime {}
    public struct TimeOnly {}
    public struct DateTimeOffset {}
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
    public void Generator_WithGlobalNamespace_GeneratesWithoutUsingNamespace()
    {
        string source = @"
[EricksonLopez.DomainPrimitives.StringPrimitiveAttribute]
public readonly partial struct GlobalPrimitive { }
";
        var compilation = CreateCompilation(source);
        var generator = new EFCoreValueConverterGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("class GlobalPrimitiveValueConverter : ValueConverter<GlobalPrimitive, string>");
        generatedSource.Should().NotContain("using <global namespace>;");
        generatedSource.Should().Contain("configurationBuilder.Properties<GlobalPrimitive>()");
    }

    [Fact]
    public void Generator_WithClass_OrValueObject_ShouldNotGenerateConverters()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.StringPrimitiveAttribute]
public class ClassPrimitive { }

[EricksonLopez.DomainPrimitives.ValueObjectAttribute]
public readonly partial record struct VoPrimitive { public int X { get; } }

public readonly partial struct NoAttrStruct { }
";
        var compilation = CreateCompilation(source);
        var generator = new EFCoreValueConverterGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedTrees = outputCompilation.SyntaxTrees.Skip(2).ToList();
        generatedTrees.Should().BeEmpty();
    }

    [Fact]
    public void Generator_WithDatePrimitiveKindZero_DefaultsToDateOnly()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.DatePrimitiveAttribute(Kind = 0)]
public readonly partial struct ExactDateOnly { }
";
        var compilation = CreateCompilation(source);
        var generator = new EFCoreValueConverterGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("class ExactDateOnlyValueConverter : ValueConverter<ExactDateOnly, global::System.DateOnly>");
    }

    [Fact]
    public void Generator_WithVariousShortcutAttributes_GeneratesConverters()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.EmailAttribute]
public readonly partial struct UserEmail { }

[EricksonLopez.DomainPrimitives.PhoneAttribute]
public readonly partial struct UserPhone { }

[EricksonLopez.DomainPrimitives.BirthDateAttribute]
public readonly partial struct UserBirthDate { }

[EricksonLopez.DomainPrimitives.QuantityAttribute]
public readonly partial struct ItemQty { }
";
        var compilation = CreateCompilation(source);
        var generator = new EFCoreValueConverterGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("class UserEmailValueConverter");
        generatedSource.Should().Contain("class UserPhoneValueConverter");
        generatedSource.Should().Contain("class UserBirthDateValueConverter");
        generatedSource.Should().Contain("class ItemQtyValueConverter");
    }

    [Fact]
    public void PrimitiveInfo_ComprehensiveEqualityAndHashCode()
    {
        var baseInfo = new PrimitiveInfo("NS", "MyType", "string", isSmartEnum: false, maxLength: 50, precision: 10, scale: 2);
        var sameInfo = new PrimitiveInfo("NS", "MyType", "string", isSmartEnum: false, maxLength: 50, precision: 10, scale: 2);
        
        var diffNs = new PrimitiveInfo("OtherNS", "MyType", "string", isSmartEnum: false, maxLength: 50, precision: 10, scale: 2);
        var diffName = new PrimitiveInfo("NS", "OtherType", "string", isSmartEnum: false, maxLength: 50, precision: 10, scale: 2);
        var diffBacking = new PrimitiveInfo("NS", "MyType", "int", isSmartEnum: false, maxLength: 50, precision: 10, scale: 2);
        var diffEnum = new PrimitiveInfo("NS", "MyType", "string", isSmartEnum: true, maxLength: 50, precision: 10, scale: 2);
        var diffMax = new PrimitiveInfo("NS", "MyType", "string", isSmartEnum: false, maxLength: 100, precision: 10, scale: 2);
        var diffPrec = new PrimitiveInfo("NS", "MyType", "string", isSmartEnum: false, maxLength: 50, precision: 18, scale: 2);
        var diffScale = new PrimitiveInfo("NS", "MyType", "string", isSmartEnum: false, maxLength: 50, precision: 10, scale: 4);

        baseInfo.Equals(sameInfo).Should().BeTrue();
        baseInfo.Equals((object)sameInfo).Should().BeTrue();
        baseInfo.Equals(diffNs).Should().BeFalse();
        baseInfo.Equals(diffName).Should().BeFalse();
        baseInfo.Equals(diffBacking).Should().BeFalse();
        baseInfo.Equals(diffEnum).Should().BeFalse();
        baseInfo.Equals(diffMax).Should().BeFalse();
        baseInfo.Equals(diffPrec).Should().BeFalse();
        baseInfo.Equals(diffScale).Should().BeFalse();
        baseInfo.Equals(null).Should().BeFalse();
        baseInfo.Equals("some other object").Should().BeFalse();

        baseInfo.GetHashCode().Should().Be(sameInfo.GetHashCode());
        baseInfo.GetHashCode().Should().NotBe(diffNs.GetHashCode());
        baseInfo.GetHashCode().Should().NotBe(diffName.GetHashCode());
        baseInfo.GetHashCode().Should().NotBe(diffBacking.GetHashCode());
        baseInfo.GetHashCode().Should().NotBe(diffEnum.GetHashCode());
        baseInfo.GetHashCode().Should().NotBe(diffMax.GetHashCode());
        baseInfo.GetHashCode().Should().NotBe(diffPrec.GetHashCode());
        baseInfo.GetHashCode().Should().NotBe(diffScale.GetHashCode());
    }

    [Fact]
    public void Generator_WithAttributeFromDifferentNamespace_ShouldIgnore()
    {
        string source = @"
namespace CustomNamespace
{
    public class StringPrimitiveAttribute : System.Attribute { }
}
namespace TestNamespace
{
    [CustomNamespace.StringPrimitiveAttribute]
    public readonly partial struct UnrelatedStruct { }
}
";
        var compilation = CreateCompilation(source);
        var generator = new EFCoreValueConverterGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedTrees = outputCompilation.SyntaxTrees.Skip(2).ToList();
        generatedTrees.Should().BeEmpty();
    }

    [Fact]
    public void Generator_WithMultiplePrimitives_GeneratesAllConvertersAndRegistersThem()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.StringPrimitiveAttribute]
public readonly partial struct Code1 { }

[EricksonLopez.DomainPrimitives.MoneyAttribute]
public readonly partial struct MoneyVal { }
";
        var compilation = CreateCompilation(source);
        var generator = new EFCoreValueConverterGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        generatedSource.Should().Contain("class Code1ValueConverter");
        generatedSource.Should().Contain("class MoneyValValueConverter");
        generatedSource.Should().Contain("configurationBuilder.Properties<TestNamespace.Code1>()");
        generatedSource.Should().Contain("configurationBuilder.Properties<TestNamespace.MoneyVal>()");
    }
}


