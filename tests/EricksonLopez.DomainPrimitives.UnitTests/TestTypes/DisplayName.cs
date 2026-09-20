// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.UnitTests.TestTypes;

/// <summary>
/// String primitive with multiple normalizations (trim + uppercase + whitespace).
/// </summary>
[StringPrimitive]
[Trim]
[UpperCase]
[NormalizeWhitespace]
[NotEmpty]
public readonly partial record struct DisplayName;
