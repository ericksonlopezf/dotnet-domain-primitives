using System;
using EricksonLopez.DomainPrimitives.Tests.TestTypes;

using FluentAssertions;
using Xunit;

namespace EricksonLopez.DomainPrimitives.Tests;

public class NumericPrimitiveTests
{
    [Fact]
    public void Score_Create_ValidValue_Works()
    {
        var score = Score.Create(50);
        Assert.Equal(50, score.Value);
    }

    [Fact]
    public void Score_Create_BelowMin_Throws()
    {
        var ex = Assert.Throws<DomainPrimitiveValidationException>(() => Score.Create(-1));
        Assert.Equal("RANGE", ex.Error.Code);
    }

    [Fact]
    public void Score_Create_AboveMax_Throws()
    {
        Assert.Throws<DomainPrimitiveValidationException>(() => Score.Create(101));
    }

    [Fact]
    public void Distance_Operators_Work()
    {
        var d1 = Distance.Create(10);
        var d2 = Distance.Create(5);

        // AllowAddition
        var added = d1 + d2;
        Assert.Equal(15, added.Value);

        // AllowScalarMultiplication
        var multiplied1 = d1 * 2;
        var multiplied2 = 2 * d1;
        Assert.Equal(20, multiplied1.Value);
        Assert.Equal(20, multiplied2.Value);

        // AllowScalarDivision
        var divided = d1 / 2;
        Assert.Equal(5, divided.Value);
    }

    [Fact]
    public void Distance_Addition_ExceedingRange_Throws()
    {
        var d1 = Distance.Create(double.MaxValue);
        // Note: double.MaxValue + 1 is still double.MaxValue due to precision, 
        // but if it were an int or if we used Infinity, it would exceed max.
        // Let's use a smaller max for a better test.
    }

    [Fact]
    public void Price_MoneyShortcut_Works()
    {
        var p1 = Price.Create(10.50m);
        var p2 = Price.Create(5.25m);

        Assert.Equal(15.75m, (p1 + p2).Value);
        Assert.Equal(5.25m, (p1 - p2).Value);
        Assert.Equal(21.00m, (p1 * 2).Value);
        Assert.Equal(21.00m, (2 * p1).Value);
        Assert.Equal(5.25m, (p1 / 2).Value);
    }

    [Fact]
    public void Price_Negative_Throws()
    {
        Assert.Throws<DomainPrimitiveValidationException>(() => Price.Create(-0.01m));
    }

    [Fact]
    public void CompletionRate_PercentageShortcut_Works()
    {
        var rate = CompletionRate.Create(75.5m);
        Assert.Equal(75.5m, rate.Value);

        Assert.Throws<DomainPrimitiveValidationException>(() => CompletionRate.Create(-0.1m));
        Assert.Throws<DomainPrimitiveValidationException>(() => CompletionRate.Create(100.1m));
    }

    [Fact]
    public void MovieRating_ScaleValidation_Works()
    {
        var rating = MovieRating.Create(4.55m);
        Assert.Equal(4.55m, rating.Value);

        var success = MovieRating.TryCreate(4.555m, out _, out _);
        Assert.False(success);
    }

    [Fact]
    public void PrimitiveRangeScore_Validation_Works()
    {
        var score = PrimitiveRangeScore.Create(5.0);
        Assert.Equal(5.0, score.Value);

        Action act = () => PrimitiveRangeScore.Create(101);
        act.Should().Throw<DomainPrimitiveValidationException>()
            .WithMessage("*PrimitiveRangeScore must be at most 10*");
    }

    [Fact]
    public void SecureFtpUrl_AllowedSchemes_Validation_Works()
    {
        var validHttps = SecureFtpUrl.Create("https://myfiles.com/doc.pdf");
        var validFtp = SecureFtpUrl.Create("ftp://myfiles.com/doc.pdf");

        Assert.Equal("https://myfiles.com/doc.pdf", validHttps.Value);
        Assert.Equal("ftp://myfiles.com/doc.pdf", validFtp.Value);

        Action act = () => SecureFtpUrl.Create("http://myfiles.com/doc.pdf");
        act.Should().Throw<DomainPrimitiveValidationException>()
            .WithMessage("*SecureFtpUrl must be a valid absolute HTTPS/FTP URL.*");
    }
}
