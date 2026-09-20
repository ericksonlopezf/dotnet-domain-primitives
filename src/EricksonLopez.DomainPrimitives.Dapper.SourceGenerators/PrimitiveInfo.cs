// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives.Dapper.SourceGenerators;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal readonly struct PrimitiveInfo : IEquatable<PrimitiveInfo>
{
    public string Namespace { get; }
    public string TypeName { get; }
    public string BackingType { get; }
    public bool IsSmartEnum { get; }

    /// <summary>
    /// When <see langword="true"/>, the type's static <c>Create()</c> factory returns <c>Result&lt;T&gt;</c>
    /// instead of <c>T</c> directly. The generated TypeHandler will check <c>result.IsFailure</c>
    /// and throw <see cref="System.Data.DataException"/> on corruption instead of blindly calling <c>.Value</c>.
    /// This is the correct behavior for domain Value Objects with validation invariants.
    /// </summary>
    public bool IsResultReturning { get; }

    public PrimitiveInfo(string @namespace, string typeName, string backingType, bool isSmartEnum = false, bool isResultReturning = false)
    {
        Namespace = @namespace;
        TypeName = typeName;
        BackingType = backingType;
        IsSmartEnum = isSmartEnum;
        IsResultReturning = isResultReturning;
    }

    public bool Equals(PrimitiveInfo other) =>
        Namespace == other.Namespace &&
        TypeName == other.TypeName &&
        BackingType == other.BackingType &&
        IsSmartEnum == other.IsSmartEnum &&
        IsResultReturning == other.IsResultReturning;

    public override bool Equals(object? obj) => obj is PrimitiveInfo other && Equals(other);

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Namespace.GetHashCode();
            hashCode = (hashCode * 397) ^ TypeName.GetHashCode();
            hashCode = (hashCode * 397) ^ BackingType.GetHashCode();
            hashCode = (hashCode * 397) ^ IsSmartEnum.GetHashCode();
            hashCode = (hashCode * 397) ^ IsResultReturning.GetHashCode();
            return hashCode;
        }
    }
}
