// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives.Generators.Models;

/// <summary>
/// Equatable model representing a Value Object type to generate.
/// </summary>
internal sealed record ValueObjectTypeInfo(
    string Namespace,
    string TypeName,
    string Accessibility,
    EquatableArray<string> ContainingTypes,
    EquatableArray<ValueObjectPropertyInfo> Properties,
    string? CustomExceptionType = null) : IEquatable<ValueObjectTypeInfo>;
