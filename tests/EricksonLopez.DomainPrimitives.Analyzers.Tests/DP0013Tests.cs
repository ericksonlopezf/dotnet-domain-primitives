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

using CSharpAnalyzerTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<
    EricksonLopez.DomainPrimitives.Analyzers.DuplicatePrimitiveLogicAnalyzer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace EricksonLopez.DomainPrimitives.Analyzers.Tests;

public class DP0013Tests
{
    private const string AttributeCode = RoslynTestSnippets.AllAttributes;

    [Fact]
    public async Task DuplicatePrimitives_TriggersDP0013()
    {
        var testCode = @"
using EricksonLopez.DomainPrimitives;
using EricksonLopez.DomainPrimitives.Validation;

" + AttributeCode + @"
[StringPrimitive]
[Regex(""^[A-Z]+$"")]
public readonly partial record struct {|DP0013:UserId|} { }

[StringPrimitive]
[Regex(""^[A-Z]+$"")]
public readonly partial record struct EmployeeId { }
";

        var test = new CSharpAnalyzerTest { TestCode = testCode };
        await test.RunAsync();
    }

    [Fact]
    public async Task DuplicateId_TriggersDP0013()
    {
        var testCode = @"

" + AttributeCode + @"
[Id]
public readonly partial record struct {|DP0013:UserId|} { }

[Id]
public readonly partial record struct EmployeeId { }
";
        var test = new CSharpAnalyzerTest { TestCode = testCode };
        await test.RunAsync();
    }

    [Fact]
    public async Task DuplicateValueObject_TriggersDP0013()
    {
        var testCode = @"

" + AttributeCode + @"
[ValueObject]
public readonly partial record struct Address { }

[ValueObject]
public readonly partial record struct {|DP0013:Location|} { }
";
        var test = new CSharpAnalyzerTest { TestCode = testCode };
        await test.RunAsync();
    }

    [Fact]
    public async Task DuplicateCodeAttribute_TriggersDP0013()
    {
        var testCode = @"

" + AttributeCode + @"
[MyCode]
public readonly partial record struct CodeA { }

[MyCode]
public readonly partial record struct {|DP0013:CodeB|} { }
";
        var test = new CSharpAnalyzerTest { TestCode = testCode };
        await test.RunAsync();
    }

    [Fact]
    public async Task DuplicateShortcutAttribute_TriggersDP0013()
    {
        var testCode = @"

" + AttributeCode + @"
[Email]
public readonly partial record struct {|DP0013:UserEmail|} { }

[Email]
public readonly partial record struct AdminEmail { }
";
        var test = new CSharpAnalyzerTest { TestCode = testCode };
        await test.RunAsync();
    }

    [Fact]
    public async Task AttributesInDifferentOrder_TriggersDP0013()
    {
        var testCode = @"

" + AttributeCode + @"
[StringPrimitive, MinLength(5)]
public readonly partial record struct OrderA { }

[MinLength(5), StringPrimitive]
public readonly partial record struct {|DP0013:OrderB|} { }
";
        var test = new CSharpAnalyzerTest { TestCode = testCode };
        await test.RunAsync();
    }

    [Fact]
    public async Task NamedArgumentsInDifferentOrder_TriggersDP0013()
    {
        var testCode = @"

" + AttributeCode + @"
[DatePrimitive(PastOnly = true, Format = ""yyyy"")]
public readonly partial record struct DateA { }

[DatePrimitive(Format = ""yyyy"", PastOnly = true)]
public readonly partial record struct {|DP0013:DateB|} { }
";
        var test = new CSharpAnalyzerTest { TestCode = testCode };
        await test.RunAsync();
    }

    [Fact]
    public async Task DistinctAttributeValues_ProducesNoDiagnostics()
    {
        var testCode = @"

" + AttributeCode + @"
[StringPrimitive]
[MinLength(5)]
public readonly partial record struct TypeA { }

[StringPrimitive]
[MinLength(10)]
public readonly partial record struct TypeB { }
";
        var test = new CSharpAnalyzerTest { TestCode = testCode };
        await test.RunAsync();
    }

    [Fact]
    public async Task NullAttributeArgument_ProducesDeterministicSignature()
    {
        var testCode = @"

" + AttributeCode + @"
[Range(null, null)]
public readonly partial record struct RangeA { }

[Range(null, null)]
public readonly partial record struct {|DP0013:RangeB|} { }
";
        var test = new CSharpAnalyzerTest { TestCode = testCode, CompilerDiagnostics = CompilerDiagnostics.None };
        await test.RunAsync();
    }

    [Fact]
    public async Task NonDomainAttributesWithPrimitive_DoesNotPreventDetection()
    {
        var testCode = @"

" + AttributeCode + @"
[Serializable]
[StringPrimitive]
public readonly partial record struct PrimA { }

[StringPrimitive]
public readonly partial record struct {|DP0013:PrimB|} { }
";
        var test = new CSharpAnalyzerTest { TestCode = testCode, CompilerDiagnostics = CompilerDiagnostics.None };
        await test.RunAsync();
    }

    [Fact]
    public async Task UniquePrimitive_WithDuplicates_OnlyDuplicatesReportDP0013()
    {
        var testCode = @"

" + AttributeCode + @"
[StringPrimitive]
public readonly partial record struct Dup1 { }

[StringPrimitive]
public readonly partial record struct {|DP0013:Dup2|} { }

[StringPrimitive]
[MinLength(99)]
public readonly partial record struct UniqueSolo { }
";
        var test = new CSharpAnalyzerTest { TestCode = testCode };
        await test.RunAsync();
    }

    [Fact]
    public async Task DifferentNamespaceAttributes_ProducesNoDiagnostics()
    {
        var testCode = @"

namespace CustomNs
{
    public class StringPrimitiveAttribute : Attribute {}
}

[CustomNs.StringPrimitive]
public readonly partial record struct OtherA { }

[CustomNs.StringPrimitive]
public readonly partial record struct OtherB { }
";
        var test = new CSharpAnalyzerTest { TestCode = testCode, CompilerDiagnostics = CompilerDiagnostics.None };
        await test.RunAsync();
    }

    [Fact]
    public async Task DuplicateRecordClass_DoesNotTriggerDP0013()
    {
        var testCode = @"

" + AttributeCode + @"
[StringPrimitive]
public record class RecordClassA { }

[StringPrimitive]
public record class RecordClassB { }
";
        var test = new CSharpAnalyzerTest { TestCode = testCode, CompilerDiagnostics = CompilerDiagnostics.None };
        await test.RunAsync();
    }
}





