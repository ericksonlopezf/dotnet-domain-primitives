// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives.Generators.Models;

/// <summary>
/// Information about a property on a value object.
/// </summary>
internal sealed record ValueObjectPropertyInfo(string Name, string TypeName, string CamelCaseName);
