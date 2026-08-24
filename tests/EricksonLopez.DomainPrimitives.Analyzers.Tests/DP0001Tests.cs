// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.DomainPrimitives.Analyzers;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

using CSharpCodeFixTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixTest<
    EricksonLopez.DomainPrimitives.Analyzers.StructDeclarationAnalyzer,
    EricksonLopez.DomainPrimitives.Analyzers.StructDeclarationCodeFixProvider,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace EricksonLopez.DomainPrimitives.Analyzers.Tests;

public class DP0001Tests
{
    private const string AttributeCode = RoslynTestSnippets.BaseAttributes;

    [Fact]
    public async Task MissingPartial_TriggersDiagnostic_AndCodeFixApplies()
    {
        var testCode = @"
" + AttributeCode + @"
[StrongId<Guid>]
public readonly record struct {|DP0001:UserId|} { }
";

        var fixedCode = @"
" + AttributeCode + @"
[StrongId<Guid>]
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
    public async Task AliasAttribute_TriggersDiagnostic()
    {
        var testCode = @"
" + AttributeCode + @"
[global::EricksonLopez.DomainPrimitives.StrongId<Guid>]
public readonly record struct {|DP0001:UserId|} { }
";

        var fixedCode = @"
" + AttributeCode + @"
[global::EricksonLopez.DomainPrimitives.StrongId<Guid>]
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
    public async Task MultipleAttributes_SecondMatches_TriggersDiagnostic()
    {
        var testCode = @"
" + AttributeCode + @"
[System.Serializable, StrongId<Guid>]
public readonly record struct {|DP0001:UserId|} { }
";

        var fixedCode = @"
" + AttributeCode + @"
[System.Serializable, StrongId<Guid>]
public readonly partial record struct UserId { }
";

        var test = new CSharpCodeFixTest
        {
            TestCode = testCode,
            FixedCode = fixedCode,
        };

        await test.RunAsync();
    }
}





