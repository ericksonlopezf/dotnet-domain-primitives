// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that a struct is a week domain primitive.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class WeekAttribute : Attribute
{
    /// <summary>Gets a value indicating whether ISO 8601 week numbering is used. Default: <see langword="true"/>.</summary>
    public bool IsoWeekNumbering { get; init; } = true;
}
