// Copyright © Erickson Lopez. MIT License.
using System;
using System.IO;
using System.Linq;
using AwesomeAssertions;
using EricksonLopez.DomainPrimitives.Dapper.SourceGenerators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace EricksonLopez.DomainPrimitives.Dapper.SourceGenerators.Tests;

public class DapperInternalMethodsUnitTests
{
    private static Compilation CreateCompilation(string source)
    {
        string dummyTypes = @"
namespace EricksonLopez.DomainPrimitives
{
    public interface IEntityId { }
    public interface IEntityId<TSelf> : IEntityId { }
    public interface IStrongId<TSelf> { }
    public interface IStrongId<TSelf, TValue> { }
}
namespace System
{
    public struct Guid { }
    public struct DateOnly { }
    public struct DateTime { }
    public struct TimeOnly { }
    public struct DateTimeOffset { }
}
public class Result { }
public class Result<T> { }
public class NonScalarClass { }
";
        var syntaxTrees = new[]
        {
            CSharpSyntaxTree.ParseText(source),
            CSharpSyntaxTree.ParseText(dummyTypes)
        };
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .Cast<MetadataReference>()
            .ToArray();

        return CSharpCompilation.Create("TestCompilation",
            syntaxTrees,
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }

    [Theory]
    [InlineData("<global namespace>", "Global")]
    [InlineData("MyCompany.Domain", "MyCompany_Domain")]
    [InlineData("Nested<Type>.Namespace", "NestedType_Namespace")]
    public void GetSafeNamespace_ReplacesCorrectly(string input, string expected)
    {
        DapperTypeHandlerGenerator.GetSafeNamespace(input).Should().Be(expected);
    }

    [Fact]
    public void IsResultType_MatchesResultAndGenericResult()
    {
        string source = @"
public class TestService
{
    public Result Method1() => null!;
    public Result<int> Method2() => null!;
    public int Method3() => 0;
    public void Method4() { }
}";
        var comp = CreateCompilation(source);
        var type = comp.GetTypeByMetadataName("TestService")!;
        var m1 = type.GetMembers("Method1").OfType<IMethodSymbol>().First();
        var m2 = type.GetMembers("Method2").OfType<IMethodSymbol>().First();
        var m3 = type.GetMembers("Method3").OfType<IMethodSymbol>().First();
        var m4 = type.GetMembers("Method4").OfType<IMethodSymbol>().First();

        DapperTypeHandlerGenerator.IsResultType(m1.ReturnType).Should().BeTrue();
        DapperTypeHandlerGenerator.IsResultType(m2.ReturnType).Should().BeTrue();
        DapperTypeHandlerGenerator.IsResultType(m3.ReturnType).Should().BeFalse();
        DapperTypeHandlerGenerator.IsResultType(m4.ReturnType).Should().BeFalse();
    }

    [Fact]
    public void IsScalarType_MatchesPrimitivesAndDateTimeTypes()
    {
        string source = @"
public class TypeHolder
{
    public int PInt { get; set; }
    public string PString { get; set; } = """";
    public bool PBool { get; set; }
    public decimal PDecimal { get; set; }
    public System.Guid PGuid { get; set; }
    public System.DateOnly PDateOnly { get; set; }
    public System.DateTime PDateTime { get; set; }
    public System.DateTimeOffset PDateTimeOffset { get; set; }
    public System.TimeOnly PTimeOnly { get; set; }
    public NonScalarClass PNonScalar { get; set; } = null!;
}";
        var comp = CreateCompilation(source);
        var holder = comp.GetTypeByMetadataName("TypeHolder")!;
        
        bool Check(string propName) =>
            DapperTypeHandlerGenerator.IsScalarType(holder.GetMembers(propName).OfType<IPropertySymbol>().First().Type);

        Check("PInt").Should().BeTrue();
        Check("PString").Should().BeTrue();
        Check("PBool").Should().BeTrue();
        Check("PDecimal").Should().BeTrue();
        Check("PGuid").Should().BeTrue();
        Check("PDateOnly").Should().BeTrue();
        Check("PDateTime").Should().BeTrue();
        Check("PDateTimeOffset").Should().BeTrue();
        Check("PTimeOnly").Should().BeTrue();
        Check("PNonScalar").Should().BeFalse();
    }

    [Fact]
    public void DapperTypeHandlerGenerator_GetPrimitiveInfoFromSymbol_IdentifiesAllVariants()
    {
        string source = @"
namespace TestNs
{
    public class ClassType { }

    // StrongId 2 type args
    public readonly struct StrongIdTwo : EricksonLopez.DomainPrimitives.IStrongId<StrongIdTwo, System.Guid> { }

    // StrongId 1 type arg with Value prop
    public readonly struct StrongIdOneWithValue : EricksonLopez.DomainPrimitives.IStrongId<StrongIdOneWithValue>
    {
        public string Value => """";
    }

    // StrongId 1 type arg without Value prop
    public readonly struct StrongIdOneWithoutValue : EricksonLopez.DomainPrimitives.IStrongId<StrongIdOneWithoutValue> { }

    // Convention *Id with Value and static Create
    public readonly struct OrderId
    {
        public int Value => 0;
        public static OrderId Create(int val) => default;
    }

    // Convention *Id with Value and static Create returning Result<T>
    public readonly struct ResultId
    {
        public int Value => 0;
        public static Result<ResultId> Create(int val) => null!;
    }

    // Convention *Id with Value but NO Create
    public readonly struct NoCreateId
    {
        public int Value => 0;
    }

    // Convention *Id with instance Create
    public readonly struct InstanceCreateId
    {
        public int Value => 0;
        public InstanceCreateId Create(int val) => default;
    }

    // Convention *Id with Create having 2 parameters
    public readonly struct TwoParamCreateId
    {
        public int Value => 0;
        public static TwoParamCreateId Create(int val, string extra) => default;
    }

    // Value Object (non-Id, public readonly record struct with scalar Value and static Create)
    public readonly record struct BranchCode
    {
        public string Value => """";
        public static BranchCode Create(string val) => default;
    }

    // Value Object with non-scalar Value
    public readonly record struct NonScalarVO
    {
        public NonScalarClass Value => null!;
        public static NonScalarVO Create(NonScalarClass val) => default;
    }

    // Value Object with instance Create
    public readonly record struct InstanceCreateVO
    {
        public string Value => """";
        public InstanceCreateVO Create(string val) => default;
    }

    // Value Object with 2-param Create
    public readonly record struct TwoParamCreateVO
    {
        public string Value => """";
        public static TwoParamCreateVO Create(string val, int num) => default;
    }
}";
        var comp = CreateCompilation(source);

        INamedTypeSymbol GetSym(string name) => comp.GetTypeByMetadataName($"TestNs.{name}")!;

        // Non-value type returns null
        DapperTypeHandlerGenerator.GetPrimitiveInfoFromSymbol(GetSym("ClassType")).Should().BeNull();

        // StrongId 2 type args
        var infoTwo = DapperTypeHandlerGenerator.GetPrimitiveInfoFromSymbol(GetSym("StrongIdTwo"));
        infoTwo.Should().NotBeNull();
        infoTwo!.Value.BackingType.Should().Contain("Guid");
        infoTwo.Value.IsSmartEnum.Should().BeFalse();
        infoTwo.Value.IsResultReturning.Should().BeFalse();

        // StrongId 1 type arg with Value prop
        var infoOneVal = DapperTypeHandlerGenerator.GetPrimitiveInfoFromSymbol(GetSym("StrongIdOneWithValue"));
        infoOneVal.Should().NotBeNull();
        infoOneVal!.Value.BackingType.Should().Contain("string");

        // StrongId 1 type arg without Value prop
        var infoOneNoVal = DapperTypeHandlerGenerator.GetPrimitiveInfoFromSymbol(GetSym("StrongIdOneWithoutValue"));
        infoOneNoVal.Should().NotBeNull();
        infoOneNoVal!.Value.BackingType.Should().Be("global::System.Guid");

        // OrderId
        var infoOrder = DapperTypeHandlerGenerator.GetPrimitiveInfoFromSymbol(GetSym("OrderId"));
        infoOrder.Should().NotBeNull();
        infoOrder!.Value.IsResultReturning.Should().BeFalse();

        // ResultId
        var infoResult = DapperTypeHandlerGenerator.GetPrimitiveInfoFromSymbol(GetSym("ResultId"));
        infoResult.Should().NotBeNull();
        infoResult!.Value.IsResultReturning.Should().BeTrue();

        // Invalid Id variants
        DapperTypeHandlerGenerator.GetPrimitiveInfoFromSymbol(GetSym("NoCreateId")).Should().BeNull();
        DapperTypeHandlerGenerator.GetPrimitiveInfoFromSymbol(GetSym("InstanceCreateId")).Should().BeNull();
        DapperTypeHandlerGenerator.GetPrimitiveInfoFromSymbol(GetSym("TwoParamCreateId")).Should().BeNull();

        // Valid Value Object
        var infoVO = DapperTypeHandlerGenerator.GetPrimitiveInfoFromSymbol(GetSym("BranchCode"));
        infoVO.Should().NotBeNull();
        infoVO!.Value.TypeName.Should().Be("BranchCode");
        infoVO.Value.IsSmartEnum.Should().BeFalse();

        // Invalid Value Objects
        DapperTypeHandlerGenerator.GetPrimitiveInfoFromSymbol(GetSym("NonScalarVO")).Should().BeNull();
        DapperTypeHandlerGenerator.GetPrimitiveInfoFromSymbol(GetSym("InstanceCreateVO")).Should().BeNull();
        DapperTypeHandlerGenerator.GetPrimitiveInfoFromSymbol(GetSym("TwoParamCreateVO")).Should().BeNull();
    }

    [Fact]
    public void JsonConverterGenerator_GetPrimitiveInfoFromSymbol_IdentifiesAllVariants()
    {
        string source = @"
namespace TestNs
{
    public class ClassType { }

    // IEntityId with 0 type args
    public readonly struct EntityIdZero : EricksonLopez.DomainPrimitives.IEntityId { }

    // IEntityId with 1 type arg
    public readonly struct EntityIdOne : EricksonLopez.DomainPrimitives.IEntityId<EntityIdOne> { }

    // IStrongId with 2 type args
    public readonly struct StrongIdTwo : EricksonLopez.DomainPrimitives.IStrongId<StrongIdTwo, int> { }

    // Convention *Id
    public readonly struct CustomerId
    {
        public string Value => """";
        public static CustomerId Create(string val) => default;
    }

    // Convention *Id without Create
    public readonly struct NoCreateCustomerId
    {
        public string Value => """";
    }

    // Convention *Id with instance Create
    public readonly struct InstanceCreateCustomerId
    {
        public string Value => """";
        public InstanceCreateCustomerId Create(string val) => default;
    }

    // Convention *Id with 2 param Create
    public readonly struct TwoParamCustomerId
    {
        public string Value => """";
        public static TwoParamCustomerId Create(string val, int num) => default;
    }
}";
        var comp = CreateCompilation(source);

        INamedTypeSymbol GetSym(string name) => comp.GetTypeByMetadataName($"TestNs.{name}")!;

        // Non value type
        JsonConverterGenerator.GetPrimitiveInfoFromSymbol(GetSym("ClassType")).Should().BeNull();

        // IEntityId 0 type args
        var infoZero = JsonConverterGenerator.GetPrimitiveInfoFromSymbol(GetSym("EntityIdZero"));
        infoZero.Should().NotBeNull();
        infoZero!.Value.BackingType.Should().Be("global::System.Guid");

        // IEntityId 1 type arg
        var infoOne = JsonConverterGenerator.GetPrimitiveInfoFromSymbol(GetSym("EntityIdOne"));
        infoOne.Should().NotBeNull();
        infoOne!.Value.BackingType.Should().Be("global::System.Guid");

        // IStrongId 2 type args
        var infoStrong = JsonConverterGenerator.GetPrimitiveInfoFromSymbol(GetSym("StrongIdTwo"));
        infoStrong.Should().NotBeNull();
        infoStrong!.Value.BackingType.Should().Contain("int");

        // CustomerId
        var infoCustomer = JsonConverterGenerator.GetPrimitiveInfoFromSymbol(GetSym("CustomerId"));
        infoCustomer.Should().NotBeNull();
        infoCustomer!.Value.TypeName.Should().Be("CustomerId");

        // Invalid *Id variants
        JsonConverterGenerator.GetPrimitiveInfoFromSymbol(GetSym("NoCreateCustomerId")).Should().BeNull();
        JsonConverterGenerator.GetPrimitiveInfoFromSymbol(GetSym("InstanceCreateCustomerId")).Should().BeNull();
        JsonConverterGenerator.GetPrimitiveInfoFromSymbol(GetSym("TwoParamCustomerId")).Should().BeNull();
    }

    [Theory]
    [InlineData("Guid")]
    [InlineData("System.Guid")]
    public void JsonConverterGenerator_GenerateJsonConverter_WithGuidVariants_GeneratesGuidReadAndWrite(string backing)
    {
        var info = new PrimitiveInfo("MyCompany.Domain", "MyGuidId", backing, false);
        var code = JsonConverterGenerator.GenerateJsonConverter(info);

        code.Should().Contain("reader.GetGuid()");
        code.Should().Contain("writer.WriteStringValue(value.Value)");
    }
}
