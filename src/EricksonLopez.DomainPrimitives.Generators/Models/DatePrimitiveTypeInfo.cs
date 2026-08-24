// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives.Generators.Models;

/// <summary>
/// Equatable model representing a Date Primitive type to generate.
/// </summary>
internal sealed record DatePrimitiveTypeInfo(
    string Namespace,
    string TypeName,
    string BackingTypeName,
    string Accessibility,
    EquatableArray<string> ContainingTypes,
    string Kind,
    bool PastOnly,
    bool FutureOnly,
    int? MaxAge,
    string? DomainShortcut,
    string? CustomExceptionType = null) : IEquatable<DatePrimitiveTypeInfo>;
