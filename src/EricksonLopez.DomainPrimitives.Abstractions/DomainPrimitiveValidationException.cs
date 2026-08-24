// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives.Validation;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Represents the exception thrown when a domain primitive cannot be created due to invalid input.
/// </summary>
/// <remarks>
/// <para>
/// Inherits from <see cref="ArgumentException"/> so callers can catch it with either
/// <c>catch (DomainPrimitiveValidationException)</c> or <c>catch (ArgumentException)</c>.
/// </para>
/// <para>
/// This exception is thrown by the generated <c>Create(TValue)</c> factory method.
/// For non-throwing creation, use the generated <c>TryCreate(TValue, out TSelf, out PrimitiveError)</c> overload instead.
/// </para>
/// </remarks>
public sealed class DomainPrimitiveValidationException : ArgumentException
{
    /// <summary>
    /// Gets the structured validation error that caused the exception.
    /// </summary>
    /// <remarks>
    /// Use this property to access the machine-readable error code and message
    /// without parsing the exception message string.
    /// </remarks>
    public PrimitiveError Error { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainPrimitiveValidationException"/> class
    /// with a structured validation error and an optional parameter name.
    /// </summary>
    /// <param name="error">The structured validation error containing the error code and message.</param>
    /// <param name="paramName">The name of the parameter that caused the exception. Defaults to <c>"value"</c>.</param>
    public DomainPrimitiveValidationException(PrimitiveError error, string paramName = "value")
        : base($"[{error.Code}] {error.Message}", paramName)
    {
        Error = error;
    }
}
