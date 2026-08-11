using System;
using System.Collections.Generic;
using EricksonLopez.DomainPrimitives.Testing;
using FluentAssertions;
using Xunit;

namespace EricksonLopez.DomainPrimitives.Testing.UnitTests;

public class DomainPrimitiveFakeFactoryTests
{
    [Fact]
    public void StringPrimitives_ShouldReturnExactValues()
    {
        DomainPrimitiveFakeFactory.ValidEmails.Should().BeEquivalentTo(["user@example.com", "user.name+tag@example.co.uk", "firstname.lastname@subdomain.example.com", "x@example.com", "valid-email@domain.org"]);
        DomainPrimitiveFakeFactory.ValidEmail.Should().Be("user@example.com");
        DomainPrimitiveFakeFactory.InvalidEmails.Should().BeEquivalentTo(["", "   ", "notanemail", "@missing-local.org", "missing-at-sign", "missing-domain@", "two@@at.com", "space in@email.com", "toolong" + new string('a', 320) + "@example.com"]);

        DomainPrimitiveFakeFactory.ValidPhones.Should().BeEquivalentTo(["+12125551234", "+442071234567", "+34911234567", "+525512345678"]);
        DomainPrimitiveFakeFactory.ValidPhone.Should().Be("+12125551234");
        DomainPrimitiveFakeFactory.InvalidPhones.Should().BeEquivalentTo(["", "5551234", "+1", "+(12) 345-6789", "+9999999999999999"]);

        DomainPrimitiveFakeFactory.ValidUrls.Should().BeEquivalentTo(["https://www.example.com", "https://example.com/path?query=1", "http://localhost:5000/api/v1", "https://sub.domain.example.org/page#anchor"]);
        DomainPrimitiveFakeFactory.ValidUrl.Should().Be("https://www.example.com");
        DomainPrimitiveFakeFactory.InvalidUrls.Should().BeEquivalentTo(["", "not-a-url", "ftp://blocked-scheme.com", "javascript:alert('xss')", "/relative/path"]);

        DomainPrimitiveFakeFactory.ValidSlugs.Should().BeEquivalentTo(["my-article-title", "product-123", "a", "hello-world-2024"]);
        DomainPrimitiveFakeFactory.ValidSlug.Should().Be("my-article-title");
        DomainPrimitiveFakeFactory.InvalidSlugs.Should().BeEquivalentTo(["", "   ", "Has Spaces", "HAS_UPPERCASE", "special!chars@here", new string('a', 201)]);

        DomainPrimitiveFakeFactory.ValidCountryCodes.Should().BeEquivalentTo(["US", "GB", "DE", "ES", "FR", "JP", "CN", "BR"]);
        DomainPrimitiveFakeFactory.ValidCountryCode.Should().Be("US");
    }

    [Fact]
    public void NumericPrimitives_ShouldReturnExactValues()
    {
        DomainPrimitiveFakeFactory.ValidMoneyAmounts.Should().BeEquivalentTo([0m, 0.01m, 9.99m, 100m, 9999999.99m]);
        DomainPrimitiveFakeFactory.ValidMoneyAmount.Should().Be(0m);
        DomainPrimitiveFakeFactory.InvalidMoneyAmounts.Should().BeEquivalentTo([-0.01m, -1m, decimal.MinValue]);

        DomainPrimitiveFakeFactory.ValidAges.Should().BeEquivalentTo([0, 1, 18, 65, 100, 150]);
        DomainPrimitiveFakeFactory.ValidAge.Should().Be(18);
        DomainPrimitiveFakeFactory.InvalidAges.Should().BeEquivalentTo([-1, 151, int.MaxValue]);

        DomainPrimitiveFakeFactory.ValidLatitudes.Should().BeEquivalentTo([-90.0, -45.5, 0.0, 45.5, 90.0]);
        DomainPrimitiveFakeFactory.InvalidLatitudes.Should().BeEquivalentTo([-90.1, 90.1, double.MaxValue]);

        DomainPrimitiveFakeFactory.ValidLongitudes.Should().BeEquivalentTo([-180.0, -90.0, 0.0, 90.0, 180.0]);
        DomainPrimitiveFakeFactory.InvalidLongitudes.Should().BeEquivalentTo([-180.1, 180.1, double.MinValue]);

        DomainPrimitiveFakeFactory.ValidPercentages.Should().BeEquivalentTo([0m, 25.5m, 50m, 100m]);
        DomainPrimitiveFakeFactory.InvalidPercentages.Should().BeEquivalentTo([-0.01m, 100.01m, 150m]);

        DomainPrimitiveFakeFactory.ValidWeights.Should().BeEquivalentTo([0.1, 70.5, 500.0, 1000.0]);
        DomainPrimitiveFakeFactory.InvalidWeights.Should().BeEquivalentTo([-1.0, 1000.1]);

        DomainPrimitiveFakeFactory.ValidHeights.Should().BeEquivalentTo([1.0, 175.5, 290.0, 300.0]);
        DomainPrimitiveFakeFactory.InvalidHeights.Should().BeEquivalentTo([-5.0, 300.1]);

        DomainPrimitiveFakeFactory.ValidDistances.Should().BeEquivalentTo([0.0, 1000.5, 40075000.0]);
        DomainPrimitiveFakeFactory.InvalidDistances.Should().BeEquivalentTo([-0.1, -100.0]);

        DomainPrimitiveFakeFactory.ValidTemperatures.Should().BeEquivalentTo([-273.15, 0.0, 36.6, 100.0]);
        DomainPrimitiveFakeFactory.InvalidTemperatures.Should().BeEquivalentTo([-273.16, -500.0]);

        DomainPrimitiveFakeFactory.ValidScores.Should().BeEquivalentTo([0, 50, 100]);
        DomainPrimitiveFakeFactory.InvalidScores.Should().BeEquivalentTo([-1, 101]);

        DomainPrimitiveFakeFactory.ValidQuantities.Should().BeEquivalentTo([0, 1, 100, 1000]);
        DomainPrimitiveFakeFactory.InvalidQuantities.Should().BeEquivalentTo([-1, -100]);

        DomainPrimitiveFakeFactory.ValidPrices.Should().BeEquivalentTo([0m, 19.99m, 1500m]);
        DomainPrimitiveFakeFactory.InvalidPrices.Should().BeEquivalentTo([-0.01m, -100m]);

        DomainPrimitiveFakeFactory.ValidTaxRates.Should().BeEquivalentTo([0m, 16m, 21m, 100m]);
        DomainPrimitiveFakeFactory.InvalidTaxRates.Should().BeEquivalentTo([-0.1m, 100.1m]);

        DomainPrimitiveFakeFactory.ValidDiscounts.Should().BeEquivalentTo([0m, 10m, 50m, 100m]);
        DomainPrimitiveFakeFactory.InvalidDiscounts.Should().BeEquivalentTo([-1m, 101m]);
    }

    [Fact]
    public void IdPrimitives_ShouldReturnExactValues()
    {
        DomainPrimitiveFakeFactory.ValidGuids.Should().BeEquivalentTo([
            new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
            new Guid("00000000-0000-0000-0000-000000000001"),
            new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff")
        ]);
        DomainPrimitiveFakeFactory.ValidGuid.Should().Be(new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"));
        
        DomainPrimitiveFakeFactory.ValidGuidStrings.Should().BeEquivalentTo([
            "3fa85f64-5717-4562-b3fc-2c963f66afa6",
            "3FA85F64-5717-4562-B3FC-2C963F66AFA6",
            "{3fa85f64-5717-4562-b3fc-2c963f66afa6}",
            "3fa85f6457174562b3fc2c963f66afa6"
        ]);
        DomainPrimitiveFakeFactory.InvalidGuidStrings.Should().BeEquivalentTo([
            "",
            "not-a-guid",
            "3fa85f64-5717-4562-b3fc",
            "3fa85f64-5717-4562-b3fc-2c963f66afa6-extra"
        ]);
    }

    [Fact]
    public void DatePrimitives_ShouldReturnExpectedValues()
    {
        var today = DomainPrimitiveFakeFactory.Today;
        
        DomainPrimitiveFakeFactory.ValidBirthDate.Should().BeOneOf(today.AddYears(-30), today.AddDays(1).AddYears(-30), today.AddDays(-1).AddYears(-30));
        DomainPrimitiveFakeFactory.PastDate.Should().BeOneOf(today.AddYears(-5), today.AddDays(1).AddYears(-5), today.AddDays(-1).AddYears(-5));
        DomainPrimitiveFakeFactory.FutureDate.Should().BeOneOf(today.AddYears(5), today.AddDays(1).AddYears(5), today.AddDays(-1).AddYears(5));
        
        DomainPrimitiveFakeFactory.ValidExpirationDates.Should().HaveCount(3);
        var initToday = DomainPrimitiveFakeFactory.ValidExpirationDates[0].AddDays(-1);
        DomainPrimitiveFakeFactory.ValidExpirationDates[1].Should().Be(initToday.AddMonths(6));
        DomainPrimitiveFakeFactory.ValidExpirationDates[2].Should().Be(initToday.AddYears(2));

        DomainPrimitiveFakeFactory.InvalidExpirationDates.Should().HaveCount(2);
        var initTodayInvalid = DomainPrimitiveFakeFactory.InvalidExpirationDates[0].AddDays(1);
        DomainPrimitiveFakeFactory.InvalidExpirationDates[1].Should().Be(initTodayInvalid.AddYears(-1));

        DomainPrimitiveFakeFactory.ValidBusinessDates.Should().HaveCount(2);
        foreach (var date in DomainPrimitiveFakeFactory.ValidBusinessDates)
        {
            date.DayOfWeek.Should().NotBe(DayOfWeek.Saturday).And.NotBe(DayOfWeek.Sunday);
        }
        var diffDays = DomainPrimitiveFakeFactory.ValidBusinessDates[1].DayNumber - DomainPrimitiveFakeFactory.ValidBusinessDates[0].DayNumber;
        diffDays.Should().BeOneOf(1, 3);

        DomainPrimitiveFakeFactory.InvalidBusinessDates.Should().HaveCount(2);
        foreach (var date in DomainPrimitiveFakeFactory.InvalidBusinessDates)
        {
            (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday).Should().BeTrue();
        }
        var diffDaysInvalid = DomainPrimitiveFakeFactory.InvalidBusinessDates[1].DayNumber - DomainPrimitiveFakeFactory.InvalidBusinessDates[0].DayNumber;
        diffDaysInvalid.Should().BeOneOf(1, 6);

        DomainPrimitiveFakeFactory.ValidFiscalYears.Should().BeEquivalentTo([1900, 2024, 2026]);
        DomainPrimitiveFakeFactory.InvalidFiscalYears.Should().BeEquivalentTo([1899, 0, -1]);
    }

    [Fact]
    public void DomainShortcutFakeData_ShouldReturnExactValues()
    {
        DomainPrimitiveFakeFactory.ValidCurrencyCodes.Should().BeEquivalentTo(["USD", "EUR", "GBP", "JPY", "CAD"]);
        DomainPrimitiveFakeFactory.ValidIBANs.Should().BeEquivalentTo(["DE89370400440532013000", "GB29NWBK60161331926819"]);
        DomainPrimitiveFakeFactory.ValidISBNs.Should().BeEquivalentTo(["978-3-16-148410-0", "978-0-306-40615-7"]);
        DomainPrimitiveFakeFactory.ValidVINs.Should().BeEquivalentTo(["1HGCR2F83HA000000", "1FA6P8CF0H5100000"]);
        DomainPrimitiveFakeFactory.ValidHexColors.Should().BeEquivalentTo(["#FF5733", "#00FF00", "#000000", "#FFFFFF"]);
        DomainPrimitiveFakeFactory.ValidRatings.Should().BeEquivalentTo([0.0m, 2.5m, 4.0m, 4.8m, 5.0m]);
    }
}
