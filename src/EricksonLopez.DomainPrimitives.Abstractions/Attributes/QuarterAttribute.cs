// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that a struct is a quarter domain primitive.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class QuarterAttribute : Attribute
{
}
