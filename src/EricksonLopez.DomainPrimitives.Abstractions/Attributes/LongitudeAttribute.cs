// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that a struct is a longitude domain primitive.
/// Equivalent to: <c>[NumericPrimitive&lt;double&gt;] [PrimitiveRange(-180, 180)]</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class LongitudeAttribute : Attribute
{
}
