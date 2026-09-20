// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;
using AwesomeAssertions;
using EricksonLopez.DomainPrimitives.UnitTests.TestTypes;
using FsCheck;
using FsCheck.Xunit;
using Xunit;

namespace EricksonLopez.DomainPrimitives.UnitTests;

/// <summary>
/// Property-based contract testing establishing mathematical invariants for domain primitives.
/// </summary>
public class PropertyBasedContractTests
{
    // ─── Equality Properties ──────────────────────────────────────────────────

    [Property(MaxTest = 100)]
    public bool Equality_Is_Reflexive_For_FirstName(NonEmptyString raw)
    {
        var input = raw.Get;
        if (input.Length > 100) input = input.Substring(0, 100);

        if (FirstName.TryCreate(input, out var p1, out _))
        {
            var same = p1;
            return p1.Equals(same) && (p1 == same) && !(p1 != same);
        }

        return true;
    }

    [Property(MaxTest = 100)]
    public bool Equality_Is_Symmetric_For_FirstName(NonEmptyString raw)
    {
        var input = raw.Get;
        if (input.Length > 100) input = input.Substring(0, 100);

        if (FirstName.TryCreate(input, out var p1, out _) &&
            FirstName.TryCreate(input, out var p2, out _))
        {
            return (p1.Equals(p2) == p2.Equals(p1)) && ((p1 == p2) == (p2 == p1));
        }

        return true;
    }

    [Property(MaxTest = 100)]
    public bool HashCode_Follows_Equality_For_FirstName(NonEmptyString raw)
    {
        var input = raw.Get;
        if (input.Length > 100) input = input.Substring(0, 100);

        if (FirstName.TryCreate(input, out var p1, out _) &&
            FirstName.TryCreate(input, out var p2, out _))
        {
            if (p1.Equals(p2))
            {
                return p1.GetHashCode() == p2.GetHashCode();
            }
        }

        return true;
    }

    // ─── Parsing & Formatting Round-Trip Property ─────────────────────────────

    [Property(MaxTest = 100)]
    public bool Parse_ToString_RoundTrip_For_FirstName(NonEmptyString raw)
    {
        var input = raw.Get;
        if (input.Length > 100) input = input.Substring(0, 100);

        if (FirstName.TryCreate(input, out var original, out _))
        {
            var serialized = original.ToString();
            var parsed = FirstName.Parse(serialized, null);
            return parsed == original;
        }

        return true;
    }

    // ─── Numeric Primitive Mathematical Properties ────────────────────────────

    [Property(MaxTest = 100)]
    public bool Numeric_Equality_Follows_Value_Equality(int val)
    {
        if (Score.TryCreate(val, out var s1, out _) &&
            Score.TryCreate(val, out var s2, out _))
        {
            return s1 == s2 && s1.GetHashCode() == s2.GetHashCode();
        }

        return true;
    }

    [Property(MaxTest = 100)]
    public bool Numeric_Comparison_Consistent_With_Operators(int a, int b)
    {
        if (Score.TryCreate(a, out var s1, out _) &&
            Score.TryCreate(b, out var s2, out _))
        {
            int comp = s1.CompareTo(s2);
            bool lt = s1 < s2;
            bool gt = s1 > s2;
            bool eq = s1 == s2;

            if (comp < 0) return lt && !gt && !eq;
            if (comp > 0) return !lt && gt && !eq;
            return !lt && !gt && eq;
        }

        return true;
    }

    // ─── Strongly Typed ID Properties ─────────────────────────────────────────

    [Property(MaxTest = 100)]
    public bool StrongId_Guid_RoundTrip_Parse(Guid idVal)
    {
        if (idVal == Guid.Empty) return true;

        if (CustomerId.TryCreate(idVal, out var id, out _))
        {
            var str = id.ToString();
            var parsed = CustomerId.Parse(str, null);
            return parsed == id && parsed.Value == idVal;
        }

        return true;
    }

    [Property(MaxTest = 100)]
    public bool StrongId_Dictionary_Lookup_Consistency(Guid[] idValues)
    {
        if (idValues == null || idValues.Length == 0) return true;

        var dict = new Dictionary<CustomerId, string>();
        var validIds = new List<CustomerId>();

        foreach (var g in idValues)
        {
            if (g != Guid.Empty && CustomerId.TryCreate(g, out var id, out _))
            {
                dict[id] = id.ToString();
                validIds.Add(id);
            }
        }

        foreach (var id in validIds)
        {
            if (!dict.TryGetValue(id, out var val) || val != id.ToString())
                return false;
        }

        return true;
    }
}
