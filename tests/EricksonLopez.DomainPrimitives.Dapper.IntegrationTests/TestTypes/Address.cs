// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.Dapper.Tests;

/// <summary>
/// Test address value object for Dapper tests.
/// </summary>
[ValueObject]
public readonly partial record struct Address
{
    public string Street { get; init; }
    public string City { get; init; }
}
