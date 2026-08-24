// Copyright © Erickson Lopez. MIT License.
using System;
using System.Reflection;

namespace EricksonLopez.DomainPrimitives.Testing;

/// <summary>
/// Provides utilities for creating domain primitive instances in test scenarios, providing both
/// the validated (via <c>Create()</c>) and exceptional unvalidated paths.
/// </summary>
/// <remarks>
/// <para>
/// The primary way to create test primitives is through the standard <c>Create()</c> factory.
/// Use <see cref="CreateUnvalidated{TPrimitive, TValue}"/> only when you need to test how your
/// system behaves when it receives an already-constructed but logically invalid primitive
/// (e.g., to test repositories or mappers with pre-existing database data).
/// </para>
/// </remarks>
public static class DomainPrimitiveTestBuilder
{
    /// <summary>
    /// Creates a valid domain primitive using the standard <c>Create()</c> factory.
    /// Throws <see cref="DomainPrimitiveValidationException"/> if the value fails validation.
    /// </summary>
    /// <typeparam name="TPrimitive">The domain primitive type.</typeparam>
    /// <typeparam name="TValue">The backing value type.</typeparam>
    /// <param name="value">A valid backing value.</param>
    /// <returns>A validated domain primitive instance.</returns>
    /// <exception cref="DomainPrimitiveValidationException">Thrown when the value is invalid.</exception>
    public static TPrimitive Create<TPrimitive, TValue>(TValue value)
        where TPrimitive : struct, IDomainPrimitive<TPrimitive, TValue>
        where TValue : notnull
    {
        return TPrimitive.Create(value);
    }

    /// <summary>
    /// Validates that creating a primitive from the given value fails and returns the
    /// thrown <see cref="DomainPrimitiveValidationException"/> for further inspection.
    /// </summary>
    /// <typeparam name="TPrimitive">The domain primitive type.</typeparam>
    /// <typeparam name="TValue">The backing value type.</typeparam>
    /// <param name="value">A value expected to fail validation.</param>
    /// <returns>The exception thrown during validation.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if creation succeeds unexpectedly (i.e., the value is actually valid).
    /// </exception>
    public static DomainPrimitiveValidationException AssertCreationFails<TPrimitive, TValue>(TValue value)
        where TPrimitive : struct, IDomainPrimitive<TPrimitive, TValue>
        where TValue : notnull
    {
        try
        {
            TPrimitive.Create(value);
            throw new InvalidOperationException($"Expected creation of {typeof(TPrimitive).Name} with value '{value}' to fail, but it succeeded.");
        }
        catch (DomainPrimitiveValidationException ex)
        {
            return ex;
        }
    }

    /// <summary>
    /// Creates a domain primitive instance directly from its backing value, bypassing all validation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <strong>Use with extreme caution and only in testing.</strong> This method uses reflection
    /// to invoke the private source-generated constructor, which circumvents the domain invariants.
    /// </para>
    /// <para>
    /// Appropriate use cases:
    /// <list type="bullet">
    ///   <item>Testing repository/mapper behavior with pre-existing "dirty" database data.</item>
    ///   <item>Testing how your application handles primitives reconstructed from external systems.</item>
    /// </list>
    /// </para>
    /// <para>
    /// Do <em>not</em> use this to avoid writing proper test data — prefer <see cref="DomainPrimitiveFakeFactory"/>
    /// for valid values and <see cref="DomainPrimitiveScenarios"/> for parameterized invalid values.
    /// </para>
    /// </remarks>
    /// <typeparam name="TPrimitive">The domain primitive type.</typeparam>
    /// <typeparam name="TValue">The backing value type.</typeparam>
    /// <param name="value">The raw value to wrap without validation.</param>
    /// <returns>An unvalidated domain primitive instance.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the generated private constructor cannot be located.
    /// </exception>
    [System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessage("Trimming", "IL2070", Justification = "Reflection is explicitly intended and acceptable here as this is a test utility strictly for test environments.")]
    public static TPrimitive CreateUnvalidated<TPrimitive, TValue>(TValue value)
        where TPrimitive : struct, IDomainPrimitive<TPrimitive, TValue>
        where TValue : notnull
    {
        // Reflection is intentionally utilized exclusively within this test utility method
        // to instantiate unvalidated domain primitives for legacy/dirty data integration testing.
        // Source-generated domain primitives use a private constructor taking the backing value.
        var constructor = typeof(TPrimitive).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance,
            binder: null,
            types: [typeof(TValue)],
            modifiers: null);

        if (constructor == null)
        {
            throw new InvalidOperationException(
                $"Type {typeof(TPrimitive).Name} does not have a private constructor taking {typeof(TValue).Name}. " +
                $"Is it a valid domain primitive generated by EricksonLopez.DomainPrimitives?");
        }

        return (TPrimitive)constructor.Invoke([value]);
    }
}


