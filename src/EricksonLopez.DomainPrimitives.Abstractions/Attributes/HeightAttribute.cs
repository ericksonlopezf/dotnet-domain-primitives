// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that a struct is a height domain primitive.
/// Equivalent to: <c>[NumericPrimitive&lt;double&gt;] [PrimitiveRange(0, 300)]</c>.
/// </summary>
/// <remarks>
/// Represents height in centimeters. Maximum 300 cm covers the tallest recorded human measurements.
/// </remarks>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class HeightAttribute : Attribute
{
    /// <summary>Gets the minimum allowed height in cm. Default: 0.</summary>
    public double Min { get; init; } = 0;

    /// <summary>Gets the maximum allowed height in cm. Default: 300.</summary>
    public double Max { get; init; } = 300;
}
