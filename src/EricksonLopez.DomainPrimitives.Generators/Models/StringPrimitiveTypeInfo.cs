// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives.Generators.Models;

/// <summary>
/// Equatable model representing a String Primitive type to generate.
/// </summary>
internal sealed record StringPrimitiveTypeInfo(
    string Namespace,
    string TypeName,
    string Accessibility,
    EquatableArray<string> ContainingTypes,
    // Normalization
    bool Trim,
    bool TrimStart,
    bool TrimEnd,
    bool LowerCase,
    bool UpperCase,
    bool NormalizeWhitespace,
    // Validation
    int? MinLength,
    int? MaxLength,
    int? ExactLength,
    bool NotEmpty,
    EquatableArray<RegexInfo> RegexPatterns,
    // Domain shortcut
    string? DomainShortcut,
    bool HasCustomValidator,
    EquatableArray<string> AllowedSchemes = default,
    string? CustomExceptionType = null) : IEquatable<StringPrimitiveTypeInfo>;
