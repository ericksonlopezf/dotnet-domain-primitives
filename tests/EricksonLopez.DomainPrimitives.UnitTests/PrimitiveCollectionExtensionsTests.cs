using System;
using System.Collections.Generic;
using System.Linq;
using EricksonLopez.DomainPrimitives;
using FluentAssertions;
using Xunit;

namespace EricksonLopez.DomainPrimitives.UnitTests;

public class PrimitiveCollectionExtensionsTests
{
    private readonly record struct TestPrimitive(int Value) : IDomainPrimitive<TestPrimitive, int>
    {
        public static string PrimitiveName => "TestPrimitive";
        public bool IsDefault => false;

        public static TestPrimitive Create(int value)
        {
            if (value < 0) throw new DomainPrimitiveValidationException(new EricksonLopez.DomainPrimitives.Validation.PrimitiveError("TestPrimitive", "Must be positive"));
            return new TestPrimitive(value);
        }

        public static bool TryCreate(int value, out TestPrimitive result, out EricksonLopez.DomainPrimitives.Validation.PrimitiveError validationError)
        {
            if (value < 0)
            {
                result = default;
                validationError = new EricksonLopez.DomainPrimitives.Validation.PrimitiveError("TestPrimitive", "Must be positive");
                return false;
            }
            result = new TestPrimitive(value);
            validationError = EricksonLopez.DomainPrimitives.Validation.PrimitiveError.None;
            return true;
        }
    }

    [Fact]
    public void ToDomainPrimitiveList_WhenCollection_ShouldExtractValues()
    {
        var values = new[] { 1, 2, 3 };
        var result = values.ToDomainPrimitiveList<TestPrimitive, int>();
        
        result.Should().HaveCount(3);
        result[0].Value.Should().Be(1);
    }
    
    [Fact]
    public void ToDomainPrimitiveList_WhenNotCollection_ShouldExtractValues()
    {
        var result = GetValues().ToDomainPrimitiveList<TestPrimitive, int>();
        
        result.Should().HaveCount(3);
        result[0].Value.Should().Be(1);
        
        static IEnumerable<int> GetValues()
        {
            yield return 1;
            yield return 2;
            yield return 3;
        }
    }

    [Fact]
    public void ToDomainPrimitiveList_WhenValueIsInvalid_ShouldThrow()
    {
        var values = new[] { 1, -1, 3 };
        Action act = () => values.ToDomainPrimitiveList<TestPrimitive, int>();
        act.Should().Throw<DomainPrimitiveValidationException>();
    }

    [Fact]
    public void ToDomainPrimitiveArray_WhenCollection_ShouldExtractValues()
    {
        var values = new[] { 1, 2, 3 };
        var result = values.ToDomainPrimitiveArray<TestPrimitive, int>();

        result.Should().HaveCount(3);
        result[0].Value.Should().Be(1);
    }
    
    [Fact]
    public void ToDomainPrimitiveArray_WhenNotCollection_ShouldExtractValues()
    {
        var result = GetValues().ToDomainPrimitiveArray<TestPrimitive, int>();

        result.Should().HaveCount(3);
        result[0].Value.Should().Be(1);
        
        static IEnumerable<int> GetValues()
        {
            yield return 1;
            yield return 2;
            yield return 3;
        }
    }

    [Fact]
    public void ToDomainPrimitiveArray_WhenValueIsInvalid_ShouldThrow()
    {
        var values = new[] { 1, -1, 3 };
        Action act = () => values.ToDomainPrimitiveArray<TestPrimitive, int>();
        act.Should().Throw<DomainPrimitiveValidationException>();
    }

    [Fact]
    public void ToDomainPrimitiveArray_FromSpan_ShouldExtractValues()
    {
        var values = new[] { 1, 2, 3 }.AsSpan();
        var result = values.ToDomainPrimitiveArray<TestPrimitive, int>();

        result.Should().HaveCount(3);
        result[0].Value.Should().Be(1);
    }

    [Fact]
    public void ToDomainPrimitiveArray_FromSpan_WhenValueIsInvalid_ShouldThrow()
    {
        Action act = () => new[] { 1, -1, 3 }.AsSpan().ToDomainPrimitiveArray<TestPrimitive, int>();
        act.Should().Throw<DomainPrimitiveValidationException>();
    }
}
