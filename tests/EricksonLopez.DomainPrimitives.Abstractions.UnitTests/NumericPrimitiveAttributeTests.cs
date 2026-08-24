// Copyright © Erickson Lopez. MIT License.
using AwesomeAssertions;
using EricksonLopez.DomainPrimitives;
using Xunit;

namespace EricksonLopez.DomainPrimitives.Abstractions.UnitTests;

public class NumericPrimitiveAttributeTests
{
    [Fact]
    public void Constructor_Default_ShouldSetOperationsToNone()
    {
        // Act
        var attr = new NumericPrimitiveAttribute<int>();

        // Assert
        attr.Operations.Should().Be(NumericOperations.None);
    }

    [Fact]
    public void Constructor_WithOperations_ShouldSetOperations()
    {
        // Act
        var attr = new NumericPrimitiveAttribute<int> { Operations = NumericOperations.All };

        // Assert
        attr.Operations.Should().Be(NumericOperations.All);
    }
}
