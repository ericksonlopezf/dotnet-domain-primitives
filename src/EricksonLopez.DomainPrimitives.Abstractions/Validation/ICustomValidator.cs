// Copyright © Erickson Lopez. MIT License.

namespace EricksonLopez.DomainPrimitives.Validation;

/// <summary>
/// Defines a reusable custom validator for domain primitive values.
/// </summary>
/// <remarks>
/// <para>
/// Implement this interface to create reusable validation logic that can be
/// applied to multiple domain primitives via <see cref="CustomValidatorAttribute{T}"/>.
/// </para>
/// <para>
/// For one-off validation, use the <c>static partial void ValidateCustom</c>
/// hook method directly on the domain primitive type instead.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// public sealed class LuhnCheckValidator : ICustomValidator&lt;string&gt;
/// {
///     public static PrimitiveError Validate(string value)
///     {
///         var error = PrimitiveError.None;
///         if (!LuhnCheck(value))
///             error = new PrimitiveError("Luhn.InvalidCheckDigit", "Value has an invalid check digit.");
///         return error;
///     }
/// }
/// </code>
/// </example>
/// <typeparam name="T">The type of value being validated.</typeparam>
public interface ICustomValidator<in T>
{
#if NET7_0_OR_GREATER
    /// <summary>
    /// Validates the specified value and returns any validation errors.
    /// </summary>
    /// <param name="value">The value to validate (already normalized).</param>
    /// <returns>The validation errors produced during validation, or an empty collection if validation passes.</returns>
    static abstract PrimitiveError Validate(T value);
#endif
}
