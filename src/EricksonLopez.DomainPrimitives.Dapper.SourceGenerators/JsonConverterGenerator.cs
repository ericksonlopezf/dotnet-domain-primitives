// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace EricksonLopez.DomainPrimitives.Dapper.SourceGenerators;

/// <summary>
/// Incremental source generator that emits System.Text.Json JsonConverter&lt;T&gt; classes
/// and a DomainPrimitivesJsonRegistration.RegisterAll(JsonSerializerOptions) method
/// for all domain primitive types implementing IEntityId&lt;TSelf&gt; or IStrongId&lt;TSelf, TValue&gt;.
/// </summary>
/// <remarks>
/// <para>
/// Mirrors DapperTypeHandlerGenerator exactly:
/// (1) Syntax pipeline discovers primitives in the current compilation source trees.
/// (2) Reference pipeline discovers primitives defined in referenced assemblies.
/// (3) Merge + dedup by (Namespace, TypeName) to avoid duplicate converters.
/// (4) Emit one {TypeName}JsonConverter.g.cs per type, plus DomainPrimitivesJsonRegistration.g.cs.
/// </para>
/// <para>
/// AOT / Trimming: Generated converters use only direct property access and JsonSerializer
/// primitive operations. Zero runtime reflection. The RegisterAll registration is AOT-safe:
/// it explicitly instantiates each generated converter class.
/// </para>
/// </remarks>
[Generator(LanguageNames.CSharp)]
internal sealed class JsonConverterGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Pipeline 1: Syntax-based discovery in the CURRENT project source trees.
        var syntaxPrimitives = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => s is StructDeclarationSyntax or RecordDeclarationSyntax,
                transform: static (ctx, _) => GetPrimitiveInfoFromSyntax(ctx))
            .Where(static m => m.HasValue)
            .Select(static (m, _) => m!.Value);

        // Pipeline 2: Symbol-based discovery in ALL referenced assemblies.
        var referencedPrimitives = context.CompilationProvider
            .Select(static (compilation, _) => GetReferencedAssemblyPrimitives(compilation));

        // Merge both pipelines.
        var combined = context.CompilationProvider
            .Combine(syntaxPrimitives.Collect())
            .Combine(referencedPrimitives);

        context.RegisterSourceOutput(combined, (spc, source) =>
        {
            var compilation = source.Left.Left;
            var syntaxFound = source.Left.Right;
            var referencedFound = source.Right;

            var all = syntaxFound
                .Concat(referencedFound)
                .Distinct()
                .ToImmutableArray();

            Execute(compilation, all, spc);
        });
    }

    private static ImmutableArray<PrimitiveInfo> GetReferencedAssemblyPrimitives(Compilation compilation)
    {
        var results = new List<PrimitiveInfo>();

        foreach (var reference in compilation.References)
        {
            if (compilation.GetAssemblyOrModuleSymbol(reference) is not IAssemblySymbol assemblySymbol)
                continue;

            var assemblyName = assemblySymbol.Name;
            if (assemblyName.StartsWith("System", StringComparison.Ordinal)
                || assemblyName.StartsWith("Microsoft", StringComparison.Ordinal)
                || assemblyName.StartsWith("netstandard", StringComparison.Ordinal)
                || assemblyName.StartsWith("mscorlib", StringComparison.Ordinal)
                || assemblyName.StartsWith("EricksonLopez.DomainPrimitives", StringComparison.Ordinal))
                continue;

            WalkNamespace(assemblySymbol.GlobalNamespace, results);
        }

        return results.ToImmutableArray();
    }

    private static void WalkNamespace(INamespaceSymbol ns, List<PrimitiveInfo> results)
    {
        foreach (var member in ns.GetMembers())
        {
            if (member is INamespaceSymbol childNs)
            {
                WalkNamespace(childNs, results);
            }
            else if (member is INamedTypeSymbol typeSymbol && typeSymbol.IsValueType)
            {
                var info = GetPrimitiveInfoFromSymbol(typeSymbol);
                if (info.HasValue)
                    results.Add(info.Value);
            }
        }
    }

    private static PrimitiveInfo? GetPrimitiveInfoFromSyntax(GeneratorSyntaxContext context)
    {
        var typeDecl = (TypeDeclarationSyntax)context.Node;
        var symbol = context.SemanticModel.GetDeclaredSymbol(typeDecl) as INamedTypeSymbol;
        if (symbol is null || !symbol.IsValueType) return null;
        return GetPrimitiveInfoFromSymbol(symbol);
    }

    private static PrimitiveInfo? GetPrimitiveInfoFromSymbol(INamedTypeSymbol symbol)
    {
        if (!symbol.IsValueType) return null;

        // Priority 1: IEntityId<TSelf> or IEntityId -- Guid-backed entity ID
        var entityIdIface = symbol.AllInterfaces.FirstOrDefault(i =>
            i.Name == "IEntityId" && (i.TypeArguments.Length == 1 || i.TypeArguments.Length == 0));
        if (entityIdIface != null)
        {
            return new PrimitiveInfo(
                symbol.ContainingNamespace.ToDisplayString(),
                symbol.Name,
                "global::System.Guid",
                false);
        }

        // Priority 2: IStrongId<TSelf, TValue>
        var strongIdIface = symbol.AllInterfaces.FirstOrDefault(i =>
            i.Name == "IStrongId" && i.TypeArguments.Length == 2);
        if (strongIdIface != null)
        {
            var backingType = strongIdIface.TypeArguments[1].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            return new PrimitiveInfo(
                symbol.ContainingNamespace.ToDisplayString(),
                symbol.Name,
                backingType,
                false);
        }

        // Priority 3: Convention -- *Id struct with Value property and static Create(TValue)
        if (symbol.Name.EndsWith("Id", StringComparison.Ordinal))
        {
            var valueProp = symbol.GetMembers("Value").OfType<IPropertySymbol>().FirstOrDefault();
            if (valueProp != null)
            {
                var backingType = valueProp.Type;
                var hasCreateMethod = symbol.GetMembers("Create").OfType<IMethodSymbol>()
                    .Any(m => m.IsStatic
                              && m.Parameters.Length == 1
                              && m.Parameters[0].Type.Equals(backingType, SymbolEqualityComparer.Default));
                if (!hasCreateMethod) return null;

                return new PrimitiveInfo(
                    symbol.ContainingNamespace.ToDisplayString(),
                    symbol.Name,
                    backingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                    false);
            }
        }

        return null;
    }

    private static void Execute(
        Compilation compilation,
        ImmutableArray<PrimitiveInfo> primitives,
        SourceProductionContext context)
    {
        if (primitives.IsDefaultOrEmpty) return;

        var converterClasses = new List<string>();
        var distinctPrimitives = primitives.Distinct().ToList();

        var seenTypeNames = new HashSet<string>(StringComparer.Ordinal);
        var seenFullNames = new HashSet<string>(StringComparer.Ordinal);

        foreach (var primitive in distinctPrimitives)
        {
            var fullName = $"{primitive.Namespace}.{primitive.TypeName}";
            if (!seenFullNames.Add(fullName))
                continue;

            var safeNamespace = primitive.Namespace == "<global namespace>"
                ? "Global"
                : primitive.Namespace.Replace(".", "_").Replace("<", "").Replace(">", "");

            var converterClassName = seenTypeNames.Add(primitive.TypeName)
                ? $"{primitive.TypeName}JsonConverter"
                : $"{safeNamespace}_{primitive.TypeName}JsonConverter";

            converterClasses.Add(converterClassName);

            var hintName = $"{safeNamespace}_{converterClassName}.g.cs";

            var code = GenerateJsonConverter(primitive, converterClassName);
            context.AddSource(hintName, SourceText.From(code, Encoding.UTF8));
        }

        var regCode = GenerateRegistration(converterClasses);
        context.AddSource("DomainPrimitivesJsonRegistration.g.cs", SourceText.From(regCode, Encoding.UTF8));
    }

    internal static string GenerateJsonConverter(PrimitiveInfo primitive, string? converterClassName = null)
    {
        converterClassName ??= $"{primitive.TypeName}JsonConverter";
        var fullyQualifiedTypeName = primitive.Namespace == "<global namespace>"
            ? primitive.TypeName
            : $"global::{primitive.Namespace}.{primitive.TypeName}";

        var backingWithoutGlobal = primitive.BackingType.Replace("global::", "");
        var sb = new StringBuilder();

        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("#nullable enable");
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Text.Json;");
        sb.AppendLine("using System.Text.Json.Serialization;");
        sb.AppendLine();
        sb.AppendLine("namespace EricksonLopez.DomainPrimitives.Dapper.Generated;");
        sb.AppendLine();
        sb.AppendLine("[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]");
        sb.AppendLine($"internal sealed class {converterClassName} : JsonConverter<{fullyQualifiedTypeName}>");
        sb.AppendLine("{");

        // ── Read ──────────────────────────────────────────────────────────────
        sb.AppendLine($"    public override {fullyQualifiedTypeName} Read(ref Utf8JsonReader reader, global::System.Type typeToConvert, JsonSerializerOptions options)");
        sb.AppendLine("    {");

        string factoryMethod = primitive.IsSmartEnum ? "FromValue" : "Create";

        if (backingWithoutGlobal is "Guid" or "System.Guid")
        {
            sb.AppendLine($"        if (reader.TokenType == JsonTokenType.Null) throw new JsonException(\"Cannot deserialize null as {primitive.TypeName}.\");");
            sb.AppendLine($"        return {fullyQualifiedTypeName}.{factoryMethod}(reader.GetGuid());");
        }
        else if (backingWithoutGlobal == "string")
        {
            sb.AppendLine($"        if (reader.TokenType == JsonTokenType.Null) throw new JsonException(\"Cannot deserialize null as {primitive.TypeName}.\");");
            sb.AppendLine($"        return {fullyQualifiedTypeName}.{factoryMethod}(reader.GetString() ?? string.Empty);");
        }
        else if (backingWithoutGlobal is "int" or "Int32" or "System.Int32")
        {
            sb.AppendLine($"        return {fullyQualifiedTypeName}.{factoryMethod}(reader.GetInt32());");
        }
        else if (backingWithoutGlobal is "long" or "Int64" or "System.Int64")
        {
            sb.AppendLine($"        return {fullyQualifiedTypeName}.{factoryMethod}(reader.GetInt64());");
        }
        else if (backingWithoutGlobal is "short" or "Int16" or "System.Int16")
        {
            sb.AppendLine($"        return {fullyQualifiedTypeName}.{factoryMethod}((short)reader.GetInt32());");
        }
        else if (backingWithoutGlobal is "byte" or "Byte" or "System.Byte")
        {
            sb.AppendLine($"        return {fullyQualifiedTypeName}.{factoryMethod}(reader.GetByte());");
        }
        else if (backingWithoutGlobal is "decimal" or "Decimal" or "System.Decimal")
        {
            sb.AppendLine($"        return {fullyQualifiedTypeName}.{factoryMethod}(reader.GetDecimal());");
        }
        else if (backingWithoutGlobal is "System.DateOnly" or "DateOnly")
        {
            sb.AppendLine($"        var s = reader.GetString() ?? throw new JsonException(\"Cannot deserialize null as {primitive.TypeName}.\");");
            sb.AppendLine($"        return {fullyQualifiedTypeName}.{factoryMethod}(global::System.DateOnly.Parse(s, global::System.Globalization.CultureInfo.InvariantCulture));");
        }
        else if (backingWithoutGlobal is "System.DateTimeOffset" or "DateTimeOffset")
        {
            sb.AppendLine($"        return {fullyQualifiedTypeName}.{factoryMethod}(reader.GetDateTimeOffset());");
        }
        else if (backingWithoutGlobal is "System.DateTime" or "DateTime")
        {
            sb.AppendLine($"        return {fullyQualifiedTypeName}.{factoryMethod}(reader.GetDateTime());");
        }
        else
        {
            sb.AppendLine($"        throw new JsonException(\"JsonConverter for {primitive.TypeName} does not support backing type {primitive.BackingType}.\");");
        }

        sb.AppendLine("    }");
        sb.AppendLine();

        // ── Write ─────────────────────────────────────────────────────────────
        sb.AppendLine($"    public override void Write(Utf8JsonWriter writer, {fullyQualifiedTypeName} value, JsonSerializerOptions options)");
        sb.AppendLine("    {");

        if (backingWithoutGlobal is "Guid" or "System.Guid" or "string")
        {
            sb.AppendLine("        writer.WriteStringValue(value.Value);");
        }
        else if (backingWithoutGlobal is "int" or "Int32" or "System.Int32"
                                       or "long" or "Int64" or "System.Int64"
                                       or "short" or "Int16" or "System.Int16"
                                       or "byte" or "Byte" or "System.Byte"
                                       or "decimal" or "Decimal" or "System.Decimal")
        {
            sb.AppendLine("        writer.WriteNumberValue(value.Value);");
        }
        else if (backingWithoutGlobal is "System.DateOnly" or "DateOnly")
        {
            sb.AppendLine("        writer.WriteStringValue(value.Value.ToString(\"O\", global::System.Globalization.CultureInfo.InvariantCulture));");
        }
        else if (backingWithoutGlobal is "System.DateTimeOffset" or "DateTimeOffset")
        {
            sb.AppendLine("        writer.WriteStringValue(value.Value);");
        }
        else if (backingWithoutGlobal is "System.DateTime" or "DateTime")
        {
            sb.AppendLine("        writer.WriteStringValue(value.Value);");
        }
        else
        {
            sb.AppendLine($"        throw new JsonException(\"JsonConverter for {primitive.TypeName} does not support backing type {primitive.BackingType}.\");");
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }

    internal static string GenerateRegistration(IReadOnlyList<string> converterClasses)
    {
        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("#nullable enable");
        sb.AppendLine("using System.Text.Json;");
        sb.AppendLine();
        sb.AppendLine("namespace EricksonLopez.DomainPrimitives.Dapper.Generated;");
        sb.AppendLine();
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// Automatically registers all System.Text.Json converters for Domain Primitives.");
        sb.AppendLine("/// Call RegisterAll(JsonSerializerOptions) once at application startup.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine("/// <remarks>");
        sb.AppendLine("/// AOT compatible: all converters are explicitly instantiated, no runtime reflection.");
        sb.AppendLine("/// Thread-safe: uses double-checked locking. Safe to call multiple times.");
        sb.AppendLine("/// </remarks>");
        sb.AppendLine("[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]");
        sb.AppendLine("public static class DomainPrimitivesJsonRegistration");
        sb.AppendLine("{");
        sb.AppendLine("    private static bool _registered;");
        sb.AppendLine("    private static readonly object _lock = new object();");
        sb.AppendLine();
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Registers all generated JsonConverter instances into the provided JsonSerializerOptions.");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public static void RegisterAll(global::System.Text.Json.JsonSerializerOptions options)");
        sb.AppendLine("    {");
        sb.AppendLine("        global::System.ArgumentNullException.ThrowIfNull(options);");
        sb.AppendLine("        if (_registered) return;");
        sb.AppendLine("        lock (_lock)");
        sb.AppendLine("        {");
        sb.AppendLine("            if (_registered) return;");
        foreach (var converter in converterClasses)
        {
            sb.AppendLine($"            options.Converters.Add(new {converter}());");
        }
        sb.AppendLine("            _registered = true;");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }
}
