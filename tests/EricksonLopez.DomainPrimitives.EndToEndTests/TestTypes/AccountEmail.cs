// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.EndToEndTests;

/// <summary>
/// Test account email primitive for end-to-end tests.
/// </summary>
[Email]
public readonly partial record struct AccountEmail;
