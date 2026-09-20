// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.OpenApi.Tests;

/// <summary>
/// Test order ID primitive for OpenAPI schema filter tests.
/// </summary>
[StrongId<int>]
public readonly partial record struct OpenApiTestOrderId;
