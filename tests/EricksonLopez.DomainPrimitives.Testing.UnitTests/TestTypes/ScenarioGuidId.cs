// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.Testing.UnitTests;

/// <summary>
/// Test Guid strong ID primitive for scenario testing suite.
/// </summary>
[StrongId<Guid>]
public readonly partial record struct ScenarioGuidId;
