using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Defines a custom normalization strategy.
/// </summary>
public interface INormalizer<T>
{
#if NET7_0_OR_GREATER
    /// <summary>
    /// Normalizes the input value.
    /// </summary>
    /// <param name="value">The value to normalize.</param>
    /// <returns>The normalized value.</returns>
    static abstract T Normalize(T value);
#endif
}

/// <summary>
/// Applies a custom normalization strategy to a domain primitive.
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
