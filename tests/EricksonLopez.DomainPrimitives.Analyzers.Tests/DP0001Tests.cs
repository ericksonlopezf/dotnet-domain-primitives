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
using CSharpCodeFixTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixTest<
    EricksonLopez.DomainPrimitives.Analyzers.StructDeclarationAnalyzer,
    EricksonLopez.DomainPrimitives.Analyzers.StructDeclarationCodeFixProvider,
    Microsoft.CodeAnalysis.Testing.Verifiers.XUnitVerifier>;

namespace EricksonLopez.DomainPrimitives.Analyzers.Tests;

public class DP0001Tests
{
    private const string AttributeCode = @"
namespace EricksonLopez.DomainPrimitives
{
    public class StrongIdAttribute<T> : System.Attribute {}
}
";

    [Fact]
    public async Task MissingPartial_TriggersDiagnostic_AndCodeFixApplies()
    {
        var testCode = @"
using EricksonLopez.DomainPrimitives;
" + AttributeCode + @"
[StrongId<System.Guid>]
public readonly record struct {|DP0001:UserId|} { }
";

        var fixedCode = @"
" + AttributeCode + @"
[StrongId<System.Guid>]
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

