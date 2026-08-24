// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that a struct is a price domain primitive.
/// Equivalent to: <c>[NumericPrimitive&lt;decimal&gt;] [PrimitiveRange(0, double.MaxValue)]</c>
/// with addition and subtraction operators.
/// </summary>
/// <remarks>
/// Differs from <see cref="MoneyAttribute"/> in that it does not carry a currency code.
/// Use <see cref="MoneyAttribute"/> when the currency is part of the domain concept.
/// </remarks>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class PriceAttribute : Attribute
{
    /// <summary>Gets the minimum price. Default: 0.</summary>
    public double Min { get; init; } = 0;

    /// <summary>Gets the maximum price. Default: <see cref="double.MaxValue"/>.</summary>
    public double Max { get; init; } = double.MaxValue;
}
