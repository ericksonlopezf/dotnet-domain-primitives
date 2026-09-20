// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives.EndToEndTests;

/// <summary>
/// Sample aggregate for end-to-end tests.
/// </summary>
public sealed record UserAccountAggregate
{
    public AccountId Id { get; init; }
    public AccountEmail Email { get; init; }
    public AccountBalance Balance { get; init; }
    public OwnerBirthDate BirthDate { get; init; }
    public AccountTier Tier { get; init; }
}
