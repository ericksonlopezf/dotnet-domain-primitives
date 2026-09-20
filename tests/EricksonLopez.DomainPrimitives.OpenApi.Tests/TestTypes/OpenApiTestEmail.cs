// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.OpenApi.Tests;

/// <summary>
/// Test email primitive for OpenAPI schema filter tests.
/// </summary>
[Email]
public readonly partial record struct OpenApiTestEmail;
