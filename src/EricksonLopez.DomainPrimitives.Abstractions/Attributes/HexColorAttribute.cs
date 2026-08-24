// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that a struct is a CSS hex color domain primitive (e.g., "#FF5733").
/// Equivalent to: <c>[StringPrimitive] [Trim] [UpperCase] [Regex(hex-color)]</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class HexColorAttribute : Attribute
{
}
