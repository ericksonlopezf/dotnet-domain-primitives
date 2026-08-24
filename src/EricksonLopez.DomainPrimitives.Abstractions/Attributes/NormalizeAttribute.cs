// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies a custom normalization strategy to apply to a domain primitive.
/// </summary>
/// <remarks>
/// The normalizer is invoked before validation.
/// Multiple <c>[Normalize]</c> attributes can be stacked.
/// </remarks>
/// <example>
/// <code>
/// [StringPrimitive]
/// [Normalize&lt;RemoveSpacesNormalizer&gt;]
/// public readonly partial record struct Code;
/// </code>
/// </example>
/// <typeparam name="TNormalizer">
/// The normalizer type implementing <see cref="INormalizer{T}"/>.
/// </typeparam>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
public sealed class NormalizeAttribute<TNormalizer> : Attribute
{
}
