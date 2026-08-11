using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VerifyXunit;
using Xunit;
using EricksonLopez.DomainPrimitives.Generators;

namespace EricksonLopez.DomainPrimitives.Generators.Tests
{
    [UsesVerify]
    public class StringPrimitiveGeneratorSnapshotTests
    {
        [Fact]
        public Task GeneratesStringPrimitiveCorrectly()
        {
            string source = @"
using EricksonLopez.DomainPrimitives;

using EricksonLopez.DomainPrimitives.Normalization;

namespace TestNamespace
{
    [StringPrimitive]
    [MinLength(3)]
    [MaxLength(10)]
    [LowerCase]
    [Trim]
    public readonly partial record struct Username;
}

namespace EricksonLopez.DomainPrimitives
{
    public class StringPrimitiveAttribute : System.Attribute {}
}
namespace EricksonLopez.DomainPrimitives.Validation
{
    public class MinLengthAttribute : System.Attribute { public MinLengthAttribute(int len) {} }
    public class MaxLengthAttribute : System.Attribute { public MaxLengthAttribute(int len) {} }
}
namespace EricksonLopez.DomainPrimitives.Normalization
{
    public class LowerCaseAttribute : System.Attribute {}
    public class TrimAttribute : System.Attribute {}
}
";

            var syntaxTree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.CSharp11));
            var compilation = CSharpCompilation.Create(
                assemblyName: "Tests",
                syntaxTrees: new[] { syntaxTree },
                references: Basic.Reference.Assemblies.Net80.References.All,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var generator = new StringPrimitiveGenerator();
            var driver = (GeneratorDriver)CSharpGeneratorDriver.Create(generator);
            driver = driver.RunGenerators(compilation);
            var runResult = driver.GetRunResult();
            var diagnostics = runResult.Diagnostics;
            Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
            Assert.NotEmpty(runResult.GeneratedTrees);

            return Verifier.Verify(driver).UseDirectory("Snapshots");
        }
        [Fact]
        public Task GeneratesEmailAndUrlCorrectly()
        {
            string source = @"
using EricksonLopez.DomainPrimitives.Validation;
using EricksonLopez.DomainPrimitives;

namespace TestNamespace
{
    [Email]
    public readonly partial record struct EmailAddress;

    [Url]
    public readonly partial record struct WebsiteUrl;

    [Phone] public readonly partial record struct PhoneNumber;
    [Slug] public readonly partial record struct ArticleSlug;
    [CountryCode] public readonly partial record struct CtryCode;
    [LanguageCode] public readonly partial record struct LangCode;
    [CurrencyCode] public readonly partial record struct CurrCode;
    [Username] public readonly partial record struct UserName;
    [PasswordHash] public readonly partial record struct PwdHash;
    [HexColor] public readonly partial record struct ThemeColor;
    [IPAddress] public readonly partial record struct ClientIP;
    [MacAddress] public readonly partial record struct DeviceMac;
    [IBAN] public readonly partial record struct BankIBAN;
    [ISBN] public readonly partial record struct BookISBN;
    [VIN] public readonly partial record struct VehicleVIN;
}

namespace EricksonLopez.DomainPrimitives
{
    public class EmailAttribute : StringPrimitiveAttribute {}
    public class UrlAttribute : StringPrimitiveAttribute {}
    public class PhoneAttribute : StringPrimitiveAttribute {}
    public class SlugAttribute : StringPrimitiveAttribute {}
    public class CountryCodeAttribute : StringPrimitiveAttribute {}
    public class LanguageCodeAttribute : StringPrimitiveAttribute {}
    public class CurrencyCodeAttribute : StringPrimitiveAttribute {}
    public class UsernameAttribute : StringPrimitiveAttribute {}
    public class PasswordHashAttribute : StringPrimitiveAttribute {}
    public class HexColorAttribute : StringPrimitiveAttribute {}
    public class IPAddressAttribute : StringPrimitiveAttribute {}
    public class MacAddressAttribute : StringPrimitiveAttribute {}
    public class IBANAttribute : StringPrimitiveAttribute {}
    public class ISBNAttribute : StringPrimitiveAttribute {}
    public class VINAttribute : StringPrimitiveAttribute {}
}
";

            var syntaxTree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.CSharp11));
            var compilation = CSharpCompilation.Create(
                assemblyName: "Tests",
                syntaxTrees: new[] { syntaxTree },
                references: Basic.Reference.Assemblies.Net80.References.All,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var generator = new StringPrimitiveGenerator();
            var driver = (GeneratorDriver)CSharpGeneratorDriver.Create(generator);
            driver = driver.RunGenerators(compilation);
            var runResult = driver.GetRunResult();
            var diagnostics = runResult.Diagnostics;
            Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
            Assert.NotEmpty(runResult.GeneratedTrees);

            return Verifier.Verify(driver).UseDirectory("Snapshots");
        }

        [Fact]
        public Task GeneratesExplicitAttributesCorrectly()
        {
            string source = @"
using EricksonLopez.DomainPrimitives;
using EricksonLopez.DomainPrimitives.Validation;
using EricksonLopez.DomainPrimitives.Normalization;

namespace TestNamespace
{
    [StringPrimitive]
    [UpperCase]
    [NormalizeWhitespace]
    [Length(5, 50)]
    [Regex(""^[A-Z]+$"", ErrorMessage = ""Must be caps"")]
    [CustomValidator<MyValidator>]
    public readonly partial record struct ComplexString;
}

namespace EricksonLopez.DomainPrimitives
{
    public class StringPrimitiveAttribute : System.Attribute {}
}
namespace EricksonLopez.DomainPrimitives.Validation
{
    public class LengthAttribute : System.Attribute { public LengthAttribute(int min, int max) {} }
    public class RegexAttribute : System.Attribute { 
        public RegexAttribute(string pattern) {} 
        public string ErrorMessage { get; set; }
    }
    public class CustomValidatorAttribute<T> : System.Attribute {}
}
namespace EricksonLopez.DomainPrimitives.Normalization
{
    public class UpperCaseAttribute : System.Attribute {}
    public class NormalizeWhitespaceAttribute : System.Attribute {}
}
public class MyValidator {}
";

            var syntaxTree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.CSharp11));
            var compilation = CSharpCompilation.Create(
                assemblyName: "Tests",
                syntaxTrees: new[] { syntaxTree },
                references: Basic.Reference.Assemblies.Net80.References.All,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var generator = new StringPrimitiveGenerator();
            var driver = (GeneratorDriver)CSharpGeneratorDriver.Create(generator);
            driver = driver.RunGenerators(compilation);
            var runResult = driver.GetRunResult();
            var diagnostics = runResult.Diagnostics;
            Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
            Assert.NotEmpty(runResult.GeneratedTrees);

            return Verifier.Verify(driver).UseDirectory("Snapshots");
        }

        [Fact]
        public Task GeneratesDomainShortcutOverridesCorrectly()
        {
            string source = @"
using EricksonLopez.DomainPrimitives;

namespace TestNamespace
{
    [Email(MaxLength = 100)]
    public readonly partial record struct CustomEmail;

    [Slug(MaxLength = 50)]
    public readonly partial record struct CustomSlug;

    [Username(MinLength = 5, MaxLength = 20)]
    public readonly partial record struct CustomUsername;
}

namespace EricksonLopez.DomainPrimitives
{
    public class EmailAttribute : StringPrimitiveAttribute { public int MaxLength { get; set; } }
    public class SlugAttribute : StringPrimitiveAttribute { public int MaxLength { get; set; } }
    public class UsernameAttribute : StringPrimitiveAttribute { public int MinLength { get; set; } public int MaxLength { get; set; } }
}
";
            var syntaxTree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.CSharp11));
            var compilation = CSharpCompilation.Create(
                assemblyName: "Tests",
                syntaxTrees: new[] { syntaxTree },
                references: Basic.Reference.Assemblies.Net80.References.All,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var generator = new StringPrimitiveGenerator();
            var driver = (GeneratorDriver)CSharpGeneratorDriver.Create(generator);
            driver = driver.RunGenerators(compilation);
            var runResult = driver.GetRunResult();
            var diagnostics = runResult.Diagnostics;
            Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
            Assert.NotEmpty(runResult.GeneratedTrees);

            return Verifier.Verify(driver).UseDirectory("Snapshots");
        }
        [Fact]
        public Task GeneratesNestedTypeCorrectly()
        {
            string source = @"
using EricksonLopez.DomainPrimitives;

namespace TestNamespace
{
    public partial class OuterClass
    {
        public partial class InnerClass
        {
            [StringPrimitive]
            public readonly partial record struct NestedPrimitive;
        }
    }
}
namespace EricksonLopez.DomainPrimitives
{
    public class StringPrimitiveAttribute : System.Attribute {}
}
";
            var syntaxTree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.CSharp11));
            var compilation = Microsoft.CodeAnalysis.CSharp.CSharpCompilation.Create("Tests", new[] { syntaxTree }, Basic.Reference.Assemblies.Net80.References.All, new Microsoft.CodeAnalysis.CSharp.CSharpCompilationOptions(Microsoft.CodeAnalysis.OutputKind.DynamicallyLinkedLibrary));
            var generator = new EricksonLopez.DomainPrimitives.Generators.StringPrimitiveGenerator();
            Microsoft.CodeAnalysis.GeneratorDriver driver = Microsoft.CodeAnalysis.CSharp.CSharpGeneratorDriver.Create(generator);
            driver = driver.RunGenerators(compilation);
            var runResult = driver.GetRunResult();
            var diagnostics = runResult.Diagnostics;
            Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
            Assert.NotEmpty(runResult.GeneratedTrees);
            return VerifyXunit.Verifier.Verify(driver).UseDirectory("Snapshots");
        }
    }
}



