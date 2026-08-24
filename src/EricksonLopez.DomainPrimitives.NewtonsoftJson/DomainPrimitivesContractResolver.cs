// Copyright © Erickson Lopez. MIT License.
using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json.Serialization;

namespace EricksonLopez.DomainPrimitives.NewtonsoftJson;

/// <summary>
/// A <see cref="DefaultContractResolver"/> that automatically attaches domain primitive JSON converters
/// to all types decorating or implementing domain primitives.
/// </summary>
[RequiresDynamicCode("Newtonsoft.Json uses reflection and is not compatible with NativeAOT.")]
public class DomainPrimitivesContractResolver : DefaultContractResolver
{
    private static readonly DomainPrimitiveUniversalNewtonsoftJsonConverter UniversalConverter = new();

    /// <inheritdoc />
    protected override JsonContract CreateContract(Type objectType)
    {
        var contract = base.CreateContract(objectType);

        if (UniversalConverter.CanConvert(objectType))
        {
            contract.Converter = UniversalConverter;
        }

        return contract;
    }
}
