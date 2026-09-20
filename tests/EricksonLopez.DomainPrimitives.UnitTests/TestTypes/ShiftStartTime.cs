// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.UnitTests.TestTypes;

/// <summary>
/// Test shift start time primitive.
/// </summary>
[DatePrimitive(Kind = DatePrimitiveKind.TimeOnly, FutureOnly = true)]
public readonly partial record struct ShiftStartTime;
