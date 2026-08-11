using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Marks a <c>readonly partial record struct</c> as a composite value object
/// with structural equality across all properties.
/// </summary>
/// <remarks>
/// <para>
/// Unlike single-value domain primitives, a value object can have multiple properties.
/// The source generator produces zero-boxing equality, <c>GetHashCode</c>, and formatting
/// based on all public properties.
/// </para>
/// <para>
/// All properties must use <c>required</c> and <c>init</c> to ensure immutability
/// and complete initialization.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// [ValueObject]
/// public readonly partial record struct Address
/// {
///     public required string Street { get; init; }
///     public required string City { get; init; }
///     public required string State { get; init; }
///     public required string ZipCode { get; init; }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class ValueObjectAttribute : Attribute
{
}
