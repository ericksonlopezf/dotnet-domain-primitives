// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using Microsoft.CodeAnalysis;
using System.Threading.Tasks;

namespace EricksonLopez.DomainPrimitives.Generators;

internal static class GeneratorHelpers
{
    public static string ResolveSpecialType(ITypeSymbol type)
    {
        return type.SpecialType switch
        {
            SpecialType.System_String => "string",
            SpecialType.System_Int32 => "int",
            SpecialType.System_Int64 => "long",
            SpecialType.System_Int16 => "short",
            SpecialType.System_Byte => "byte",
            SpecialType.System_UInt32 => "uint",
            SpecialType.System_UInt64 => "ulong",
            SpecialType.System_UInt16 => "ushort",
            SpecialType.System_SByte => "sbyte",
            SpecialType.System_Single => "float",
            SpecialType.System_Double => "double",
            SpecialType.System_Decimal => "decimal",
            SpecialType.System_Boolean => "bool",
            SpecialType.System_Char => "char",
            SpecialType.System_Object => "object",
            _ => type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Replace("global::", "")
        };
    }

    public static Models.AssemblyDefaultsInfo ExtractAssemblyDefaults(Compilation compilation)
    {
        bool trim = false;
        bool notEmpty = false;
        int? maxLength = null;
        string? exceptionTypeFullName = null;

        foreach (var attr in compilation.Assembly.GetAttributes())
        {
            var attrClass = attr.AttributeClass;
            if (attrClass?.Name is "DomainPrimitivesDefaultsAttribute" or "DomainPrimitivesDefaults" &&
                attrClass.ContainingNamespace?.ToDisplayString().StartsWith("EricksonLopez.DomainPrimitives", StringComparison.Ordinal) == true)
            {
                foreach (var named in attr.NamedArguments)
                {
                    if (named.Key == "Trim" && named.Value.Value is bool t)
                        trim = t;
                    else if (named.Key == "NotEmpty" && named.Value.Value is bool ne)
                        notEmpty = ne;
                    else if (named.Key == "MaxLength" && named.Value.Value is int ml)
                        maxLength = ml;
                    else if (named.Key == "ExceptionType" && named.Value.Value is INamedTypeSymbol excType)
                        exceptionTypeFullName = excType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                }
            }
        }

        return new Models.AssemblyDefaultsInfo(trim, notEmpty, maxLength, exceptionTypeFullName);
    }

    public static void GenerateJsonConverter(SourceBuilder sb, string typeName, string backingType)
    {
        sb.AppendLine("// ─── JSON Serialization ───────────────────────────────────────────────");
        sb.AppendLine();
        sb.AppendLine($"private sealed class {typeName}JsonConverter : global::System.Text.Json.Serialization.JsonConverter<{typeName}>");
        sb.OpenBrace();
        sb.AppendLine($"public override {typeName} Read(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)");
        sb.OpenBrace();
        
        if (backingType == "string")
        {
            // Zero-allocation hot path: reads UTF-8 bytes directly from the reader span.
            // HasValueSequence is true only for extremely large JSON strings (>16KB) that straddle
            // multiple buffer segments — an exceedingly rare scenario in practice.
            sb.AppendLine("#if NET8_0_OR_GREATER");
            sb.AppendLine("// Hot path: zero-allocation via ValueSpan (only fires when HasValueSequence=false).");
            sb.AppendLine("if (!reader.HasValueSequence)");
            sb.OpenBrace();
            sb.AppendLine($"if ({typeName}.TryParse(reader.ValueSpan, null, out var spanResult)) return spanResult;");
            sb.AppendLine($"throw new global::System.Text.Json.JsonException($\"Invalid {typeName}: unable to parse value.\");");
            sb.CloseBrace();
            sb.AppendLine("// Rare fallback: value spans multiple segments (>16KB JSON string). Accepts one allocation.");
            sb.AppendLine("var stringValue = reader.GetString();");
            sb.AppendLine("if (stringValue is null) return default;");
            sb.AppendLine($"if ({typeName}.TryCreate(stringValue, out var result, out var err)) return result;");
            sb.AppendLine($"throw new global::System.Text.Json.JsonException($\"Invalid {typeName}: {{err.Message}}\");");
            sb.AppendLine("#else");
            sb.AppendLine("// Fallback for older TFMs: allocates a string.");
            sb.AppendLine("var stringValue = reader.GetString();");
            sb.AppendLine("if (stringValue is null) return default;");
            sb.AppendLine($"if ({typeName}.TryCreate(stringValue, out var result, out var err)) return result;");
            sb.AppendLine($"throw new global::System.Text.Json.JsonException($\"Invalid {typeName}: {{err.Message}}\");");
            sb.AppendLine("#endif");
        }
        else if (backingType == "int")
        {
            sb.AppendLine($"if (!reader.TryGetInt32(out var value)) throw new global::System.Text.Json.JsonException($\"Invalid {typeName}: expected int.\");");
            sb.AppendLine($"if ({typeName}.TryCreate(value, out var result, out var err)) return result;");
            sb.AppendLine($"throw new global::System.Text.Json.JsonException($\"Invalid {typeName}: {{err.Message}}\");");
        }
        else if (backingType == "long")
        {
            sb.AppendLine($"if (!reader.TryGetInt64(out var value)) throw new global::System.Text.Json.JsonException($\"Invalid {typeName}: expected long.\");");
            sb.AppendLine($"if ({typeName}.TryCreate(value, out var result, out var err)) return result;");
            sb.AppendLine($"throw new global::System.Text.Json.JsonException($\"Invalid {typeName}: {{err.Message}}\");");
        }
        else if (backingType == "decimal")
        {
            sb.AppendLine($"if (!reader.TryGetDecimal(out var value)) throw new global::System.Text.Json.JsonException($\"Invalid {typeName}: expected decimal.\");");
            sb.AppendLine($"if ({typeName}.TryCreate(value, out var result, out var err)) return result;");
            sb.AppendLine($"throw new global::System.Text.Json.JsonException($\"Invalid {typeName}: {{err.Message}}\");");
        }
        else if (backingType is "Guid" or "global::Guid" or "System.Guid" or "global::System.Guid")
        {
            sb.AppendLine($"if (!reader.TryGetGuid(out var value)) throw new global::System.Text.Json.JsonException($\"Invalid {typeName}: expected Guid.\");");
            sb.AppendLine($"if ({typeName}.TryCreate(value, out var result, out var err)) return result;");
            sb.AppendLine($"throw new global::System.Text.Json.JsonException($\"Invalid {typeName}: {{err.Message}}\");");
        }
        else if (backingType is "DateTime" or "global::DateTime" or "System.DateTime" or "global::System.DateTime")
        {
            sb.AppendLine($"if (!reader.TryGetDateTime(out var value)) throw new global::System.Text.Json.JsonException($\"Invalid {typeName}: expected DateTime.\");");
            sb.AppendLine($"if ({typeName}.TryCreate(value, out var result, out var err)) return result;");
            sb.AppendLine($"throw new global::System.Text.Json.JsonException($\"Invalid {typeName}: {{err.Message}}\");");
        }
        else if (backingType is "DateTimeOffset" or "global::DateTimeOffset" or "System.DateTimeOffset" or "global::System.DateTimeOffset")
        {
            sb.AppendLine($"if (!reader.TryGetDateTimeOffset(out var value)) throw new global::System.Text.Json.JsonException($\"Invalid {typeName}: expected DateTimeOffset.\");");
            sb.AppendLine($"if ({typeName}.TryCreate(value, out var result, out var err)) return result;");
            sb.AppendLine($"throw new global::System.Text.Json.JsonException($\"Invalid {typeName}: {{err.Message}}\");");
        }
        else
        {
            // Fallback for types that STJ doesn't have direct Read methods for
            sb.AppendLine($"var value = global::System.Text.Json.JsonSerializer.Deserialize<{backingType}>(ref reader, options);");
            
            sb.AppendLine($"if ({typeName}.TryCreate(value, out var result, out var err)) return result;");
            sb.AppendLine($"throw new global::System.Text.Json.JsonException($\"Invalid {typeName}: {{err.Message}}\");");
        }
        
        sb.CloseBrace();
        sb.AppendLine();
        
        sb.AppendLine($"public override void Write(global::System.Text.Json.Utf8JsonWriter writer, {typeName} value, global::System.Text.Json.JsonSerializerOptions options)");
        sb.OpenBrace();
        if (backingType == "string")
        {
            sb.AppendLine("writer.WriteStringValue(value.Value);");
        }
        else if (backingType == "int" || backingType == "long" || backingType == "decimal" || backingType == "float" || backingType == "double")
        {
            sb.AppendLine("writer.WriteNumberValue(value.Value);");
        }
        else if (backingType == "bool")
        {
            sb.AppendLine("writer.WriteBooleanValue(value.Value);");
        }
        else
        {
            sb.AppendLine($"global::System.Text.Json.JsonSerializer.Serialize(writer, value.Value, options);");
        }
        sb.CloseBrace();
        
        sb.CloseBrace();
        sb.AppendLine();
    }
}



