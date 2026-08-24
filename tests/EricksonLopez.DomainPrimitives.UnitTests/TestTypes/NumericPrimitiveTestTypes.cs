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

/// <summary>
/// Measure with addition and scalar math.
/// </summary>
[NumericPrimitive<double>(Operations = EricksonLopez.DomainPrimitives.NumericOperations.Addition | EricksonLopez.DomainPrimitives.NumericOperations.ScalarMultiplication | EricksonLopez.DomainPrimitives.NumericOperations.ScalarDivision)]
[PrimitiveRange(0, 1000)] // Bounded for range testing
public readonly partial record struct Distance;

/// <summary>
/// Money domain shortcut.
/// </summary>
[Money(Min = 0)]
public readonly partial record struct Price;

/// <summary>
/// Percentage domain shortcut.
/// </summary>
[Percentage] // Default Range(0, 100)
public readonly partial record struct CompletionRate;

/// <summary>
/// Rating with custom scale.
/// </summary>
[Rating(Min = 0, Max = 5, Scale = 2)]
public readonly partial record struct MovieRating;

/// <summary>
/// Score using non-ambiguous PrimitiveRange attribute.
/// </summary>
[NumericPrimitive<double>]
[PrimitiveRange(1, 10)]
public readonly partial record struct PrimitiveRangeScore;


