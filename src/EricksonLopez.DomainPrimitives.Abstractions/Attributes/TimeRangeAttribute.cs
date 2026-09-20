// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that a struct is a time range domain primitive.
/// <para><strong>Architectural Guidance:</strong> In Domain-Driven Design, when modeling a compound interval with both start and end times and cross-property validation (<c>Start &lt;= End</c>), prefer creating a compound <c>[ValueObject]</c>.</para>
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class TimeRangeAttribute : Attribute
{
}
