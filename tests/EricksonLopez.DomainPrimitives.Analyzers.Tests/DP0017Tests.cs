// Copyright © Erickson Lopez. MIT License.
using System;
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

public class DP0017Tests
{
    private const string DefaultsAttributeCode = RoslynTestSnippets.DefaultsAttributes;

    [Fact]
    public async Task ValidExceptionType_ProducesNoDiagnostics()
    {
        var testCode = @"
using System;
using EricksonLopez.DomainPrimitives;

[assembly: DomainPrimitivesDefaults(ExceptionType = typeof(MyValidException))]

" + DefaultsAttributeCode + @"

public class MyValidException : Exception
{
    public MyValidException(string message) : base(message) { }
}
";

        var test = new CSharpAnalyzerTest { TestCode = testCode };
        await test.RunAsync();
    }

    [Fact]
    public async Task InvalidExceptionType_NotDerivingFromException_TriggersDP0017()
    {
        var testCode = @"
using System;
using EricksonLopez.DomainPrimitives;

[assembly: DomainPrimitivesDefaults(ExceptionType = typeof(NotAnException))]

" + DefaultsAttributeCode + @"

public class NotAnException
{
    public NotAnException(string message) { }
}
";

        var test = new CSharpAnalyzerTest { TestCode = testCode };
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult("DP0017", DiagnosticSeverity.Error)
                .WithSpan(5, 12, 5, 76)
                .WithArguments("NotAnException"));

        await test.RunAsync();
    }

    [Fact]
    public async Task InvalidExceptionType_MissingStringConstructor_TriggersDP0017()
    {
        var testCode = @"
using System;
using EricksonLopez.DomainPrimitives;

[assembly: DomainPrimitivesDefaults(ExceptionType = typeof(NoStringCtorException))]

" + DefaultsAttributeCode + @"

public class NoStringCtorException : Exception
{
    public NoStringCtorException() { }
}
";

        var test = new CSharpAnalyzerTest { TestCode = testCode };
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult("DP0017", DiagnosticSeverity.Error)
                .WithSpan(5, 12, 5, 83)
                .WithArguments("NoStringCtorException"));

        await test.RunAsync();
    }

    [Fact]
    public async Task InvalidExceptionType_InternalStringConstructor_TriggersDP0017()
    {
        var testCode = @"
using System;
using EricksonLopez.DomainPrimitives;

[assembly: DomainPrimitivesDefaults(ExceptionType = typeof(InternalCtorException))]

" + DefaultsAttributeCode + @"

public class InternalCtorException : Exception
{
    internal InternalCtorException(string message) : base(message) { }
}
";

        var test = new CSharpAnalyzerTest { TestCode = testCode };
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult("DP0017", DiagnosticSeverity.Error)
                .WithSpan(5, 12, 5, 83)
                .WithArguments("InternalCtorException"));

        await test.RunAsync();
    }

    [Fact]
    public async Task InvalidExceptionType_NonStringParameterConstructor_TriggersDP0017()
    {
        var testCode = @"
using System;
using EricksonLopez.DomainPrimitives;

[assembly: DomainPrimitivesDefaults(ExceptionType = typeof(IntParamException))]

" + DefaultsAttributeCode + @"

public class IntParamException : Exception
{
    public IntParamException(int errorCode) { }
}
";

        var test = new CSharpAnalyzerTest { TestCode = testCode };
        test.ExpectedDiagnostics.Add(
            new DiagnosticResult("DP0017", DiagnosticSeverity.Error)
                .WithSpan(5, 12, 5, 79)
                .WithArguments("IntParamException"));

        await test.RunAsync();
    }

    [Fact]
    public async Task ValidExceptionType_DeepInheritanceAndMultipleConstructors_ProducesNoDiagnostics()
    {
        var testCode = @"
using System;
using EricksonLopez.DomainPrimitives;

[assembly: DomainPrimitivesDefaults(ExceptionType = typeof(GrandChildException))]

" + DefaultsAttributeCode + @"

public class ParentException : Exception
{
    public ParentException() { }
    public ParentException(string message) : base(message) { }
}

public class GrandChildException : ParentException
{
    public GrandChildException() { }
    public GrandChildException(int code) { }
    public GrandChildException(string message) : base(message) { }
}
";

        var test = new CSharpAnalyzerTest { TestCode = testCode };
        await test.RunAsync();
    }

    [Fact]
    public async Task AssemblyAttribute_FromOtherNamespace_ProducesNoDiagnostics()
    {
        var testCode = @"
using System;
using OtherNamespace;

[assembly: DomainPrimitivesDefaults(ExceptionType = typeof(string))]

namespace OtherNamespace
{
    [System.AttributeUsage(System.AttributeTargets.Assembly)]
    public class DomainPrimitivesDefaultsAttribute : System.Attribute
    {
        public System.Type ExceptionType { get; set; }
    }
}
";

        var test = new CSharpAnalyzerTest { TestCode = testCode };
        await test.RunAsync();
    }

    [Fact]
    public async Task DefaultsWithOtherNamedArguments_DoesNotTriggerDP0017()
    {
        var testCode = @"
using System;
using EricksonLopez.DomainPrimitives;

[assembly: DomainPrimitivesDefaults(Trim = true, MaxLength = 50)]

" + DefaultsAttributeCode + @"
";

        var test = new CSharpAnalyzerTest { TestCode = testCode };
        await test.RunAsync();
    }
}

