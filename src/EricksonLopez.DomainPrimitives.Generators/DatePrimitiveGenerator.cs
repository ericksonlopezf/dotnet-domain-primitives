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

[Generator(LanguageNames.CSharp)]
internal sealed class DatePrimitiveGenerator : IIncrementalGenerator
{
    // FQN array for all 10 date-primitive trigger attributes.
    private static readonly string[] TriggerFqns =
    [
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.DatePrimitiveFqn,
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.BirthDateFqn,
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.ExpirationDateFqn,
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.BusinessDateFqn,
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.FiscalYearFqn,
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.MonthFqn,
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.QuarterFqn,
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.WeekFqn,
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.DateRangeFqn,
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.TimeRangeFqn,
    ];

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // TD-014: ForAttributeWithMetadataName with merged+deduplicated pattern.
        IncrementalValuesProvider<DatePrimitiveTypeInfo?> merged = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                TriggerFqns[0],
                predicate: static (node, ct) => EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.IsReadonlyRecordStruct(node, ct),
                transform: static (ctx, ct) => ExtractTypeInfo(ctx.SemanticModel, (RecordDeclarationSyntax)ctx.TargetNode, ct));

        for (int i = 1; i < TriggerFqns.Length; i++)
        {
            var fqn = TriggerFqns[i];
            var additional = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    fqn,
                    predicate: static (node, ct) => EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.IsReadonlyRecordStruct(node, ct),
                    transform: static (ctx, ct) => ExtractTypeInfo(ctx.SemanticModel, (RecordDeclarationSyntax)ctx.TargetNode, ct));
            merged = merged.Collect().Combine(additional.Collect())
                .SelectMany(static (pair, _) =>
                {
                    var list = new System.Collections.Generic.List<DatePrimitiveTypeInfo?>(pair.Left.Length + pair.Right.Length);
                    list.AddRange(pair.Left);
                    list.AddRange(pair.Right);
                    return list;
                });
        }

        var deduped = merged
            .Where(static info => info is not null)
            .Select(static (info, _) => info!)
            .Collect()
            .SelectMany(static (all, _) =>
            {
                var seen = new System.Collections.Generic.HashSet<string>();
                var result = new System.Collections.Generic.List<DatePrimitiveTypeInfo>();
                foreach (var info in all)
                    if (seen.Add($"{info.Namespace}.{info.TypeName}"))
                        result.Add(info);
                return result;
            });

        context.RegisterSourceOutput(deduped, static (spc, info) =>
        {
            var source = GenerateDatePrimitive(info);
            spc.AddSource($"{info.TypeName}.g.cs", source);
        });
    }

    private static DatePrimitiveTypeInfo? ExtractTypeInfo(
        GeneratorSyntaxContext context,
        CancellationToken ct)
        => ExtractTypeInfo(context.SemanticModel, (RecordDeclarationSyntax)context.Node, ct);

    private static DatePrimitiveTypeInfo? ExtractTypeInfo(
        SemanticModel semanticModel,
        RecordDeclarationSyntax recordSyntax,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var typeSymbol = semanticModel.GetDeclaredSymbol(recordSyntax, ct) as INamedTypeSymbol;
        if (typeSymbol is null)
            return null;

        var attributes = typeSymbol.GetAttributes();

        string? domainShortcut = null;
        bool hasDatePrimitive = false;

        string kind = "DateOnly";
        bool pastOnly = false;
        bool futureOnly = false;
        int? maxAge = null;

        foreach (var attr in attributes)
        {
            var attrName = attr.AttributeClass?.Name;
            if (attrName == "BirthDateAttribute") domainShortcut = "BirthDate";
            else if (attrName == "ExpirationDateAttribute") domainShortcut = "ExpirationDate";
            else if (attrName == "BusinessDateAttribute") domainShortcut = "BusinessDate";
            else if (attrName == "FiscalYearAttribute") domainShortcut = "FiscalYear";
            else if (attrName == "MonthAttribute") domainShortcut = "Month";
            else if (attrName == "QuarterAttribute") domainShortcut = "Quarter";
            else if (attrName == "WeekAttribute") domainShortcut = "Week";
            else if (attrName == "DateRangeAttribute") domainShortcut = "DateRange";
            else if (attrName == "TimeRangeAttribute") domainShortcut = "TimeRange";
            else if (attrName == "DatePrimitiveAttribute")
            {
                hasDatePrimitive = true;
                foreach (var named in attr.NamedArguments)
                {
                    if (named.Key == "Kind" && named.Value.Value is int kindInt)
                    {
                        kind = kindInt switch
                        {
                            0 => "DateOnly",
                            1 => "DateTime",
                            2 => "DateTimeOffset",
                            3 => "TimeOnly",
                            _ => "DateOnly"
                        };
                    }
                    else if (named.Key == "PastOnly" && named.Value.Value is bool po) pastOnly = po;
                    else if (named.Key == "FutureOnly" && named.Value.Value is bool fo) futureOnly = fo;
                }
            }
        }

        if (!hasDatePrimitive && domainShortcut is null)
            return null;

        if (domainShortcut == "BirthDate")
        {
            kind = "DateOnly";
            pastOnly = true;
            maxAge = 150;
            foreach (var attr in attributes)
            {
                if (attr.AttributeClass?.Name == "BirthDateAttribute")
                {
                    foreach (var named in attr.NamedArguments)
                    {
                        if (named.Key == "MaxAge" && named.Value.Value is int ma) maxAge = ma;
                    }
                }
            }
        }
        else if (domainShortcut == "ExpirationDate")
        {
            kind = "DateOnly";
            futureOnly = true;
        }
        else if (domainShortcut == "TimeRange")
        {
            kind = "TimeOnly";
        }

        var backingTypeName = kind switch
        {
            "DateOnly" => "System.DateOnly",
            "DateTime" => "System.DateTime",
            "DateTimeOffset" => "System.DateTimeOffset",
            "TimeOnly" => "System.TimeOnly",
            _ => "System.DateOnly"
        };

        var containingType = typeSymbol.ContainingType;
        var containingList = new System.Collections.Generic.List<string>();
        while (containingType is not null)
        {
            containingList.Insert(0, containingType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
            containingType = containingType.ContainingType;
        }

        return new DatePrimitiveTypeInfo(
            Namespace: typeSymbol.ContainingNamespace.ToDisplayString(),
            TypeName: typeSymbol.Name,
            BackingTypeName: backingTypeName,
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
            Kind: kind,
            PastOnly: pastOnly,
            FutureOnly: futureOnly,
            MaxAge: maxAge,
            DomainShortcut: domainShortcut);
    }

    private static string GenerateDatePrimitive(DatePrimitiveTypeInfo info)
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


        sb.AppendLine($"[global::System.Text.Json.Serialization.JsonConverter(typeof({info.TypeName}JsonConverter))]");
        sb.AppendLine($"[global::System.ComponentModel.TypeConverter(typeof({info.TypeName}TypeConverter))]");
        sb.AppendLine($"[global::System.Diagnostics.DebuggerDisplay(\"{{{info.TypeName}}}({{IsDefault ? \\\"<default>\\\" : _value}})\")]");
        sb.AppendLine("[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]");
        sb.AppendLine($"{info.Accessibility} readonly partial record struct {info.TypeName} :");
        sb.IncreaseIndent();
        sb.AppendLine($"IDomainPrimitive<{info.TypeName}, {info.BackingTypeName}>,");
        sb.AppendLine($"IParsable<{info.TypeName}>,");
        sb.AppendLine($"ISpanParsable<{info.TypeName}>,");
        sb.AppendLine("#if NET8_0_OR_GREATER");
        sb.AppendLine($"IUtf8SpanParsable<{info.TypeName}>,");
        sb.AppendLine("#endif");
        sb.AppendLine("IFormattable,");
        sb.AppendLine("ISpanFormattable,");
        sb.AppendLine("#if NET8_0_OR_GREATER");
        sb.AppendLine("IUtf8SpanFormattable,");
        sb.AppendLine("#endif");
        sb.AppendLine($"IComparable<{info.TypeName}>,");
        sb.AppendLine("IComparable,");
        sb.AppendLine($"System.Numerics.IEqualityOperators<{info.TypeName}, {info.TypeName}, bool>,");
        sb.AppendLine($"System.Numerics.IComparisonOperators<{info.TypeName}, {info.TypeName}, bool>");
        sb.DecreaseIndent();
        sb.OpenBrace();

        // Backing field
        sb.AppendLine($"private readonly {info.BackingTypeName} _value;");
        sb.AppendLine();
        sb.AppendLine("/// <summary>Returns true if this instance was created via default(T) rather than via Create().</summary>");
        sb.AppendLine($"public bool IsDefault => _value.Equals(default({info.BackingTypeName}));");
        sb.AppendLine();
        sb.AppendLine("/// <summary>The underlying primitive value.</summary>");
        sb.AppendLine($"public {info.BackingTypeName} Value");
        sb.OpenBrace();
        sb.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine("get");
        sb.OpenBrace();
        sb.AppendLine($"if (IsDefault) throw new InvalidOperationException($\"Value accessed on a default instance of {info.TypeName}. Check IsDefault before accessing Value.\");");
        sb.AppendLine("return _value;");
        sb.CloseBrace();
        sb.CloseBrace();
        sb.AppendLine();

        sb.AppendLine("/// <inheritdoc/>");
        sb.AppendLine($"public static string PrimitiveName => \"{info.TypeName}\";");
        sb.AppendLine();

        sb.AppendLine("/// <summary>Canonical error codes for this primitive.</summary>");
        sb.AppendLine("public static class Errors");
        sb.OpenBrace();
        sb.AppendLine("public const string NullInput = \"NULL_INPUT\";");
        sb.AppendLine("public const string Temporal = \"TEMPORAL\";");
        sb.AppendLine("public const string Invariant = \"INVARIANT\";");
        sb.CloseBrace();
        sb.AppendLine();

        sb.AppendLine($"private {info.TypeName}({info.BackingTypeName} value) => _value = value;");
        sb.AppendLine();

        GenerateValidation(sb, info);
        GenerateFactoryMethods(sb, info);
        GenerateParsing(sb, info);
        GenerateFormatting(sb, info);
        GenerateOperators(sb, info);
        GenerateComparison(sb, info);
        GenerateTypeConverter(sb, info);

        GenerateShortcutMethods(sb, info);
        GeneratorHelpers.GenerateJsonConverter(sb, info.TypeName, info.BackingTypeName);
        sb.CloseBrace();

        return sb.ToString();
    }

    private static void GenerateValidation(SourceBuilder sb, DatePrimitiveTypeInfo info)
    {
        sb.AppendLine("// ─── Validation ─────────────────────────────────────────────────");
        sb.AppendLine();

        sb.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine($"private static void Validate({info.BackingTypeName} value)");
        sb.OpenBrace();
        sb.AppendLine($"var error = TryValidate(value);");
        sb.AppendLine("if (error.IsError)");
        sb.OpenBrace();
        sb.AppendLine($"throw new DomainPrimitiveValidationException(error);");
        sb.CloseBrace();
        sb.CloseBrace();
        sb.AppendLine();

        sb.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine($"private static global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError TryValidate({info.BackingTypeName} value)");
        sb.OpenBrace();

        if (info.PastOnly || info.FutureOnly || info.MaxAge.HasValue)
        {
            var nowExpr = info.Kind switch
            {
                "DateOnly" => "DateOnly.FromDateTime(DateTime.UtcNow)",
                "DateTime" => "DateTime.UtcNow",
                "DateTimeOffset" => "DateTimeOffset.UtcNow",
                "TimeOnly" => "TimeOnly.FromDateTime(DateTime.UtcNow)",
                _ => "DateTime.UtcNow"
            };

            sb.AppendLine($"var now = {nowExpr};");

            if (info.PastOnly)
            {
                sb.AppendLine($"if (value >= now)");
                sb.IncreaseIndent();
                sb.AppendLine($"return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"TEMPORAL\", \"{info.TypeName} must be in the past.\");");
                sb.DecreaseIndent();
            }

            if (info.FutureOnly)
            {
                sb.AppendLine($"if (value <= now)");
                sb.IncreaseIndent();
                sb.AppendLine($"return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"TEMPORAL\", \"{info.TypeName} must be in the future.\");");
                sb.DecreaseIndent();
            }

            if (info.DomainShortcut == "BirthDate" && info.MaxAge.HasValue)
            {
                sb.AppendLine($"var age = now.Year - value.Year;");
                sb.AppendLine($"if (value > now.AddYears(-age)) age--;");
                sb.AppendLine($"if (age > {info.MaxAge.Value})");
                sb.IncreaseIndent();
                sb.AppendLine($"return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"TEMPORAL\", $\"{info.TypeName} exceeds maximum age of {info.MaxAge.Value}.\");");
                sb.DecreaseIndent();
            }
        }

        sb.AppendLine("return global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError.None;");
        sb.CloseBrace();
        sb.AppendLine();
    }

    private static void GenerateFactoryMethods(SourceBuilder sb, DatePrimitiveTypeInfo info)
    {
        sb.AppendLine("// ─── Factory Methods ─────────────────────────────────────────────");
        sb.AppendLine();

        sb.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine($"public static {info.TypeName} Create({info.BackingTypeName} value)");
        sb.OpenBrace();
        sb.AppendLine("Validate(value);");
        sb.AppendLine($"return new {info.TypeName}(value);");
        sb.CloseBrace();
        sb.AppendLine();



        sb.AppendLine("/// <summary>Tries to create a valid instance. Returns a boolean indicating success.</summary>");
        sb.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine($"public static bool TryCreate({info.BackingTypeName} value, out {info.TypeName} result, out global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError validationError)");
        sb.OpenBrace();
        sb.AppendLine("validationError = TryValidate(value);");
        sb.AppendLine("if (validationError.IsError)");
        sb.OpenBrace();
        sb.AppendLine("result = default;");
        sb.AppendLine("return false;");
        sb.CloseBrace();
        sb.AppendLine($"result = new {info.TypeName}(value);");
        sb.AppendLine("return true;");
        sb.CloseBrace();
        sb.AppendLine();
    }

    private static void GenerateParsing(SourceBuilder sb, DatePrimitiveTypeInfo info)
    {
        sb.AppendLine("// ─── Parsing ─────────────────────────────────────────────────────");
        sb.AppendLine();

        sb.AppendLine($"public static {info.TypeName} Parse(string s) => Parse(s, null);");
        sb.AppendLine($"public static {info.TypeName} Parse(string s, IFormatProvider? provider)");
        sb.OpenBrace();
        sb.AppendLine("if (TryParse(s, provider, out var result)) return result;");
        sb.AppendLine($"throw new System.FormatException($\"The value '{{s}}' is not valid for {info.TypeName}.\");");
        sb.CloseBrace();
        sb.AppendLine();

        sb.AppendLine($"public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out {info.TypeName} result)");
        sb.OpenBrace();
        sb.AppendLine($"if ({info.BackingTypeName}.TryParse(s, provider, out var parsed))");
        sb.OpenBrace();
        sb.AppendLine($"if (TryCreate(parsed, out result, out _)) return true;");
        sb.CloseBrace();
        sb.AppendLine("result = default;");
        sb.AppendLine("return false;");
        sb.CloseBrace();
        sb.AppendLine();

        sb.AppendLine($"public static {info.TypeName} Parse(ReadOnlySpan<char> s, IFormatProvider? provider = null)");
        sb.OpenBrace();
        sb.AppendLine($"if (TryParse(s, provider, out var result)) return result;");
        sb.AppendLine($"throw new System.FormatException(\"The span value is not valid.\");");
        sb.CloseBrace();
        sb.AppendLine();

        sb.AppendLine($"public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out {info.TypeName} result)");
        sb.OpenBrace();
        sb.AppendLine($"if ({info.BackingTypeName}.TryParse(s, provider, out var parsed))");
        sb.OpenBrace();
        sb.AppendLine($"if (TryCreate(parsed, out result, out _)) return true;");
        sb.CloseBrace();
        sb.AppendLine("result = default;");
        sb.AppendLine("return false;");
        sb.CloseBrace();
        sb.AppendLine();

        sb.AppendLine("#if NET8_0_OR_GREATER");
        sb.AppendLine($"public static {info.TypeName} Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider = null)");
        sb.OpenBrace();
        sb.AppendLine($"if (TryParse(utf8Text, provider, out var result)) return result;");
        sb.AppendLine($"throw new System.FormatException(\"The UTF-8 span value is not valid.\");");
        sb.CloseBrace();
        sb.AppendLine();

        sb.AppendLine($"public static bool TryParse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider, out {info.TypeName} result)");
        sb.OpenBrace();
        sb.AppendLine("int count = System.Text.Encoding.UTF8.GetCharCount(utf8Text);");
        sb.AppendLine("if (count <= 512)");
        sb.OpenBrace();
        sb.AppendLine("System.Span<char> chars = stackalloc char[count];");
        sb.AppendLine("System.Text.Encoding.UTF8.GetChars(utf8Text, chars);");
        sb.AppendLine($"if ({info.BackingTypeName}.TryParse((System.ReadOnlySpan<char>)chars, provider, out var parsed))");
        sb.OpenBrace();
        sb.AppendLine($"if (TryCreate(parsed, out result, out _)) return true;");
        sb.CloseBrace();
        sb.CloseBrace();
        sb.AppendLine("else");
        sb.OpenBrace();
        sb.AppendLine("var rented = System.Buffers.ArrayPool<char>.Shared.Rent(count);");
        sb.AppendLine("try");
        sb.OpenBrace();
        sb.AppendLine("System.Text.Encoding.UTF8.GetChars(utf8Text, rented);");
        sb.AppendLine($"if ({info.BackingTypeName}.TryParse((System.ReadOnlySpan<char>)rented.AsSpan(0, count), provider, out var parsed))");
        sb.OpenBrace();
        sb.AppendLine($"if (TryCreate(parsed, out result, out _)) return true;");
        sb.CloseBrace();
        sb.CloseBrace();
        sb.AppendLine("finally");
        sb.OpenBrace();
        sb.AppendLine("System.Buffers.ArrayPool<char>.Shared.Return(rented);");
        sb.CloseBrace();
        sb.CloseBrace();
        sb.AppendLine("result = default;");
        sb.AppendLine("return false;");
        sb.CloseBrace();
        sb.AppendLine("#endif");
        sb.AppendLine();
    }

    private static void GenerateFormatting(SourceBuilder sb, DatePrimitiveTypeInfo info)
    {
        sb.AppendLine("// ─── Formatting ──────────────────────────────────────────────────");
        sb.AppendLine();

        sb.AppendLine("public void Deconstruct(out " + info.BackingTypeName + " value) => value = _value;");
        sb.AppendLine();
        sb.AppendLine("public override string ToString() => _value.ToString();");
        sb.AppendLine();

        sb.AppendLine("public string ToString(string? format, IFormatProvider? formatProvider)");
        sb.AppendLine("    => _value.ToString(format, formatProvider);");
        sb.AppendLine();

        sb.AppendLine("public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)");
        sb.AppendLine("    => _value.TryFormat(destination, out charsWritten, format, provider);");
        sb.AppendLine();

        sb.AppendLine("#if NET8_0_OR_GREATER");
        sb.AppendLine("public bool TryFormat(Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)");
        sb.OpenBrace();
        // Fallback for Date types that might not have a direct utf8 tryformat yet in older standard
        sb.AppendLine("    var s = _value.ToString(format != default ? format.ToString() : null, provider);");
        sb.AppendLine("    return System.Text.Encoding.UTF8.TryGetBytes(s.AsSpan(), utf8Destination, out bytesWritten);");
        sb.CloseBrace();
        sb.AppendLine("#endif");
        sb.AppendLine();
    }

    private static void GenerateOperators(SourceBuilder sb, DatePrimitiveTypeInfo info)
    {
        sb.AppendLine("// ─── Operators ────────────────────────────────────────────────────");
        sb.AppendLine();

        sb.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine($"public static explicit operator {info.BackingTypeName}({info.TypeName} value) => value._value;");
        sb.AppendLine();

        sb.AppendLine($"public static explicit operator {info.TypeName}({info.BackingTypeName} value) => Create(value);");
        sb.AppendLine();
    }

    private static void GenerateComparison(SourceBuilder sb, DatePrimitiveTypeInfo info)
    {
        sb.AppendLine("// ─── Comparison ──────────────────────────────────────────────────");
        sb.AppendLine();

        sb.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine($"public int CompareTo({info.TypeName} other) => _value.CompareTo(other._value);");
        sb.AppendLine();

        sb.AppendLine("public int CompareTo(object? obj) => obj switch");
        sb.OpenBrace();
        sb.AppendLine("null => 1,");
        sb.AppendLine($"{info.TypeName} other => CompareTo(other),");
        sb.AppendLine($"_ => throw new ArgumentException($\"Object must be of type {info.TypeName}.\")");
        sb.CloseBrace(";");
        sb.AppendLine();

        sb.AppendLine($"public static bool operator <({info.TypeName} left, {info.TypeName} right) => left.CompareTo(right) < 0;");
        sb.AppendLine($"public static bool operator <=({info.TypeName} left, {info.TypeName} right) => left.CompareTo(right) <= 0;");
        sb.AppendLine($"public static bool operator >({info.TypeName} left, {info.TypeName} right) => left.CompareTo(right) > 0;");
        sb.AppendLine($"public static bool operator >=({info.TypeName} left, {info.TypeName} right) => left.CompareTo(right) >= 0;");
        sb.AppendLine();
    }

    private static void GenerateShortcutMethods(SourceBuilder sb, DatePrimitiveTypeInfo info)
    {
        if (info.DomainShortcut == "BirthDate")
        {
            sb.AppendLine("// ─── BirthDate Helpers ───────────────────────────────────────────");
            sb.AppendLine();
            sb.AppendLine("public int Age");
            sb.OpenBrace();
            sb.AppendLine("get");
            sb.OpenBrace();
            sb.AppendLine("var today = DateOnly.FromDateTime(DateTime.UtcNow);");
            sb.AppendLine("var age = today.Year - _value.Year;");
            sb.AppendLine("if (_value > today.AddYears(-age)) age--;");
            sb.AppendLine("return age;");
            sb.CloseBrace();
            sb.CloseBrace();
            sb.AppendLine();
        }
        else if (info.DomainShortcut == "ExpirationDate")
        {
            sb.AppendLine("// ─── Expiration Helpers ──────────────────────────────────────────");
            sb.AppendLine();
            sb.AppendLine("public bool IsExpired()");
            sb.OpenBrace();
            sb.AppendLine("return DateOnly.FromDateTime(DateTime.UtcNow) > _value;");
            sb.CloseBrace();
            sb.AppendLine();
            sb.AppendLine("public int DaysUntilExpiration()");
            sb.OpenBrace();
            sb.AppendLine("var today = DateOnly.FromDateTime(DateTime.UtcNow);");
            sb.AppendLine("return _value.DayNumber - today.DayNumber;");
            sb.CloseBrace();
            sb.AppendLine();
        }
    }

    private static void GenerateTypeConverter(SourceBuilder sb, DatePrimitiveTypeInfo info)
    {
        sb.AppendLine("// ─── TypeConverter ───────────────────────────────────────────────");
        sb.AppendLine();
        sb.AppendLine($"public sealed class {info.TypeName}TypeConverter : global::System.ComponentModel.TypeConverter");
        sb.OpenBrace();
        sb.AppendLine($"public override bool CanConvertFrom(global::System.ComponentModel.ITypeDescriptorContext? context, Type sourceType) => sourceType == typeof(string) || sourceType == typeof({info.BackingTypeName}) || base.CanConvertFrom(context, sourceType);");
        sb.AppendLine($"public override object? ConvertFrom(global::System.ComponentModel.ITypeDescriptorContext? context, global::System.Globalization.CultureInfo? culture, object value)");
        sb.OpenBrace();
        sb.AppendLine($"if (value is string s) return Parse(s, culture);");
        sb.AppendLine($"if (value is {info.BackingTypeName} v) return Create(v);");
        sb.AppendLine($"return base.ConvertFrom(context, culture, value);");
        sb.CloseBrace();
        sb.AppendLine($"public override bool CanConvertTo(global::System.ComponentModel.ITypeDescriptorContext? context, Type? destinationType) => destinationType == typeof(string) || destinationType == typeof({info.BackingTypeName}) || base.CanConvertTo(context, destinationType);");
        sb.AppendLine($"public override object? ConvertTo(global::System.ComponentModel.ITypeDescriptorContext? context, global::System.Globalization.CultureInfo? culture, object? value, Type destinationType)");
        sb.OpenBrace();
        sb.AppendLine($"if (value is {info.TypeName} instance)");
        sb.OpenBrace();
        sb.AppendLine($"if (destinationType == typeof(string)) return instance.ToString();");
        sb.AppendLine($"if (destinationType == typeof({info.BackingTypeName})) return instance.Value;");
        sb.CloseBrace();
        sb.AppendLine($"return base.ConvertTo(context, culture, value, destinationType);");
        sb.CloseBrace();
        sb.CloseBrace();
        sb.AppendLine();
    }
}






