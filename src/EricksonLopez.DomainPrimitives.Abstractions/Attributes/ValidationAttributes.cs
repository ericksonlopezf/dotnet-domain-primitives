using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
namespace EricksonLopez.DomainPrimitives;



/// <summary>
/// Specifies the minimum length for a string domain primitive.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public sealed class MinLengthAttribute : Attribute
{
    /// <param name="length">The minimum number of characters the value must have (inclusive).</param>
    public MinLengthAttribute(int length) => Length = length;
    /// <summary>Gets the minimum required length (inclusive).</summary>
    public int Length { get; }
    /// <summary>Gets or sets the error code emitted when the constraint is violated. Defaults to <c>"LENGTH"</c>.</summary>
    public string? ErrorCode { get; set; } = "LENGTH";
    /// <summary>Gets or sets the error message emitted when the value is shorter than the minimum length.</summary>
    public string? ErrorMessage { get; set; } = "Value is too short.";
}

/// <summary>
/// Specifies the maximum length for a string domain primitive.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public sealed class MaxLengthAttribute : Attribute
{
    /// <param name="length">The maximum number of characters the value may have (inclusive).</param>
    public MaxLengthAttribute(int length) => Length = length;
    /// <summary>Gets the maximum allowed length (inclusive).</summary>
    public int Length { get; }
    /// <summary>Gets or sets the error code emitted when the constraint is violated. Defaults to <c>"LENGTH"</c>.</summary>
    public string? ErrorCode { get; set; } = "LENGTH";
    /// <summary>Gets or sets the error message emitted when the value exceeds the maximum length.</summary>
    public string? ErrorMessage { get; set; } = "Value is too long.";
}

/// <summary>
/// Specifies both minimum and maximum length for a string domain primitive.
/// </summary>
/// <remarks>
/// Combines <see cref="MinLengthAttribute"/> and <see cref="MaxLengthAttribute"/> in a single attribute.
/// The generated error code is <c>"LENGTH"</c> by default and can be customized via <see cref="ErrorCode"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public sealed class LengthAttribute : Attribute
{
    /// <param name="min">The minimum allowed length (inclusive).</param>
    /// <param name="max">The maximum allowed length (inclusive).</param>
    public LengthAttribute(int min, int max)
    {
        Min = min;
        Max = max;
    }
    /// <summary>Gets the minimum allowed length (inclusive).</summary>
    public int Min { get; }
    /// <summary>Gets the maximum allowed length (inclusive).</summary>
    public int Max { get; }
    /// <summary>Gets or sets the error code emitted when the length constraint is violated. Defaults to <c>"LENGTH"</c>.</summary>
    public string? ErrorCode { get; set; } = "LENGTH";
    /// <summary>Gets or sets the error message emitted when the length constraint is violated.</summary>
    public string? ErrorMessage { get; set; } = "Value length is outside the allowed range.";
}

/// <summary>
/// Specifies that a string domain primitive must have exactly <c>n</c> characters.
/// </summary>
/// <remarks>
/// Convenience shortcut for <c>[Length(n, n)]</c>. The generated code enforces
/// <c>value.Length == length</c> with a <c>"LENGTH"</c> error code.
/// Common uses: ISO 3166-1 alpha-2 country codes (2), VIN numbers (17), IBAN check digits, etc.
/// </remarks>
/// <example>
/// <code>
/// [StringPrimitive]
/// [ExactLength(2)]
/// public readonly partial record struct IsoAlpha2Code;
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public sealed class ExactLengthAttribute : Attribute
{
    /// <param name="length">The exact number of characters the value must have.</param>
    public ExactLengthAttribute(int length) => Length = length;
    /// <summary>Gets the required exact length.</summary>
    public int Length { get; }
    /// <summary>Gets or sets the error code emitted when the constraint is violated. Defaults to <c>"LENGTH"</c>.</summary>
    public string? ErrorCode { get; set; } = "LENGTH";
    /// <summary>Gets or sets the error message. If null, the generator produces a message with the expected length and actual length.</summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Specifies a regular expression pattern that the string value must match.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public sealed class RegexAttribute : Attribute
{
    /// <param name="pattern">The regular expression pattern the value must match.</param>
    public RegexAttribute(string pattern) => Pattern = pattern;
    /// <summary>Gets the regular expression pattern the value must match.</summary>
    public string Pattern { get; }
    /// <summary>Gets or sets the error code emitted when the pattern is not matched. Defaults to <c>"FORMAT"</c>.</summary>
    public string? ErrorCode { get; set; } = "FORMAT";
    /// <summary>Gets or sets the error message emitted when the value does not match the pattern.</summary>
    public string? ErrorMessage { get; set; } = "Value does not match the required pattern.";
}

/// <summary>
/// Preferred, collision-free alias for range validation on domain primitives.
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
    /// <param name="min">The minimum allowed value (inclusive by default).</param>
    /// <param name="max">The maximum allowed value (inclusive by default).</param>
    public PrimitiveRangeAttribute(double min, double max)
    {
        Min = min;
        Max = max;
    }

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

/// <summary>
/// Domain-oriented alias for range validation on domain primitives.
/// Semantically equivalent to <see cref="PrimitiveRangeAttribute"/>; use whichever reads
/// more naturally for the domain concept being modeled.
/// </summary>
/// <remarks>
/// Accepts <c>double</c> bounds due to a C# language limitation — <c>decimal</c> is not
/// a valid attribute parameter type. For exact decimal ranges, use the <c>(string, string)</c>
/// constructor overload (e.g., <c>[DomainRange("0.00", "1000000.00")]</c>).
/// </remarks>


/// <summary>
/// Specifies that a string domain primitive must not be empty or whitespace-only.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public sealed class NotEmptyAttribute : Attribute
{
    /// <summary>Gets or sets the error code emitted when the value is empty or whitespace-only. Defaults to <c>"EMPTY"</c>.</summary>
    public string? ErrorCode { get; set; } = "EMPTY";
    /// <summary>Gets or sets the error message emitted when the value is empty or whitespace-only.</summary>
    public string? ErrorMessage { get; set; } = "Value must not be empty.";
}
