// Copyright © Erickson Lopez. MIT License.

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies the backing temporal type for a date primitive.
/// </summary>
public enum DatePrimitiveKind
{
    /// <summary>
    /// Specifies backing by <see cref="System.DateOnly"/> for dates without time components.
    /// </summary>
    DateOnly,

    /// <summary>
    /// Specifies backing by <see cref="System.DateTime"/> when both date and time are needed.
    /// </summary>
    DateTime,

    /// <summary>
    /// Specifies backing by <see cref="System.DateTimeOffset"/> when timezone-aware timestamps are needed.
    /// </summary>
    DateTimeOffset,

    /// <summary>
    /// Specifies backing by <see cref="System.TimeOnly"/> for time-of-day without a date component.
    /// </summary>
    TimeOnly
}


