using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
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

/// <summary>
/// Applies a custom <see cref="ICustomValidator{T}"/> to a domain primitive.
/// </summary>
/// <remarks>
/// The validator is invoked after built-in validation (normalization → built-in rules → custom).
/// Multiple <c>[CustomValidator]</c> attributes can be stacked.
/// </remarks>
/// <example>
/// <code>
/// [StringPrimitive]
/// [CustomValidator&lt;LuhnCheckValidator&gt;]
/// public readonly partial record struct CreditCardNumber;
/// </code>
/// </example>
/// <typeparam name="TValidator">
/// The validator type implementing <see cref="ICustomValidator{T}"/>.
/// Must have a parameterless constructor or use only static methods.
/// </typeparam>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
public sealed class CustomValidatorAttribute<TValidator> : Attribute
{
}
