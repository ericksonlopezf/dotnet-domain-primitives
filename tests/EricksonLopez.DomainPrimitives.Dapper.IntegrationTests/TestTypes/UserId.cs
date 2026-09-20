// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.Dapper.Tests;

/// <summary>
/// Test user ID primitive for Dapper type handler tests.
/// </summary>
[StrongId<Guid>]
public readonly partial record struct UserId;
