// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that a struct is an expiration date domain primitive.
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
