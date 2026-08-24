// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

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
    /// <summary>
    /// Initializes a new instance of the <see cref="LengthAttribute"/> class with the specified minimum and maximum lengths.
    /// </summary>
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
