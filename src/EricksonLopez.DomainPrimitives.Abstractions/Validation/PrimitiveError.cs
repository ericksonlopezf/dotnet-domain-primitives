// Copyright © Erickson Lopez. MIT License.
#nullable enable

namespace EricksonLopez.DomainPrimitives.Validation;

/// <summary>
/// Represents a single validation error from domain primitive validation.
/// A <see cref="PrimitiveError"/> is a zero-allocation value type — no heap allocation occurs
/// on either the error path or the success path (<see cref="None"/>).
/// </summary>
/// <remarks>
/// The <see cref="None"/> sentinel is the default value of this struct, where both
/// <see cref="Code"/> and <see cref="Message"/> are <c>null</c>. The <see cref="IsError"/>
/// property correctly identifies this state by checking <c>Code is not null</c>.
/// </remarks>
/// <param name="Code">
/// The error code identifying the failure category (e.g., "LENGTH", "FORMAT", "EMPTY", "NULL_INPUT").
/// <c>null</c> only for the <see cref="None"/> sentinel — never set this explicitly.
/// </param>
/// <param name="Message">
/// A human-readable error message describing the validation failure.
/// <c>null</c> only for the <see cref="None"/> sentinel — never set this explicitly.
/// </param>
public readonly record struct PrimitiveError(string? Code, string? Message)
{
    /// <summary>
    /// Represents no error (a successful validation result).
    /// This is the default value of <see cref="PrimitiveError"/> where both <see cref="Code"/>
    /// and <see cref="Message"/> are <c>null</c>.
    /// </summary>
    public static PrimitiveError None => default;

    /// <summary>
    /// Gets a value indicating whether this instance represents a validation error.
    /// Returns <c>false</c> for <see cref="None"/>.
    /// </summary>
    public bool IsError => Code is not null;

    /// <summary>
    /// Creates a new <see cref="PrimitiveError"/> with the given code and message.
    /// </summary>
    /// <param name="code">The error code (e.g., "LENGTH", "FORMAT"). Must not be null.</param>
    /// <param name="message">The human-readable error message. Must not be null.</param>
    /// <returns>A new error instance.</returns>
    public static PrimitiveError Create(string code, string message) =>
        new PrimitiveError(code, message);
}




