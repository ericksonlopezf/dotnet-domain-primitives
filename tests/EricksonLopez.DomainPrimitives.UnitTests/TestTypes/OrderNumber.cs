// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.UnitTests.TestTypes;

/// <summary>
/// Test strong ID backed by int.
/// </summary>
[StrongId<int>]
public readonly partial record struct OrderNumber;
