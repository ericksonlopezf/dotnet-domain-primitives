using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies the backing temporal type for a date primitive.
/// </summary>
public enum DatePrimitiveKind
{
    /// <summary>
    /// Backed by <see cref="System.DateOnly"/>. Use for dates without time components.
    /// </summary>
    DateOnly,

    /// <summary>
    /// Backed by <see cref="System.DateTime"/>. Use when both date and time are needed.
    /// </summary>
    DateTime,

    /// <summary>
    /// Backed by <see cref="System.DateTimeOffset"/>. Use when timezone-aware timestamps are needed.
    /// </summary>
    DateTimeOffset,

    /// <summary>
    /// Backed by <see cref="System.TimeOnly"/>. Use for time-of-day without a date component.
    /// </summary>
    TimeOnly
}
