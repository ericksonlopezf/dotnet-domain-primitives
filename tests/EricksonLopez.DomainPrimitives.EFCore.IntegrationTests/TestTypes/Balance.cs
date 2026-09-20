// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.EFCore.IntegrationTests;

/// <summary>
/// Test balance primitive for EF Core integration tests.
/// </summary>
[Money]
public readonly partial record struct Balance;
