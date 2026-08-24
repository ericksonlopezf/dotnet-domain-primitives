// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies the maximum length for a string domain primitive.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public sealed class MaxLengthAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MaxLengthAttribute"/> class with the specified maximum length.
    /// </summary>
    /// <param name="length">The maximum number of characters the value may have (inclusive).</param>
    public MaxLengthAttribute(int length) => Length = length;
    /// <summary>Gets the maximum allowed length (inclusive).</summary>
    public int Length { get; }
    /// <summary>Gets or sets the error code emitted when the constraint is violated. Defaults to <c>"LENGTH"</c>.</summary>
    public string? ErrorCode { get; set; } = "LENGTH";
    /// <summary>Gets or sets the error message emitted when the value exceeds the maximum length.</summary>
    public string? ErrorMessage { get; set; } = "Value is too long.";
}
