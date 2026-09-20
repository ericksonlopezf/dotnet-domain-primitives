// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.UnitTests.TestTypes;

/// <summary>
/// Basic score with constrained range.
/// </summary>
[NumericPrimitive<int>]
[PrimitiveRange(0, 100)]
public readonly partial record struct Score;
