// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.IntegrationTests;

/// <summary>
/// Test email primitive for integration tests.
/// </summary>
[Email]
public readonly partial record struct TestEmail;
