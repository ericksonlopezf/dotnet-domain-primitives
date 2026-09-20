// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.Dapper.Tests;

/// <summary>
/// Test points primitive for Dapper type handler tests.
/// </summary>
[NumericPrimitive<int>]
public readonly partial record struct Points;
