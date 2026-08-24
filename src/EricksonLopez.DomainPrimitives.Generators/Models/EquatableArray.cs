// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace EricksonLopez.DomainPrimitives.Generators.Models;

/// <summary>
/// ImmutableArray wrapper that implements value equality for incremental generator caching.
/// </summary>
internal readonly struct EquatableArray<T> : IEquatable<EquatableArray<T>>
    where T : IEquatable<T>
{
    private readonly ImmutableArray<T> _array;

    public EquatableArray(ImmutableArray<T> array) => _array = array;
    public EquatableArray(IEnumerable<T> items)
    {
        var builder = ImmutableArray.CreateBuilder<T>();
        foreach (var item in items) builder.Add(item);
        _array = builder.ToImmutable();
    }

    internal ImmutableArray<T> Values => _array.IsDefault ? ImmutableArray<T>.Empty : _array;
    internal int Length => Values.Length;
    internal T this[int index] => Values[index];

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
        var values = Values;
        unchecked
        {
            int hash = 17;
            for (int i = 0; i < values.Length; i++)
            {
                var item = values[i];
                hash = hash * 31 + (item is not null ? item.GetHashCode() : 0);
            }
            return hash;
        }
    }

    public static implicit operator EquatableArray<T>(ImmutableArray<T> array) => new(array);
}
