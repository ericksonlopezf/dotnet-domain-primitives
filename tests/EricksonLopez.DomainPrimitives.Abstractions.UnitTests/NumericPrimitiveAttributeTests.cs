using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.DomainPrimitives;
using FluentAssertions;
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

#pragma warning disable CS0618
    [Fact]
    public void Constructor_WithPolicy_ShouldSetOperations()
    {
        // Act
        var attr = new NumericPrimitiveAttribute<int> { Policy = ArithmeticPolicy.Additive };

        // Assert
        attr.Operations.Should().Be(NumericOperations.Additive);
        attr.Policy.Should().Be(ArithmeticPolicy.Additive);
    }
#pragma warning restore CS0618
}
