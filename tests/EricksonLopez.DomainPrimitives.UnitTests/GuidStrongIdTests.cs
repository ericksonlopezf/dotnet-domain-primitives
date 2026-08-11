using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.DomainPrimitives.Tests.TestTypes;
using FluentAssertions;

namespace EricksonLopez.DomainPrimitives.Tests;

public sealed class GuidStrongIdTests
{
    // ─── Construction ────────────────────────────────────────────────────────

    [Fact]
    public void New_Creates_UniqueId()
    {
        // Arrange & Act
        var id1 = CustomerId.Create();
        var id2 = CustomerId.Create();

        // Assert
        id1.Should().NotBe(id2);
        id1.Value.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Create_Wraps_Value()
    {
        var guid = Guid.NewGuid();
        var id = CustomerId.Create(guid);

        id.Value.Should().Be(guid);
    }

    [Fact]
    public void TryCreate_Returns_Success()
    {
        var guid = Guid.NewGuid();
        var success = CustomerId.TryCreate(guid, out var result, out _);
        success.Should().BeTrue();
        result.Value.Should().Be(guid);
    }

    [Fact]
    public void Empty_Returns_EmptyGuid()
    {
        var empty = CustomerId.Empty;

        empty.IsDefault.Should().BeTrue();
        var act = () => empty.Value;
        act.Should().Throw<InvalidOperationException>();
    }

    // ─── Equality ────────────────────────────────────────────────────────────

    [Fact]
    public void Equal_Ids_Are_Equal()
    {
        var guid = Guid.NewGuid();
        var id1 = CustomerId.Create(guid);
        var id2 = CustomerId.Create(guid);

        id1.Should().Be(id2);
        (id1 == id2).Should().BeTrue();
        (id1 != id2).Should().BeFalse();
    }

    [Fact]
    public void Different_Ids_Are_Not_Equal()
    {
        var id1 = CustomerId.Create();
        var id2 = CustomerId.Create();

        id1.Should().NotBe(id2);
        (id1 != id2).Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_Is_Consistent()
    {
        var guid = Guid.NewGuid();
        var id1 = CustomerId.Create(guid);
        var id2 = CustomerId.Create(guid);

        id1.GetHashCode().Should().Be(id2.GetHashCode());
    }

    // ─── Parsing ─────────────────────────────────────────────────────────────

    [Fact]
    public void Parse_String_Succeeds()
    {
        var guid = Guid.NewGuid();
        var id = CustomerId.Parse(guid.ToString());

        id.Value.Should().Be(guid);
    }

    [Fact]
    public void TryParse_Valid_String_Returns_True()
    {
        var guid = Guid.NewGuid();
        var success = CustomerId.TryParse(guid.ToString(), null, out var id);

        success.Should().BeTrue();
        id.Value.Should().Be(guid);
    }

    [Fact]
    public void TryParse_Invalid_String_Returns_False()
    {
        // Arrange
        const string invalidGuidString = "not-a-valid-guid-format";

        // Act
        var success = CustomerId.TryParse(invalidGuidString, null, out var id);

        // Assert
        success.Should().BeFalse();
        id.Should().Be(default(CustomerId));
    }

    [Fact]
    public void Parse_Span_Succeeds()
    {
        var guid = Guid.NewGuid();
        var id = CustomerId.Parse(guid.ToString().AsSpan());

        id.Value.Should().Be(guid);
    }

    // ─── Formatting ──────────────────────────────────────────────────────────

    [Fact]
    public void ToString_Returns_Guid_String()
    {
        var guid = Guid.NewGuid();
        var id = CustomerId.Create(guid);

        id.ToString().Should().Be(guid.ToString());
    }

    [Fact]
    public void TryFormat_Span_Succeeds()
    {
        var guid = Guid.NewGuid();
        var id = CustomerId.Create(guid);
        Span<char> buffer = stackalloc char[36];

        var success = id.TryFormat(buffer, out int charsWritten);

        success.Should().BeTrue();
        charsWritten.Should().Be(36);
        new string(buffer).Should().Be(guid.ToString());
    }

    // ─── Operators ───────────────────────────────────────────────────────────

    [Fact]
    public void Explicit_Operator_To_Guid()
    {
        var guid = Guid.NewGuid();
        var id = CustomerId.Create(guid);

        var extracted = (Guid)id;
        extracted.Should().Be(guid);
    }

    [Fact]
    public void Explicit_Operator_From_Guid()
    {
        var guid = Guid.NewGuid();
        var id = (CustomerId)guid;

        id.Value.Should().Be(guid);
    }

    // ─── Comparison ──────────────────────────────────────────────────────────

    [Fact]
    public void CompareTo_Works()
    {
        var guid1 = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var guid2 = Guid.Parse("00000000-0000-0000-0000-000000000002");
        var id1 = CustomerId.Create(guid1);
        var id2 = CustomerId.Create(guid2);

        (id1 < id2).Should().BeTrue();
        (id2 > id1).Should().BeTrue();
        
        var id1Copy = CustomerId.Create(guid1);
        (id1 <= id1Copy).Should().BeTrue();
        (id1 >= id1Copy).Should().BeTrue();
    }

    // ─── Dictionary / HashSet ────────────────────────────────────────────────

    [Fact]
    public void Works_In_Dictionary()
    {
        // Arrange
        var id = CustomerId.Create();
        const string expectedValue = "SampleValue";
        var dict = new Dictionary<CustomerId, string>();

        // Act
        dict[id] = expectedValue;

        // Assert
        dict.Should().ContainKey(id);
        dict[id].Should().Be(expectedValue);
    }

    [Fact]
    public void Works_In_HashSet()
    {
        var id = CustomerId.Create();
        var set = new HashSet<CustomerId> { id };

        set.Should().Contain(id);
        set.Add(CustomerId.Create(id.Value)).Should().BeFalse(); // duplicate
    }

    // ─── PrimitiveName ───────────────────────────────────────────────────────

    [Fact]
    public void PrimitiveName_Returns_TypeName()
    {
        CustomerId.PrimitiveName.Should().Be("CustomerId");
    }
}
