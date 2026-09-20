// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.OpenApi.Tests;

/// <summary>
/// Test user ID primitive for OpenAPI schema filter tests.
/// </summary>
[StrongId<Guid>]
public readonly partial record struct OpenApiTestUserId;
