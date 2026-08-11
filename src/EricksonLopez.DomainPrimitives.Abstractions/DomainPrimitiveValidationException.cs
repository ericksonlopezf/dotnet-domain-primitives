using System;
using EricksonLopez.DomainPrimitives.Validation;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Exception thrown when a domain primitive cannot be created due to invalid input.
/// </summary>
public sealed class DomainPrimitiveValidationException : ArgumentException
{
    /// <summary>
    /// The structured validation error that caused the exception.
    /// </summary>
    public PrimitiveError Error { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="DomainPrimitiveValidationException"/>.
    /// </summary>
    /// <param name="error">The validation error containing code and message.</param>
    /// <param name="paramName">The name of the parameter that caused the exception.</param>
    public DomainPrimitiveValidationException(PrimitiveError error, string paramName = "value")
        : base($"[{error.Code}] {error.Message}", paramName)
    {
        Error = error;
    }
}
