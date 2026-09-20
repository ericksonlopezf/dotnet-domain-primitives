// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.EFCore.UnitTests;

/// <summary>
/// Test score primitive for EF Core converter unit tests.
/// </summary>
[NumericPrimitive<int>]
public readonly partial record struct EFCoreTestScore;
