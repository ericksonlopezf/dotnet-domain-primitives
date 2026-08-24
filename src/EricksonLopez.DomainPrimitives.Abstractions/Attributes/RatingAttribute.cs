// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that a struct is a rating domain primitive.
/// Equivalent to: <c>[NumericPrimitive&lt;decimal&gt;] [PrimitiveRange(0, 5)]</c>.
/// </summary>
/// <remarks>
/// Represents a star rating on a 0–5 scale with decimal precision (e.g., 4.5 stars).
/// Adjust <see cref="Min"/>, <see cref="Max"/>, and <see cref="Scale"/> for other rating systems (e.g., 1–10).
/// </remarks>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class RatingAttribute : Attribute
{
    /// <summary>Gets the minimum rating. Default: 0.</summary>
    public double Min { get; init; } = 0;

    /// <summary>Gets the maximum rating. Default: 5.</summary>
    public double Max { get; init; } = 5;

    /// <summary>
    /// Gets the number of decimal places allowed. Default: 1 (e.g., 4.5).
    /// Use 0 for whole-number-only ratings.
    /// </summary>
    public int Scale { get; init; } = 1;
}
