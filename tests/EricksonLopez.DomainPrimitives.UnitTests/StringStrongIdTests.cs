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

public sealed class StringStrongIdTests
{
    [Fact]
    public void Create_Wraps_Value()
    {
        var id = Sku.Create("SKU-12345");
        id.Value.Should().Be("SKU-12345");
    }

    [Fact]
    public void TryCreate_Returns_Success()
    {
        var success = Sku.TryCreate("SKU-999", out var result, out _);
        success.Should().BeTrue();
        result.Value.Should().Be("SKU-999");
    }

    [Fact]
    public void New_Throws_NotSupportedException()
    {
        var act = () => Sku.Create();
        act.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void Empty_Sentinel_Value_Throws()
    {
        // Sku.Empty returns default(Sku). Accessing .Value on a default/uninitialized
        // instance is intentionally disallowed — it throws InvalidOperationException.
        // Use IsDefault to guard before accessing Value in production code.
        var empty = Sku.Empty;
        empty.IsDefault.Should().BeTrue();
        var act = () => _ = empty.Value;
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Equal_Ids_Are_Equal()
    {
        var id1 = Sku.Create("ABC");
        var id2 = Sku.Create("ABC");

        id1.Should().Be(id2);
        (id1 == id2).Should().BeTrue();
    }

    [Fact]
    public void Parse_Returns_Id()
    {
        var id = Sku.Parse("ABC");
        id.Value.Should().Be("ABC");
    }

    [Fact]
    public void ToString_Returns_Value()
    {
        var id = Sku.Create("SKU-001");
        id.ToString().Should().Be("SKU-001");
    }

    [Fact]
    public void Explicit_Operator_Roundtrip()
    {
        var id = Sku.Create("TEST");
        string raw = (string)id;
        var back = (Sku)raw;

        raw.Should().Be("TEST");
        back.Should().Be(id);
    }

    [Fact]
    public void Comparison_Uses_Ordinal()
    {
        var a = Sku.Create("A");
        var b = Sku.Create("B");

        (a < b).Should().BeTrue();
        (b > a).Should().BeTrue();
    }

    [Fact]
    public void PrimitiveName_Returns_TypeName()
    {
        Sku.PrimitiveName.Should().Be("Sku");
    }
}
