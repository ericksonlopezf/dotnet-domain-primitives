// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that a struct is a weight domain primitive.
/// Equivalent to: <c>[NumericPrimitive&lt;double&gt;] [PrimitiveRange(0, double.MaxValue)]</c>.
/// </summary>
/// <remarks>
/// Represents weight in kilograms (SI unit). For other units, use <c>[NumericPrimitive&lt;double&gt;]</c> directly.
/// </remarks>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class WeightAttribute : Attribute
{
    /// <summary>Gets the minimum allowed weight in kg. Default: 0.</summary>
    public double Min { get; init; } = 0;

    /// <summary>Gets the maximum allowed weight in kg. Default: 1000.</summary>
    public double Max { get; init; } = 1_000;
}
