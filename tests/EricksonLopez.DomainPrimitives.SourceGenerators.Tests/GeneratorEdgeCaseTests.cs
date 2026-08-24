// Copyright © Erickson Lopez. MIT License.
using System.Linq;
using System.Threading.Tasks;
using AwesomeAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;
using EricksonLopez.DomainPrimitives.Generators;

namespace EricksonLopez.DomainPrimitives.Generators.Tests;

public class GeneratorEdgeCaseTests
{
    private static GeneratorDriverRunResult RunGenerator(IIncrementalGenerator generator, string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(EricksonLopez.DomainPrimitives.SourceGenerators.Tests.TestCompilationHelper.EnsureUsings(source), new CSharpParseOptions(LanguageVersion.CSharp11));
        var compilation = CSharpCompilation.Create(
            "Tests",
            new[] { syntaxTree },
            Basic.Reference.Assemblies.Net80.References.All,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var driver = CSharpGeneratorDriver.Create(generator);
        var driverResult = driver.RunGenerators(compilation);
        return driverResult.GetRunResult();
    }

    [Fact]
    public void StringPrimitive_EdgeCases_AreCovered()
    {
        var source = @"
using System;
using EricksonLopez.DomainPrimitives;
using EricksonLopez.DomainPrimitives.Normalization;

[assembly: DomainPrimitivesDefaults(Trim = true, NotEmpty = true, MaxLength = 100, ExceptionType = typeof(ArgumentException))]

namespace TestNamespace1 {
    using StringPrimitiveAttribute = System.ComponentModel.DescriptionAttribute;
    [StringPrimitive] public record struct FakeAttributeStruct {}
}

namespace TestNamespace {
    [StringPrimitive] public record NotAStructRecord {}
    [System.ComponentModel.Description] public record struct NoAttributesStruct {}

    [StringPrimitive]
    [MinLength(5)]
    [MaxLength(10)]
    [NotEmpty]
    public readonly partial record struct MinMaxPrimitive;

    [StringPrimitive]
    [Length(1, 100)]
    [ExactLength(10)]
    [TrimStart]
    [TrimEnd]
    [LowerCase]
    [UpperCase]
    [NormalizeWhitespace]
    [Regex(@""^[A-Z]+$"", ""Must be upper"")]
    public readonly partial record struct LengthPrimitive;

    public class CustomValidatorAttribute : System.Attribute {}
    [StringPrimitive]
    [CustomValidator]
    public readonly partial record struct InvalidValidatorPrimitive;

    [Email(MaxLength = 50)] public readonly partial record struct EmailPrim;
    [Phone] public readonly partial record struct PhonePrim;
    [Url(AllowedSchemes = new[] { ""https"", ""ftp"" })] internal readonly partial record struct UrlPrim;
    [Url] public readonly partial record struct DefaultHttpUrl;
    [Url(AllowedSchemes = new[] { ""https"" })] public readonly partial record struct SingleSchemeUrl;
    [Url(AllowedSchemes = new[] { ""http"", ""https"" })] public readonly partial record struct CustomOrderedHttpUrl;
    [Slug(MaxLength = 40)] protected internal readonly partial record struct SlugPrim;
    [CountryCode] public readonly partial record struct CountryPrim;
    [LanguageCode] public readonly partial record struct LangPrim;
    [CurrencyCode] public readonly partial record struct CurrencyPrim;
    [Username(MinLength = 4, MaxLength = 16)] public readonly partial record struct UsernamePrim;
    [PasswordHash] public readonly partial record struct PwdHashPrim;
    [HexColor] public readonly partial record struct HexPrim;
    [IPAddress] public readonly partial record struct IpPrim;
    [MacAddress] public readonly partial record struct MacPrim;
    [IBAN] public readonly partial record struct IbanPrim;
    [ISBN] public readonly partial record struct IsbnPrim;
    [VIN] public readonly partial record struct VinPrim;
    [StringPrimitive] [Email] public readonly partial record struct DupAttrPrim;

    [StringPrimitive] [Trim] public readonly partial record struct TrimOnlyNoValidation;
    [StringPrimitive] public readonly partial record struct PlainNoValidation;
    [StringPrimitive] [TrimStart] public readonly partial record struct TrimStartOnly;
    [StringPrimitive] [TrimEnd] public readonly partial record struct TrimEndOnly;
    [StringPrimitive] [NormalizeWhitespace] [Trim] public readonly partial record struct NormWsWithTrim;
    [StringPrimitive] [NormalizeWhitespace] public readonly partial record struct NormWsNoTrim;

    [Email] [Regex(@""^custom@"", ""Custom email regex"")] public readonly partial record struct EmailWithCustomRegex;
    [Phone] [Regex(@""^\d+$"", ""Digits only"")] public readonly partial record struct PhoneWithCustomRegex;
    [Slug] [Regex(@""^[a-z]+$"", ""Simple slug"")] public readonly partial record struct SlugWithCustomRegex;
    [Username] [Regex(@""^[a-z]+$"", ""Simple username"")] public readonly partial record struct UsernameWithCustomRegex;
    [StringPrimitive]
    [Regex(@""^[A-Z]"", ""Must start with capital"")]
    [Regex(@""\d$"", ""Must end with digit"")]
    public readonly partial record struct MultiRegexPrimitive;
    [StringPrimitive]
    [Regex(@""^[a-z]+$"", ErrorMessage = ""Only lowercase"")]
    public readonly partial record struct NamedErrorRegexPrimitive;
    [StringPrimitive]
    [Regex(@""^[0-9]+$"")]
    public readonly partial record struct RegexWithDefaultError;
    [StringPrimitive]
    [Regex(@""^[0-9]+$"", ""Custom format message."")]
    public readonly partial record struct RegexWithCustomError;
    [StringPrimitive]
    [Regex(@""""""hello""""world\\test"", @""Must match """"hello"""" with \\ test"")]
    public readonly partial record struct EscapedRegexPrimitive;

    public class OuterClass {
        [StringPrimitive]
        public readonly partial record struct NestedStringPrimitive;

        [StringPrimitive]
        private readonly partial record struct PrivateNestedPrimitive;
    }
}

namespace EricksonLopez.DomainPrimitives
{
    public class DomainPrimitivesDefaultsAttribute : System.Attribute { public bool Trim { get; set; } public bool NotEmpty { get; set; } public int MaxLength { get; set; } public Type ExceptionType { get; set; } }
    public class StringPrimitiveAttribute : System.Attribute {}
    public class EmailAttribute : System.Attribute { public int MaxLength { get; set; } }
    public class PhoneAttribute : System.Attribute {}
    public class UrlAttribute : System.Attribute { public string[] AllowedSchemes { get; set; } }
    public class SlugAttribute : System.Attribute { public int MaxLength { get; set; } }
    public class CountryCodeAttribute : System.Attribute {}
    public class LanguageCodeAttribute : System.Attribute {}
    public class CurrencyCodeAttribute : System.Attribute {}
    public class UsernameAttribute : System.Attribute { public int MinLength { get; set; } public int MaxLength { get; set; } }
    public class PasswordHashAttribute : System.Attribute {}
    public class HexColorAttribute : System.Attribute {}
    public class IPAddressAttribute : System.Attribute {}
    public class MacAddressAttribute : System.Attribute {}
    public class IBANAttribute : System.Attribute {}
    public class ISBNAttribute : System.Attribute {}
    public class VINAttribute : System.Attribute {}
}

namespace EricksonLopez.DomainPrimitives.Normalization
{
    public class MinLengthAttribute : System.Attribute { public MinLengthAttribute(int i) {} }
    public class MaxLengthAttribute : System.Attribute { public MaxLengthAttribute(int i) {} }
    public class LengthAttribute : System.Attribute { public LengthAttribute(int min, int max) {} }
    public class ExactLengthAttribute : System.Attribute { public ExactLengthAttribute(int l) {} }
    public class NotEmptyAttribute : System.Attribute {}
    public class TrimStartAttribute : System.Attribute {}
    public class TrimEndAttribute : System.Attribute {}
    public class LowerCaseAttribute : System.Attribute {}
    public class UpperCaseAttribute : System.Attribute {}
    public class NormalizeWhitespaceAttribute : System.Attribute {}
    public class RegexAttribute : System.Attribute { public RegexAttribute(string pattern, string error = null) {} public string ErrorMessage { get; set; } }
}
";
        var runResult = RunGenerator(new StringPrimitiveGenerator(), source);
        var allGenerated = string.Join("\n", runResult.GeneratedTrees.Select(t => t.ToString()));

        allGenerated.Should().Contain("throw new global::System.ArgumentException(error.Message);");

        var ifExactMatches = System.Text.RegularExpressions.Regex.Matches(allGenerated, "if \\(value\\.Length != 10\\)");
        ifExactMatches.Count.Should().Be(2);

        var exactLengthMatches = System.Text.RegularExpressions.Regex.Matches(allGenerated, "must be exactly 10 character\\(s\\)\\. Got \\{value\\.Length\\}\\.");
        exactLengthMatches.Count.Should().Be(2);

        var httpMatches = System.Text.RegularExpressions.Regex.Matches(allGenerated, "must be a valid absolute HTTP\\(S\\) URL\\.");
        httpMatches.Count.Should().Be(2);

        var ftpMatches = System.Text.RegularExpressions.Regex.Matches(allGenerated, "must be a valid absolute HTTPS/FTP URL\\.");
        ftpMatches.Count.Should().Be(2);

        var singleSchemeMatches = System.Text.RegularExpressions.Regex.Matches(allGenerated, "must be a valid absolute HTTPS URL\\.");
        singleSchemeMatches.Count.Should().Be(2);

        var customOrderedMatches = System.Text.RegularExpressions.Regex.Matches(allGenerated, "must be a valid absolute HTTP/HTTPS URL\\.");
        customOrderedMatches.Count.Should().Be(2);

        var r1Matches = System.Text.RegularExpressions.Regex.Matches(allGenerated, "ValidationRegex1\\.IsMatch\\(value\\)");
        r1Matches.Count.Should().Be(2);

        var r2Matches = System.Text.RegularExpressions.Regex.Matches(allGenerated, "ValidationRegex2\\.IsMatch\\(value\\)");
        r2Matches.Count.Should().Be(2);

        var invalidSpanMatches = System.Text.RegularExpressions.Regex.Matches(allGenerated, "throw new System\\.FormatException\\(\"The span value is not valid\\.\"\\);");
        invalidSpanMatches.Count.Should().BeGreaterThan(0);

        allGenerated.Should().Contain("CountryPrim must be at least 2 character(s)");
        allGenerated.Should().Contain("CountryPrim must be at most 2 character(s)");
        allGenerated.Should().Contain("LangPrim must be at least 2 character(s)");
        allGenerated.Should().Contain("LangPrim must be at most 2 character(s)");
        allGenerated.Should().Contain("CurrencyPrim must be at least 3 character(s)");
        allGenerated.Should().Contain("CurrencyPrim must be at most 3 character(s)");
        allGenerated.Should().Contain("UsernamePrim must be at least 4 character(s)");
        allGenerated.Should().Contain("UsernamePrim must be at most 16 character(s)");
        allGenerated.Should().Contain("SlugPrim must be at most 40 character(s)");
        allGenerated.Should().Contain("EmailPrim must be at most 50 character(s)");
        allGenerated.Should().Contain("PlainNoValidation must be at most 100 character(s)");
        allGenerated.Should().Contain("PlainNoValidation must not be empty.");

        allGenerated.Should().Contain("value = value.TrimStart();");
        allGenerated.Should().Contain("value = value.TrimEnd();");
        allGenerated.Should().Contain("var trimmed = s.ToString().Trim();");
        allGenerated.Should().Contain("private static readonly System.Text.RegularExpressions.Regex ValidationRegex1 =");
        allGenerated.Should().Contain("private static readonly System.Text.RegularExpressions.Regex ValidationRegex2 =");
        allGenerated.Should().Contain("private static readonly System.Text.RegularExpressions.Regex ValidationRegex =");
        allGenerated.Should().Contain("private static readonly System.Text.RegularExpressions.Regex WhitespaceRegex =");
        allGenerated.Should().Contain("RegexWithDefaultError has an invalid format.");
        allGenerated.Should().Contain("Only lowercase");
        allGenerated.Should().Contain("Custom format message.");

        runResult.GeneratedTrees.Count(t => t.FilePath.EndsWith("DupAttrPrim.g.cs", System.StringComparison.Ordinal)).Should().Be(1);

        var noDefaultsSource = @"
using EricksonLopez.DomainPrimitives;
using EricksonLopez.DomainPrimitives.Normalization;

namespace TestNamespace {
    [StringPrimitive] [NormalizeWhitespace] public readonly partial record struct NormWsNoTrim;
}
namespace EricksonLopez.DomainPrimitives {
    public class StringPrimitiveAttribute : System.Attribute {}
}
namespace EricksonLopez.DomainPrimitives.Normalization {
    public class NormalizeWhitespaceAttribute : System.Attribute {}
}
";
        var noDefaultsRun = RunGenerator(new StringPrimitiveGenerator(), noDefaultsSource);
        var noDefaultsGen = string.Join("\n", noDefaultsRun.GeneratedTrees.Select(t => t.ToString()));
        noDefaultsGen.Should().Contain("var trimmed = s.ToString();");
    }

    [Fact]
    public void StringPrimitive_ExtractTypeInfo_EdgeCases()
    {
        var source = @"
using System;
using EricksonLopez.DomainPrimitives;
using EricksonLopez.DomainPrimitives.Normalization;

[assembly: DomainPrimitivesDefaults(Trim = true, NotEmpty = true, MaxLength = 120)]

namespace TestNamespace {
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public record struct PlainStruct;

    [StringPrimitive]
    [Length(5, 50)]
    [ExactLength(10)]
    public readonly partial record struct ExactAndLengthStruct;

    [Email(MaxLength = 80)]
    public readonly partial record struct CustomEmailStruct;

    [Email]
    public readonly partial record struct DefaultEmailStruct;

    [Phone]
    public readonly partial record struct DefaultPhoneStruct;

    [Url]
    public readonly partial record struct DefaultUrlStruct;

    [Slug]
    public readonly partial record struct DefaultSlugStruct;

    [CountryCode]
    public readonly partial record struct DefaultCountryStruct;

    [LanguageCode]
    public readonly partial record struct DefaultLangStruct;

    [CurrencyCode]
    public readonly partial record struct DefaultCurrencyStruct;

    [Username]
    public readonly partial record struct DefaultUsernameStruct;

    [PasswordHash]
    public readonly partial record struct DefaultPwdStruct;

    [HexColor]
    public readonly partial record struct DefaultHexStruct;

    [IPAddress]
    public readonly partial record struct DefaultIpStruct;

    [MacAddress]
    public readonly partial record struct DefaultMacStruct;

    [IBAN]
    public readonly partial record struct DefaultIbanStruct;

    [ISBN]
    public readonly partial record struct DefaultIsbnStruct;

    [VIN]
    public readonly partial record struct DefaultVinStruct;

    [StringPrimitive]
    [TrimStart] [TrimEnd] [LowerCase] [UpperCase] [NormalizeWhitespace] [NotEmpty]
    public readonly partial record struct AllFlagsStruct;

    [StringPrimitive]
    public readonly partial record struct InheritsAssemblyDefaults;

    public class OuterLevel1 {
        internal class OuterLevel2 {
            [StringPrimitive]
            private readonly partial record struct DeepNestedStruct;
            [StringPrimitive]
            protected internal readonly partial record struct DeepProtIntStruct;
            [StringPrimitive]
            internal readonly partial record struct DeepIntStruct;
        }
    }
}
namespace EricksonLopez.DomainPrimitives {
    public class DomainPrimitivesDefaultsAttribute : System.Attribute { public bool Trim { get; set; } public bool NotEmpty { get; set; } public int MaxLength { get; set; } public Type ExceptionType { get; set; } }
    public class StringPrimitiveAttribute : System.Attribute {}
    public class EmailAttribute : System.Attribute { public int MaxLength { get; set; } }
    public class PhoneAttribute : System.Attribute {}
    public class UrlAttribute : System.Attribute { public string[] AllowedSchemes { get; set; } }
    public class SlugAttribute : System.Attribute { public int MaxLength { get; set; } }
    public class CountryCodeAttribute : System.Attribute {}
    public class LanguageCodeAttribute : System.Attribute {}
    public class CurrencyCodeAttribute : System.Attribute {}
    public class UsernameAttribute : System.Attribute { public int MinLength { get; set; } public int MaxLength { get; set; } }
    public class PasswordHashAttribute : System.Attribute {}
    public class HexColorAttribute : System.Attribute {}
    public class IPAddressAttribute : System.Attribute {}
    public class MacAddressAttribute : System.Attribute {}
    public class IBANAttribute : System.Attribute {}
    public class ISBNAttribute : System.Attribute {}
    public class VINAttribute : System.Attribute {}
}
namespace EricksonLopez.DomainPrimitives.Normalization {
    public class LengthAttribute : System.Attribute { public LengthAttribute(int min, int max) {} }
    public class ExactLengthAttribute : System.Attribute { public ExactLengthAttribute(int l) {} }
    public class MinLengthAttribute : System.Attribute { public MinLengthAttribute(int l) {} }
    public class MaxLengthAttribute : System.Attribute { public MaxLengthAttribute(int l) {} }
    public class TrimStartAttribute : System.Attribute {}
    public class TrimEndAttribute : System.Attribute {}
    public class LowerCaseAttribute : System.Attribute {}
    public class UpperCaseAttribute : System.Attribute {}
    public class NormalizeWhitespaceAttribute : System.Attribute {}
    public class NotEmptyAttribute : System.Attribute {}
}
";
        var syntaxTree = CSharpSyntaxTree.ParseText(EricksonLopez.DomainPrimitives.SourceGenerators.Tests.TestCompilationHelper.EnsureUsings(source), new CSharpParseOptions(LanguageVersion.CSharp11));
        var compilation = CSharpCompilation.Create(
            "Tests",
            new[] { syntaxTree },
            Basic.Reference.Assemblies.Net80.References.All,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var root = syntaxTree.GetRoot();
        var records = root.DescendantNodes().OfType<Microsoft.CodeAnalysis.CSharp.Syntax.RecordDeclarationSyntax>().ToList();
        
        var plainStructSyntax = records.First(r => r.Identifier.Text == "PlainStruct");
        var resultPlain = StringPrimitiveGenerator.ExtractTypeInfo(semanticModel, plainStructSyntax, default);
        resultPlain.Should().BeNull();

        var exactStructSyntax = records.First(r => r.Identifier.Text == "ExactAndLengthStruct");
        
        // Cancellation test
        System.Action cancelAct = () => StringPrimitiveGenerator.ExtractTypeInfo(semanticModel, exactStructSyntax, new System.Threading.CancellationToken(true));
        cancelAct.Should().Throw<System.OperationCanceledException>();

        var resultExact = StringPrimitiveGenerator.ExtractTypeInfo(semanticModel, exactStructSyntax, default);
        resultExact.Should().NotBeNull();
        resultExact!.ExactLength.Should().Be(10);
        resultExact.MinLength.Should().Be(10);
        resultExact.MaxLength.Should().Be(10);

        var emailStructSyntax = records.First(r => r.Identifier.Text == "CustomEmailStruct");
        var resultEmail = StringPrimitiveGenerator.ExtractTypeInfo(semanticModel, emailStructSyntax, default);
        resultEmail.Should().NotBeNull();
        resultEmail!.DomainShortcut.Should().Be("Email");
        resultEmail.MaxLength.Should().Be(80);

        var defaultEmailSyntax = records.First(r => r.Identifier.Text == "DefaultEmailStruct");
        var resultDefaultEmail = StringPrimitiveGenerator.ExtractTypeInfo(semanticModel, defaultEmailSyntax, default);
        resultDefaultEmail!.MaxLength.Should().Be(320);
        resultDefaultEmail.LowerCase.Should().BeTrue();
        resultDefaultEmail.Trim.Should().BeTrue();

        var defaultSlugSyntax = records.First(r => r.Identifier.Text == "DefaultSlugStruct");
        var resultDefaultSlug = StringPrimitiveGenerator.ExtractTypeInfo(semanticModel, defaultSlugSyntax, default);
        resultDefaultSlug!.MaxLength.Should().Be(200);

        var defaultCountrySyntax = records.First(r => r.Identifier.Text == "DefaultCountryStruct");
        var resultCountry = StringPrimitiveGenerator.ExtractTypeInfo(semanticModel, defaultCountrySyntax, default);
        resultCountry!.MinLength.Should().Be(2);
        resultCountry.MaxLength.Should().Be(2);
        resultCountry.UpperCase.Should().BeTrue();

        var defaultLangSyntax = records.First(r => r.Identifier.Text == "DefaultLangStruct");
        var resultLang = StringPrimitiveGenerator.ExtractTypeInfo(semanticModel, defaultLangSyntax, default);
        resultLang!.MinLength.Should().Be(2);
        resultLang.MaxLength.Should().Be(2);
        resultLang.LowerCase.Should().BeTrue();

        var defaultCurrencySyntax = records.First(r => r.Identifier.Text == "DefaultCurrencyStruct");
        var resultCurrency = StringPrimitiveGenerator.ExtractTypeInfo(semanticModel, defaultCurrencySyntax, default);
        resultCurrency!.MinLength.Should().Be(3);
        resultCurrency.MaxLength.Should().Be(3);
        resultCurrency.UpperCase.Should().BeTrue();

        var defaultUsernameSyntax = records.First(r => r.Identifier.Text == "DefaultUsernameStruct");
        var resultUsername = StringPrimitiveGenerator.ExtractTypeInfo(semanticModel, defaultUsernameSyntax, default);
        resultUsername!.MinLength.Should().Be(3);
        resultUsername.MaxLength.Should().Be(50);

        var defaultPhoneSyntax = records.First(r => r.Identifier.Text == "DefaultPhoneStruct");
        var resultPhone = StringPrimitiveGenerator.ExtractTypeInfo(semanticModel, defaultPhoneSyntax, default);
        resultPhone!.NotEmpty.Should().BeTrue();

        var defaultUrlSyntax = records.First(r => r.Identifier.Text == "DefaultUrlStruct");
        var resultUrl = StringPrimitiveGenerator.ExtractTypeInfo(semanticModel, defaultUrlSyntax, default);
        resultUrl!.AllowedSchemes.Length.Should().Be(2);

        var defaultPwdSyntax = records.First(r => r.Identifier.Text == "DefaultPwdStruct");
        var resultPwd = StringPrimitiveGenerator.ExtractTypeInfo(semanticModel, defaultPwdSyntax, default);
        resultPwd!.NotEmpty.Should().BeTrue();

        var defaultHexSyntax = records.First(r => r.Identifier.Text == "DefaultHexStruct");
        var resultHex = StringPrimitiveGenerator.ExtractTypeInfo(semanticModel, defaultHexSyntax, default);
        resultHex!.UpperCase.Should().BeTrue();

        var defaultIpSyntax = records.First(r => r.Identifier.Text == "DefaultIpStruct");
        var resultIp = StringPrimitiveGenerator.ExtractTypeInfo(semanticModel, defaultIpSyntax, default);
        resultIp!.NotEmpty.Should().BeTrue();

        var defaultMacSyntax = records.First(r => r.Identifier.Text == "DefaultMacStruct");
        var resultMac = StringPrimitiveGenerator.ExtractTypeInfo(semanticModel, defaultMacSyntax, default);
        resultMac!.UpperCase.Should().BeTrue();

        var defaultIbanSyntax = records.First(r => r.Identifier.Text == "DefaultIbanStruct");
        var resultIban = StringPrimitiveGenerator.ExtractTypeInfo(semanticModel, defaultIbanSyntax, default);
        resultIban!.UpperCase.Should().BeTrue();

        var defaultIsbnSyntax = records.First(r => r.Identifier.Text == "DefaultIsbnStruct");
        var resultIsbn = StringPrimitiveGenerator.ExtractTypeInfo(semanticModel, defaultIsbnSyntax, default);
        resultIsbn!.NotEmpty.Should().BeTrue();

        var defaultVinSyntax = records.First(r => r.Identifier.Text == "DefaultVinStruct");
        var resultVin = StringPrimitiveGenerator.ExtractTypeInfo(semanticModel, defaultVinSyntax, default);
        resultVin!.UpperCase.Should().BeTrue();

        var allFlagsSyntax = records.First(r => r.Identifier.Text == "AllFlagsStruct");
        var resultAllFlags = StringPrimitiveGenerator.ExtractTypeInfo(semanticModel, allFlagsSyntax, default);
        resultAllFlags!.TrimStart.Should().BeTrue();
        resultAllFlags.TrimEnd.Should().BeTrue();
        resultAllFlags.LowerCase.Should().BeTrue();
        resultAllFlags.UpperCase.Should().BeTrue();
        resultAllFlags.NormalizeWhitespace.Should().BeTrue();
        resultAllFlags.NotEmpty.Should().BeTrue();

        var assemblyDefaultsSyntax = records.First(r => r.Identifier.Text == "InheritsAssemblyDefaults");
        var resultAssemblyDefaults = StringPrimitiveGenerator.ExtractTypeInfo(semanticModel, assemblyDefaultsSyntax, default);
        resultAssemblyDefaults!.Trim.Should().BeTrue();
        resultAssemblyDefaults.NotEmpty.Should().BeTrue();
        resultAssemblyDefaults.MaxLength.Should().Be(120);

        var deepNestedSyntax = records.First(r => r.Identifier.Text == "DeepNestedStruct");
        var resultDeep = StringPrimitiveGenerator.ExtractTypeInfo(semanticModel, deepNestedSyntax, default);
        resultDeep!.ContainingTypes.Values.ToArray().Should().Equal("OuterLevel1", "OuterLevel2");
        resultDeep.Accessibility.Should().Be("private");

        var deepProtIntSyntax = records.First(r => r.Identifier.Text == "DeepProtIntStruct");
        var resultProtInt = StringPrimitiveGenerator.ExtractTypeInfo(semanticModel, deepProtIntSyntax, default);
        resultProtInt!.Accessibility.Should().Be("protected internal");

        var deepIntSyntax = records.First(r => r.Identifier.Text == "DeepIntStruct");
        var resultInt = StringPrimitiveGenerator.ExtractTypeInfo(semanticModel, deepIntSyntax, default);
        resultInt!.Accessibility.Should().Be("internal");
    }

    [Fact]
    public void NumericPrimitive_EdgeCases_AreCovered()
    {
        var source = @"
using System;
using EricksonLopez.DomainPrimitives;
using EricksonLopez.DomainPrimitives.Normalization;

namespace TestNamespace1 {
    using NumericPrimitiveAttribute = System.ComponentModel.DescriptionAttribute;
    [NumericPrimitive] public record struct FakeAttributeStruct {}
}

namespace TestNamespace {
    [NumericPrimitive<int>] public record NotAStructRecord {}
    [System.ComponentModel.Description] public record struct NoAttributesStruct {}

    [NumericPrimitive<int>(AllowAddition = true, AllowSubtraction = true, AllowScalarMultiplication = true, AllowScalarDivision = true, AllowNegation = true)]
    [Minimum(5)]
    [Maximum(10)]
    public readonly partial record struct MinMaxPrimitive;

    [NumericPrimitive<double>]
    [Range(1.0, 100.0, MinExclusive = true, MaxExclusive = true)]
    public readonly partial record struct RangePrimitive;

    [NumericPrimitive<long>] public readonly partial record struct LongPrimitive;
    [NumericPrimitive<short>] public readonly partial record struct ShortPrimitive;
    [NumericPrimitive<byte>] public readonly partial record struct BytePrimitive;
    [NumericPrimitive<float>] public readonly partial record struct FloatPrimitive;
    [NumericPrimitive<decimal>] public readonly partial record struct DecimalPrimitive;
    [NumericPrimitive<uint>] public readonly partial record struct UIntPrimitive;
    [NumericPrimitive<ulong>] public readonly partial record struct ULongPrimitive;
    [NumericPrimitive<ushort>] public readonly partial record struct UShortPrimitive;
    [NumericPrimitive<sbyte>] public readonly partial record struct SBytePrimitive;

    [Money] public readonly partial record struct MoneyPrim;
    [Percentage] public readonly partial record struct PercPrim;
    [Weight] public readonly partial record struct WeightPrim;
    [Height] public readonly partial record struct HeightPrim;
    [Distance] public readonly partial record struct DistPrim;
    [Temperature] public readonly partial record struct TempPrim;
    [Score] public readonly partial record struct ScorePrim;
    [Quantity] public readonly partial record struct QtyPrim;
    [Price] public readonly partial record struct PricePrim;
    [TaxRate] public readonly partial record struct TaxPrim;
    [Discount] public readonly partial record struct DiscPrim;
    [Rating(Scale = 2)] public readonly partial record struct RatingPrim;

    public class OuterNumeric {
        [NumericPrimitive<int>]
        public readonly partial record struct NestedNumeric;
    }
}

namespace EricksonLopez.DomainPrimitives
{
    public class NumericPrimitiveAttribute<T> : System.Attribute { public bool AllowAddition { get; set; } public bool AllowSubtraction { get; set; } public bool AllowScalarMultiplication { get; set; } public bool AllowScalarDivision { get; set; } public bool AllowNegation { get; set; } }
    public class MoneyAttribute : System.Attribute {}
    public class PercentageAttribute : System.Attribute {}
    public class WeightAttribute : System.Attribute {}
    public class HeightAttribute : System.Attribute {}
    public class DistanceAttribute : System.Attribute {}
    public class TemperatureAttribute : System.Attribute {}
    public class ScoreAttribute : System.Attribute {}
    public class QuantityAttribute : System.Attribute {}
    public class PriceAttribute : System.Attribute {}
    public class TaxRateAttribute : System.Attribute {}
    public class DiscountAttribute : System.Attribute {}
    public class RatingAttribute : System.Attribute { public int Scale { get; set; } }
}
namespace EricksonLopez.DomainPrimitives.Normalization
{
    public class MinimumAttribute : System.Attribute { public MinimumAttribute(object i) {} }
    public class MaximumAttribute : System.Attribute { public MaximumAttribute(object i) {} }
    public class RangeAttribute : System.Attribute { public RangeAttribute(object min, object max) { MinExclusive = false; MaxExclusive = false; } public bool MinExclusive { get; set; } public bool MaxExclusive { get; set; } }
}
";
        RunGenerator(new NumericPrimitiveGenerator(), source);
    }
    
    [Fact]
    public void DatePrimitive_EdgeCases_AreCovered()
    {
        var source = @"
using System;
using EricksonLopez.DomainPrimitives;

[assembly: DomainPrimitivesDefaults(ExceptionType = typeof(ArgumentException))]

namespace TestNamespace1 {
    using DatePrimitiveAttribute = System.ComponentModel.DescriptionAttribute;
    [DatePrimitive] public record struct FakeAttributeStruct {}
}

namespace TestNamespace {
    [DatePrimitive] public record NotAStructRecord {}
    [System.ComponentModel.Description] public record struct NoAttributesStruct {}

    [DatePrimitive(Kind = 0, PastOnly = true)] public readonly partial record struct DateOnlyPast;
    [DatePrimitive(Kind = 1, FutureOnly = true)] public readonly partial record struct DateTimeFuture;
    [DatePrimitive(Kind = 2)] public readonly partial record struct DateTimeOffsetPrim;
    [DatePrimitive(Kind = 3)] public readonly partial record struct TimeOnlyPrim;
    [DatePrimitive(Kind = 99)] public readonly partial record struct DefaultKindPrim;

    [BirthDate(MaxAge = 120)] public readonly partial record struct CustomBirthDate;
    [BirthDate] public readonly partial record struct DefaultBirthDate;
    [ExpirationDate] public readonly partial record struct ExpDate;
    [BusinessDate] public readonly partial record struct BizDate;
    [FiscalYear] public readonly partial record struct FiscYear;
    [Month] public readonly partial record struct MonthPrim;
    [Quarter] public readonly partial record struct QtrPrim;
    [Week] public readonly partial record struct WeekPrim;
    [DateRange] public readonly partial record struct DateRangePrim;
    [TimeRange] public readonly partial record struct TimeRangePrim;

    // Deduplication test (both DatePrimitive and BirthDate on the same type)
    [DatePrimitive] [BirthDate] public readonly partial record struct DupAttrDatePrim;

    public class OuterDate {
        [DatePrimitive]
        public readonly partial record struct NestedDate;
    }
}

namespace EricksonLopez.DomainPrimitives
{
    public class DomainPrimitivesDefaultsAttribute : System.Attribute { public System.Type ExceptionType { get; set; } }
    public class DatePrimitiveAttribute : System.Attribute { public int Kind { get; set; } public bool PastOnly { get; set; } public bool FutureOnly { get; set; } }
    public class BirthDateAttribute : System.Attribute { public int MaxAge { get; set; } }
    public class ExpirationDateAttribute : System.Attribute {}
    public class BusinessDateAttribute : System.Attribute {}
    public class FiscalYearAttribute : System.Attribute {}
    public class MonthAttribute : System.Attribute {}
    public class QuarterAttribute : System.Attribute {}
    public class WeekAttribute : System.Attribute {}
    public class DateRangeAttribute : System.Attribute {}
    public class TimeRangeAttribute : System.Attribute {}
}
";
        var result = RunGenerator(new DatePrimitiveGenerator(), source);
        result.GeneratedTrees.Length.Should().BeGreaterThan(10);
    }
    
    [Fact]
    public void SmartEnum_EdgeCases_AreCovered()
    {
        var source = @"
using System;
using EricksonLopez.DomainPrimitives;

namespace TestNamespace1 {
    using SmartEnumAttribute = System.ComponentModel.DescriptionAttribute;
    [SmartEnum] public record struct FakeAttributeStruct {}
}

namespace TestNamespace {
    [SmartEnum<int>] public record NotAStructRecord {}
    [System.ComponentModel.Description] public record struct NoAttributesStruct {}

    [SmartEnum<int>]
    public readonly partial record struct OrderStatus
    {
        public static readonly OrderStatus Pending = new(1, nameof(Pending));
        public static readonly OrderStatus Shipped = new(2, nameof(Shipped));
    }

    [SmartEnum<string>]
    public sealed partial class Priority
    {
        public static readonly Priority High = new(""H"", nameof(High));
        public static readonly Priority Low = new(""L"", nameof(Low));
    }

    public class OuterEnum {
        [SmartEnum<int>]
        public readonly partial record struct NestedEnum
        {
            public static readonly NestedEnum One = new(1, nameof(One));
        }
    }
}

namespace EricksonLopez.DomainPrimitives
{
    public class SmartEnumAttribute<T> : System.Attribute {}
}
";
        RunGenerator(new SmartEnumGenerator(), source);
    }
    
    [Fact]
    public void StrongId_EdgeCases_AreCovered()
    {
        var source = @"
using System;
using EricksonLopez.DomainPrimitives;

namespace TestNamespace1 {
    using StrongIdAttribute = System.ComponentModel.DescriptionAttribute;
    [StrongId] public record struct FakeAttributeStruct {}
}

namespace TestNamespace {
    [StrongId<int>] public record NotAStructRecord {}
    [System.ComponentModel.Description] public record struct NoAttributesStruct {}

    [StrongId<int>(RejectEmpty = true)]
    public readonly partial record struct IntId;

    [StrongId<long>(RejectEmpty = false)]
    public readonly partial record struct LongId;

    [StrongId<string>(RejectEmpty = true)]
    public readonly partial record struct StringId;

    [StrongId<Guid>(RejectEmpty = true)]
    public readonly partial record struct GuidId;

    [StrongId<short>]
    public readonly partial record struct ShortId;

    public class OuterId {
        [StrongId<Guid>]
        public readonly partial record struct NestedId;
    }
}

namespace EricksonLopez.DomainPrimitives
{
    public class StrongIdAttribute<TValue> : System.Attribute { public bool RejectEmpty { get; set; } }
}
";
        RunGenerator(new StrongIdGenerator(), source);
    }
    
    [Fact]
    public void ValueObject_EdgeCases_AreCovered()
    {
        var source = @"
using System;
using EricksonLopez.DomainPrimitives;

namespace TestNamespace1 {
    using ValueObjectAttribute = System.ComponentModel.DescriptionAttribute;
    [ValueObject] public record struct FakeAttributeStruct {}
}

namespace TestNamespace {
    [ValueObject] public record NotAStructRecord {}
    [System.ComponentModel.Description] public record struct NoAttributesStruct {}

    [ValueObject]
    public readonly partial record struct Address(string Street, string City, int ZipCode);

    [ValueObject]
    public readonly partial record struct Coordinate
    {
        public double Latitude { get; init; }
        public double Longitude { get; init; }
    }

    public class OuterVo {
        [ValueObject]
        public readonly partial record struct NestedVo(string Code);
    }
}

namespace EricksonLopez.DomainPrimitives
{
    public class ValueObjectAttribute : System.Attribute {}
}
";
        RunGenerator(new ValueObjectGenerator(), source);
    }

    [Fact]
    public void AllGenerators_RecordStructs_And_FileScopedAccess_AreCovered()
    {
        var source = @"
using System;
using EricksonLopez.DomainPrimitives;

namespace TestNamespace
{
    [DatePrimitive]
    file readonly partial record struct FileDateRecordStruct;

    [NumericPrimitive<int>]
    file readonly partial record struct FileNumericRecordStruct;

    [SmartEnum<int>]
    file readonly partial record struct FileSmartEnumRecordStruct;

    [StrongId<Guid>]
    file readonly partial record struct FileStrongIdRecordStruct;

    [ValueObject]
    file readonly partial record struct FileVoRecordStruct(string X);

    [DatePrimitive]
    public readonly partial record struct PublicDateRecordStruct;

    [NumericPrimitive<int>]
    public readonly partial record struct PublicNumericRecordStruct;

    [SmartEnum<int>]
    public readonly partial record struct PublicSmartEnumRecordStruct;

    [StrongId<Guid>]
    public readonly partial record struct PublicStrongIdRecordStruct;

    [ValueObject]
    public readonly partial record struct PublicVoRecordStruct(string A, int B);

    public record struct PlainRecordStructWithoutAttributes;
}

namespace EricksonLopez.DomainPrimitives
{
    public class DatePrimitiveAttribute : System.Attribute { public int Kind { get; set; } }
    public class NumericPrimitiveAttribute<T> : System.Attribute {}
    public class SmartEnumAttribute<T> : System.Attribute {}
    public class StrongIdAttribute<T> : System.Attribute { public bool RejectEmpty { get; set; } }
    public class ValueObjectAttribute : System.Attribute {}
}
";
        RunGenerator(new DatePrimitiveGenerator(), source);
        RunGenerator(new NumericPrimitiveGenerator(), source);
        RunGenerator(new SmartEnumGenerator(), source);
        RunGenerator(new StrongIdGenerator(), source);
        RunGenerator(new ValueObjectGenerator(), source);
    }

    [Fact]
    public void AllGenerators_WithEmptyAndMalformedSource_ShouldNotThrowOrCrash()
    {
        var emptySource = "";
        var invalidSource = @"
namespace BrokenNamespace {
    public class IncompleteClass {
";

        var generators = new IIncrementalGenerator[]
        {
            new StringPrimitiveGenerator(),
            new NumericPrimitiveGenerator(),
            new DatePrimitiveGenerator(),
            new SmartEnumGenerator(),
            new StrongIdGenerator(),
            new ValueObjectGenerator()
        };

        foreach (var gen in generators)
        {
            var resEmpty = RunGenerator(gen, emptySource);
            resEmpty.GeneratedTrees.Should().BeEmpty();

            var resInvalid = RunGenerator(gen, invalidSource);
            resInvalid.Diagnostics.Should().BeEmpty();
        }
    }
}


