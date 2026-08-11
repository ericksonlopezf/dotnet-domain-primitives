using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Marks a <c>readonly partial record struct</c> as a string-backed domain primitive.
/// </summary>
/// <remarks>
/// <para>
/// Use this attribute for string values that need type safety and optional
/// validation/normalization. Combine with validation attributes
/// (<see cref="Validation.MinLengthAttribute"/>, <see cref="Validation.MaxLengthAttribute"/>,
/// <see cref="Validation.RegexAttribute"/>) and normalization attributes
/// (<see cref="Normalization.TrimAttribute"/>, <see cref="Normalization.LowerCaseAttribute"/>)
/// to compose the desired behavior.
/// </para>
/// <para>
/// For domain-specific string types (email, phone, URL), use the shortcut attributes
/// (<see cref="EmailAttribute"/>, <see cref="PhoneAttribute"/>, <see cref="UrlAttribute"/>)
/// which combine <c>[StringPrimitive]</c> with appropriate validation and normalization.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// [StringPrimitive]
/// [Trim]
/// [MinLength(1)]
/// [MaxLength(100)]
/// public readonly partial record struct FirstName;
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class StringPrimitiveAttribute : Attribute
{
}
