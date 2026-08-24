// Copyright © Erickson Lopez. MIT License.

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Defines a contract for a custom normalization strategy.
/// </summary>
/// <typeparam name="T">The type of value to normalize.</typeparam>
public interface INormalizer<T>
{
#if NET7_0_OR_GREATER
    /// <summary>
    /// Normalizes the specified input value.
    /// </summary>
    /// <param name="value">The value to normalize.</param>
    /// <returns>The normalized value resulting from the transformation.</returns>
    static abstract T Normalize(T value);
#endif
}
