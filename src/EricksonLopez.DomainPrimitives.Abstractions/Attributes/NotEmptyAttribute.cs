// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

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
