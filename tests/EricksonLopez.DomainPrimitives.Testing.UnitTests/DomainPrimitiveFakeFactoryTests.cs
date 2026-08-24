// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;
using AwesomeAssertions;
using EricksonLopez.DomainPrimitives.Testing;
using Xunit;

namespace EricksonLopez.DomainPrimitives.Testing.UnitTests;

public class DomainPrimitiveFakeFactoryTests
{
    // ─── String Primitives ───────────────────────────────────────────────────

    [Fact]
    public void ValidEmails_ShouldReturnNonEmpty_AndCreateSuccessfully()
    {
        DomainPrimitiveFakeFactory.Strings.ValidEmails.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.Strings.ValidEmail.Should().Be(DomainPrimitiveFakeFactory.Strings.ValidEmails[0]);
        foreach (var email in DomainPrimitiveFakeFactory.Strings.ValidEmails)
        {
            var created = ScenarioEmail.Create(email);
            created.Value.Should().NotBeNullOrWhiteSpace();
        }
    }

    [Fact]
    public void InvalidEmails_ShouldThrowValidationException()
    {
        DomainPrimitiveFakeFactory.Strings.InvalidEmails.Should().NotBeEmpty();
        foreach (var invalidEmail in DomainPrimitiveFakeFactory.Strings.InvalidEmails)
        {
            Action act = () => ScenarioEmail.Create(invalidEmail);
            act.Should().Throw<DomainPrimitiveValidationException>();
        }
    }

    [Fact]
    public void ValidPhones_ShouldReturnNonEmpty_AndCreateSuccessfully()
    {
        DomainPrimitiveFakeFactory.Strings.ValidPhones.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.Strings.ValidPhone.Should().Be(DomainPrimitiveFakeFactory.Strings.ValidPhones[0]);
        foreach (var phone in DomainPrimitiveFakeFactory.Strings.ValidPhones)
        {
            var created = ScenarioPhone.Create(phone);
            created.Value.Should().NotBeNullOrWhiteSpace();
        }
    }

    [Fact]
    public void InvalidPhones_ShouldThrowValidationException()
    {
        DomainPrimitiveFakeFactory.Strings.InvalidPhones.Should().NotBeEmpty();
        foreach (var invalidPhone in DomainPrimitiveFakeFactory.Strings.InvalidPhones)
        {
            Action act = () => ScenarioPhone.Create(invalidPhone);
            act.Should().Throw<DomainPrimitiveValidationException>();
        }
    }

    [Fact]
    public void ValidUrls_ShouldReturnNonEmpty()
    {
        DomainPrimitiveFakeFactory.Strings.ValidUrls.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.Strings.ValidUrl.Should().Be(DomainPrimitiveFakeFactory.Strings.ValidUrls[0]);
    }

    [Fact]
    public void InvalidUrls_ShouldReturnNonEmpty()
    {
        DomainPrimitiveFakeFactory.Strings.InvalidUrls.Should().NotBeEmpty();
    }

    [Fact]
    public void ValidSlugs_ShouldReturnNonEmpty_AndCreateSuccessfully()
    {
        DomainPrimitiveFakeFactory.Strings.ValidSlugs.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.Strings.ValidSlug.Should().Be(DomainPrimitiveFakeFactory.Strings.ValidSlugs[0]);
        foreach (var slug in DomainPrimitiveFakeFactory.Strings.ValidSlugs)
        {
            var created = ScenarioSlug.Create(slug);
            created.Value.Should().NotBeNullOrWhiteSpace();
        }
    }

    [Fact]
    public void InvalidSlugs_ShouldReturnNonEmpty()
    {
        DomainPrimitiveFakeFactory.Strings.InvalidSlugs.Should().NotBeEmpty();
    }

    [Fact]
    public void ValidCountryCodes_ShouldReturnNonEmpty_AndCreateSuccessfully()
    {
        DomainPrimitiveFakeFactory.Strings.ValidCountryCodes.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.Strings.ValidCountryCode.Should().Be(DomainPrimitiveFakeFactory.Strings.ValidCountryCodes[0]);
        foreach (var code in DomainPrimitiveFakeFactory.Strings.ValidCountryCodes)
        {
            var created = ScenarioCountryCode.Create(code);
            created.Value.Should().Be(code);
        }
    }

    // ─── Numeric Primitives ──────────────────────────────────────────────────

    [Fact]
    public void ValidAges_ShouldReturnExpectedValues_AndCreateSuccessfully()
    {
        DomainPrimitiveFakeFactory.Numerics.ValidAges.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.Numerics.ValidAge.Should().Be(18);
        foreach (var age in DomainPrimitiveFakeFactory.Numerics.ValidAges)
        {
            var created = ScenarioAge.Create(age);
            created.Value.Should().Be(age);
        }
    }

    [Fact]
    public void InvalidAges_ShouldThrowValidationException()
    {
        DomainPrimitiveFakeFactory.Numerics.InvalidAges.Should().NotBeEmpty();
        foreach (var invalidAge in DomainPrimitiveFakeFactory.Numerics.InvalidAges)
        {
            Action act = () => ScenarioAge.Create(invalidAge);
            act.Should().Throw<DomainPrimitiveValidationException>();
        }
    }

    [Fact]
    public void ValidPercentages_ShouldReturnExpectedValues_AndCreateSuccessfully()
    {
        DomainPrimitiveFakeFactory.Numerics.ValidPercentages.Should().NotBeEmpty();
        foreach (var percentage in DomainPrimitiveFakeFactory.Numerics.ValidPercentages)
        {
            var created = ScenarioPercentage.Create(percentage);
            created.Value.Should().Be(percentage);
        }
    }

    [Fact]
    public void InvalidPercentages_ShouldThrowValidationException()
    {
        DomainPrimitiveFakeFactory.Numerics.InvalidPercentages.Should().NotBeEmpty();
        foreach (var invalidPercentage in DomainPrimitiveFakeFactory.Numerics.InvalidPercentages)
        {
            Action act = () => ScenarioPercentage.Create(invalidPercentage);
            act.Should().Throw<DomainPrimitiveValidationException>();
        }
    }

    [Fact]
    public void ValidAndInvalidMoneyAmounts_ShouldReturnExpectedValues()
    {
        DomainPrimitiveFakeFactory.Numerics.ValidMoneyAmounts.Should().Equal(0m, 0.01m, 9.99m, 100m, 9999999.99m);
        DomainPrimitiveFakeFactory.Numerics.ValidMoneyAmount.Should().Be(0m);
        DomainPrimitiveFakeFactory.Numerics.InvalidMoneyAmounts.Should().Equal(-0.01m, -1m, decimal.MinValue);
    }

    [Fact]
    public void ValidAndInvalidLatitudes_ShouldReturnExpectedValues()
    {
        DomainPrimitiveFakeFactory.Numerics.ValidLatitudes.Should().Equal(-90.0, -45.5, 0.0, 45.5, 90.0);
        DomainPrimitiveFakeFactory.Numerics.InvalidLatitudes.Should().Equal(-90.1, 90.1, double.MaxValue);
    }

    [Fact]
    public void ValidAndInvalidLongitudes_ShouldReturnExpectedValues()
    {
        DomainPrimitiveFakeFactory.Numerics.ValidLongitudes.Should().Equal(-180.0, -90.0, 0.0, 90.0, 180.0);
        DomainPrimitiveFakeFactory.Numerics.InvalidLongitudes.Should().Equal(-180.1, 180.1, double.MinValue);
    }

    [Fact]
    public void ValidAndInvalidWeights_ShouldReturnExpectedValues()
    {
        DomainPrimitiveFakeFactory.Numerics.ValidWeights.Should().Equal(0.1, 70.5, 500.0, 1000.0);
        DomainPrimitiveFakeFactory.Numerics.InvalidWeights.Should().Equal(-1.0, 1000.1);
    }

    [Fact]
    public void ValidAndInvalidHeights_ShouldReturnExpectedValues()
    {
        DomainPrimitiveFakeFactory.Numerics.ValidHeights.Should().Equal(1.0, 175.5, 290.0, 300.0);
        DomainPrimitiveFakeFactory.Numerics.InvalidHeights.Should().Equal(-5.0, 300.1);
    }

    [Fact]
    public void ValidAndInvalidDistances_ShouldReturnExpectedValues()
    {
        DomainPrimitiveFakeFactory.Numerics.ValidDistances.Should().Equal(0.0, 1000.5, 40075000.0);
        DomainPrimitiveFakeFactory.Numerics.InvalidDistances.Should().Equal(-0.1, -100.0);
    }

    [Fact]
    public void ValidAndInvalidTemperatures_ShouldReturnExpectedValues()
    {
        DomainPrimitiveFakeFactory.Numerics.ValidTemperatures.Should().Equal(-273.15, 0.0, 36.6, 100.0);
        DomainPrimitiveFakeFactory.Numerics.InvalidTemperatures.Should().Equal(-273.16, -500.0);
    }

    [Fact]
    public void ValidAndInvalidScores_ShouldReturnExpectedValues()
    {
        DomainPrimitiveFakeFactory.Numerics.ValidScores.Should().Equal(0, 50, 100);
        DomainPrimitiveFakeFactory.Numerics.InvalidScores.Should().Equal(-1, 101);
    }

    [Fact]
    public void ValidAndInvalidQuantities_ShouldReturnExpectedValues()
    {
        DomainPrimitiveFakeFactory.Numerics.ValidQuantities.Should().Equal(0, 1, 100, 1000);
        DomainPrimitiveFakeFactory.Numerics.InvalidQuantities.Should().Equal(-1, -100);
    }

    [Fact]
    public void ValidAndInvalidPrices_ShouldReturnExpectedValues()
    {
        DomainPrimitiveFakeFactory.Numerics.ValidPrices.Should().Equal(0m, 19.99m, 1500m);
        DomainPrimitiveFakeFactory.Numerics.InvalidPrices.Should().Equal(-0.01m, -100m);
    }

    [Fact]
    public void ValidAndInvalidTaxRates_ShouldReturnExpectedValues()
    {
        DomainPrimitiveFakeFactory.Numerics.ValidTaxRates.Should().Equal(0m, 16m, 21m, 100m);
        DomainPrimitiveFakeFactory.Numerics.InvalidTaxRates.Should().Equal(-0.1m, 100.1m);
    }

    [Fact]
    public void ValidAndInvalidDiscounts_ShouldReturnExpectedValues()
    {
        DomainPrimitiveFakeFactory.Numerics.ValidDiscounts.Should().Equal(0m, 10m, 50m, 100m);
        DomainPrimitiveFakeFactory.Numerics.InvalidDiscounts.Should().Equal(-1m, 101m);
    }

    // ─── ID Primitives ───────────────────────────────────────────────────────

    [Fact]
    public void ValidGuids_ShouldReturnNonEmpty_AndCreateSuccessfully()
    {
        DomainPrimitiveFakeFactory.Identifiers.ValidGuids.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.Identifiers.ValidGuid.Should().NotBe(Guid.Empty);
        
        foreach (var guid in DomainPrimitiveFakeFactory.Identifiers.ValidGuids)
        {
            var id = ScenarioGuidId.Create(guid);
            id.Value.Should().Be(guid);
        }
    }

    [Fact]
    public void ValidGuidStrings_ShouldParseSuccessfully()
    {
        DomainPrimitiveFakeFactory.Identifiers.ValidGuidStrings.Should().Equal(
            "3fa85f64-5717-4562-b3fc-2c963f66afa6",
            "3FA85F64-5717-4562-B3FC-2C963F66AFA6",
            "{3fa85f64-5717-4562-b3fc-2c963f66afa6}",
            "3fa85f6457174562b3fc2c963f66afa6"
        );
        foreach (var guidStr in DomainPrimitiveFakeFactory.Identifiers.ValidGuidStrings)
        {
            var id = ScenarioGuidId.Parse(guidStr);
            id.Value.Should().NotBe(Guid.Empty);
        }
    }

    [Fact]
    public void InvalidGuidStrings_ShouldThrowFormatException()
    {
        DomainPrimitiveFakeFactory.Identifiers.InvalidGuidStrings.Should().Equal(
            "",
            "not-a-guid",
            "3fa85f64-5717-4562-b3fc",
            "3fa85f64-5717-4562-b3fc-2c963f66afa6-extra"
        );
        foreach (var invalidGuidStr in DomainPrimitiveFakeFactory.Identifiers.InvalidGuidStrings)
        {
            Action act = () => ScenarioGuidId.Parse(invalidGuidStr);
            act.Should().Throw<FormatException>();
        }
    }

    // ─── Date Primitives ─────────────────────────────────────────────────────

    [Fact]
    public void ValidBirthDate_ShouldBeDeterministically30YearsInPast()
    {
        var today = DomainPrimitiveFakeFactory.Dates.Today;
        DomainPrimitiveFakeFactory.Dates.ValidBirthDate.Should().Be(today.AddYears(-30));
    }

    [Fact]
    public void PastDate_ShouldBeDeterministically5YearsInPast()
    {
        var today = DomainPrimitiveFakeFactory.Dates.Today;
        DomainPrimitiveFakeFactory.Dates.PastDate.Should().Be(today.AddYears(-5));
    }

    [Fact]
    public void FutureDate_ShouldBeDeterministically5YearsInFuture()
    {
        var today = DomainPrimitiveFakeFactory.Dates.Today;
        DomainPrimitiveFakeFactory.Dates.FutureDate.Should().Be(today.AddYears(5));
    }

    [Fact]
    public void ValidExpirationDates_ShouldHaveExpectedCountAndBeInFuture()
    {
        DomainPrimitiveFakeFactory.Dates.ValidExpirationDates.Should().HaveCount(3);
        var baseToday = DomainPrimitiveFakeFactory.Dates.Today;
        DomainPrimitiveFakeFactory.Dates.ValidExpirationDates[0].Should().Be(baseToday.AddDays(1));
        DomainPrimitiveFakeFactory.Dates.ValidExpirationDates[1].Should().Be(baseToday.AddMonths(6));
        DomainPrimitiveFakeFactory.Dates.ValidExpirationDates[2].Should().Be(baseToday.AddYears(2));
    }

    [Fact]
    public void InvalidExpirationDates_ShouldHaveExpectedCountAndBeInPast()
    {
        DomainPrimitiveFakeFactory.Dates.InvalidExpirationDates.Should().HaveCount(2);
        var baseToday = DomainPrimitiveFakeFactory.Dates.Today;
        DomainPrimitiveFakeFactory.Dates.InvalidExpirationDates[0].Should().Be(baseToday.AddDays(-1));
        DomainPrimitiveFakeFactory.Dates.InvalidExpirationDates[1].Should().Be(baseToday.AddYears(-1));
    }

    [Fact]
    public void ValidBusinessDates_ShouldNotFallOnWeekends()
    {
        DomainPrimitiveFakeFactory.Dates.ValidBusinessDates.Should().HaveCount(2);
        foreach (var date in DomainPrimitiveFakeFactory.Dates.ValidBusinessDates)
        {
            date.DayOfWeek.Should().NotBe(DayOfWeek.Saturday).And.NotBe(DayOfWeek.Sunday);
        }
    }

    [Fact]
    public void InvalidBusinessDates_ShouldFallOnWeekends()
    {
        DomainPrimitiveFakeFactory.Dates.InvalidBusinessDates.Should().HaveCount(2);
        foreach (var date in DomainPrimitiveFakeFactory.Dates.InvalidBusinessDates)
        {
            (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday).Should().BeTrue();
        }
    }

    [Fact]
    public void FiscalYears_ShouldReturnExpectedValidAndInvalidValues()
    {
        DomainPrimitiveFakeFactory.Dates.ValidFiscalYears.Should().BeEquivalentTo([1900, 2024, 2026]);
        DomainPrimitiveFakeFactory.Dates.InvalidFiscalYears.Should().BeEquivalentTo([1899, 0, -1]);
    }

    // ─── Domain Shortcut Fake Data ───────────────────────────────────────────

    [Fact]
    public void ValidCurrencyCodes_ShouldReturnExpectedIsoCodes()
    {
        DomainPrimitiveFakeFactory.Shortcuts.ValidCurrencyCodes.Should().BeEquivalentTo(["USD", "EUR", "GBP", "JPY", "CAD"]);
    }

    [Fact]
    public void ValidIBANs_ShouldReturnExpectedValues()
    {
        DomainPrimitiveFakeFactory.Shortcuts.ValidIBANs.Should().BeEquivalentTo(["DE89370400440532013000", "GB29NWBK60161331926819"]);
    }

    [Fact]
    public void ValidISBNs_ShouldReturnExpectedValues()
    {
        DomainPrimitiveFakeFactory.Shortcuts.ValidISBNs.Should().BeEquivalentTo(["978-3-16-148410-0", "978-0-306-40615-7"]);
    }

    [Fact]
    public void ValidVINs_ShouldReturnExpectedValues()
    {
        DomainPrimitiveFakeFactory.Shortcuts.ValidVINs.Should().BeEquivalentTo(["1HGCR2F83HA000000", "1FA6P8CF0H5100000"]);
    }

    [Fact]
    public void ValidHexColors_ShouldReturnExpectedValues()
    {
        DomainPrimitiveFakeFactory.Shortcuts.ValidHexColors.Should().BeEquivalentTo(["#FF5733", "#00FF00", "#000000", "#FFFFFF"]);
    }

    [Fact]
    public void ValidRatings_ShouldReturnExpectedValues()
    {
        DomainPrimitiveFakeFactory.Shortcuts.ValidRatings.Should().BeEquivalentTo([0.0m, 2.5m, 4.0m, 4.8m, 5.0m]);
    }
}



