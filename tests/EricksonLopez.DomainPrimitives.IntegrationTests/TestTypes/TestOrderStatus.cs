// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.IntegrationTests;

/// <summary>
/// Test order status smart enum for integration tests.
/// </summary>
[SmartEnum<int>]
public readonly partial record struct TestOrderStatus
{
    public static readonly TestOrderStatus Pending = new(1, "Pending");
    public static readonly TestOrderStatus Shipped = new(2, "Shipped");
    public static readonly TestOrderStatus Delivered = new(3, "Delivered");
}
