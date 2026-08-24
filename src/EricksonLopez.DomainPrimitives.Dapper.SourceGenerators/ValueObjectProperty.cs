// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives.Dapper.SourceGenerators;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal readonly struct ValueObjectProperty : IEquatable<ValueObjectProperty>
{
    public string Name { get; }
    public string Type { get; }

    public ValueObjectProperty(string name, string type)
    {
        Name = name;
        Type = type;
    }

    public bool Equals(ValueObjectProperty other) => Name == other.Name && Type == other.Type;
    public override bool Equals(object? obj) => obj is ValueObjectProperty other && Equals(other);
    public override int GetHashCode()
    {
        unchecked
        {
            return (Name.GetHashCode() * 397) ^ Type.GetHashCode();
        }
    }
}
