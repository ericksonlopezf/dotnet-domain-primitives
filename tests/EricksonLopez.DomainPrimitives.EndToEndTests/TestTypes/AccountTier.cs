// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.EndToEndTests;

/// <summary>
/// Test account tier smart enum for end-to-end tests.
/// </summary>
[SmartEnum<int>]
public readonly partial record struct AccountTier
{
    public static readonly AccountTier Bronze = new(1, "Bronze");
    public static readonly AccountTier Silver = new(2, "Silver");
    public static readonly AccountTier Gold = new(3, "Gold");
}
