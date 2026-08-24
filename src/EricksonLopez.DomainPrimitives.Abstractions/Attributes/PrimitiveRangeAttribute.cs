// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies range validation constraints on numeric domain primitives.
/// Use this instead of <c>RangeAttribute</c> when your project also references
/// <c>System.ComponentModel.DataAnnotations</c> to avoid ambiguous attribute errors.
/// </summary>
/// <remarks>
/// Accepts <c>double</c> bounds due to a C# language limitation — <c>decimal</c> is not
/// a valid attribute parameter type. For exact decimal ranges, use the <c>(string, string)</c>
/// constructor overload (e.g., <c>[PrimitiveRange("0.00", "1000000.00")]</c>).
/// </remarks>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public sealed class PrimitiveRangeAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PrimitiveRangeAttribute"/> class with the specified double bounds.
    /// </summary>
    /// <param name="min">The minimum allowed value (inclusive by default).</param>
    /// <param name="max">The maximum allowed value (inclusive by default).</param>
    public PrimitiveRangeAttribute(double min, double max)
    {
        Min = min;
        Max = max;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PrimitiveRangeAttribute"/> class with the specified string bounds for exact precision.
    /// </summary>
    /// <param name="stringMin">The minimum bound expressed as a parseable string. Use for exact <see langword="decimal"/> precision.</param>
    /// <param name="stringMax">The maximum bound expressed as a parseable string. Use for exact <see langword="decimal"/> precision.</param>
    public PrimitiveRangeAttribute(string stringMin, string stringMax)
    {
        StringMin = stringMin;
        StringMax = stringMax;
    }

    /// <summary>Gets the minimum allowed value when specified as a <see langword="double"/>. Zero when the string overload is used.</summary>
    public double Min { get; }
    /// <summary>Gets the maximum allowed value when specified as a <see langword="double"/>. Zero when the string overload is used.</summary>
    public double Max { get; }
    /// <summary>Gets the minimum bound as a string when specified via the string overload; otherwise <see langword="null"/>.</summary>
    public string? StringMin { get; }
    /// <summary>Gets the maximum bound as a string when specified via the string overload; otherwise <see langword="null"/>.</summary>
    public string? StringMax { get; }
    /// <summary>Gets or sets a value indicating whether the minimum bound is exclusive. Default: <see langword="false"/> (inclusive).</summary>
    public bool MinExclusive { get; init; }
    /// <summary>Gets or sets a value indicating whether the maximum bound is exclusive. Default: <see langword="false"/> (inclusive).</summary>
    public bool MaxExclusive { get; init; }
    /// <summary>Gets or sets the error code emitted when the range constraint is violated. Defaults to <c>"RANGE"</c>.</summary>
    public string? ErrorCode { get; set; } = "RANGE";
    /// <summary>Gets or sets the error message emitted when the value is outside the allowed range.</summary>
    public string? ErrorMessage { get; set; } = "Value is outside the allowed range.";
}
