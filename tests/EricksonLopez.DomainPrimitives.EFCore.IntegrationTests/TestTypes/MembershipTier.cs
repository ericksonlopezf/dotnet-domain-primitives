// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.EFCore.IntegrationTests;

/// <summary>
/// Test membership tier smart enum for EF Core integration tests.
/// </summary>
[SmartEnum<int>]
public readonly partial record struct MembershipTier
{
    public static readonly MembershipTier Standard = new(1, "Standard");
    public static readonly MembershipTier Premium = new(2, "Premium");
    public static readonly MembershipTier Vip = new(3, "Vip");
}
