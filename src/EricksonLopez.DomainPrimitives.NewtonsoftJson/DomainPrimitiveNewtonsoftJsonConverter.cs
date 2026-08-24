// Copyright © Erickson Lopez. MIT License.
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using Newtonsoft.Json;

namespace EricksonLopez.DomainPrimitives.NewtonsoftJson;

/// <summary>
/// A strongly typed Newtonsoft.Json <see cref="JsonConverter{T}"/> for domain primitives.
/// </summary>
/// <typeparam name="TPrimitive">The domain primitive type.</typeparam>
/// <typeparam name="TValue">The underlying value type.</typeparam>
[RequiresDynamicCode("Newtonsoft.Json uses reflection and is not compatible with NativeAOT.")]
public class DomainPrimitiveNewtonsoftJsonConverter<TPrimitive, TValue> : JsonConverter<TPrimitive>
    where TPrimitive : struct
{
    private static readonly MethodInfo? TryCreateMethod = typeof(TPrimitive).GetMethod(
        "TryCreate",
        BindingFlags.Public | BindingFlags.Static,
        null,
        new[] { typeof(TValue), typeof(TPrimitive).MakeByRefType(), typeof(Validation.PrimitiveError).MakeByRefType() },
        null);

    private static readonly MethodInfo? CreateMethod = typeof(TPrimitive).GetMethod(
        "Create",
        BindingFlags.Public | BindingFlags.Static,
        null,
        new[] { typeof(TValue) },
        null);

    private static readonly PropertyInfo? IsDefaultProperty = typeof(TPrimitive).GetProperty(
        "IsDefault",
        BindingFlags.Public | BindingFlags.Instance);

    private static readonly PropertyInfo? ValueProperty = typeof(TPrimitive).GetProperty(
        "Value",
        BindingFlags.Public | BindingFlags.Instance);

    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, TPrimitive value, JsonSerializer serializer)
    {
        if (IsDefaultProperty is not null)
        {
            var isDefault = (bool)IsDefaultProperty.GetValue(value)!;
            if (isDefault)
            {
                writer.WriteNull();
                return;
            }
        }

        if (ValueProperty is not null)
        {
            var rawValue = ValueProperty.GetValue(value);
            if (rawValue is null)
            {
                writer.WriteNull();
            }
            else if (rawValue is DateOnly dateOnly)
            {
                writer.WriteValue(dateOnly.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            }
            else if (rawValue is TimeOnly timeOnly)
            {
                writer.WriteValue(timeOnly.ToString("HH:mm:ss", CultureInfo.InvariantCulture));
            }
            else
            {
                writer.WriteValue(rawValue);
            }
        }
        else
        {
            writer.WriteNull();
        }
    }

    /// <inheritdoc />
    public override TPrimitive ReadJson(
        JsonReader reader,
        Type objectType,
        TPrimitive existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return default;
        }

        object? rawValue = typeof(TValue) == typeof(DateOnly)
            ? (reader.TokenType == JsonToken.Date && reader.Value is DateTime dt
                ? DateOnly.FromDateTime(dt)
                : reader.TokenType == JsonToken.Date && reader.Value is DateTimeOffset dto
                    ? DateOnly.FromDateTime(dto.DateTime)
                    : reader.Value is string dateStr && DateOnly.TryParse(dateStr, CultureInfo.InvariantCulture, out var parsedDate)
                        ? parsedDate
                        : serializer.Deserialize<TValue>(reader))
            : typeof(TValue) == typeof(TimeOnly)
                ? (reader.TokenType == JsonToken.Date && reader.Value is DateTime dtTime
                    ? TimeOnly.FromDateTime(dtTime)
                    : reader.Value is string timeStr && TimeOnly.TryParse(timeStr, CultureInfo.InvariantCulture, out var parsedTime)
                        ? parsedTime
                        : serializer.Deserialize<TValue>(reader))
                : serializer.Deserialize<TValue>(reader);

        if (rawValue is null)
        {
            return default;
        }

        if (TryCreateMethod is not null)
        {
            var parameters = new object?[] { rawValue, null, null };
            var success = (bool)TryCreateMethod.Invoke(null, parameters)!;
            if (success)
            {
                return (TPrimitive)parameters[1]!;
            }

            var error = (Validation.PrimitiveError)parameters[2]!;
            throw new JsonSerializationException($"Invalid {typeof(TPrimitive).Name}: {error.Message}");
        }

        if (CreateMethod is not null)
        {
            try
            {
                return (TPrimitive)CreateMethod.Invoke(null, new object?[] { rawValue })!;
            }
            catch (TargetInvocationException ex) when (ex.InnerException is not null)
            {
                throw new JsonSerializationException($"Invalid {typeof(TPrimitive).Name}: {ex.InnerException.Message}", ex.InnerException);
            }
        }

        throw new JsonSerializationException($"Type {typeof(TPrimitive).Name} does not define a suitable Create or TryCreate factory method.");
    }
}
