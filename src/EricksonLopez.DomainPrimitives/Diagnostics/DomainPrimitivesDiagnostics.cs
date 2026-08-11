using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Diagnostics.Metrics;
#if NET5_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace EricksonLopez.DomainPrimitives.Diagnostics;

/// <summary>
/// Provides <see cref="DiagnosticListener"/> integration and OpenTelemetry metrics for domain primitives.
/// </summary>
/// <remarks>
/// Moved from <c>EricksonLopez.DomainPrimitives.Abstractions</c> to <c>EricksonLopez.DomainPrimitives</c> in v1.2.0.
/// Abstractions should contain only: attributes, marker interfaces, and <c>PrimitiveError</c>.
/// See BREAKING_CHANGES.md for migration details.
/// </remarks>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static class DomainPrimitivesDiagnostics
{
    /// <summary>The name of the <see cref="DiagnosticListener"/> used for events.</summary>
    public static readonly string ListenerName = "EricksonLopez.DomainPrimitives";
    
    private static readonly DiagnosticListener Source = new(ListenerName);
    
    /// <summary>The <see cref="Meter"/> used for domain primitive metrics.</summary>
    public static readonly Meter Meter = new("EricksonLopez.DomainPrimitives", "1.0.0");

    /// <summary>Payload carried by a validation failure diagnostic event.</summary>
    /// <param name="PrimitiveName">The name of the domain primitive type that failed validation (e.g., <c>"EmailAddress"</c>).</param>
    /// <param name="ErrorType">The error code identifying the failure category (e.g., <c>"FORMAT"</c>, <c>"LENGTH"</c>).</param>
    /// <param name="ErrorMessage">The human-readable description of the validation failure.</param>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public readonly record struct ValidationFailurePayload(string PrimitiveName, string ErrorType, string ErrorMessage);

    /// <summary>Payload carried by a validation success diagnostic event.</summary>
    /// <param name="PrimitiveName">The name of the domain primitive type that was successfully validated (e.g., <c>"EmailAddress"</c>).</param>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public readonly record struct ValidationSuccessPayload(string PrimitiveName);

    /// <summary>
    /// Writes a validation success event to the <see cref="DiagnosticListener"/> if any listener is subscribed.
    /// </summary>
    /// <param name="primitiveName">The name of the domain primitive type that was successfully validated.</param>
#if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "DiagnosticSource event payloads")]
#endif
    public static void WriteValidationSuccess(string primitiveName)
    {
        if (Source.IsEnabled("ValidationSuccess"))
        {
            Source.Write("ValidationSuccess", new ValidationSuccessPayload(primitiveName));
        }
    }

    /// <summary>
    /// Writes a validation failure event to the <see cref="DiagnosticListener"/> and increments the failure counter
    /// via <see cref="DomainPrimitiveEventSource.OnValidationFailed"/>.
    /// </summary>
    /// <param name="primitiveName">The name of the domain primitive type that failed validation.</param>
    /// <param name="errorType">The error code identifying the failure category (e.g., <c>"FORMAT"</c>).</param>
    /// <param name="errorMessage">The human-readable description of the validation failure.</param>
#if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "DiagnosticSource event payloads")]
#endif
    public static void WriteValidationFailure(string primitiveName, string errorType, string errorMessage)
    {
        if (Source.IsEnabled("ValidationFailure"))
        {
            Source.Write("ValidationFailure", new ValidationFailurePayload(primitiveName, errorType, errorMessage));
        }
        
        DomainPrimitiveEventSource.NotifyValidationFailed(primitiveName, errorType, errorMessage);
    }
}
