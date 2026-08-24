// Copyright © Erickson Lopez. MIT License.
using EricksonLopez.DomainPrimitives.Validation;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Defines the contract for strongly-typed identifiers.
/// </summary>
/// <remarks>
/// <para>
/// Strong IDs provide type-safe identity wrappers that prevent accidental
/// interchange of identifiers (e.g., passing a <c>ProductId</c> where a
/// <c>CustomerId</c> is expected).
/// </para>
/// <para>
/// Unlike other domain primitives, strong IDs have no validation beyond
/// the underlying type constraints. They provide:
/// </para>
/// <list type="bullet">
///   <item><see cref="Create()"/> — creates a new unique identifier</item>
///   <item><see cref="Empty"/> — an uninitialized/empty sentinel</item>
/// </list>
/// </remarks>
/// <typeparam name="TSelf">The concrete strong ID type (CRTP pattern).</typeparam>
/// <typeparam name="TValue">The backing identity type (e.g., <see cref="Guid"/>, <see cref="int"/>, <see cref="long"/>).</typeparam>
public interface IStrongId<TSelf, TValue> : IDomainPrimitive<TSelf, TValue>
    where TSelf : IStrongId<TSelf, TValue>
    where TValue : notnull
{
#if NET7_0_OR_GREATER
    /// <summary>
    /// Creates a new, unique identifier.
    /// </summary>
    /// <remarks>
    /// For <see cref="Guid"/>-backed IDs, this calls <see cref="Guid.NewGuid"/>.
    /// For integer-backed IDs, this is not supported and throws <see cref="NotSupportedException"/>.
    /// </remarks>
    /// <returns>A new strong ID wrapping a freshly generated value.</returns>
    static abstract TSelf Create();
#endif

#if NET7_0_OR_GREATER
    /// <summary>
    /// Gets the empty or uninitialized sentinel value for this identifier type.
    /// </summary>
    /// <remarks>
    /// Equivalent to <c>default</c> for the backing type, but explicitly named
    /// for discoverability and intent.
    /// </remarks>
    static abstract TSelf Empty { get; }
#endif
}


