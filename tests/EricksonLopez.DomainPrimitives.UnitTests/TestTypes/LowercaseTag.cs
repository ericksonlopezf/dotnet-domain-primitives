// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.UnitTests.TestTypes;

/// <summary>
/// Lowercase email for NFC normalization tests.
/// </summary>
[StringPrimitive]
[Trim]
[LowerCase]
[MinLength(1)]
[MaxLength(254)]
public readonly partial record struct LowercaseTag;
