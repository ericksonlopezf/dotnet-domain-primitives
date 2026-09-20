// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.Testing.UnitTests;

/// <summary>
/// Test strict slug primitive for scenario testing suite.
/// </summary>
[StringPrimitive]
[Trim]
[Regex(@"^[a-z0-9]+(?:-[a-z0-9]+)*$")]
public readonly partial record struct ScenarioStrictSlug;
