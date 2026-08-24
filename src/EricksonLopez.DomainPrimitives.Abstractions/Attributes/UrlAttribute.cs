// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that a struct is a URL domain primitive.
/// Equivalent to: <c>[StringPrimitive] [Trim]</c> with <see cref="System.Uri"/> validation.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class UrlAttribute : Attribute
{
    /// <summary>
    /// Gets the allowed URI schemes. Default: <c>["https", "http"]</c>.
    /// </summary>
    public string[] AllowedSchemes { get; init; } = ["https", "http"];
}
