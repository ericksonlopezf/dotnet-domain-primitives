using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
// ============================================================================
// CHAPTER 18: OBSERVABILITY, METRICS AND OPENTELEMETRY
// ============================================================================
// In this chapter you will learn how the library natively instruments
// System.Diagnostics.Metrics metrics compatible with Prometheus and OpenTelemetry.
//
// NATIVELY RECORDED METRICS:
// 1. `domain_primitive.creation`: Count of created instances by type.
// 2. `domain_primitive.validation.success`: Validation successes.
// 3. `domain_primitive.validation.failure`: Validation failures with error type tags.
// ============================================================================

using System.Diagnostics.Metrics;
using Chapter18;
using EricksonLopez.DomainPrimitives;
using EricksonLopez.DomainPrimitives.Diagnostics;

Console.WriteLine("=========================================================");
Console.WriteLine(" 📘 CHAPTER 18: OBSERVABILITY & OPENTELEMETRY METRICS");
Console.WriteLine("=========================================================\n");

// ----------------------------------------------------------------------------
// 1. METRICS LISTENER CONFIGURATION (OPENTELEMETRY / PROMETHEUS SIMULATOR)
// ----------------------------------------------------------------------------
Console.WriteLine("--- 📊 INITIALIZING OPENTELEMETRY METERLISTENER ---");

using var meterListener = new MeterListener();

meterListener.InstrumentPublished = (instrument, listener) =>
{
    if (instrument.Meter.Name == DomainPrimitivesMetrics.MeterName)
    {
        listener.EnableMeasurementEvents(instrument);
        Console.WriteLine($"[MeterListener] Subscribed to metric: {instrument.Name}");
    }
};

meterListener.SetMeasurementEventCallback<long>((instrument, measurement, tags, state) =>
{
    string typeTag = tags.ToArray().FirstOrDefault(t => t.Key == "primitive_type").Value?.ToString() ?? "N/A";
    Console.WriteLine($" 📈 [OPENTELEMETRY METRIC] {instrument.Name} = +{measurement} (primitive_type: {typeTag})");
});

meterListener.Start();

Console.WriteLine("\n--- ⚡ GENERATING DOMAIN EVENTS TO MEASURE METRICS ---");

// Create valid instances (Triggers success metrics)
_ = EmailAddress.Create("observability.ok@domain.com");
_ = CustomerId.Create();
_ = Money.Create(100.00m);

// Create invalid instance (Triggers validation failure metric)
bool isInvalidResultSuccess = EmailAddress.TryCreate("email-with-failure", out var invalidResultVal, out var invalidResultError);
if (!isInvalidResultSuccess)
{
    Console.WriteLine($"❌ Registered Validation Error: {invalidResultError.Message}");
}

// Force metrics collection
meterListener.RecordObservableInstruments();

Console.WriteLine("\nCHAPTER 18 COMPLETED SUCCESSFULLY.\n");


// ============================================================================
// DEFINITION OF PRIMITIVES
// ============================================================================

namespace Chapter18
{
    [StrongId<Guid>]
    public readonly partial record struct CustomerId;

    [Email]
    public readonly partial record struct EmailAddress;

    [Money(Min = 0)]
    public readonly partial record struct Money;
}


