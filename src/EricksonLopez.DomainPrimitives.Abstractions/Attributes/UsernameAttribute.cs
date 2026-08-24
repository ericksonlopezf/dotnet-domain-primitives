// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that a struct is a username domain primitive.
/// Equivalent to: <c>[StringPrimitive] [Trim] [Regex(alphanumeric + ._-)]</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class UsernameAttribute : Attribute
{
    /// <summary>Gets the minimum length. Default: 3.</summary>
    public int MinLength { get; init; } = 3;

    /// <summary>Gets the maximum length. Default: 50.</summary>
    public int MaxLength { get; init; } = 50;
}
