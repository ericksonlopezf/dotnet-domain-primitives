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
    EricksonLopez.DomainPrimitives.Analyzers.ValueObjectAnalyzer,
    Microsoft.CodeAnalysis.Testing.Verifiers.XUnitVerifier>;

namespace EricksonLopez.DomainPrimitives.Analyzers.Tests;

public class DP0008Tests
{
    private const string AttributeCode = @"
namespace EricksonLopez.DomainPrimitives
{
    public class ValueObjectAttribute : System.Attribute {}
}
";

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
namespace System.Runtime.CompilerServices { public class IsExternalInit {} }

" + AttributeCode + @"
[ValueObject]
public readonly partial record struct Address 
{
    public string Street { get; init; }
    public string City { get; }
}
";

        var test = new CSharpAnalyzerTest { TestCode = testCode };
        await test.RunAsync();
    }
}
