// Copyright © Erickson Lopez. MIT License.
using System;
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.DomainPrimitives.Analyzers;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

using CSharpAnalyzerTest = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<
    EricksonLopez.DomainPrimitives.Analyzers.StructDeclarationAnalyzer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace EricksonLopez.DomainPrimitives.Analyzers.Tests;

public class StructDeclarationAnalyzerTests
{
    [Fact]
    public async Task MissingPartial_ReportsError_DP0001()
    {
        var source = @"

namespace EricksonLopez.DomainPrimitives
{
    public class StrongIdAttribute<T> : System.Attribute {}
}

namespace TestNamespace
{
    using System;
    [EricksonLopez.DomainPrimitives.StrongId<Guid>]
    public readonly record struct {|DP0001:MyId|} { }
}";

        var test = new CSharpAnalyzerTest
        {
            TestCode = source,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task MissingReadonly_ReportsError_DP0002()
    {
        var source = @"

namespace EricksonLopez.DomainPrimitives
{
    public class StrongIdAttribute<T> : System.Attribute {}
}

namespace TestNamespace
{
    using System;
    [EricksonLopez.DomainPrimitives.StrongId<Guid>]
    public partial record struct {|DP0002:MyId|} { }
}";

        var test = new CSharpAnalyzerTest
        {
            TestCode = source,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task NotRecordStruct_ReportsError_DP0003()
    {
        var source = @"

namespace EricksonLopez.DomainPrimitives
{
    public class StrongIdAttribute<T> : System.Attribute {}
}

namespace TestNamespace
{
    using System;
    [EricksonLopez.DomainPrimitives.StrongId<Guid>]
    public readonly partial struct {|DP0003:MyId|} { }
}";

        var test = new CSharpAnalyzerTest
        {
            TestCode = source,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80
        };
        await test.RunAsync();
    }

    [Fact]
    public async Task ValidReadonlyPartialRecordStruct_ReportsNoError()
    {
        var source = @"

namespace EricksonLopez.DomainPrimitives
{
    public class StrongIdAttribute<T> : System.Attribute {}
}

namespace TestNamespace
{
    using System;
    [Serializable]
    [global::EricksonLopez.DomainPrimitives.StrongId<Guid>]
    public readonly partial record struct MyValidId { }

    [Serializable]
    public struct OrdinaryStruct { }
}";

        var test = new CSharpAnalyzerTest
        {
            TestCode = source,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80
        };
        await test.RunAsync();
    }
}






