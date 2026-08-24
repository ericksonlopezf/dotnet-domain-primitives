// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that a struct is an IPv4 or IPv6 address domain primitive.
/// Equivalent to: <c>[StringPrimitive] [Trim] [Regex(ip-pattern)]</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class IPAddressAttribute : Attribute
{
}
