// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives.OpenApi.SourceGenerators;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal readonly struct PrimitiveInfo : IEquatable<PrimitiveInfo>
{
    public string Namespace { get; }
    public string TypeName { get; }
    public string OpenApiType { get; }
    public string OpenApiFormat { get; }
    public bool IsSmartEnum { get; }

    public PrimitiveInfo(string @namespace, string typeName, string openApiType, string openApiFormat, bool isSmartEnum = false)
    {
        Namespace = @namespace;
        TypeName = typeName;
        OpenApiType = openApiType;
        OpenApiFormat = openApiFormat;
        IsSmartEnum = isSmartEnum;
    }

    public bool Equals(PrimitiveInfo other)
    {
        return Namespace == other.Namespace &&
               TypeName == other.TypeName &&
               OpenApiType == other.OpenApiType &&
               OpenApiFormat == other.OpenApiFormat &&
               IsSmartEnum == other.IsSmartEnum;
    }

    public override bool Equals(object? obj) => obj is PrimitiveInfo other && Equals(other);

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Namespace.GetHashCode();
            hashCode = (hashCode * 397) ^ TypeName.GetHashCode();
            hashCode = (hashCode * 397) ^ OpenApiType.GetHashCode();
            hashCode = (hashCode * 397) ^ OpenApiFormat.GetHashCode();
            hashCode = (hashCode * 397) ^ IsSmartEnum.GetHashCode();
            return hashCode;
        }
    }
}
