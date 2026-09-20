// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.UnitTests.TestTypes;

/// <summary>
/// Test registration timestamp primitive.
/// </summary>
[DatePrimitive(Kind = DatePrimitiveKind.DateTime, PastOnly = true)]
public readonly partial record struct RegistrationTimestamp;
