using System;
using Xunit;
using FluentAssertions;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.Abstractions.UnitTests;

public class ShortcutAttributesCoverageTests
{
    [Fact]
    public void MoneyAttribute_CanBeInstantiated_AndPropertiesSet()
    {
        var attr1 = new MoneyAttribute();
        attr1.Currency.Should().Be("USD");
        attr1.Min.Should().Be(0);
        attr1.Max.Should().Be(double.MaxValue);

        var attr2 = new MoneyAttribute("EUR");
        attr2.Currency.Should().Be("EUR");

        var attr3 = new MoneyAttribute("GBP", 10, 100);
        attr3.Currency.Should().Be("GBP");
        attr3.Min.Should().Be(10);
        attr3.Max.Should().Be(100);
    }

    [Fact]
    public void PercentageAttribute_CanBeInstantiated_AndPropertiesSet()
    {
        var attr = new PercentageAttribute { Min = 10, Max = 90 };
        attr.Min.Should().Be(10);
        attr.Max.Should().Be(90);
    }

    [Fact]
    public void LatitudeAttribute_CanBeInstantiated()
    {
        var attr = new LatitudeAttribute();
        attr.Should().NotBeNull();
    }

    [Fact]
    public void LongitudeAttribute_CanBeInstantiated()
    {
        var attr = new LongitudeAttribute();
        attr.Should().NotBeNull();
    }

    [Fact]
    public void AgeAttribute_CanBeInstantiated()
    {
        var attr = new AgeAttribute();
        attr.Should().NotBeNull();
    }

    [Fact]
    public void WeightAttribute_CanBeInstantiated_AndPropertiesSet()
    {
        var attr = new WeightAttribute { Min = 10, Max = 200 };
        attr.Min.Should().Be(10);
        attr.Max.Should().Be(200);
    }

    [Fact]
    public void HeightAttribute_CanBeInstantiated_AndPropertiesSet()
    {
        var attr = new HeightAttribute { Min = 10, Max = 250 };
        attr.Min.Should().Be(10);
        attr.Max.Should().Be(250);
    }

    [Fact]
    public void DistanceAttribute_CanBeInstantiated_AndPropertiesSet()
    {
        var attr = new DistanceAttribute { Min = 10, Max = 1000 };
        attr.Min.Should().Be(10);
        attr.Max.Should().Be(1000);
    }

    [Fact]
    public void TemperatureAttribute_CanBeInstantiated_AndPropertiesSet()
    {
        var attr = new TemperatureAttribute { Min = -100, Max = 100 };
        attr.Min.Should().Be(-100);
        attr.Max.Should().Be(100);
    }

    [Fact]
    public void ScoreAttribute_CanBeInstantiated_AndPropertiesSet()
    {
        var attr = new ScoreAttribute { Min = 10, Max = 90 };
        attr.Min.Should().Be(10);
        attr.Max.Should().Be(90);
    }

    [Fact]
    public void QuantityAttribute_CanBeInstantiated_AndPropertiesSet()
    {
        var attr = new QuantityAttribute { Min = 10, Max = 90 };
        attr.Min.Should().Be(10);
        attr.Max.Should().Be(90);
    }

    [Fact]
    public void PriceAttribute_CanBeInstantiated_AndPropertiesSet()
    {
        var attr = new PriceAttribute { Min = 10, Max = 90 };
        attr.Min.Should().Be(10);
        attr.Max.Should().Be(90);
    }

    [Fact]
    public void TaxRateAttribute_CanBeInstantiated_AndPropertiesSet()
    {
        var attr = new TaxRateAttribute { Min = 10, Max = 90 };
        attr.Min.Should().Be(10);
        attr.Max.Should().Be(90);
    }

    [Fact]
    public void DiscountAttribute_CanBeInstantiated_AndPropertiesSet()
    {
        var attr = new DiscountAttribute { Min = 10, Max = 90 };
        attr.Min.Should().Be(10);
        attr.Max.Should().Be(90);
    }

    [Fact]
    public void RatingAttribute_CanBeInstantiated_AndPropertiesSet()
    {
        var attr = new RatingAttribute { Min = 1, Max = 10, Scale = 2 };
        attr.Min.Should().Be(1);
        attr.Max.Should().Be(10);
        attr.Scale.Should().Be(2);
    }

    [Fact]
    public void BirthDateAttribute_CanBeInstantiated_AndPropertiesSet()
    {
        var attr = new BirthDateAttribute { MaxAge = 100 };
        attr.MaxAge.Should().Be(100);
    }

    [Fact]
    public void ExpirationDateAttribute_CanBeInstantiated() { var attr = new ExpirationDateAttribute(); attr.Should().NotBeNull(); }

    [Fact]
    public void BusinessDateAttribute_CanBeInstantiated_AndPropertiesSet()
    {
        var attr = new BusinessDateAttribute { AllowWeekends = true };
        attr.AllowWeekends.Should().BeTrue();
    }

    [Fact]
    public void FiscalYearAttribute_CanBeInstantiated_AndPropertiesSet()
    {
        var attr = new FiscalYearAttribute { MinYear = 2000 };
        attr.MinYear.Should().Be(2000);
    }

    [Fact]
    public void MonthAttribute_CanBeInstantiated() { var attr = new MonthAttribute(); attr.Should().NotBeNull(); }

    [Fact]
    public void QuarterAttribute_CanBeInstantiated() { var attr = new QuarterAttribute(); attr.Should().NotBeNull(); }

    [Fact]
    public void WeekAttribute_CanBeInstantiated_AndPropertiesSet()
    {
        var attr = new WeekAttribute { IsoWeekNumbering = false };
        attr.IsoWeekNumbering.Should().BeFalse();
    }

    [Fact]
    public void DateRangeAttribute_CanBeInstantiated() { var attr = new DateRangeAttribute(); attr.Should().NotBeNull(); }

    [Fact]
    public void TimeRangeAttribute_CanBeInstantiated() { var attr = new TimeRangeAttribute(); attr.Should().NotBeNull(); }

    [Fact]
    public void EmailAttribute_CanBeInstantiated_AndPropertiesSet()
    {
        var attr = new EmailAttribute { MaxLength = 100 };
        attr.MaxLength.Should().Be(100);
    }

    [Fact]
    public void PhoneAttribute_CanBeInstantiated() { var attr = new PhoneAttribute(); attr.Should().NotBeNull(); }

    [Fact]
    public void UrlAttribute_CanBeInstantiated_AndPropertiesSet()
    {
        var attr = new UrlAttribute { AllowedSchemes = new[] { "ftp" } };
        attr.AllowedSchemes.Should().Contain("ftp");
    }

    [Fact]
    public void SlugAttribute_CanBeInstantiated_AndPropertiesSet()
    {
        var attr = new SlugAttribute { MaxLength = 50 };
        attr.MaxLength.Should().Be(50);
    }

    [Fact]
    public void CountryCodeAttribute_CanBeInstantiated() { var attr = new CountryCodeAttribute(); attr.Should().NotBeNull(); }

    [Fact]
    public void LanguageCodeAttribute_CanBeInstantiated() { var attr = new LanguageCodeAttribute(); attr.Should().NotBeNull(); }

    [Fact]
    public void CurrencyCodeAttribute_CanBeInstantiated() { var attr = new CurrencyCodeAttribute(); attr.Should().NotBeNull(); }

    [Fact]
    public void UsernameAttribute_CanBeInstantiated_AndPropertiesSet()
    {
        var attr = new UsernameAttribute { MinLength = 5, MaxLength = 20 };
        attr.MinLength.Should().Be(5);
        attr.MaxLength.Should().Be(20);
    }

    [Fact]
    public void PasswordHashAttribute_CanBeInstantiated() { var attr = new PasswordHashAttribute(); attr.Should().NotBeNull(); }

    [Fact]
    public void HexColorAttribute_CanBeInstantiated() { var attr = new HexColorAttribute(); attr.Should().NotBeNull(); }

    [Fact]
    public void IPAddressAttribute_CanBeInstantiated() { var attr = new IPAddressAttribute(); attr.Should().NotBeNull(); }

    [Fact]
    public void MacAddressAttribute_CanBeInstantiated() { var attr = new MacAddressAttribute(); attr.Should().NotBeNull(); }

    [Fact]
    public void IBANAttribute_CanBeInstantiated() { var attr = new IBANAttribute(); attr.Should().NotBeNull(); }

    [Fact]
    public void ISBNAttribute_CanBeInstantiated() { var attr = new ISBNAttribute(); attr.Should().NotBeNull(); }

    [Fact]
    public void VINAttribute_CanBeInstantiated() { var attr = new VINAttribute(); attr.Should().NotBeNull(); }
}

