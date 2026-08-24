// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that a struct is a password hash domain primitive.
/// Equivalent to: <c>[StringPrimitive] [NotEmpty]</c>.
/// </summary>
/// <remarks>
/// No normalization is applied — hashes must never be trimmed or case-changed.
/// </remarks>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class PasswordHashAttribute : Attribute
{
}
