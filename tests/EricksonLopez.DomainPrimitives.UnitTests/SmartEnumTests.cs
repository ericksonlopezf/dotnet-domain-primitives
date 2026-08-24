// Copyright © Erickson Lopez. MIT License.
using System;
using AwesomeAssertions;
using EricksonLopez.DomainPrimitives.UnitTests.TestTypes;
using Xunit;

namespace EricksonLopez.DomainPrimitives.UnitTests;

public class SmartEnumTests
{
    [Fact]
    public void Match_ExecutesCorrectCase()
    {
        var status = TestOrderStatus.Processing;

        var result = status.Match(
            whenPending: () => "PendingCase",
            whenProcessing: () => "ProcessingCase",
            whenCompleted: () => "CompletedCase"
        );

        result.Should().Be("ProcessingCase");
    }

    [Fact]
    public void Switch_ExecutesCorrectAction()
    {
        var status = TestOrderStatus.Completed;
        var executed = false;

        status.Switch(
            whenPending: () => { },
            whenProcessing: () => { },
            whenCompleted: () => { executed = true; }
        );

        executed.Should().BeTrue();
    }

    [Fact]
    public void TryFromValue_ReturnsTrue_ForValidValue()
    {
        TestOrderStatus.TryFromValue(1, out var status).Should().BeTrue();
        status.Should().Be(TestOrderStatus.Pending);
    }

    [Fact]
    public void TryFromValue_ReturnsFalse_ForInvalidValue()
    {
        TestOrderStatus.TryFromValue(999, out var status).Should().BeFalse();
        status.Should().Be(default(TestOrderStatus));
    }

    [Fact]
    public void FromValue_ReturnsMember_WhenValueMatches()
    {
        var status = TestOrderStatus.FromValue(1);
        status.Should().Be(TestOrderStatus.Pending);
    }

    [Fact]
    public void FromValue_ThrowsArgumentException_WhenValueNotFound()
    {
        var act = () => TestOrderStatus.FromValue(999);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*No TestOrderStatus found with value 999*");
    }

    [Fact]
    public void Map_ExecutesCorrectCase_AndPassesMember()
    {
        var status = TestOrderStatus.Processing;

        var result = status.Map(
            whenPending: s => $"{s.Name}_Mapped",
            whenProcessing: s => $"{s.Name}_Mapped",
            whenCompleted: s => $"{s.Name}_Mapped"
        );

        result.Should().Be("Processing_Mapped");
    }

    [Fact]
    public void TryFromName_CaseInsensitive_ReturnsTrue()
    {
        TestOrderStatus.TryFromName("completed", ignoreCase: true, out var status).Should().BeTrue();
        status.Should().Be(TestOrderStatus.Completed);
    }

    [Fact]
    public void TryFromName_CaseSensitive_ReturnsFalse_WhenCaseDoesNotMatch()
    {
        TestOrderStatus.TryFromName("completed", ignoreCase: false, out var status).Should().BeFalse();
        status.Should().Be(default(TestOrderStatus));
    }

    [Fact]
    public void FromName_ReturnsMember_WhenNameMatches()
    {
        var status = TestOrderStatus.FromName("Completed");
        status.Should().Be(TestOrderStatus.Completed);
    }

    [Fact]
    public void FromName_WithIgnoreCase_ReturnsMember()
    {
        var status = TestOrderStatus.FromName("processing", ignoreCase: true);
        status.Should().Be(TestOrderStatus.Processing);
    }

    [Fact]
    public void FromName_ThrowsArgumentException_WhenNameNotFound()
    {
        var act = () => TestOrderStatus.FromName("NonExistentStatus");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Parse_And_TryParse_String_Works()
    {
        var parsed = TestOrderStatus.Parse("Pending", null);
        parsed.Should().Be(TestOrderStatus.Pending);

        TestOrderStatus.TryParse("Completed", null, out var status).Should().BeTrue();
        status.Should().Be(TestOrderStatus.Completed);

        TestOrderStatus.TryParse("InvalidName", null, out var invalidStatus).Should().BeFalse();
        invalidStatus.Should().Be(default(TestOrderStatus));

        TestOrderStatus.TryParse((string?)null, null, out var nullStatus).Should().BeFalse();
        nullStatus.Should().Be(default(TestOrderStatus));
    }

    [Fact]
    public void All_ContainsAllDefinedMembers()
    {
        var all = TestOrderStatus.All;

        all.Should().NotBeNull();
        all.Should().HaveCount(3);
        all.Should().Contain(TestOrderStatus.Pending);
        all.Should().Contain(TestOrderStatus.Processing);
        all.Should().Contain(TestOrderStatus.Completed);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void TryFromName_WithWhitespaceStrings_ReturnsFalse(string invalidName)
    {
        TestOrderStatus.TryFromName(invalidName, out var status).Should().BeFalse();
        status.Should().Be(default(TestOrderStatus));
    }

    [Theory]
    [InlineData("Unknown")]
    [InlineData("PENDING_EXTRA")]
    public void TryFromName_WithInvalidStrings_ReturnsFalse(string invalidName)
    {
        TestOrderStatus.TryFromName(invalidName, out var status).Should().BeFalse();
        status.Should().Be(default(TestOrderStatus));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    public void TryFromValue_WithBoundaryValues_ReturnsFalse(int outOfRangeValue)
    {
        TestOrderStatus.TryFromValue(outOfRangeValue, out var status).Should().BeFalse();
        status.Should().Be(default(TestOrderStatus));
    }
}




