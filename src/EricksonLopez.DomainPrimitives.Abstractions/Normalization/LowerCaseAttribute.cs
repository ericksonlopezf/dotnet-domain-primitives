// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that a string domain primitive should be converted to lower case before validation.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public sealed class LowerCaseAttribute : Attribute
{
}
