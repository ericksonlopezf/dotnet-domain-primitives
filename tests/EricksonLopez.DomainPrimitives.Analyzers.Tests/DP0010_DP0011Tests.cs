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
    EricksonLopez.DomainPrimitives.Analyzers.StringComparisonAnalyzer,
    Microsoft.CodeAnalysis.Testing.Verifiers.XUnitVerifier>;
using CSharpAnalyzerVerifier = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<
    EricksonLopez.DomainPrimitives.Analyzers.StringComparisonAnalyzer>;

namespace EricksonLopez.DomainPrimitives.Analyzers.Tests;

public class DP0010Tests
{
    private const string DomainPrimitiveStubs = @"
namespace System.Runtime.CompilerServices
{
    public class IsExternalInit { }
}

namespace EricksonLopez.DomainPrimitives
{
    public interface IDomainPrimitive<TSelf> where TSelf : IDomainPrimitive<TSelf> { }
    public interface IDomainPrimitive<TSelf, TValue> : IDomainPrimitive<TSelf>
        where TSelf : IDomainPrimitive<TSelf, TValue>
        where TValue : notnull
    {
        TValue Value { get; }
    }

    public readonly partial record struct EmailAddress : IDomainPrimitive<EmailAddress, string>
    {
        public string Value { get; init; }
        public static EmailAddress Create(string value) => new() { Value = value };
        public static implicit operator string(EmailAddress email) => email.Value;
    }
}
";

    [Fact]
    public async Task StringComparedWithPrimitive_LeftSideString_TriggersDiagnostic()
    {
        var testCode = "using EricksonLopez.DomainPrimitives;\n" + DomainPrimitiveStubs + @"

class Test
{
    void M(EmailAddress email)
    {
        string raw = ""test@example.com"";
        _ = {|DP0010:raw == email|};
    }
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
    public async Task StringComparedWithPrimitive_RightSideString_TriggersDiagnostic()
    {
        var testCode = "using EricksonLopez.DomainPrimitives;\n" + DomainPrimitiveStubs + @"

class Test
{
    void M(EmailAddress email)
    {
        string raw = ""test@example.com"";
        _ = {|DP0010:email == raw|};
    }
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
    public async Task PrimitiveToPrimitiveComparison_NoDiagnostic()
    {
        var testCode = "using EricksonLopez.DomainPrimitives;\n" + DomainPrimitiveStubs + @"

class Test
{
    void M(EmailAddress a, EmailAddress b)
    {
        _ = a == b; // Both are primitives — no diagnostic
    }
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
    public async Task StringAssignedFromPrimitive_LocalDecl_TriggersDiagnostic()
    {
        var testCode = "using EricksonLopez.DomainPrimitives;\n" + DomainPrimitiveStubs + @"

class Test
{
    void M(EmailAddress email)
    {
        string {|DP0011:s = email|};
    }
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
    public async Task StringAssignedFromPrimitive_AssignmentExpr_TriggersDiagnostic()
    {
        var testCode = "using EricksonLopez.DomainPrimitives;\n" + DomainPrimitiveStubs + @"

class Test
{
    void M(EmailAddress email)
    {
        string s = string.Empty;
        {|DP0011:s = email|};
    }
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
    public async Task StringAssignedFromValue_NoDiagnostic()
    {
        var testCode = "using EricksonLopez.DomainPrimitives;\n" + DomainPrimitiveStubs + @"

class Test
{
    void M(EmailAddress email)
    {
        string s = email.Value; // Correct usage — no diagnostic
    }
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
    public async Task StringNotEqualsPrimitive_LeftSideString_TriggersDiagnostic()
    {
        var testCode = "using EricksonLopez.DomainPrimitives;\n" + DomainPrimitiveStubs + @"

class Test
{
    void M(EmailAddress email)
    {
        string raw = ""test@example.com"";
        _ = {|DP0010:raw != email|};
    }
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
    public async Task StringAssignedFromPrimitive_FieldDecl_TriggersDiagnostic()
    {
        var testCode = "using EricksonLopez.DomainPrimitives;\n" + DomainPrimitiveStubs + @"

class Test
{
    private static readonly EmailAddress email = EmailAddress.Create(""test@test.com"");
    private string {|DP0011:s = email|};
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
    public async Task StringAssignedFromPrimitive_Argument_TriggersDiagnostic()
    {
        var testCode = "using EricksonLopez.DomainPrimitives;\n" + DomainPrimitiveStubs + @"

class Test
{
    void RequiresString(string s) { }

    void M(EmailAddress email)
    {
        RequiresString({|DP0011:email|});
    }
}
";
        var test = new CSharpAnalyzerTest
        {
            TestCode = testCode,
            CompilerDiagnostics = CompilerDiagnostics.None
        };
        await test.RunAsync();
    }
}

