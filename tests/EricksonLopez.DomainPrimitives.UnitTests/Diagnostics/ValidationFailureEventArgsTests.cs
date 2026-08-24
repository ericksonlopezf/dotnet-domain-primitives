// Copyright © Erickson Lopez. MIT License.
using System;
using AwesomeAssertions;
using EricksonLopez.DomainPrimitives.Diagnostics;
using Xunit;

namespace EricksonLopez.DomainPrimitives.UnitTests.Diagnostics;

public class ValidationFailureEventArgsTests
{
    [Fact]
    public void Constructor_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var primitiveName = "TestPrimitive";
        var errorType = "TestError";
        var errorMessage = "This is a test error message.";

        // Act
        var args = new ValidationFailureEventArgs(primitiveName, errorType, errorMessage);

        // Assert
        args.PrimitiveName.Should().Be(primitiveName);
        args.ErrorType.Should().Be(errorType);
        args.ErrorMessage.Should().Be(errorMessage);
    }
}


