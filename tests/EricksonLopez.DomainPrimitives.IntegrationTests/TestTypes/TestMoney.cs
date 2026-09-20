// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.IntegrationTests;

/// <summary>
/// Test money primitive for integration tests.
/// </summary>
[Money]
public readonly partial record struct TestMoney;
