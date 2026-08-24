// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;
using AwesomeAssertions;
using EricksonLopez.DomainPrimitives.UnitTests.TestTypes;
using EricksonLopez.DomainPrimitives.Validation;
using Xunit;

namespace EricksonLopez.DomainPrimitives.UnitTests;

public sealed class IntStrongIdTests
{
    // ─── Construction ────────────────────────────────────────────────────────

    [Fact]
    public void Create_Wraps_Value()
    {
        var id = OrderNumber.Create(42);
        id.Value.Should().Be(42);
        id.IsDefault.Should().BeFalse();
    }

    [Fact]
    public void TryCreate_Returns_Success()
    {
        var success = OrderNumber.TryCreate(12345, out var result, out var error);
        success.Should().BeTrue();
        result.Value.Should().Be(12345);
        result.IsDefault.Should().BeFalse();
        error.Should().Be(PrimitiveError.None);
    }

    [Fact]
    public void New_Throws_NotSupportedException()
    {
        var act = () => OrderNumber.Create();
        act.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void Empty_Returns_DefaultInstance_And_Throws_On_Value_Access()
    {
        var empty = OrderNumber.Empty;

        empty.IsDefault.Should().BeTrue();
        var act = () => empty.Value;
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*default instance of OrderNumber*");
    }

    [Fact]
    public void Default_Instance_IsDefault_True()
    {
        OrderNumber defaultId = default;
        defaultId.IsDefault.Should().BeTrue();
    }

    // ─── Equality ────────────────────────────────────────────────────────────

    [Fact]
    public void Equal_Ids_Are_Equal()
    {
        var id1 = OrderNumber.Create(42);
        var id2 = OrderNumber.Create(42);

        id1.Should().Be(id2);
        (id1 == id2).Should().BeTrue();
        (id1 != id2).Should().BeFalse();
        id1.Equals(id2).Should().BeTrue();
        id1.Equals((object)id2).Should().BeTrue();
    }

    [Fact]
    public void Different_Ids_Are_Not_Equal()
    {
        var id1 = OrderNumber.Create(42);
        var id2 = OrderNumber.Create(43);

        id1.Should().NotBe(id2);
        (id1 == id2).Should().BeFalse();
        (id1 != id2).Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_Is_Consistent()
    {
        var id1 = OrderNumber.Create(100);
        var id2 = OrderNumber.Create(100);

        id1.GetHashCode().Should().Be(id2.GetHashCode());
    }

    // ─── Parsing & Formatting ─────────────────────────────────────────────────

    [Fact]
    public void Parse_String_Succeeds()
    {
        var id = OrderNumber.Parse("42");
        id.Value.Should().Be(42);
    }

    [Fact]
    public void Parse_Span_Succeeds()
    {
        ReadOnlySpan<char> span = "999".AsSpan();
        var id = OrderNumber.Parse(span);
        id.Value.Should().Be(999);
    }

    [Fact]
    public void TryParse_Valid_Returns_True()
    {
        var success = OrderNumber.TryParse("42", null, out var id);
        success.Should().BeTrue();
        id.Value.Should().Be(42);
    }

    [Fact]
    public void TryParse_Span_Valid_Returns_True()
    {
        ReadOnlySpan<char> span = "123".AsSpan();
        var success = OrderNumber.TryParse(span, null, out var id);
        success.Should().BeTrue();
        id.Value.Should().Be(123);
    }

    [Fact]
    public void TryParse_Invalid_Returns_False()
    {
        var success = OrderNumber.TryParse("not-a-number", null, out var id);
        success.Should().BeFalse();
        id.IsDefault.Should().BeTrue();
    }

    [Fact]
    public void TryParse_Null_Returns_False()
    {
        var success = OrderNumber.TryParse((string?)null, null, out var id);
        success.Should().BeFalse();
        id.IsDefault.Should().BeTrue();
    }

    [Fact]
    public void ToString_Returns_Value_String()
    {
        var id = OrderNumber.Create(42);
        id.ToString().Should().Be("42");
    }

    [Fact]
    public void TryFormat_Span_Succeeds()
    {
        var id = OrderNumber.Create(12345);
        Span<char> destination = stackalloc char[32];
        var success = id.TryFormat(destination, out var charsWritten, default, null);

        success.Should().BeTrue();
        destination.Slice(0, charsWritten).ToString().Should().Be("12345");
    }

    // ─── Casts & Operators ───────────────────────────────────────────────────

    [Fact]
    public void Explicit_Operator_Roundtrip()
    {
        var id = OrderNumber.Create(42);
        int raw = (int)id;
        var back = (OrderNumber)raw;

        raw.Should().Be(42);
        back.Should().Be(id);
    }

    // ─── Comparison ──────────────────────────────────────────────────────────

    [Fact]
    public void Comparison_Operators_Work()
    {
        var small = OrderNumber.Create(1);
        var large = OrderNumber.Create(100);

        (small < large).Should().BeTrue();
        (large > small).Should().BeTrue();
        
        var smallCopy = OrderNumber.Create(1);
        (small <= smallCopy).Should().BeTrue();
        (large >= small).Should().BeTrue();
    }

    [Fact]
    public void CompareTo_Works()
    {
        var id1 = OrderNumber.Create(10);
        var id2 = OrderNumber.Create(20);

        id1.CompareTo(id2).Should().BeNegative();
        id2.CompareTo(id1).Should().BePositive();
        id1.CompareTo(id1).Should().Be(0);
    }

    // ─── Collections ─────────────────────────────────────────────────────────

    [Fact]
    public void Works_In_Dictionary()
    {
        var dict = new Dictionary<OrderNumber, string>();
        var id = OrderNumber.Create(1001);

        dict[id] = "First Order";

        dict.Should().ContainKey(id);
        dict[id].Should().Be("First Order");
    }

    [Fact]
    public void Works_In_HashSet()
    {
        var set = new HashSet<OrderNumber>();
        var id1 = OrderNumber.Create(500);
        var id2 = OrderNumber.Create(500);

        set.Add(id1);
        set.Add(id2);

        set.Should().HaveCount(1);
        set.Should().Contain(id1);
    }

    // ─── Boundaries ──────────────────────────────────────────────────────────

    [Theory]
    [InlineData(1)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    public void Boundaries_With_Extreme_NonZero_Integers_Create_Successfully(int boundaryValue)
    {
        var id = OrderNumber.Create(boundaryValue);
        id.Value.Should().Be(boundaryValue);
        id.IsDefault.Should().BeFalse();
    }

    [Fact]
    public void Zero_Throws_EmptyError()
    {
        Action act = () => OrderNumber.Create(0);
        act.Should().Throw<DomainPrimitiveValidationException>()
            .WithMessage("*OrderNumber must not be empty*")
            .Where(e => e.Error.Code == "EMPTY");

        var success = OrderNumber.TryCreate(0, out var id, out var error);
        success.Should().BeFalse();
        id.IsDefault.Should().BeTrue();
        error.Code.Should().Be("EMPTY");
    }

    // ─── Metadata ────────────────────────────────────────────────────────────

    [Fact]
    public void PrimitiveName_Returns_TypeName()
    {
        OrderNumber.PrimitiveName.Should().Be("OrderNumber");
    }
}




