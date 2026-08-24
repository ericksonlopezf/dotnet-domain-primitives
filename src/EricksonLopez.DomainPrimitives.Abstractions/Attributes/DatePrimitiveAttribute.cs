// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that a struct is a date- or time-backed domain primitive.
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
    /// Gets the backing temporal type. Default: <see cref="DatePrimitiveKind.DateOnly"/>.
    /// </summary>
    public DatePrimitiveKind Kind { get; init; } = DatePrimitiveKind.DateOnly;

    /// <summary>
    /// Gets a value indicating whether only past dates or times are allowed (before current UTC time).
    /// </summary>
    public bool PastOnly { get; init; }

    /// <summary>
    /// Gets a value indicating whether only future dates or times are allowed (after current UTC time).
    /// </summary>
    public bool FutureOnly { get; init; }
}


