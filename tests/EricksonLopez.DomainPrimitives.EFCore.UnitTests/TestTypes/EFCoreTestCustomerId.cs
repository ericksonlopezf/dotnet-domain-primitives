// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.EFCore.UnitTests;

/// <summary>
/// Test customer ID primitive for EF Core converter unit tests.
/// </summary>
[StrongId<Guid>]
public readonly partial record struct EFCoreTestCustomerId;
