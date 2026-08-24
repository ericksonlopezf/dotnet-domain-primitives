// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that a struct is a latitude domain primitive.
/// Equivalent to: <c>[NumericPrimitive&lt;double&gt;] [PrimitiveRange(-90, 90)]</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class LatitudeAttribute : Attribute
{
}
