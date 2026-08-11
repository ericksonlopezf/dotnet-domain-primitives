using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Numerics;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Defines the allowed arithmetic operations for a numeric domain primitive.
/// </summary>
[Flags]
public enum NumericOperations
{
    /// <summary>No arithmetic operations are allowed.</summary>
    None = 0,
    /// <summary>Allows addition.</summary>
    Addition = 1 << 0,
    /// <summary>Allows subtraction.</summary>
    Subtraction = 1 << 1,
    /// <summary>Allows scalar multiplication.</summary>
    ScalarMultiplication = 1 << 2,
    /// <summary>Allows scalar division.</summary>
    ScalarDivision = 1 << 3,
    /// <summary>Allows unary negation.</summary>
    Negation = 1 << 4,
    /// <summary>Allows addition and subtraction.</summary>
    Additive = Addition | Subtraction | Negation,
    /// <summary>Allows scalar multiplication and division.</summary>
    Multiplicative = ScalarMultiplication | ScalarDivision,
    /// <summary>Allows all arithmetic operations.</summary>
    All = Additive | Multiplicative
}

/// <summary>
/// Deprecated alias for <see cref="NumericOperations"/>. Use <see cref="NumericOperations"/> instead.
/// </summary>
/// <remarks>
/// <strong>DEPRECATED:</strong> This enum was renamed to <see cref="NumericOperations"/> to align with
/// Framework Design Guidelines naming conventions. Replace all uses of <c>ArithmeticPolicy</c> with
/// <see cref="NumericOperations"/>. Will be removed in v3.0.
/// </remarks>
[Obsolete(
    "ArithmeticPolicy is deprecated. Use NumericOperations instead. " +
    "Replace [NumericPrimitive<T>(Policy = ArithmeticPolicy.X)] with " +
    "[NumericPrimitive<T>(Operations = NumericOperations.X)]. Will be removed in v3.0. " +
    "See BREAKING_CHANGES.md.",
    error: false)]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
[Flags]
public enum ArithmeticPolicy
{
    /// <summary>No arithmetic operations are allowed.</summary>
    None = 0,
    /// <summary>Allows addition.</summary>
    Addition = 1,
    /// <summary>Allows subtraction.</summary>
    Subtraction = 2,
    /// <summary>Allows scalar multiplication.</summary>
    ScalarMultiplication = 4,
    /// <summary>Allows scalar division.</summary>
    ScalarDivision = 8,
    /// <summary>Allows unary negation.</summary>
    Negation = 16,
    /// <summary>Allows addition and subtraction.</summary>
    Additive = Addition | Subtraction | Negation,
    /// <summary>Allows scalar multiplication and division.</summary>
    Multiplicative = ScalarMultiplication | ScalarDivision,
    /// <summary>Allows all arithmetic operations.</summary>
    All = Multiplicative | Additive
}

/// <summary>
/// Marks a <c>readonly partial record struct</c> as a numeric-backed domain primitive.
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
/// [Range(0, 100)]
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
    /// Specifies which arithmetic operations are allowed for this primitive.
    /// Default is <see cref="NumericOperations.None"/>.
    /// </summary>
    public NumericOperations Operations { get; init; } = NumericOperations.None;

    /// <summary>
    /// Deprecated. Use <see cref="Operations"/> instead.
    /// </summary>
    /// <remarks>
    /// <strong>DEPRECATED:</strong> This property was renamed to <see cref="Operations"/>.
    /// Replace <c>[NumericPrimitive&lt;T&gt;(Policy = ArithmeticPolicy.X)]</c> with
    /// <c>[NumericPrimitive&lt;T&gt;(Operations = NumericOperations.X)]</c>. Will be removed in v3.0.
    /// </remarks>
    [Obsolete(
        "NumericPrimitiveAttribute.Policy is deprecated. Use Operations instead. " +
        "Replace Policy = ArithmeticPolicy.X with Operations = NumericOperations.X. " +
        "Will be removed in v3.0. See BREAKING_CHANGES.md.",
        error: false)]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#pragma warning disable CS0618 // Intentional deprecated-to-deprecated bridge
    public ArithmeticPolicy Policy
    {
        get => (ArithmeticPolicy)(int)Operations;
        init => Operations = (NumericOperations)(int)value;
    }
#pragma warning restore CS0618
}
