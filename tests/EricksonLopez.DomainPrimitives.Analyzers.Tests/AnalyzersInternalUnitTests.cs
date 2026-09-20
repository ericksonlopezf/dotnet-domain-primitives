// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using AwesomeAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace EricksonLopez.DomainPrimitives.Analyzers.Tests;

public class AnalyzersInternalUnitTests
{
    [Fact]
    public void StructDeclarationAnalyzer_Metadata_IsCorrect()
    {
        var analyzer = new StructDeclarationAnalyzer();
        analyzer.SupportedDiagnostics.Should().Equal(
            DiagnosticDescriptors.DP0001_MustBePartial,
            DiagnosticDescriptors.DP0002_MustBeReadonly,
            DiagnosticDescriptors.DP0003_MustBeRecordStruct);

        StructDeclarationAnalyzer.TriggerAttributes.Should().Contain(new[]
        {
            "StrongId", "StringPrimitive", "NumericPrimitive", "DatePrimitive", "ValueObject",
            "Email", "Phone", "Url", "Slug", "CountryCode", "LanguageCode", "CurrencyCode", "Username",
            "PasswordHash", "HexColor", "IPAddress", "MacAddress", "IBAN", "ISBN", "VIN",
            "Money", "Percentage", "Latitude", "Longitude", "Age", "Weight", "Height", "Distance", "Temperature", "Score",
            "Quantity", "Price", "TaxRate", "Discount", "Rating",
            "BirthDate", "ExpirationDate", "BusinessDate", "FiscalYear", "Month", "Quarter", "Week", "DateRange", "TimeRange"
        });
    }

    [Fact]
    public void AttributeValidationAnalyzer_Metadata_IsCorrect()
    {
        var analyzer = new AttributeValidationAnalyzer();
        analyzer.SupportedDiagnostics.Should().Equal(
            DiagnosticDescriptors.DP0004_InvalidRegex,
            DiagnosticDescriptors.DP0005_ConflictingNormalization,
            DiagnosticDescriptors.DP0006_InvalidConstraintBounds,
            DiagnosticDescriptors.DP0017_InvalidExceptionType);
    }

    [Fact]
    public void MissingValidationAnalyzer_Metadata_IsCorrect()
    {
        var analyzer = new MissingValidationAnalyzer();
        analyzer.SupportedDiagnostics.Should().Equal(
            DiagnosticDescriptors.DP0009_MissingValidation);

        MissingValidationAnalyzer.ValidationAttributeNames.Should().Equal(
            "MinLengthAttribute", "MaxLengthAttribute", "LengthAttribute", 
            "RegexAttribute", "RangeAttribute", "PrimitiveRangeAttribute", "NotEmptyAttribute", "CustomValidatorAttribute");

        MissingValidationAnalyzer.StringIdConstraintAttributeNames.Should().Equal(
            "MinLengthAttribute", "MaxLengthAttribute", "LengthAttribute", "RegexAttribute");

        MissingValidationAnalyzer.DomainShortcutAttributeNames.Should().Contain(new[]
        {
            "EmailAttribute", "PhoneAttribute", "UrlAttribute", "SlugAttribute",
            "CountryCodeAttribute", "LanguageCodeAttribute", "CurrencyCodeAttribute",
            "UsernameAttribute", "PasswordHashAttribute", "HexColorAttribute",
            "IPAddressAttribute", "MacAddressAttribute", "IBANAttribute", "ISBNAttribute", "VINAttribute",
            "LatitudeAttribute", "LongitudeAttribute", "AgeAttribute", "WeightAttribute", "HeightAttribute",
            "DistanceAttribute", "TemperatureAttribute", "ScoreAttribute", "QuantityAttribute",
            "PriceAttribute", "TaxRateAttribute", "DiscountAttribute", "RatingAttribute",
            "PercentageAttribute", "MoneyAttribute", "BirthDateAttribute", "ExpirationDateAttribute",
            "BusinessDateAttribute", "FiscalYearAttribute", "MonthAttribute", "QuarterAttribute",
            "WeekAttribute", "DateRangeAttribute", "TimeRangeAttribute"
        });
    }

    [Fact]
    public void DuplicatePrimitiveLogicAnalyzer_Metadata_IsCorrect()
    {
        var analyzer = new DuplicatePrimitiveLogicAnalyzer();
        analyzer.SupportedDiagnostics.Should().Equal(
            DiagnosticDescriptors.DP0013_PossibleDuplicatePrimitiveLogic);

        DuplicatePrimitiveLogicAnalyzer.IsShortcutAttribute(null).Should().BeFalse();
        DuplicatePrimitiveLogicAnalyzer.IsShortcutAttribute("NonExistentAttribute").Should().BeFalse();
        DuplicatePrimitiveLogicAnalyzer.IsShortcutAttribute("EmailAttribute").Should().BeTrue();
        DuplicatePrimitiveLogicAnalyzer.IsShortcutAttribute("TimeRangeAttribute").Should().BeTrue();
    }

    [Fact]
    public void PublicConstructorBypassAnalyzer_Metadata_IsCorrect()
    {
        var analyzer = new PublicConstructorBypassAnalyzer();
        analyzer.SupportedDiagnostics.Should().Equal(
            DiagnosticDescriptors.DP0012_PublicConstructorBypass);

        PublicConstructorBypassAnalyzer.DomainPrimitiveAttributeNames.Should().Equal(
            "StrongIdAttribute", "StringPrimitiveAttribute", "NumericPrimitiveAttribute",
            "DatePrimitiveAttribute", "SmartEnumAttribute",
            "EmailAttribute", "PhoneAttribute", "UrlAttribute", "SlugAttribute",
            "CountryCodeAttribute", "LanguageCodeAttribute", "CurrencyCodeAttribute",
            "UsernameAttribute", "PasswordHashAttribute", "HexColorAttribute",
            "IPAddressAttribute", "MacAddressAttribute", "IBANAttribute", "ISBNAttribute", "VINAttribute",
            "MoneyAttribute", "PercentageAttribute", "LatitudeAttribute", "LongitudeAttribute",
            "AgeAttribute", "WeightAttribute", "HeightAttribute", "DistanceAttribute",
            "TemperatureAttribute", "ScoreAttribute", "QuantityAttribute", "PriceAttribute",
            "TaxRateAttribute", "DiscountAttribute", "RatingAttribute",
            "BirthDateAttribute", "ExpirationDateAttribute", "BusinessDateAttribute",
            "FiscalYearAttribute", "MonthAttribute", "QuarterAttribute", "WeekAttribute",
            "DateRangeAttribute", "TimeRangeAttribute");
    }

    [Fact]
    public void StringComparisonAnalyzer_Metadata_IsCorrect()
    {
        var analyzer = new StringComparisonAnalyzer();
        analyzer.SupportedDiagnostics.Should().Equal(
            DiagnosticDescriptors.DP0010_StringComparedWithPrimitive,
            DiagnosticDescriptors.DP0011_StringAssignedFromPrimitive);
    }

    [Fact]
    public void ValueObjectAnalyzer_Metadata_IsCorrect()
    {
        var analyzer = new ValueObjectAnalyzer();
        analyzer.SupportedDiagnostics.Should().Equal(
            DiagnosticDescriptors.DP0008_ValueObjectRequiresInit,
            DiagnosticDescriptors.DP0018_ValueObjectMutableCollection);
    }

    [Fact]
    public void PrimitiveUsageAnalyzer_Metadata_IsCorrect()
    {
        var analyzer = new PrimitiveUsageAnalyzer();
        analyzer.SupportedDiagnostics.Should().Equal(
            DiagnosticDescriptors.DP0007_AvoidDefaultConstructor);
    }

    [Fact]
    public void ApiReviewAnalyzer_Metadata_IsCorrect()
    {
        var analyzer = new ApiReviewAnalyzer();
        analyzer.SupportedDiagnostics.Should().Equal(
            DiagnosticDescriptors.DP0014_ApiSurfaceBudgetExceeded,
            DiagnosticDescriptors.DP0015_MissingXmlDocumentation,
            DiagnosticDescriptors.DP0016_InvalidFactoryMethodName);

        ApiReviewAnalyzer.PrimitiveAttributeNames.Should().BeEquivalentTo(
            "StringPrimitive", "StringPrimitiveAttribute",
            "NumericPrimitive", "NumericPrimitiveAttribute",
            "DatePrimitive", "DatePrimitiveAttribute",
            "StrongId", "StrongIdAttribute",
            "ValueObject", "ValueObjectAttribute",
            "SmartEnum", "SmartEnumAttribute");

        ApiReviewAnalyzer.ValidFactoryNames.Should().BeEquivalentTo(
            "Create", "TryCreate", "Parse", "TryParse");
    }

    [Fact]
    public void PublicConstructorBypassAnalyzer_IsInGeneratedCode_ReturnsExpected()
    {
        var syntaxTreeGenerated = CSharpSyntaxTree.ParseText("public struct Test {}", path: "Generated.g.cs");
        var locGen = syntaxTreeGenerated.GetRoot().GetLocation();
        PublicConstructorBypassAnalyzer.IsInGeneratedCode(locGen).Should().BeTrue();

        var syntaxTreeGenerated2 = CSharpSyntaxTree.ParseText("public struct Test2 {}", path: "Generated.generated.cs");
        var locGen2 = syntaxTreeGenerated2.GetRoot().GetLocation();
        PublicConstructorBypassAnalyzer.IsInGeneratedCode(locGen2).Should().BeTrue();

        var syntaxTreeUser = CSharpSyntaxTree.ParseText("public struct UserTest {}", path: "UserTest.cs");
        var locUser = syntaxTreeUser.GetRoot().GetLocation();
        PublicConstructorBypassAnalyzer.IsInGeneratedCode(locUser).Should().BeFalse();

        // Location.None (no source tree)
        PublicConstructorBypassAnalyzer.IsInGeneratedCode(Location.None).Should().BeFalse();
    }

    [Fact]
    public void StructDeclarationAnalyzer_Initialize_EnablesConcurrentExecution()
    {
        var analyzer = new StructDeclarationAnalyzer();
        var ctx = new MockAnalysisContext();
        analyzer.Initialize(ctx);
        ctx.ConcurrentExecutionEnabled.Should().BeTrue();
        ctx.GeneratedCodeFlags.Should().Be(GeneratedCodeAnalysisFlags.None);
        ctx.SyntaxNodeActions.Should().NotBeEmpty();
    }

    [Fact]
    public void PublicConstructorBypassAnalyzer_Initialize_EnablesConcurrentExecution()
    {
        var analyzer = new PublicConstructorBypassAnalyzer();
        var ctx = new MockAnalysisContext();
        analyzer.Initialize(ctx);
        ctx.ConcurrentExecutionEnabled.Should().BeTrue();
        ctx.GeneratedCodeFlags.Should().Be(GeneratedCodeAnalysisFlags.None);
        ctx.SymbolActions.Should().NotBeEmpty();
    }

    [Fact]
    public void StringComparisonAnalyzer_Initialize_EnablesConcurrentExecution()
    {
        var analyzer = new StringComparisonAnalyzer();
        var ctx = new MockAnalysisContext();
        analyzer.Initialize(ctx);
        ctx.ConcurrentExecutionEnabled.Should().BeTrue();
        ctx.GeneratedCodeFlags.Should().Be(GeneratedCodeAnalysisFlags.None);
        ctx.SyntaxNodeActions.Should().NotBeEmpty();
    }

    [Fact]
    public void AttributeValidationAnalyzer_Initialize_EnablesConcurrentExecution()
    {
        var analyzer = new AttributeValidationAnalyzer();
        var ctx = new MockAnalysisContext();
        analyzer.Initialize(ctx);
        ctx.ConcurrentExecutionEnabled.Should().BeTrue();
        ctx.GeneratedCodeFlags.Should().Be(GeneratedCodeAnalysisFlags.None);
        ctx.SyntaxNodeActions.Should().NotBeEmpty();
    }

    [Fact]
    public void ValueObjectAnalyzer_Initialize_EnablesConcurrentExecution()
    {
        var analyzer = new ValueObjectAnalyzer();
        var ctx = new MockAnalysisContext();
        analyzer.Initialize(ctx);
        ctx.ConcurrentExecutionEnabled.Should().BeTrue();
        ctx.GeneratedCodeFlags.Should().Be(GeneratedCodeAnalysisFlags.None);
        ctx.SyntaxNodeActions.Should().NotBeEmpty();
    }

    [Fact]
    public void PrimitiveUsageAnalyzer_Initialize_EnablesConcurrentExecution()
    {
        var analyzer = new PrimitiveUsageAnalyzer();
        var ctx = new MockAnalysisContext();
        analyzer.Initialize(ctx);
        ctx.ConcurrentExecutionEnabled.Should().BeTrue();
        ctx.GeneratedCodeFlags.Should().Be(GeneratedCodeAnalysisFlags.None);
        ctx.SyntaxNodeActions.Should().NotBeEmpty();
    }

    [Fact]
    public void MissingValidationAnalyzer_Initialize_EnablesConcurrentExecution()
    {
        var analyzer = new MissingValidationAnalyzer();
        var ctx = new MockAnalysisContext();
        analyzer.Initialize(ctx);
        ctx.ConcurrentExecutionEnabled.Should().BeTrue();
        ctx.GeneratedCodeFlags.Should().Be(GeneratedCodeAnalysisFlags.None);
        ctx.SyntaxNodeActions.Should().NotBeEmpty();
    }

    [Fact]
    public void DuplicatePrimitiveLogicAnalyzer_Initialize_EnablesConcurrentExecution()
    {
        var analyzer = new DuplicatePrimitiveLogicAnalyzer();
        var ctx = new MockAnalysisContext();
        analyzer.Initialize(ctx);
        ctx.ConcurrentExecutionEnabled.Should().BeTrue();
        ctx.GeneratedCodeFlags.Should().Be(GeneratedCodeAnalysisFlags.None);
        ctx.CompilationStartActions.Should().NotBeEmpty();
    }

    [Fact]
    public void DuplicatePrimitiveLogicAnalyzer_GetAttributeSignature_FormatsCorrectly()
    {
        var code = @"
namespace EricksonLopez.DomainPrimitives
{
    public class StringPrimitiveAttribute : System.Attribute {}
    public class DatePrimitiveAttribute : System.Attribute { public bool PastOnly { get; set; } public string Format { get; set; } }
}
namespace EricksonLopez.DomainPrimitives.Validation
{
    public class MinLengthAttribute : System.Attribute { public MinLengthAttribute(int len) {} }
    public class RangeAttribute : System.Attribute { public RangeAttribute(object min, object max) {} }
}
namespace OtherNs
{
    public class OtherAttribute : System.Attribute {}
}

[OtherNs.Other]
[EricksonLopez.DomainPrimitives.StringPrimitive]
[EricksonLopez.DomainPrimitives.Validation.MinLength(5)]
[EricksonLopez.DomainPrimitives.DatePrimitive(PastOnly = true, Format = null)]
[EricksonLopez.DomainPrimitives.Validation.Range(null, null)]
public record struct SampleStruct;
";
        var tree = CSharpSyntaxTree.ParseText(code);
        var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
        var compilation = CSharpCompilation.Create("TestComp", new[] { tree }, new[] { mscorlib });
        var model = compilation.GetSemanticModel(tree);
        var structDecl = tree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().First();
        var symbol = model.GetDeclaredSymbol(structDecl)!;

        var otherAttr = symbol.GetAttributes().First(a => a.AttributeClass?.Name == "OtherAttribute");
        var domainAttr = symbol.GetAttributes().First(a => a.AttributeClass?.Name == "StringPrimitiveAttribute");

        DuplicatePrimitiveLogicAnalyzer.IsDomainPrimitiveAttribute(otherAttr).Should().BeFalse();
        DuplicatePrimitiveLogicAnalyzer.IsDomainPrimitiveAttribute(domainAttr).Should().BeTrue();

        var signature = DuplicatePrimitiveLogicAnalyzer.GetAttributeSignature(symbol);
        signature.Should().Be("DatePrimitiveAttribute(){Format=null,PastOnly=True}|MinLengthAttribute(5){}|RangeAttribute(null,null){}|StringPrimitiveAttribute(){}");
    }

    [Fact]
    public void ApiReviewCodeFixProvider_GetMemberName_ExtractsExpectedNames()
    {
        var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("void"), "MyMethod");
        ApiReviewCodeFixProvider.GetMemberName(method).Should().Be("MyMethod");

        var prop = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName("int"), "MyProp");
        ApiReviewCodeFixProvider.GetMemberName(prop).Should().Be("MyProp");

        var fieldWithVar = SyntaxFactory.FieldDeclaration(
            SyntaxFactory.VariableDeclaration(
                SyntaxFactory.ParseTypeName("int"),
                SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator("myField"))));
        ApiReviewCodeFixProvider.GetMemberName(fieldWithVar).Should().Be("myField");

        var fieldEmpty = SyntaxFactory.FieldDeclaration(
            SyntaxFactory.VariableDeclaration(SyntaxFactory.ParseTypeName("int")));
        ApiReviewCodeFixProvider.GetMemberName(fieldEmpty).Should().Be("member");

        var other = SyntaxFactory.EventFieldDeclaration(
            SyntaxFactory.VariableDeclaration(SyntaxFactory.ParseTypeName("System.EventHandler")));
        ApiReviewCodeFixProvider.GetMemberName(other).Should().Be("member");
    }

    [Fact]
    public void ApiReviewCodeFixProvider_CreateXmlDocTrivia_HandlesTriviaVariations()
    {
        var prop = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName("int"), "Val");

        // Whitespace only
        var withWhitespace = prop.WithLeadingTrivia(SyntaxFactory.Whitespace("    "));
        var trivia1 = ApiReviewCodeFixProvider.CreateXmlDocTrivia(withWhitespace);
        trivia1.ToFullString().Should().Be("    /// <summary>\r\n    /// Gets or sets the Val.\r\n    /// </summary>\r\n    ");

        // Comment + whitespace
        var withComment = prop.WithLeadingTrivia(SyntaxFactory.Comment("// note"), SyntaxFactory.Whitespace("  "));
        var trivia2 = ApiReviewCodeFixProvider.CreateXmlDocTrivia(withComment);
        trivia2.ToFullString().Should().Be("// note  /// <summary>\r\n  /// Gets or sets the Val.\r\n  /// </summary>\r\n  ");

        // Empty trivia
        var withEmpty = prop.WithLeadingTrivia();
        var trivia3 = ApiReviewCodeFixProvider.CreateXmlDocTrivia(withEmpty);
        trivia3.ToFullString().Should().Be("/// <summary>\r\n/// Gets or sets the Val.\r\n/// </summary>\r\n");
    }

    [Fact]
    public void ApiReviewAnalyzer_Initialize_EnablesConcurrentExecution()
    {
        var analyzer = new ApiReviewAnalyzer();
        var ctx = new MockAnalysisContext();
        analyzer.Initialize(ctx);
        ctx.ConcurrentExecutionEnabled.Should().BeTrue();
        ctx.GeneratedCodeFlags.Should().Be(GeneratedCodeAnalysisFlags.None);
        ctx.SymbolStartActions.Should().NotBeEmpty();
    }

    [Fact]
    public void ValueObjectAnalyzer_IsValueObjectType_EdgeCases_Covered()
    {
        // 1. Record struct with [ValueObject] (without Attribute suffix)
        var sourceWithVoAttr = @"
public class ValueObject : System.Attribute {}
[ValueObject]
public readonly record struct StructWithVo {}
";
        var tree1 = CSharpSyntaxTree.ParseText(sourceWithVoAttr);
        var comp1 = CSharpCompilation.Create("Test1", new[] { tree1 }, Basic.Reference.Assemblies.Net80.References.All);
        var model1 = comp1.GetSemanticModel(tree1);
        var decl1 = tree1.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>().First();
        var symbol1 = model1.GetDeclaredSymbol(decl1)!;

        ValueObjectAnalyzer.IsValueObjectType(decl1, symbol1).Should().BeTrue();

        // 2. Class inheriting from ValueObject in global namespace
        var sourceGlobalVo = @"
public abstract class ValueObject {}
public class DerivedGlobalVo : ValueObject {}
public class NonVoBase {}
public class DerivedNonVo : NonVoBase {}
";
        var tree2 = CSharpSyntaxTree.ParseText(sourceGlobalVo);
        var comp2 = CSharpCompilation.Create("Test2", new[] { tree2 }, Basic.Reference.Assemblies.Net80.References.All);
        var model2 = comp2.GetSemanticModel(tree2);
        var decl2 = tree2.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().First(c => c.Identifier.Text == "DerivedGlobalVo");
        var symbol2 = model2.GetDeclaredSymbol(decl2)!;
        ValueObjectAnalyzer.IsValueObjectType(decl2, symbol2).Should().BeTrue();

        // 3. Class inheriting from base not named ValueObject (verifies && vs ||)
        var decl3 = tree2.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().First(c => c.Identifier.Text == "DerivedNonVo");
        var symbol3 = model2.GetDeclaredSymbol(decl3)!;
        ValueObjectAnalyzer.IsValueObjectType(decl3, symbol3).Should().BeFalse();

        // 4. Class inheriting from ValueObject in a third-party non-DomainPrimitives namespace (kills mutant 1029)
        var sourceThirdPartyVo = @"
namespace ThirdParty
{
    public abstract class ValueObject {}
}
namespace MyProject
{
    public class DerivedThirdPartyVo : ThirdParty.ValueObject {}
}
";
        var tree4 = CSharpSyntaxTree.ParseText(sourceThirdPartyVo);
        var comp4 = CSharpCompilation.Create("Test4", new[] { tree4 }, Basic.Reference.Assemblies.Net80.References.All);
        var model4 = comp4.GetSemanticModel(tree4);
        var decl4 = tree4.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().First(c => c.Identifier.Text == "DerivedThirdPartyVo");
        var symbol4 = model4.GetDeclaredSymbol(decl4)!;
        ValueObjectAnalyzer.IsValueObjectType(decl4, symbol4).Should().BeFalse();

        // 5. Class inheriting from ValueObject in EricksonLopez.DomainPrimitives namespace
        var sourceElVo = @"
namespace EricksonLopez.DomainPrimitives
{
    public abstract class ValueObject {}
}
namespace MyProject
{
    public class DerivedElVo : EricksonLopez.DomainPrimitives.ValueObject {}
}
";
        var tree5 = CSharpSyntaxTree.ParseText(sourceElVo);
        var comp5 = CSharpCompilation.Create("Test5", new[] { tree5 }, Basic.Reference.Assemblies.Net80.References.All);
        var model5 = comp5.GetSemanticModel(tree5);
        var decl5 = tree5.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().First(c => c.Identifier.Text == "DerivedElVo");
        var symbol5 = model5.GetDeclaredSymbol(decl5)!;
        ValueObjectAnalyzer.IsValueObjectType(decl5, symbol5).Should().BeTrue();
    }

    [Fact]
    public void ValueObjectAnalyzer_WhenParentTypeIsNull_DoesNotThrow()
    {
        var analyzer = new ValueObjectAnalyzer();
        var detachedTree = CSharpSyntaxTree.ParseText("int Prop { get; set; }");
        var propNode = detachedTree.GetRoot().DescendantNodes().OfType<PropertyDeclarationSyntax>().First();

        var emptyTree = CSharpSyntaxTree.ParseText("// empty");
        var comp = CSharpCompilation.Create("Empty", new[] { emptyTree });
        var model = comp.GetSemanticModel(emptyTree);

#pragma warning disable CS0618
        var context = new SyntaxNodeAnalysisContext(propNode, model, new AnalyzerOptions(ImmutableArray<AdditionalText>.Empty), _ => { }, _ => true, CancellationToken.None);
#pragma warning restore CS0618
        var act = () => analyzer.AnalyzePropertyDeclaration(context);
        act.Should().NotThrow();
    }

    [Fact]
    public void ApiReviewAnalyzer_IsGeneratedMember_EvaluatesFilePathsCorrectly()
    {
        var gTree = CSharpSyntaxTree.ParseText("public class GenFoo {}", path: "C:\\MyPath\\GenFoo.g.cs");
        var genTree = CSharpSyntaxTree.ParseText("public class GenBar {}", path: "C:\\MyPath\\GenBar.generated.cs");
        var nonGenTree = CSharpSyntaxTree.ParseText("public class PlainBaz {}", path: "C:\\MyPath\\PlainBaz.cs");
        var noPathTree = CSharpSyntaxTree.ParseText("public class NoPathQux {}");

        var comp = CSharpCompilation.Create("TestComp", new[] { gTree, genTree, nonGenTree, noPathTree });

        var genFoo = comp.GetTypeByMetadataName("GenFoo")!;
        var genBar = comp.GetTypeByMetadataName("GenBar")!;
        var plainBaz = comp.GetTypeByMetadataName("PlainBaz")!;
        var noPathQux = comp.GetTypeByMetadataName("NoPathQux")!;

        ApiReviewAnalyzer.IsGeneratedMember(genFoo).Should().BeTrue();
        ApiReviewAnalyzer.IsGeneratedMember(genBar).Should().BeTrue();
        ApiReviewAnalyzer.IsGeneratedMember(plainBaz).Should().BeFalse();
        ApiReviewAnalyzer.IsGeneratedMember(noPathQux).Should().BeFalse();
    }

    [Fact]
    public void PublicConstructorBypassAnalyzer_AnalyzeNamedType_WithMetadataConstructor_DoesNotReportDiagnostic()
    {
        string dummyAttr = @"
namespace EricksonLopez.DomainPrimitives
{
    public class StringPrimitiveAttribute : System.Attribute { }
}
namespace Test
{
    [EricksonLopez.DomainPrimitives.StringPrimitive]
    public readonly record struct MetadataPrimitive
    {
        public MetadataPrimitive(string s) {}
    }
}";
        var refCompilation = CSharpCompilation.Create(
            "ExternalLib",
            new[] { CSharpSyntaxTree.ParseText(dummyAttr) },
            new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var ms = new System.IO.MemoryStream();
        var emitResult = refCompilation.Emit(ms);
        emitResult.Success.Should().BeTrue();

        var metaRef = MetadataReference.CreateFromImage(ms.ToArray());
        var consumerComp = CSharpCompilation.Create(
            "Consumer",
            Array.Empty<SyntaxTree>(),
            new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location), metaRef },
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var typeSymbol = consumerComp.GetTypeByMetadataName("Test.MetadataPrimitive")!;
        var diagnostics = new List<Diagnostic>();

#pragma warning disable CS0618
        var context = new SymbolAnalysisContext(
            typeSymbol,
            consumerComp,
            new AnalyzerOptions(ImmutableArray<AdditionalText>.Empty),
            diagnostics.Add,
            _ => true,
            CancellationToken.None);
#pragma warning restore CS0618

        PublicConstructorBypassAnalyzer.AnalyzeNamedType(context);

        diagnostics.Should().BeEmpty();
    }

    private sealed class MockAnalysisContext : AnalysisContext
    {
        public bool ConcurrentExecutionEnabled { get; private set; }
        public GeneratedCodeAnalysisFlags GeneratedCodeFlags { get; private set; }
        public List<object> SymbolActions { get; } = new();
        public List<object> SyntaxNodeActions { get; } = new();
        public List<object> CompilationStartActions { get; } = new();
        public List<object> SymbolStartActions { get; } = new();

        public override void EnableConcurrentExecution() => ConcurrentExecutionEnabled = true;
        public override void ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags analysisMode) => GeneratedCodeFlags = analysisMode;
        public override void RegisterSymbolAction(System.Action<SymbolAnalysisContext> action, ImmutableArray<SymbolKind> symbolKinds) => SymbolActions.Add(action);
        public override void RegisterSyntaxNodeAction<TLanguageKindEnum>(System.Action<SyntaxNodeAnalysisContext> action, ImmutableArray<TLanguageKindEnum> syntaxKinds) => SyntaxNodeActions.Add(action);
        public override void RegisterCompilationAction(System.Action<CompilationAnalysisContext> action) { }
        public override void RegisterCompilationStartAction(System.Action<CompilationStartAnalysisContext> action) => CompilationStartActions.Add(action);
        public override void RegisterSymbolStartAction(System.Action<SymbolStartAnalysisContext> action, SymbolKind symbolKind) => SymbolStartActions.Add(action);
        public override void RegisterSemanticModelAction(System.Action<SemanticModelAnalysisContext> action) { }
        public override void RegisterSyntaxTreeAction(System.Action<SyntaxTreeAnalysisContext> action) { }
        public override void RegisterCodeBlockAction(System.Action<CodeBlockAnalysisContext> action) { }
        public override void RegisterCodeBlockStartAction<TLanguageKindEnum>(System.Action<CodeBlockStartAnalysisContext<TLanguageKindEnum>> action) { }
    }
}





