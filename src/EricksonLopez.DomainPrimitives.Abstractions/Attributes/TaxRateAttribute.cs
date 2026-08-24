// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that a struct is a tax rate domain primitive.
/// Equivalent to: <c>[NumericPrimitive&lt;decimal&gt;] [PrimitiveRange(0, 100)]</c>.
/// </summary>
/// <remarks>
/// Represents a tax rate as a percentage (e.g., 21.0 for 21%). To use a 0–1 decimal
/// fraction instead, set <c>Max = 1</c>.
/// </remarks>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class TaxRateAttribute : Attribute
{
    /// <summary>Gets the minimum tax rate percentage. Default: 0.</summary>
    public double Min { get; init; } = 0;

    /// <summary>Gets the maximum tax rate percentage. Default: 100.</summary>
    public double Max { get; init; } = 100;
}
