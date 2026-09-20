// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.OpenApi.Tests;

/// <summary>
/// Test date primitive for OpenAPI schema filter tests.
/// </summary>
[DatePrimitive(Kind = 0)]
public readonly partial record struct OpenApiTestDate;
