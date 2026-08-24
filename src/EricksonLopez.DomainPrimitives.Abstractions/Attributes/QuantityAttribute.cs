// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that a struct is a quantity domain primitive.
/// Equivalent to: <c>[NumericPrimitive&lt;int&gt;] [PrimitiveRange(0, int.MaxValue)]</c>.
/// </summary>
/// <remarks>
/// Represents a non-negative integer count (units in stock, items ordered, etc.).
/// </remarks>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class QuantityAttribute : Attribute
{
    /// <summary>Gets the minimum quantity (inclusive). Default: 0.</summary>
    public int Min { get; init; } = 0;

    /// <summary>Gets the maximum quantity (inclusive). Default: <see cref="int.MaxValue"/>.</summary>
    public int Max { get; init; } = int.MaxValue;
}
