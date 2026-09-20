// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.UnitTests.TestTypes;

/// <summary>
/// API secret token — sensitive type. Error messages must not include the rejected value (SEC-005).
/// </summary>
[StringPrimitive]
[NotEmpty]
[MinLength(32)]
public readonly partial record struct ApiSecret;
