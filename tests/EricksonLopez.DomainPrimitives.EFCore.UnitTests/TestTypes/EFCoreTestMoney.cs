// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.EFCore.UnitTests;

/// <summary>
/// Test money primitive for EF Core converter unit tests.
/// </summary>
[Money]
public readonly partial record struct EFCoreTestMoney;
