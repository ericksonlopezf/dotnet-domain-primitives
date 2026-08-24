// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that a struct is an email address domain primitive.
/// Equivalent to: <c>[StringPrimitive] [Trim] [LowerCase] [MaxLength(320)] [Regex(RFC5322)]</c>.
/// </summary>
/// <remarks>
/// <para>Generates RFC 5322-compliant email validation with automatic trimming and lowercasing.</para>
/// <para>Override <see cref="MaxLength"/> to change the default 320-character limit.</para>
/// </remarks>
/// <example>
/// <code>
/// [Email]
/// public readonly partial record struct EmailAddress;
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class EmailAttribute : Attribute
{
    /// <summary>Gets the maximum length. Default: 320 (RFC 5321).</summary>
    public int MaxLength { get; init; } = 320;
}
