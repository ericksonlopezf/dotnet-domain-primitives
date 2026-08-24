// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.DomainPrimitives.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

using CSharpAnalyzerTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<
    EricksonLopez.DomainPrimitives.Analyzers.MissingValidationAnalyzer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;
using CSharpCodeFixTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixTest<
    EricksonLopez.DomainPrimitives.Analyzers.MissingValidationAnalyzer,
    EricksonLopez.DomainPrimitives.Analyzers.MissingValidationCodeFixProvider,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace EricksonLopez.DomainPrimitives.Analyzers.Tests;

public class DP0009Tests
{
    private const string AttributeCode = RoslynTestSnippets.AllAttributes;

    [Fact]
    public async Task PrimitiveWithoutValidation_TriggersDP0009()
    {
        var testCode = @"
using EricksonLopez.DomainPrimitives;

" + AttributeCode + @"
[StringPrimitive]
public readonly partial record struct {|DP0009:UserId|} { }
";

        var test = new CSharpAnalyzerTest
        {
            TestCode = testCode,
        };

        await test.RunAsync();
    }

    [Fact]
    public async Task PrimitiveWithValidation_DoesNotTriggerDP0009()
    {
        var testCode = @"
using EricksonLopez.DomainPrimitives.Validation;

" + AttributeCode + @"
[StringPrimitive]
[NotEmpty]
public readonly partial record struct UserId { }
";

        var test = new CSharpAnalyzerTest
        {
            TestCode = testCode,
        };

        await test.RunAsync();
    }

    [Fact]
    public async Task NumericPrimitiveWithoutValidation_TriggersDP0009()
    {
        var testCode = @"

" + AttributeCode + @"
[NumericPrimitive<int>]
public readonly partial record struct {|DP0009:UserId|} { }
";

        var test = new CSharpAnalyzerTest
        {
            TestCode = testCode,
        };

        await test.RunAsync();
    }

    [Fact]
    public async Task GenericNumericStringWithoutValidation_TriggersOnlyStandardDP0009()
    {
        var testCode = AttributeCode + @"
[NumericPrimitive<string>]
public readonly partial record struct {|#0:StrNum|} { }
";
        var test = new CSharpAnalyzerTest { TestCode = testCode };
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult("DP0009", DiagnosticSeverity.Warning)
                .WithLocation(0)
                .WithArguments("StrNum"));
        await test.RunAsync();
    }

    [Fact]
    public async Task PrimitiveWithShortcut_DoesNotTriggerDP0009()
    {
        var testCode = @"

" + AttributeCode + @"
[StringPrimitive]
[Email]
public readonly partial record struct UserId { }
";

        var test = new CSharpAnalyzerTest
        {
            TestCode = testCode,
        };

        await test.RunAsync();
    }
    [Fact]
    public async Task PrimitiveWithoutValidation_AppliesCodeFix()
    {
        var testCode = @"

" + AttributeCode + @"
[StringPrimitive]
public readonly partial record struct {|DP0009:UserId|} { }
";

        var fixedCode = @"

" + AttributeCode + @"
[StringPrimitive]
[NotEmpty]
public readonly partial record struct UserId { }
";

        var test = new CSharpCodeFixTest
        {
            TestCode = testCode,
            FixedCode = fixedCode,
        };

        await test.RunAsync();
    }

    [Fact]
    public async Task DatePrimitiveWithoutValidation_TriggersDP0009()
    {
        var testCode = AttributeCode + @"
[DatePrimitive]
public readonly partial record struct {|DP0009:BirthDate|} { }
";
        await new CSharpAnalyzerTest { TestCode = testCode }.RunAsync();
    }

    [Fact]
    public async Task DatePrimitiveWithValidation_DoesNotTriggerDP0009()
    {
        var testCode = AttributeCode + @"
[DatePrimitive(PastOnly = true)]
public readonly partial record struct BirthDate { }
";
        await new CSharpAnalyzerTest { TestCode = testCode }.RunAsync();
    }

    [Fact]
    public async Task StrongIdStringWithoutValidation_TriggersDP0009()
    {
        var testCode = AttributeCode + @"
[StrongId<string>]
public readonly partial record struct {|#0:UserId|} { }
";
        var test = new CSharpAnalyzerTest { TestCode = testCode };
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult("DP0009", DiagnosticSeverity.Warning)
                .WithLocation(0)
                .WithArguments("UserId (StrongId<string> without length or format constraints — add [MinLength], [MaxLength], or [Regex])"));
        await test.RunAsync();
    }

    [Fact]
    public async Task StrongIdStringWithValidation_DoesNotTriggerDP0009()
    {
        var testCode = AttributeCode + @"
[StrongId<string>]
[MinLength(1)]
public readonly partial record struct UserId { }
";
        await new CSharpAnalyzerTest { TestCode = testCode }.RunAsync();
    }

    [Fact]
    public async Task StrongIdInt_DoesNotTriggerDP0009()
    {
        var testCode = AttributeCode + @"
[StrongId<int>]
public readonly partial record struct UserId { }
";
        await new CSharpAnalyzerTest { TestCode = testCode }.RunAsync();
    }

    [Fact]
    public async Task NoAttributes_IsIgnored()
    {
        var testCode = @"
public readonly partial record struct UserId { }
";
        await new CSharpAnalyzerTest { TestCode = testCode }.RunAsync();
    }

    [Fact]
    public async Task NonDomainAttribute_IsIgnored()
    {
        var testCode = @"
using System;

[Serializable]
public readonly partial record struct UserId { }
";
        await new CSharpAnalyzerTest { TestCode = testCode }.RunAsync();
    }

    [Fact]
    public async Task DifferentNamespaceAttribute_IsIgnored()
    {
        var testCode = @"
namespace SomeOtherNamespace
{
    public class StringPrimitiveAttribute : System.Attribute {}
}
[SomeOtherNamespace.StringPrimitive]
public readonly partial record struct UserId { }
";
        await new CSharpAnalyzerTest { TestCode = testCode }.RunAsync();
    }

    [Fact]
    public async Task MultipleAttributes_IsCandidate_HitsBreak()
    {
        var testCode = AttributeCode + @"
[System.Serializable, StringPrimitive, NotEmpty]
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public readonly partial record struct UserId { }
";
        await new CSharpAnalyzerTest { TestCode = testCode }.RunAsync();
    }
    [Fact]
    public async Task DatePrimitive_NoValidation_TriggersDP0009()
    {
        var testCode = @"

" + AttributeCode + @"
[DatePrimitive]
public readonly partial record struct {|DP0009:Date1|} { }

[DatePrimitive(PastOnly = false)]
public readonly partial record struct {|DP0009:Date2|} { }

[DatePrimitive(FutureOnly = false)]
public readonly partial record struct {|DP0009:Date3|} { }

[DatePrimitive(PastOnly = true)]
public readonly partial record struct Date4 { }

[DatePrimitive(FutureOnly = true)]
public readonly partial record struct Date5 { }

[DatePrimitive(Format = ""yyyy"")]
public readonly partial record struct {|DP0009:Date6|} { }

[DatePrimitive(PastOnly = 1)]
public readonly partial record struct {|DP0009:Date7|} { }
";
        var test = new CSharpAnalyzerTest { 
            TestCode = testCode, 
            CompilerDiagnostics = CompilerDiagnostics.None // Ignore CS0029 Cannot implicitly convert type 'int' to 'bool'
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task GlobalNamespaceAttribute_IsIgnored()
    {
        var testCode = @"
using System;

public class StringPrimitiveAttribute : Attribute {}

[StringPrimitive]
public readonly partial record struct Date1 { }
";
        await new CSharpAnalyzerTest { TestCode = testCode }.RunAsync();
    }
}





