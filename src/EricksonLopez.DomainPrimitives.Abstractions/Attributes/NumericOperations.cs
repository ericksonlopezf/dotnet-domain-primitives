// Copyright © Erickson Lopez. MIT License.
using System;

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
