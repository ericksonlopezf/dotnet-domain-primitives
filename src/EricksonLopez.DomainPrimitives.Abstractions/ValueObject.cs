using System;
namespace EricksonLopez.DomainPrimitives;


/// <summary>
/// Represents the base record for Value Objects — types defined by their structural equality.
/// </summary>
/// <remarks>
/// <para>
/// A value object has no identity — two value objects are equal if all their
/// components are equal. Value objects <b>must be immutable</b>.
/// </para>
/// <para>
/// By inheriting from this <c>record class</c>, the compiler automatically generates 
/// <see cref="IEquatable{T}.Equals(T)"/> and <see cref="object.GetHashCode"/> 
/// that provide value equality over all properties and fields, with zero boxing allocations.
/// </para>
/// <code>
/// // ✅ Correct — immutable value object
/// public sealed record Money(decimal Amount, string Currency) : ValueObject;
/// </code>
/// </remarks>
public abstract record class ValueObject;

