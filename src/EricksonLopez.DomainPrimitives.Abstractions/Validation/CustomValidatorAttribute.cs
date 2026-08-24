// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives.Validation;

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
