// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that a struct is a phone number domain primitive.
/// Equivalent to: <c>[StringPrimitive] [Trim] [Regex(E.164)]</c>.
/// </summary>
/// <remarks>
/// Generates E.164-compliant phone number validation.
/// </remarks>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class PhoneAttribute : Attribute
{
}
