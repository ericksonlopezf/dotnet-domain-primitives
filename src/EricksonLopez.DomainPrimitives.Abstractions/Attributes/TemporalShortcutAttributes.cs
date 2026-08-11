using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Shortcut for a birth date domain primitive.
/// Equivalent to: <c>[DatePrimitive(Kind = DateOnly, PastOnly = true)]</c>.
/// </summary>
/// <remarks>
/// Validates that the date is in the past and not more than <see cref="MaxAge"/> years ago.
/// Generates an <c>Age</c> computed property.
/// </remarks>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class BirthDateAttribute : Attribute
{
    /// <summary>Maximum allowed age in years. Default: 150.</summary>
    public int MaxAge { get; init; } = 150;
}

/// <summary>
/// Shortcut for an expiration date domain primitive.
/// Equivalent to: <c>[DatePrimitive(Kind = DateOnly, FutureOnly = true)]</c>.
/// </summary>
/// <remarks>
/// Validates that the date is in the future.
/// Generates <c>IsExpired()</c> and <c>DaysUntilExpiration()</c> methods.
/// </remarks>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class ExpirationDateAttribute : Attribute
{
}

/// <summary>
/// Shortcut for a BusinessDate domain primitive.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class BusinessDateAttribute : Attribute
{
    /// <summary>Whether to allow weekends. Default: false.</summary>
    public bool AllowWeekends { get; init; } = false;
}

/// <summary>
/// Shortcut for a FiscalYear domain primitive.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class FiscalYearAttribute : Attribute
{
    /// <summary>Minimum valid fiscal year. Default: 1900.</summary>
    public int MinYear { get; init; } = 1900;
}

/// <summary>
/// Shortcut for a Month domain primitive.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class MonthAttribute : Attribute
{
}

/// <summary>
/// Shortcut for a Quarter domain primitive.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class QuarterAttribute : Attribute
{
}

/// <summary>
/// Shortcut for a Week domain primitive.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class WeekAttribute : Attribute
{
    /// <summary>Whether to use ISO 8601 week numbering. Default: true.</summary>
    public bool IsoWeekNumbering { get; init; } = true;
}

/// <summary>
/// Shortcut for a DateRange domain primitive.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class DateRangeAttribute : Attribute
{
}

/// <summary>
/// Shortcut for a TimeRange domain primitive.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class TimeRangeAttribute : Attribute
{
}
