// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.UnitTests.TestTypes;

/// <summary>
/// Test strong ID backed by string with required length constraints.
/// A Sku without bounds would be flagged by DP0009 (MED-001).
/// </summary>
[StrongId<string>]
[MinLength(1), MaxLength(50)]
public readonly partial record struct Sku;
