// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.UnitTests.TestTypes;

/// <summary>
/// Measure with addition and scalar math.
/// </summary>
[NumericPrimitive<double>(Operations = EricksonLopez.DomainPrimitives.NumericOperations.Addition | EricksonLopez.DomainPrimitives.NumericOperations.ScalarMultiplication | EricksonLopez.DomainPrimitives.NumericOperations.ScalarDivision)]
[PrimitiveRange(0, 1000)]
public readonly partial record struct Distance;
