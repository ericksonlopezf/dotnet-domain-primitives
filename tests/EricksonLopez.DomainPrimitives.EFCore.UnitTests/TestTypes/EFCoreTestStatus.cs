// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.EFCore.UnitTests;

/// <summary>
/// Test smart enum primitive for EF Core converter unit tests.
/// </summary>
[SmartEnum<int>]
public readonly partial record struct EFCoreTestStatus
{
    public static readonly EFCoreTestStatus Pending = new(1, "Pending");
    public static readonly EFCoreTestStatus Completed = new(2, "Completed");
}
