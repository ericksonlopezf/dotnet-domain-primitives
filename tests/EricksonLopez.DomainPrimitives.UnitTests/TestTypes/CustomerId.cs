// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.UnitTests.TestTypes;

/// <summary>
/// Test strong ID backed by Guid.
/// </summary>
[StrongId<Guid>]
public readonly partial record struct CustomerId;
