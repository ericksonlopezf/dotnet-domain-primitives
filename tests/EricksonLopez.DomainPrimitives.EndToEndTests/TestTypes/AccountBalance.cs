// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.EndToEndTests;

/// <summary>
/// Test account balance primitive for end-to-end tests.
/// </summary>
[Money]
public readonly partial record struct AccountBalance;
