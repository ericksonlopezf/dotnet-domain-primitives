namespace EricksonLopez.DomainPrimitives.Advanced;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using EricksonLopez.DomainPrimitives;
using EricksonLopez.DomainPrimitives.Validation;

/// <summary>
/// Provides a fluent builder for programmatically validating and creating domain primitive instances
/// with support for additional custom validation rules.
/// </summary>
/// <remarks>
/// This builder allocates heap memory and is not suitable for hot paths.
/// For zero-allocation creation, use <see cref="IDomainPrimitive{TSelf,TValue}.Create"/> or
/// <see cref="IDomainPrimitive{TSelf,TValue}.TryCreate"/> directly.
/// </remarks>
/// <typeparam name="TPrimitive">The domain primitive struct type to build.</typeparam>
/// <typeparam name="TValue">The underlying backing value type.</typeparam>
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class PrimitiveBuilder<TPrimitive, TValue>
    where TPrimitive : struct, IDomainPrimitive<TPrimitive, TValue>
    where TValue : notnull
{
    private TValue? _value;
    private readonly List<Func<TValue, PrimitiveError>> _customRules = [];

    private PrimitiveBuilder() { }

    /// <summary>Creates a new, empty <see cref="PrimitiveBuilder{TPrimitive, TValue}"/> with no value configured.</summary>
    /// <returns>A new builder instance ready for configuration via <see cref="WithValue"/> and <see cref="Must"/>.</returns>
    public static PrimitiveBuilder<TPrimitive, TValue> For() => new();

    /// <summary>
    /// Sets the backing value for the primitive.
    /// </summary>
    /// <param name="value">The raw value.</param>
    /// <returns>The builder instance for chaining.</returns>
    public PrimitiveBuilder<TPrimitive, TValue> WithValue(TValue value)
    {
        _value = value;
        return this;
    }

    /// <summary>
    /// Appends a custom rule predicate to the builder pipeline.
    /// </summary>
    /// <param name="predicate">Predicate function returning true if valid.</param>
    /// <param name="errorCode">Error code if validation fails.</param>
    /// <param name="errorMessage">Error message if validation fails.</param>
    /// <returns>The builder instance for chaining.</returns>
    public PrimitiveBuilder<TPrimitive, TValue> Must(Func<TValue, bool> predicate, string errorCode, string errorMessage)
    {
        _customRules.Add(val => predicate(val) ? PrimitiveError.None : new PrimitiveError(errorCode, errorMessage));
        return this;
    }

    /// <summary>
    /// Validates and constructs the domain primitive instance. Throws <see cref="DomainPrimitiveValidationException"/> on failure.
    /// </summary>
    /// <returns>A valid domain primitive instance.</returns>
    /// <exception cref="DomainPrimitiveValidationException">Thrown when validation fails or value is missing.</exception>
    public TPrimitive BuildOrThrow()
    {
        if (_value is null)
        {
            // Stryker disable once String
            throw new DomainPrimitiveValidationException(new PrimitiveError("NULL_INPUT", "Value was not provided to PrimitiveBuilder."), "value");
        }

        foreach (var rule in _customRules)
        {
            var err = rule(_value);
            if (err.IsError)
            {
                // Stryker disable once String
                throw new DomainPrimitiveValidationException(err, "value");
            }
        }

#if NET7_0_OR_GREATER
        return TPrimitive.Create(_value);
#else
        throw new NotSupportedException("PrimitiveBuilder requires .NET 7.0 or greater for static abstract interface members.");
#endif
    }

    /// <summary>
    /// Validates and constructs the domain primitive instance.
    /// </summary>
    /// <param name="result">The constructed instance if successful; <c>default</c> otherwise.</param>
    /// <returns><c>true</c> if creation succeeded; otherwise <c>false</c>.</returns>
    public bool Build(out TPrimitive result)
    {
        if (_value is null)
        {
            result = default;
            return false;
        }

        foreach (var rule in _customRules)
        {
            var err = rule(_value);
            if (err.IsError)
            {
                result = default;
                return false;
            }
        }

#if NET7_0_OR_GREATER
        return TPrimitive.TryCreate(_value, out result, out _);
#else
        throw new NotSupportedException("PrimitiveBuilder requires .NET 7.0 or greater for static abstract interface members.");
#endif
    }

    /// <summary>
    /// Deprecated. Use <see cref="Build(out TPrimitive)"/> instead.
    /// </summary>
    /// <remarks>
    /// <strong>DEPRECATED:</strong> <c>BuildResult()</c> has been removed because it depended on
    /// <c>EricksonLopez.Result</c> which is no longer a dependency of this library.
    /// Use <see cref="Build(out TPrimitive)"/> for a non-throwing creation path, or
    /// <see cref="BuildOrThrow()"/> to throw on failure.
    /// Will be removed in v3.0.
    /// </remarks>
    /// <returns>Nothing — always throws.</returns>
    /// <exception cref="NotSupportedException">Always thrown — this method is a stub for binary compatibility only.</exception>
    [Obsolete(
        "BuildResult() is deprecated and no longer functional. The EricksonLopez.Result dependency was removed. " +
        "Use Build(out TPrimitive result) for a non-throwing creation path, or BuildOrThrow() to throw on failure. " +
        "Will be removed in v3.0. See BREAKING_CHANGES.md.",
        error: false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [ExcludeFromCodeCoverage]
    public object BuildResult()
    {
        throw new NotSupportedException(
            "BuildResult() is deprecated and no longer functional. The EricksonLopez.Result dependency was removed. " +
            "Use Build(out TPrimitive result) or BuildOrThrow() instead.");
    }
}
