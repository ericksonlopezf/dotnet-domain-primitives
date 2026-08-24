// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;

namespace EricksonLopez.DomainPrimitives.Advanced;

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
    /// Sets the backing value for the primitive to build.
    /// </summary>
    /// <param name="value">The raw value to configure.</param>
    /// <returns>The builder instance for chaining.</returns>
    public PrimitiveBuilder<TPrimitive, TValue> WithValue(TValue value)
    {
        _value = value;
        return this;
    }

    /// <summary>
    /// Appends a custom validation rule predicate to the builder pipeline.
    /// </summary>
    /// <param name="predicate">A predicate function returning <see langword="true"/> if the value is valid.</param>
    /// <param name="errorCode">The error code identifying the failure when validation fails.</param>
    /// <param name="errorMessage">The human-readable description of the error when validation fails.</param>
    /// <returns>The builder instance for chaining.</returns>
    public PrimitiveBuilder<TPrimitive, TValue> Must(Func<TValue, bool> predicate, string errorCode, string errorMessage)
    {
        _customRules.Add(val => predicate(val) ? PrimitiveError.None : new PrimitiveError(errorCode, errorMessage));
        return this;
    }

    /// <summary>
    /// Validates and constructs the domain primitive instance.
    /// </summary>
    /// <returns>A valid domain primitive instance.</returns>
    /// <exception cref="DomainPrimitiveValidationException">Validation fails or the value was not provided</exception>
    public TPrimitive BuildOrThrow()
    {
        if (_value is null)
        {
            throw new DomainPrimitiveValidationException(new PrimitiveError("NULL_INPUT", "Value was not provided to PrimitiveBuilder."), "value");
        }

        foreach (var rule in _customRules)
        {
            var err = rule(_value);
            if (err.IsError)
            {
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
    /// Attempts to validate and construct the domain primitive instance without throwing exceptions.
    /// </summary>
    /// <param name="result">When this method returns <see langword="true"/>, contains the constructed domain primitive instance; otherwise, the default value for <typeparamref name="TPrimitive"/>.</param>
    /// <returns><see langword="true"/> if creation succeeded; otherwise, <see langword="false"/>.</returns>
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
}




