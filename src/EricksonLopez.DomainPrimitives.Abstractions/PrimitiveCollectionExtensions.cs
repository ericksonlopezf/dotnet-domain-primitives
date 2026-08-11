using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace EricksonLopez.DomainPrimitives;

/// <summary>
/// Provides extension methods for converting collections of raw values into collections of domain primitives.
/// </summary>
public static class PrimitiveCollectionExtensions
{
    /// <summary>
    /// Creates a <see cref="List{T}"/> of domain primitives by creating each element from the corresponding raw value.
    /// </summary>
    /// <remarks>
    /// Each element is created via <see cref="IDomainPrimitive{TSelf, TValue}.Create"/>, which applies
    /// normalization and validation. Creation stops at the first invalid value.
    /// </remarks>
    /// <typeparam name="TPrimitive">The domain primitive struct type to create.</typeparam>
    /// <typeparam name="TValue">The raw backing value type.</typeparam>
    /// <param name="values">The sequence of raw values to convert.</param>
    /// <returns>
    /// A new <see cref="List{T}"/> containing one validated domain primitive for each element in <paramref name="values"/>.
    /// Never returns <see langword="null"/> but may be empty if the source sequence is empty.
    /// </returns>
    /// <exception cref="DomainPrimitiveValidationException">Any element in <paramref name="values"/> fails the domain validation rules.</exception>
    public static List<TPrimitive> ToDomainPrimitiveList<TPrimitive, TValue>(
        this IEnumerable<TValue> values)
#if NET7_0_OR_GREATER
        where TPrimitive : struct, IDomainPrimitive<TPrimitive, TValue>
#else
        where TPrimitive : struct, IDomainPrimitive<TPrimitive>
#endif
        where TValue : notnull
    {
#if NET7_0_OR_GREATER
        var list = new List<TPrimitive>(values is ICollection<TValue> collection ? collection.Count : 4);
        foreach (var value in values)
        {
            list.Add(TPrimitive.Create(value));
        }
        return list;
#else
        throw new System.NotSupportedException("This feature requires .NET 7 or greater.");
#endif
    }

    /// <summary>
    /// Creates an array of domain primitives by creating each element from the corresponding raw value.
    /// </summary>
    /// <remarks>
    /// For sequences that implement <see cref="System.Collections.Generic.ICollection{T}"/>, the output array
    /// is pre-allocated to the exact count. For other enumerables, elements are buffered before conversion.
    /// </remarks>
    /// <typeparam name="TPrimitive">The domain primitive struct type to create.</typeparam>
    /// <typeparam name="TValue">The raw backing value type.</typeparam>
    /// <param name="values">The sequence of raw values to convert.</param>
    /// <returns>
    /// A new array containing one validated domain primitive for each element in <paramref name="values"/>.
    /// Never returns <see langword="null"/> but may be empty if the source sequence is empty.
    /// </returns>
    /// <exception cref="DomainPrimitiveValidationException">Any element in <paramref name="values"/> fails the domain validation rules.</exception>
    public static TPrimitive[] ToDomainPrimitiveArray<TPrimitive, TValue>(
        this IEnumerable<TValue> values)
#if NET7_0_OR_GREATER
        where TPrimitive : struct, IDomainPrimitive<TPrimitive, TValue>
#else
        where TPrimitive : struct, IDomainPrimitive<TPrimitive>
#endif
        where TValue : notnull
    {
#if NET7_0_OR_GREATER
        if (values is ICollection<TValue> collection)
        {
            var array = new TPrimitive[collection.Count];
            int i = 0;
            foreach (var value in collection)
            {
                array[i++] = TPrimitive.Create(value);
            }
            return array;
        }

        // Fallback for non-ICollection enumerables: enumerate once into a list, then convert.
        // Using a loop instead of LINQ to avoid iterator allocation overhead.
        var list = new List<TPrimitive>(capacity: 8);
        foreach (var value in values)
            list.Add(TPrimitive.Create(value));
        return list.ToArray();
#else
        throw new System.NotSupportedException("This feature requires .NET 7 or greater.");
#endif
    }

#if NET7_0_OR_GREATER
    /// <summary>
    /// Creates an array of domain primitives by creating each element from the corresponding raw value in a <see cref="System.ReadOnlySpan{T}"/>.
    /// </summary>
    /// <remarks>
    /// Prefer this overload over <see cref="ToDomainPrimitiveArray{TPrimitive,TValue}(IEnumerable{TValue})"/>
    /// when the source is already a span, as it avoids enumeration overhead.
    /// This overload is only available on .NET 7 and later.
    /// </remarks>
    /// <typeparam name="TPrimitive">The domain primitive struct type to create.</typeparam>
    /// <typeparam name="TValue">The raw backing value type.</typeparam>
    /// <param name="values">The span of raw values to convert.</param>
    /// <returns>
    /// A new array containing one validated domain primitive for each element in <paramref name="values"/>.
    /// Never returns <see langword="null"/> but may be empty if <paramref name="values"/> is empty.
    /// </returns>
    /// <exception cref="DomainPrimitiveValidationException">Any element in <paramref name="values"/> fails the domain validation rules.</exception>
    public static TPrimitive[] ToDomainPrimitiveArray<TPrimitive, TValue>(
        this System.ReadOnlySpan<TValue> values)
        where TPrimitive : struct, IDomainPrimitive<TPrimitive, TValue>
        where TValue : notnull
    {
        var array = new TPrimitive[values.Length];
        for (int i = 0; i < values.Length; i++)
        {
            array[i] = TPrimitive.Create(values[i]);
        }
        return array;
    }
#endif
}
