// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives.Validation;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Defines a domain primitive wrapping a single backing value of type <typeparamref name="TValue"/>.
/// </summary>
/// <remarks>
/// <para>
/// Provides the standard factory methods (<see cref="Create"/> and <see cref="TryCreate(TValue, out TSelf, out PrimitiveError)"/>)
/// for creating valid instances. Direct construction is blocked by the source generator,
/// which generates a private constructor.
/// </para>
/// <para>
/// <see cref="Create"/> throws on invalid input.
/// <see cref="TryCreate(TValue, out TSelf, out PrimitiveError)"/> provides exception-free validation.
/// </para>
/// </remarks>
/// <typeparam name="TSelf">The concrete domain primitive type (CRTP pattern).</typeparam>
/// <typeparam name="TValue">The underlying primitive type (e.g., <see cref="Guid"/>, <see cref="string"/>, <see cref="decimal"/>).</typeparam>
public interface IDomainPrimitive<TSelf, TValue> : IDomainPrimitive<TSelf>
    where TSelf : IDomainPrimitive<TSelf, TValue>
    where TValue : notnull
{
    /// <summary>Gets the validated, normalized underlying value wrapped by this domain primitive.</summary>
    TValue Value { get; }

#if NET7_0_OR_GREATER
    /// <summary>
    /// Creates a new, validated instance from the specified raw value,
    /// applying normalization before validation.
    /// </summary>
    /// <param name="value">The raw value to normalize and validate.</param>
    /// <returns>A valid, normalized domain primitive instance.</returns>
    static abstract TSelf Create(TValue value);

    /// <summary>
    /// Attempts to create a new, validated instance from the specified raw value
    /// without throwing on failure.
    /// </summary>
    /// <param name="value">The raw value to normalize and validate.</param>
    /// <param name="result">When this method returns <see langword="true"/>, contains the valid primitive instance; otherwise, the default value for <typeparamref name="TSelf"/>.</param>
    /// <param name="validationError">When this method returns <see langword="false"/>, contains the first validation error that occurred.</param>
    /// <returns><see langword="true"/> if the instance was created successfully; otherwise, <see langword="false"/>.</returns>
    static abstract bool TryCreate(TValue value, out TSelf result, out global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError validationError);
#endif
}
