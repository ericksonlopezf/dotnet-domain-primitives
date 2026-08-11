using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Shortcut for a monetary value domain primitive.
/// Equivalent to: <c>[NumericPrimitive&lt;decimal&gt;] [Range(0, max)]</c>
/// with addition, subtraction, and scalar multiplication/division operators.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class MoneyAttribute : Attribute
{
    /// <summary>Initializes a new instance of <see cref="MoneyAttribute"/> with default USD currency and no explicit range constraints.</summary>
    public MoneyAttribute() { }

    /// <summary>Initializes a new instance of <see cref="MoneyAttribute"/> with the specified currency code.</summary>
    /// <param name="currency">The ISO 4217 currency code (e.g., <c>"USD"</c>, <c>"EUR"</c>).</param>
    public MoneyAttribute(string currency)
    {
        Currency = currency;
    }

    /// <summary>Initializes a new instance of <see cref="MoneyAttribute"/> with the specified currency code and value range.</summary>
    /// <param name="currency">The ISO 4217 currency code (e.g., <c>"USD"</c>, <c>"EUR"</c>).</param>
    /// <param name="min">The minimum allowed monetary value (inclusive). Use 0 for non-negative amounts.</param>
    /// <param name="max">The maximum allowed monetary value (inclusive). Defaults to <see cref="double.MaxValue"/>.</param>
    public MoneyAttribute(string currency, double min, double max = double.MaxValue)
    {
        Currency = currency;
        Min = min;
        Max = max;
    }

    /// <summary>ISO 4217 currency code. Default: "USD".</summary>
    public string Currency { get; init; } = "USD";

    /// <summary>Minimum allowed value. Default: 0.</summary>
    public double Min { get; init; }

    /// <summary>Maximum allowed value. Default: <see cref="double.MaxValue"/>.</summary>
    public double Max { get; init; } = double.MaxValue;
}

/// <summary>
/// Shortcut for a percentage value domain primitive (0-100 scale).
/// Equivalent to: <c>[NumericPrimitive&lt;decimal&gt;] [Range(0, 100)]</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class PercentageAttribute : Attribute
{
    /// <summary>Minimum allowed value. Default: 0.</summary>
    public double Min { get; init; }

    /// <summary>Maximum allowed value. Default: 100.</summary>
    public double Max { get; init; } = 100;
}

/// <summary>
/// Shortcut for a Latitude domain primitive.
/// Equivalent to: <c>[NumericPrimitive&lt;double&gt;] [Range(-90, 90)]</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class LatitudeAttribute : Attribute
{
}

/// <summary>
/// Shortcut for a Longitude domain primitive.
/// Equivalent to: <c>[NumericPrimitive&lt;double&gt;] [Range(-180, 180)]</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class LongitudeAttribute : Attribute
{
}

/// <summary>
/// Shortcut for an Age domain primitive.
/// Equivalent to: <c>[NumericPrimitive&lt;int&gt;] [Range(0, 150)]</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class AgeAttribute : Attribute
{
}

/// <summary>
/// Shortcut for a Weight domain primitive.
/// Equivalent to: <c>[NumericPrimitive&lt;double&gt;] [Range(0, double.MaxValue)]</c>.
/// </summary>
/// <remarks>
/// Represents weight in kilograms (SI unit). For other units, use <c>[NumericPrimitive&lt;double&gt;]</c> directly.
/// </remarks>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class WeightAttribute : Attribute
{
    /// <summary>Minimum allowed weight in kg. Default: 0.</summary>
    public double Min { get; init; } = 0;

    /// <summary>Maximum allowed weight in kg. Default: 1000 (appropriate for most human/object scenarios).</summary>
    public double Max { get; init; } = 1_000;
}

/// <summary>
/// Shortcut for a Height domain primitive.
/// Equivalent to: <c>[NumericPrimitive&lt;double&gt;] [Range(0, 300)]</c>.
/// </summary>
/// <remarks>
/// Represents height in centimeters. Maximum 300 cm covers the tallest recorded human measurements.
/// </remarks>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class HeightAttribute : Attribute
{
    /// <summary>Minimum allowed height in cm. Default: 0.</summary>
    public double Min { get; init; } = 0;

    /// <summary>Maximum allowed height in cm. Default: 300.</summary>
    public double Max { get; init; } = 300;
}

/// <summary>
/// Shortcut for a Distance domain primitive.
/// Equivalent to: <c>[NumericPrimitive&lt;double&gt;] [Range(0, double.MaxValue)]</c>.
/// </summary>
/// <remarks>
/// Represents distance in meters (SI unit). No upper bound by default — suitable for geographic
/// distances. Override <see cref="Max"/> for domain-specific constraints.
/// </remarks>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class DistanceAttribute : Attribute
{
    /// <summary>Minimum allowed distance in meters. Default: 0.</summary>
    public double Min { get; init; } = 0;

    /// <summary>Maximum allowed distance in meters. Default: <see cref="double.MaxValue"/>.</summary>
    public double Max { get; init; } = double.MaxValue;
}

/// <summary>
/// Shortcut for a Temperature domain primitive.
/// Equivalent to: <c>[NumericPrimitive&lt;double&gt;] [Range(-273.15, double.MaxValue)]</c>.
/// </summary>
/// <remarks>
/// Default minimum of -273.15°C corresponds to absolute zero (0 Kelvin).
/// This works for Celsius units. Override <see cref="Min"/> for Fahrenheit or Kelvin.
/// </remarks>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class TemperatureAttribute : Attribute
{
    /// <summary>Minimum temperature. Default: -273.15 (absolute zero in Celsius).</summary>
    public double Min { get; init; } = -273.15;

    /// <summary>Maximum temperature. Default: <see cref="double.MaxValue"/>.</summary>
    public double Max { get; init; } = double.MaxValue;
}

/// <summary>
/// Shortcut for a Score domain primitive.
/// Equivalent to: <c>[NumericPrimitive&lt;int&gt;] [Range(0, 100)]</c>.
/// </summary>
/// <remarks>
/// Represents a generic score on a 0-100 integer scale. For decimal scores or
/// different ranges, use <c>[NumericPrimitive&lt;T&gt;]</c> directly.
/// </remarks>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class ScoreAttribute : Attribute
{
    /// <summary>Minimum score (inclusive). Default: 0.</summary>
    public int Min { get; init; } = 0;

    /// <summary>Maximum score (inclusive). Default: 100.</summary>
    public int Max { get; init; } = 100;
}

/// <summary>
/// Shortcut for a Quantity domain primitive.
/// Equivalent to: <c>[NumericPrimitive&lt;int&gt;] [Range(0, int.MaxValue)]</c>.
/// </summary>
/// <remarks>
/// Represents a non-negative integer count (units in stock, items ordered, etc.).
/// </remarks>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class QuantityAttribute : Attribute
{
    /// <summary>Minimum quantity (inclusive). Default: 0.</summary>
    public int Min { get; init; } = 0;

    /// <summary>Maximum quantity (inclusive). Default: <see cref="int.MaxValue"/>.</summary>
    public int Max { get; init; } = int.MaxValue;
}

/// <summary>
/// Shortcut for a Price domain primitive.
/// Equivalent to: <c>[NumericPrimitive&lt;decimal&gt;] [Range(0, double.MaxValue)]</c>
/// with addition and subtraction operators.
/// </summary>
/// <remarks>
/// Differs from <see cref="MoneyAttribute"/> in that it does not carry a currency code.
/// Use <see cref="MoneyAttribute"/> when the currency is part of the domain concept.
/// </remarks>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class PriceAttribute : Attribute
{
    /// <summary>Minimum price. Default: 0.</summary>
    public double Min { get; init; } = 0;

    /// <summary>Maximum price. Default: <see cref="double.MaxValue"/>.</summary>
    public double Max { get; init; } = double.MaxValue;
}

/// <summary>
/// Shortcut for a TaxRate domain primitive.
/// Equivalent to: <c>[NumericPrimitive&lt;decimal&gt;] [Range(0, 100)]</c>.
/// </summary>
/// <remarks>
/// Represents a tax rate as a percentage (e.g., 21.0 for 21%). To use a 0–1 decimal
/// fraction instead, set <c>Max = 1</c>.
/// </remarks>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class TaxRateAttribute : Attribute
{
    /// <summary>Minimum tax rate percentage. Default: 0.</summary>
    public double Min { get; init; } = 0;

    /// <summary>Maximum tax rate percentage. Default: 100.</summary>
    public double Max { get; init; } = 100;
}

/// <summary>
/// Shortcut for a Discount domain primitive.
/// Equivalent to: <c>[NumericPrimitive&lt;decimal&gt;] [Range(0, 100)]</c>.
/// </summary>
/// <remarks>
/// Represents a discount as a percentage (e.g., 15.0 for 15% off).
/// </remarks>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class DiscountAttribute : Attribute
{
    /// <summary>Minimum discount percentage. Default: 0.</summary>
    public double Min { get; init; } = 0;

    /// <summary>Maximum discount percentage. Default: 100.</summary>
    public double Max { get; init; } = 100;
}

/// <summary>
/// Shortcut for a Rating domain primitive.
/// Equivalent to: <c>[NumericPrimitive&lt;decimal&gt;] [Range(0, 5)]</c>.
/// </summary>
/// <remarks>
/// Represents a star rating on a 0–5 scale with decimal precision (e.g., 4.5 stars).
/// Adjust <see cref="Min"/>, <see cref="Max"/>, and <see cref="Scale"/> for other rating systems (e.g., 1–10).
/// </remarks>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class RatingAttribute : Attribute
{
    /// <summary>Minimum rating. Default: 0.</summary>
    public double Min { get; init; } = 0;

    /// <summary>Maximum rating. Default: 5.</summary>
    public double Max { get; init; } = 5;

    /// <summary>
    /// Number of decimal places allowed. Default: 1 (e.g., 4.5).
    /// Use 0 for whole-number-only ratings.
    /// </summary>
    public int Scale { get; init; } = 1;
}
