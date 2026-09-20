// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.UnitTests.TestTypes;

/// <summary>
/// ISO 3166-1 alpha-2 country code (e.g., "US", "DE").
/// </summary>
[CountryCode]
public readonly partial record struct Country;
