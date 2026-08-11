using System;
using System.Diagnostics.Metrics;

namespace EricksonLopez.DomainPrimitives.Diagnostics;

/// <summary>
/// Provides OpenTelemetry metrics for Domain Primitives, specifically tracking
/// validation success and failure rates.
/// </summary>
/// <remarks>
/// Moved from <c>EricksonLopez.DomainPrimitives.Abstractions</c> to <c>EricksonLopez.DomainPrimitives</c> in v1.2.0.
/// Abstractions should contain only: attributes, marker interfaces, and <c>PrimitiveError</c>.
/// See BREAKING_CHANGES.md for migration details.
/// </remarks>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static class DomainPrimitivesMetrics
{
    /// <summary>The name of the <see cref="System.Diagnostics.Metrics.Meter"/> used for domain primitive metrics.</summary>
    public static readonly string MeterName = DomainPrimitivesDiagnostics.Meter.Name;
    private static readonly Meter Meter = DomainPrimitivesDiagnostics.Meter;

    private static readonly Counter<long> ValidationSuccessCounter = Meter.CreateCounter<long>(
        "domain_primitive.validation.success",
        description: "Number of successfully validated domain primitives.");

    private static readonly Counter<long> ValidationFailureCounter = Meter.CreateCounter<long>(
        "domain_primitive.validation.failure",
        description: "Number of domain primitives that failed validation.");

    private static readonly Counter<long> CreationCounter = Meter.CreateCounter<long>(
        "domain_primitive.creation",
        description: "Number of domain primitives successfully created by type.");

    /// <summary>Gets or sets a value indicating whether metrics collection is globally active. Default: <see langword="true"/>.</summary>
    public static bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Records the successful creation of a domain primitive instance.
    /// </summary>
    /// <param name="primitiveName">The name of the primitive type.</param>
    public static void RecordCreation(string primitiveName)
    {
        if (!IsEnabled) return;
        var tags = new System.Diagnostics.TagList { { "primitive_type", primitiveName } };
        CreationCounter.Add(1, tags);
    }

    /// <summary>
    /// Records a successful validation of a domain primitive.
    /// </summary>
    /// <param name="primitiveName">The name of the primitive type (e.g. 'EmailAddress').</param>
    public static void RecordValidationSuccess(string primitiveName)
    {
        if (!IsEnabled) return;
        var tags = new System.Diagnostics.TagList { { "primitive_type", primitiveName } };
        ValidationSuccessCounter.Add(1, tags);
        DomainPrimitivesDiagnostics.WriteValidationSuccess(primitiveName);
    }

    /// <summary>
    /// Records a validation failure for a domain primitive.
    /// </summary>
    /// <param name="primitiveName">The name of the primitive type.</param>
    /// <param name="errorType">The type of error (e.g. 'Format', 'Length').</param>
    /// <param name="errorMessage">The detailed error message.</param>
    public static void RecordValidationFailure(string primitiveName, string errorType, string errorMessage)
    {
        if (!IsEnabled) return;
        var tags = new System.Diagnostics.TagList 
        { 
            { "primitive_type", primitiveName }, 
            { "error_type", errorType } 
        };
        ValidationFailureCounter.Add(1, tags);
        DomainPrimitivesDiagnostics.WriteValidationFailure(primitiveName, errorType, errorMessage);
    }
}
