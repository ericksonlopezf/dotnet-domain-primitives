// Copyright © Erickson Lopez. MIT License.
using System;
using System.Linq;
using AwesomeAssertions;
using EricksonLopez.DomainPrimitives.UnitTests.TestTypes;
using EricksonLopez.DomainPrimitives.Validation;
using Xunit;

namespace EricksonLopez.DomainPrimitives.UnitTests;

public class NumericPrimitiveTests
{
    [Fact]
    public void Score_Create_ValidValue_Works()
    {
        var score = Score.Create(50);
        score.Value.Should().Be(50);
    }

    [Fact]
    public void Score_Create_BelowMin_Throws_WithRangeError()
    {
        Action act = () => Score.Create(-1);
        act.Should().Throw<DomainPrimitiveValidationException>()
            .WithMessage("*Score must be at least 0*")
            .Where(e => e.Error.Code == "RANGE");
    }

    [Fact]
    public void Score_Create_AboveMax_Throws_WithRangeError()
    {
        Action act = () => Score.Create(101);
        act.Should().Throw<DomainPrimitiveValidationException>()
            .WithMessage("*Score must be at most 100*")
            .Where(e => e.Error.Code == "RANGE");
    }

    [Fact]
    public void Score_TryCreate_ValidValue_ReturnsTrue()
    {
        var success = Score.TryCreate(75, out var score, out var error);
        success.Should().BeTrue();
        score.Value.Should().Be(75);
        error.Should().Be(PrimitiveError.None);
    }

    [Fact]
    public void Score_TryCreate_OutOfRange_ReturnsFalse()
    {
        var success = Score.TryCreate(150, out var score, out var error);
        success.Should().BeFalse();
        score.IsDefault.Should().BeTrue();
        error.Code.Should().Be("RANGE");
    }

    [Fact]
    public void Score_TryParse_Valid_Invalid_And_Null_Works()
    {
        Score.TryParse("85", null, out var score).Should().BeTrue();
        score.Value.Should().Be(85);

        Score.TryParse("not-a-number", null, out var invalidScore).Should().BeFalse();
        invalidScore.IsDefault.Should().BeTrue();

        Score.TryParse((string?)null, null, out var nullScore).Should().BeFalse();
        nullScore.IsDefault.Should().BeTrue();
    }

    [Fact]
    public void Distance_Operators_Work()
    {
        var d1 = Distance.Create(10);
        var d2 = Distance.Create(5);

        // AllowAddition
        var added = d1 + d2;
        added.Value.Should().Be(15);

        // AllowScalarMultiplication
        var multiplied1 = d1 * 2;
        var multiplied2 = 2 * d1;
        multiplied1.Value.Should().Be(20);
        multiplied2.Value.Should().Be(20);

        // AllowScalarDivision
    }

    [Fact]
    public void NumericPrimitive_SubtractionOperator_Works()
    {
        // Price allows subtraction through [Money] shortcut
        var p1 = Price.Create(10.50m);
        var p2 = Price.Create(5.25m);

        var subtracted = p1 - p2;
        subtracted.Value.Should().Be(5.25m);
    }

    [Fact]
    public void Distance_Addition_ExceedingRange_Throws()
    {
        var d1 = Distance.Create(600);
        var d2 = Distance.Create(500);

        Action act = () => _ = d1 + d2;
        act.Should().Throw<DomainPrimitiveValidationException>()
            .WithMessage("*Distance must be at most 1000*")
            .Where(e => e.Error.Code == "RANGE");
    }

    [Fact]
    public void Price_MoneyShortcut_Works()
    {
        var p1 = Price.Create(10.50m);
        var p2 = Price.Create(5.25m);

        (p1 + p2).Value.Should().Be(15.75m);
        (p1 - p2).Value.Should().Be(5.25m);
        (p1 * 2).Value.Should().Be(21.00m);
        (2 * p1).Value.Should().Be(21.00m);
        (p1 / 2).Value.Should().Be(5.25m);
    }

    [Fact]
    public void Price_Negative_Throws_WithRangeError()
    {
        Action act = () => Price.Create(-0.01m);
        act.Should().Throw<DomainPrimitiveValidationException>()
            .WithMessage("*Price must be at least 0*")
            .Where(e => e.Error.Code == "RANGE");
    }

    [Fact]
    public void CompletionRate_PercentageShortcut_Works()
    {
        var rate = CompletionRate.Create(75.5m);
        rate.Value.Should().Be(75.5m);

        Action actBelow = () => CompletionRate.Create(-0.1m);
        actBelow.Should().Throw<DomainPrimitiveValidationException>()
            .Where(e => e.Error.Code == "RANGE");

        Action actAbove = () => CompletionRate.Create(100.1m);
        actAbove.Should().Throw<DomainPrimitiveValidationException>()
            .Where(e => e.Error.Code == "RANGE");
    }

    [Fact]
    public void MovieRating_ScaleValidation_Works()
    {
        var rating = MovieRating.Create(4.55m);
        rating.Value.Should().Be(4.55m);

        var success = MovieRating.TryCreate(4.555m, out var result, out var error);
        success.Should().BeFalse();
        result.IsDefault.Should().BeTrue();
        error.Code.Should().Be("FORMAT");
    }

    [Fact]
    public void PrimitiveRangeScore_Validation_Works()
    {
        var score = PrimitiveRangeScore.Create(5.0);
        score.Value.Should().Be(5.0);

        Action act = () => PrimitiveRangeScore.Create(101);
        act.Should().Throw<DomainPrimitiveValidationException>()
            .WithMessage("*PrimitiveRangeScore must be at most 10*")
            .Where(e => e.Error.Code == "RANGE");
    }
}





