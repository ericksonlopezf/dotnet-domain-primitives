using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.CodeAnalysis.Testing;
using CSharpAnalyzerTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<
    EricksonLopez.DomainPrimitives.Analyzers.ApiReviewAnalyzer,
    Microsoft.CodeAnalysis.Testing.Verifiers.XUnitVerifier>;
using CSharpCodeFixTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixTest<
    EricksonLopez.DomainPrimitives.Analyzers.ApiReviewAnalyzer,
    EricksonLopez.DomainPrimitives.Analyzers.ApiReviewCodeFixProvider,
    Microsoft.CodeAnalysis.Testing.Verifiers.XUnitVerifier>;

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
    public async Task CoverageBranches_EvaluatesBudgetCorrectly()
    {
        var testCode = "using EricksonLopez.DomainPrimitives;\r\n" + AttributeCode + @"
[NumericPrimitive] public readonly partial record struct NumericTest { }
[StrongId<System.Guid>] public readonly partial record struct StrongIdTest { }
[DatePrimitive] public readonly partial record struct DateTest { }
[ValueObject] public readonly partial record struct ValueObjectTest { /// <summary>Doc</summary>
 public string Prop1 { get; set; } }
[SmartEnum] public readonly partial record struct SmartEnumTest { /// <summary>Doc</summary>
 public static readonly SmartEnumTest Val = default; }
";
        var test = new CSharpAnalyzerTest
        {
            TestCode = testCode,
            CompilerDiagnostics = CompilerDiagnostics.None
        };
        await test.RunAsync();
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
}
