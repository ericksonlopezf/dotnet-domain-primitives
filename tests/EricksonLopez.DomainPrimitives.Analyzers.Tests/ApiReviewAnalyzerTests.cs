// Copyright © Erickson Lopez. MIT License.
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

using CSharpAnalyzerTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<
    EricksonLopez.DomainPrimitives.Analyzers.ApiReviewAnalyzer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;
using CSharpCodeFixTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixTest<
    EricksonLopez.DomainPrimitives.Analyzers.ApiReviewAnalyzer,
    EricksonLopez.DomainPrimitives.Analyzers.ApiReviewCodeFixProvider,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace EricksonLopez.DomainPrimitives.Analyzers.Tests;

public class ApiReviewAnalyzerTests
{
    private const string AttributeCode = @"
namespace System.Runtime.CompilerServices
{
    public class IsExternalInit { }
}
namespace EricksonLopez.DomainPrimitives
{
    public class StringPrimitiveAttribute : System.Attribute {}
    public class NumericPrimitiveAttribute : System.Attribute {}
    public class StrongIdAttribute<T> : System.Attribute {}
    public class DatePrimitiveAttribute : System.Attribute {}
    public class ValueObjectAttribute : System.Attribute {}
    public class SmartEnumAttribute : System.Attribute {}
}
";

    [Fact]
    public async Task MissingXmlDocumentation_TriggersDP0015_AppliesCodeFix()
    {
        var testCode = "using EricksonLopez.DomainPrimitives;\n" + AttributeCode + @"
[StringPrimitive]
public readonly partial record struct UserId
{
    public string {|DP0015:Value|} { get; init; }
}
";
        var fixedCode = "using EricksonLopez.DomainPrimitives;\r\n" + AttributeCode + @"
[StringPrimitive]
public readonly partial record struct UserId
{
    /// <summary>
    /// Gets or sets the Value.
    /// </summary>
    public string Value { get; init; }
}
";
        var test = new CSharpCodeFixTest
        {
            TestCode = testCode.Replace("\n", "\r\n").Replace("\r\r", "\r"),
            FixedCode = fixedCode.Replace("\n", "\r\n").Replace("\r\r", "\r"),
            CompilerDiagnostics = CompilerDiagnostics.None
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task InvalidFactoryMethodName_TriggersDP0016_AppliesCodeFix()
    {
        var testCode = "using EricksonLopez.DomainPrimitives;\r\n" + AttributeCode + @"
[StringPrimitive]
public readonly partial record struct UserId
{
    /// <summary>Value</summary>
    public string Value { get; init; }
    
    /// <summary>
    /// Builds a new UserId.
    /// </summary>
    public static UserId {|DP0016:Build|}(string value) => new UserId { Value = value };
}
";
        var fixedCode = "using EricksonLopez.DomainPrimitives;\r\n" + AttributeCode + @"
[StringPrimitive]
public readonly partial record struct UserId
{
    /// <summary>Value</summary>
    public string Value { get; init; }
    
    /// <summary>
    /// Builds a new UserId.
    /// </summary>
    public static UserId Create(string value) => new UserId { Value = value };
}
";
        var test = new CSharpCodeFixTest
        {
            TestCode = testCode.Replace("\n", "\r\n").Replace("\r\r", "\r"),
            FixedCode = fixedCode.Replace("\n", "\r\n").Replace("\r\r", "\r"),
            CompilerDiagnostics = CompilerDiagnostics.None
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task ApiSurfaceBudgetExceeded_TriggersDP0014()
    {
        var properties = string.Join("\n", Enumerable.Range(1, 26).Select(i => $"    /// <summary>Prop{i}</summary>\n    public string Prop{i} {{ get; set; }}"));
        var testCode = "using EricksonLopez.DomainPrimitives;\r\n" + AttributeCode + @"
[StringPrimitive]
public readonly partial record struct {|DP0014:UserId|}
{
" + properties + @"
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
    public async Task NonDomainPrimitive_ProducesNoDiagnostics()
    {
        var testCode = @"
public readonly partial record struct PlainStruct
{
    public string Value { get; set; }
}
";
        var test = new CSharpAnalyzerTest { TestCode = testCode, CompilerDiagnostics = CompilerDiagnostics.None };
        await test.RunAsync();
    }

    [Fact]
    public async Task OverrideAndNonFactoryMethods_ProduceNoDiagnostics()
    {
        var testCode = "using EricksonLopez.DomainPrimitives;\r\n" + AttributeCode + @"
[StringPrimitive]
public readonly partial record struct HelperTest
{
    /// <summary>Doc</summary>
    public static int SomeStaticHelper() => 1;

    /// <summary>Doc</summary>
    public HelperTest InstanceMethod() => default;

    public override string ToString() => string.Empty;
}
";
        var test = new CSharpAnalyzerTest { TestCode = testCode, CompilerDiagnostics = CompilerDiagnostics.None };
        await test.RunAsync();
    }

    [Fact]
    public async Task NumericPrimitive_BudgetBoundaries()
    {
        var methods27 = string.Join("\n", Enumerable.Range(1, 27).Select(i => $"    /// <summary>M{i}</summary>\n    public void M{i}() {{}}"));
        var testCodeExact = "using EricksonLopez.DomainPrimitives;\r\n" + AttributeCode + @"
[NumericPrimitive]
public readonly partial record struct ExactNum
{
" + methods27 + @"
}
";
        await new CSharpAnalyzerTest { TestCode = testCodeExact, CompilerDiagnostics = CompilerDiagnostics.None }.RunAsync();

        var methods28 = string.Join("\n", Enumerable.Range(1, 28).Select(i => $"    /// <summary>M{i}</summary>\n    public void M{i}() {{}}"));
        var testCodeExceeded = "using EricksonLopez.DomainPrimitives;\r\n" + AttributeCode + @"
[NumericPrimitive]
public readonly partial record struct {|DP0014:ExceededNum|}
{
" + methods28 + @"
}
";
        await new CSharpAnalyzerTest { TestCode = testCodeExceeded, CompilerDiagnostics = CompilerDiagnostics.None }.RunAsync();
    }

    [Fact]
    public async Task StrongId_BudgetBoundaries()
    {
        var methods15 = string.Join("\n", Enumerable.Range(1, 15).Select(i => $"    /// <summary>M{i}</summary>\n    public void M{i}() {{}}"));
        var testCodeExact = "using EricksonLopez.DomainPrimitives;\r\n" + AttributeCode + @"
[StrongId<Guid>]
public readonly partial record struct ExactId
{
" + methods15 + @"
}
";
        await new CSharpAnalyzerTest { TestCode = testCodeExact, CompilerDiagnostics = CompilerDiagnostics.None }.RunAsync();

        var methods16 = string.Join("\n", Enumerable.Range(1, 16).Select(i => $"    /// <summary>M{i}</summary>\n    public void M{i}() {{}}"));
        var testCodeExceeded = "using EricksonLopez.DomainPrimitives;\r\n" + AttributeCode + @"
[StrongId<Guid>]
public readonly partial record struct {|DP0014:ExceededId|}
{
" + methods16 + @"
}
";
        await new CSharpAnalyzerTest { TestCode = testCodeExceeded, CompilerDiagnostics = CompilerDiagnostics.None }.RunAsync();
    }

    [Fact]
    public async Task DatePrimitive_BudgetBoundaries()
    {
        var methods23 = string.Join("\n", Enumerable.Range(1, 23).Select(i => $"    /// <summary>M{i}</summary>\n    public void M{i}() {{}}"));
        var testCodeExact = "using EricksonLopez.DomainPrimitives;\r\n" + AttributeCode + @"
[DatePrimitive]
public readonly partial record struct ExactDate
{
" + methods23 + @"
}
";
        await new CSharpAnalyzerTest { TestCode = testCodeExact, CompilerDiagnostics = CompilerDiagnostics.None }.RunAsync();

        var methods24 = string.Join("\n", Enumerable.Range(1, 24).Select(i => $"    /// <summary>M{i}</summary>\n    public void M{i}() {{}}"));
        var testCodeExceeded = "using EricksonLopez.DomainPrimitives;\r\n" + AttributeCode + @"
[DatePrimitive]
public readonly partial record struct {|DP0014:ExceededDate|}
{
" + methods24 + @"
}
";
        await new CSharpAnalyzerTest { TestCode = testCodeExceeded, CompilerDiagnostics = CompilerDiagnostics.None }.RunAsync();
    }

    [Fact]
    public async Task ValueObject_BudgetBoundaries()
    {
        // 20 methods + 2 fields = 22 members, budget is 20 + 2 = 22 (Exact)
        var methods20 = string.Join("\n", Enumerable.Range(1, 20).Select(i => $"    /// <summary>M{i}</summary>\n    public void M{i}() {{}}"));
        var testCodeExact = "using EricksonLopez.DomainPrimitives;\r\n" + AttributeCode + @"
[ValueObject]
public readonly partial record struct ExactVo
{
    /// <summary>F1</summary>
    public string F1;
    /// <summary>F2</summary>
    public string F2;
" + methods20 + @"
}
";
        await new CSharpAnalyzerTest { TestCode = testCodeExact, CompilerDiagnostics = CompilerDiagnostics.None }.RunAsync();

        // 21 methods + 2 fields = 23 members, budget is 20 + 2 = 22 (Exceeded)
        var methods21 = string.Join("\n", Enumerable.Range(1, 21).Select(i => $"    /// <summary>M{i}</summary>\n    public void M{i}() {{}}"));
        var testCodeExceeded = "using EricksonLopez.DomainPrimitives;\r\n" + AttributeCode + @"
[ValueObject]
public readonly partial record struct {|DP0014:ExceededVo|}
{
    /// <summary>F1</summary>
    public string F1;
    /// <summary>F2</summary>
    public string F2;
" + methods21 + @"
}
";
        await new CSharpAnalyzerTest { TestCode = testCodeExceeded, CompilerDiagnostics = CompilerDiagnostics.None }.RunAsync();
    }

    [Fact]
    public async Task SmartEnum_BudgetBoundaries()
    {
        // 11 methods + 1 static field (1) + 1 static property (2) = 14 members, budget is 12 + 2 = 14 (Exact)
        var methods11 = string.Join("\n", Enumerable.Range(1, 11).Select(i => $"    /// <summary>M{i}</summary>\n    public void M{i}() {{}}"));
        var testCodeExact = "using EricksonLopez.DomainPrimitives;\r\n" + AttributeCode + @"
[SmartEnum]
public readonly partial record struct ExactEnum
{
    /// <summary>V1</summary>
    public static readonly ExactEnum V1 = default;
    /// <summary>V2</summary>
    public static ExactEnum V2 => default;
" + methods11 + @"
}
";
        await new CSharpAnalyzerTest { TestCode = testCodeExact, CompilerDiagnostics = CompilerDiagnostics.None }.RunAsync();

        // 12 methods + 1 static field (1) + 1 static property (2) = 15 members, budget is 12 + 2 = 14 (Exceeded)
        var methods12 = string.Join("\n", Enumerable.Range(1, 12).Select(i => $"    /// <summary>M{i}</summary>\n    public void M{i}() {{}}"));
        var testCodeExceeded = "using EricksonLopez.DomainPrimitives;\r\n" + AttributeCode + @"
[SmartEnum]
public readonly partial record struct {|DP0014:ExceededEnum|}
{
    /// <summary>V1</summary>
    public static readonly ExactEnum V1 = default;
    /// <summary>V2</summary>
    public static ExactEnum V2 => default;
" + methods12 + @"
}
";
        await new CSharpAnalyzerTest { TestCode = testCodeExceeded, CompilerDiagnostics = CompilerDiagnostics.None }.RunAsync();
    }

    [Fact]
    public async Task MissingXmlDocumentation_Method_AppliesCodeFix()
    {
        var testCode = "using EricksonLopez.DomainPrimitives;\r\n" + AttributeCode + @"
[StringPrimitive]
public readonly partial record struct UserId
{
    public void {|DP0015:DoSomething|}() { }
}
";
        var fixedCode = "using EricksonLopez.DomainPrimitives;\r\n" + AttributeCode + @"
[StringPrimitive]
public readonly partial record struct UserId
{
    /// <summary>
    /// Gets or sets the DoSomething.
    /// </summary>
    public void DoSomething() { }
}
";
        var test = new CSharpCodeFixTest { TestCode = testCode.Replace("\n", "\r\n").Replace("\r\r", "\r"), FixedCode = fixedCode.Replace("\n", "\r\n").Replace("\r\r", "\r"), CompilerDiagnostics = CompilerDiagnostics.None };
        await test.RunAsync();
    }

    [Fact]
    public async Task MissingXmlDocumentation_Field_AppliesCodeFix()
    {
        var testCode = "using EricksonLopez.DomainPrimitives;\r\n" + AttributeCode + @"
[StringPrimitive]
public readonly partial record struct UserId
{
    public int {|DP0015:SomeField|};
}
";
        var fixedCode = "using EricksonLopez.DomainPrimitives;\r\n" + AttributeCode + @"
[StringPrimitive]
public readonly partial record struct UserId
{
    /// <summary>
    /// Gets or sets the SomeField.
    /// </summary>
    public int SomeField;
}
";
        var test = new CSharpCodeFixTest { TestCode = testCode.Replace("\n", "\r\n").Replace("\r\r", "\r"), FixedCode = fixedCode.Replace("\n", "\r\n").Replace("\r\r", "\r"), CompilerDiagnostics = CompilerDiagnostics.None };
        await test.RunAsync();
    }

    [Fact]
    public async Task NonDomainWith26Props_DoesNotTriggerDP0014()
    {
        var props26 = string.Join("\n", Enumerable.Range(1, 26).Select(i => $"    public string Prop{i} {{ get; set; }}"));
        var testCode = @"
[Serializable]
public readonly partial record struct NonDomainStruct
{
" + props26 + @"
}
";
        var test = new CSharpAnalyzerTest { TestCode = testCode, CompilerDiagnostics = CompilerDiagnostics.None };
        await test.RunAsync();
    }

    [Fact]
    public async Task ValueObject_BudgetBoundaries_WithProperties()
    {
        // 20 methods + 2 properties (which expand into 2 props + 4 accessors = 6 members)
        // ValueObject counting: fields = 2 (props). maxBudget = 20 + 2 = 22.
        var methods20 = string.Join("\n", Enumerable.Range(1, 20).Select(i => $"    /// <summary>M{i}</summary>\n    public void M{i}() {{}}"));
        var testCodeExact = "using EricksonLopez.DomainPrimitives;\r\n" + AttributeCode + @"
[ValueObject]
public readonly partial record struct ExactVoProps
{
    /// <summary>F1</summary>
    public string F1;
    /// <summary>F2</summary>
    public string F2;
" + methods20 + @"
}
";
        await new CSharpAnalyzerTest { TestCode = testCodeExact, CompilerDiagnostics = CompilerDiagnostics.None }.RunAsync();
    }

    [Fact]
    public async Task MissingXmlDocumentation_MultipleLeadingTrivia_AppliesCodeFix()
    {
        var testCode = "using EricksonLopez.DomainPrimitives;\r\n" + AttributeCode + @"
[StringPrimitive]
public readonly partial record struct UserId
{
    // Leading comment
    public string {|DP0015:Value|} { get; init; }
}
";
        var fixedCode = "using EricksonLopez.DomainPrimitives;\r\n" + AttributeCode + @"
[StringPrimitive]
public readonly partial record struct UserId
{
    // Leading comment
    /// <summary>
    /// Gets or sets the Value.
    /// </summary>
    public string Value { get; init; }
}
";
        var test = new CSharpCodeFixTest
        {
            TestCode = testCode.Replace("\n", "\r\n").Replace("\r\r", "\r"),
            FixedCode = fixedCode.Replace("\n", "\r\n").Replace("\r\r", "\r"),
            CompilerDiagnostics = CompilerDiagnostics.None
        };
        await test.RunAsync();
    }
}





