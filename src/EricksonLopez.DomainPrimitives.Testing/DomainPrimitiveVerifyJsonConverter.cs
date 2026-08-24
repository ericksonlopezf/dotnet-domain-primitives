// Copyright © Erickson Lopez. MIT License.
using System;
using System.Linq;
using System.Reflection;

namespace EricksonLopez.DomainPrimitives.Testing;

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
