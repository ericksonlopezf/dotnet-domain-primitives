// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.EFCore.IntegrationTests;

/// <summary>
/// Test customer email primitive for EF Core integration tests.
/// </summary>
[Email]
public readonly partial record struct CustomerEmail;
