// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.DomainPrimitives.Generators.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EricksonLopez.DomainPrimitives.Generators;

[Generator(LanguageNames.CSharp)]
internal sealed class NumericPrimitiveGenerator : IIncrementalGenerator
{
    // FQN array for all 16 numeric-primitive trigger attributes.
    private static readonly string[] TriggerFqns =
    [
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.NumericPrimitiveFqn,
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.MoneyFqn,
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.PercentageFqn,
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.LatitudeFqn,
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.LongitudeFqn,
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.AgeFqn,
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.WeightFqn,
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.HeightFqn,
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.DistanceFqn,
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.TemperatureFqn,
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.ScoreFqn,
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.QuantityFqn,
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.PriceFqn,
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.TaxRateFqn,
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.DiscountFqn,
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.RatingFqn,
    ];

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // TD-014: ForAttributeWithMetadataName with merged+deduplicated pattern.
        // Prevents duplicate hintName when a type carries [NumericPrimitive<T>] AND [Money].
        IncrementalValuesProvider<NumericPrimitiveTypeInfo?> merged = context.SyntaxProvider
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
                    var list = new List<NumericPrimitiveTypeInfo?>(pair.Left.Length + pair.Right.Length);
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
                var seen = new HashSet<string>();
                var result = new List<NumericPrimitiveTypeInfo>();
                foreach (var info in all)
                    if (seen.Add($"{info.Namespace}.{info.TypeName}"))
                        result.Add(info);
                return result;
            });

        context.RegisterSourceOutput(deduped, static (spc, info) =>
        {
            var source = GenerateNumericPrimitive(info);
            spc.AddSource($"{info.TypeName}.g.cs", source);
        });
    }

    internal static NumericPrimitiveTypeInfo? ExtractTypeInfo(
        SemanticModel semanticModel,
        RecordDeclarationSyntax recordSyntax,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var typeSymbol = (INamedTypeSymbol)semanticModel.GetDeclaredSymbol(recordSyntax, ct)!;

        var attributes = typeSymbol.GetAttributes();

        string? domainShortcut = null;
        string? backingTypeName = null;

        bool allowAddition = false;
        bool allowSubtraction = false;
        bool allowScalarMultiplication = false;
        bool allowScalarDivision = false;
        bool allowNegation = false;

        foreach (var attr in attributes)
        {
            var attrName = attr.AttributeClass?.Name;
            if (attrName == "MoneyAttribute") domainShortcut = "Money";
            else if (attrName == "PercentageAttribute") domainShortcut = "Percentage";
            else if (attrName == "LatitudeAttribute") domainShortcut = "Latitude";
            else if (attrName == "LongitudeAttribute") domainShortcut = "Longitude";
            else if (attrName == "AgeAttribute") domainShortcut = "Age";
            else if (attrName == "WeightAttribute") domainShortcut = "Weight";
            else if (attrName == "HeightAttribute") domainShortcut = "Height";
            else if (attrName == "DistanceAttribute") domainShortcut = "Distance";
            else if (attrName == "TemperatureAttribute") domainShortcut = "Temperature";
            else if (attrName == "ScoreAttribute") domainShortcut = "Score";
            else if (attrName == "QuantityAttribute") domainShortcut = "Quantity";
            else if (attrName == "PriceAttribute") domainShortcut = "Price";
            else if (attrName == "TaxRateAttribute") domainShortcut = "TaxRate";
            else if (attrName == "DiscountAttribute") domainShortcut = "Discount";
            else if (attrName == "RatingAttribute") domainShortcut = "Rating";
            else if (attr.AttributeClass is { IsGenericType: true } ac &&
                     ac.OriginalDefinition.Name == "NumericPrimitiveAttribute")
            {
                backingTypeName = GeneratorHelpers.ResolveSpecialType(ac.TypeArguments[0]);

                foreach (var named in attr.NamedArguments)
                {
                    if (named.Key == "Operations" && named.Value.Value is int p)
                    {
                        allowAddition = (p & 1) != 0;
                        allowSubtraction = (p & 2) != 0;
                        allowScalarMultiplication = (p & 4) != 0;
                        allowScalarDivision = (p & 8) != 0;
                        allowNegation = (p & 16) != 0;
                    }
                }
            }
        }

        if (backingTypeName is null && domainShortcut is null)
            return null;

        double? rangeMin = null;
        double? rangeMax = null;
        bool rangeMinExclusive = false;
        bool rangeMaxExclusive = false;
        string? rangeStringMin = null;
        string? rangeStringMax = null;

        foreach (var attr in attributes)
        {
            var attrName = attr.AttributeClass?.Name;
            if (attrName is "RangeAttribute" or "PrimitiveRangeAttribute" )
            {
                if (attr.ConstructorArguments.Length >= 2)
                {
                    if (attr.ConstructorArguments[0].Type?.SpecialType == SpecialType.System_String)
                    {
                        rangeStringMin = attr.ConstructorArguments[0].Value as string;
                        rangeStringMax = attr.ConstructorArguments[1].Value as string;
                    }
                    else
                    {
                        try
                        {
                            if (attr.ConstructorArguments[0].Value is not null)
                                rangeMin = Convert.ToDouble(attr.ConstructorArguments[0].Value, System.Globalization.CultureInfo.InvariantCulture);
                            if (attr.ConstructorArguments[1].Value is not null)
                                rangeMax = Convert.ToDouble(attr.ConstructorArguments[1].Value, System.Globalization.CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            // Fallback if conversion fails
                        }
                    }
                }
                foreach (var named in attr.NamedArguments)
                {
                    if (named.Key == "MinExclusive" && named.Value.Value is bool me) rangeMinExclusive = me;
                    if (named.Key == "MaxExclusive" && named.Value.Value is bool mx) rangeMaxExclusive = mx;
                    if (named.Key == "StringMin" && named.Value.Value is string smn) rangeStringMin = smn;
                    if (named.Key == "StringMax" && named.Value.Value is string smx) rangeStringMax = smx;
                }
            }
        }

        if (domainShortcut == "Money")
        {
            backingTypeName ??= "decimal";
            allowAddition = true;
            allowSubtraction = true;
            allowScalarMultiplication = true;
            allowScalarDivision = true;

            double min = 0;
            double max = double.MaxValue;
            foreach (var attr in attributes)
            {
                if (attr.AttributeClass?.Name == "MoneyAttribute")
                {
                    if (attr.ConstructorArguments.Length >= 2 && attr.ConstructorArguments[1].Value is double mnCtor)
                        min = mnCtor;
                    if (attr.ConstructorArguments.Length >= 3 && attr.ConstructorArguments[2].Value is double mxCtor)
                        max = mxCtor;

                    foreach (var named in attr.NamedArguments)
                    {
                        if (named.Key == "Min" && named.Value.Value is double mn) min = mn;
                        if (named.Key == "Max" && named.Value.Value is double mx) max = mx;
                    }
                }
            }
            rangeMin ??= min;
            rangeMax ??= max;
        }
        else if (domainShortcut == "Percentage")
        {
            backingTypeName ??= "decimal";
            double min = 0;
            double max = 100;
            foreach (var attr in attributes)
            {
                if (attr.AttributeClass?.Name == "PercentageAttribute")
                {
                    foreach (var named in attr.NamedArguments)
                    {
                        if (named.Key == "Min" && named.Value.Value is double mn) min = mn;
                        if (named.Key == "Max" && named.Value.Value is double mx) max = mx;
                    }
                }
            }
            rangeMin ??= min;
            rangeMax ??= max;
        }
        else if (domainShortcut == "Latitude")
        {
            backingTypeName ??= "double";
            rangeMin ??= -90;
            rangeMax ??= 90;
        }
        else if (domainShortcut == "Longitude")
        {
            backingTypeName ??= "double";
            rangeMin ??= -180;
            rangeMax ??= 180;
        }
        else if (domainShortcut == "Age")
        {
            backingTypeName ??= "int";
            rangeMin ??= 0;
            rangeMax ??= 150;
        }
        int? scale = null;
        if (domainShortcut == "Rating")
        {
            backingTypeName ??= "decimal";
            double min = 0;
            double max = 5;
            int scaleVal = 1;
            foreach (var attr in attributes)
            {
                if (attr.AttributeClass?.Name == "RatingAttribute")
                {
                    foreach (var named in attr.NamedArguments)
                    {
                        if (named.Key == "Min" && named.Value.Value is double mn) min = mn;
                        if (named.Key == "Max" && named.Value.Value is double mx) max = mx;
                        if (named.Key == "Scale" && named.Value.Value is int sc) scaleVal = sc;
                    }
                }
            }
            rangeMin ??= min;
            rangeMax ??= max;
            scale = scaleVal;
        }
        else if (domainShortcut is "Weight" or "Height" or "Distance" or "Temperature" or "Score" or "Quantity")
        {
            backingTypeName ??= "double";
        }
        else if (domainShortcut is "Price" or "TaxRate" or "Discount")
        {
            backingTypeName ??= "decimal";
        }

        if (backingTypeName is null) return null;

        var containingType = typeSymbol.ContainingType;
        var containingList = new List<string>();
        while (containingType is not null)
        {
            containingList.Insert(0, containingType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
            containingType = containingType.ContainingType;
        }

        var defaults = GeneratorHelpers.ExtractAssemblyDefaults(semanticModel.Compilation);

        return new NumericPrimitiveTypeInfo(
            Namespace: typeSymbol.ContainingNamespace.ToDisplayString(),
            TypeName: typeSymbol.Name,
            BackingTypeName: backingTypeName,
            Accessibility: typeSymbol.DeclaredAccessibility switch
            {
                Accessibility.Internal => "internal",
                Accessibility.Protected => "protected",
                Accessibility.Private => "private",
                Accessibility.ProtectedOrInternal => "protected internal",
                Accessibility.ProtectedAndInternal => "private protected",
                _ => "public"
            },
            ContainingTypes: new EquatableArray<string>(containingList.ToImmutableArray()),
            AllowAddition: allowAddition,
            AllowSubtraction: allowSubtraction,
            AllowScalarMultiplication: allowScalarMultiplication,
            AllowScalarDivision: allowScalarDivision,
            AllowNegation: allowNegation,
            RangeMin: rangeMin,
            RangeMax: rangeMax,
            RangeMinExclusive: rangeMinExclusive,
            RangeMaxExclusive: rangeMaxExclusive,
            DomainShortcut: domainShortcut,
            Scale: scale,
            RangeStringMin: rangeStringMin,
            RangeStringMax: rangeStringMax,
            CustomExceptionType: defaults.ExceptionTypeFullName);
    }



    internal static string GenerateNumericPrimitive(NumericPrimitiveTypeInfo info)
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
        sb.AppendLine($"[global::System.Diagnostics.DebuggerTypeProxy(typeof({info.TypeName}DebugView))]");
        sb.AppendLine($"[global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Auto)]");
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
        sb.Append($"System.Numerics.IComparisonOperators<{info.TypeName}, {info.TypeName}, bool>");
        if (info.AllowAddition) sb.Append($",\n    System.Numerics.IAdditionOperators<{info.TypeName}, {info.TypeName}, {info.TypeName}>");
        if (info.AllowSubtraction) sb.Append($",\n    System.Numerics.ISubtractionOperators<{info.TypeName}, {info.TypeName}, {info.TypeName}>");
        if (info.AllowScalarMultiplication) sb.Append($",\n    System.Numerics.IMultiplyOperators<{info.TypeName}, {info.BackingTypeName}, {info.TypeName}>");
        if (info.AllowScalarDivision) sb.Append($",\n    System.Numerics.IDivisionOperators<{info.TypeName}, {info.BackingTypeName}, {info.TypeName}>");
        if (info.AllowNegation) sb.Append($",\n    System.Numerics.IUnaryNegationOperators<{info.TypeName}, {info.TypeName}>");
        sb.AppendLine();
        sb.DecreaseIndent();
        sb.OpenBrace();

        // Backing field
        sb.AppendLine($"private readonly {info.BackingTypeName} _value;");
        sb.AppendLine("private readonly bool _isInitialized;");
        sb.AppendLine();

        sb.AppendLine("/// <summary>Returns true if this instance was created via default(T) rather than via Create().</summary>");
        sb.AppendLine($"public bool IsDefault => !_isInitialized;");
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

        // Error Constants
        sb.AppendLine("/// <summary>Canonical error codes for this primitive.</summary>");
        sb.AppendLine("public static class Errors");
        sb.OpenBrace();
        sb.AppendLine("public const string NullInput = \"NULL_INPUT\";");
        sb.AppendLine("public const string Range = \"RANGE\";");
        sb.AppendLine("public const string Invariant = \"INVARIANT\";");
        sb.CloseBrace();
        sb.AppendLine();

        sb.AppendLine($"private {info.TypeName}({info.BackingTypeName} value)");
        sb.OpenBrace();
        sb.AppendLine("_value = value;");
        sb.AppendLine("_isInitialized = true;");
        sb.CloseBrace();
        sb.AppendLine();

        GenerateValidation(sb, info);
        GenerateFactoryMethods(sb, info);
        GenerateParsing(sb, info);
        GenerateFormatting(sb, info);
        GenerateOperators(sb, info);

        EricksonLopez.DomainPrimitives.Generators.Shared.ComparisonTemplate.GenerateComparison(sb, info.TypeName, isStringBacked: false);
        EricksonLopez.DomainPrimitives.Generators.Shared.TypeConverterTemplate.GenerateTypeConverter(sb, info.TypeName, info.BackingTypeName);

        sb.AppendLine($"private sealed class {info.TypeName}DebugView");
        sb.OpenBrace();
        sb.AppendLine($"private readonly {info.TypeName} _t;");
        sb.AppendLine($"public {info.TypeName}DebugView({info.TypeName} t) => _t = t;");
        sb.AppendLine("public string Value => _t.IsDefault ? \"default\" : _t.Value.ToString();");
        sb.CloseBrace();

        GeneratorHelpers.GenerateJsonConverter(sb, info.TypeName, info.BackingTypeName);
        sb.CloseBrace();

        return sb.ToString();
    }

    private static void GenerateValidation(SourceBuilder sb, NumericPrimitiveTypeInfo info)
    {
        sb.AppendLine("// ─── Validation ─────────────────────────────────────────────────");
        sb.AppendLine();

        sb.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine($"private static void Validate({info.BackingTypeName} value)");
        sb.OpenBrace();
        sb.AppendLine($"var error = TryValidate(value);");
        sb.AppendLine($"if (error.IsError)");
        sb.OpenBrace();
        if (!string.IsNullOrEmpty(info.CustomExceptionType))
        {
            sb.AppendLine($"throw new {info.CustomExceptionType}(error.Message);");
        }
        else
        {
            sb.AppendLine($"throw new DomainPrimitiveValidationException(error);");
        }
        sb.CloseBrace();
        sb.CloseBrace();
        sb.AppendLine();

        sb.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine($"private static global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError TryValidate({info.BackingTypeName} value)");
        sb.OpenBrace();

        if (info.RangeStringMin is not null)
        {
            var op = info.RangeMinExclusive ? "<=" : "<";
            var msg = info.RangeMinExclusive ? "greater than" : "at least";
            var literal = info.BackingTypeName == "decimal" ? $"{info.RangeStringMin}m" : info.RangeStringMin;
            sb.AppendLine($"if (value {op} {literal})");
            sb.IncreaseIndent();
            sb.AppendLine($"return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"RANGE\", $\"{info.TypeName} must be {msg} {info.RangeStringMin}. Got {{value}}.\");");
            sb.DecreaseIndent();
        }
        else if (info.RangeMin.HasValue)
        {
            var op = info.RangeMinExclusive ? "<=" : "<";
            var msg = info.RangeMinExclusive ? "greater than" : "at least";
            sb.AppendLine($"if (value {op} ({info.BackingTypeName}){info.RangeMin.Value})");
            sb.IncreaseIndent();
            sb.AppendLine($"return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"RANGE\", $\"{info.TypeName} must be {msg} {info.RangeMin.Value}. Got {{value}}.\");");
            sb.DecreaseIndent();
        }

        if (info.RangeStringMax is not null)
        {
            var op = info.RangeMaxExclusive ? ">=" : ">";
            var msg = info.RangeMaxExclusive ? "less than" : "at most";
            var literal = info.BackingTypeName == "decimal" ? $"{info.RangeStringMax}m" : info.RangeStringMax;
            sb.AppendLine($"if (value {op} {literal})");
            sb.IncreaseIndent();
            sb.AppendLine($"return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"RANGE\", $\"{info.TypeName} must be {msg} {info.RangeStringMax}. Got {{value}}.\");");
            sb.DecreaseIndent();
        }
        else if (info.RangeMax.HasValue && info.RangeMax.Value != double.MaxValue)
        {
            var op = info.RangeMaxExclusive ? ">=" : ">";
            var msg = info.RangeMaxExclusive ? "less than" : "at most";
            sb.AppendLine($"if (value {op} ({info.BackingTypeName}){info.RangeMax.Value})");
            sb.IncreaseIndent();
            sb.AppendLine($"return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"RANGE\", $\"{info.TypeName} must be {msg} {info.RangeMax.Value}. Got {{value}}.\");");
            sb.DecreaseIndent();
        }

        if (info.Scale.HasValue)
        {
            sb.AppendLine($"if (Math.Round((double)value, {info.Scale.Value}) != (double)value)");
            sb.IncreaseIndent();
            sb.AppendLine($"return new global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError(\"FORMAT\", $\"{info.TypeName} must have at most {info.Scale.Value} decimal place(s).\");");
            sb.DecreaseIndent();
        }

        sb.AppendLine("return global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError.None;");
        sb.CloseBrace();
        sb.AppendLine();
    }

    private static void GenerateFactoryMethods(SourceBuilder sb, NumericPrimitiveTypeInfo info)
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

    private static void GenerateParsing(SourceBuilder sb, NumericPrimitiveTypeInfo info)
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
        sb.AppendLine($"if ({info.BackingTypeName}.TryParse(utf8Text, provider, out var parsed))");
        sb.OpenBrace();
        sb.AppendLine($"if (TryCreate(parsed, out result, out _)) return true;");

        sb.CloseBrace();
        sb.AppendLine("result = default;");
        sb.AppendLine("return false;");
        sb.CloseBrace();
        sb.AppendLine("#endif");
    }

    private static void GenerateFormatting(SourceBuilder sb, NumericPrimitiveTypeInfo info)
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
        sb.AppendLine("    => _value.TryFormat(utf8Destination, out bytesWritten, format, provider);");
        sb.AppendLine("#endif");
        sb.AppendLine();
    }

    private static void GenerateOperators(SourceBuilder sb, NumericPrimitiveTypeInfo info)
    {
        sb.AppendLine("// ─── Operators ────────────────────────────────────────────────────");
        sb.AppendLine();

        sb.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine($"public static explicit operator {info.BackingTypeName}({info.TypeName} value) => value._value;");
        sb.AppendLine();

        sb.AppendLine($"public static explicit operator {info.TypeName}({info.BackingTypeName} value) => Create(value);");
        sb.AppendLine();

        if (info.AllowAddition)
        {
            sb.AppendLine($"public static {info.TypeName} operator +({info.TypeName} left, {info.TypeName} right) => Create(({info.BackingTypeName})(left.Value + right.Value));");
            sb.AppendLine();
        }

        if (info.AllowSubtraction)
        {
            sb.AppendLine($"public static {info.TypeName} operator -({info.TypeName} left, {info.TypeName} right) => Create(({info.BackingTypeName})(left.Value - right.Value));");
            sb.AppendLine();
        }

        if (info.AllowScalarMultiplication)
        {
            sb.AppendLine($"public static {info.TypeName} operator *({info.TypeName} left, {info.BackingTypeName} right) => Create(({info.BackingTypeName})(left.Value * right));");
            sb.AppendLine($"public static {info.TypeName} operator *({info.BackingTypeName} left, {info.TypeName} right) => Create(({info.BackingTypeName})(left * right.Value));");
            sb.AppendLine();
        }

        if (info.AllowScalarDivision)
        {
            sb.AppendLine($"public static {info.TypeName} operator /({info.TypeName} left, {info.BackingTypeName} right) => Create(({info.BackingTypeName})(left.Value / right));");
            sb.AppendLine();
        }

        if (info.AllowNegation)
        {
            sb.AppendLine($"public static {info.TypeName} operator -({info.TypeName} value) => Create(({info.BackingTypeName})(-value.Value));");
            sb.AppendLine();
        }
    }
}








