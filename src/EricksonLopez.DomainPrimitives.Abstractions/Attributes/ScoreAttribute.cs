// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that a struct is a score domain primitive.
/// Equivalent to: <c>[NumericPrimitive&lt;int&gt;] [PrimitiveRange(0, 100)]</c>.
/// </summary>
/// <remarks>
/// Represents a generic score on a 0-100 integer scale. For decimal scores or
/// different ranges, use <c>[NumericPrimitive&lt;T&gt;]</c> directly.
/// </remarks>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class ScoreAttribute : Attribute
{
    /// <summary>Gets the minimum score (inclusive). Default: 0.</summary>
    public int Min { get; init; } = 0;

    /// <summary>Gets the maximum score (inclusive). Default: 100.</summary>
    public int Max { get; init; } = 100;
}
