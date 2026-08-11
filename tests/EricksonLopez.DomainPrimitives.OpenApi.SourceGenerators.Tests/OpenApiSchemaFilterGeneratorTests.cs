using System;
using System.Linq;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;
using EricksonLopez.DomainPrimitives.OpenApi.SourceGenerators;

namespace EricksonLopez.DomainPrimitives.OpenApi.SourceGenerators.Tests;

public class OpenApiSchemaFilterGeneratorTests
{
    private static CSharpParseOptions ParseOptions => new CSharpParseOptions(LanguageVersion.CSharp11);

    private static Compilation CreateCompilation(string source)
    {
        string dummyAttributes = @"
namespace EricksonLopez.DomainPrimitives
{
    public class JsonAttribute : System.Attribute { }
    public class MapsterAttribute : System.Attribute { }
    public class OpenApiAttribute : System.Attribute { }
    public class StringPrimitiveAttribute : System.Attribute { }
    public class ValueObjectAttribute : System.Attribute { }
    public class NumericPrimitiveAttribute<T> : System.Attribute { }

public class AgeAttribute : System.Attribute { }
public class BirthDateAttribute : System.Attribute { }
public class BusinessDateAttribute : System.Attribute { }
public class CountryCodeAttribute : System.Attribute { }
public class CurrencyCodeAttribute : System.Attribute { }
public class DatePrimitiveAttribute : System.Attribute { public int Kind { get; set; } }
public class DateRangeAttribute : System.Attribute { }
public class DiscountAttribute : System.Attribute { }
public class DistanceAttribute : System.Attribute { }
public class EmailAttribute : System.Attribute { }
public class ExpirationDateAttribute : System.Attribute { }
public class FiscalYearAttribute : System.Attribute { }
public class HeightAttribute : System.Attribute { }
public class HexColorAttribute : System.Attribute { }
public class LanguageCodeAttribute : System.Attribute { }
public class LatitudeAttribute : System.Attribute { }
public class LongitudeAttribute : System.Attribute { }
public class MoneyAttribute : System.Attribute { }
public class MonthAttribute : System.Attribute { }
public class PasswordHashAttribute : System.Attribute { }
public class PercentageAttribute : System.Attribute { }
public class PhoneAttribute : System.Attribute { }
public class PriceAttribute : System.Attribute { }
public class QuantityAttribute : System.Attribute { }
public class QuarterAttribute : System.Attribute { }
public class RatingAttribute : System.Attribute { }
public class ScoreAttribute : System.Attribute { }
public class SlugAttribute : System.Attribute { }
public class SmartEnumAttribute : System.Attribute { }
public class SmartEnumAttribute<T> : System.Attribute { }
public class StrongIdAttribute<T> : System.Attribute { }
public class TaxRateAttribute : System.Attribute { }
public class TemperatureAttribute : System.Attribute { }
public class TimeRangeAttribute : System.Attribute { }
public class UrlAttribute : System.Attribute { }
public class UsernameAttribute : System.Attribute { }
public class WeekAttribute : System.Attribute { }
public class WeightAttribute : System.Attribute { }

}
namespace System
{
    public struct Guid {}
    public struct DateTime {}
    public struct DateTimeOffset {}
    public struct TimeOnly {}
    public struct DateOnly { public static DateOnly FromDateTime(DateTime dt) => default; }
}
";
        var syntaxTrees = new[] { 
            CSharpSyntaxTree.ParseText(source, ParseOptions),
            CSharpSyntaxTree.ParseText(dummyAttributes, ParseOptions)
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
    public void Generator_WithAllMissingBranches_ShouldGenerateCode()
    {
        string source = @"
namespace TestNamespace;
[EricksonLopez.DomainPrimitives.StrongIdAttribute<long>]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct0 { }

[EricksonLopez.DomainPrimitives.StrongIdAttribute<short>]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct1 { }

[EricksonLopez.DomainPrimitives.StrongIdAttribute<byte>]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct2 { }

[EricksonLopez.DomainPrimitives.StrongIdAttribute<uint>]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct3 { }

[EricksonLopez.DomainPrimitives.StrongIdAttribute<ulong>]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct4 { }

[EricksonLopez.DomainPrimitives.StrongIdAttribute<ushort>]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct5 { }

[EricksonLopez.DomainPrimitives.StrongIdAttribute<sbyte>]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct6 { }

[EricksonLopez.DomainPrimitives.StrongIdAttribute<float>]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct7 { }

[EricksonLopez.DomainPrimitives.StrongIdAttribute<double>]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct8 { }

[EricksonLopez.DomainPrimitives.StrongIdAttribute<decimal>]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct9 { }

[EricksonLopez.DomainPrimitives.DatePrimitiveAttribute(Kind = 1)]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct10 { }

[EricksonLopez.DomainPrimitives.DatePrimitiveAttribute(Kind = 2)]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct11 { }

[EricksonLopez.DomainPrimitives.DatePrimitiveAttribute(Kind = 3)]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct12 { }

[EricksonLopez.DomainPrimitives.DatePrimitiveAttribute]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct13 { }

[EricksonLopez.DomainPrimitives.MoneyAttribute]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct14 { }

[EricksonLopez.DomainPrimitives.PercentageAttribute]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct15 { }

[EricksonLopez.DomainPrimitives.PriceAttribute]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct16 { }

[EricksonLopez.DomainPrimitives.TaxRateAttribute]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct17 { }

[EricksonLopez.DomainPrimitives.DiscountAttribute]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct18 { }

[EricksonLopez.DomainPrimitives.LatitudeAttribute]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct19 { }

[EricksonLopez.DomainPrimitives.LongitudeAttribute]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct20 { }

[EricksonLopez.DomainPrimitives.WeightAttribute]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct21 { }

[EricksonLopez.DomainPrimitives.HeightAttribute]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct22 { }

[EricksonLopez.DomainPrimitives.DistanceAttribute]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct23 { }

[EricksonLopez.DomainPrimitives.TemperatureAttribute]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct24 { }

[EricksonLopez.DomainPrimitives.ScoreAttribute]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct25 { }

[EricksonLopez.DomainPrimitives.QuantityAttribute]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct26 { }

[EricksonLopez.DomainPrimitives.RatingAttribute]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct27 { }

[EricksonLopez.DomainPrimitives.AgeAttribute]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct28 { }

[EricksonLopez.DomainPrimitives.SmartEnumAttribute]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct29 { }

[EricksonLopez.DomainPrimitives.SmartEnumAttribute<long>]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct30 { }

[EricksonLopez.DomainPrimitives.FiscalYearAttribute]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct31 { }

[EricksonLopez.DomainPrimitives.MonthAttribute]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct32 { }

[EricksonLopez.DomainPrimitives.QuarterAttribute]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct33 { }

[EricksonLopez.DomainPrimitives.WeekAttribute]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct34 { }

[EricksonLopez.DomainPrimitives.TimeRangeAttribute]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct35 { }

[EricksonLopez.DomainPrimitives.DateRangeAttribute]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct36 { }

[EricksonLopez.DomainPrimitives.BirthDateAttribute]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct37 { }

[EricksonLopez.DomainPrimitives.ExpirationDateAttribute]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct38 { }

[EricksonLopez.DomainPrimitives.BusinessDateAttribute]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct39 { }

[EricksonLopez.DomainPrimitives.EmailAttribute]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct40 { }

[EricksonLopez.DomainPrimitives.PhoneAttribute]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct41 { }

[EricksonLopez.DomainPrimitives.UrlAttribute]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct42 { }

[EricksonLopez.DomainPrimitives.SlugAttribute]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct43 { }

[EricksonLopez.DomainPrimitives.CountryCodeAttribute]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct44 { }

[EricksonLopez.DomainPrimitives.LanguageCodeAttribute]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct45 { }

[EricksonLopez.DomainPrimitives.CurrencyCodeAttribute]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct46 { }

[EricksonLopez.DomainPrimitives.UsernameAttribute]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct47 { }

[EricksonLopez.DomainPrimitives.PasswordHashAttribute]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct48 { }

[EricksonLopez.DomainPrimitives.HexColorAttribute]
[EricksonLopez.DomainPrimitives.OpenApiAttribute]
public readonly partial struct MyStruct49 { }


";
        var compilation = CreateCompilation(source);
        var generator = new OpenApiSchemaFilterGenerator();
        var driver = CSharpGeneratorDriver.Create(new[] { generator.AsSourceGenerator() }, parseOptions: ParseOptions);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        var generatedSource = string.Join(Environment.NewLine, outputCompilation.SyntaxTrees.Skip(2).Select(t => t.ToString()));
        
        // Assert we generate classes
        generatedSource.Should().NotBeNullOrWhiteSpace();
    }
}
