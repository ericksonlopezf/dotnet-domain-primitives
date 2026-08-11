using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Marks a <c>readonly partial record struct</c> as a date/time-backed domain primitive.
/// </summary>
/// <remarks>
/// <para>
/// Use this attribute for temporal values that carry domain-specific validation
/// or calculation rules. Use <see cref="Kind"/> to select the backing type.
/// </para>
/// <para>
/// For domain-specific date types, use shortcut attributes like
/// <see cref="BirthDateAttribute"/>, <see cref="ExpirationDateAttribute"/>.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// [DatePrimitive(Kind = DatePrimitiveKind.DateOnly)]
/// public readonly partial record struct RegistrationDate;
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class DatePrimitiveAttribute : Attribute
{
    /// <summary>
    /// The backing temporal type. Default: <see cref="DatePrimitiveKind.DateOnly"/>.
    /// </summary>
    public DatePrimitiveKind Kind { get; init; } = DatePrimitiveKind.DateOnly;

    /// <summary>
    /// If <c>true</c>, only past dates/times are allowed (before <c>now</c>).
    /// Default: <c>false</c>.
    /// </summary>
    public bool PastOnly { get; init; }

    /// <summary>
    /// If <c>true</c>, only future dates/times are allowed (after <c>now</c>).
    /// Default: <c>false</c>.
    /// </summary>
    public bool FutureOnly { get; init; }
}
