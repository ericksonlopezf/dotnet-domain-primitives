using System;
using System.Linq;
using System.Reflection;
using EricksonLopez.DomainPrimitives;
using FluentAssertions;
using Xunit;

namespace EricksonLopez.DomainPrimitives.Abstractions.UnitTests;

public class AttributesTests
{
    [Fact]
    public void MoneyAttribute_Defaults_ShouldBeCorrect()
    {
        var attr = new MoneyAttribute();
        attr.Currency.Should().Be("USD");
        attr.Min.Should().Be(0);
        attr.Max.Should().Be(double.MaxValue);
    }

    [Fact]
    public void MoneyAttribute_CustomCurrency_ShouldBeCorrect()
    {
        var attr = new MoneyAttribute("EUR");
        attr.Currency.Should().Be("EUR");
        attr.Min.Should().Be(0);
        attr.Max.Should().Be(double.MaxValue);
    }

    [Fact]
    public void MoneyAttribute_CustomAll_ShouldBeCorrect()
    {
        var attr = new MoneyAttribute("GBP", 10.5, 100.5);
        attr.Currency.Should().Be("GBP");
        attr.Min.Should().Be(10.5);
        attr.Max.Should().Be(100.5);
    }

    [Fact]
    public void PercentageAttribute_Defaults_ShouldBeCorrect()
    {
        var attr = new PercentageAttribute();
        attr.Min.Should().Be(0);
        attr.Max.Should().Be(100);
    }

    [Fact]
    public void WeightAttribute_Defaults_ShouldBeCorrect()
    {
        var attr = new WeightAttribute();
        attr.Min.Should().Be(0);
        attr.Max.Should().Be(1_000);
    }

    [Fact]
    public void HeightAttribute_Defaults_ShouldBeCorrect()
    {
        var attr = new HeightAttribute();
        attr.Min.Should().Be(0);
        attr.Max.Should().Be(300);
    }

    [Fact]
    public void DistanceAttribute_Defaults_ShouldBeCorrect()
    {
        var attr = new DistanceAttribute();
        attr.Min.Should().Be(0);
        attr.Max.Should().Be(double.MaxValue);
    }

    [Fact]
    public void TemperatureAttribute_Defaults_ShouldBeCorrect()
    {
        var attr = new TemperatureAttribute();
        attr.Min.Should().Be(-273.15);
        attr.Max.Should().Be(double.MaxValue);
    }

    [Fact]
    public void ScoreAttribute_Defaults_ShouldBeCorrect()
    {
        var attr = new ScoreAttribute();
        attr.Min.Should().Be(0);
        attr.Max.Should().Be(100);
    }

    [Fact]
    public void QuantityAttribute_Defaults_ShouldBeCorrect()
    {
        var attr = new QuantityAttribute();
        attr.Min.Should().Be(0);
        attr.Max.Should().Be(int.MaxValue);
    }

    [Fact]
    public void PriceAttribute_Defaults_ShouldBeCorrect()
    {
        var attr = new PriceAttribute();
        attr.Min.Should().Be(0);
        attr.Max.Should().Be(double.MaxValue);
    }

    [Fact]
    public void TaxRateAttribute_Defaults_ShouldBeCorrect()
    {
        var attr = new TaxRateAttribute();
        attr.Min.Should().Be(0);
        attr.Max.Should().Be(100);
    }

    [Fact]
    public void DiscountAttribute_Defaults_ShouldBeCorrect()
    {
        var attr = new DiscountAttribute();
        attr.Min.Should().Be(0);
        attr.Max.Should().Be(100);
    }

    [Fact]
    public void RatingAttribute_Defaults_ShouldBeCorrect()
    {
        var attr = new RatingAttribute();
        attr.Min.Should().Be(0);
        attr.Max.Should().Be(5);
        attr.Scale.Should().Be(1);
    }

    [Fact]
    public void StringShortcutAttributes_EmailAttribute_Defaults()
    {
        var attr = new EmailAttribute();
        attr.MaxLength.Should().Be(320);
    }

    [Fact]
    public void StringShortcutAttributes_UrlAttribute_Defaults()
    {
        var attr = new UrlAttribute();
        attr.AllowedSchemes.Should().BeEquivalentTo(["https", "http"]);
    }
    
    [Fact]
    public void StringShortcutAttributes_SlugAttribute_Defaults()
    {
        var attr = new SlugAttribute();
        attr.MaxLength.Should().Be(200);
    }
    
    [Fact]
    public void StringShortcutAttributes_UsernameAttribute_Defaults()
    {
        var attr = new UsernameAttribute();
        attr.MinLength.Should().Be(3);
        attr.MaxLength.Should().Be(50);
    }

    [Fact]
    public void TemporalShortcutAttributes_BusinessDateAttribute_Defaults()
    {
        var attr = new BusinessDateAttribute();
        attr.AllowWeekends.Should().BeFalse();
    }
    
    [Fact]
    public void TemporalShortcutAttributes_WeekAttribute_Defaults()
    {
        var attr = new WeekAttribute();
        attr.IsoWeekNumbering.Should().BeTrue();
    }
    
    [Fact]
    public void TemporalShortcutAttributes_BirthDateAttribute_Defaults()
    {
        var attr = new BirthDateAttribute();
        attr.MaxAge.Should().Be(150);
    }
    
    [Fact]
    public void TemporalShortcutAttributes_FiscalYearAttribute_Defaults()
    {
        var attr = new FiscalYearAttribute();
        attr.MinYear.Should().Be(1900);
    }
    
    [Fact]
    public void DatePrimitiveAttribute_Defaults()
    {
        var attr = new DatePrimitiveAttribute();
        attr.Kind.Should().Be(DatePrimitiveKind.DateOnly);
        attr.PastOnly.Should().BeFalse();
        attr.FutureOnly.Should().BeFalse();
    }
    
    [Fact]
    public void DatePrimitiveAttribute_Custom()
    {
        var attr = new DatePrimitiveAttribute { Kind = DatePrimitiveKind.DateTimeOffset, PastOnly = true, FutureOnly = true };
        attr.Kind.Should().Be(DatePrimitiveKind.DateTimeOffset);
        attr.PastOnly.Should().BeTrue();
        attr.FutureOnly.Should().BeTrue();
    }
    
    [Fact]
    public void StrongIdAttribute_Defaults()
    {
        var attr = new StrongIdAttribute<Guid>();
        attr.RejectEmpty.Should().BeTrue();
    }
    
    [Fact]
    public void StrongIdAttribute_Custom()
    {
        var attr = new StrongIdAttribute<Guid> { RejectEmpty = true };
        attr.RejectEmpty.Should().BeTrue();
    }
}
