// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies that a struct is an International Standard Book Number (ISBN) domain primitive.
/// Equivalent to: <c>[StringPrimitive] [Trim] [Regex(isbn-pattern)]</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class ISBNAttribute : Attribute
{
}
