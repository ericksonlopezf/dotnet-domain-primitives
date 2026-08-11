using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Legacy attribute for JSON serialization generation.
/// </summary>
[Obsolete("The DomainPrimitives.Json package was removed. The core generators now automatically emit System.Text.Json converters. This attribute is a no-op and will be removed in v3.0.", false)]
[AttributeUsage(AttributeTargets.Struct)]
public sealed class JsonAttribute : Attribute
{
}
