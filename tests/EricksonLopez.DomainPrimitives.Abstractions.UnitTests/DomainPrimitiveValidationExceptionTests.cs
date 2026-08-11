using EricksonLopez.DomainPrimitives.Validation;
using FluentAssertions;
using Xunit;

namespace EricksonLopez.DomainPrimitives.Abstractions.UnitTests;

public class DomainPrimitiveValidationExceptionTests
{
    [Fact]
    public void Constructor_ShouldSetErrorProperty()
    {
        // Arrange
        var error = new PrimitiveError("TEST_CODE", "Test message");

        // Act
        var ex = new DomainPrimitiveValidationException(error);

        // Assert
        ex.Error.Should().Be(error);
        ex.Message.Should().StartWith("[TEST_CODE] Test message");
        ex.ParamName.Should().Be("value");
    }
}
