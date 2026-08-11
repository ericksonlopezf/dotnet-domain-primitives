using System;

namespace EricksonLopez.DomainPrimitives.Diagnostics;

/// <summary>Event arguments describing a domain primitive validation failure.</summary>
/// <param name="PrimitiveName">The name of the domain primitive type that failed validation (e.g., <c>"EmailAddress"</c>).</param>
/// <param name="ErrorType">The error code identifying the failure category (e.g., <c>"FORMAT"</c>, <c>"LENGTH"</c>).</param>
/// <param name="ErrorMessage">The human-readable description of the validation failure.</param>
public readonly record struct ValidationFailureEventArgs(string PrimitiveName, string ErrorType, string ErrorMessage);

/// <summary>
/// Provides a static event source for consuming domain primitive validation events
/// without requiring Dependency Injection.
/// </summary>
/// <remarks>
/// Subscribe to <see cref="OnValidationFailed"/> to route validation failure events to any sink
/// (e.g., <c>ILogger</c>, metrics, structured logging) without coupling domain primitives to DI infrastructure.
/// Moved from <c>EricksonLopez.DomainPrimitives.Abstractions</c> to <c>EricksonLopez.DomainPrimitives</c> in v1.2.0.
/// See BREAKING_CHANGES.md for migration details.
/// </remarks>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static class DomainPrimitiveEventSource
{
    /// <summary>
    /// Occurs when a domain primitive fails validation.
    /// </summary>
    /// <remarks>
    /// Invoked from the validation pipeline. Subscribe once during application startup
    /// to forward events to <c>ILogger</c>, metrics, or other observability sinks.
    /// The event sender is always <see langword="null"/>.
    /// </remarks>
    public static event EventHandler<ValidationFailureEventArgs>? OnValidationFailed;

    internal static void NotifyValidationFailed(string primitiveName, string errorType, string errorMessage)
    {
        OnValidationFailed?.Invoke(null, new ValidationFailureEventArgs(primitiveName, errorType, errorMessage));
    }
}
