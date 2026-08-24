// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that a struct is an ISO 4217 currency code (e.g., "USD", "EUR").
/// Equivalent to: <c>[StringPrimitive] [Trim] [UpperCase] [Length(3, 3)]</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class CurrencyCodeAttribute : Attribute
{
}
