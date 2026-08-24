// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that a struct is a birth date domain primitive.
/// Equivalent to: <c>[DatePrimitive(Kind = DateOnly, PastOnly = true)]</c>.
/// </summary>
/// <remarks>
/// Validates that the date is in the past and not more than <see cref="MaxAge"/> years ago.
/// Generates an <c>Age</c> computed property.
/// </remarks>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class BirthDateAttribute : Attribute
{
    /// <summary>Gets the maximum allowed age in years. Default: 150.</summary>
    public int MaxAge { get; init; } = 150;
}
