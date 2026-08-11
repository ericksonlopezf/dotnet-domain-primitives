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

        Assert.Equal("123 Main St", address.Street);
        Assert.Equal("Seattle", address.City);
        Assert.Equal("WA", address.State);
        Assert.Equal("98101", address.ZipCode);
    }

    [Fact]
    public void Address_Create_Invalid_Throws()
    {
        var ex = Assert.Throws<DomainPrimitiveValidationException>(() => Address.Create(
            street: "   ",
            city: "Seattle",
            state: "WA",
            zipCode: "98101"
        ));
        Assert.Contains("Street cannot be empty", ex.Message);
    }

    [Fact]
    public void Address_TryCreate_Valid_ReturnsSuccess()
    {
        var success = Address.TryCreate("123 Main St", "Seattle", "WA", "98101", out var result, out _);

        // Assert
        success.Should().BeTrue();
        result.Street.Should().Be("123 Main St");
    }

    [Fact]
    public void Address_ToString_FormatsCorrectly()
    {
        var address = Address.Create("123 Main St", "Seattle", "WA", "98101");
        var str = address.ToString();

        Assert.Contains("Street = 123 Main St", str);
        Assert.Contains("City = Seattle", str);
    }

    [Fact]
    public void Address_Equality_Works()
    {
        var a1 = Address.Create("123 Main St", "Seattle", "WA", "98101");
        var a2 = Address.Create("123 Main St", "Seattle", "WA", "98101");
        var a3 = Address.Create("456 Broad St", "Seattle", "WA", "98101");

        Assert.Equal(a1, a2);
        Assert.NotEqual(a1, a3);
        Assert.True(a1 == a2);
        Assert.True(a1 != a3);
    }
}
