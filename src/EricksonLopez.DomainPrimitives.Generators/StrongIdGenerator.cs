using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Immutable;
using EricksonLopez.DomainPrimitives.Generators.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EricksonLopez.DomainPrimitives.Generators;

/// <summary>
/// Incremental source generator for <c>[StrongId&lt;T&gt;]</c> domain primitives.
/// </summary>
/// <remarks>
/// Generates a complete strong ID implementation including:
/// factory methods, parsing, formatting, comparison, operators, and interface implementations.
/// </remarks>
[Generator(LanguageNames.CSharp)]
internal sealed class StrongIdGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // TD-014: ForAttributeWithMetadataName for [StrongId<T>] — single trigger attribute.
        var provider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.StrongIdFqn,
                predicate: static (node, ct) => EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.IsReadonlyRecordStruct(node, ct),
                transform: static (ctx, ct) => ExtractTypeInfo(ctx.SemanticModel, (RecordDeclarationSyntax)ctx.TargetNode, ct))
            .Where(static info => info is not null)
            .Select(static (info, _) => info!);

        context.RegisterSourceOutput(provider, static (spc, info) =>
        {
            var source = GenerateStrongId(info);
            spc.AddSource($"{info.TypeName}.g.cs", source);
        });
    }

    private static StrongIdTypeInfo? ExtractTypeInfo(
        GeneratorSyntaxContext context,
        CancellationToken ct)
        => ExtractTypeInfo(context.SemanticModel, (RecordDeclarationSyntax)context.Node, ct);

    private static StrongIdTypeInfo? ExtractTypeInfo(
        SemanticModel semanticModel,
        RecordDeclarationSyntax recordSyntax,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var typeSymbol = semanticModel.GetDeclaredSymbol(recordSyntax, ct) as INamedTypeSymbol;
        if (typeSymbol is null)
            return null;

        // Find the StrongIdAttribute<T>
        AttributeData? strongIdAttr = null;
        foreach (var attr in typeSymbol.GetAttributes())
        {
            if (attr.AttributeClass is INamedTypeSymbol attrClass &&
                attrClass.Name == "StrongIdAttribute" &&
                attrClass.TypeArguments.Length == 1)
            {
                strongIdAttr = attr;
                break;
            }
        }

        if (strongIdAttr?.AttributeClass is not { TypeArguments.Length: 1 } attrType)
            return null;

        var backingType = attrType.TypeArguments[0];

        // Validate and resolve backing type using GeneratorHelpers
        string? backingFullName = GeneratorHelpers.ResolveSpecialType(backingType);
        
        if (backingFullName != "int" && backingFullName != "long" && backingFullName != "string" && backingFullName != "System.Guid")
            return null;

        // Extract containing types for nested type support
        var containingType = typeSymbol.ContainingType;
        var containingList = new System.Collections.Generic.List<string>();
        while (containingType is not null)
        {
            containingList.Insert(0, containingType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
            containingType = containingType.ContainingType;
        }

        // RFC-0002: RejectEmpty defaults to true (matching StrongIdAttribute.RejectEmpty = true).
        // We only override if the user explicitly sets RejectEmpty = false.
        bool rejectEmpty = true;
        if (strongIdAttr != null)
        {
            foreach (var named in strongIdAttr.NamedArguments)
            {
                if (named.Key == "RejectEmpty" && named.Value.Value is bool b)
                    rejectEmpty = b;
            }
        }

        return new StrongIdTypeInfo(
            Namespace: typeSymbol.ContainingNamespace.ToDisplayString(),
            TypeName: typeSymbol.Name,
            BackingTypeName: backingType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
            BackingTypeFullName: backingFullName,
            Accessibility: typeSymbol.DeclaredAccessibility switch
            {
                Accessibility.Public => "public",
                Accessibility.Internal => "internal",
                Accessibility.Protected => "protected",
                Accessibility.Private => "private",
                Accessibility.ProtectedOrInternal => "protected internal",
                Accessibility.ProtectedAndInternal => "private protected",
                _ => "public"
            },
            ContainingTypes: new EquatableArray<string>(containingList.ToImmutableArray()),
            RejectEmpty: rejectEmpty);
    }

    private static string GenerateStrongId(StrongIdTypeInfo info)
    {
        var sb = new SourceBuilder();

        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("#nullable enable");
        sb.AppendLine();
        sb.AppendLine("using System;");
        sb.AppendLine("using System.ComponentModel;");
        sb.AppendLine("using System.Diagnostics.CodeAnalysis;");
        sb.AppendLine("using System.Globalization;");
        sb.AppendLine("using System.Runtime.CompilerServices;");
        sb.AppendLine("using EricksonLopez.DomainPrimitives;");
        sb.AppendLine("using EricksonLopez.DomainPrimitives.Validation;");
        sb.AppendLine();

        sb.AppendLine($"namespace {info.Namespace};");
        sb.AppendLine();

        // Generate type declaration
        sb.AppendLine($"[global::System.Text.Json.Serialization.JsonConverter(typeof({info.TypeName}JsonConverter))]");
        sb.AppendLine($"[global::System.ComponentModel.TypeConverter(typeof({info.TypeName}TypeConverter))]");
        sb.AppendLine($"[global::System.Diagnostics.DebuggerDisplay(\"{{{info.TypeName}}}({{IsDefault ? \\\"<default>\\\" : _value}})\")]");
        sb.AppendLine($"[global::System.Diagnostics.DebuggerTypeProxy(typeof({info.TypeName}DebugView))]");
        sb.AppendLine($"[global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Auto)]");
        sb.AppendLine($"{info.Accessibility} readonly partial record struct {info.TypeName} :");
        sb.IncreaseIndent();
        sb.AppendLine($"IStrongId<{info.TypeName}, {info.BackingTypeName}>,");
        sb.AppendLine($"IParsable<{info.TypeName}>,");
        sb.AppendLine($"ISpanParsable<{info.TypeName}>,");
        sb.AppendLine("#if NET8_0_OR_GREATER");
        sb.AppendLine($"IUtf8SpanParsable<{info.TypeName}>,");
        sb.AppendLine("#endif");
        sb.AppendLine($"IFormattable,");
        sb.AppendLine($"ISpanFormattable,");
        sb.AppendLine("#if NET8_0_OR_GREATER");
        sb.AppendLine($"IUtf8SpanFormattable,");
        sb.AppendLine("#endif");
        sb.AppendLine($"IComparable<{info.TypeName}>,");
        sb.AppendLine($"IComparable,");

        // Generic math comparison operators
        sb.AppendLine($"System.Numerics.IEqualityOperators<{info.TypeName}, {info.TypeName}, bool>,");
        sb.AppendLine($"System.Numerics.IComparisonOperators<{info.TypeName}, {info.TypeName}, bool>");
        sb.DecreaseIndent();
        sb.OpenBrace();

        // ─── Backing field ───
        sb.AppendLine($"private readonly {info.BackingTypeName} _value;");
        sb.AppendLine("private readonly bool _isInitialized;");
        sb.AppendLine();

        // ─── Value property ───
        sb.AppendLine("/// <summary>Returns true if this instance was created via default(T) rather than via Create().</summary>");
        sb.AppendLine("public bool IsDefault => !_isInitialized;");
        sb.AppendLine();
        sb.AppendLine("/// <summary>The underlying primitive value.</summary>");
        sb.AppendLine("public " + info.BackingTypeName + " Value");
        sb.OpenBrace();
        sb.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine("get");
        sb.OpenBrace();
        sb.AppendLine($"if (IsDefault) throw new InvalidOperationException($\"Value accessed on a default instance of {info.TypeName}. Check IsDefault before accessing Value.\");");
        sb.AppendLine("return _value;");
        sb.CloseBrace();
        sb.CloseBrace();
        sb.AppendLine();

        // ─── PrimitiveName ───
        sb.AppendLine("/// <inheritdoc/>");
        sb.AppendLine($"public static string PrimitiveName => \"{info.TypeName}\";");
        sb.AppendLine();

        // Error Constants
        sb.AppendLine("/// <summary>Canonical error codes for this primitive.</summary>");
        sb.AppendLine("public static class Errors");
        sb.OpenBrace();
        sb.AppendLine("public const string NullInput = \"NULL_INPUT\";");
        sb.AppendLine("public const string Empty = \"EMPTY\";");
        sb.AppendLine("public const string Format = \"FORMAT\";");
        sb.CloseBrace();
        sb.AppendLine();

        // ─── Private constructor ───
        sb.AppendLine($"private {info.TypeName}({info.BackingTypeName} value)");
        sb.OpenBrace();
        sb.AppendLine("_value = value;");
        sb.AppendLine("_isInitialized = true;");
        sb.CloseBrace();
        sb.AppendLine();

        // ─── Factory methods ───
        GenerateFactoryMethods(sb, info);

        // ─── Parsing ───
        GenerateParsing(sb, info);

        // ─── Formatting ───
        GenerateFormatting(sb, info);

        // ─── Operators ───
        GenerateOperators(sb, info);

        // ─── Comparison and TypeConverter ───
        EricksonLopez.DomainPrimitives.Generators.Shared.ComparisonTemplate.GenerateComparison(sb, info.TypeName, info.IsStringBacked);
        EricksonLopez.DomainPrimitives.Generators.Shared.TypeConverterTemplate.GenerateTypeConverter(sb, info.TypeName, info.BackingTypeName);

        sb.AppendLine($"private sealed class {info.TypeName}DebugView");
        sb.OpenBrace();
        sb.AppendLine($"private readonly {info.TypeName} _t;");
        sb.AppendLine($"public {info.TypeName}DebugView({info.TypeName} t) => _t = t;");
        sb.AppendLine("public string Value => _t.IsDefault ? \"default\" : _t.Value.ToString();");
        sb.CloseBrace();

        GeneratorHelpers.GenerateJsonConverter(sb, info.TypeName, info.BackingTypeName);
        sb.CloseBrace(); // close type

        return sb.ToString();
    }

    private static void GenerateFactoryMethods(SourceBuilder sb, StrongIdTypeInfo info)
    {
        sb.AppendLine("// ─── Factory Methods ─────────────────────────────────────────────");
        sb.AppendLine();

        // New() — only for Guid-backed IDs
        if (info.IsGuidBacked)
        {
            sb.AppendLine("/// <summary>Creates a new, unique identifier.</summary>");
            sb.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
            sb.AppendLine($"public static {info.TypeName} Create()");
            sb.OpenBrace();
            sb.AppendLine("return new(Guid.NewGuid());");
            sb.CloseBrace();
        }
        else if (info.IsIntegerBacked)
        {
            sb.AppendLine("/// <summary>Not supported for integer-backed IDs. Use Create(value) with a known value.</summary>");
            sb.AppendLine($"public static {info.TypeName} Create() => throw new NotSupportedException(\"Cannot generate a new {info.TypeName}. Integer-backed IDs must be assigned by the persistence layer.\");");
        }
        else if (info.IsStringBacked)
        {
            sb.AppendLine("/// <summary>Not supported for string-backed IDs. Use Create(value) with a known value.</summary>");
            sb.AppendLine($"public static {info.TypeName} Create() => throw new NotSupportedException(\"Cannot generate a new {info.TypeName}. String-backed IDs must be assigned explicitly.\");");
        }
        sb.AppendLine();

        // TryValidate
        if (info.RejectEmpty)
        {
            sb.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
            sb.AppendLine($"private static PrimitiveError TryValidate({info.BackingTypeName} value)");
            sb.OpenBrace();
            if (info.IsGuidBacked)
                sb.AppendLine("if (value == Guid.Empty)");
            else if (info.IsStringBacked)
                sb.AppendLine("if (string.IsNullOrWhiteSpace(value))");
            else
                sb.AppendLine("if (value == 0)");
            sb.IncreaseIndent();
            sb.AppendLine($"return new PrimitiveError(\"EMPTY\", \"{info.TypeName} must not be empty.\");");
            sb.DecreaseIndent();
            sb.AppendLine("return PrimitiveError.None;");
            sb.CloseBrace();
            sb.AppendLine();
        }

        // Create()
        sb.AppendLine("/// <summary>Creates a strong ID from an existing value.</summary>");
        if (info.IsStringBacked)
            sb.AppendLine("/// <exception cref=\"ArgumentNullException\">Thrown when the value is null.</exception>");
        sb.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine($"public static {info.TypeName} Create({info.BackingTypeName} value)");
        sb.OpenBrace();
        if (info.IsStringBacked)
            sb.AppendLine("ArgumentNullException.ThrowIfNull(value);");
        if (info.RejectEmpty)
        {
            sb.AppendLine("var error = TryValidate(value);");
            sb.AppendLine("if (error.IsError)");
            sb.IncreaseIndent();
            sb.AppendLine("throw new DomainPrimitiveValidationException(error);");
            sb.DecreaseIndent();
        }
        sb.AppendLine("return new(value);");
        sb.CloseBrace();
        sb.AppendLine();



        sb.AppendLine("/// <summary>Tries to create a valid instance. Returns a boolean indicating success.</summary>");
        sb.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine($"public static bool TryCreate({info.BackingTypeName} value, out {info.TypeName} result, out PrimitiveError validationError)");
        sb.OpenBrace();
        if (info.IsStringBacked)
        {
            sb.AppendLine("if (value is null)");
            sb.OpenBrace();
            sb.AppendLine("result = default;");
            sb.AppendLine($"validationError = new PrimitiveError(\"NULL_INPUT\", \"Value cannot be null.\");");
            sb.AppendLine("return false;");
            sb.CloseBrace();
        }
        if (info.RejectEmpty)
        {
            sb.AppendLine("validationError = TryValidate(value);");
            sb.AppendLine("if (validationError.IsError)");
            sb.OpenBrace();
            sb.AppendLine("result = default;");
            sb.AppendLine("return false;");
            sb.CloseBrace();
        }
        else
        {
            sb.AppendLine("validationError = PrimitiveError.None;");
        }
        sb.AppendLine($"result = new {info.TypeName}(value);");
        sb.AppendLine("return true;");
        sb.CloseBrace();
        sb.AppendLine();

        // Empty
        sb.AppendLine("/// <summary>The empty/uninitialized sentinel.</summary>");
        if (info.IsStringBacked && !info.RejectEmpty)
            sb.AppendLine($"public static {info.TypeName} Empty {{ get => new(string.Empty); }}");
        else
            sb.AppendLine($"public static {info.TypeName} Empty {{ get => default; }}");
        sb.AppendLine();
    }

    private static void GenerateParsing(SourceBuilder sb, StrongIdTypeInfo info)
    {
        sb.AppendLine("// ─── Parsing (IParsable, ISpanParsable, IUtf8SpanParsable) ───────");
        sb.AppendLine();

        // Parse(string)
        sb.AppendLine($"public static {info.TypeName} Parse(string s) => Parse(s, null);");
        sb.AppendLine($"public static {info.TypeName} Parse(string s, IFormatProvider? provider)");
        sb.OpenBrace();
        sb.AppendLine("if (TryParse(s, provider, out var result)) return result;");
        sb.AppendLine($"throw new System.FormatException($\"The value '{{s}}' is not valid for {info.TypeName}.\");");
        sb.CloseBrace();
        sb.AppendLine();

        // TryParse(string)
        sb.AppendLine($"public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out {info.TypeName} result)");
        sb.OpenBrace();
        if (info.IsGuidBacked)
        {
            sb.AppendLine("if (Guid.TryParse(s, out var parsed))");
            sb.OpenBrace();
            sb.AppendLine($"result = new {info.TypeName}(parsed);");
            sb.AppendLine("return true;");
            sb.CloseBrace();
        }
        else if (info.IsIntegerBacked)
        {
            sb.AppendLine($"if ({info.BackingTypeName}.TryParse(s, provider, out var parsed))");
            sb.OpenBrace();
            sb.AppendLine($"result = new {info.TypeName}(parsed);");
            sb.AppendLine("return true;");
            sb.CloseBrace();
        }
        else // string-backed
        {
            // NOTE: For string-backed StrongIds, TryParse returns true for any non-null string,
            // including string.Empty. This is by design — Empty ('') is a valid string ID in some
            // domains (e.g., a sentinel key). If you need non-empty enforcement, use [StringPrimitive]
            // with [NotEmpty] instead of [StrongId<string>].
            sb.AppendLine("if (s is not null)");
            sb.OpenBrace();
            sb.AppendLine($"result = new {info.TypeName}(s);");
            sb.AppendLine("return true;");
            sb.CloseBrace();
        }
        sb.AppendLine($"result = default;");
        sb.AppendLine("return false;");
        sb.CloseBrace();
        sb.AppendLine();

        // Parse(ReadOnlySpan<char>)
        sb.AppendLine($"public static {info.TypeName} Parse(ReadOnlySpan<char> s, IFormatProvider? provider = null)");
        sb.OpenBrace();
        sb.AppendLine($"if (TryParse(s, provider, out var result)) return result;");
        sb.AppendLine("throw new System.FormatException(\"The span value is not valid.\");");
        sb.CloseBrace();
        sb.AppendLine();

        // TryParse(ReadOnlySpan<char>)
        sb.AppendLine($"public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out {info.TypeName} result)");
        sb.OpenBrace();
        if (info.IsGuidBacked)
        {
            sb.AppendLine("if (Guid.TryParse(s, out var parsed))");
        }
        else if (info.IsIntegerBacked)
        {
            sb.AppendLine($"if ({info.BackingTypeName}.TryParse(s, provider, out var parsed))");
        }
        else
        {
            sb.AppendLine("var parsed = s.ToString();");
            sb.AppendLine("if (parsed.Length > 0)");
        }
        sb.OpenBrace();
        sb.AppendLine($"result = new {info.TypeName}(parsed);");
        sb.AppendLine("return true;");
        sb.CloseBrace();
        sb.AppendLine("result = default;");
        sb.AppendLine("return false;");
        sb.CloseBrace();
        sb.AppendLine();

        // Parse(ReadOnlySpan<byte>) — UTF-8
        sb.AppendLine("#if NET8_0_OR_GREATER");
        sb.AppendLine($"public static {info.TypeName} Parse(ReadOnlySpan<byte> utf8, IFormatProvider? provider = null)");
        sb.OpenBrace();
        sb.AppendLine($"if (TryParse(utf8, provider, out var result)) return result;");
        sb.AppendLine("throw new System.FormatException(\"The UTF-8 span value is not valid.\");");
        sb.CloseBrace();
        sb.AppendLine();

        // TryParse(ReadOnlySpan<byte>)
        sb.AppendLine($"public static bool TryParse(ReadOnlySpan<byte> utf8, IFormatProvider? provider, out {info.TypeName} result)");
        sb.OpenBrace();
        if (info.IsGuidBacked)
        {
            sb.AppendLine("if (System.Buffers.Text.Utf8Parser.TryParse(utf8, out Guid parsed, out _))");
        }
        else if (info.IsIntegerBacked)
        {
            sb.AppendLine($"if ({info.BackingTypeName}.TryParse(utf8, provider, out var parsed))");
        }
        else
        {
            sb.AppendLine("int count = System.Text.Encoding.UTF8.GetCharCount(utf8);");
            sb.AppendLine("if (count <= 512)");
            sb.OpenBrace();
            sb.AppendLine("System.Span<char> chars = stackalloc char[count];");
            sb.AppendLine("System.Text.Encoding.UTF8.GetChars(utf8, chars);");
            sb.AppendLine("return TryParse((System.ReadOnlySpan<char>)chars, provider, out result);");
            sb.CloseBrace();
            sb.AppendLine("else");
            sb.OpenBrace();
            sb.AppendLine("var rented = System.Buffers.ArrayPool<char>.Shared.Rent(count);");
            sb.AppendLine("try");
            sb.OpenBrace();
            sb.AppendLine("System.Text.Encoding.UTF8.GetChars(utf8, rented);");
            sb.AppendLine($"return TryParse(rented.AsSpan(0, count), provider, out result);");
            sb.CloseBrace();
            sb.AppendLine("finally");
            sb.OpenBrace();
            sb.AppendLine("System.Buffers.ArrayPool<char>.Shared.Return(rented);");
            sb.CloseBrace();
            sb.CloseBrace();
        }
        if (!info.IsStringBacked)
        {
            sb.OpenBrace();
            sb.AppendLine($"result = new {info.TypeName}(parsed);");
            sb.AppendLine("return true;");
            sb.CloseBrace();
            sb.AppendLine("result = default;");
            sb.AppendLine("return false;");
        }
        sb.CloseBrace();
        sb.AppendLine("#endif");
        sb.AppendLine();
    }

    private static void GenerateFormatting(SourceBuilder sb, StrongIdTypeInfo info)
    {
        sb.AppendLine("// ─── Formatting (IFormattable, ISpanFormattable, IUtf8SpanFormattable) ───");
        sb.AppendLine();

        sb.AppendLine("public void Deconstruct(out " + info.BackingTypeName + " value) => value = _value;");
        sb.AppendLine();
        sb.AppendLine("public override string ToString() => _value.ToString()!;");
        sb.AppendLine();

        sb.AppendLine("public string ToString(string? format, IFormatProvider? formatProvider)");
        if (info.IsGuidBacked || info.IsIntegerBacked)
            sb.AppendLine("    => _value.ToString(format, formatProvider);");
        else
            sb.AppendLine("    => _value.ToString();");
        sb.AppendLine();

        // ISpanFormattable
        sb.AppendLine("public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)");
        if (info.IsGuidBacked)
            sb.AppendLine("    => ((ISpanFormattable)_value).TryFormat(destination, out charsWritten, format, provider);");
        else if (info.IsIntegerBacked)
            sb.AppendLine("    => _value.TryFormat(destination, out charsWritten, format, provider);");
        else
        {
            sb.OpenBrace();
            sb.AppendLine("if (_value is null) { charsWritten = 0; return false; }");
            sb.AppendLine("if (_value.AsSpan().TryCopyTo(destination))");
            sb.OpenBrace();
            sb.AppendLine("charsWritten = _value.Length;");
            sb.AppendLine("return true;");
            sb.CloseBrace();
            sb.AppendLine("charsWritten = 0;");
            sb.AppendLine("return false;");
            sb.CloseBrace();
        }
        sb.AppendLine();

        // IUtf8SpanFormattable
        sb.AppendLine("#if NET8_0_OR_GREATER");
        sb.AppendLine("public bool TryFormat(Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)");
        if (info.IsGuidBacked)
            sb.AppendLine("    => ((IUtf8SpanFormattable)_value).TryFormat(utf8Destination, out bytesWritten, format, provider);");
        else if (info.IsIntegerBacked)
            sb.AppendLine("    => _value.TryFormat(utf8Destination, out bytesWritten, format, provider);");
        else
        {
            sb.OpenBrace();
            sb.AppendLine("if (_value is null) { bytesWritten = 0; return false; }");
            sb.AppendLine("return System.Text.Encoding.UTF8.TryGetBytes(_value.AsSpan(), utf8Destination, out bytesWritten);");
            sb.CloseBrace();
        }
        sb.AppendLine("#endif");
        sb.AppendLine();
    }

    private static void GenerateOperators(SourceBuilder sb, StrongIdTypeInfo info)
    {
        sb.AppendLine("// ─── Operators ────────────────────────────────────────────────────");
        sb.AppendLine();

        // Explicit operator TO backing type
        sb.AppendLine("/// <summary>Explicit conversion to the backing type.</summary>");
        sb.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        if (info.IsStringBacked)
        {
            sb.AppendLine($"public static explicit operator {info.BackingTypeName}({info.TypeName} id) =>");
            sb.AppendLine($"    id._value ?? throw new InvalidOperationException(\"Cannot convert a default {info.TypeName} to {info.BackingTypeName}. Check IsDefault before casting.\");");
        }
        else
        {
            sb.AppendLine($"public static explicit operator {info.BackingTypeName}({info.TypeName} id) => id._value;");
        }
        sb.AppendLine();

        // Explicit operator FROM backing type
        sb.AppendLine("/// <summary>Explicit conversion from the backing type.</summary>");
        sb.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine($"public static explicit operator {info.TypeName}({info.BackingTypeName} value) => new(value);");
        sb.AppendLine();
    }
}



