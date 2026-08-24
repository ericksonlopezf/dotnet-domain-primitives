// Copyright © Erickson Lopez. MIT License.
using AwesomeAssertions;
using EricksonLopez.DomainPrimitives.Validation;
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

    [Theory]
    [InlineData("email", "EMAIL_INVALID", "Email format is invalid")]
    [InlineData("age", "OUT_OF_RANGE", "Value must be between 18 and 99")]
    [InlineData("username", "TOO_SHORT", "Minimum 3 characters required")]
    public void Constructor_WithCustomParamName_ShouldPreserveParamNameAndFormattedMessage(
        string paramName, string errorCode, string errorMessage)
    {
        // Arrange
        var error = new PrimitiveError(errorCode, errorMessage);

        // Act
        var ex = new DomainPrimitiveValidationException(error, paramName);

        // Assert
        ex.Error.Should().Be(error);
        ex.ParamName.Should().Be(paramName);
        ex.Message.Should().StartWith($"[{errorCode}] {errorMessage}");
        ex.Error.Code.Should().Be(errorCode);
        ex.Error.Message.Should().Be(errorMessage);
    }

    [Fact]
    public void Constructor_WithEmptyError_ShouldFormatProperly()
    {
        // Arrange
        var error = PrimitiveError.None;

        // Act
        var ex = new DomainPrimitiveValidationException(error);

        // Assert
        ex.Error.Should().Be(PrimitiveError.None);
        ex.Message.Should().StartWith("[] ");
    }
}


