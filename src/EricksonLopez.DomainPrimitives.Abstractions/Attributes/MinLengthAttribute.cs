// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies the minimum length for a string domain primitive.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public sealed class MinLengthAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MinLengthAttribute"/> class with the specified minimum length.
    /// </summary>
    /// <param name="length">The minimum number of characters the value must have (inclusive).</param>
    public MinLengthAttribute(int length) => Length = length;
    /// <summary>Gets the minimum required length (inclusive).</summary>
    public int Length { get; }
    /// <summary>Gets or sets the error code emitted when the constraint is violated. Defaults to <c>"LENGTH"</c>.</summary>
    public string? ErrorCode { get; set; } = "LENGTH";
    /// <summary>Gets or sets the error message emitted when the value is shorter than the minimum length.</summary>
    public string? ErrorMessage { get; set; } = "Value is too short.";
}
