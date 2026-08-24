// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that a struct is a percentage value domain primitive (0-100 scale).
/// Equivalent to: <c>[NumericPrimitive&lt;decimal&gt;] [PrimitiveRange(0, 100)]</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class PercentageAttribute : Attribute
{
    /// <summary>Gets the minimum allowed value. Default: 0.</summary>
    public double Min { get; init; }

    /// <summary>Gets the maximum allowed value. Default: 100.</summary>
    public double Max { get; init; } = 100;
}
