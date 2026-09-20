// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.UnitTests.TestTypes;

/// <summary>
/// Email address using the [Email] domain shortcut.
/// </summary>
[Email]
public readonly partial record struct EmailAddress;
