// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.EFCore.UnitTests;

/// <summary>
/// Test order ID primitive for EF Core converter unit tests.
/// </summary>
[StrongId<int>]
public readonly partial record struct EFCoreTestOrderId;
