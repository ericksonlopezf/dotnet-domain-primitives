// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.UnitTests;

/// <summary>
/// Sample ISO country code for README verification tests.
/// </summary>
[StringPrimitive]
[Trim, UpperCase, Length(2, 2)]
public readonly partial record struct CountryIsoCode;
