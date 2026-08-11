using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.DomainPrimitives.Validation;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Marker interface for all domain primitives.
/// Enables generic constraints and discovery without introducing runtime overhead.
/// </summary>
/// <remarks>
/// <para>
/// All domain primitives — strong IDs, string primitives, numeric primitives,
/// date primitives, smart enums, and composite value objects — implement this interface.
/// </para>
/// <para>
/// This interface uses the Curiously Recurring Template Pattern (CRTP) to enable
/// <c>static abstract</c> members that return the concrete type.
/// </para>
/// </remarks>
/// <typeparam name="TSelf">The concrete domain primitive type (CRTP pattern).</typeparam>
public interface IDomainPrimitive<TSelf>
    where TSelf : IDomainPrimitive<TSelf>
{
#if NET7_0_OR_GREATER
    /// <summary>Gets the name of this domain primitive type, used in diagnostics, error messages, and observability.</summary>
    /// <example>
    /// <code>
    /// // For a type "CustomerId", this returns "CustomerId"
    /// string name = CustomerId.PrimitiveName; // "CustomerId"
    /// </code>
    /// </example>
    static abstract string PrimitiveName { get; }
#endif
    /// <summary>Gets a value indicating whether this instance was created via <see langword="default"/> rather than a factory method.</summary>
    /// <remarks>
    /// A default instance carries no validated value. Callers should check this property
    /// before accessing <see cref="IDomainPrimitive{TSelf, TValue}.Value"/> to avoid operating on uninitialized data.
    /// </remarks>
    bool IsDefault { get; }
}

/// <summary>
/// A domain primitive wrapping a single backing value of type <typeparamref name="TValue"/>.
/// </summary>
/// <remarks>
/// <para>
/// Provides the standard factory methods (<see cref="Create"/> and <see cref="TryCreate(TValue)"/>)
/// for creating valid instances. Direct construction is blocked by the source generator,
/// which generates a private constructor.
/// </para>
/// <para>
/// <see cref="Create"/> throws <see cref="DomainPrimitiveValidationException"/> on invalid input.
/// <see cref="TryCreate(TValue, out TSelf)"/> provides exception-free validation.
/// </para>
/// </remarks>
/// <seealso cref="IStrongId{TSelf, TValue}"/>
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
    /// <exception cref="DomainPrimitiveValidationException"><paramref name="value"/> fails the domain validation rules.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    static abstract TSelf Create(TValue value);
#endif

#if NET7_0_OR_GREATER
    /// <summary>
    /// Attempts to create a new, validated instance from the specified raw value
    /// without throwing on failure.
    /// </summary>
    /// <param name="value">The raw value to normalize and validate.</param>
    /// <param name="result"
    /// >When this method returns <see langword="true"/>, contains the valid primitive instance;
    /// otherwise, the default value for <typeparamref name="TSelf"/>.</param>
    /// <param name="validationError">When this method returns <see langword="false"/>, contains the first validation error that occurred.</param>
    /// <returns><see langword="true"/> if the instance was created successfully; otherwise, <see langword="false"/>.</returns>
    static abstract bool TryCreate(TValue value, out TSelf result, out global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError validationError);
#endif
}
