using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.DomainPrimitives.Tests.TestTypes;
using FluentAssertions;

namespace EricksonLopez.DomainPrimitives.Tests;

public sealed class IntStrongIdTests
{
    [Fact]
    public void Create_Wraps_Value()
    {
        var id = OrderNumber.Create(42);
        id.Value.Should().Be(42);
    }

    [Fact]
    public void TryCreate_Returns_Success()
    {
        var success = OrderNumber.TryCreate(12345, out var result, out _);
        success.Should().BeTrue();
        result.Value.Should().Be(12345);
    }

    [Fact]
    public void New_Throws_NotSupportedException()
    {
        var act = () => OrderNumber.Create();
        act.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void Equal_Ids_Are_Equal()
    {
        var id1 = OrderNumber.Create(42);
        var id2 = OrderNumber.Create(42);

        id1.Should().Be(id2);
        (id1 == id2).Should().BeTrue();
    }

    [Fact]
    public void Parse_String_Succeeds()
    {
        var id = OrderNumber.Parse("42");
        id.Value.Should().Be(42);
    }

    [Fact]
    public void TryParse_Invalid_Returns_False()
    {
        var success = OrderNumber.TryParse("not-a-number", null, out _);
        success.Should().BeFalse();
    }

    [Fact]
    public void ToString_Returns_Value_String()
    {
        var id = OrderNumber.Create(42);
        id.ToString().Should().Be("42");
    }

    [Fact]
    public void Explicit_Operator_Roundtrip()
    {
        var id = OrderNumber.Create(42);
        int raw = (int)id;
        var back = (OrderNumber)raw;

        raw.Should().Be(42);
        back.Should().Be(id);
    }

    [Fact]
    public void Comparison_Operators_Work()
    {
        var small = OrderNumber.Create(1);
        var large = OrderNumber.Create(100);

        (small < large).Should().BeTrue();
        (large > small).Should().BeTrue();
        
        var smallCopy = OrderNumber.Create(1);
        (small <= smallCopy).Should().BeTrue();
    }

    [Fact]
    public void PrimitiveName_Returns_TypeName()
    {
        OrderNumber.PrimitiveName.Should().Be("OrderNumber");
    }
}
