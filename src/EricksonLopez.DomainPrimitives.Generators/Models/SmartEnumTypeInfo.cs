// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives.Generators.Models;

/// <summary>
/// Equatable model representing a Smart Enum type to generate.
/// </summary>
internal sealed record SmartEnumTypeInfo(
    string Namespace,
    string TypeName,
    string BackingTypeName,
    EquatableArray<string> MemberNames,
    bool IsReferenceType,
    string? CustomExceptionType = null) : IEquatable<SmartEnumTypeInfo>
{
    public string FullName => string.IsNullOrEmpty(Namespace) ? TypeName : $"{Namespace}.{TypeName}";
}
