// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives.Generators.Models;

/// <summary>
/// Equatable model representing a Numeric Primitive type to generate.
/// </summary>
internal sealed record NumericPrimitiveTypeInfo(
    string Namespace,
    string TypeName,
    string BackingTypeName,
    string Accessibility,
    EquatableArray<string> ContainingTypes,
    bool AllowAddition,
    bool AllowSubtraction,
    bool AllowScalarMultiplication,
    bool AllowScalarDivision,
    bool AllowNegation,
    double? RangeMin,
    double? RangeMax,
    bool RangeMinExclusive,
    bool RangeMaxExclusive,
    string? DomainShortcut,
    int? Scale = null,
    string? RangeStringMin = null,
    string? RangeStringMax = null,
    string? CustomExceptionType = null) : IEquatable<NumericPrimitiveTypeInfo>;
