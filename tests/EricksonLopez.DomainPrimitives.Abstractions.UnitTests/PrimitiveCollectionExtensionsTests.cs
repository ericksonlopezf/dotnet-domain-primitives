// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;
using AwesomeAssertions;
using EricksonLopez.DomainPrimitives;
using Xunit;

namespace EricksonLopez.DomainPrimitives.Abstractions.UnitTests;

public class PrimitiveCollectionExtensionsTests
{
    [Fact]
    public void ToDomainPrimitiveList_FromEnumerable_ShouldConvertValues()
    {
        // Arrange
        IEnumerable<int> values = new[] { 1, 2, 3 };

        // Act
        var result = values.ToDomainPrimitiveList<TestPrimitive, int>();

        // Assert
        result.Should().HaveCount(3);
        result[0].Value.Should().Be(1);
        result[1].Value.Should().Be(2);
        result[2].Value.Should().Be(3);
    }

    [Fact]
    public void ToDomainPrimitiveList_FromNonICollection_ShouldConvertValues()
    {
        // Arrange
        IEnumerable<int> GetValues()
        {
            yield return 1;
            yield return 2;
            yield return 3;
        }

        // Act
        var result = GetValues().ToDomainPrimitiveList<TestPrimitive, int>();

        // Assert
        result.Should().HaveCount(3);
        result[0].Value.Should().Be(1);
        result[1].Value.Should().Be(2);
        result[2].Value.Should().Be(3);
    }

    [Fact]
    public void ToDomainPrimitiveList_FromEnumerable_WithInvalidValue_ShouldThrow()
    {
        // Arrange
        IEnumerable<int> values = new[] { 1, -2, 3 };

        // Act
        Action act = () => values.ToDomainPrimitiveList<TestPrimitive, int>();

        // Assert — LOW-04: InvalidValue wraps as ArgumentException with element index context.
        // The outer exception includes the 0-based index of the offending element.
        // The inner exception is the original DomainPrimitiveValidationException.
        act.Should().Throw<ArgumentException>()
           .WithMessage("Element at index 1 failed validation:*")
           .WithInnerException<DomainPrimitiveValidationException>()
           .WithMessage("[Invalid] Value cannot be negative.*");
    }

    [Fact]
    public void ToDomainPrimitiveArray_FromICollection_ShouldConvertValues()
    {
        // Arrange
        ICollection<int> values = new List<int> { 1, 2, 3 };

        // Act
        var result = values.ToDomainPrimitiveArray<TestPrimitive, int>();

        // Assert
        result.Should().HaveCount(3);
        result[0].Value.Should().Be(1);
        result[1].Value.Should().Be(2);
        result[2].Value.Should().Be(3);
    }

    [Fact]
    public void ToDomainPrimitiveArray_FromIEnumerable_ShouldConvertValues()
    {
        // Arrange
        IEnumerable<int> GetValues()
        {
            yield return 1;
            yield return 2;
            yield return 3;
        }

        // Act
        var result = GetValues().ToDomainPrimitiveArray<TestPrimitive, int>();

        // Assert
        result.Should().HaveCount(3);
        result[0].Value.Should().Be(1);
        result[1].Value.Should().Be(2);
        result[2].Value.Should().Be(3);
    }

    [Fact]
    public void ToDomainPrimitiveArray_FromSpan_ShouldConvertValues()
    {
        // Arrange
        int[] rawArray = new[] { 1, 2, 3 };
        ReadOnlySpan<int> span = rawArray.AsSpan();

        // Act
        var result = span.ToDomainPrimitiveArray<TestPrimitive, int>();

        // Assert
        result.Should().HaveCount(3);
        result[0].Value.Should().Be(1);
        result[1].Value.Should().Be(2);
        result[2].Value.Should().Be(3);
    }

    [Fact]
    public void ToDomainPrimitiveList_FromEmptyEnumerable_ShouldReturnEmptyList()
    {
        // Arrange
        IEnumerable<int> values = Array.Empty<int>();

        // Act
        var result = values.ToDomainPrimitiveList<TestPrimitive, int>();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public void ToDomainPrimitiveArray_FromEmptyICollection_ShouldReturnEmptyArray()
    {
        // Arrange
        ICollection<int> values = new List<int>();

        // Act
        var result = values.ToDomainPrimitiveArray<TestPrimitive, int>();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public void ToDomainPrimitiveArray_FromEmptySpan_ShouldReturnEmptyArray()
    {
        // Arrange
        ReadOnlySpan<int> span = ReadOnlySpan<int>.Empty;

        // Act
        var result = span.ToDomainPrimitiveArray<TestPrimitive, int>();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}





