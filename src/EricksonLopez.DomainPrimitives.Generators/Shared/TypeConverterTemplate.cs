// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives.Generators.Shared;

internal static class TypeConverterTemplate
{
    public static void GenerateTypeConverter(SourceBuilder sb, string typeName, string backingTypeName)
    {
        sb.AppendLine("// ─── TypeConverter ───────────────────────────────────────────────");
        sb.AppendLine();
        sb.AppendLine($"private sealed class {typeName}TypeConverter : global::System.ComponentModel.TypeConverter");
        sb.OpenBrace();

        sb.AppendLine("public override bool CanConvertFrom(global::System.ComponentModel.ITypeDescriptorContext? context, Type sourceType)");
        sb.OpenBrace();
        if (backingTypeName == "string" || backingTypeName == "System.String")
            sb.AppendLine($"return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);");
        else
            sb.AppendLine($"return sourceType == typeof(string) || sourceType == typeof({backingTypeName}) || base.CanConvertFrom(context, sourceType);");
        sb.CloseBrace();
        sb.AppendLine();

        sb.AppendLine("public override object? ConvertFrom(global::System.ComponentModel.ITypeDescriptorContext? context, global::System.Globalization.CultureInfo? culture, object value)");
        sb.OpenBrace();
        if (backingTypeName == "string" || backingTypeName == "System.String")
        {
            sb.AppendLine($"if (value is string s) return {typeName}.Create(s);");
        }
        else
        {
            sb.AppendLine($"if (value is string s) return {typeName}.Parse(s, culture);");
            sb.AppendLine($"if (value is {backingTypeName} v) return {typeName}.Create(v);");
        }
        sb.AppendLine("return base.ConvertFrom(context, culture, value);");
        sb.CloseBrace();
        sb.AppendLine();

        sb.AppendLine("public override bool CanConvertTo(global::System.ComponentModel.ITypeDescriptorContext? context, Type? destinationType)");
        sb.OpenBrace();
        if (backingTypeName == "string" || backingTypeName == "System.String")
            sb.AppendLine($"return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);");
        else
            sb.AppendLine($"return destinationType == typeof(string) || destinationType == typeof({backingTypeName}) || base.CanConvertTo(context, destinationType);");
        sb.CloseBrace();
        sb.AppendLine();

        sb.AppendLine("public override object? ConvertTo(global::System.ComponentModel.ITypeDescriptorContext? context, global::System.Globalization.CultureInfo? culture, object? value, Type destinationType)");
        sb.OpenBrace();
        sb.AppendLine($"if (value is {typeName} id)");
        sb.OpenBrace();
        if (backingTypeName == "string" || backingTypeName == "System.String")
        {
            sb.AppendLine($"if (destinationType == typeof(string)) return id.Value;");
        }
        else
        {
            sb.AppendLine("if (destinationType == typeof(string)) return id.ToString();");
            sb.AppendLine($"if (destinationType == typeof({backingTypeName})) return id.Value;");
        }
        sb.CloseBrace();
        sb.AppendLine("return base.ConvertTo(context, culture, value, destinationType);");
        sb.CloseBrace();

        sb.CloseBrace();
        sb.AppendLine();
    }
}

