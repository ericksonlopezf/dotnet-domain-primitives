// Copyright © Erickson Lopez. MIT License.
using System;
using System.Text.Json;
using AwesomeAssertions;
using EricksonLopez.DomainPrimitives.UnitTests.TestTypes;
using EricksonLopez.DomainPrimitives.Validation;
using Xunit;

namespace EricksonLopez.DomainPrimitives.UnitTests;

public class ValueObjectTests
{
    [Fact]
    public void Address_Create_Valid_Works()
    {
        var address = Address.Create(
            street: "123 Main St",
            city: "Seattle",
            state: "WA",
            zipCode: "98101"
        );

        address.Street.Should().Be("123 Main St");
        address.City.Should().Be("Seattle");
        address.State.Should().Be("WA");
        address.ZipCode.Should().Be("98101");
    }

    [Fact]
    public void Address_Create_Invalid_Throws()
    {
        Action act = () => Address.Create(
            street: "   ",
            city: "Seattle",
            state: "WA",
            zipCode: "98101"
        );
        
        act.Should().Throw<DomainPrimitiveValidationException>()
           .WithMessage("*Street cannot be empty*");
    }

    [Fact]
    public void Address_Create_InvalidCity_Throws()
    {
        Action act = () => Address.Create(
            street: "123 Main St",
            city: "   ",
            state: "WA",
            zipCode: "98101"
        );
        
        act.Should().Throw<DomainPrimitiveValidationException>()
           .WithMessage("*City cannot be empty*");
    }

    [Fact]
    public void Address_Create_MultipleInvalidFields_ThrowsFirstValidationError()
    {
        Action act = () => Address.Create(
            street: "   ",
            city: "   ",
            state: "WA",
            zipCode: "98101"
        );
        
        act.Should().Throw<DomainPrimitiveValidationException>()
           .WithMessage("*Street cannot be empty*");
    }

    [Fact]
    public void Address_TryCreate_Valid_ReturnsSuccess()
    {
        var success = Address.TryCreate("123 Main St", "Seattle", "WA", "98101", out var result, out var error);

        success.Should().BeTrue();
        result.Street.Should().Be("123 Main St");
        error.Should().Be(PrimitiveError.None);
    }

    [Fact]
    public void Address_TryCreate_Invalid_ReturnsFailure()
    {
        var success = Address.TryCreate("   ", "Seattle", "WA", "98101", out var result, out var error);

        success.Should().BeFalse();
        result.IsDefault.Should().BeTrue();
        error.Code.Should().Be("Address");
    }

    [Fact]
    public void Address_ToString_FormatsCorrectly()
    {
        var address = Address.Create("123 Main St", "Seattle", "WA", "98101");
        var str = address.ToString();

        str.Should().Contain("Street = 123 Main St");
        str.Should().Contain("City = Seattle");
    }

    [Fact]
    public void Address_Equality_Works()
    {
        var a1 = Address.Create("123 Main St", "Seattle", "WA", "98101");
        var a2 = Address.Create("123 Main St", "Seattle", "WA", "98101");
        var a3 = Address.Create("456 Broad St", "Seattle", "WA", "98101");

        a1.Should().Be(a2);
        a1.Should().NotBe(a3);
        (a1 == a2).Should().BeTrue();
        (a1 != a3).Should().BeTrue();
    }

    [Fact]
    public void Address_Parse_ValidJson_ReturnsInstance()
    {
        var original = Address.Create("123 Main St", "Seattle", "WA", "98101");
        var json = JsonSerializer.Serialize(original);

        var parsed = Address.Parse(json);

        parsed.Should().Be(original);
    }

    [Fact]
    public void Address_TryParse_ValidJson_ReturnsTrue()
    {
        var original = Address.Create("123 Main St", "Seattle", "WA", "98101");
        var json = JsonSerializer.Serialize(original);

        var success = Address.TryParse(json, null, out var parsed);

        success.Should().BeTrue();
        parsed.Should().Be(original);
    }

    [Fact]
    public void Address_TryParse_InvalidJson_ReturnsFalse()
    {
        var success = Address.TryParse("{ invalid json }", null, out var parsed);

        success.Should().BeFalse();
        parsed.IsDefault.Should().BeTrue();
    }

    [Fact]
    public void Address_TryParse_Span_Works()
    {
        var original = Address.Create("123 Main St", "Seattle", "WA", "98101");
        var json = JsonSerializer.Serialize(original);

        var success = Address.TryParse(json.AsSpan(), null, out var parsed);

        success.Should().BeTrue();
        parsed.Should().Be(original);
    }

    [Fact]
    public void Address_TryFormat_SpanChar_Works()
    {
        var address = Address.Create("123 Main St", "Seattle", "WA", "98101");
        Span<char> buffer = stackalloc char[256];

        var success = address.TryFormat(buffer, out var charsWritten);

        success.Should().BeTrue();
        charsWritten.Should().BeGreaterThan(0);
        buffer.Slice(0, charsWritten).ToString().Should().Be(address.ToString());
    }

#if NET8_0_OR_GREATER
    [Fact]
    public void Address_TryParse_Utf8Bytes_Works()
    {
        var original = Address.Create("123 Main St", "Seattle", "WA", "98101");
        var utf8Json = JsonSerializer.SerializeToUtf8Bytes(original);

        var success = Address.TryParse((ReadOnlySpan<byte>)utf8Json, null, out var parsed);

        success.Should().BeTrue();
        parsed.Should().Be(original);
    }

    [Fact]
    public void Address_TryFormat_Utf8Bytes_Works()
    {
        var address = Address.Create("123 Main St", "Seattle", "WA", "98101");
        Span<byte> buffer = stackalloc byte[256];

        var success = address.TryFormat(buffer, out var bytesWritten);

        success.Should().BeTrue();
        bytesWritten.Should().BeGreaterThan(0);
        System.Text.Encoding.UTF8.GetString(buffer.Slice(0, bytesWritten)).Should().Be(address.ToString());
    }
#endif
}
