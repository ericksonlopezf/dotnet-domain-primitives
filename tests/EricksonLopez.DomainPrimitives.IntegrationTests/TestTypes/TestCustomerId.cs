// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.IntegrationTests;

/// <summary>
/// Test customer ID primitive for integration tests.
/// </summary>
[StrongId<Guid>]
public readonly partial record struct TestCustomerId;
