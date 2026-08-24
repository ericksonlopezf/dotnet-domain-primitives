// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives.EFCore.SourceGenerators;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal readonly struct PrimitiveInfo : IEquatable<PrimitiveInfo>
{
    public string Namespace { get; }
    public string TypeName { get; }
    public string BackingType { get; }
    public bool IsSmartEnum { get; }
    public int? MaxLength { get; }
    public int? Precision { get; }
    public int? Scale { get; }

    public PrimitiveInfo(string @namespace, string typeName, string backingType, bool isSmartEnum = false, int? maxLength = null, int? precision = null, int? scale = null)
    {
        Namespace = @namespace;
        TypeName = typeName;
        BackingType = backingType;
        IsSmartEnum = isSmartEnum;
        MaxLength = maxLength;
        Precision = precision;
        Scale = scale;
    }
    
    public bool Equals(PrimitiveInfo other)
    {
        return Namespace == other.Namespace &&
               TypeName == other.TypeName &&
               BackingType == other.BackingType &&
               IsSmartEnum == other.IsSmartEnum &&
               MaxLength == other.MaxLength &&
               Precision == other.Precision &&
               Scale == other.Scale;
    }

    public override bool Equals(object? obj) => obj is PrimitiveInfo other && Equals(other);

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Namespace.GetHashCode();
            hashCode = (hashCode * 397) ^ TypeName.GetHashCode();
            hashCode = (hashCode * 397) ^ BackingType.GetHashCode();
            hashCode = (hashCode * 397) ^ IsSmartEnum.GetHashCode();
            hashCode = (hashCode * 397) ^ MaxLength.GetHashCode();
            hashCode = (hashCode * 397) ^ Precision.GetHashCode();
            hashCode = (hashCode * 397) ^ Scale.GetHashCode();
            return hashCode;
        }
    }
}
