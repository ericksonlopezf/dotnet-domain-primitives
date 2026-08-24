// Copyright © Erickson Lopez. MIT License.
using System;
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.DomainPrimitives.Analyzers;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

using CSharpCodeFixTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixTest<
    EricksonLopez.DomainPrimitives.Analyzers.StructDeclarationAnalyzer,
    EricksonLopez.DomainPrimitives.Analyzers.StructDeclarationCodeFixProvider,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace EricksonLopez.DomainPrimitives.Analyzers.Tests;

public class StructDeclarationCodeFixTests
{
    private const string AttributeStubs = @"
namespace EricksonLopez.DomainPrimitives
{
    public class StrongIdAttribute<T> : System.Attribute {}
}
";

    [Fact]
    public async Task FixMissingPartial_AddsPartialKeyword()
    {
        var testCode = AttributeStubs + @"
namespace TestNamespace
{
    using System;
    [EricksonLopez.DomainPrimitives.StrongId<Guid>]
    public readonly record struct {|DP0001:MyId|} { }
}";

        var fixedCode = AttributeStubs + @"
namespace TestNamespace
{
    using System;
    [EricksonLopez.DomainPrimitives.StrongId<Guid>]
    public partial readonly record struct MyId { }
}";
        // It might be 'public readonly partial record struct MyId { }'. The provider inserts it at the end of modifiers or after visibility. 
        // Let's check provider: newModifiers = node.Modifiers.Add(SyntaxFactory.Token(SyntaxKind.PartialKeyword));
        // So 'public readonly partial record struct'

        fixedCode = AttributeStubs + @"
namespace TestNamespace
{
    using System;
    [EricksonLopez.DomainPrimitives.StrongId<Guid>]
    public readonly partial record struct MyId { }
}";

        var test = new CSharpCodeFixTest
        {
            TestCode = testCode,
            FixedCode = fixedCode,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80
        };

        await test.RunAsync();
    }

    [Fact]
    public async Task FixMissingReadonly_AddsReadonlyKeyword()
    {
        var testCode = AttributeStubs + @"
namespace TestNamespace
{
    using System;
    [EricksonLopez.DomainPrimitives.StrongId<Guid>]
    public partial record struct {|DP0002:MyId|} { }
}";

        var fixedCode = AttributeStubs + @"
namespace TestNamespace
{
    using System;
    [EricksonLopez.DomainPrimitives.StrongId<Guid>]
    public readonly partial record struct MyId { }
}";

        var test = new CSharpCodeFixTest
        {
            TestCode = testCode,
            FixedCode = fixedCode,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80
        };

        await test.RunAsync();
    }

    [Fact]
    public async Task FixNotRecordStruct_ConvertsToRecordStruct()
    {
        var testCode = AttributeStubs + @"
namespace TestNamespace
{
    using System;
    [EricksonLopez.DomainPrimitives.StrongId<Guid>]
    public readonly partial struct {|DP0003:MyId|} { }
}";

        var fixedCode = AttributeStubs + @"
namespace TestNamespace
{
    using System;
    [EricksonLopez.DomainPrimitives.StrongId<Guid>]
    public readonly partial record struct MyId { }
}";

        var test = new CSharpCodeFixTest
        {
            TestCode = testCode,
            FixedCode = fixedCode,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80
        };

        await test.RunAsync();
    }

    [Fact]
    public async Task FixNotRecordStruct_AddsMissingModifiersAndConvertsToRecordStruct()
    {
        var testCode = AttributeStubs + @"
namespace TestNamespace
{
    using System;
    [EricksonLopez.DomainPrimitives.StrongId<Guid>]
    public struct {|DP0003:MyId|} { }
}";

        var fixedCode = AttributeStubs + @"
namespace TestNamespace
{
    using System;
    [EricksonLopez.DomainPrimitives.StrongId<Guid>]
    readonly public partial record struct MyId { }
}";

        var test = new CSharpCodeFixTest
        {
            TestCode = testCode,
            FixedCode = fixedCode,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
            CodeActionEquivalenceKey = "DP0003_MakeRecordStruct"
        };

        await test.RunAsync();
    }
}





