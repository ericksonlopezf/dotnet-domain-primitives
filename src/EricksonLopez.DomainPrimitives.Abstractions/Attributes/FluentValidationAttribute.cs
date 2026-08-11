using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Legacy attribute for FluentValidation integration.
/// </summary>
[Obsolete("The DomainPrimitives.FluentValidation package was removed. The FluentValidation integration is no longer needed. This attribute is a no-op and will be removed in v3.0.", false)]
[AttributeUsage(AttributeTargets.Struct)]
public sealed class FluentValidationAttribute : Attribute
{
}
