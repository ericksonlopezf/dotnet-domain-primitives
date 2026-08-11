using System;
using EricksonLopez.DomainPrimitives.Testing;
using FluentAssertions;
using Xunit;

namespace EricksonLopez.DomainPrimitives.UnitTests;

public class FakeFactoryTests
{
    [Fact]
    public void FakeFactory_StringFakes_ShouldNotBeEmpty()
    {
        DomainPrimitiveFakeFactory.ValidEmails.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.InvalidEmails.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.ValidPhones.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.InvalidPhones.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.ValidUrls.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.InvalidUrls.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.ValidSlugs.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.InvalidSlugs.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.ValidCountryCodes.Should().NotBeEmpty();
    }

    [Fact]
    public void FakeFactory_NumericFakes_ShouldNotBeEmpty()
    {
        DomainPrimitiveFakeFactory.ValidMoneyAmounts.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.InvalidMoneyAmounts.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.ValidAges.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.InvalidAges.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.ValidLatitudes.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.InvalidLatitudes.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.ValidLongitudes.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.InvalidLongitudes.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.ValidPercentages.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.InvalidPercentages.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.ValidWeights.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.InvalidWeights.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.ValidHeights.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.InvalidHeights.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.ValidDistances.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.InvalidDistances.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.ValidTemperatures.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.InvalidTemperatures.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.ValidScores.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.InvalidScores.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.ValidQuantities.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.InvalidQuantities.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.ValidPrices.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.InvalidPrices.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.ValidTaxRates.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.InvalidTaxRates.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.ValidDiscounts.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.InvalidDiscounts.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.ValidRatings.Should().NotBeEmpty();
    }

    [Fact]
    public void FakeFactory_DateFakes_ShouldNotBeEmpty()
    {
        DomainPrimitiveFakeFactory.ValidBirthDate.Should().BeBefore(DomainPrimitiveFakeFactory.Today);
        DomainPrimitiveFakeFactory.PastDate.Should().BeBefore(DomainPrimitiveFakeFactory.Today);
        DomainPrimitiveFakeFactory.FutureDate.Should().BeAfter(DomainPrimitiveFakeFactory.Today);
        DomainPrimitiveFakeFactory.ValidExpirationDates.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.InvalidExpirationDates.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.ValidBusinessDates.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.InvalidBusinessDates.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.ValidFiscalYears.Should().NotBeEmpty();
        DomainPrimitiveFakeFactory.InvalidFiscalYears.Should().NotBeEmpty();
    }
}
