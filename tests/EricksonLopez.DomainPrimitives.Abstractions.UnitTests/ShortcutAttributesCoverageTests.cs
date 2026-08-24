// Copyright © Erickson Lopez. MIT License.
using System;
using AwesomeAssertions;
using EricksonLopez.DomainPrimitives;
using Xunit;

namespace EricksonLopez.DomainPrimitives.Abstractions.UnitTests;

/// <summary>
/// Unit tests verifying default initialization and property assignments for domain shortcut attributes.
/// Ensures that parameterless constructors and property mutations do not throw and instantiate correctly (Stryker constructor coverage).
/// </summary>
public class ShortcutAttributesCoverageTests
{
    public static readonly TheoryData<Type> ParameterlessAttributeTypes = new()
    {
        typeof(LatitudeAttribute),
        typeof(LongitudeAttribute),
        typeof(AgeAttribute),
        typeof(ExpirationDateAttribute),
        typeof(MonthAttribute),
        typeof(QuarterAttribute),
        typeof(DateRangeAttribute),
        typeof(TimeRangeAttribute),
        typeof(PhoneAttribute),
        typeof(CountryCodeAttribute),
        typeof(LanguageCodeAttribute),
        typeof(CurrencyCodeAttribute),
        typeof(PasswordHashAttribute),
        typeof(HexColorAttribute),
        typeof(IPAddressAttribute),
        typeof(MacAddressAttribute),
        typeof(IBANAttribute),
        typeof(ISBNAttribute),
        typeof(VINAttribute)
    };

    [Theory]
    [MemberData(nameof(ParameterlessAttributeTypes))]
    public void ParameterlessShortcutAttributes_InstantiateSuccessfully(Type attributeType)
    {
        var instance = Activator.CreateInstance(attributeType);
        instance.Should().NotBeNull();
    }

    [Fact]
    public void MoneyAttribute_ConstructorAndProperties_WorkAsExpected()
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
    public void PercentageAttribute_ConstructorAndProperties_WorkAsExpected()
    {
        var attr = new PercentageAttribute { Min = 10, Max = 90 };
        attr.Min.Should().Be(10);
        attr.Max.Should().Be(90);
    }

    [Fact]
    public void WeightAttribute_ConstructorAndProperties_WorkAsExpected()
    {
        var attr = new WeightAttribute { Min = 10, Max = 200 };
        attr.Min.Should().Be(10);
        attr.Max.Should().Be(200);
    }

    [Fact]
    public void HeightAttribute_ConstructorAndProperties_WorkAsExpected()
    {
        var attr = new HeightAttribute { Min = 10, Max = 250 };
        attr.Min.Should().Be(10);
        attr.Max.Should().Be(250);
    }

    [Fact]
    public void DistanceAttribute_ConstructorAndProperties_WorkAsExpected()
    {
        var attr = new DistanceAttribute { Min = 10, Max = 1000 };
        attr.Min.Should().Be(10);
        attr.Max.Should().Be(1000);
    }

    [Fact]
    public void TemperatureAttribute_ConstructorAndProperties_WorkAsExpected()
    {
        var attr = new TemperatureAttribute { Min = -100, Max = 100 };
        attr.Min.Should().Be(-100);
        attr.Max.Should().Be(100);
    }

    [Fact]
    public void ScoreAttribute_ConstructorAndProperties_WorkAsExpected()
    {
        var attr = new ScoreAttribute { Min = 10, Max = 90 };
        attr.Min.Should().Be(10);
        attr.Max.Should().Be(90);
    }

    [Fact]
    public void QuantityAttribute_ConstructorAndProperties_WorkAsExpected()
    {
        var attr = new QuantityAttribute { Min = 10, Max = 90 };
        attr.Min.Should().Be(10);
        attr.Max.Should().Be(90);
    }

    [Fact]
    public void PriceAttribute_ConstructorAndProperties_WorkAsExpected()
    {
        var attr = new PriceAttribute { Min = 10, Max = 90 };
        attr.Min.Should().Be(10);
        attr.Max.Should().Be(90);
    }

    [Fact]
    public void TaxRateAttribute_ConstructorAndProperties_WorkAsExpected()
    {
        var attr = new TaxRateAttribute { Min = 10, Max = 90 };
        attr.Min.Should().Be(10);
        attr.Max.Should().Be(90);
    }

    [Fact]
    public void DiscountAttribute_ConstructorAndProperties_WorkAsExpected()
    {
        var attr = new DiscountAttribute { Min = 10, Max = 90 };
        attr.Min.Should().Be(10);
        attr.Max.Should().Be(90);
    }

    [Fact]
    public void RatingAttribute_ConstructorAndProperties_WorkAsExpected()
    {
        var attr = new RatingAttribute { Min = 1, Max = 10, Scale = 2 };
        attr.Min.Should().Be(1);
        attr.Max.Should().Be(10);
        attr.Scale.Should().Be(2);
    }

    [Fact]
    public void BirthDateAttribute_ConstructorAndProperties_WorkAsExpected()
    {
        var attr = new BirthDateAttribute { MaxAge = 100 };
        attr.MaxAge.Should().Be(100);
    }

    [Fact]
    public void BusinessDateAttribute_ConstructorAndProperties_WorkAsExpected()
    {
        var attr = new BusinessDateAttribute { AllowWeekends = true };
        attr.AllowWeekends.Should().BeTrue();
    }

    [Fact]
    public void FiscalYearAttribute_ConstructorAndProperties_WorkAsExpected()
    {
        var attr = new FiscalYearAttribute { MinYear = 2000 };
        attr.MinYear.Should().Be(2000);
    }

    [Fact]
    public void WeekAttribute_ConstructorAndProperties_WorkAsExpected()
    {
        var attr = new WeekAttribute { IsoWeekNumbering = false };
        attr.IsoWeekNumbering.Should().BeFalse();
    }

    [Fact]
    public void EmailAttribute_ConstructorAndProperties_WorkAsExpected()
    {
        var attr = new EmailAttribute { MaxLength = 100 };
        attr.MaxLength.Should().Be(100);
    }

    [Fact]
    public void UrlAttribute_ConstructorAndProperties_WorkAsExpected()
    {
        var attr = new UrlAttribute { AllowedSchemes = new[] { "ftp" } };
        attr.AllowedSchemes.Should().Contain("ftp");
    }

    [Fact]
    public void SlugAttribute_ConstructorAndProperties_WorkAsExpected()
    {
        var attr = new SlugAttribute { MaxLength = 50 };
        attr.MaxLength.Should().Be(50);
    }

    [Fact]
    public void UsernameAttribute_ConstructorAndProperties_WorkAsExpected()
    {
        var attr = new UsernameAttribute { MinLength = 5, MaxLength = 20 };
        attr.MinLength.Should().Be(5);
        attr.MaxLength.Should().Be(20);
    }
}



