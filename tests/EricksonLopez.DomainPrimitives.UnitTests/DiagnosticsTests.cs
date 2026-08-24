// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using AwesomeAssertions;
using EricksonLopez.DomainPrimitives.Diagnostics;
using Xunit;

namespace EricksonLopez.DomainPrimitives.UnitTests
{
    [Collection("Diagnostics")]
    public class DiagnosticsTests
    {
        [Fact]
        public void DomainPrimitivesDiagnostics_ListenerName_And_Meter_Constants_ShouldBeExpected()
        {
            DomainPrimitivesDiagnostics.ListenerName.Should().Be("EricksonLopez.DomainPrimitives");
            DomainPrimitivesDiagnostics.Source.Name.Should().Be("EricksonLopez.DomainPrimitives");
            DomainPrimitivesDiagnostics.Meter.Name.Should().Be("EricksonLopez.DomainPrimitives");
            DomainPrimitivesDiagnostics.Meter.Version.Should().Be("1.0.0");
            DomainPrimitivesMetrics.MeterName.Should().Be("EricksonLopez.DomainPrimitives");
        }

        [Fact]
        public void DomainPrimitivesDiagnostics_WriteValidationSuccess_WithListener_ShouldLog()
        {
            var observer = new DirectDiagnosticObserver();
            using var subscription = DomainPrimitivesDiagnostics.Source.Subscribe(observer);

            DomainPrimitivesDiagnostics.WriteValidationSuccess("TestPrimitive");

            observer.SuccessPayload.Should().NotBeNull();
            observer.SuccessPayload?.PrimitiveName.Should().Be("TestPrimitive");
        }

        [Fact]
        public void DomainPrimitivesDiagnostics_WriteValidationFailure_WithListener_ShouldLog()
        {
            var observer = new DirectDiagnosticObserver();
            using var subscription = DomainPrimitivesDiagnostics.Source.Subscribe(observer);

            DomainPrimitivesDiagnostics.WriteValidationFailure("TestPrimitive", "Error1", "Message");

            observer.FailurePayload.Should().NotBeNull();
            observer.FailurePayload?.PrimitiveName.Should().Be("TestPrimitive");
            observer.FailurePayload?.ErrorType.Should().Be("Error1");
            observer.FailurePayload?.ErrorMessage.Should().Be("Message");
        }

        [Fact]
        public void DomainPrimitivesDiagnostics_DirectWrites_WhenNoListeners_DoNotThrow()
        {
            var actSuccess = () => DomainPrimitivesDiagnostics.WriteValidationSuccess("DirectPrimitive");
            actSuccess.Should().NotThrow();

            var actFailure = () => DomainPrimitivesDiagnostics.WriteValidationFailure("DirectPrimitive", "ERR_CODE", "Error message");
            actFailure.Should().NotThrow();
        }

        [Fact]
        public void DomainPrimitivesDiagnostics_WriteValidationSuccess_WithExactEventPredicate_ShouldCheckExactName()
        {
            var observer = new DirectDiagnosticObserver();
            using var subscription = DomainPrimitivesDiagnostics.Source.Subscribe(observer, name => name == "ValidationSuccess");

            DomainPrimitivesDiagnostics.WriteValidationSuccess("ExactPrimitiveSuccess");

            observer.SuccessPayload.Should().NotBeNull();
            observer.SuccessPayload?.PrimitiveName.Should().Be("ExactPrimitiveSuccess");
        }

        [Fact]
        public void DomainPrimitivesDiagnostics_WriteValidationFailure_WithExactEventPredicate_ShouldCheckExactName()
        {
            var observer = new DirectDiagnosticObserver();
            using var subscription = DomainPrimitivesDiagnostics.Source.Subscribe(observer, name => name == "ValidationFailure");

            DomainPrimitivesDiagnostics.WriteValidationFailure("ExactPrimitiveFail", "FORMAT", "Exact fail msg");

            observer.FailurePayload.Should().NotBeNull();
            observer.FailurePayload?.PrimitiveName.Should().Be("ExactPrimitiveFail");
            observer.FailurePayload?.ErrorType.Should().Be("FORMAT");
            observer.FailurePayload?.ErrorMessage.Should().Be("Exact fail msg");
        }

        [Fact]
        public void ValidationPayloads_Properties_AreInitializedCorrectly()
        {
            var successPayload = new DomainPrimitivesDiagnostics.ValidationSuccessPayload("PrimitiveA");
            successPayload.PrimitiveName.Should().Be("PrimitiveA");

            var failurePayload = new DomainPrimitivesDiagnostics.ValidationFailurePayload("PrimitiveB", "CODE", "Description");
            failurePayload.PrimitiveName.Should().Be("PrimitiveB");
            failurePayload.ErrorType.Should().Be("CODE");
            failurePayload.ErrorMessage.Should().Be("Description");
        }

        [Fact]
        public void DomainPrimitivesMetrics_DefaultIsEnabled_ShouldBeTrue()
        {
            DomainPrimitivesMetrics.IsEnabled.Should().BeTrue();
        }

        [Fact]
        public void DomainPrimitivesMetrics_RecordCreation_ShouldRecordMetric_WhenEnabled()
        {
            long recordedValue = 0;
            var tags = new List<KeyValuePair<string, object?>>();
            string description = "";
            string meterName = "";
            string meterVersion = "";

            using var listener = CreateMeterListener("domain_primitive.creation", (val, tagList, desc, mName, mVersion) =>
            {
                recordedValue = val;
                tags.AddRange(tagList);
                description = desc;
                meterName = mName;
                meterVersion = mVersion;
            });

            DomainPrimitivesMetrics.RecordCreation("MyPrimitive");

            recordedValue.Should().Be(1);
            tags.Should().Contain(new KeyValuePair<string, object?>("primitive_type", "MyPrimitive"));
            description.Should().Be("Number of domain primitives successfully created by type.");
            meterName.Should().Be("EricksonLopez.DomainPrimitives");
            meterVersion.Should().Be("1.0.0");
        }

        [Fact]
        public void DomainPrimitivesMetrics_RecordCreation_ShouldNotRecordMetric_WhenDisabled()
        {
            WithDisabledMetrics(() =>
            {
                long recordedValue = 0;
                using var listener = CreateMeterListener("domain_primitive.creation", (val, _, _, _, _) =>
                {
                    recordedValue = val;
                });

                DomainPrimitivesMetrics.RecordCreation("MyPrimitive");

                recordedValue.Should().Be(0);
            });
        }

        [Fact]
        public void DomainPrimitivesMetrics_RecordValidationSuccess_ShouldRecordMetric()
        {
            long recordedValue = 0;
            var tags = new List<KeyValuePair<string, object?>>();
            string description = "";

            using var listener = CreateMeterListener("domain_primitive.validation.success", (val, tagList, desc, _, _) =>
            {
                recordedValue = val;
                tags.AddRange(tagList);
                description = desc;
            });

            DomainPrimitivesMetrics.RecordValidationSuccess("SuccessPrimitive");

            recordedValue.Should().Be(1);
            tags.Should().Contain(new KeyValuePair<string, object?>("primitive_type", "SuccessPrimitive"));
            description.Should().Be("Number of successfully validated domain primitives.");
        }

        [Fact]
        public void DomainPrimitivesMetrics_RecordValidationSuccess_ShouldNotRecordMetric_WhenDisabled()
        {
            WithDisabledMetrics(() =>
            {
                long recordedValue = 0;
                using var listener = CreateMeterListener("domain_primitive.validation.success", (val, _, _, _, _) =>
                {
                    recordedValue = val;
                });

                DomainPrimitivesMetrics.RecordValidationSuccess("SuccessPrimitive");

                recordedValue.Should().Be(0);
            });
        }

        [Fact]
        public void DomainPrimitivesMetrics_RecordValidationFailure_ShouldRecordMetric()
        {
            long recordedValue = 0;
            var tags = new List<KeyValuePair<string, object?>>();
            string description = "";

            using var listener = CreateMeterListener("domain_primitive.validation.failure", (val, tagList, desc, _, _) =>
            {
                recordedValue = val;
                tags.AddRange(tagList);
                description = desc;
            });

            DomainPrimitivesMetrics.RecordValidationFailure("FailPrimitive", "ErrType", "ErrMsg");

            recordedValue.Should().Be(1);
            tags.Should().Contain(new KeyValuePair<string, object?>("primitive_type", "FailPrimitive"));
            tags.Should().Contain(new KeyValuePair<string, object?>("error_type", "ErrType"));
            description.Should().Be("Number of domain primitives that failed validation.");
        }

        [Fact]
        public void DomainPrimitivesMetrics_RecordValidationFailure_ShouldNotRecordMetric_WhenDisabled()
        {
            WithDisabledMetrics(() =>
            {
                long recordedValue = 0;
                using var listener = CreateMeterListener("domain_primitive.validation.failure", (val, _, _, _, _) =>
                {
                    recordedValue = val;
                });

                DomainPrimitivesMetrics.RecordValidationFailure("FailPrimitive", "ErrType", "ErrMsg");

                recordedValue.Should().Be(0);
            });
        }

        [Fact]
        public void DomainPrimitivesMetrics_RecordValidationSuccess_ShouldAlsoWriteToDiagnosticListener()
        {
            var observer = new DirectDiagnosticObserver();
            using var subscription = DomainPrimitivesDiagnostics.Source.Subscribe(observer);

            DomainPrimitivesMetrics.RecordValidationSuccess("EmailAddress");

            observer.SuccessPayload.Should().NotBeNull();
            observer.SuccessPayload?.PrimitiveName.Should().Be("EmailAddress");
        }

        [Fact]
        public void DomainPrimitivesMetrics_RecordValidationFailure_ShouldAlsoWriteToDiagnosticListener()
        {
            var observer = new DirectDiagnosticObserver();
            using var subscription = DomainPrimitivesDiagnostics.Source.Subscribe(observer);

            DomainPrimitivesMetrics.RecordValidationFailure("EmailAddress", "FORMAT", "Invalid email format");

            observer.FailurePayload.Should().NotBeNull();
            observer.FailurePayload?.PrimitiveName.Should().Be("EmailAddress");
            observer.FailurePayload?.ErrorType.Should().Be("FORMAT");
            observer.FailurePayload?.ErrorMessage.Should().Be("Invalid email format");
        }

        [Fact]
        public void DomainPrimitiveEventSource_OnValidationFailed_InvokesSubscribers()
        {
            ValidationFailureEventArgs? capturedArgs = null;
            EventHandler<ValidationFailureEventArgs> handler = (sender, args) =>
            {
                capturedArgs = args;
            };

            DomainPrimitiveEventSource.OnValidationFailed += handler;
            try
            {
                DomainPrimitivesDiagnostics.WriteValidationFailure("EmailPrimitive", "INVALID_FORMAT", "Invalid format");
                capturedArgs.Should().NotBeNull();
                capturedArgs!.Value.PrimitiveName.Should().Be("EmailPrimitive");
                capturedArgs!.Value.ErrorType.Should().Be("INVALID_FORMAT");
                capturedArgs!.Value.ErrorMessage.Should().Be("Invalid format");
            }
            finally
            {
                DomainPrimitiveEventSource.OnValidationFailed -= handler;
            }
        }

        [Fact]
        public void DomainPrimitiveEventSource_EventSubscriptionAndInvocation_ShouldWork()
        {
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
                DomainPrimitivesDiagnostics.WriteValidationFailure("TestPrimitive", "Error1", "Message");
                wasCalled.Should().BeTrue();
            }
            finally
            {
                DomainPrimitiveEventSource.OnValidationFailed -= handler;
            }
        }

        /// <summary>
        /// Creates, configures, and starts a <see cref="MeterListener"/> to capture measurements published by the Domain Primitives meter.
        /// This is the official .NET BCL mechanism for testing System.Diagnostics.Metrics without third-party APM or telemetry dependencies.
        /// </summary>
        /// <param name="targetInstrumentName">The exact name of the instrument to listen to.</param>
        /// <param name="onMeasurement">Callback invoked whenever a measurement with its tags is recorded.</param>
        /// <returns>An active <see cref="MeterListener"/> instance that must be disposed when the test finishes.</returns>
        private static MeterListener CreateMeterListener(
            string targetInstrumentName,
            Action<long, List<KeyValuePair<string, object?>>, string, string, string> onMeasurement)
        {
            var listener = new MeterListener();
            string description = "";
            string meterName = "";
            string meterVersion = "";

            listener.InstrumentPublished = (instrument, l) =>
            {
                if (instrument.Name == targetInstrumentName)
                {
                    description = instrument.Description ?? "";
                    meterName = instrument.Meter.Name;
                    meterVersion = instrument.Meter.Version ?? "";
                    l.EnableMeasurementEvents(instrument);
                }
            };

            listener.SetMeasurementEventCallback<long>((instrument, measurement, tagsCollection, state) =>
            {
                var tags = new List<KeyValuePair<string, object?>>();
                foreach (var tag in tagsCollection)
                {
                    tags.Add(new KeyValuePair<string, object?>(tag.Key, tag.Value));
                }
                onMeasurement(measurement, tags, description, meterName, meterVersion);
            });

            listener.Start();
            return listener;
        }

        /// <summary>
        /// Executes an action with metrics temporarily disabled (<see cref="DomainPrimitivesMetrics.IsEnabled"/> = false)
        /// and guarantees that the original metric state is restored in a finally block to prevent test pollution.
        /// </summary>
        private static void WithDisabledMetrics(Action action)
        {
            var original = DomainPrimitivesMetrics.IsEnabled;
            try
            {
                DomainPrimitivesMetrics.IsEnabled = false;
                action();
            }
            finally
            {
                DomainPrimitivesMetrics.IsEnabled = original;
            }
        }

        private sealed class DirectDiagnosticObserver : IObserver<KeyValuePair<string, object?>>
        {
            public DomainPrimitivesDiagnostics.ValidationSuccessPayload? SuccessPayload { get; private set; }
            public DomainPrimitivesDiagnostics.ValidationFailurePayload? FailurePayload { get; private set; }

            public void OnCompleted() { }
            public void OnError(Exception error) { }

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
}


