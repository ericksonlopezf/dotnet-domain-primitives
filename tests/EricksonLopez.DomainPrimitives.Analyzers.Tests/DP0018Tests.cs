// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EricksonLopez.DomainPrimitives.Analyzers;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

using CSharpAnalyzerTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<
    EricksonLopez.DomainPrimitives.Analyzers.ValueObjectAnalyzer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace EricksonLopez.DomainPrimitives.Analyzers.Tests;

public class DP0018Tests
{
    private const string AttributeCode = RoslynTestSnippets.BaseAttributes;

    [Fact]
    public async Task ValueObject_WithArrayProperty_TriggersDP0018()
    {
        var testCode = @"
using EricksonLopez.DomainPrimitives;

" + AttributeCode + @"
[ValueObject]
public readonly partial record struct OrderDetails
{
    public {|DP0018:string[]|} Tags { get; init; }
}
namespace System.Runtime.CompilerServices { public class IsExternalInit {} }
";

        var test = new CSharpAnalyzerTest { TestCode = testCode };
        await test.RunAsync();
    }

    [Fact]
    public async Task ValueObject_WithListProperty_TriggersDP0018()
    {
        var testCode = @"
using System.Collections.Generic;
using EricksonLopez.DomainPrimitives;

" + AttributeCode + @"
[ValueObject]
public readonly partial record struct OrderDetails
{
    public {|DP0018:List<string>|} Items { get; init; }
}
namespace System.Runtime.CompilerServices { public class IsExternalInit {} }
";

        var test = new CSharpAnalyzerTest { TestCode = testCode };
        await test.RunAsync();
    }

    [Fact]
    public async Task ValueObject_WithImmutableOrScalarProperties_DoesNotTriggerDP0018()
    {
        var testCode = @"
using System;
using EricksonLopez.DomainPrimitives;

" + AttributeCode + @"
[ValueObject]
public readonly partial record struct OrderDetails
{
    public string Name { get; init; }
    public int Quantity { get; init; }
}
namespace System.Runtime.CompilerServices { public class IsExternalInit {} }
";

        var test = new CSharpAnalyzerTest { TestCode = testCode };
        await test.RunAsync();
    }

    [Theory]
    [InlineData("Dictionary<string, int>")]
    [InlineData("HashSet<string>")]
    [InlineData("Queue<string>")]
    [InlineData("Stack<string>")]
    [InlineData("LinkedList<string>")]
    [InlineData("SortedDictionary<string, int>")]
    [InlineData("SortedList<string, int>")]
    [InlineData("SortedSet<string>")]
    public async Task ValueObject_WithMutableCollectionProperty_TriggersDP0018(string collectionType)
    {
        var testCode = $@"
using System.Collections.Generic;
using EricksonLopez.DomainPrimitives;

{AttributeCode}
[ValueObject]
public readonly partial record struct OrderDetails
{{
    public {{|DP0018:{collectionType}|}} Items {{ get; init; }}
}}
namespace System.Runtime.CompilerServices {{ public class IsExternalInit {{}} }}
";

        var test = new CSharpAnalyzerTest { TestCode = testCode };
        await test.RunAsync();
    }
}
