using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Marks a <c>readonly partial record struct</c> as a strongly-typed identifier
/// wrapping <typeparamref name="TValue"/>.
/// </summary>
/// <remarks>
/// <para>
/// The source generator produces a complete strong ID implementation including:
/// factory methods (<c>Create()</c>, <c>TryCreate()</c>, <c>Empty</c>),
/// parsing (<see cref="IParsable{TSelf}"/>, <see cref="ISpanParsable{TSelf}"/>, <see cref="IUtf8SpanParsable{TSelf}"/>),
/// formatting (<see cref="IFormattable"/>, <see cref="ISpanFormattable"/>, <see cref="IUtf8SpanFormattable"/>),
/// comparison, and explicit operators.
/// </para>
/// <para>
/// Supported backing types: <see cref="Guid"/>, <see cref="int"/>, <see cref="long"/>,
/// <see cref="string"/>.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// [StrongId&lt;Guid&gt;]
/// public readonly partial record struct CustomerId;
///
/// // Usage:
/// var id = CustomerId.Create();
/// var parsed = CustomerId.Parse("a1b2c3d4-...");
/// Guid raw = (Guid)id;
/// </code>
/// </example>
/// <typeparam name="TValue">The backing identity type.</typeparam>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class StrongIdAttribute<TValue> : Attribute
    where TValue : notnull
{
    /// <summary>
    /// Gets or sets a value indicating whether empty backing values are rejected.
    /// When <see langword="true"/>, <c>Create()</c> throws and <c>TryCreate()</c> returns <see langword="false"/> for
    /// empty values (e.g., <see cref="System.Guid.Empty"/>, <c>0</c>, or <c>""</c>).
    /// Defaults to <see langword="true"/>.
    /// </summary>
    public bool RejectEmpty { get; set; } = true;
}
