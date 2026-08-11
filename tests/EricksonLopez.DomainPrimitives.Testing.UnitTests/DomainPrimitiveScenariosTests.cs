using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.DomainPrimitives.Testing;
using FluentAssertions;
using Xunit;

namespace EricksonLopez.DomainPrimitives.Testing.UnitTests;

public class DomainPrimitiveScenariosTests
{
    [Fact]
    public void ValidEmailInputs_ShouldNotBeEmpty()
    {
        DomainPrimitiveScenarios.ValidEmailInputs.Should().NotBeEmpty();
    }

    [Fact]
    public void InvalidEmailInputs_ShouldNotBeEmpty()
    {
        DomainPrimitiveScenarios.InvalidEmailInputs.Should().NotBeEmpty();
    }

    [Fact]
    public void ValidPhoneInputs_ShouldNotBeEmpty()
    {
        DomainPrimitiveScenarios.ValidPhoneInputs.Should().NotBeEmpty();
    }

    [Fact]
    public void InvalidPhoneInputs_ShouldNotBeEmpty()
    {
        DomainPrimitiveScenarios.InvalidPhoneInputs.Should().NotBeEmpty();
    }

    [Fact]
    public void ValidSlugInputs_ShouldNotBeEmpty()
    {
        DomainPrimitiveScenarios.ValidSlugInputs.Should().NotBeEmpty();
    }

    [Fact]
    public void InvalidSlugInputs_ShouldNotBeEmpty()
    {
        DomainPrimitiveScenarios.InvalidSlugInputs.Should().NotBeEmpty();
    }

    [Fact]
    public void ValidGuidStrings_ShouldNotBeEmpty()
    {
        DomainPrimitiveScenarios.ValidGuidStrings.Should().NotBeEmpty();
    }

    [Fact]
    public void InvalidGuidStrings_ShouldNotBeEmpty()
    {
        DomainPrimitiveScenarios.InvalidGuidStrings.Should().NotBeEmpty();
    }

    [Fact]
    public void ValidAgeValues_ShouldNotBeEmpty()
    {
        DomainPrimitiveScenarios.ValidAgeValues.Should().NotBeEmpty();
    }

    [Fact]
    public void InvalidAgeValues_ShouldNotBeEmpty()
    {
        DomainPrimitiveScenarios.InvalidAgeValues.Should().NotBeEmpty();
    }

    [Fact]
    public void ValidPercentageValues_ShouldNotBeEmpty()
    {
        DomainPrimitiveScenarios.ValidPercentageValues.Should().NotBeEmpty();
    }

    [Fact]
    public void InvalidPercentageValues_ShouldNotBeEmpty()
    {
        DomainPrimitiveScenarios.InvalidPercentageValues.Should().NotBeEmpty();
    }

    [Fact]
    public void EmailNormalizationScenarios_ShouldNotBeEmpty()
    {
        DomainPrimitiveScenarios.EmailNormalizationScenarios.Should().NotBeEmpty();
    }

    [Fact]
    public void CountryCodeNormalizationScenarios_ShouldNotBeEmpty()
    {
        DomainPrimitiveScenarios.CountryCodeNormalizationScenarios.Should().NotBeEmpty();
    }
}
