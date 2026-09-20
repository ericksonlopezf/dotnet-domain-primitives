// Copyright © Erickson Lopez. MIT License.
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AwesomeAssertions;
using EricksonLopez.DomainPrimitives.AspNetCore;
using EricksonLopez.DomainPrimitives.UnitTests.TestTypes;
using Xunit;

namespace EricksonLopez.DomainPrimitives.AspNetCore.UnitTests;

public class DomainPrimitiveValidationAttributeTests
{
    private sealed class DummyContainer
    {
        public string? Email { get; set; }
    }

    [Fact]
    public void IsValid_WithNullValue_ReturnsSuccess()
    {
        var attribute = new DomainPrimitiveValidationAttribute<EmailAddress, string>();
        var context = new ValidationContext(new DummyContainer()) { MemberName = nameof(DummyContainer.Email) };

        var result = attribute.GetValidationResult(null, context);

        result.Should().Be(ValidationResult.Success);
    }

    [Fact]
    public void IsValid_WithInvalidType_ReturnsValidationError_WithMemberNameAndTypes()
    {
        var attribute = new DomainPrimitiveValidationAttribute<EmailAddress, string>();
        var context = new ValidationContext(new DummyContainer()) { MemberName = "Email" };

        var result = attribute.GetValidationResult(12345, context);

        result.Should().NotBeNull();
        result!.ErrorMessage.Should().Contain("Expected a value of type String but received Int32.");
        result.MemberNames.Should().Equal("Email");
    }

    [Fact]
    public void IsValid_WithInvalidType_WhenMemberNameIsNull_UsesEmptyString()
    {
        var attribute = new DomainPrimitiveValidationAttribute<EmailAddress, string>();
        var context = new ValidationContext(new DummyContainer()) { MemberName = null };

        var result = attribute.GetValidationResult(12345, context);

        result.Should().NotBeNull();
        result!.ErrorMessage.Should().Contain("Expected a value of type String but received Int32.");
        result.MemberNames.Should().Equal(string.Empty);
    }

    [Fact]
    public void IsValid_WithValidValue_ReturnsSuccess()
    {
        var attribute = new DomainPrimitiveValidationAttribute<EmailAddress, string>();
        var context = new ValidationContext(new DummyContainer()) { MemberName = "Email" };

        var result = attribute.GetValidationResult("valid@example.com", context);

        result.Should().Be(ValidationResult.Success);
    }

    [Fact]
    public void IsValid_WithInvalidPrimitiveValue_ReturnsValidationError_WithMemberName()
    {
        var attribute = new DomainPrimitiveValidationAttribute<EmailAddress, string>();
        var context = new ValidationContext(new DummyContainer()) { MemberName = "Email" };

        var result = attribute.GetValidationResult("invalid-email", context);

        result.Should().NotBeNull();
        result.Should().NotBe(ValidationResult.Success);
        result!.MemberNames.Should().Equal("Email");
    }

    [Fact]
    public void IsValid_WithInvalidPrimitiveValue_WhenMemberNameIsNull_UsesEmptyString()
    {
        var attribute = new DomainPrimitiveValidationAttribute<EmailAddress, string>();
        var context = new ValidationContext(new DummyContainer()) { MemberName = null };

        var result = attribute.GetValidationResult("invalid-email", context);

        result.Should().NotBeNull();
        result.Should().NotBe(ValidationResult.Success);
        result!.MemberNames.Should().Equal(string.Empty);
    }
}
