using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using EricksonLopez.DomainPrimitives.Analyzers;
using Microsoft.CodeAnalysis.Testing;
using CSharpAnalyzerTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<
    EricksonLopez.DomainPrimitives.Analyzers.MissingValidationAnalyzer,
    Microsoft.CodeAnalysis.Testing.Verifiers.XUnitVerifier>;

namespace EricksonLopez.DomainPrimitives.Analyzers.Tests;

public class DP0009Tests
{
    private const string AttributeCode = @"
namespace EricksonLopez.DomainPrimitives
{
    public class StringPrimitiveAttribute : System.Attribute {}
    public class NumericPrimitiveAttribute<T> : System.Attribute {}
    public class EmailAttribute : System.Attribute {}
}
namespace EricksonLopez.DomainPrimitives.Validation
{
    public class NotEmptyAttribute : System.Attribute {}
}
";

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
}
