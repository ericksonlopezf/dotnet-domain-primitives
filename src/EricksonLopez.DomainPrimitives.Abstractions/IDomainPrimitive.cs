// Copyright © Erickson Lopez. MIT License.

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Defines the contract for all domain primitive types.
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
    /// before accessing value properties to avoid operating on uninitialized data.
    /// </remarks>
    bool IsDefault { get; }
}
