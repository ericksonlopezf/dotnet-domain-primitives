// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.UnitTests.TestTypes;

/// <summary>
/// Test global timestamp primitive.
/// </summary>
[DatePrimitive(Kind = DatePrimitiveKind.DateTimeOffset, PastOnly = true)]
public readonly partial record struct GlobalTimestamp;
