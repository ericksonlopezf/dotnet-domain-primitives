// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EricksonLopez.DomainPrimitives.NewtonsoftJson;

/// <summary>
/// A non-generic Newtonsoft.Json <see cref="JsonConverter"/> that can convert any domain primitive type.
/// </summary>
[RequiresDynamicCode("Newtonsoft.Json uses reflection and is not compatible with NativeAOT.")]
public class DomainPrimitiveUniversalNewtonsoftJsonConverter : JsonConverter
{
    private static readonly ConcurrentDictionary<Type, PrimitiveTypeMetadata?> Cache = new();

    internal static void ClearCache() => Cache.Clear();

    private sealed record PrimitiveTypeMetadata(
        Type PrimitiveType,
        Type? ValueType,
        PropertyInfo? ValueProperty,
        PropertyInfo? IsDefaultProperty,
        MethodInfo? TryCreateMethod,
        MethodInfo? CreateMethod,
        MethodInfo? ParseMethod,
        bool IsValueObject);

    /// <inheritdoc />
    public override bool CanConvert(Type objectType)
    {
        return GetMetadata(objectType) is not null;
    }

    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is null)
        {
            writer.WriteNull();
            return;
        }

        var meta = GetMetadata(value.GetType());
        if (meta is null)
        {
            writer.WriteNull();
            return;
        }

        if (meta.IsValueObject)
        {
            // For ValueObject, serialize properties into a JSON object
            var jo = new JObject();
            foreach (var prop in meta.PrimitiveType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.Name is "IsDefault") continue;
                var propVal = prop.GetValue(value);
                jo.Add(prop.Name, propVal is null ? JValue.CreateNull() : JToken.FromObject(propVal, serializer));
            }
            jo.WriteTo(writer);
            return;
        }

        if (meta.IsDefaultProperty is not null)
        {
            var isDefault = (bool)meta.IsDefaultProperty.GetValue(value)!;
            if (isDefault)
            {
                writer.WriteNull();
                return;
            }
        }

        if (meta.ValueProperty is not null)
        {
            var rawValue = meta.ValueProperty.GetValue(value);
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
            return;
        }

        writer.WriteNull();
    }

    /// <inheritdoc />
    public override object? ReadJson(
        JsonReader reader,
        Type objectType,
        object? existingValue,
        JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return Activator.CreateInstance(objectType);
        }

        var meta = GetMetadata(objectType);
        if (meta is null)
        {
            return serializer.Deserialize(reader, objectType);
        }

        if (meta.IsValueObject)
        {
            var jo = JObject.Load(reader);
            var json = jo.ToString(Formatting.None);

            if (meta.ParseMethod is not null)
            {
                try
                {
                    var parseParams = meta.ParseMethod.GetParameters();
                    if (parseParams.Length == 2)
                    {
                        return meta.ParseMethod.Invoke(null, new object?[] { json, null });
                    }
                    return meta.ParseMethod.Invoke(null, new object?[] { json });
                }
                catch (TargetInvocationException ex) when (ex.InnerException is not null)
                {
                    throw new JsonSerializationException($"Invalid {objectType.Name}: {ex.InnerException.Message}", ex.InnerException);
                }
            }

            // Fallback: populate object via property matching
            var instance = Activator.CreateInstance(objectType);
            foreach (var prop in objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.Name is "IsDefault" || !prop.CanWrite) continue;
                var token = jo.GetValue(prop.Name, StringComparison.OrdinalIgnoreCase);
                if (token is not null)
                {
                    var propVal = token.ToObject(prop.PropertyType, serializer);
                    prop.SetValue(instance, propVal);
                }
            }
            return instance;
        }

        var valueType = meta.ValueType ?? typeof(string);
        object? rawValue = valueType == typeof(DateOnly)
            ? (reader.TokenType == JsonToken.Date && reader.Value is DateTime dt
                ? DateOnly.FromDateTime(dt)
                : reader.TokenType == JsonToken.Date && reader.Value is DateTimeOffset dto
                    ? DateOnly.FromDateTime(dto.DateTime)
                    : reader.Value is string dateStr && DateOnly.TryParse(dateStr, CultureInfo.InvariantCulture, out var parsedDate)
                        ? parsedDate
                        : serializer.Deserialize(reader, valueType))
            : valueType == typeof(TimeOnly)
                ? (reader.TokenType == JsonToken.Date && reader.Value is DateTime dtTime
                    ? TimeOnly.FromDateTime(dtTime)
                    : reader.Value is string timeStr && TimeOnly.TryParse(timeStr, CultureInfo.InvariantCulture, out var parsedTime)
                        ? parsedTime
                        : serializer.Deserialize(reader, valueType))
                : serializer.Deserialize(reader, valueType);

        if (rawValue is null)
        {
            return Activator.CreateInstance(objectType);
        }

        if (meta.TryCreateMethod is not null)
        {
            var parameters = new object?[] { rawValue, null, null };
            var success = (bool)meta.TryCreateMethod.Invoke(null, parameters)!;
            if (success)
            {
                return parameters[1];
            }

            var error = (Validation.PrimitiveError)parameters[2]!;
            throw new JsonSerializationException($"Invalid {objectType.Name}: {error.Message}");
        }

        if (meta.CreateMethod is not null)
        {
            try
            {
                return meta.CreateMethod.Invoke(null, new object?[] { rawValue });
            }
            catch (TargetInvocationException ex) when (ex.InnerException is not null)
            {
                throw new JsonSerializationException($"Invalid {objectType.Name}: {ex.InnerException.Message}", ex.InnerException);
            }
        }

        throw new JsonSerializationException($"Type {objectType.Name} does not define a suitable Create or TryCreate factory method.");
    }

    private static PrimitiveTypeMetadata? GetMetadata(Type type)
    {
        return Cache.GetOrAdd(type, static t =>
        {
            if (!t.IsValueType) return null;

            var isValueObject = Attribute.GetCustomAttributes(t).Any(static a => a.GetType().Name == "ValueObjectAttribute");
            var isPrimitive = isValueObject
                || Attribute.GetCustomAttributes(t).Any(static a => a.GetType().Name is "StringPrimitiveAttribute" or "NumericPrimitiveAttribute`1" or "DatePrimitiveAttribute"
                    or "StrongIdAttribute`1" or "SmartEnumAttribute`1"
                    or "EmailAttribute" or "PhoneAttribute" or "UrlAttribute" or "MoneyAttribute"
                    or "PercentageAttribute" or "BirthDateAttribute" or "ExpirationDateAttribute")
                || t.GetInterfaces().Any(static i => i.Name.StartsWith("IDomainPrimitive", StringComparison.Ordinal));

            if (!isPrimitive) return null;

            var valProp = t.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
            var valType = valProp?.PropertyType;
            var isDefaultProp = t.GetProperty("IsDefault", BindingFlags.Public | BindingFlags.Instance);

            var tryCreate = valType is not null
                ? t.GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .FirstOrDefault(m => m.Name == "TryCreate" && m.GetParameters().Length == 3)
                : null;

            var create = valType is not null
                ? t.GetMethod("Create", BindingFlags.Public | BindingFlags.Static, null, new[] { valType }, null)
                : null;

            var parse = t.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m => m.Name == "Parse" && m.GetParameters().Length >= 1 && m.GetParameters()[0].ParameterType == typeof(string));

            return new PrimitiveTypeMetadata(t, valType, valProp, isDefaultProp, tryCreate, create, parse, isValueObject);
        });
    }
}
