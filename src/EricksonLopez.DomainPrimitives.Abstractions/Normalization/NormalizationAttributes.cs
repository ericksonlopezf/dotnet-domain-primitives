using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
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

/// <summary>
/// Specifies that leading whitespace only should be trimmed from the string value.
/// </summary>
/// <remarks>
/// Normalization runs before validation. Applied via <see cref="string.TrimStart()"/>.
/// Use this when trailing whitespace is meaningful but leading whitespace is not.
/// Cannot be combined with <see cref="TrimAttribute"/> — analyzer DP0005 reports a conflict.
/// </remarks>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class TrimStartAttribute : Attribute
{
}

/// <summary>
/// Specifies that trailing whitespace only should be trimmed from the string value.
/// </summary>
/// <remarks>
/// Normalization runs before validation. Applied via <see cref="string.TrimEnd()"/>.
/// Use this when leading whitespace is meaningful but trailing whitespace is not.
/// Cannot be combined with <see cref="TrimAttribute"/> — analyzer DP0005 reports a conflict.
/// </remarks>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class TrimEndAttribute : Attribute
{
}

/// <summary>
/// Specifies that the string value should be converted to lowercase using invariant culture.
/// </summary>
/// <remarks>
/// Normalization runs before validation. Applied via <see cref="string.ToLowerInvariant()"/>.
/// Cannot be combined with <see cref="UpperCaseAttribute"/> — analyzer DP0005 reports a conflict.
/// </remarks>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class LowerCaseAttribute : Attribute
{
}

/// <summary>
/// Specifies that the string value should be converted to uppercase using invariant culture.
/// </summary>
/// <remarks>
/// Normalization runs before validation. Applied via <see cref="string.ToUpperInvariant()"/>.
/// Cannot be combined with <see cref="LowerCaseAttribute"/> — analyzer DP0005 reports a conflict.
/// </remarks>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class UpperCaseAttribute : Attribute
{
}

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
