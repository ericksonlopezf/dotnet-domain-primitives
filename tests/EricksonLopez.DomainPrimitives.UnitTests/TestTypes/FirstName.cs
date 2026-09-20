// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.UnitTests.TestTypes;

/// <summary>
/// Basic string primitive with trim + length validation.
/// </summary>
[StringPrimitive]
[Trim]
[MinLength(1)]
[MaxLength(100)]
public readonly partial record struct FirstName;
