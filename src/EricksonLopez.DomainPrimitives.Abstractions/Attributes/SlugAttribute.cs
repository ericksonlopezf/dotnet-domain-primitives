// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that a struct is a URL slug domain primitive.
/// Equivalent to: <c>[StringPrimitive] [Trim] [LowerCase] [Regex(slug-pattern)]</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class SlugAttribute : Attribute
{
    /// <summary>Gets the maximum length. Default: 200.</summary>
    public int MaxLength { get; init; } = 200;
}
