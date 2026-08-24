// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that internal whitespace should be normalized by collapsing consecutive
/// whitespace characters into a single space.
/// </summary>
/// <remarks>
/// Applied after <see cref="TrimAttribute"/> in the normalization pipeline.
/// Useful for display names, titles, and similar user-facing strings.
/// </remarks>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class NormalizeWhitespaceAttribute : Attribute
{
}
