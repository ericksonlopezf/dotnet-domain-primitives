// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.EFCore.IntegrationTests;

/// <summary>
/// Test customer ID primitive for EF Core integration tests.
/// </summary>
[StrongId<Guid>]
public readonly partial record struct CustomerId;
