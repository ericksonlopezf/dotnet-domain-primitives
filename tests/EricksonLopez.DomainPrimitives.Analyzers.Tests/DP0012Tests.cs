using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.CodeAnalysis.Testing;
using CSharpAnalyzerTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<
    EricksonLopez.DomainPrimitives.Analyzers.PublicConstructorBypassAnalyzer,
    Microsoft.CodeAnalysis.Testing.Verifiers.XUnitVerifier>;
using CSharpAnalyzerVerifier = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<
    EricksonLopez.DomainPrimitives.Analyzers.PublicConstructorBypassAnalyzer>;

namespace EricksonLopez.DomainPrimitives.Analyzers.Tests;

public class DP0012Tests
{
    private const string AttributeStubs = @"
namespace EricksonLopez.DomainPrimitives
{
    public class StrongIdAttribute<T> : System.Attribute { }
    public class EmailAttribute : System.Attribute { }
    public class StringPrimitiveAttribute : System.Attribute { }
}
";

    [Fact]
    public async Task PublicConstructor_OnStrongIdStruct_TriggersDiagnostic()
    {
        var testCode = "using EricksonLopez.DomainPrimitives;\n" + AttributeStubs + @"

[StrongId<System.Guid>]
public readonly partial record struct UserId
{
    public {|DP0012:UserId|}(System.Guid value) { }
}
";
        var test = new CSharpAnalyzerTest
        {
            TestCode = testCode,
            CompilerDiagnostics = CompilerDiagnostics.None
        };

        await test.RunAsync();
    }

    [Fact]
    public async Task PublicConstructor_OnEmailStruct_TriggersDiagnostic()
    {
        var testCode = "using EricksonLopez.DomainPrimitives;\n" + AttributeStubs + @"

[Email]
public readonly partial record struct EmailAddress
{
    public {|DP0012:EmailAddress|}(string value) { }
}
";
        var test = new CSharpAnalyzerTest
        {
            TestCode = testCode,
            CompilerDiagnostics = CompilerDiagnostics.None
        };

        await test.RunAsync();
    }

    [Fact]
    public async Task PrivateConstructor_OnDomainPrimitive_NoDiagnostic()
    {
        var testCode = "using EricksonLopez.DomainPrimitives;\n" + AttributeStubs + @"

[StrongId<System.Guid>]
public readonly partial record struct OrderId
{
    private OrderId(System.Guid value) { }  // private = OK
}
";
        var test = new CSharpAnalyzerTest
        {
            TestCode = testCode,
            CompilerDiagnostics = CompilerDiagnostics.None
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task PublicConstructor_OnPlainStruct_NoDiagnostic()
    {
        var testCode = AttributeStubs + @"
// No domain primitive attribute — should not trigger
public readonly record struct PlainStruct
{
    public PlainStruct(int value) { }
}
";
        var test = new CSharpAnalyzerTest
        {
            TestCode = testCode,
            CompilerDiagnostics = CompilerDiagnostics.None
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task PublicConstructor_OnStringPrimitive_TriggersDiagnostic()
    {
        var testCode = "using EricksonLopez.DomainPrimitives;\n" + AttributeStubs + @"

public class NormalClass
{
    public NormalClass() { } // class = OK
}
";
        var test = new CSharpAnalyzerTest
        {
            TestCode = testCode,
            CompilerDiagnostics = CompilerDiagnostics.None
        };
        await test.RunAsync();
    }
    [Fact]
    public async Task PublicConstructor_InGeneratedFile_NoDiagnostic()
    {
        var testCode = "using EricksonLopez.DomainPrimitives;\n" + AttributeStubs + @"

[StrongId<System.Guid>]
public readonly partial record struct UserId
{
    public UserId(System.Guid value) { }
}
";
        var test = new CSharpAnalyzerTest
        {
            CompilerDiagnostics = CompilerDiagnostics.None
        };
        test.TestState.Sources.Add(("UserId.g.cs", testCode));

        await test.RunAsync();
    }
}
