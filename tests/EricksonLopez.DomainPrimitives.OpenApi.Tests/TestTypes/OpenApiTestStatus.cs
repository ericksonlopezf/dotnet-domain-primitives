// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.OpenApi.Tests;

/// <summary>
/// Test status smart enum for OpenAPI schema filter tests.
/// </summary>
[SmartEnum<int>]
public readonly partial record struct OpenApiTestStatus
{
    public static readonly OpenApiTestStatus Active = new(1, "Active");
    public static readonly OpenApiTestStatus Inactive = new(2, "Inactive");
}
