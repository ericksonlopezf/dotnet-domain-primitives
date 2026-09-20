// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.UnitTests.TestTypes;

/// <summary>
/// Test credit card expiration date primitive.
/// </summary>
[ExpirationDate]
public readonly partial record struct CreditCardExpiration;
