// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.EndToEndTests;

/// <summary>
/// Test owner birth date primitive for end-to-end tests.
/// </summary>
[BirthDate]
public readonly partial record struct OwnerBirthDate;
