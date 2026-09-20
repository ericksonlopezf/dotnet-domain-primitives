// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.UnitTests.TestTypes;

/// <summary>
/// Password hash — sensitive type. Error messages must not include the rejected value (SEC-005).
/// </summary>
[PasswordHash]
public readonly partial record struct PasswordHashValue;
