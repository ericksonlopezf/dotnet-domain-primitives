using System.Diagnostics.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace EricksonLopez.DomainPrimitives.Generators.Models;

/// <summary>
/// Equatable model representing a Strong ID type to generate.
/// Extracted from the syntax/semantic analysis phase and used in the generation phase.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed record StrongIdTypeInfo(
    string Namespace,
    string TypeName,
    string BackingTypeName,
    string BackingTypeFullName,
    string Accessibility,
    EquatableArray<string> ContainingTypes,
    bool RejectEmpty) : IEquatable<StrongIdTypeInfo>
{
    /// <summary>
    /// Whether the backing type is Guid (has New() factory).
    /// </summary>
    public bool IsGuidBacked => BackingTypeFullName is "System.Guid" or "Guid";

    /// <summary>
    /// Whether the backing type is string.
    /// </summary>
    public bool IsStringBacked => BackingTypeFullName is "System.String" or "string";

    /// <summary>
    /// Whether the backing type is an integer type (int, long).
    /// </summary>
    public bool IsIntegerBacked => BackingTypeFullName is "System.Int32" or "System.Int64" or "int" or "long";
}

/// <summary>
/// Equatable model representing a String Primitive type to generate.
/// </summary>
[ExcludeFromCodeCoverage]
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
    EquatableArray<string> AllowedSchemes = default) : IEquatable<StringPrimitiveTypeInfo>;


/// <summary>
/// Information about a regex pattern applied to a string primitive.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed record RegexInfo(string Pattern, string? ErrorMessage);

/// <summary>
/// Equatable model representing a Numeric Primitive type to generate.
/// </summary>
[ExcludeFromCodeCoverage]
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
    string? RangeStringMax = null) : IEquatable<NumericPrimitiveTypeInfo>;

/// <summary>
/// Equatable model representing a Date Primitive type to generate.
/// </summary>
[ExcludeFromCodeCoverage]
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
    string? DomainShortcut) : IEquatable<DatePrimitiveTypeInfo>;

/// <summary>
/// Equatable model representing a Smart Enum type to generate.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed record SmartEnumTypeInfo(
    string Namespace,
    string TypeName,
    string BackingTypeName,
    EquatableArray<string> MemberNames,
    bool IsReferenceType) : IEquatable<SmartEnumTypeInfo>
{
    public string FullName => string.IsNullOrEmpty(Namespace) ? TypeName : $"{Namespace}.{TypeName}";
}

/// <summary>
/// Equatable model representing a Value Object type to generate.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed record ValueObjectTypeInfo(
    string Namespace,
    string TypeName,
    string Accessibility,
    EquatableArray<string> ContainingTypes,
    EquatableArray<ValueObjectPropertyInfo> Properties) : IEquatable<ValueObjectTypeInfo>;

/// <summary>
/// Information about a property on a value object.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed record ValueObjectPropertyInfo(string Name, string TypeName, string CamelCaseName);

/// <summary>
/// ImmutableArray wrapper that implements value equality for incremental generator caching.
/// </summary>
[ExcludeFromCodeCoverage]
internal readonly struct EquatableArray<T> : IEquatable<EquatableArray<T>>
    where T : IEquatable<T>
{
    private readonly ImmutableArray<T> _array;

    public EquatableArray(ImmutableArray<T> array) => _array = array;
    public EquatableArray(IEnumerable<T> items)
    {
        var builder = ImmutableArray.CreateBuilder<T>();
        foreach (var item in items) builder.Add(item);
        _array = builder.ToImmutable();
    }

    internal ImmutableArray<T> Values => _array.IsDefault ? ImmutableArray<T>.Empty : _array;
    internal int Length => Values.Length;
    internal T this[int index] => Values[index];

    public bool Equals(EquatableArray<T> other)
    {
        var self = Values;
        var otherValues = other.Values;

        if (self.Length != otherValues.Length)
            return false;

        for (int i = 0; i < self.Length; i++)
        {
            if (!self[i].Equals(otherValues[i]))
                return false;
        }

        return true;
    }

    public override bool Equals(object? obj) => obj is EquatableArray<T> other && Equals(other);

    public override int GetHashCode()
    {
        var values = Values;
        unchecked
        {
            int hash = 17;
            for (int i = 0; i < values.Length; i++)
            {
                var item = values[i];
                hash = hash * 31 + (item is not null ? item.GetHashCode() : 0);
            }
            return hash;
        }
    }

    public static implicit operator EquatableArray<T>(ImmutableArray<T> array) => new(array);
}


