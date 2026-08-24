// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

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
    /// <summary>
    /// Initializes a new instance of the <see cref="ExactLengthAttribute"/> class with the specified exact length.
    /// </summary>
    /// <param name="length">The exact number of characters the value must have.</param>
    public ExactLengthAttribute(int length) => Length = length;
    /// <summary>Gets the required exact length.</summary>
    public int Length { get; }
    /// <summary>Gets or sets the error code emitted when the constraint is violated. Defaults to <c>"LENGTH"</c>.</summary>
    public string? ErrorCode { get; set; } = "LENGTH";
    /// <summary>Gets or sets the error message. If null, the generator produces a message with the expected length and actual length.</summary>
    public string? ErrorMessage { get; set; }
}
