// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;
using AwesomeAssertions;
using EricksonLopez.DomainPrimitives;
using EricksonLopez.DomainPrimitives.Testing;
using Xunit;

namespace EricksonLopez.DomainPrimitives.Testing.UnitTests;

[Email]
public readonly partial record struct ScenarioEmail;

[Phone]
public readonly partial record struct ScenarioPhone;

[Slug]
public readonly partial record struct ScenarioSlug;

[StringPrimitive]
[Trim]
[Regex(@"^[a-z0-9]+(?:-[a-z0-9]+)*$")]
public readonly partial record struct ScenarioStrictSlug;

[StrongId<Guid>]
public readonly partial record struct ScenarioGuidId;

[Age]
public readonly partial record struct ScenarioAge;

[Percentage]
public readonly partial record struct ScenarioPercentage;

[CountryCode]
public readonly partial record struct ScenarioCountryCode;

public class DomainPrimitiveScenariosTests
{
    [Theory]
    [MemberData(nameof(DomainPrimitiveScenarios.ValidEmailInputs), MemberType = typeof(DomainPrimitiveScenarios))]
    public void ValidEmailInputs_ShouldSuccessfullyCreateEmail(string email)
    {
        var created = ScenarioEmail.Create(email);
        created.Value.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [MemberData(nameof(DomainPrimitiveScenarios.InvalidEmailInputs), MemberType = typeof(DomainPrimitiveScenarios))]
    public void InvalidEmailInputs_ShouldFailValidation(string email)
    {
        Action act = () => ScenarioEmail.Create(email);
        act.Should().Throw<DomainPrimitiveValidationException>();
    }

    [Theory]
    [MemberData(nameof(DomainPrimitiveScenarios.ValidPhoneInputs), MemberType = typeof(DomainPrimitiveScenarios))]
    public void ValidPhoneInputs_ShouldSuccessfullyCreatePhone(string phone)
    {
        var created = ScenarioPhone.Create(phone);
        created.Value.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [MemberData(nameof(DomainPrimitiveScenarios.InvalidPhoneInputs), MemberType = typeof(DomainPrimitiveScenarios))]
    public void InvalidPhoneInputs_ShouldFailValidation(string phone)
    {
        Action act = () => ScenarioPhone.Create(phone);
        act.Should().Throw<DomainPrimitiveValidationException>();
    }

    [Theory]
    [MemberData(nameof(DomainPrimitiveScenarios.ValidSlugInputs), MemberType = typeof(DomainPrimitiveScenarios))]
    public void ValidSlugInputs_ShouldSuccessfullyCreateSlug(string slug)
    {
        var created = ScenarioSlug.Create(slug);
        created.Value.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [MemberData(nameof(DomainPrimitiveScenarios.InvalidSlugInputs), MemberType = typeof(DomainPrimitiveScenarios))]
    public void InvalidSlugInputs_ShouldFailValidation(string slug)
    {
        Action act = () => ScenarioStrictSlug.Create(slug);
        act.Should().Throw<DomainPrimitiveValidationException>();
    }

    [Theory]
    [MemberData(nameof(DomainPrimitiveScenarios.ValidGuidStrings), MemberType = typeof(DomainPrimitiveScenarios))]
    public void ValidGuidStrings_ShouldSuccessfullyParseGuidId(string guidStr)
    {
        var created = ScenarioGuidId.Parse(guidStr);
        created.Value.Should().NotBeEmpty();
    }

    [Theory]
    [MemberData(nameof(DomainPrimitiveScenarios.InvalidGuidStrings), MemberType = typeof(DomainPrimitiveScenarios))]
    public void InvalidGuidStrings_ShouldFailValidation(string guidStr)
    {
        Action act = () => ScenarioGuidId.Parse(guidStr);
        act.Should().Throw<FormatException>();
    }

    [Theory]
    [MemberData(nameof(DomainPrimitiveScenarios.ValidAgeValues), MemberType = typeof(DomainPrimitiveScenarios))]
    public void ValidAgeValues_ShouldSuccessfullyCreateAge(int age)
    {
        var created = ScenarioAge.Create(age);
        created.Value.Should().Be(age);
    }

    [Theory]
    [MemberData(nameof(DomainPrimitiveScenarios.InvalidAgeValues), MemberType = typeof(DomainPrimitiveScenarios))]
    public void InvalidAgeValues_ShouldFailValidation(int age)
    {
        Action act = () => ScenarioAge.Create(age);
        act.Should().Throw<DomainPrimitiveValidationException>();
    }

    [Theory]
    [MemberData(nameof(DomainPrimitiveScenarios.ValidPercentageValues), MemberType = typeof(DomainPrimitiveScenarios))]
    public void ValidPercentageValues_ShouldSuccessfullyCreatePercentage(decimal percentage)
    {
        var created = ScenarioPercentage.Create(percentage);
        created.Value.Should().Be(percentage);
    }

    [Theory]
    [MemberData(nameof(DomainPrimitiveScenarios.InvalidPercentageValues), MemberType = typeof(DomainPrimitiveScenarios))]
    public void InvalidPercentageValues_ShouldFailValidation(decimal percentage)
    {
        Action act = () => ScenarioPercentage.Create(percentage);
        act.Should().Throw<DomainPrimitiveValidationException>();
    }

    [Theory]
    [MemberData(nameof(DomainPrimitiveScenarios.EmailNormalizationScenarios), MemberType = typeof(DomainPrimitiveScenarios))]
    public void EmailNormalizationScenarios_ShouldNormalizeEmailAsExpected(string rawInput, string expectedNormalized)
    {
        var created = ScenarioEmail.Create(rawInput);
        created.Value.Should().Be(expectedNormalized);
    }

    [Theory]
    [MemberData(nameof(DomainPrimitiveScenarios.CountryCodeNormalizationScenarios), MemberType = typeof(DomainPrimitiveScenarios))]
    public void CountryCodeNormalizationScenarios_ShouldNormalizeCountryCodeAsExpected(string rawInput, string expectedNormalized)
    {
        var created = ScenarioCountryCode.Create(rawInput);
        created.Value.Should().Be(expectedNormalized);
    }
}


