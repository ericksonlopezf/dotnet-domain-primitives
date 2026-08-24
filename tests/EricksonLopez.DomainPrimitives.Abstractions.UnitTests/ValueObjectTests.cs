// Copyright © Erickson Lopez. MIT License.
using AwesomeAssertions;
using EricksonLopez.DomainPrimitives;
using Xunit;

#pragma warning disable CS8602, CS8625
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

        vo.Equals((TestValueObject?)null).Should().BeFalse();
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

    private sealed record class NestedCompoundValueObject : ValueObject
    {
        public TestValueObject Child { get; }
        public string Tag { get; }

        public NestedCompoundValueObject(TestValueObject child, string tag)
        {
            Child = child;
            Tag = tag;
        }
    }

    [Fact]
    public void Equals_WithNestedCompoundValueObjects_ShouldEvaluateStructuralEquality()
    {
        var compound1 = new NestedCompoundValueObject(new TestValueObject(10, "nested"), "tagA");
        var compound2 = new NestedCompoundValueObject(new TestValueObject(10, "nested"), "tagA");
        var compound3 = new NestedCompoundValueObject(new TestValueObject(20, "nested"), "tagA");

        compound1.Should().Be(compound2);
        compound1.GetHashCode().Should().Be(compound2.GetHashCode());
        compound1.Should().NotBe(compound3);
        (compound1 == compound2).Should().BeTrue();
        (compound1 != compound3).Should().BeTrue();
    }

    [Fact]
    public void Equals_MathematicalAxioms_ReflexiveSymmetricTransitive()
    {
        var x = new TestValueObject(5, "axiom");
        var y = new TestValueObject(5, "axiom");
        var z = new TestValueObject(5, "axiom");

        // Reflexive: x == x
        x.Equals(x).Should().BeTrue();

        // Symmetric: x == y implies y == x
        x.Equals(y).Should().BeTrue();
        y.Equals(x).Should().BeTrue();

        // Transitive: x == y and y == z implies x == z
        x.Equals(y).Should().BeTrue();
        y.Equals(z).Should().BeTrue();
        x.Equals(z).Should().BeTrue();
    }
}


