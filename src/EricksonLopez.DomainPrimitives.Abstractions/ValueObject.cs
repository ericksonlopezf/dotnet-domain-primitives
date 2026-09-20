// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;


/// <summary>
/// Base record class for complex multi-property Value Objects — types defined by their structural equality.
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
/// <para>
/// <b>When to use <c>ValueObject</c> vs source-generator attributes:</b>
/// <list type="table">
///   <listheader><term>Use case</term><term>Recommended approach</term></listheader>
///   <item>
///     <term>Single-wrapping primitive (string, int, Guid, decimal, DateOnly...)</term>
///     <term>Source-generator attributes: <c>[StringPrimitive]</c>, <c>[Money]</c>, <c>[StrongId]</c>, etc.</term>
///   </item>
///   <item>
///     <term>Complex multi-property value object (e.g., Money + Currency, Address)</term>
///     <term>Inherit from <c>ValueObject</c> as a <c>sealed record</c></term>
///   </item>
/// </list>
/// </para>
/// <para>
/// <b>Important</b>: The source-generated primitives (e.g., <c>[Money]</c> generates a <c>readonly record struct Money</c>)
/// are <b>independent value types</b> and do <b>not</b> inherit from <c>ValueObject</c>.
/// <c>ValueObject</c> is a heap-allocated <c>record class</c>, while generated primitives are stack-allocated <c>record struct</c>s.
/// </para>
/// <code>
/// // ✅ Correct — complex multi-property value object (use ValueObject base)
/// public sealed record Money(decimal Amount, string Currency) : ValueObject;
///
/// // ✅ Correct — single-wrapping primitive (use source generator attribute)
/// [Money]
/// public partial struct ProductPrice;  // generates: readonly partial record struct ProductPrice
/// </code>
/// </remarks>
public abstract record class ValueObject;


