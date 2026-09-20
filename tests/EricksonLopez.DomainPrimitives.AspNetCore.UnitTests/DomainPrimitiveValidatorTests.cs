// Copyright © Erickson Lopez. MIT License.
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AwesomeAssertions;
using EricksonLopez.DomainPrimitives.AspNetCore;
using EricksonLopez.DomainPrimitives.UnitTests.TestTypes;
using Xunit;

namespace EricksonLopez.DomainPrimitives.AspNetCore.UnitTests;

/// <summary>
/// Tests for ARCH-03: DomainPrimitiveValidator and DomainPrimitiveValidationAttribute
/// IValidatableObject / DataAnnotations integration.
/// </summary>
public sealed class DomainPrimitiveValidatorTests
{
    // ─── Validate<TPrimitive, TValue> ────────────────────────────────────────

    [Fact]
    public void Validate_WithValidFirstName_ReturnsEmpty()
    {
        // Arrange: FirstName accepts non-empty strings
        const string valid = "Alice";

        // Act
        var results = DomainPrimitiveValidator
            .Validate<FirstName, string>(valid, nameof(valid))
            .ToList();

        // Assert
        results.Should().BeEmpty();
    }

    [Fact]
    public void Validate_WithInvalidFirstName_ReturnsSingleValidationResult()
    {
        // Arrange: FirstName rejects empty strings
        const string invalid = "";

        // Act
        var results = DomainPrimitiveValidator
            .Validate<FirstName, string>(invalid, "FullName")
            .ToList();

        // Assert
        results.Should().HaveCount(1);
        results[0].MemberNames.Should().Contain("FullName");
        FirstName.TryCreate(invalid, out _, out var expectedError);
        results[0].ErrorMessage.Should().Be(expectedError.Message);
    }

    // ─── ValidateSingle<TPrimitive, TValue> ──────────────────────────────────

    [Fact]
    public void ValidateSingle_WithValidValue_ReturnsSuccess()
    {
        // Act
        var result = DomainPrimitiveValidator
            .ValidateSingle<FirstName, string>("Bob", "Name");

        // Assert
        result.Should().Be(ValidationResult.Success);
    }

    [Fact]
    public void ValidateSingle_WithInvalidValue_ReturnsValidationResultWithMemberName()
    {
        // Act
        var result = DomainPrimitiveValidator
            .ValidateSingle<FirstName, string>("", "CustomerName");

        // Assert
        result.Should().NotBe(ValidationResult.Success);
        result!.MemberNames.Should().Contain("CustomerName");
        FirstName.TryCreate("", out _, out var expectedError);
        result.ErrorMessage.Should().Be(expectedError.Message);
    }

    // ─── DomainPrimitiveValidationAttribute ──────────────────────────────────

    [Fact]
    public void ValidationAttribute_WithValidValue_ReturnsSuccess()
    {
        // Arrange
        var attribute = new DomainPrimitiveValidationAttribute<FirstName, string>();
        var context = new ValidationContext(new object()) { MemberName = "OwnerName" };

        // Act
        var result = attribute.GetValidationResult("Charlie", context);

        // Assert
        result.Should().Be(ValidationResult.Success);
    }

    [Fact]
    public void ValidationAttribute_WithNullValue_ReturnsSuccess_NullIsHandledByRequiredAttribute()
    {
        // Arrange — null is the domain of [Required], not [DomainPrimitiveValidation]
        var attribute = new DomainPrimitiveValidationAttribute<FirstName, string>();
        var context = new ValidationContext(new object()) { MemberName = "OwnerName" };

        // Act
        var result = attribute.GetValidationResult(null, context);

        // Assert: null passthrough — [Required] handles null
        result.Should().Be(ValidationResult.Success);
    }

    [Fact]
    public void ValidationAttribute_WithWrongType_ReturnsTypeError()
    {
        // Arrange — passing int where string is expected
        var attribute = new DomainPrimitiveValidationAttribute<FirstName, string>();
        var context = new ValidationContext(new object()) { MemberName = "OwnerName" };

        // Act
        var result = attribute.GetValidationResult(42, context);

        // Assert
        result.Should().NotBe(ValidationResult.Success);
        result!.ErrorMessage.Should().Contain("Int32");
    }

    [Fact]
    public void ValidationAttribute_WithInvalidValue_ReturnsValidationError()
    {
        // Arrange — empty string is rejected by FirstName
        var attribute = new DomainPrimitiveValidationAttribute<FirstName, string>();
        var context = new ValidationContext(new object()) { MemberName = "PersonName" };

        // Act
        var result = attribute.GetValidationResult("", context);

        // Assert
        result.Should().NotBe(ValidationResult.Success);
        result!.MemberNames.Should().Contain("PersonName");
    }

    // ─── Null Message Fallback ───────────────────────────────────────────────

    [Fact]
    public void Validate_WhenErrorMessageIsNull_UsesFallbackMessage()
    {
        var results = DomainPrimitiveValidator
            .Validate<NullErrorMessagePrimitive, string>("any", "TestField")
            .ToList();

        results.Should().HaveCount(1);
        results[0].MemberNames.Should().Contain("TestField");
        results[0].ErrorMessage.Should().Be("The value is not a valid NullErrorMessagePrimitive.");
    }

    [Fact]
    public void ValidateSingle_WhenErrorMessageIsNull_UsesFallbackMessage()
    {
        var result = DomainPrimitiveValidator
            .ValidateSingle<NullErrorMessagePrimitive, string>("any", "TestField");

        result.Should().NotBeNull();
        result.Should().NotBe(ValidationResult.Success);
        result!.MemberNames.Should().Contain("TestField");
        result.ErrorMessage.Should().Be("The value is not a valid NullErrorMessagePrimitive.");
    }

    private readonly record struct NullErrorMessagePrimitive(string Value) : IDomainPrimitive<NullErrorMessagePrimitive, string>
    {
        public static string PrimitiveName => nameof(NullErrorMessagePrimitive);
        public bool IsDefault => string.IsNullOrEmpty(Value);
        public static NullErrorMessagePrimitive Create(string value) => new(value);
        public static bool TryCreate(string value, out NullErrorMessagePrimitive result, out global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError validationError)
        {
            result = default;
            validationError = new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError("CODE", null);
            return false;
        }
    }
}
