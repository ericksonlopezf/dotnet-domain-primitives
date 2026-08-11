using System;
using EricksonLopez.DomainPrimitives;
using FluentAssertions;
using Xunit;

namespace EricksonLopez.DomainPrimitives.UnitTests;

public class PrimitiveAttributesTests
{
    [Fact]
    public void MaxLengthAttribute_ShouldStoreLength()
    {
        var attr = new MaxLengthAttribute(50);
        attr.Length.Should().Be(50);
    }

    [Fact]
    public void MinLengthAttribute_ShouldStoreLength()
    {
        var attr = new MinLengthAttribute(5);
        attr.Length.Should().Be(5);
    }

    [Fact]
    public void RegexAttribute_ShouldStorePattern()
    {
        var pattern = "^[A-Z]+$";
        var attr = new RegexAttribute(pattern);
        attr.Pattern.Should().Be(pattern);
    }

    [Fact]
    public void NumericPrimitiveAttribute_ShouldSetProperties()
    {
        var attr = new NumericPrimitiveAttribute<int>
        {
            Operations = NumericOperations.Addition | NumericOperations.Subtraction | NumericOperations.ScalarMultiplication | NumericOperations.ScalarDivision | NumericOperations.Negation
        };

        attr.Operations.HasFlag(NumericOperations.Addition).Should().BeTrue();
        attr.Operations.HasFlag(NumericOperations.Subtraction).Should().BeTrue();
        attr.Operations.HasFlag(NumericOperations.ScalarMultiplication).Should().BeTrue();
        attr.Operations.HasFlag(NumericOperations.ScalarDivision).Should().BeTrue();
        attr.Operations.HasFlag(NumericOperations.Negation).Should().BeTrue();
    }
}
