// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that a struct is an International Bank Account Number (IBAN) domain primitive.
/// Equivalent to: <c>[StringPrimitive] [Trim] [UpperCase] [Regex(iban-pattern)]</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class IBANAttribute : Attribute
{
}
