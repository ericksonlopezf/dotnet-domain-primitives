// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that a struct is a distance domain primitive.
/// Equivalent to: <c>[NumericPrimitive&lt;double&gt;] [PrimitiveRange(0, double.MaxValue)]</c>.
/// </summary>
/// <remarks>
/// Represents distance in meters (SI unit). No upper bound by default — suitable for geographic
/// distances. Override <see cref="Max"/> for domain-specific constraints.
/// </remarks>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class DistanceAttribute : Attribute
{
    /// <summary>Gets the minimum allowed distance in meters. Default: 0.</summary>
    public double Min { get; init; } = 0;

    /// <summary>Gets the maximum allowed distance in meters. Default: <see cref="double.MaxValue"/>.</summary>
    public double Max { get; init; } = double.MaxValue;
}
