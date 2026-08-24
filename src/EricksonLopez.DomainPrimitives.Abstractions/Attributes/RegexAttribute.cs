// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies a regular expression pattern that the string value must match.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public sealed class RegexAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RegexAttribute"/> class with the specified regular expression pattern.
    /// </summary>
    /// <param name="pattern">The regular expression pattern the value must match.</param>
    public RegexAttribute(string pattern) => Pattern = pattern;
    /// <summary>Gets the regular expression pattern the value must match.</summary>
    public string Pattern { get; }
    /// <summary>Gets or sets the error code emitted when the pattern is not matched. Defaults to <c>"FORMAT"</c>.</summary>
    public string? ErrorCode { get; set; } = "FORMAT";
    /// <summary>Gets or sets the error message emitted when the value does not match the pattern.</summary>
    public string? ErrorMessage { get; set; } = "Value does not match the required pattern.";
}
