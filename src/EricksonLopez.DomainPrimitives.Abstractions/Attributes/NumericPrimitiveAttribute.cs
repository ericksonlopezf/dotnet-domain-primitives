// Copyright © Erickson Lopez. MIT License.
using System;
using System.Numerics;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that a struct is a numeric-backed domain primitive.
/// </summary>
/// <remarks>
/// <para>
/// Use this attribute for numeric values that carry domain semantics beyond
/// their raw numeric value. Combine with <see cref="PrimitiveRangeAttribute"/>
/// to constrain the allowed range.
/// </para>
/// <para>
/// The source generator produces comparison operators, equality, formatting,
/// and parsing implementations. Arithmetic operators are generated based on
/// the <see cref="NumericOperations"/> value.
/// </para>
/// <para>
/// For domain-specific numeric types, use shortcut attributes like
/// <see cref="MoneyAttribute"/>, <see cref="PercentageAttribute"/>.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// [NumericPrimitive&lt;decimal&gt;(Operations = NumericOperations.Additive)]
/// [PrimitiveRange(0, 100)]
/// public readonly partial record struct Score;
/// </code>
/// </example>
/// <typeparam name="TValue">The backing numeric type (e.g., <see cref="int"/>, <see cref="decimal"/>, <see cref="double"/>).</typeparam>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class NumericPrimitiveAttribute<TValue> : Attribute
#if NET7_0_OR_GREATER
    where TValue : struct, INumber<TValue>
#else
    where TValue : struct
#endif
{
    /// <summary>
    /// Gets the allowed arithmetic operations for this numeric domain primitive.
    /// Default is <see cref="NumericOperations.None"/>.
    /// </summary>
    public NumericOperations Operations { get; init; } = NumericOperations.None;
}
