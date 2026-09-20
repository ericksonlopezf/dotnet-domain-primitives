// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.UnitTests.TestTypes;

/// <summary>
/// Score using non-ambiguous PrimitiveRange attribute.
/// </summary>
[NumericPrimitive<double>]
[PrimitiveRange(1, 10)]
public readonly partial record struct PrimitiveRangeScore;
