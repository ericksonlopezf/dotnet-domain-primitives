// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that a struct is an ISO 639-1 language code (e.g., "en", "es").
/// Equivalent to: <c>[StringPrimitive] [Trim] [LowerCase] [Length(2, 2)]</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class LanguageCodeAttribute : Attribute
{
}
