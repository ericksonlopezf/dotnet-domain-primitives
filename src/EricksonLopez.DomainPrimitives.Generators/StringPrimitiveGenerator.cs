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
/// Incremental source generator for [StringPrimitive] and string domain shortcut attributes.
/// Generates a complete readonly partial record struct with composable normalization
/// and validation pipelines.
/// </summary>
[Generator(LanguageNames.CSharp)]
internal sealed partial class StringPrimitiveGenerator : IIncrementalGenerator
{
    // FQN array for all 16 string-primitive trigger attributes.
    // ForAttributeWithMetadataName requires the fully-qualified metadata name.
    // These are centrally defined in GeneratorShared to avoid duplication.
    private static readonly string[] TriggerFqns =
    [
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.StringPrimitiveFqn,
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.EmailFqn,
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.PhoneFqn,
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.UrlFqn,
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.SlugFqn,
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.CountryCodeFqn,
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.LanguageCodeFqn,
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.CurrencyCodeFqn,
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.UsernameFqn,
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.PasswordHashFqn,
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.HexColorFqn,
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.IPAddressFqn,
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.MacAddressFqn,
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.IBANFqn,
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.ISBNFqn,
        EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.VINFqn,
    ];

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // TD-014: Use ForAttributeWithMetadataName for all trigger FQNs.
        // IMPORTANT: a user type may carry BOTH [StringPrimitive] AND [Email] (canonical usage
        // for string shortcut types). To prevent duplicate hintNames, we collect all per-FQN
        // providers into a single stream and deduplicate by FullTypeName before emitting source.
        IncrementalValuesProvider<StringPrimitiveTypeInfo?> firstProvider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                TriggerFqns[0],
                predicate: static (node, ct) => EricksonLopez.DomainPrimitives.Generators.Shared.GeneratorShared.IsReadonlyRecordStruct(node, ct),
                transform: static (ctx, ct) => ExtractTypeInfo(ctx.SemanticModel, (RecordDeclarationSyntax)ctx.TargetNode, ct));

        IncrementalValuesProvider<StringPrimitiveTypeInfo?> merged = firstProvider;
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
                    var list = new System.Collections.Generic.List<StringPrimitiveTypeInfo?>(pair.Left.Length + pair.Right.Length);
                    list.AddRange(pair.Left);
                    list.AddRange(pair.Right);
                    return list;
                });
        }

        // Deduplicate by FullTypeName — a type with [StringPrimitive]+[Email] only emits once.
        var deduped = merged
            .Where(static info => info is not null)
            .Select(static (info, _) => info!)
            .Collect()
            .SelectMany(static (all, _) =>
            {
                var seen = new System.Collections.Generic.HashSet<string>();
                var result = new System.Collections.Generic.List<StringPrimitiveTypeInfo>();
                foreach (var info in all)
                {
                    if (seen.Add($"{info.Namespace}.{info.TypeName}"))
                        result.Add(info);
                }
                return result;
            });

        context.RegisterSourceOutput(deduped, static (spc, info) =>
        {
            var source = GenerateStringPrimitive(info);
            spc.AddSource($"{info.TypeName}.g.cs", source);
        });
    }

    // ─── Candidate Detection ─────────────────────────────────────────────────

    // IsCandidateRecordStruct uses GeneratorShared

    // ─── Type Info Extraction ────────────────────────────────────────────────

    private static StringPrimitiveTypeInfo? ExtractTypeInfo(
        GeneratorSyntaxContext context,
        CancellationToken ct)
        => ExtractTypeInfo(context.SemanticModel, (RecordDeclarationSyntax)context.Node, ct);

    private static StringPrimitiveTypeInfo? ExtractTypeInfo(
        SemanticModel semanticModel,
        RecordDeclarationSyntax recordSyntax,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var typeSymbol = semanticModel.GetDeclaredSymbol(recordSyntax, ct) as INamedTypeSymbol;
        if (typeSymbol is null)
            return null;

        var attributes = typeSymbol.GetAttributes();

        // Determine if this is a [StringPrimitive] or a domain shortcut
        string? domainShortcut = null;
        bool hasStringPrimitive = false;

        foreach (var attr in attributes)
        {
            var attrClass = attr.AttributeClass;
            if (attrClass is null) continue;

            var ns = attrClass.ContainingNamespace.ToDisplayString();
            if (!ns.StartsWith("EricksonLopez.DomainPrimitives", System.StringComparison.Ordinal))
                continue;

            var attrName = attrClass.Name;
            if (attrName == "StringPrimitiveAttribute") hasStringPrimitive = true;
            else if (attrName == "EmailAttribute") domainShortcut = "Email";
            else if (attrName == "PhoneAttribute") domainShortcut = "Phone";
            else if (attrName == "UrlAttribute") domainShortcut = "Url";
            else if (attrName == "SlugAttribute") domainShortcut = "Slug";
            else if (attrName == "CountryCodeAttribute") domainShortcut = "CountryCode";
            else if (attrName == "LanguageCodeAttribute") domainShortcut = "LanguageCode";
            else if (attrName == "CurrencyCodeAttribute") domainShortcut = "CurrencyCode";
            else if (attrName == "UsernameAttribute") domainShortcut = "Username";
            else if (attrName == "PasswordHashAttribute") domainShortcut = "PasswordHash";
            else if (attrName == "HexColorAttribute") domainShortcut = "HexColor";
            else if (attrName == "IPAddressAttribute") domainShortcut = "IPAddress";
            else if (attrName == "MacAddressAttribute") domainShortcut = "MacAddress";
            else if (attrName == "IBANAttribute") domainShortcut = "IBAN";
            else if (attrName == "ISBNAttribute") domainShortcut = "ISBN";
            else if (attrName == "VINAttribute") domainShortcut = "VIN";
        }

        if (!hasStringPrimitive && domainShortcut is null)
            return null;

        // Read explicit normalization/validation attributes
        bool trim = false, trimStart = false, trimEnd = false;
        bool lowerCase = false, upperCase = false, normalizeWhitespace = false;
        int? minLength = null, maxLength = null, exactLength = null;
        bool notEmpty = false;
        bool hasCustomValidator = false;
        var regexPatterns = ImmutableArray.CreateBuilder<RegexInfo>();

        foreach (var attr in attributes)
        {
            // HIGH-006: Guard normalization/validation attributes by namespace.
            // Without this guard, a user-defined TrimAttribute or LowerCaseAttribute
            // in their own project would accidentally trigger the generator's normalization.
            var attrNs = attr.AttributeClass?.ContainingNamespace?.ToDisplayString() ?? string.Empty;
            bool isOurNamespace = attrNs.StartsWith("EricksonLopez.DomainPrimitives", System.StringComparison.Ordinal);

            // Check for CustomValidatorAttribute<T> (in EricksonLopez.DomainPrimitives.Validation)
            if (attr.AttributeClass is { IsGenericType: true } customAc &&
                customAc.OriginalDefinition.Name == "CustomValidatorAttribute" &&
                customAc.ContainingNamespace.ToDisplayString().StartsWith("EricksonLopez.DomainPrimitives", System.StringComparison.Ordinal))
            {
                hasCustomValidator = true;
            }

            if (!isOurNamespace) continue;

            var attrName = attr.AttributeClass?.Name;
            switch (attrName)
            {
                case "TrimAttribute": trim = true; break;
                case "TrimStartAttribute": trimStart = true; break;
                case "TrimEndAttribute": trimEnd = true; break;
                case "LowerCaseAttribute": lowerCase = true; break;
                case "UpperCaseAttribute": upperCase = true; break;
                case "NormalizeWhitespaceAttribute": normalizeWhitespace = true; break;
                case "NotEmptyAttribute": notEmpty = true; break;
                case "MinLengthAttribute":
                    if (attr.ConstructorArguments.Length > 0 && attr.ConstructorArguments[0].Value is int min)
                        minLength = min;
                    break;
                case "MaxLengthAttribute":
                    if (attr.ConstructorArguments.Length > 0 && attr.ConstructorArguments[0].Value is int max)
                        maxLength = max;
                    break;
                case "LengthAttribute":
                    if (attr.ConstructorArguments.Length >= 2)
                    {
                        if (attr.ConstructorArguments[0].Value is int lenMin)
                            minLength = lenMin;
                        if (attr.ConstructorArguments[1].Value is int lenMax)
                            maxLength = lenMax;
                    }
                    break;
                case "ExactLengthAttribute":
                    if (attr.ConstructorArguments.Length > 0 && attr.ConstructorArguments[0].Value is int exact)
                        exactLength = exact;
                    break;
                case "RegexAttribute":
                    if (attr.ConstructorArguments.Length > 0 && attr.ConstructorArguments[0].Value is string pattern)
                    {
                        string? errorMessage = null;
                        foreach (var named in attr.NamedArguments)
                        {
                            if (named.Key == "ErrorMessage" && named.Value.Value is string msg)
                                errorMessage = msg;
                        }
                        regexPatterns.Add(new RegexInfo(pattern, errorMessage));
                    }
                    break;
            }
        }

        var allowedSchemes = ImmutableArray.CreateBuilder<string>();

        // Apply domain shortcut defaults (merge with explicit attributes)
        ApplyDomainShortcutDefaults(domainShortcut, attributes,
            ref trim, ref lowerCase, ref upperCase, ref normalizeWhitespace,
            ref notEmpty, ref minLength, ref maxLength, regexPatterns, allowedSchemes);

        // Extract containing types for nested type support
        var containingType = typeSymbol.ContainingType;
        var containingList = ImmutableArray.CreateBuilder<string>();
        while (containingType is not null)
        {
            containingList.Insert(0, containingType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
            containingType = containingType.ContainingType;
        }

        return new StringPrimitiveTypeInfo(
            Namespace: typeSymbol.ContainingNamespace.ToDisplayString(),
            TypeName: typeSymbol.Name,
            Accessibility: typeSymbol.DeclaredAccessibility switch
            {
                Accessibility.Public => "public",
                Accessibility.Internal => "internal",
                Accessibility.Private => "private",
                Accessibility.ProtectedOrInternal => "protected internal",
                _ => "internal"
            },
            ContainingTypes: new EquatableArray<string>(containingList.ToImmutable()),
            Trim: trim,
            TrimStart: trimStart,
            TrimEnd: trimEnd,
            LowerCase: lowerCase,
            UpperCase: upperCase,
            NormalizeWhitespace: normalizeWhitespace,
            MinLength: exactLength.HasValue ? exactLength : minLength,
            MaxLength: exactLength.HasValue ? exactLength : maxLength,
            ExactLength: exactLength,
            NotEmpty: notEmpty,
            RegexPatterns: new EquatableArray<RegexInfo>(regexPatterns.ToImmutableArray()),
            DomainShortcut: domainShortcut,
            HasCustomValidator: hasCustomValidator,
            AllowedSchemes: new EquatableArray<string>(allowedSchemes.ToImmutableArray()));
    }

    private static void ApplyDomainShortcutDefaults(
        string? shortcut,
        ImmutableArray<AttributeData> attributes,
        ref bool trim, ref bool lowerCase, ref bool upperCase,
        ref bool normalizeWhitespace, ref bool notEmpty,
        ref int? minLength, ref int? maxLength,
        System.Collections.Immutable.ImmutableArray<RegexInfo>.Builder regexPatterns,
        System.Collections.Immutable.ImmutableArray<string>.Builder allowedSchemes)
    {
        if (shortcut is null) return;

        switch (shortcut)
        {
            case "Email":
                trim = true;
                lowerCase = true;
                notEmpty = true;
                // Read MaxLength from EmailAttribute (default 320)
                int emailMax = 320;
                foreach (var attr in attributes)
                {
                    if (attr.AttributeClass?.Name == "EmailAttribute")
                    {
                        foreach (var named in attr.NamedArguments)
                        {
                            if (named.Key == "MaxLength" && named.Value.Value is int m)
                                emailMax = m;
                        }
                    }
                }
                maxLength ??= emailMax;
                // RFC 5322 simplified email regex
                if (regexPatterns.Count == 0)
                    regexPatterns.Add(new RegexInfo(
                        @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$",
                        "Invalid email format."));
                break;

            case "Phone":
                trim = true;
                notEmpty = true;
                if (regexPatterns.Count == 0)
                    regexPatterns.Add(new RegexInfo(@"^\+[1-9]\d{1,14}$", "Invalid phone number. Must be in E.164 format."));
                break;

            case "Url":
                trim = true;
                notEmpty = true;
                foreach (var attr in attributes)
                {
                    if (attr.AttributeClass?.Name == "UrlAttribute")
                    {
                        foreach (var named in attr.NamedArguments)
                        {
                            if (named.Key == "AllowedSchemes" && named.Value.Kind == TypedConstantKind.Array)
                            {
                                foreach (var item in named.Value.Values)
                                {
                                    if (item.Value is string s)
                                        allowedSchemes.Add(s);
                                }
                            }
                        }
                    }
                }
                if (allowedSchemes.Count == 0)
                {
                    allowedSchemes.Add("https");
                    allowedSchemes.Add("http");
                }
                break;

            case "Slug":
                trim = true;
                lowerCase = true;
                notEmpty = true;
                int slugMax = 200;
                foreach (var attr in attributes)
                {
                    if (attr.AttributeClass?.Name == "SlugAttribute")
                    {
                        foreach (var named in attr.NamedArguments)
                        {
                            if (named.Key == "MaxLength" && named.Value.Value is int m)
                                slugMax = m;
                        }
                    }
                }
                maxLength ??= slugMax;
                if (regexPatterns.Count == 0)
                    regexPatterns.Add(new RegexInfo(@"^[a-z0-9]+(-[a-z0-9]+)*$", "Invalid slug format."));
                break;

            case "CountryCode":
                trim = true;
                upperCase = true;
                minLength ??= 2;
                maxLength ??= 2;
                break;

            case "LanguageCode":
                trim = true;
                lowerCase = true;
                minLength ??= 2;
                maxLength ??= 2;
                break;

            case "CurrencyCode":
                trim = true;
                upperCase = true;
                minLength ??= 3;
                maxLength ??= 3;
                break;

            case "Username":
                trim = true;
                int usernameMin = 3, usernameMax = 50;
                foreach (var attr in attributes)
                {
                    if (attr.AttributeClass?.Name == "UsernameAttribute")
                    {
                        foreach (var named in attr.NamedArguments)
                        {
                            if (named.Key == "MinLength" && named.Value.Value is int mn)
                                usernameMin = mn;
                            if (named.Key == "MaxLength" && named.Value.Value is int mx)
                                usernameMax = mx;
                        }
                    }
                }
                minLength ??= usernameMin;
                maxLength ??= usernameMax;
                if (regexPatterns.Count == 0)
                    regexPatterns.Add(new RegexInfo(@"^[a-zA-Z0-9._-]+$", "Username can only contain letters, numbers, dots, underscores, and hyphens."));
                break;

            case "PasswordHash":
                notEmpty = true;
                break;

            case "HexColor":
                trim = true;
                upperCase = true;
                if (regexPatterns.Count == 0)
                    regexPatterns.Add(new RegexInfo(@"^#([0-9A-F]{3}|[0-9A-F]{4}|[0-9A-F]{6}|[0-9A-F]{8})$", "Invalid hex color. Must be in #RGB, #RGBA, #RRGGBB, or #RRGGBBAA format."));
                break;

            case "IPAddress":
                trim = true;
                notEmpty = true;
                if (regexPatterns.Count == 0)
                {
                    // This regex covers both IPv4 and IPv6 (including :: abbreviation forms).
                    // It is intentionally verbose because no concise regex can correctly validate all IPv6 forms.
                    // MAINTENANCE NOTE: If this regex needs updating, consider using System.Net.IPAddress.TryParse()
                    // as a supplemental validation in a custom [CustomValidator] instead of extending this regex further.
                    // The regex is sufficient for the vast majority (>99.9%) of practical address formats.
                    regexPatterns.Add(new RegexInfo(
                        @"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$|^(?:[0-9a-fA-F]{1,4}:){7}[0-9a-fA-F]{1,4}$|^(?:[0-9a-fA-F]{1,4}:){1,7}:$|^:(?::[0-9a-fA-F]{1,4}){1,7}$|^(?:[0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}$|^(?:[0-9a-fA-F]{1,4}:){1,5}(?::[0-9a-fA-F]{1,4}){1,2}$|^(?:[0-9a-fA-F]{1,4}:){1,4}(?::[0-9a-fA-F]{1,4}){1,3}$|^(?:[0-9a-fA-F]{1,4}:){1,3}(?::[0-9a-fA-F]{1,4}){1,4}$|^(?:[0-9a-fA-F]{1,4}:){1,2}(?::[0-9a-fA-F]{1,4}){1,5}$|^[0-9a-fA-F]{1,4}:(?::[0-9a-fA-F]{1,4}){1,6}$|^::$",
                        "Invalid IP address. Must be a valid IPv4 or IPv6 address."));
                }
                break;

            case "MacAddress":
                trim = true;
                upperCase = true;
                if (regexPatterns.Count == 0)
                    regexPatterns.Add(new RegexInfo(@"^([0-9A-F]{2}[:-]){5}([0-9A-F]{2})$", "Invalid MAC address."));
                break;

            case "IBAN":
                trim = true;
                upperCase = true;
                if (regexPatterns.Count == 0)
                    regexPatterns.Add(new RegexInfo(@"^[A-Z]{2}[0-9]{2}[A-Z0-9]{11,30}$", "Invalid IBAN format."));
                break;

            case "ISBN":
                trim = true;
                notEmpty = true;
                if (regexPatterns.Count == 0)
                    regexPatterns.Add(new RegexInfo(@"^(?:ISBN(?:-1[03])?:? )?(?=[0-9X]{10}$|(?=(?:[0-9]+[- ]){3})[- 0-9X]{13}$|97[89][0-9]{10}$|(?=(?:[0-9]+[- ]){4})[- 0-9]{17}$)(?:97[89][- ]?)?[0-9]{1,5}[- ]?[0-9]+[- ]?[0-9]+[- ]?[0-9X]$", "Invalid ISBN format."));
                break;

            case "VIN":
                trim = true;
                upperCase = true;
                if (regexPatterns.Count == 0)
                    regexPatterns.Add(new RegexInfo(@"^[A-HJ-NPR-Z0-9]{17}$", "Invalid VIN format."));
                break;
        }
    }

    // ─── Code Generation ─────────────────────────────────────────────────────

    private static string GenerateStringPrimitive(StringPrimitiveTypeInfo info)
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
        if (info.RegexPatterns.Length > 0)
            sb.AppendLine("using System.Text.RegularExpressions;");
        sb.AppendLine("using EricksonLopez.DomainPrimitives;");
        sb.AppendLine("using EricksonLopez.DomainPrimitives.Validation;");
        sb.AppendLine();

        sb.AppendLine($"namespace {info.Namespace};");
        sb.AppendLine();



        // Type declaration
        sb.AppendLine($"[global::System.Text.Json.Serialization.JsonConverter(typeof({info.TypeName}JsonConverter))]");
        sb.AppendLine($"[global::System.ComponentModel.TypeConverter(typeof({info.TypeName}TypeConverter))]");
        sb.AppendLine($"[global::System.Diagnostics.DebuggerDisplay(\"{{{info.TypeName}}}({{IsDefault ? \\\"<default>\\\" : _value}})\")]");
        sb.AppendLine($"[global::System.Diagnostics.DebuggerTypeProxy(typeof({info.TypeName}DebugView))]");
        sb.AppendLine($"[global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Auto)]");
        sb.AppendLine("[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]");
        sb.AppendLine($"{info.Accessibility} readonly partial record struct {info.TypeName} :");
        sb.IncreaseIndent();
        sb.AppendLine($"IDomainPrimitive<{info.TypeName}, string>,");
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
        sb.AppendLine("private readonly string _value;");
        sb.AppendLine();

        // IsDefault and Value property
        sb.AppendLine("/// <summary>Returns true if this instance was created via default(T) rather than via Create().</summary>");
        sb.AppendLine("public bool IsDefault => _value is null;");
        sb.AppendLine();
        sb.AppendLine("/// <summary>The underlying string value.</summary>");
        sb.AppendLine("public string Value");
        sb.OpenBrace();
        sb.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine("get");
        sb.OpenBrace();
        sb.AppendLine($"if (_value is null) throw new InvalidOperationException($\"Value accessed on a default instance of {info.TypeName}. Check IsDefault before accessing Value.\");");
        sb.AppendLine("return _value;");
        sb.CloseBrace();
        sb.CloseBrace();
        sb.AppendLine();

        // PrimitiveName
        sb.AppendLine("/// <inheritdoc/>");
        sb.AppendLine($"public static string PrimitiveName => \"{info.TypeName}\";");
        sb.AppendLine();

        // Error Constants
        sb.AppendLine("/// <summary>Canonical error codes for this primitive.</summary>");
        sb.AppendLine("public static class Errors");
        sb.OpenBrace();
        sb.AppendLine("public const string NullInput = \"NULL_INPUT\";");
        sb.AppendLine("public const string Empty = \"EMPTY\";");
        sb.AppendLine("public const string Length = \"LENGTH\";");
        sb.AppendLine("public const string Format = \"FORMAT\";");
        sb.AppendLine("public const string Invariant = \"INVARIANT\";");
        sb.CloseBrace();
        sb.AppendLine();

        // Private constructor
        sb.AppendLine($"private {info.TypeName}(string value) => _value = value;");
        sb.AppendLine();

        // Regex fields (GeneratedRegex)
        GenerateRegexFields(sb, info);

        // Normalize method
        GenerateNormalize(sb, info);

        // Validate / TryValidate
        GenerateValidation(sb, info);

        // Factory methods
        GenerateFactoryMethods(sb, info);

        // ValidateSpan
        GenerateSpanValidation(sb, info);

        // Parsing
        GenerateParsing(sb, info);

        // Formatting
        GenerateFormatting(sb, info);

        // Operators
        GenerateOperators(sb, info);

        // Comparison
        EricksonLopez.DomainPrimitives.Generators.Shared.ComparisonTemplate.GenerateComparison(sb, info.TypeName, isStringBacked: true);

        // TypeConverter
        EricksonLopez.DomainPrimitives.Generators.Shared.TypeConverterTemplate.GenerateTypeConverter(sb, info.TypeName, "string");

        sb.AppendLine($"private sealed class {info.TypeName}DebugView");
        sb.OpenBrace();
        sb.AppendLine($"private readonly {info.TypeName} _t;");
        sb.AppendLine($"public {info.TypeName}DebugView({info.TypeName} t) => _t = t;");
        sb.AppendLine("public string Value => _t.IsDefault ? \"default\" : _t.Value;");
        sb.CloseBrace();

        GeneratorHelpers.GenerateJsonConverter(sb, info.TypeName, "string");
        sb.CloseBrace(); // close type

        return sb.ToString();
    }

    // ─── Regex Fields ────────────────────────────────────────────────────────


    // ─── Normalization ───────────────────────────────────────────────────────


    // ─── Validation ──────────────────────────────────────────────────────────


    // ─── Factory Methods ─────────────────────────────────────────────────────



    // ─── Parsing ─────────────────────────────────────────────────────────────


    // ─── Formatting ──────────────────────────────────────────────────────────


    // ─── Operators ───────────────────────────────────────────────────────────


    // ─── String Helpers ──────────────────────────────────────────────────────

    private static string EscapeVerbatimString(string s) => s.Replace("\"", "\"\"");
    private static string EscapeString(string s) => s.Replace("\\", "\\\\").Replace("\"", "\\\"");
}




