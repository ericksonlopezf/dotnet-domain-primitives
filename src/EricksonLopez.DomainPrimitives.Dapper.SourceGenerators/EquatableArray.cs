// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace EricksonLopez.DomainPrimitives.Dapper.SourceGenerators;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal readonly struct EquatableArray<T> : IEquatable<EquatableArray<T>>
    where T : IEquatable<T>
{
    private readonly ImmutableArray<T> _array;

    public EquatableArray(ImmutableArray<T> array) => _array = array;
    public EquatableArray(IEnumerable<T> items) => _array = items.ToImmutableArray();

    public ImmutableArray<T> Values => _array.IsDefault ? ImmutableArray<T>.Empty : _array;
    public int Length => Values.Length;
    public T this[int index] => Values[index];

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
        unchecked
        {
            var hashCode = 0;
            foreach (var item in Values)
            {
                hashCode = (hashCode * 397) ^ item.GetHashCode();
            }
            return hashCode;
        }
    }
}
