using System;
using System.ComponentModel;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Exception thrown when a string cannot be parsed into a domain primitive,
/// typically thrown by the <c>Parse</c> method.
/// </summary>
/// <remarks>
/// <para>
/// <strong>DEPRECATED:</strong> This exception type is deprecated. The <c>Parse()</c> method now throws
/// <see cref="System.FormatException"/> directly, which aligns with BCL conventions
/// (<c>int.Parse()</c>, <c>Guid.Parse()</c>, etc.).
/// </para>
/// <para>
/// <strong>Migration:</strong> Replace <c>catch (DomainPrimitiveFormatException)</c> with
/// <c>catch (FormatException)</c>. See BREAKING_CHANGES.md for full migration guidance.
/// </para>
/// <para>
/// <strong>Removal:</strong> This type will be removed in v3.0. It will be marked as an error
/// in v2.0. Update your catch clauses before upgrading.
/// </para>
/// </remarks>
[Obsolete(
    "DomainPrimitiveFormatException is deprecated. Parse() now throws System.FormatException " +
    "for BCL consistency (like int.Parse, Guid.Parse). Replace catch(DomainPrimitiveFormatException) " +
    "with catch(FormatException). Will be removed in v3.0. See BREAKING_CHANGES.md for migration guide.",
    error: false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class DomainPrimitiveFormatException : FormatException
{
    /// <summary>
    /// The name of the domain primitive type that failed validation.
    /// </summary>
    public string PrimitiveName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainPrimitiveFormatException"/> class with a specified error message.
    /// </summary>
    /// <param name="primitiveName">The name of the primitive that caused the error.</param>
    /// <param name="message">The message that describes the error.</param>
    public DomainPrimitiveFormatException(string primitiveName, string message)
        : base(message)
    {
        PrimitiveName = primitiveName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainPrimitiveFormatException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="primitiveName">The name of the primitive that caused the error.</param>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public DomainPrimitiveFormatException(string primitiveName, string message, Exception innerException)
        : base(message, innerException)
    {
        PrimitiveName = primitiveName;
    }
}
