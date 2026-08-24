// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace EricksonLopez.DomainPrimitives.Analyzers.Tests;

public class FastAnalyzerDriverUnitTests
{
    private static async Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync(DiagnosticAnalyzer analyzer, string source)
    {
        var stubTree = CSharpSyntaxTree.ParseText(RoslynTestSnippets.CommonFrameworkStubs, new CSharpParseOptions(LanguageVersion.CSharp11));
        var userCode = "using System;\r\nusing EricksonLopez.DomainPrimitives;\r\nusing EricksonLopez.DomainPrimitives.Validation;\r\nusing EricksonLopez.DomainPrimitives.Normalization;\r\n" + source;
        var userTree = CSharpSyntaxTree.ParseText(userCode, new CSharpParseOptions(LanguageVersion.CSharp11));
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            new[] { stubTree, userTree },
            Basic.Reference.Assemblies.Net80.References.All,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var compilationWithAnalyzers = compilation.WithAnalyzers(ImmutableArray.Create(analyzer));
        return await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync();
    }

    [Fact]
    public async Task StructDeclarationAnalyzer_AllRulesAndModifiers_Covered()
    {
        var analyzer = new StructDeclarationAnalyzer();

        // DP0003: Struct instead of Record Struct
        var srcStruct = @"
using EricksonLopez.DomainPrimitives;
[StringPrimitive]
public struct PlainStructId {}
";
        var diags = await GetDiagnosticsAsync(analyzer, srcStruct);
        diags.Should().Contain(d => d.Id == "DP0003");

        // DP0002: Not Readonly
        var srcNotReadonly = @"
[StringPrimitive]
public partial record struct MutableId {}
";
        diags = await GetDiagnosticsAsync(analyzer, srcNotReadonly);
        diags.Should().Contain(d => d.Id == "DP0002");

        // DP0001: Not Partial
        var srcNotPartial = @"
[EmailAttribute]
public readonly record struct NonPartialEmailId {}
";
        diags = await GetDiagnosticsAsync(analyzer, srcNotPartial);
        diags.Should().Contain(d => d.Id == "DP0001");

        // Fully valid
        var srcValid = @"
[StringPrimitive]
public readonly partial record struct ValidId {}
";
        diags = await GetDiagnosticsAsync(analyzer, srcValid);
        diags.Should().BeEmpty();
    }

    [Fact]
    public async Task AttributeValidationAnalyzer_AllValidationRules_Covered()
    {
        var analyzer = new AttributeValidationAnalyzer();

        // DP0004: Invalid Regex
        var srcRegex = @"
using EricksonLopez.DomainPrimitives.Validation;
[Regex(""["")]
public readonly partial record struct BadRegexId {}
";
        var diags = await GetDiagnosticsAsync(analyzer, srcRegex);
        diags.Should().Contain(d => d.Id == "DP0004");

        // DP0005: LowerCase + UpperCase
        var srcConflict = @"
using EricksonLopez.DomainPrimitives.Normalization;
[LowerCase, UpperCase]
public readonly partial record struct ConflictCasingId {}
";
        diags = await GetDiagnosticsAsync(analyzer, srcConflict);
        diags.Should().Contain(d => d.Id == "DP0005");

        // DP0006: MinLength > MaxLength
        var srcMinMax = @"
[MinLength(10), MaxLength(5)]
public readonly partial record struct BadMinMaxId {}
";
        diags = await GetDiagnosticsAsync(analyzer, srcMinMax);
        diags.Should().Contain(d => d.Id == "DP0006");

        // DP0006: Length(10, 5)
        var srcLength = @"
[Length(10, 5)]
public readonly partial record struct BadLengthId {}
";
        diags = await GetDiagnosticsAsync(analyzer, srcLength);
        diags.Should().Contain(d => d.Id == "DP0006");

        // DP0006: Range(10, 5)
        var srcRangeInt = @"
[Range(10, 5)]
public readonly partial record struct BadRangeIntId {}
";
        diags = await GetDiagnosticsAsync(analyzer, srcRangeInt);
        diags.Should().Contain(d => d.Id == "DP0006");

        // DP0006: Range(10.0, 5.0)
        var srcRangeDouble = @"
[Range(10.0, 5.0)]
public readonly partial record struct BadRangeDoubleId {}
";
        diags = await GetDiagnosticsAsync(analyzer, srcRangeDouble);
        diags.Should().Contain(d => d.Id == "DP0006");

        // DP0006: Range("10.0", "5.0")
        var srcRangeString = @"
[Range(""10.0"", ""5.0"")]
public readonly partial record struct BadRangeStringId {}
";
        diags = await GetDiagnosticsAsync(analyzer, srcRangeString);
        diags.Should().Contain(d => d.Id == "DP0006");

        // DP0017: Invalid exception type in assembly defaults
        var srcDefaultsNotExc = @"
[assembly: DomainPrimitivesDefaults(ExceptionType = typeof(string))]
";
        diags = await GetDiagnosticsAsync(analyzer, srcDefaultsNotExc);
        diags.Should().Contain(d => d.Id == "DP0017");

        var srcDefaultsNoStringCtor = @"
[assembly: DomainPrimitivesDefaults(ExceptionType = typeof(CustomNoStringCtorException))]
public class CustomNoStringCtorException : Exception { public CustomNoStringCtorException() {} }
";
        diags = await GetDiagnosticsAsync(analyzer, srcDefaultsNoStringCtor);
        diags.Should().Contain(d => d.Id == "DP0017");
    }

    [Fact]
    public async Task MissingValidationAnalyzer_AllCases_Covered()
    {
        var analyzer = new MissingValidationAnalyzer();

        // DP0009: StringPrimitive without validation
        var srcMissing = @"
[StringPrimitive]
public readonly partial record struct NakedStringId {}
";
        var diags = await GetDiagnosticsAsync(analyzer, srcMissing);
        diags.Should().Contain(d => d.Id == "DP0009");

        // Valid with NotEmpty
        var srcWithValidation = @"
[StringPrimitive, NotEmpty]
public readonly partial record struct ValidatedStringId {}
";
        diags = await GetDiagnosticsAsync(analyzer, srcWithValidation);
        diags.Should().BeEmpty();

        // DP0009: StrongId<string> without constraints
        var srcStrongIdString = @"
[StrongId<string>]
public readonly partial record struct NakedStrongStringId {}
";
        diags = await GetDiagnosticsAsync(analyzer, srcStrongIdString);
        diags.Should().Contain(d => d.Id == "DP0009");

        // Non-string StrongId should not trigger DP0009
        var srcStrongIdInt = @"
[StrongId<int>]
public readonly partial record struct IntStrongId {}
";
        diags = await GetDiagnosticsAsync(analyzer, srcStrongIdInt);
        diags.Should().BeEmpty();
    }

    [Fact]
    public async Task StringComparisonAnalyzer_AllCases_Covered()
    {
        var analyzer = new StringComparisonAnalyzer();

        var src = @"

public readonly partial record struct CustomerCode : IDomainPrimitive<CustomerCode>
{
    public string Value => ""test"";
}

public class Usage
{
    public void Test(CustomerCode code, string raw)
    {
        // DP0010: String compared directly with primitive
        if (code == raw) {}
        if (raw == code) {}

        // DP0011: String assigned from primitive
        string s1 = code;
        string s2;
        s2 = code;
        TakesString(code);
    }

    public void TakesString(string x) {}
}
";
        var diags = await GetDiagnosticsAsync(analyzer, src);
        diags.Should().Contain(d => d.Id == "DP0010");
        diags.Should().Contain(d => d.Id == "DP0011");
    }

    [Fact]
    public async Task PublicConstructorBypassAnalyzer_AllCases_Covered()
    {
        var analyzer = new PublicConstructorBypassAnalyzer();

        var srcBypass = @"
[StringPrimitive]
public readonly partial record struct BypassPrimitive
{
    public BypassPrimitive(string val) {}
}
";
        var diags = await GetDiagnosticsAsync(analyzer, srcBypass);
        diags.Should().Contain(d => d.Id == "DP0012");

        var srcNoBypass = @"
[StringPrimitive]
public readonly partial record struct SafePrimitive
{
    private SafePrimitive(string val) {}
}
";
        diags = await GetDiagnosticsAsync(analyzer, srcNoBypass);
        diags.Should().BeEmpty();
    }

    [Fact]
    public async Task ValueObjectAnalyzer_AllCases_Covered()
    {
        var analyzer = new ValueObjectAnalyzer();

        var srcBadVo = @"
[ValueObject]
public readonly partial record struct BadVo
{
    public int Amount { get; set; }
}
";
        var diags = await GetDiagnosticsAsync(analyzer, srcBadVo);
        diags.Should().Contain(d => d.Id == "DP0008");

        var srcGoodVo = @"
[ValueObject]
public readonly partial record struct GoodVo
{
    public int Amount { get; init; }
}
";
        diags = await GetDiagnosticsAsync(analyzer, srcGoodVo);
        diags.Should().BeEmpty();
    }

    [Fact]
    public async Task PrimitiveUsageAnalyzer_AllCases_Covered()
    {
        var analyzer = new PrimitiveUsageAnalyzer();

        var src = @"
public readonly partial record struct OrderId : IStrongId<OrderId, int> {}

public class Consumer
{
    public void Test()
    {
        // DP0007: Default constructor / default expression
        var id1 = new OrderId();
        OrderId id2 = default;
    }
}
";
        var diags = await GetDiagnosticsAsync(analyzer, src);
        diags.Should().Contain(d => d.Id == "DP0007");
    }

    [Fact]
    public async Task ApiReviewAnalyzer_AllCases_Covered()
    {
        var analyzer = new ApiReviewAnalyzer();

        // DP0016: Invalid factory method name & DP0015: Missing XML Doc
        var src = @"
namespace MyApiReviewNamespace
{
    [StringPrimitive]
    public readonly partial record struct EmailAddress
    {
        public static EmailAddress FromString(string s) => default;
    }
}
";
        var diags = await GetDiagnosticsAsync(analyzer, src);
        diags.Should().Contain(d => d.Id == "DP0016");
        diags.Should().Contain(d => d.Id == "DP0015");
    }

    [Fact]
    public async Task DuplicatePrimitiveLogicAnalyzer_DuplicateSignatures_TriggersDP0013()
    {
        var analyzer = new DuplicatePrimitiveLogicAnalyzer();

        var srcDuplicates = @"

[StringPrimitive, MinLength(5)]
public readonly partial record struct PrimitiveA {}

[StringPrimitive, MinLength(5)]
public readonly partial record struct PrimitiveB {}
";
        var diags = await GetDiagnosticsAsync(analyzer, srcDuplicates);
        diags.Should().Contain(d => d.Id == "DP0013");
    }

    [Fact]
    public async Task DuplicatePrimitiveLogicAnalyzer_UniqueSignatures_ProducesNoDiagnostics()
    {
        var analyzer = new DuplicatePrimitiveLogicAnalyzer();

        var srcUnique = @"

[StringPrimitive, MinLength(5)]
public readonly partial record struct PrimitiveA {}

[StringPrimitive, MinLength(10)]
public readonly partial record struct PrimitiveB {}
";
        var diags = await GetDiagnosticsAsync(analyzer, srcUnique);
        diags.Should().BeEmpty();
    }

    [Fact]
    public async Task StructDeclarationAnalyzer_WithAliasQualifiedAttribute_TriggersDiagnostics()
    {
        var analyzer = new StructDeclarationAnalyzer();
        var src = @"
using StringPrimitive = EricksonLopez.DomainPrimitives.StringPrimitiveAttribute;

[global::StringPrimitive]
public struct StructWithAliasAttributeOnType
{
}
";
        var diags = await GetDiagnosticsAsync(analyzer, src);
        diags.Should().NotBeEmpty();
    }

    [Fact]
    public async Task MissingValidationAnalyzer_WithNoAttributes_ProducesNoDiagnostics()
    {
        var analyzer = new MissingValidationAnalyzer();
        var src = @"
public struct PlainStruct {}
public class PlainClass {}
";
        var diags = await GetDiagnosticsAsync(analyzer, src);
        diags.Should().BeEmpty();
    }

    [Fact]
    public async Task MissingValidationAnalyzer_WithInvalidStructSyntax_TriggersExpectedDiagnostic()
    {
        var analyzer = new MissingValidationAnalyzer();
        var src = @"
[StringPrimitive]
public struct;
";
        var diags = await GetDiagnosticsAsync(analyzer, src);
        diags.Should().Contain(d => d.Id == "DP0009");
    }

    [Fact]
    public async Task StructDeclarationAnalyzer_WithReadonlyPlainStruct_TriggersDP0003()
    {
        var analyzer = new StructDeclarationAnalyzer();
        var src = @"
[StringPrimitive]
public readonly partial struct PlainStructPrimitiveId {}
";
        var diags = await GetDiagnosticsAsync(analyzer, src);
        diags.Should().Contain(d => d.Id == "DP0003");
    }

    [Fact]
    public async Task StructDeclarationAnalyzer_WithNestedRecordStruct_TriggersExpectedDiagnostics()
    {
        var analyzer = new StructDeclarationAnalyzer();
        var src = @"
public class OuterContainer
{
    [StringPrimitive]
    public record struct NestedNonReadonlyId {}
}
";
        var diags = await GetDiagnosticsAsync(analyzer, src);
        diags.Should().Contain(d => d.Id == "DP0002" || d.Id == "DP0001");
    }

    [Fact]
    public async Task PublicConstructorBypassAnalyzer_WithMultipleConstructors_DetectsPublicOne()
    {
        var analyzer = new PublicConstructorBypassAnalyzer();
        var src = @"
[StringPrimitive]
public readonly partial record struct MultiCtorPrimitive
{
    private MultiCtorPrimitive(string a, int b) {}
    public MultiCtorPrimitive(string a) {}
}
";
        var diags = await GetDiagnosticsAsync(analyzer, src);
        diags.Should().Contain(d => d.Id == "DP0012");
    }

    [Fact]
    public async Task PublicConstructorBypassAnalyzer_WithPrivateAndInternalConstructors_DoesNotTrigger()
    {
        var analyzer = new PublicConstructorBypassAnalyzer();
        var src = @"
[StringPrimitive]
public readonly partial record struct InternalCtorPrimitive
{
    internal InternalCtorPrimitive(string a) {}
    private InternalCtorPrimitive(string a, int b) {}
}
";
        var diags = await GetDiagnosticsAsync(analyzer, src);
        diags.Should().BeEmpty();
    }

    [Fact]
    public async Task PublicConstructorBypassAnalyzer_WithNonDomainAttributes_DoesNotTrigger()
    {
        var analyzer = new PublicConstructorBypassAnalyzer();
        var src = @"
[Serializable]
public readonly partial record struct SerializableNonDomainStruct
{
    public SerializableNonDomainStruct(string a) {}
}
";
        var diags = await GetDiagnosticsAsync(analyzer, src);
        diags.Should().BeEmpty();
    }

    [Fact]
    public async Task PublicConstructorBypassAnalyzer_OnClass_DoesNotTrigger()
    {
        var analyzer = new PublicConstructorBypassAnalyzer();
        var src = @"
[StringPrimitive]
public class ClassWithPrimitiveAttribute
{
    public ClassWithPrimitiveAttribute(string a) {}
}
";
        var diags = await GetDiagnosticsAsync(analyzer, src);
        diags.Should().BeEmpty();
    }

    [Fact]
    public async Task PublicConstructorBypassAnalyzer_OnNonRecordStruct_DoesNotTrigger()
    {
        var analyzer = new PublicConstructorBypassAnalyzer();
        var src = @"
[StringPrimitive]
public struct PlainStructWithPrimitiveAttribute
{
    public PlainStructWithPrimitiveAttribute(string a) {}
}
";
        var diags = await GetDiagnosticsAsync(analyzer, src);
        diags.Should().BeEmpty();
    }

    [Fact]
    public async Task StructDeclarationAnalyzer_WithNonDomainStruct_DoesNotTrigger()
    {
        var analyzer = new StructDeclarationAnalyzer();
        var src = @"
[Serializable]
public struct RegularSerializableStruct {}
";
        var diags = await GetDiagnosticsAsync(analyzer, src);
        diags.Should().BeEmpty();
    }

    [Fact]
    public async Task StructDeclarationAnalyzer_WithQualifiedAttributeName_TriggersDiagnostics()
    {
        var analyzer = new StructDeclarationAnalyzer();
        var src = @"
[EricksonLopez.DomainPrimitives.StringPrimitive]
public struct QualifiedAttrStructId {}
";
        var diags = await GetDiagnosticsAsync(analyzer, src);
        diags.Should().Contain(d => d.Id == "DP0003");
    }

    [Fact]
    public async Task PublicConstructorBypassAnalyzer_InGeneratedFileTree_DoesNotTrigger()
    {
        var analyzer = new PublicConstructorBypassAnalyzer();
        var src = @"
[StringPrimitive]
public readonly partial record struct GeneratedPrim
{
    public GeneratedPrim(string a) {}
}
";
        var stubTree = CSharpSyntaxTree.ParseText(RoslynTestSnippets.CommonFrameworkStubs, new CSharpParseOptions(LanguageVersion.CSharp11));
        var genTree = CSharpSyntaxTree.ParseText(src, new CSharpParseOptions(LanguageVersion.CSharp11), path: "GeneratedPrim.g.cs");
        var compilation = CSharpCompilation.Create(
            "TestGenAssembly",
            new[] { stubTree, genTree },
            Basic.Reference.Assemblies.Net80.References.All,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var compilationWithAnalyzers = compilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(analyzer));
        var diags = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync();
        diags.Should().BeEmpty();
    }

    [Fact]
    public async Task StructDeclarationAnalyzer_WithInvalidStructSyntax_HandledSafely()
    {
        var analyzer = new StructDeclarationAnalyzer();
        var src = @"
[StringPrimitive]
public struct;
";
        var diags = await GetDiagnosticsAsync(analyzer, src);
        diags.Should().NotBeNull();
    }

    [Fact]
    public async Task StringComparisonAnalyzer_WithUninitializedVariablesAndFields_DoesNotThrow()
    {
        var analyzer = new StringComparisonAnalyzer();
        var src = @"
public class MyService
{
    private string uninitField;
    private int intField = 42;

    public void Run()
    {
        string uninitLocal;
        int intLocal = 123;
    }
}
";
        var diags = await GetDiagnosticsAsync(analyzer, src);
        diags.Should().BeEmpty();
    }

    [Fact]
    public async Task AttributeValidationAnalyzer_CompilationWithoutExceptionType_DoesNotThrow()
    {
        var analyzer = new AttributeValidationAnalyzer();
        var compilation = CSharpCompilation.Create(
            "EmptyCompilation",
            new[] { CSharpSyntaxTree.ParseText("[assembly: EricksonLopez.DomainPrimitives.DomainPrimitivesDefaults]") },
            null,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var compilationWithAnalyzers = compilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(analyzer));
        var diags = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync();
        diags.Should().NotBeNull();
    }

    [Fact]
    public async Task AttributeValidationAnalyzer_InheritedExceptionWithMultipleCtors_ProducesNoDiagnostic()
    {
        var analyzer = new AttributeValidationAnalyzer();
        var src = @"
[assembly: DomainPrimitivesDefaults(ExceptionType = typeof(GrandChildException))]

public class BaseCustomException : Exception
{
    public BaseCustomException() { }
    public BaseCustomException(string msg) : base(msg) { }
}

public class GrandChildException : BaseCustomException
{
    public GrandChildException(string msg) : base(msg) { }
}
";
        var diags = await GetDiagnosticsAsync(analyzer, src);
        diags.Should().NotContain(d => d.Id == "DP0017");
    }

    [Fact]
    public async Task ValueObjectAnalyzer_InvalidSyntaxOrNullSymbol_HandledSafely()
    {
        var analyzer = new ValueObjectAnalyzer();
        var src = @"
[ValueObject]
public readonly partial record struct IncompleteVo
{
    public string Name { get; set; }
}
";
        var diags = await GetDiagnosticsAsync(analyzer, src);
        diags.Should().Contain(d => d.Id == "DP0008");
    }

    [Fact]
    public async Task StringComparisonAnalyzer_WithNonStringTypeVariableAssignedToPrimitive_DoesNotTriggerDP0011()
    {
        var analyzer = new StringComparisonAnalyzer();
        var src = @"

[StringPrimitive]
public readonly partial record struct Email : IDomainPrimitive<Email, string>
{
    public string Value { get; init; }
    public static implicit operator int(Email e) => 42;
}

public class Consumer
{
    private int fieldVal = Email.Create(""test"");

    public void Run()
    {
        int localVal = Email.Create(""test"");
        object objVal = Email.Create(""test"");
    }
}
";
        var diags = await GetDiagnosticsAsync(analyzer, src);
        diags.Should().NotContain(d => d.Id == "DP0011");
    }

    [Fact]
    public async Task StringComparisonAnalyzer_WithFakeInterfaceInOtherNamespace_DoesNotTriggerDP0010()
    {
        var analyzer = new StringComparisonAnalyzer();
        var src = @"
namespace OtherNamespace
{
    public interface IDomainPrimitive { }
}

public struct FakePrim : OtherNamespace.IDomainPrimitive
{
    public static implicit operator string(FakePrim f) => """";
}

public class Consumer
{
    public void Run(FakePrim f)
    {
        string s = """";
        _ = s == f;
    }
}
";
        var diags = await GetDiagnosticsAsync(analyzer, src);
        diags.Should().NotContain(d => d.Id == "DP0010");
    }

    [Fact]
    public async Task AttributeValidationAnalyzer_CompilationWithoutSystemException_ReturnsSafely()
    {
        var analyzer = new AttributeValidationAnalyzer();
        var src = @"
namespace EricksonLopez.DomainPrimitives
{
    public class DomainPrimitivesDefaultsAttribute : System.Attribute { public System.Type ExceptionType { get; set; } }
}
[assembly: EricksonLopez.DomainPrimitives.DomainPrimitivesDefaults(ExceptionType = typeof(string))]
";
        var syntaxTree = CSharpSyntaxTree.ParseText(src);
        var compilation = CSharpCompilation.Create("NoExceptionProj", new[] { syntaxTree }, null, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var compilationWithAnalyzers = compilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(analyzer));
        var diags = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync();
        diags.Should().BeEmpty();
    }
}






