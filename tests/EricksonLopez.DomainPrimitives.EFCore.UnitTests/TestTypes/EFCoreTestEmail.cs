// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.EFCore.UnitTests;

/// <summary>
/// Test email primitive for EF Core converter unit tests.
/// </summary>
[Email]
public readonly partial record struct EFCoreTestEmail;
