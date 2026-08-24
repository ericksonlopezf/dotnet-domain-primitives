// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives.Generators.Models;

/// <summary>
/// Equatable model representing a Strong ID type to generate.
/// Extracted from the syntax/semantic analysis phase and used in the generation phase.
/// </summary>
internal sealed record StrongIdTypeInfo(
    string Namespace,
    string TypeName,
    string BackingTypeName,
    string BackingTypeFullName,
    string Accessibility,
    EquatableArray<string> ContainingTypes,
    bool RejectEmpty,
    string? CustomExceptionType = null) : IEquatable<StrongIdTypeInfo>
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
