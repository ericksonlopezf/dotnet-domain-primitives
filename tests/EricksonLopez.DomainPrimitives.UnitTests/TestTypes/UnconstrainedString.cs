// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.UnitTests.TestTypes;

/// <summary>
/// Unconstrained string primitive with no explicit MaxLength to test the default 4096 security ceiling (SEC-001).
/// </summary>
#pragma warning disable DP0009
[StringPrimitive]
public readonly partial record struct UnconstrainedString;
#pragma warning restore DP0009
