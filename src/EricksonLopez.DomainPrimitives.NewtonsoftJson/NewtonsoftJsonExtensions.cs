// Copyright © Erickson Lopez. MIT License.
using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace EricksonLopez.DomainPrimitives.NewtonsoftJson;

/// <summary>
/// Extension methods for configuring Newtonsoft.Json with EricksonLopez.DomainPrimitives converters.
/// </summary>
[RequiresDynamicCode("Newtonsoft.Json uses reflection and is not compatible with NativeAOT.")]
public static class NewtonsoftJsonExtensions
{
    /// <summary>
    /// Adds the universal domain primitive JSON converter to the serializer settings.
    /// </summary>
    /// <param name="settings">The <see cref="JsonSerializerSettings"/> instance.</param>
    /// <returns>The same <see cref="JsonSerializerSettings"/> for chaining.</returns>
    public static JsonSerializerSettings AddDomainPrimitives(this JsonSerializerSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        settings.Converters.Add(new DomainPrimitiveUniversalNewtonsoftJsonConverter());
        return settings;
    }

    /// <summary>
    /// Adds the universal domain primitive JSON converter to the serializer.
    /// </summary>
    /// <param name="serializer">The <see cref="JsonSerializer"/> instance.</param>
    /// <returns>The same <see cref="JsonSerializer"/> for chaining.</returns>
    public static JsonSerializer AddDomainPrimitives(this JsonSerializer serializer)
    {
        ArgumentNullException.ThrowIfNull(serializer);

        serializer.Converters.Add(new DomainPrimitiveUniversalNewtonsoftJsonConverter());
        return serializer;
    }
}
