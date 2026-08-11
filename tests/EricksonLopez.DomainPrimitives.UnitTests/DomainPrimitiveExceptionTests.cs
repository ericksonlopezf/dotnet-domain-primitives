using System;
using EricksonLopez.DomainPrimitives;
using FluentAssertions;
using Xunit;

namespace EricksonLopez.DomainPrimitives.UnitTests;

public class DomainPrimitiveValidationExceptionTests
{
    [Fact]
    public void Constructor_ShouldSetProperties_Correctly()
    {
        // Arrange
        var primitiveName = "MyPrimitive";
        var errorMessage = "Invalid value provided.";
        var error = new EricksonLopez.DomainPrimitives.Validation.PrimitiveError(primitiveName, errorMessage);

        // Act
        var exception = new DomainPrimitiveValidationException(error);

        // Assert
        exception.Error.Code.Should().Be(primitiveName);
        exception.Message.Should().Contain(errorMessage);
        exception.Message.Should().Contain(primitiveName);
    }
}
