using EricksonLopez.DomainPrimitives.Validation;
using FluentAssertions;
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
}
