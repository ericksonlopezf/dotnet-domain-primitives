// Copyright © Erickson Lopez. MIT License.
using System;
using System.ComponentModel.DataAnnotations;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.AspNetCore;

/// <summary>
/// A <see cref="ValidationAttribute"/> that validates a raw value as a domain primitive
/// using the primitive's built-in <c>TryCreate</c> validation logic.
/// </summary>
/// <typeparam name="TPrimitive">The domain primitive type to validate against.</typeparam>
/// <typeparam name="TValue">The backing value type of the domain primitive.</typeparam>
/// <remarks>
/// <para>
/// Apply this attribute to properties in DTOs or request/response models:
/// </para>
/// <code>
/// public class CreateProductRequest
/// {
///     [DomainPrimitiveValidation&lt;ProductName, string&gt;]
///     public string Name { get; set; }
///
///     [DomainPrimitiveValidation&lt;ProductPrice, decimal&gt;]
///     public decimal Price { get; set; }
/// }
/// </code>
/// <para>
/// This enables domain primitive validation to participate in ASP.NET Core's standard
/// model validation pipeline without requiring manual controller code.
/// </para>
/// </remarks>
public sealed class DomainPrimitiveValidationAttribute<TPrimitive, TValue> : ValidationAttribute
    where TPrimitive : struct, IDomainPrimitive<TPrimitive, TValue>
    where TValue : notnull
{
    /// <inheritdoc />
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            // Null handling is the responsibility of [Required] — not this attribute.
            return ValidationResult.Success;
        }

        if (value is not TValue typedValue)
        {
            return new ValidationResult(
                $"Expected a value of type {typeof(TValue).Name} but received {value.GetType().Name}.",
                new[] { validationContext.MemberName ?? string.Empty });
        }

        return DomainPrimitiveValidator.ValidateSingle<TPrimitive, TValue>(typedValue, validationContext.MemberName ?? string.Empty);
    }
}
