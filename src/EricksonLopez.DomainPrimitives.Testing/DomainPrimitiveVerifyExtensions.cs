// Copyright © Erickson Lopez. MIT License.
using System;
using VerifyTests;

namespace EricksonLopez.DomainPrimitives.Testing;

/// <summary>
/// Provides extension methods and initializers for Verify snapshot testing with domain primitives.
/// </summary>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static class DomainPrimitiveVerifyExtensions
{
    private static bool _initialized;

    /// <summary>
    /// Configures Verify to serialize domain primitives using their underlying value
    /// rather than as a complex object with a <c>Value</c> property.
    /// </summary>
    /// <remarks>
    /// This method is idempotent — calling it more than once has no effect.
    /// Invoke it once during test module initialization (e.g., in a
    /// <c>static</c> constructor, a <c>[ModuleInitializer]</c>, or a test fixture's
    /// <c>InitializeAsync</c> method) before any snapshot is generated.
    /// </remarks>
    public static void Initialize()
    {
        if (_initialized) return;

        VerifierSettings.AddExtraSettings(serializerSettings =>
        {
            serializerSettings.Converters.Add(new DomainPrimitiveVerifyJsonConverter());
        });

        _initialized = true;
    }
}
