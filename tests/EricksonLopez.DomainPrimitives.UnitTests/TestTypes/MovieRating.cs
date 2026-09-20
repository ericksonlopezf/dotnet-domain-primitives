// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.UnitTests.TestTypes;

/// <summary>
/// Rating with custom scale.
/// </summary>
[Rating(Min = 0, Max = 5, Scale = 2)]
public readonly partial record struct MovieRating;
