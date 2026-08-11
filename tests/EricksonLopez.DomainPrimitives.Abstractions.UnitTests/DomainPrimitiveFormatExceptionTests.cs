using System;
using EricksonLopez.DomainPrimitives;
using FluentAssertions;
using Xunit;

namespace EricksonLopez.DomainPrimitives.Abstractions.UnitTests;

public class DomainPrimitiveFormatExceptionTests
{
    [Fact]
    public void Constructor_WithMessage_ShouldSetProperties()
    {
        // Act
        var ex = new DomainPrimitiveFormatException("TestName", "Message");

        // Assert
        ex.PrimitiveName.Should().Be("TestName");
        ex.Message.Should().Be("Message");
        ex.InnerException.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_ShouldSetProperties()
    {
        // Arrange
        var inner = new InvalidOperationException("Inner");

        // Act
        var ex = new DomainPrimitiveFormatException("TestName", "Message", inner);

        // Assert
        ex.PrimitiveName.Should().Be("TestName");
        ex.Message.Should().Be("Message");
        ex.InnerException.Should().BeSameAs(inner);
    }
}
