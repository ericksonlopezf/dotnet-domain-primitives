// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;

namespace EricksonLopez.DomainPrimitives.Testing;

/// <summary>
/// Provides deterministic, pre-defined valid and invalid test values for domain primitive scenarios.
/// </summary>
/// <remarks>
/// <para>
/// Unlike random data generators (e.g., Bogus, AutoFixture), <see cref="DomainPrimitiveFakeFactory"/>
/// returns <strong>deterministic</strong> values to ensure reproducible tests.
/// </para>
/// <para>
/// All valid values are real-world representative samples. Invalid values are carefully chosen
/// to cover boundary conditions and common mistake patterns.
/// </para>
/// </remarks>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static partial class DomainPrimitiveFakeFactory
{
}
