// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.EndToEndTests;

/// <summary>
/// Test account ID primitive for end-to-end tests.
/// </summary>
[StrongId<Guid>]
public readonly partial record struct AccountId;
