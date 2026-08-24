// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

using CSharpAnalyzerTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<
    EricksonLopez.DomainPrimitives.Analyzers.PublicConstructorBypassAnalyzer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;
using CSharpAnalyzerVerifier = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<
    EricksonLopez.DomainPrimitives.Analyzers.PublicConstructorBypassAnalyzer>;

namespace EricksonLopez.DomainPrimitives.Analyzers.Tests;

public class DP0012Tests
{
    private const string AttributeStubs = RoslynTestSnippets.BaseAttributes;

    [Fact]
    public async Task PublicConstructor_OnStrongIdStruct_TriggersDiagnostic()
    {
        var testCode = "using EricksonLopez.DomainPrimitives;\n" + AttributeStubs + @"

[StrongId<Guid>]
public readonly partial record struct UserId
{
    public {|DP0012:UserId|}(Guid value) { }
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

[StrongId<Guid>]
public readonly partial record struct OrderId
{
    private OrderId(Guid value) { }  // private = OK
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

[StringPrimitive]
public readonly partial record struct Nickname
{
    public {|DP0012:Nickname|}(string value) { }
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
    public async Task PublicConstructor_OnClassWithDomainPrimitive_NoDiagnostic()
    {
        var testCode = "using EricksonLopez.DomainPrimitives;\n" + AttributeStubs + @"

[Email]
public class RegularClassWithEmail
{
    public RegularClassWithEmail(string value) { }
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
    public async Task PublicConstructor_OnNonRecordStructWithDomainPrimitive_NoDiagnostic()
    {
        var testCode = "using EricksonLopez.DomainPrimitives;\n" + AttributeStubs + @"

[Email]
public struct RegularStructWithEmail
{
    public RegularStructWithEmail(string value) { }
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
    public async Task PublicConstructor_OnStructWithNonDomainAttribute_NoDiagnostic()
    {
        var testCode = @"

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public readonly record struct IgnoredStruct
{
    public IgnoredStruct(int value) { }
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
    public async Task ImplicitConstructor_OnDomainPrimitive_NoDiagnostic()
    {
        var testCode = "using EricksonLopez.DomainPrimitives;\n" + AttributeStubs + @"

[Email]
public readonly partial record struct ImplicitEmailAddress;
";
        var test = new CSharpAnalyzerTest
        {
            TestCode = testCode,
            CompilerDiagnostics = CompilerDiagnostics.None
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task ExplicitParameterlessConstructor_OnDomainPrimitive_NoDiagnostic()
    {
        var testCode = "using EricksonLopez.DomainPrimitives;\n" + AttributeStubs + @"

[Email]
public readonly partial record struct EmailWithExplicitDefaultCtor
{
    public EmailWithExplicitDefaultCtor() { }
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

[StrongId<Guid>]
public readonly partial record struct UserId
{
    public UserId(Guid value) { }
}
";
        var test = new CSharpAnalyzerTest
        {
            CompilerDiagnostics = CompilerDiagnostics.None
        };
        test.TestState.Sources.Add(("UserId.g.cs", testCode));

        await test.RunAsync();
    }

    [Fact]
    public async Task PublicConstructor_InGeneratedDotGeneratedFile_NoDiagnostic()
    {
        var testCode = "using EricksonLopez.DomainPrimitives;\n" + AttributeStubs + @"

[StrongId<Guid>]
public readonly partial record struct UserId
{
    public UserId(Guid value) { }
}
";
        var test = new CSharpAnalyzerTest
        {
            CompilerDiagnostics = CompilerDiagnostics.None
        };
        test.TestState.Sources.Add(("UserId.generated.cs", testCode));

        await test.RunAsync();
    }

    [Fact]
    public async Task PublicConstructor_InGeneratedFileWithPartialSourceDeclaration_NoDiagnostic()
    {
        var sourceCode = "using EricksonLopez.DomainPrimitives;\n" + AttributeStubs + @"

[StrongId<Guid>]
public readonly partial record struct OrderId;
";
        var generatedCode = @"
public readonly partial record struct OrderId
{
    public OrderId(Guid value) { }
}
";
        var test = new CSharpAnalyzerTest
        {
            CompilerDiagnostics = CompilerDiagnostics.None
        };
        test.TestState.Sources.Add(("OrderId.cs", sourceCode));
        test.TestState.Sources.Add(("OrderId.g.cs", generatedCode));

        await test.RunAsync();
    }
}





