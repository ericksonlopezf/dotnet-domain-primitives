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
    EricksonLopez.DomainPrimitives.Analyzers.ValueObjectAnalyzer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace EricksonLopez.DomainPrimitives.Analyzers.Tests;

public class DP0008Tests
{
    private const string AttributeCode = RoslynTestSnippets.BaseAttributes;

    [Fact]
    public async Task ValueObject_WithoutInitOnlyProperties_TriggersDP0008()
    {
        var testCode = @"
using EricksonLopez.DomainPrimitives;

" + AttributeCode + @"
[ValueObject]
public partial record struct Address 
{
    public string {|DP0008:Street|} { get; set; }
}
";

        var test = new CSharpAnalyzerTest { TestCode = testCode };
        await test.RunAsync();
    }

    [Fact]
    public async Task ValueObject_WithInitOnlyProperties_DoesNotTriggerDP0008()
    {
        var testCode = @"
" + AttributeCode + @"
[ValueObject]
public readonly partial record struct Address 
{
    public string Street { get; init; }
    public string City { get; }
}
namespace System.Runtime.CompilerServices { public class IsExternalInit {} }
";

        var test = new CSharpAnalyzerTest { TestCode = testCode };
        await test.RunAsync();
    }

    [Fact]
    public async Task ValueObject_Class_IsIgnored()
    {
        var testCode = @"
" + AttributeCode + @"
[ValueObject]
public class Address 
{
    public string Street { get; set; }
}
";
        await new CSharpAnalyzerTest { TestCode = testCode }.RunAsync();
    }

    [Fact]
    public async Task ValueObject_Struct_IsIgnored()
    {
        var testCode = @"
" + AttributeCode + @"
[ValueObject]
public struct Address 
{
    public string Street { get; set; }
}
";
        await new CSharpAnalyzerTest { TestCode = testCode }.RunAsync();
    }

    [Fact]
    public async Task ValueObject_StaticProperty_IsIgnored()
    {
        var testCode = @"
" + AttributeCode + @"
[ValueObject]
public partial record struct Address 
{
    public static string Street { get; set; }
}
";
        await new CSharpAnalyzerTest { TestCode = testCode }.RunAsync();
    }

    [Fact]
    public async Task ValueObject_PrivateProperty_IsIgnored()
    {
        var testCode = @"
" + AttributeCode + @"
[ValueObject]
public partial record struct Address 
{
    private string Street { get; set; }
}
";
        await new CSharpAnalyzerTest { TestCode = testCode }.RunAsync();
    }

    [Fact]
    public async Task ValueObject_ExpressionBodiedProperty_IsIgnored()
    {
        var testCode = @"
" + AttributeCode + @"
[ValueObject]
public partial record struct Address 
{
    public string Street => ""Test"";
}
";
        await new CSharpAnalyzerTest { TestCode = testCode }.RunAsync();
    }

    [Fact]
    public async Task NonValueObject_RecordStruct_WithSet_IsIgnored()
    {
        var testCode = @"
" + AttributeCode + @"
public partial record struct NonVoAddress 
{
    public string Street { get; set; }
}
";
        await new CSharpAnalyzerTest { TestCode = testCode }.RunAsync();
    }

    [Fact]
    public async Task ValueObjectAttribute_WithSuffix_TriggersDP0008()
    {
        var testCode = @"
" + AttributeCode + @"
[ValueObjectAttribute]
public partial record struct AddressWithSuffix 
{
    public string {|DP0008:Street|} { get; set; }
}
";
        await new CSharpAnalyzerTest { TestCode = testCode }.RunAsync();
    }
}





