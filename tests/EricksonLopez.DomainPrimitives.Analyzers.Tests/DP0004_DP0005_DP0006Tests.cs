using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using EricksonLopez.DomainPrimitives.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using CSharpAnalyzerTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<
    EricksonLopez.DomainPrimitives.Analyzers.AttributeValidationAnalyzer,
    Microsoft.CodeAnalysis.Testing.Verifiers.XUnitVerifier>;

namespace EricksonLopez.DomainPrimitives.Analyzers.Tests;

public class DP0004_DP0005_DP0006Tests
{
    private const string AttributeCode = @"
namespace EricksonLopez.DomainPrimitives.Validation
{
    public class RegexAttribute : System.Attribute { public RegexAttribute(string pattern) {} }
    public class MinLengthAttribute : System.Attribute { public MinLengthAttribute(int len) {} }
    public class MaxLengthAttribute : System.Attribute { public MaxLengthAttribute(int len) {} }
    public class LengthAttribute : System.Attribute { public LengthAttribute(int min, int max) {} }
    public class RangeAttribute : System.Attribute { 
        public RangeAttribute(double min, double max) {} 
        public RangeAttribute(int min, int max) {} 
        public RangeAttribute(string min, string max) {} 
    }
}
namespace EricksonLopez.DomainPrimitives.Normalization
{
    public class LowerCaseAttribute : System.Attribute {}
    public class UpperCaseAttribute : System.Attribute {}
    public class TrimAttribute : System.Attribute {}
    public class TrimStartAttribute : System.Attribute {}
}
";

    [Fact]
    public async Task InvalidRegex_TriggersDP0004()
    {
        var testCode = @"
using EricksonLopez.DomainPrimitives.Validation;

" + AttributeCode + @"
[Regex(""["")]
public readonly partial record struct UserId { }
";

        var test = new CSharpAnalyzerTest { TestCode = testCode };
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult("DP0004", DiagnosticSeverity.Error)
                .WithLocation(25, 2)
                .WithArguments("[", "Invalid pattern '[' at offset 1. Unterminated [] set.")
        );
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
[MinLength(10), MaxLength(5)]
public readonly partial record struct UserId { }
";

        var test = new CSharpAnalyzerTest { TestCode = testCode };
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult("DP0006", DiagnosticSeverity.Error)
                .WithLocation(25, 2)
                .WithArguments("10", "5")
        );
        await test.RunAsync();
    }

    [Fact]
    public async Task InvalidBounds_Length_TriggersDP0006()
    {
        var testCode = @"

" + AttributeCode + @"
[Length(10, 5)]
public readonly partial record struct UserId { }
";

        var test = new CSharpAnalyzerTest { TestCode = testCode };
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult("DP0006", DiagnosticSeverity.Error)
                .WithLocation(25, 2)
                .WithArguments("10", "5")
        );
        await test.RunAsync();
    }

    [Fact]
    public async Task InvalidBounds_RangeInt_TriggersDP0006()
    {
        var testCode = @"

" + AttributeCode + @"
[Range(10, 5)]
public readonly partial record struct UserId { }
";

        var test = new CSharpAnalyzerTest { TestCode = testCode };
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult("DP0006", DiagnosticSeverity.Error)
                .WithLocation(25, 2)
                .WithArguments("10", "5")
        );
        await test.RunAsync();
    }

    [Fact]
    public async Task InvalidBounds_RangeDouble_TriggersDP0006()
    {
        var testCode = @"

" + AttributeCode + @"
[Range(10.0, 5.0)]
public readonly partial record struct UserId { }
";

        var test = new CSharpAnalyzerTest { TestCode = testCode };
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult("DP0006", DiagnosticSeverity.Error)
                .WithLocation(25, 2)
                .WithArguments("10", "5")
        );
        await test.RunAsync();
    }

    [Fact]
    public async Task InvalidBounds_RangeString_TriggersDP0006()
    {
        var testCode = @"

" + AttributeCode + @"
[Range(""10.0"", ""5.0"")]
public readonly partial record struct UserId { }
";

        var test = new CSharpAnalyzerTest { TestCode = testCode };
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult("DP0006", DiagnosticSeverity.Error)
                .WithLocation(25, 2)
                .WithArguments("10.0", "5.0")
        );
        await test.RunAsync();
    }
}
