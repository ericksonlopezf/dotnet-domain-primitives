// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Specifies global default policies for domain primitives generated in this assembly.
/// </summary>
/// <remarks>
/// Per-type attributes take precedence over global assembly defaults.
/// </remarks>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
public sealed class DomainPrimitivesDefaultsAttribute : Attribute
{
    /// <summary>
    /// Gets or sets a value indicating whether string primitives should automatically trim whitespace by default.
    /// </summary>
    public bool Trim { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether string primitives should automatically enforce not-empty validation by default.
    /// </summary>
    public bool NotEmpty { get; set; }

    /// <summary>
    /// Gets or sets a default maximum length for string primitives when no explicit length is specified.
    /// Default is 4096 (security limit).
    /// </summary>
    public int MaxLength { get; set; } = 4096;

    /// <summary>
    /// Gets or sets a custom exception type to throw on validation failures in <c>Create()</c>.
    /// The exception type must inherit from <see cref="Exception"/> and provide a constructor accepting a string message.
    /// If not specified, <see cref="DomainPrimitiveValidationException"/> is thrown.
    /// </summary>
    public Type? ExceptionType { get; set; }
}
