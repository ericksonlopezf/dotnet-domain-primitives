// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives.Dapper.SourceGenerators;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal sealed class ValueObjectInfo : IEquatable<ValueObjectInfo>
{
    public string Namespace { get; }
    public string TypeName { get; }
    public EquatableArray<ValueObjectProperty> Properties { get; }

    public ValueObjectInfo(string ns, string typeName, EquatableArray<ValueObjectProperty> properties)
    {
        Namespace = ns;
        TypeName = typeName;
        Properties = properties;
    }

    public bool Equals(ValueObjectInfo? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Namespace == other.Namespace && TypeName == other.TypeName && Properties.Equals(other.Properties);
    }

    public override bool Equals(object? obj) => Equals(obj as ValueObjectInfo);

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Namespace.GetHashCode();
            hashCode = (hashCode * 397) ^ TypeName.GetHashCode();
            hashCode = (hashCode * 397) ^ Properties.GetHashCode();
            return hashCode;
        }
    }
}
