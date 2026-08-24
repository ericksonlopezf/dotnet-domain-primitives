// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that a struct is a temperature domain primitive.
/// Equivalent to: <c>[NumericPrimitive&lt;double&gt;] [PrimitiveRange(-273.15, double.MaxValue)]</c>.
/// </summary>
/// <remarks>
/// Default minimum of -273.15°C corresponds to absolute zero (0 Kelvin).
/// This works for Celsius units. Override <see cref="Min"/> for Fahrenheit or Kelvin.
/// </remarks>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class TemperatureAttribute : Attribute
{
    /// <summary>Gets the minimum temperature. Default: -273.15 (absolute zero in Celsius).</summary>
    public double Min { get; init; } = -273.15;

    /// <summary>Gets the maximum temperature. Default: <see cref="double.MaxValue"/>.</summary>
    public double Max { get; init; } = double.MaxValue;
}
