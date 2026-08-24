// Copyright © Erickson Lopez. MIT License.
using System;
using AwesomeAssertions;
using EricksonLopez.DomainPrimitives;
using EricksonLopez.DomainPrimitives.Testing;
using Xunit;

namespace EricksonLopez.DomainPrimitives.IntegrationTests;

[Email]
public readonly partial record struct TestEmail;

[StrongId<Guid>]
public readonly partial record struct TestCustomerId;

[Money]
public readonly partial record struct TestMoney;

[Slug]
public readonly partial record struct TestSlug;

[CountryCode]
public readonly partial record struct TestCountryCode;

[Percentage]
public readonly partial record struct TestPercentage;

[BirthDate]
public readonly partial record struct TestBirthDate;

[SmartEnum<int>]
public readonly partial record struct TestOrderStatus
{
    public static readonly TestOrderStatus Pending = new(1, "Pending");
    public static readonly TestOrderStatus Shipped = new(2, "Shipped");
    public static readonly TestOrderStatus Delivered = new(3, "Delivered");
}

public class DomainPrimitiveTestingSuite
{
    [Fact]
    public void Create_WithValidEmailFromFakeFactory_SetsExpectedValue()
    {
        // Arrange
        string validEmail = DomainPrimitiveFakeFactory.Strings.ValidEmail;
        
        // Act
        var email = TestEmail.Create(validEmail);
        
        // Assert
        email.Value.Should().Be(validEmail);
    }

    [Fact]
    public void Create_WithValidIdFromFakeFactory_SetsExpectedValue()
    {
        // Arrange
        Guid validGuid = DomainPrimitiveFakeFactory.Identifiers.ValidGuid;
        
        // Act
        var id = TestCustomerId.Create(validGuid);
        
        // Assert
        id.Value.Should().Be(validGuid);
    }

    [Fact]
    public void Create_WithValidMoneyFromFakeFactory_SetsExpectedValue()
    {
        // Arrange
        decimal validMoney = DomainPrimitiveFakeFactory.Numerics.ValidMoneyAmount;
        
        // Act
        var money = TestMoney.Create(validMoney);
        
        // Assert
        money.Value.Should().Be(validMoney);
    }

    [Fact]
    public void Create_WithAllValidEmailsFromFakeFactory_SucceedsForAll()
    {
        foreach (var emailStr in DomainPrimitiveFakeFactory.Strings.ValidEmails)
        {
            var email = TestEmail.Create(emailStr);
            email.Value.Should().NotBeNullOrWhiteSpace();
            email.IsDefault.Should().BeFalse();
        }
    }

    [Fact]
    public void TryCreate_WithAllInvalidEmailsFromFakeFactory_ReturnsFailureResult()
    {
        foreach (var invalidEmail in DomainPrimitiveFakeFactory.Strings.InvalidEmails)
        {
            var success = TestEmail.TryCreate(invalidEmail, out var result, out var error);
            success.Should().BeFalse();
            result.IsDefault.Should().BeTrue();
            error.IsError.Should().BeTrue();
        }
    }

    [Fact]
    public void Create_WithValidSlugsAndCountryCodes_SetsExpectedValues()
    {
        foreach (var slugStr in DomainPrimitiveFakeFactory.Strings.ValidSlugs)
        {
            var slug = TestSlug.Create(slugStr);
            slug.Value.Should().Be(slugStr);
        }

        foreach (var countryStr in DomainPrimitiveFakeFactory.Strings.ValidCountryCodes)
        {
            var country = TestCountryCode.Create(countryStr);
            country.Value.Should().Be(countryStr);
        }
    }

    [Fact]
    public void Create_WithPercentagesAndMoney_ValidatesCorrectly()
    {
        foreach (var pctVal in DomainPrimitiveFakeFactory.Numerics.ValidPercentages)
        {
            var pct = TestPercentage.Create(pctVal);
            pct.Value.Should().Be(pctVal);
        }

        foreach (var invalidPct in DomainPrimitiveFakeFactory.Numerics.InvalidPercentages)
        {
            Action act = () => TestPercentage.Create(invalidPct);
            act.Should().Throw<DomainPrimitiveValidationException>();
        }
    }

    [Fact]
    public void Create_WithBirthDate_ValidatesDeterministicDates()
    {
        var birthDate = TestBirthDate.Create(DomainPrimitiveFakeFactory.Dates.ValidBirthDate);
        birthDate.Value.Should().Be(DomainPrimitiveFakeFactory.Dates.ValidBirthDate);

        // Future date is invalid for BirthDate
        Action act = () => TestBirthDate.Create(DomainPrimitiveFakeFactory.Dates.FutureDate);
        act.Should().Throw<DomainPrimitiveValidationException>();
    }

    [Fact]
    public void MatchAndTryFromValue_WithSmartEnum_ExecutesCorrectPatternMatching()
    {
        var status = TestOrderStatus.Shipped;

        var matched = status.Match(
            whenPending: () => "order is pending",
            whenShipped: () => "order is shipped",
            whenDelivered: () => "order is delivered"
        );

        matched.Should().Be("order is shipped");

        TestOrderStatus.TryFromValue(2, out var foundStatus).Should().BeTrue();
        foundStatus.Should().Be(TestOrderStatus.Shipped);
    }
}





