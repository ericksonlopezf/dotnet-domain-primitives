using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Marks a <c>readonly partial record struct</c> as a smart enum with a
/// <typeparamref name="TValue"/> backing value.
/// </summary>
/// <remarks>
/// <para>
/// Smart enums are type-safe, behavior-rich alternatives to C# <c>enum</c> types.
/// They support metadata, exhaustive matching, and parse-from-name/value operations.
/// </para>
/// <para>
/// Define instances as <c>public static readonly</c> fields within the type.
/// The source generator discovers them and generates the required infrastructure.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// [SmartEnum&lt;int&gt;]
/// public readonly partial record struct OrderStatus
/// {
///     public static readonly OrderStatus Pending = new(0, "Pending");
///     public static readonly OrderStatus Shipped = new(1, "Shipped");
///     public static readonly OrderStatus Delivered = new(2, "Delivered");
/// }
/// </code>
/// </example>
/// <typeparam name="TValue">The backing value type for enum members (e.g., <see cref="int"/>, <see cref="string"/>).</typeparam>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class SmartEnumAttribute<TValue> : Attribute
    where TValue : notnull
{
}
