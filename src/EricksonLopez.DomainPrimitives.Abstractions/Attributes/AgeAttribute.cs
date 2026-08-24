// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that a struct is an age domain primitive.
/// Equivalent to: <c>[NumericPrimitive&lt;int&gt;] [PrimitiveRange(0, 150)]</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class AgeAttribute : Attribute
{
}
