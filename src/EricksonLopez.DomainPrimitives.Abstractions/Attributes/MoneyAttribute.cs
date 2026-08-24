// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that a struct is a monetary value domain primitive.
/// Equivalent to: <c>[NumericPrimitive&lt;decimal&gt;] [PrimitiveRange(0, max)]</c>
/// with addition, subtraction, and scalar multiplication/division operators.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class MoneyAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="MoneyAttribute"/> class with default USD currency and no explicit range constraints.</summary>
    public MoneyAttribute() { }

    /// <summary>Initializes a new instance of the <see cref="MoneyAttribute"/> class with the specified currency code.</summary>
    /// <param name="currency">The ISO 4217 currency code (e.g., <c>"USD"</c>, <c>"EUR"</c>).</param>
    public MoneyAttribute(string currency)
    {
        Currency = currency;
    }

    /// <summary>Initializes a new instance of the <see cref="MoneyAttribute"/> class with the specified currency code and value range.</summary>
    /// <param name="currency">The ISO 4217 currency code (e.g., <c>"USD"</c>, <c>"EUR"</c>).</param>
    /// <param name="min">The minimum allowed monetary value (inclusive). Use 0 for non-negative amounts.</param>
    /// <param name="max">The maximum allowed monetary value (inclusive). Defaults to <see cref="double.MaxValue"/>.</param>
    public MoneyAttribute(string currency, double min, double max = double.MaxValue)
    {
        Currency = currency;
        Min = min;
        Max = max;
    }

    /// <summary>Gets the ISO 4217 currency code. Default: "USD".</summary>
    public string Currency { get; init; } = "USD";

    /// <summary>Gets the minimum allowed value. Default: 0.</summary>
    public double Min { get; init; }

    /// <summary>Gets the maximum allowed value. Default: <see cref="double.MaxValue"/>.</summary>
    public double Max { get; init; } = double.MaxValue;
}
