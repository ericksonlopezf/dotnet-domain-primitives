// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.Dapper.Tests;

/// <summary>
/// Test email address primitive for Dapper type handler tests.
/// </summary>
[StringPrimitive]
public readonly partial record struct EmailAddress;
