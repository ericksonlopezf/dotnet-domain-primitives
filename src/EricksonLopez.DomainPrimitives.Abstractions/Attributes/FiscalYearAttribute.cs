// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that a struct is a fiscal year domain primitive.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class FiscalYearAttribute : Attribute
{
    /// <summary>Gets the minimum valid fiscal year. Default: 1900.</summary>
    public int MinYear { get; init; } = 1900;
}
