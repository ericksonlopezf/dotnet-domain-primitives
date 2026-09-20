// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives.Dapper.SourceGenerators;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal readonly struct ValueObjectProperty : IEquatable<ValueObjectProperty>
{
    public string Name { get; }
    public string Type { get; }
    public string? BackingType { get; }

    public ValueObjectProperty(string name, string type) : this(name, type, null)
    {
    }

    public ValueObjectProperty(string name, string type, string? backingType)
    {
        Name = name;
        Type = type;
        BackingType = backingType;
    }

    public bool Equals(ValueObjectProperty other) => Name == other.Name && Type == other.Type && BackingType == other.BackingType;
    public override bool Equals(object? obj) => obj is ValueObjectProperty other && Equals(other);
    public override int GetHashCode()
    {
        unchecked
        {
            var hash = (Name.GetHashCode() * 397) ^ Type.GetHashCode();
            return BackingType != null ? (hash * 397) ^ BackingType.GetHashCode() : hash;
        }
    }
}
