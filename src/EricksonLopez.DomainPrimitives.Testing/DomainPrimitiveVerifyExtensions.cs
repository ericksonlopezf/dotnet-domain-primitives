using System;
using System.Linq;
using System.Reflection;
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
    /// rather than as a complex object with a 'Value' property.
    /// Call this once during test initialization (e.g., in a static constructor or [ModuleInitializer]).
    /// </summary>
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

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal sealed class DomainPrimitiveVerifyJsonConverter : Argon.JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType.IsValueType && 
               objectType.GetInterfaces().Any(i => 
                   i.IsGenericType && 
                   i.GetGenericTypeDefinition() == typeof(IDomainPrimitive<,>));
    }

    public override void WriteJson(Argon.JsonWriter writer, object value, Argon.JsonSerializer serializer)
    {
        var type = value.GetType();
        var prop = type.GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
        if (prop != null)
        {
            var innerValue = prop.GetValue(value);
            serializer.Serialize(writer, innerValue);
        }
        else
        {
            writer.WriteNull();
        }
    }

    public override object? ReadJson(Argon.JsonReader reader, Type objectType, object? existingValue, Argon.JsonSerializer serializer)
    {
        throw new NotImplementedException("Deserialization is not needed for Verify snapshot generation.");
    }
}
