// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that a struct is an ISO 3166-1 alpha-2 country code (e.g., "US", "DE").
/// Equivalent to: <c>[StringPrimitive] [Trim] [UpperCase] [Length(2, 2)]</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class CountryCodeAttribute : Attribute
{
}
