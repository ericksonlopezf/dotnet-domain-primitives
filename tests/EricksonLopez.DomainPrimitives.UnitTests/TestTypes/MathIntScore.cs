// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.UnitTests.TestTypes;

/// <summary>
/// Integer primitive with addition, scalar multiplication and negation enabled.
/// </summary>
[NumericPrimitive<int>(Operations = NumericOperations.Addition | NumericOperations.ScalarMultiplication | NumericOperations.Negation)]
[PrimitiveRange(int.MinValue, int.MaxValue)]
public readonly partial record struct MathIntScore;
