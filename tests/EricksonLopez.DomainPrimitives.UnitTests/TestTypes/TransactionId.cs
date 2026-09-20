// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.UnitTests.TestTypes;

/// <summary>
/// Test strong ID backed by long.
/// </summary>
[StrongId<long>]
public readonly partial record struct TransactionId;
