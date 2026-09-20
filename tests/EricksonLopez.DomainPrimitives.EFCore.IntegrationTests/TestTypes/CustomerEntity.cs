// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives.EFCore.IntegrationTests;

/// <summary>
/// Entity for EF Core integration tests.
/// </summary>
public sealed class CustomerEntity
{
    public CustomerId Id { get; set; }
    public CustomerEmail Email { get; set; }
    public Balance AccountBalance { get; set; }
    public LoyaltyPoints Points { get; set; }
    public MembershipTier Tier { get; set; }
}
