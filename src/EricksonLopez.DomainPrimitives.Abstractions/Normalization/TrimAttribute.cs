// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that leading and trailing whitespace should be trimmed from the string value.
/// </summary>
/// <remarks>
/// Normalization runs before validation. Applied via <see cref="string.Trim()"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class TrimAttribute : Attribute
{
}
