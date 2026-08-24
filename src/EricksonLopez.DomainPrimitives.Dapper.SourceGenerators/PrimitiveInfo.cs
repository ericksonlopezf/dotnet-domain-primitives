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

    public PrimitiveInfo(string @namespace, string typeName, string backingType, bool isSmartEnum = false)
    {
        Namespace = @namespace;
        TypeName = typeName;
        BackingType = backingType;
        IsSmartEnum = isSmartEnum;
    }

    public bool Equals(PrimitiveInfo other) =>
        Namespace == other.Namespace &&
        TypeName == other.TypeName &&
        BackingType == other.BackingType &&
        IsSmartEnum == other.IsSmartEnum;

    public override bool Equals(object? obj) => obj is PrimitiveInfo other && Equals(other);

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Namespace.GetHashCode();
            hashCode = (hashCode * 397) ^ TypeName.GetHashCode();
            hashCode = (hashCode * 397) ^ BackingType.GetHashCode();
            hashCode = (hashCode * 397) ^ IsSmartEnum.GetHashCode();
            return hashCode;
        }
    }
}
