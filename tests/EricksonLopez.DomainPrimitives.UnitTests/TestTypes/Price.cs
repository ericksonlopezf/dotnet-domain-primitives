// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.UnitTests.TestTypes;

/// <summary>
/// Money domain shortcut.
/// </summary>
[Money(Min = 0)]
public readonly partial record struct Price;
