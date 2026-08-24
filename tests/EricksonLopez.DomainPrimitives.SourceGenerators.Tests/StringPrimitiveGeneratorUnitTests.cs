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

public class StringPrimitiveGeneratorUnitTests
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
    public void ExtractTypeInfo_NullWhenNotDomainPrimitive()
    {
        var source = @"
namespace TestNamespace
{
    public record struct PlainStruct;
}";
        var model = CreateSemanticModel(source);
        var syntax = model.SyntaxTree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().First();
        var info = StringPrimitiveGenerator.ExtractTypeInfo(model, syntax, CancellationToken.None);
        info.Should().BeNull();
    }

    [Fact]
    public void ExtractTypeInfo_ForeignNamespaceAttributes_AreIgnored()
    {
        var source = @"
namespace ForeignNamespace
{
    public class EmailAttribute : System.Attribute {}
    public class TrimAttribute : System.Attribute {}
    public class CustomValidatorAttribute<T> : System.Attribute {}
}

namespace TestNamespace
{
    [ForeignNamespace.Email]
    [ForeignNamespace.Trim]
    [ForeignNamespace.CustomValidator<string>]
    public readonly partial record struct ForeignStruct;
}";
        var model = CreateSemanticModel(source);
        var syntax = model.SyntaxTree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().First();
        var info = StringPrimitiveGenerator.ExtractTypeInfo(model, syntax, CancellationToken.None);
        info.Should().BeNull();
    }

    [Fact]
    public void ExtractTypeInfo_ForeignTrimOnValidPrimitive_IsIgnored()
    {
        var source = @"
namespace ForeignNamespace
{
    public class TrimAttribute : System.Attribute {}
    public class CustomValidatorAttribute<T> : System.Attribute {}
}
namespace EricksonLopez.DomainPrimitives
{
    public class StringPrimitiveAttribute : System.Attribute {}
}

namespace TestNamespace
{
    [EricksonLopez.DomainPrimitives.StringPrimitive]
    [ForeignNamespace.Trim]
    [ForeignNamespace.CustomValidator<string>]
    public readonly partial record struct PrimitiveWithForeignTrim;
}";
        var model = CreateSemanticModel(source);
        var syntax = model.SyntaxTree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().First();
        var info = StringPrimitiveGenerator.ExtractTypeInfo(model, syntax, CancellationToken.None);
        info.Should().NotBeNull();
        info!.Trim.Should().BeFalse();
        info.HasCustomValidator.Should().BeFalse();
    }

    [Fact]
    public void ExtractTypeInfo_CustomValidator_Recognized()
    {
        var source = @"
namespace EricksonLopez.DomainPrimitives
{
    public class StringPrimitiveAttribute : System.Attribute {}
}
namespace EricksonLopez.DomainPrimitives.Validation
{
    public class CustomValidatorAttribute<T> : System.Attribute {}
}

namespace TestNamespace
{
    [EricksonLopez.DomainPrimitives.StringPrimitive]
    [EricksonLopez.DomainPrimitives.Validation.CustomValidator<object>]
    public readonly partial record struct PrimitiveWithValidator;
}";
        var model = CreateSemanticModel(source);
        var syntax = model.SyntaxTree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().First();
        var info = StringPrimitiveGenerator.ExtractTypeInfo(model, syntax, CancellationToken.None);
        info.Should().NotBeNull();
        info!.HasCustomValidator.Should().BeTrue();
    }

    [Fact]
    public void ExtractTypeInfo_IndividualShortcutsAndFlags_AreCorrect()
    {
        var source = @"
using EricksonLopez.DomainPrimitives;
using EricksonLopez.DomainPrimitives.Normalization;

namespace TestNamespace
{
    [Email] public readonly partial record struct EmailPrim;
    [Phone] public readonly partial record struct PhonePrim;
    [Url] public readonly partial record struct UrlPrim;
    [Slug] public readonly partial record struct SlugPrim;
    [CountryCode] public readonly partial record struct CountryPrim;
    [LanguageCode] public readonly partial record struct LangPrim;
    [CurrencyCode] public readonly partial record struct CurrPrim;
    [Username] public readonly partial record struct UserPrim;
    [PasswordHash] public readonly partial record struct PwdPrim;
    [HexColor] public readonly partial record struct HexPrim;
    [IPAddress] public readonly partial record struct IpPrim;
    [MacAddress] public readonly partial record struct MacPrim;
    [IBAN] public readonly partial record struct IbanPrim;
    [ISBN] public readonly partial record struct IsbnPrim;
    [VIN] public readonly partial record struct VinPrim;

    [StringPrimitive] [TrimStart] public readonly partial record struct TrimStartPrim;
    [StringPrimitive] [TrimEnd] public readonly partial record struct TrimEndPrim;
    [StringPrimitive] [NormalizeWhitespace] public readonly partial record struct NormWsPrim;
    [StringPrimitive] [NotEmpty] public readonly partial record struct NotEmptyPrim;
    [StringPrimitive] [Regex(@""^[0-9]+$"", ""Positional error message"")] public readonly partial record struct PosRegexPrim;
    [StringPrimitive] [Regex(@""^[a-z]+$"", ErrorMessage = ""Named error message"")] public readonly partial record struct NamedRegexPrim;
    [StringPrimitive] [MinLength] [MaxLength] [ExactLength] [Regex] public readonly partial record struct ZeroArgsPrim;
}

namespace EricksonLopez.DomainPrimitives
{
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
    public class TrimStartAttribute : System.Attribute {}
    public class TrimEndAttribute : System.Attribute {}
    public class NormalizeWhitespaceAttribute : System.Attribute {}
    public class NotEmptyAttribute : System.Attribute {}
    public class MinLengthAttribute : System.Attribute { public MinLengthAttribute() {} public MinLengthAttribute(int l) {} }
    public class MaxLengthAttribute : System.Attribute { public MaxLengthAttribute() {} public MaxLengthAttribute(int l) {} }
    public class ExactLengthAttribute : System.Attribute { public ExactLengthAttribute() {} public ExactLengthAttribute(int l) {} }
    public class RegexAttribute : System.Attribute { public RegexAttribute() {} public RegexAttribute(string p, string e = null) {} public string ErrorMessage { get; set; } }
}";
        var model = CreateSemanticModel(source);
        var records = model.SyntaxTree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().ToList();

        var email = StringPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "EmailPrim"), CancellationToken.None);
        email!.DomainShortcut.Should().Be("Email");
        email.MaxLength.Should().Be(320);

        var phone = StringPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "PhonePrim"), CancellationToken.None);
        phone!.DomainShortcut.Should().Be("Phone");

        var url = StringPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "UrlPrim"), CancellationToken.None);
        url!.DomainShortcut.Should().Be("Url");
        url.AllowedSchemes.Values.Should().Equal("https", "http");

        var slug = StringPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "SlugPrim"), CancellationToken.None);
        slug!.DomainShortcut.Should().Be("Slug");
        slug.MaxLength.Should().Be(200);

        var country = StringPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "CountryPrim"), CancellationToken.None);
        country!.DomainShortcut.Should().Be("CountryCode");
        country.MinLength.Should().Be(2);
        country.MaxLength.Should().Be(2);

        var lang = StringPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "LangPrim"), CancellationToken.None);
        lang!.DomainShortcut.Should().Be("LanguageCode");
        lang.MinLength.Should().Be(2);
        lang.MaxLength.Should().Be(2);

        var curr = StringPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "CurrPrim"), CancellationToken.None);
        curr!.DomainShortcut.Should().Be("CurrencyCode");
        curr.MinLength.Should().Be(3);
        curr.MaxLength.Should().Be(3);

        var user = StringPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "UserPrim"), CancellationToken.None);
        user!.DomainShortcut.Should().Be("Username");
        user.MinLength.Should().Be(3);
        user.MaxLength.Should().Be(50);

        var pwd = StringPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "PwdPrim"), CancellationToken.None);
        pwd!.DomainShortcut.Should().Be("PasswordHash");

        var hex = StringPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "HexPrim"), CancellationToken.None);
        hex!.DomainShortcut.Should().Be("HexColor");

        var ip = StringPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "IpPrim"), CancellationToken.None);
        ip!.DomainShortcut.Should().Be("IPAddress");

        var mac = StringPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "MacPrim"), CancellationToken.None);
        mac!.DomainShortcut.Should().Be("MacAddress");

        var iban = StringPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "IbanPrim"), CancellationToken.None);
        iban!.DomainShortcut.Should().Be("IBAN");

        var isbn = StringPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "IsbnPrim"), CancellationToken.None);
        isbn!.DomainShortcut.Should().Be("ISBN");

        var vin = StringPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "VinPrim"), CancellationToken.None);
        vin!.DomainShortcut.Should().Be("VIN");

        var trimStart = StringPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "TrimStartPrim"), CancellationToken.None);
        trimStart!.TrimStart.Should().BeTrue();

        var trimEnd = StringPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "TrimEndPrim"), CancellationToken.None);
        trimEnd!.TrimEnd.Should().BeTrue();

        var normWs = StringPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "NormWsPrim"), CancellationToken.None);
        normWs!.NormalizeWhitespace.Should().BeTrue();

        var notEmpty = StringPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "NotEmptyPrim"), CancellationToken.None);
        notEmpty!.NotEmpty.Should().BeTrue();

        var posRegex = StringPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "PosRegexPrim"), CancellationToken.None);
        posRegex!.RegexPatterns.Length.Should().Be(1);
        posRegex.RegexPatterns[0].Pattern.Should().Be("^[0-9]+$");
        posRegex.RegexPatterns[0].ErrorMessage.Should().Be("Positional error message");

        var namedRegex = StringPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "NamedRegexPrim"), CancellationToken.None);
        namedRegex!.RegexPatterns.Length.Should().Be(1);
        namedRegex.RegexPatterns[0].Pattern.Should().Be("^[a-z]+$");
        namedRegex.RegexPatterns[0].ErrorMessage.Should().Be("Named error message");

        var zeroArgs = StringPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "ZeroArgsPrim"), CancellationToken.None);
        zeroArgs!.MinLength.Should().BeNull();
        zeroArgs.MaxLength.Should().BeNull();
        zeroArgs.ExactLength.Should().BeNull();
        zeroArgs.RegexPatterns.Length.Should().Be(0);
    }

    [Fact]
    public void ExtractTypeInfo_AssemblyDefaults_InheritedAndOverridden()
    {
        var source = @"
using System;
using EricksonLopez.DomainPrimitives;
using EricksonLopez.DomainPrimitives.Normalization;

[assembly: DomainPrimitivesDefaults(Trim = true, NotEmpty = true, MaxLength = 80, ExceptionType = typeof(InvalidOperationException))]

namespace TestNamespace
{
    [StringPrimitive]
    public readonly partial record struct InheritedPrim;

    [StringPrimitive]
    [ExactLength(10)]
    public readonly partial record struct ExactOverridePrim;

    [StringPrimitive]
    [MaxLength(40)]
    public readonly partial record struct MaxOverridePrim;
}

namespace EricksonLopez.DomainPrimitives
{
    public class DomainPrimitivesDefaultsAttribute : System.Attribute { public bool Trim { get; set; } public bool NotEmpty { get; set; } public int MaxLength { get; set; } public Type ExceptionType { get; set; } }
    public class StringPrimitiveAttribute : System.Attribute {}
}
namespace EricksonLopez.DomainPrimitives.Normalization
{
    public class ExactLengthAttribute : System.Attribute { public ExactLengthAttribute(int l) {} }
    public class MaxLengthAttribute : System.Attribute { public MaxLengthAttribute(int l) {} }
}";
        var model = CreateSemanticModel(source);
        var records = model.SyntaxTree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().ToList();

        var inherited = StringPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "InheritedPrim"), CancellationToken.None);
        inherited!.Trim.Should().BeTrue();
        inherited.NotEmpty.Should().BeTrue();
        inherited.MaxLength.Should().Be(80);
        inherited.CustomExceptionType.Should().Contain("InvalidOperationException");

        var exact = StringPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "ExactOverridePrim"), CancellationToken.None);
        exact!.ExactLength.Should().Be(10);
        exact.MaxLength.Should().Be(10);

        var max = StringPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "MaxOverridePrim"), CancellationToken.None);
        max!.MaxLength.Should().Be(40);
        max.ExactLength.Should().BeNull();
    }

    [Fact]
    public void ExtractTypeInfo_AllDomainShortcuts_WithExplicitOverrides()
    {
        var source = @"
using EricksonLopez.DomainPrimitives;
using EricksonLopez.DomainPrimitives.Normalization;

namespace TestNamespace
{
    [Email(MaxLength = 90)]
    [MaxLength(75)]
    public readonly partial record struct EmailOverride;

    [Slug(MaxLength = 120)]
    [MaxLength(60)]
    public readonly partial record struct SlugOverride;

    [CountryCode]
    [MinLength(4)]
    [MaxLength(4)]
    public readonly partial record struct CountryOverride;

    [LanguageCode]
    [MinLength(3)]
    [MaxLength(3)]
    public readonly partial record struct LangOverride;

    [CurrencyCode]
    [MinLength(4)]
    [MaxLength(4)]
    public readonly partial record struct CurrencyOverride;

    [Username(MinLength = 6, MaxLength = 30)]
    [MinLength(8)]
    [MaxLength(20)]
    public readonly partial record struct UserOverride;

    [Url(AllowedSchemes = new[] { ""ftp"", ""sftp"" })]
    public readonly partial record struct CustomUrl;

    [Url(OtherArray = new string[] { ""custom_scheme"" })]
    public readonly partial record struct UrlWithOtherArray;
}

namespace EricksonLopez.DomainPrimitives
{
    public class EmailAttribute : System.Attribute { public int MaxLength { get; set; } }
    public class SlugAttribute : System.Attribute { public int MaxLength { get; set; } }
    public class CountryCodeAttribute : System.Attribute {}
    public class LanguageCodeAttribute : System.Attribute {}
    public class CurrencyCodeAttribute : System.Attribute {}
    public class UsernameAttribute : System.Attribute { public int MinLength { get; set; } public int MaxLength { get; set; } }
    public class UrlAttribute : System.Attribute { public string[] AllowedSchemes { get; set; } public string[] OtherArray { get; set; } }
}
namespace EricksonLopez.DomainPrimitives.Normalization
{
    public class MinLengthAttribute : System.Attribute { public MinLengthAttribute(int l) {} }
    public class MaxLengthAttribute : System.Attribute { public MaxLengthAttribute(int l) {} }
}";
        var model = CreateSemanticModel(source);
        var records = model.SyntaxTree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().ToList();

        var emailInfo = StringPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "EmailOverride"), CancellationToken.None);
        emailInfo!.MaxLength.Should().Be(75);
        emailInfo.LowerCase.Should().BeTrue();
        emailInfo.Trim.Should().BeTrue();

        var slugInfo = StringPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "SlugOverride"), CancellationToken.None);
        slugInfo!.MaxLength.Should().Be(60);

        var countryInfo = StringPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "CountryOverride"), CancellationToken.None);
        countryInfo!.MinLength.Should().Be(4);
        countryInfo.MaxLength.Should().Be(4);
        countryInfo.UpperCase.Should().BeTrue();

        var langInfo = StringPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "LangOverride"), CancellationToken.None);
        langInfo!.MinLength.Should().Be(3);
        langInfo.MaxLength.Should().Be(3);

        var currInfo = StringPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "CurrencyOverride"), CancellationToken.None);
        currInfo!.MinLength.Should().Be(4);
        currInfo.MaxLength.Should().Be(4);

        var userInfo = StringPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "UserOverride"), CancellationToken.None);
        userInfo!.MinLength.Should().Be(8);
        userInfo.MaxLength.Should().Be(20);

        var urlInfo = StringPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "CustomUrl"), CancellationToken.None);
        urlInfo!.AllowedSchemes.Values.Should().Equal("ftp", "sftp");

        var urlOtherArrayInfo = StringPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "UrlWithOtherArray"), CancellationToken.None);
        urlOtherArrayInfo!.AllowedSchemes.Values.Should().Equal("https", "http");
    }

    [Fact]
    public void GenerateStringPrimitive_EscapingAndFormatting_Covered()
    {
        var regexes = ImmutableArray.Create(
            new RegexInfo(@"^\""hello\""\\test$", @"Must match \""hello\"" with \\ test"),
            new RegexInfo(@"^[0-9]+$", null)
        );

        var info = new StringPrimitiveTypeInfo(
            Namespace: "TestNamespace",
            TypeName: "EscapedPrimitive",
            Accessibility: "public",
            ContainingTypes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            Trim: true,
            TrimStart: true,
            TrimEnd: true,
            LowerCase: false,
            UpperCase: false,
            NormalizeWhitespace: true,
            MinLength: null,
            MaxLength: null,
            ExactLength: 10,
            NotEmpty: true,
            RegexPatterns: new EquatableArray<RegexInfo>(regexes),
            DomainShortcut: "Url",
            HasCustomValidator: true,
            AllowedSchemes: new EquatableArray<string>(ImmutableArray.Create("https", "http")),
            CustomExceptionType: "global::System.ArgumentException");

        var code = StringPrimitiveGenerator.GenerateStringPrimitive(info);

        code.Should().Contain("public readonly partial record struct EscapedPrimitive :");
        code.Should().Contain("throw new global::System.ArgumentException(error.Message);");
        code.Should().Contain("private static readonly System.Text.RegularExpressions.Regex ValidationRegex1 =");
        code.Should().Contain("private static readonly System.Text.RegularExpressions.Regex ValidationRegex2 =");
        code.Should().Contain("private static readonly System.Text.RegularExpressions.Regex WhitespaceRegex =");
        code.Should().Contain("@\"^\\\"\"hello\\\"\"\\\\test$\"");
        code.Should().Contain("value = value.TrimStart();");
        code.Should().Contain("value = value.TrimEnd();");
        code.Should().Contain("if (value.Length != 10)");
        code.Should().Contain("Must match \\\\\\\"hello\\\\\\\" with \\\\\\\\ test");
        code.Should().Contain("EscapedPrimitive has an invalid format.");
        code.Should().Contain("/// <summary>Validates a pre-NFC-normalized span. Caller must ensure the span is already in NFC form.</summary>");
        code.Should().Contain("get");
        code.Should().Contain("if (_value is null) throw new InvalidOperationException($\"Value accessed on a default instance of EscapedPrimitive. Check IsDefault before accessing Value.\");");
        code.Should().Contain("return _value;");
        code.Should().Contain("var trimmed = s.ToString().Trim();");
        code.Should().Contain("finally");
        code.Should().Contain("System.Buffers.ArrayPool<char>.Shared.Return(rented);");

        var normalizedCode = code.Replace("\r\n", "\n");
        // Validate indentation and DecreaseIndent in TryValidate
        normalizedCode.Should().Contain("        if (value.Length != 10)\n            return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"LENGTH\", $\"EscapedPrimitive must be exactly 10 character(s). Got {value.Length}.\");\n        if (!Uri.TryCreate");
        
        // Validate indentation and DecreaseIndent in TryValidateSpan
        normalizedCode.Should().Contain("        if (value.Length != 10)\n            return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"LENGTH\", $\"EscapedPrimitive must be exactly 10 character(s). Got {value.Length}.\");\n        if (!Uri.TryCreate(value.ToString()");
        
        // Validate regex fallback message in TryValidateSpan
        normalizedCode.Should().Contain("        if (!ValidationRegex2.IsMatch(value))\n            return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"FORMAT\", \"EscapedPrimitive has an invalid format.\");");
        
        // Validate closing of TryValidateSpan
        normalizedCode.Should().Contain("        return global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError.None;\n    }\n");
    }

    [Fact]
    public void GenerateSpanValidation_ExactLength_IndentationAndRegexFallback_Covered()
    {
        var regexes = ImmutableArray.Create(
            new RegexInfo(@"^[0-9]+$", null)
        );

        var info = new StringPrimitiveTypeInfo(
            Namespace: "TestNamespace",
            TypeName: "SpanExactPrim",
            Accessibility: "public",
            ContainingTypes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            Trim: false,
            TrimStart: false,
            TrimEnd: false,
            LowerCase: false,
            UpperCase: false,
            NormalizeWhitespace: false,
            MinLength: null,
            MaxLength: null,
            ExactLength: 7,
            NotEmpty: false,
            RegexPatterns: new EquatableArray<RegexInfo>(regexes),
            DomainShortcut: null,
            HasCustomValidator: false,
            AllowedSchemes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            CustomExceptionType: null);

        var code = StringPrimitiveGenerator.GenerateStringPrimitive(info).Replace("\r\n", "\n");

        var expectedSpanValidation =
            "    /// <summary>Validates a pre-NFC-normalized span. Caller must ensure the span is already in NFC form.</summary>\n" +
            "    private static global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError TryValidateSpan(ReadOnlySpan<char> value)\n" +
            "    {\n" +
            "        if (value.Length != 7)\n" +
            "            return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"LENGTH\", $\"SpanExactPrim must be exactly 7 character(s). Got {value.Length}.\");\n" +
            "        if (!ValidationRegex.IsMatch(value))\n" +
            "            return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"FORMAT\", \"SpanExactPrim has an invalid format.\");\n" +
            "        return global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError.None;\n" +
            "    }\n";

        code.Should().Contain(expectedSpanValidation);
    }

    [Fact]
    public void GenerateStringPrimitive_DefaultRegexError_EmittedWhenNull()
    {
        var regexes = ImmutableArray.Create(
            new RegexInfo(@"^[0-9]+$", null)
        );

        var info = new StringPrimitiveTypeInfo(
            Namespace: "TestNamespace",
            TypeName: "DefaultErrorPrimitive",
            Accessibility: "internal",
            ContainingTypes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            Trim: false,
            TrimStart: false,
            TrimEnd: false,
            LowerCase: true,
            UpperCase: false,
            NormalizeWhitespace: false,
            MinLength: 5,
            MaxLength: 20,
            ExactLength: null,
            NotEmpty: true,
            RegexPatterns: new EquatableArray<RegexInfo>(regexes),
            DomainShortcut: null,
            HasCustomValidator: false,
            AllowedSchemes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            CustomExceptionType: null);

        var code = StringPrimitiveGenerator.GenerateStringPrimitive(info);

        code.Should().Contain("throw new DomainPrimitiveValidationException(error);");
        code.Should().Contain("private static readonly System.Text.RegularExpressions.Regex ValidationRegex =");
        code.Should().Contain("if (value.Length < 5)");
        code.Should().Contain("if (value.Length > 20)");
        code.Should().Contain("internal readonly partial record struct DefaultErrorPrimitive :");

        var defaultNormalized = code.Replace("\r\n", "\n");
        defaultNormalized.Should().Contain("    private static global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError TryValidate(string value)\n    {\n        if (string.IsNullOrWhiteSpace(value))\n            return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"EMPTY\", \"DefaultErrorPrimitive must not be empty.\");\n        if (value.Length < 5)\n            return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"LENGTH\", $\"DefaultErrorPrimitive must be at least 5 character(s). Got {value.Length}.\");\n        if (value.Length > 20)\n            return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"LENGTH\", $\"DefaultErrorPrimitive must be at most 20 character(s). Got {value.Length}.\");\n        if (!ValidationRegex.IsMatch(value))\n            return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"FORMAT\", \"DefaultErrorPrimitive has an invalid format.\");");
        defaultNormalized.Should().Contain("        if (!ValidationRegex.IsMatch(value))\n            return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"FORMAT\", \"DefaultErrorPrimitive has an invalid format.\");");
    }

    [Fact]
    public void GenerateStringPrimitive_AllNormalizationOptions_Generated()
    {
        var info = new StringPrimitiveTypeInfo(
            Namespace: "TestNamespace",
            TypeName: "AllNormPrimitive",
            Accessibility: "public",
            ContainingTypes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            Trim: true,
            TrimStart: true,
            TrimEnd: true,
            LowerCase: true,
            UpperCase: true,
            NormalizeWhitespace: true,
            MinLength: null,
            MaxLength: null,
            ExactLength: null,
            NotEmpty: false,
            RegexPatterns: new EquatableArray<RegexInfo>(ImmutableArray<RegexInfo>.Empty),
            DomainShortcut: null,
            HasCustomValidator: false,
            AllowedSchemes: new EquatableArray<string>(ImmutableArray<string>.Empty),
            CustomExceptionType: null);

        var code = StringPrimitiveGenerator.GenerateStringPrimitive(info);

        code.Should().Contain("value = value.Trim();");
        code.Should().Contain("value = value.TrimStart();");
        code.Should().Contain("value = value.TrimEnd();");
        code.Should().Contain("value = value.ToLowerInvariant();");
        code.Should().Contain("value = value.ToUpperInvariant();");
        code.Should().Contain("value = WhitespaceRegex.Replace(value, \" \");");
        code.Should().Contain("if (value.Length > 4096)");
    }

    [Fact]
    public void ExtractTypeInfo_PlainPrimitive_WithoutShortcuts_HasDefaults()
    {
        var source = @"
using EricksonLopez.DomainPrimitives;

namespace TestNamespace
{
    [StringPrimitive]
    public readonly partial record struct PlainPrim;
}

namespace EricksonLopez.DomainPrimitives
{
    public class StringPrimitiveAttribute : System.Attribute {}
}";
        var model = CreateSemanticModel(source);
        var syntax = model.SyntaxTree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().First();
        var info = StringPrimitiveGenerator.ExtractTypeInfo(model, syntax, CancellationToken.None);

        info.Should().NotBeNull();
        info!.DomainShortcut.Should().BeNull();
        info.Trim.Should().BeFalse();
        info.LowerCase.Should().BeFalse();
        info.UpperCase.Should().BeFalse();
        info.NotEmpty.Should().BeFalse();
        info.MinLength.Should().BeNull();
        info.MaxLength.Should().BeNull();
        info.ExactLength.Should().BeNull();
        info.RegexPatterns.Length.Should().Be(0);
    }

    [Fact]
    public void ExtractTypeInfo_InvalidAttributeArguments_AreGracefullyIgnored()
    {
        var source = @"
using EricksonLopez.DomainPrimitives;
using EricksonLopez.DomainPrimitives.Normalization;

namespace TestNamespace
{
    [StringPrimitive]
    [MinLength(0)]
    [MaxLength(-5)]
    [ExactLength(0)]
    [Regex(null)]
    [Regex(@""^[0-9]+$"", null)]
    [CustomWithoutSuffix]
    public readonly partial record struct InvalidCtorArgsPrim;

    [Email(MaxLength = 100)]
    [Slug(MaxLength = 80)]
    [Username(MinLength = 4, MaxLength = 16)]
    public readonly partial record struct ShortcutArgsPrim;
}

namespace EricksonLopez.DomainPrimitives
{
    public class StringPrimitiveAttribute : System.Attribute {}
    public class CustomWithoutSuffix : System.Attribute {}
    public class EmailAttribute : System.Attribute { public int MaxLength { get; set; } }
    public class SlugAttribute : System.Attribute { public int MaxLength { get; set; } }
    public class UsernameAttribute : System.Attribute { public int MinLength { get; set; } public int MaxLength { get; set; } }
}
namespace EricksonLopez.DomainPrimitives.Normalization
{
    public class MinLengthAttribute : System.Attribute { public MinLengthAttribute(int l) {} }
    public class MaxLengthAttribute : System.Attribute { public MaxLengthAttribute(int l) {} }
    public class ExactLengthAttribute : System.Attribute { public ExactLengthAttribute(int l) {} }
    public class RegexAttribute : System.Attribute { public RegexAttribute(string p, string e = null) {} }
}";
        var model = CreateSemanticModel(source);
        var records = model.SyntaxTree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().ToList();
        
        var ctorInfo = StringPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "InvalidCtorArgsPrim"), CancellationToken.None);
        ctorInfo.Should().NotBeNull();
        ctorInfo!.MinLength.Should().Be(0);
        ctorInfo.MaxLength.Should().Be(0);
        ctorInfo.ExactLength.Should().Be(0);
        ctorInfo.RegexPatterns.Length.Should().Be(1);
        ctorInfo.RegexPatterns[0].Pattern.Should().Be("^[0-9]+$");
        ctorInfo.RegexPatterns[0].ErrorMessage.Should().BeNull();

        var shortcutInfo = StringPrimitiveGenerator.ExtractTypeInfo(model, records.First(r => r.Identifier.Text == "ShortcutArgsPrim"), CancellationToken.None);
        shortcutInfo.Should().NotBeNull();
        shortcutInfo!.DomainShortcut.Should().Be("Username");
        shortcutInfo.MinLength.Should().Be(4);
        shortcutInfo.MaxLength.Should().Be(16);
    }
}


