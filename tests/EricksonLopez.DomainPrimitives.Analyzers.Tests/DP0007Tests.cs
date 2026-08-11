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
    EricksonLopez.DomainPrimitives.Analyzers.PrimitiveUsageAnalyzer,
    Microsoft.CodeAnalysis.Testing.Verifiers.XUnitVerifier>;

namespace EricksonLopez.DomainPrimitives.Analyzers.Tests;

public class DP0007Tests
{
    private const string AttributeCode = @"
    using System;
    using EricksonLopez.DomainPrimitives;

    namespace EricksonLopez.DomainPrimitives
    {
    public class StringPrimitiveAttribute : System.Attribute {}
    public interface IDomainPrimitive {}
}
";

    [Fact]
    public async Task DefaultExpression_TriggersDP0007()
    {
        var testCode = @"
using EricksonLopez.DomainPrimitives;

" + AttributeCode + @"
[StringPrimitive]
public readonly partial record struct UserId : IDomainPrimitive { }

public class TestClass
{
    public void DoSomething()
    {
        var x = {|DP0007:default(UserId)|};
    }
}
";

        var test = new CSharpAnalyzerTest { TestCode = testCode };
        await test.RunAsync();
    }

    [Fact]
    public async Task NewExpression_TriggersDP0007()
    {
        var testCode = @"

" + AttributeCode + @"
[StringPrimitive]
public readonly partial record struct UserId : IDomainPrimitive { }

public class TestClass
{
    public void DoSomething()
    {
        var x = {|DP0007:new UserId()|};
    }
}
";

        var test = new CSharpAnalyzerTest { TestCode = testCode };
        await test.RunAsync();
    }

    [Fact]
    public async Task ImplicitNewExpression_TriggersDP0007()
    {
        var testCode = @"

" + AttributeCode + @"
[StringPrimitive]
public readonly partial record struct UserId : IDomainPrimitive { }

public class TestClass
{
    public void DoSomething()
    {
        UserId x = {|DP0007:new()|};
    }
}
";

        var test = new CSharpAnalyzerTest { TestCode = testCode };
        await test.RunAsync();
    }
    [Fact]
    public async Task DefaultLiteralExpression_TriggersDP0007()
    {
        var testCode = @"
" + AttributeCode + @"
[StringPrimitive]
public readonly partial record struct UserId : IDomainPrimitive { }

public class TestClass
{
    public void DoSomething()
    {
        UserId x = {|DP0007:default|};
    }
}
";
        await new CSharpAnalyzerTest { TestCode = testCode }.RunAsync();
    }

    [Fact]
    public async Task NewExpressionWithArgs_DoesNotTriggerDP0007()
    {
        var testCode = @"
" + AttributeCode + @"
[StringPrimitive]
public readonly partial record struct UserId : IDomainPrimitive 
{ 
    public UserId(string value) { }
}

public class TestClass
{
    public void DoSomething()
    {
        var x = new UserId(""test"");
        UserId y = new(""test"");
    }
}
";
        await new CSharpAnalyzerTest { TestCode = testCode }.RunAsync();
    }

    [Fact]
    public async Task NonPrimitiveDefault_DoesNotTriggerDP0007()
    {
        var testCode = @"
public class TestClass
{
    public void DoSomething()
    {
        var x = default(int);
        int y = default;
    }
}
";
        await new CSharpAnalyzerTest { TestCode = testCode }.RunAsync();
    }
}
