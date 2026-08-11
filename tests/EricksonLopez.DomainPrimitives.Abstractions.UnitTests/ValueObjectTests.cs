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

#pragma warning disable CS8602

namespace EricksonLopez.DomainPrimitives.Abstractions.UnitTests;

public class ValueObjectTests
{
    private sealed record class TestValueObject : ValueObject
    {
        public int A { get; }
        public string B { get; }

        public TestValueObject(int a, string b)
        {
            A = a;
            B = b;
        }
    }

    private sealed record class OtherValueObject : ValueObject
    {
        public int A { get; }
        public string B { get; }

        public OtherValueObject(int a, string b)
        {
            A = a;
            B = b;
        }
    }

    [Fact]
    public void Equals_WithSameComponents_ReturnsTrue()
    {
        var vo1 = new TestValueObject(1, "test");
        var vo2 = new TestValueObject(1, "test");

        vo1.Equals(vo2).Should().BeTrue();
        vo1.Equals((object)vo2).Should().BeTrue();
        (vo1 == vo2).Should().BeTrue();
        (vo1 != vo2).Should().BeFalse();
    }

    [Fact]
    public void Equals_WithDifferentComponents_ReturnsFalse()
    {
        var vo1 = new TestValueObject(1, "test");
        var vo2 = new TestValueObject(2, "test");

        vo1.Equals(vo2).Should().BeFalse();
        (vo1 == vo2).Should().BeFalse();
        (vo1 != vo2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithNull_ReturnsFalse()
    {
        var vo = new TestValueObject(1, "test");

        vo.Equals(null).Should().BeFalse();
        vo.Equals((object?)null).Should().BeFalse();
        (vo == null).Should().BeFalse();
        (null == vo).Should().BeFalse();
        (vo != null).Should().BeTrue();
        (null != vo).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentType_ReturnsFalse()
    {
        var vo1 = new TestValueObject(1, "test");
        var vo2 = new OtherValueObject(1, "test");

        vo1.Equals(vo2).Should().BeFalse();
        vo1.Equals((object)vo2).Should().BeFalse();
        (vo1 == vo2).Should().BeFalse();
        (vo1 != vo2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentObjectType_ReturnsFalse()
    {
        var vo = new TestValueObject(1, "test");
        vo.Equals(new object()).Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_WithSameComponents_ReturnsSameHash()
    {
        var vo1 = new TestValueObject(1, "test");
        var vo2 = new TestValueObject(1, "test");

        vo1.GetHashCode().Should().Be(vo2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_WithDifferentComponents_ReturnsDifferentHash()
    {
        var vo1 = new TestValueObject(1, "test");
        var vo2 = new TestValueObject(2, "test");

        vo1.GetHashCode().Should().NotBe(vo2.GetHashCode());
    }

    [Fact]
    public void EqualityOperators_WhenBothNull_ReturnsTrue()
    {
        TestValueObject? vo1 = null;
        TestValueObject? vo2 = null;

        (vo1 == vo2).Should().BeTrue();
        (vo1 != vo2).Should().BeFalse();
    }
}
