using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using FluentAssertions;
using EricksonLopez.DomainPrimitives.Diagnostics;
using Xunit;

namespace EricksonLopez.DomainPrimitives.Tests
{
    public class DiagnosticsTests
    {
        [Fact]
        public void RecordValidationSuccess_RecordsMetricAndDiagnosticEvent()
        {
            bool eventFired = false;
            
            using var globalSubscription = DiagnosticListener.AllListeners.Subscribe(new ListenerObserver(listener => 
            {
                if (listener.Name == DomainPrimitivesDiagnostics.ListenerName)
                {
                    listener.Subscribe(new EventObserver("ValidationSuccess", _ => eventFired = true));
                }
            }));

            DomainPrimitivesMetrics.RecordValidationSuccess("TestPrimitive");
            eventFired.Should().BeTrue();
        }

        [Fact]
        public void RecordValidationFailure_RecordsMetricAndDiagnosticEvent()
        {
            bool eventFired = false;

            using var globalSubscription = DiagnosticListener.AllListeners.Subscribe(new ListenerObserver(listener => 
            {
                if (listener.Name == DomainPrimitivesDiagnostics.ListenerName)
                {
                    listener.Subscribe(new EventObserver("ValidationFailure", _ => eventFired = true));
                }
            }));

            DomainPrimitivesMetrics.RecordValidationFailure("TestPrimitive", "TestError", "TestError message");
            eventFired.Should().BeTrue();
        }

        [Fact]
        public void MeterName_IsCorrect()
        {
            DomainPrimitivesMetrics.MeterName.Should().Be("EricksonLopez.DomainPrimitives");
            DomainPrimitivesDiagnostics.ListenerName.Should().Be("EricksonLopez.DomainPrimitives");
        }

        private sealed class ListenerObserver : IObserver<DiagnosticListener>
        {
            private readonly Action<DiagnosticListener> _onNext;
            public ListenerObserver(Action<DiagnosticListener> onNext) => _onNext = onNext;
            public void OnCompleted() { }
            public void OnError(Exception error) { }
            public void OnNext(DiagnosticListener value) => _onNext(value);
        }

        private sealed class EventObserver : IObserver<KeyValuePair<string, object?>>
        {
            private readonly string _eventName;
            private readonly Action<object?> _onNext;

            public EventObserver(string eventName, Action<object?> onNext)
            {
                _eventName = eventName;
                _onNext = onNext;
            }

            public void OnCompleted() { }
            public void OnError(Exception error) { }
            public void OnNext(KeyValuePair<string, object?> value)
            {
                if (value.Key == _eventName)
                {
                    _onNext(value.Value);
                }
            }
        }
    }
}
