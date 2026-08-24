// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that a struct is a business date domain primitive.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class BusinessDateAttribute : Attribute
{
    /// <summary>Gets a value indicating whether weekend dates are allowed. Default: <see langword="false"/>.</summary>
    public bool AllowWeekends { get; init; } = false;
}
