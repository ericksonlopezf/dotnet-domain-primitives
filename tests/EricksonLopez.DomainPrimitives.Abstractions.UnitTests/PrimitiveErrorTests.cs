// Copyright © Erickson Lopez. MIT License.
using AwesomeAssertions;
using EricksonLopez.DomainPrimitives.Validation;
using Xunit;

namespace EricksonLopez.DomainPrimitives.Abstractions.UnitTests;

public class PrimitiveErrorTests
{
    [Fact]
    public void Create_ShouldReturnConfiguredError()
    {
        var error = PrimitiveError.Create("TEST_CODE", "Test message.");
        error.Code.Should().Be("TEST_CODE");
        error.Message.Should().Be("Test message.");
        error.IsError.Should().BeTrue();
    }

    [Fact]
    public void None_ShouldReturnDefaultError()
    {
        var error = PrimitiveError.None;
        error.Code.Should().BeNull();
        error.Message.Should().BeNull();
        error.IsError.Should().BeFalse();
    }
}




