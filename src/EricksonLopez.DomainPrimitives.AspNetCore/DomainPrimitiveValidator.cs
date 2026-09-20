// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EricksonLopez.DomainPrimitives;
using EricksonLopez.DomainPrimitives.Validation;

namespace EricksonLopez.DomainPrimitives.AspNetCore;

/// <summary>
/// Provides DataAnnotations / <see cref="IValidatableObject"/> integration for domain primitives
/// so they participate in ASP.NET Core's model validation pipeline via <c>ModelValidator</c>.
/// </summary>
/// <remarks>
/// <para>
/// Usage — wrap your primitive in a DTO that implements <see cref="IValidatableObject"/>:
/// </para>
/// <code>
/// public record CreateOrderRequest(string CustomerId, decimal Amount) : IValidatableObject
/// {
///     public IEnumerable&lt;ValidationResult&gt; Validate(ValidationContext validationContext)
///         => DomainPrimitiveValidator.Validate&lt;CustomerId, string&gt;(CustomerId, nameof(CustomerId))
///             .Concat(DomainPrimitiveValidator.Validate&lt;OrderAmount, decimal&gt;(Amount, nameof(Amount)));
/// }
/// </code>
/// <para>
/// Alternatively, validate a domain primitive directly in a custom <see cref="ValidationAttribute"/>:
/// </para>
/// <code>
/// [DomainPrimitiveValidation&lt;CustomerId, string&gt;]
/// public string RawCustomerId { get; set; }
/// </code>
/// </remarks>
public static class DomainPrimitiveValidator
{
    /// <summary>
    /// Validates a raw value as a domain primitive and returns <see cref="ValidationResult"/>s
    /// compatible with the DataAnnotations model validation pipeline.
    /// </summary>
    /// <typeparam name="TPrimitive">The target domain primitive type.</typeparam>
    /// <typeparam name="TValue">The backing value type.</typeparam>
    /// <param name="rawValue">The raw value to validate.</param>
    /// <param name="memberName">The member name to associate with any validation errors (used for <see cref="ValidationResult.MemberNames"/>).</param>
    /// <returns>
    /// An empty sequence if validation succeeds;
    /// a sequence containing one <see cref="ValidationResult"/> per validation error if it fails.
    /// </returns>
    public static IEnumerable<ValidationResult> Validate<TPrimitive, TValue>(TValue rawValue, string memberName)
        where TPrimitive : struct, IDomainPrimitive<TPrimitive, TValue>
        where TValue : notnull
    {
#if NET7_0_OR_GREATER
        if (!TPrimitive.TryCreate(rawValue, out _, out var error))
        {
            yield return new ValidationResult(
                error.Message ?? $"The value is not a valid {typeof(TPrimitive).Name}.",
                new[] { memberName });
        }
#else
        // Fallback for pre-.NET 7: cannot call static abstract interface member.
        yield break;
#endif
    }

    /// <summary>
    /// Validates a raw value as a domain primitive and returns a single <see cref="ValidationResult"/>
    /// or <see cref="ValidationResult.Success"/> if validation passes.
    /// </summary>
    /// <typeparam name="TPrimitive">The target domain primitive type.</typeparam>
    /// <typeparam name="TValue">The backing value type.</typeparam>
    /// <param name="rawValue">The raw value to validate.</param>
    /// <param name="memberName">The member name to associate with any validation error.</param>
    /// <returns>
    /// <see cref="ValidationResult.Success"/> if validation passes;
    /// a <see cref="ValidationResult"/> describing the first validation failure otherwise.
    /// </returns>
    public static ValidationResult? ValidateSingle<TPrimitive, TValue>(TValue rawValue, string memberName)
        where TPrimitive : struct, IDomainPrimitive<TPrimitive, TValue>
        where TValue : notnull
    {
#if NET7_0_OR_GREATER
        if (!TPrimitive.TryCreate(rawValue, out _, out var error))
        {
            return new ValidationResult(
                error.Message ?? $"The value is not a valid {typeof(TPrimitive).Name}.",
                new[] { memberName });
        }

        return ValidationResult.Success;
#else
        return ValidationResult.Success;
#endif
    }
}
