// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that a struct is a discount domain primitive.
/// Equivalent to: <c>[NumericPrimitive&lt;decimal&gt;] [PrimitiveRange(0, 100)]</c>.
/// </summary>
/// <remarks>
/// Represents a discount as a percentage (e.g., 15.0 for 15% off).
/// </remarks>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class DiscountAttribute : Attribute
{
    /// <summary>Gets the minimum discount percentage. Default: 0.</summary>
    public double Min { get; init; } = 0;

    /// <summary>Gets the maximum discount percentage. Default: 100.</summary>
    public double Max { get; init; } = 100;
}
