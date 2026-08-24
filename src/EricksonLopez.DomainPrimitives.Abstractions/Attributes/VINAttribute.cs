// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that a struct is a Vehicle Identification Number (VIN) domain primitive.
/// Equivalent to: <c>[StringPrimitive] [Trim] [UpperCase] [Regex(vin-pattern)]</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class VINAttribute : Attribute
{
}
