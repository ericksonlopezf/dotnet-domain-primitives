// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.UnitTests.TestTypes;

/// <summary>
/// String primitive with regex validation.
/// </summary>
[StringPrimitive]
[Trim]
[Regex(@"^[A-Z]{2}-\d{4}$", ErrorMessage = "Must be in format XX-0000")]
public readonly partial record struct ProductCode;
