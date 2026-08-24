// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives.Diagnostics;

/// <summary>
/// Provides a static event source for consuming domain primitive validation events
/// without requiring Dependency Injection.
/// </summary>
/// <remarks>
/// Subscribe to <see cref="OnValidationFailed"/> to route validation failure events to any sink
/// (e.g., <c>ILogger</c>, metrics, structured logging) without coupling domain primitives to DI infrastructure.
/// Provided by <c>EricksonLopez.DomainPrimitives</c> (Core package) since v1.0.0.
/// </remarks>
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
