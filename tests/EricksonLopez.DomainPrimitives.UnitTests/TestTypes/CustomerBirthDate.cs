// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.UnitTests.TestTypes;

/// <summary>
/// Test customer birth date primitive.
/// </summary>
[BirthDate(MaxAge = 120)]
public readonly partial record struct CustomerBirthDate;
