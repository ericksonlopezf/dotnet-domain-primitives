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
    EricksonLopez.DomainPrimitives.Analyzers.AttributeValidationAnalyzer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace EricksonLopez.DomainPrimitives.Analyzers.Tests;

public class DP0004_DP0005_DP0006Tests
{
    private const string AttributeCode = RoslynTestSnippets.AllAttributes;

    [Fact]
    public async Task InvalidRegex_TriggersDP0004()
    {
        var testCode = @"
using EricksonLopez.DomainPrimitives.Validation;

" + AttributeCode + @"
[{|#0:Regex(""["")|}]
public readonly partial record struct UserId { }
";

        var test = new CSharpAnalyzerTest { TestCode = testCode };
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult("DP0004", DiagnosticSeverity.Error)
                .WithLocation(0)
                .WithArguments("[", "Invalid pattern '[' at offset 1. Unterminated [] set.")
        );
        await test.RunAsync();
    }

    [Fact]
    public async Task ValidRegex_DoesNotTriggerDP0004()
    {
        var testCode = @"

" + AttributeCode + @"
[Regex(""^[A-Z]+$"")]
public readonly partial record struct UserId { }
";

        var test = new CSharpAnalyzerTest { TestCode = testCode };
        await test.RunAsync();
    }

    [Fact]
    public async Task ConflictingNormalization_TriggersDP0005()
    {
        var testCode = @"
using EricksonLopez.DomainPrimitives.Normalization;

" + AttributeCode + @"
[LowerCase, UpperCase]
public readonly partial record struct {|DP0005:UserId|} { }
";

        var test = new CSharpAnalyzerTest { TestCode = testCode };
        await test.RunAsync();
    }

    [Fact]
    public async Task InvalidBounds_TriggersDP0006()
    {
        var testCode = @"

" + AttributeCode + @"
[{|#0:MinLength(10)|}]
[MaxLength(5)]
public readonly partial record struct UserId { }
";

        var test = new CSharpAnalyzerTest { TestCode = testCode };
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult("DP0006", DiagnosticSeverity.Error)
                .WithLocation(0)
                .WithArguments("10", "5")
        );
        await test.RunAsync();
    }

    [Fact]
    public async Task InvalidBounds_MaxLengthFirst_TriggersDP0006()
    {
        var testCode = @"

" + AttributeCode + @"
[{|#0:MaxLength(5)|}]
[MinLength(10)]
public readonly partial record struct UserId { }
";

        var test = new CSharpAnalyzerTest { TestCode = testCode };
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult("DP0006", DiagnosticSeverity.Error)
                .WithLocation(0)
                .WithArguments("10", "5")
        );
        await test.RunAsync();
    }

    [Fact]
    public async Task InvalidBounds_Length_TriggersDP0006()
    {
        var testCode = @"

" + AttributeCode + @"
[{|#0:Length(10, 5)|}]
public readonly partial record struct UserId { }
";

        var test = new CSharpAnalyzerTest { TestCode = testCode };
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult("DP0006", DiagnosticSeverity.Error)
                .WithLocation(0)
                .WithArguments("10", "5")
        );
        await test.RunAsync();
    }

    [Fact]
    public async Task InvalidBounds_RangeInt_TriggersDP0006()
    {
        var testCode = @"

" + AttributeCode + @"
[{|#0:Range(10, 5)|}]
public readonly partial record struct UserId { }
";

        var test = new CSharpAnalyzerTest { TestCode = testCode };
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult("DP0006", DiagnosticSeverity.Error)
                .WithLocation(0)
                .WithArguments("10", "5")
        );
        await test.RunAsync();
    }

    [Fact]
    public async Task InvalidBounds_RangeDouble_TriggersDP0006()
    {
        var testCode = @"

" + AttributeCode + @"
[{|#0:Range(10.0, 5.0)|}]
public readonly partial record struct UserId { }
";

        var test = new CSharpAnalyzerTest { TestCode = testCode };
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult("DP0006", DiagnosticSeverity.Error)
                .WithLocation(0)
                .WithArguments("10", "5")
        );
        await test.RunAsync();
    }

    [Fact]
    public async Task InvalidBounds_RangeString_TriggersDP0006()
    {
        var testCode = @"

" + AttributeCode + @"
[{|#0:Range(""10.0"", ""5.0"")|}]
public readonly partial record struct UserId { }
";

        var test = new CSharpAnalyzerTest { TestCode = testCode };
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult("DP0006", DiagnosticSeverity.Error)
                .WithLocation(0)
                .WithArguments("10.0", "5.0")
        );
        await test.RunAsync();
    }

    [Fact]
    public async Task ValidBounds_EqualBounds_DoesNotTriggerDP0006()
    {
        var testCode = @"

" + AttributeCode + @"
[MinLength(5), MaxLength(5)]
public readonly partial record struct EqualMinMaxId { }

[Length(5, 5)]
public readonly partial record struct EqualLengthId { }

[Range(5, 5)]
public readonly partial record struct EqualRangeIntId { }

[Range(5.0, 5.0)]
public readonly partial record struct EqualRangeDoubleId { }

[Range(""5.0"", ""5.0"")]
public readonly partial record struct EqualRangeStringId { }

[Range(""abc"", ""def"")]
public readonly partial record struct NonDecimalRangeStringId { }

[LowerCase]
public readonly partial record struct SingleLowerId { }

[UpperCase]
public readonly partial record struct SingleUpperId { }

[MinLength(10)]
public readonly partial record struct SingleMinOnlyId { }

[MaxLength(5)]
public readonly partial record struct SingleMaxOnlyId { }

[Range(1, 10)]
public readonly partial record struct ValidRangeIntId { }

[Range(1.0, 10.0)]
public readonly partial record struct ValidRangeDoubleId { }

[Range(""1.0"", ""10.0"")]
public readonly partial record struct ValidRangeStringId { }
";

        var test = new CSharpAnalyzerTest { TestCode = testCode };
        await test.RunAsync();
    }

    [Fact]
    public async Task ZeroArgumentAttributes_DoNotThrowOrTriggerDiagnostics()
    {
        var testCode = @"

" + AttributeCode + @"
[Regex]
[MinLength]
[MaxLength]
[Length]
[Range]
public readonly partial record struct ZeroArgsId { }
";

        var test = new CSharpAnalyzerTest { TestCode = testCode };
        await test.RunAsync();
    }
}





