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
    EricksonLopez.DomainPrimitives.Analyzers.DuplicatePrimitiveLogicAnalyzer,
    Microsoft.CodeAnalysis.Testing.Verifiers.XUnitVerifier>;

namespace EricksonLopez.DomainPrimitives.Analyzers.Tests;

public class DP0013Tests
{
    private const string AttributeCode = @"
    using System;
    using EricksonLopez.DomainPrimitives;
    using EricksonLopez.DomainPrimitives.Validation;

    namespace EricksonLopez.DomainPrimitives
{
    public class StringPrimitiveAttribute : System.Attribute {}
    public class IdAttribute : System.Attribute {}
    public class ValueObjectAttribute : System.Attribute {}
    public class MyCodeAttribute : System.Attribute {}
    public class EmailAttribute : System.Attribute {}
}
namespace EricksonLopez.DomainPrimitives.Validation
{
    public class RegexAttribute : System.Attribute { public RegexAttribute(string pattern) {} }
}
";

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
}
