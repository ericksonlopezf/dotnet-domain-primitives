// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives.AspNetCore.SourceGenerators;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal readonly struct PrimitiveInfo : IEquatable<PrimitiveInfo>
{
    public string Namespace { get; }
    public string TypeName { get; }

    public PrimitiveInfo(string ns, string typeName)
    {
        Namespace = ns;
        TypeName = typeName;
    }

    public bool Equals(PrimitiveInfo other) => Namespace == other.Namespace && TypeName == other.TypeName;
    public override bool Equals(object? obj) => obj is PrimitiveInfo other && Equals(other);
    public override int GetHashCode() => (Namespace.GetHashCode() * 397) ^ TypeName.GetHashCode();
}
