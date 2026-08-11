using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using EricksonLopez.DomainPrimitives.Diagnostics;

namespace EricksonLopez.DomainPrimitives.Abstractions.UnitTests;

public class DiagnosticsTests
{
    [Fact]
    public void DomainPrimitivesDiagnostics_WriteValidationSuccess_WithListener_ShouldLog()
    {
        // Arrange
        var observer = new TestDiagnosticObserver();
        using var subscription = DiagnosticListener.AllListeners.Subscribe(observer);
        
        // Act
        DomainPrimitivesDiagnostics.WriteValidationSuccess("TestPrimitive");

        // Assert
        observer.SuccessPayload.Should().NotBeNull();
        observer.SuccessPayload?.PrimitiveName.Should().Be("TestPrimitive");
    }

    [Fact]
    public void DomainPrimitivesDiagnostics_WriteValidationFailure_WithListener_ShouldLog()
    {
        // Arrange
        var observer = new TestDiagnosticObserver();
        using var subscription = DiagnosticListener.AllListeners.Subscribe(observer);

        // Act
        DomainPrimitivesDiagnostics.WriteValidationFailure("TestPrimitive", "Error1", "Message");

        // Assert
        observer.FailurePayload.Should().NotBeNull();
        observer.FailurePayload?.PrimitiveName.Should().Be("TestPrimitive");
        observer.FailurePayload?.ErrorType.Should().Be("Error1");
        observer.FailurePayload?.ErrorMessage.Should().Be("Message");
    }

    [Fact]
    public void DomainPrimitivesMetrics_RecordCreation_ShouldRecordMetric_WhenEnabled()
    {
        // Arrange
        DomainPrimitivesMetrics.IsEnabled = true;
        using var listener = new MeterListener();
        long recordedValue = 0;
        var tags = new List<KeyValuePair<string, object?>>();
        string description = "";
        string meterName = "";
        string meterVersion = "";
        listener.InstrumentPublished = (instrument, listener) =>
        {
            if (instrument.Name == "domain_primitive.creation")
            {
                description = instrument.Description ?? "";
                meterName = instrument.Meter.Name;
                meterVersion = instrument.Meter.Version ?? "";
                listener.EnableMeasurementEvents(instrument);
            }
        };
        listener.SetMeasurementEventCallback<long>((instrument, measurement, tagsCollection, state) =>
        {
            recordedValue = measurement;
            foreach (var tag in tagsCollection) tags.Add(new KeyValuePair<string, object?>(tag.Key, tag.Value));
        });
        listener.Start();

        // Act
        DomainPrimitivesMetrics.RecordCreation("MyPrimitive");

        // Assert
        recordedValue.Should().Be(1);
        tags.Should().Contain(new KeyValuePair<string, object?>("primitive_type", "MyPrimitive"));
        description.Should().Be("Number of domain primitives successfully created by type.");
        meterName.Should().Be("EricksonLopez.DomainPrimitives");
        meterVersion.Should().Be("1.0.0");
    }

    [Fact]
    public void DomainPrimitivesMetrics_RecordCreation_ShouldNotRecordMetric_WhenDisabled()
    {
        // Arrange
        DomainPrimitivesMetrics.IsEnabled = false;
        using var listener = new MeterListener();
        long recordedValue = 0;
        listener.InstrumentPublished = (instrument, listener) =>
        {
            if (instrument.Name == "domain_primitive.creation") listener.EnableMeasurementEvents(instrument);
        };
        listener.SetMeasurementEventCallback<long>((instrument, measurement, tagsCollection, state) =>
        {
            recordedValue = measurement;
        });
        listener.Start();

        // Act
        DomainPrimitivesMetrics.RecordCreation("MyPrimitive");

        // Assert
        recordedValue.Should().Be(0);
        DomainPrimitivesMetrics.IsEnabled = true; // Restore
    }

    [Fact]
    public void DomainPrimitivesMetrics_RecordValidationSuccess_ShouldRecordMetric()
    {
        // Arrange
        DomainPrimitivesMetrics.IsEnabled = true;
        using var listener = new MeterListener();
        long recordedValue = 0;
        var tags = new List<KeyValuePair<string, object?>>();
        string description = "";
        listener.InstrumentPublished = (instrument, listener) =>
        {
            if (instrument.Name == "domain_primitive.validation.success")
            {
                description = instrument.Description ?? "";
                listener.EnableMeasurementEvents(instrument);
            }
        };
        listener.SetMeasurementEventCallback<long>((instrument, measurement, tagsCollection, state) =>
        {
            recordedValue = measurement;
            foreach (var tag in tagsCollection) tags.Add(new KeyValuePair<string, object?>(tag.Key, tag.Value));
        });
        listener.Start();

        // Act
        DomainPrimitivesMetrics.RecordValidationSuccess("SuccessPrimitive");

        // Assert
        recordedValue.Should().Be(1);
        tags.Should().Contain(new KeyValuePair<string, object?>("primitive_type", "SuccessPrimitive"));
        description.Should().Be("Number of successfully validated domain primitives.");
    }
    
    [Fact]
    public void DomainPrimitivesMetrics_RecordValidationSuccess_ShouldNotRecordMetric_WhenDisabled()
    {
        // Arrange
        DomainPrimitivesMetrics.IsEnabled = false;
        using var listener = new MeterListener();
        long recordedValue = 0;
        listener.InstrumentPublished = (instrument, listener) =>
        {
            if (instrument.Name == "domain_primitive.validation.success") listener.EnableMeasurementEvents(instrument);
        };
        listener.SetMeasurementEventCallback<long>((instrument, measurement, tagsCollection, state) =>
        {
            recordedValue = measurement;
        });
        listener.Start();

        // Act
        DomainPrimitivesMetrics.RecordValidationSuccess("SuccessPrimitive");

        // Assert
        recordedValue.Should().Be(0);
        DomainPrimitivesMetrics.IsEnabled = true; // Restore
    }

    [Fact]
    public void DomainPrimitivesMetrics_RecordValidationFailure_ShouldRecordMetric()
    {
        // Arrange
        DomainPrimitivesMetrics.IsEnabled = true;
        using var listener = new MeterListener();
        long recordedValue = 0;
        var tags = new List<KeyValuePair<string, object?>>();
        string description = "";
        listener.InstrumentPublished = (instrument, listener) =>
        {
            if (instrument.Name == "domain_primitive.validation.failure")
            {
                description = instrument.Description ?? "";
                listener.EnableMeasurementEvents(instrument);
            }
        };
        listener.SetMeasurementEventCallback<long>((instrument, measurement, tagsCollection, state) =>
        {
            recordedValue = measurement;
            foreach (var tag in tagsCollection) tags.Add(new KeyValuePair<string, object?>(tag.Key, tag.Value));
        });
        listener.Start();

        // Act
        DomainPrimitivesMetrics.RecordValidationFailure("FailPrimitive", "ErrType", "ErrMsg");

        // Assert
        recordedValue.Should().Be(1);
        tags.Should().Contain(new KeyValuePair<string, object?>("primitive_type", "FailPrimitive"));
        tags.Should().Contain(new KeyValuePair<string, object?>("error_type", "ErrType"));
        description.Should().Be("Number of domain primitives that failed validation.");
    }
    
    [Fact]
    public void DomainPrimitivesMetrics_RecordValidationFailure_ShouldNotRecordMetric_WhenDisabled()
    {
        // Arrange
        DomainPrimitivesMetrics.IsEnabled = false;
        using var listener = new MeterListener();
        long recordedValue = 0;
        listener.InstrumentPublished = (instrument, listener) =>
        {
            if (instrument.Name == "domain_primitive.validation.failure") listener.EnableMeasurementEvents(instrument);
        };
        listener.SetMeasurementEventCallback<long>((instrument, measurement, tagsCollection, state) =>
        {
            recordedValue = measurement;
        });
        listener.Start();

        // Act
        DomainPrimitivesMetrics.RecordValidationFailure("FailPrimitive", "ErrType", "ErrMsg");

        // Assert
        recordedValue.Should().Be(0);
        DomainPrimitivesMetrics.IsEnabled = true; // Restore
    }

    [Fact]
    public void DomainPrimitiveEventSource_EventSubscriptionAndInvocation_ShouldWork()
    {
        // Arrange
        bool wasCalled = false;
        EventHandler<ValidationFailureEventArgs> handler = (sender, args) => 
        { 
            wasCalled = true; 
            args.PrimitiveName.Should().Be("TestPrimitive");
            args.ErrorType.Should().Be("Error1");
            args.ErrorMessage.Should().Be("Message");
        };

        DomainPrimitiveEventSource.OnValidationFailed += handler;

        try
        {
            // Act - WriteValidationFailure invokes DomainPrimitiveEventSource.NotifyValidationFailed internally
            DomainPrimitivesDiagnostics.WriteValidationFailure("TestPrimitive", "Error1", "Message");
            
            // Assert
            wasCalled.Should().BeTrue();
        }
        finally
        {
            DomainPrimitiveEventSource.OnValidationFailed -= handler;
        }
    }

    private sealed class TestDiagnosticObserver : IObserver<DiagnosticListener>, IObserver<KeyValuePair<string, object?>>
    {
        private readonly List<IDisposable> _subscriptions = new();
        public DomainPrimitivesDiagnostics.ValidationSuccessPayload? SuccessPayload { get; private set; }
        public DomainPrimitivesDiagnostics.ValidationFailurePayload? FailurePayload { get; private set; }

        public void OnCompleted() { }
        public void OnError(Exception error) { }

        public void OnNext(DiagnosticListener value)
        {
            if (value.Name == "EricksonLopez.DomainPrimitives")
            {
                _subscriptions.Add(value.Subscribe(this));
            }
        }

        public void OnNext(KeyValuePair<string, object?> value)
        {
            if (value.Key == "ValidationSuccess" && value.Value is DomainPrimitivesDiagnostics.ValidationSuccessPayload success)
            {
                SuccessPayload = success;
            }
            else if (value.Key == "ValidationFailure" && value.Value is DomainPrimitivesDiagnostics.ValidationFailurePayload failure)
            {
                FailurePayload = failure;
            }
        }
    }
}

