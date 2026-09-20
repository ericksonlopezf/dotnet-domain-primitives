// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.EFCore.IntegrationTests;

/// <summary>
/// Test loyalty points primitive for EF Core integration tests.
/// </summary>
[NumericPrimitive<int>]
public readonly partial record struct LoyaltyPoints;
