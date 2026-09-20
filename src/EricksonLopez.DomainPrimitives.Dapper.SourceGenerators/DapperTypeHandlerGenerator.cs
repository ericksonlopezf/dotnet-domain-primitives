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

[Generator(LanguageNames.CSharp)]
internal sealed class DapperTypeHandlerGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Pipeline 1: Find struct/record declarations in the CURRENT project's syntax trees.
        var syntaxPrimitives = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => s is StructDeclarationSyntax or RecordDeclarationSyntax,
                transform: static (ctx, _) => GetDomainPrimitiveInfo(ctx))
            .Where(static m => m.HasValue)
            .Select(static (m, _) => m!.Value);

        // Pipeline 2: Walk ALL referenced assemblies (e.g. Domain project) via CompilationProvider
        // to discover IEntityId<TSelf>, IStrongId<TSelf, TValue>, and Value Object structs that live in
        // referenced projects and are NOT part of the current compilation's syntax trees.
        var referencedPrimitives = context.CompilationProvider
            .Select(static (compilation, _) => GetReferencedAssemblyPrimitives(compilation));

        // Merge both pipelines: syntax-based (current project) + symbol-based (referenced assemblies)
        var compilationAndStructs = context.CompilationProvider
            .Combine(syntaxPrimitives.Collect())
            .Combine(referencedPrimitives);

        // Generate the handlers and the registration class
        context.RegisterSourceOutput(compilationAndStructs,
            (spc, source) =>
            {
                var compilation = source.Left.Left;
                var syntaxFound = source.Left.Right;
                var referencedFound = source.Right;

                // Merge and deduplicate by (Namespace, TypeName)
                var all = syntaxFound
                    .Concat(referencedFound)
                    .Distinct()
                    .ToImmutableArray();

                Execute(compilation, all, spc);
            });
    }

    /// <summary>
    /// Walks all types in referenced assemblies (not the current compilation's source)
    /// to discover domain primitive structs implementing IEntityId&lt;TSelf&gt;, IStrongId&lt;TSelf,TValue&gt;,
    /// or Value Object structs with a static Create(T) method and a Value property.
    /// This enables the SourceGenerator to handle types defined in the Domain project when
    /// applied as an Analyzer to the Infrastructure project.
    /// </summary>
    private static ImmutableArray<PrimitiveInfo> GetReferencedAssemblyPrimitives(Compilation compilation)
    {
        var results = new List<PrimitiveInfo>();

        // Walk all referenced assemblies (metadata references)
        foreach (var reference in compilation.References)
        {
            if (compilation.GetAssemblyOrModuleSymbol(reference) is not IAssemblySymbol assemblySymbol)
                continue;

            // Skip well-known system, framework, and third-party infrastructure assemblies.
            // Only domain assemblies (project-specific, no well-known library prefix) should
            // be walked for TypeHandler generation.
            var assemblyName = assemblySymbol.Name;
            if (assemblyName.StartsWith("System", StringComparison.Ordinal)
                || assemblyName.StartsWith("Microsoft", StringComparison.Ordinal)
                || assemblyName.StartsWith("netstandard", StringComparison.Ordinal)
                || assemblyName.StartsWith("mscorlib", StringComparison.Ordinal)
                || assemblyName.StartsWith("Dapper", StringComparison.Ordinal)
                || assemblyName.StartsWith("EricksonLopez.DomainPrimitives", StringComparison.Ordinal)
                || assemblyName.StartsWith("Npgsql", StringComparison.Ordinal)
                || assemblyName.StartsWith("Polly", StringComparison.Ordinal)
                || assemblyName.StartsWith("FluentValidation", StringComparison.Ordinal)
                || assemblyName.StartsWith("Serilog", StringComparison.Ordinal)
                || assemblyName.StartsWith("OpenTelemetry", StringComparison.Ordinal)
                || assemblyName.StartsWith("Azure", StringComparison.Ordinal)
                || assemblyName.StartsWith("AWSSDK", StringComparison.Ordinal)
                || assemblyName.StartsWith("StackExchange", StringComparison.Ordinal)
                || assemblyName.StartsWith("Newtonsoft", StringComparison.Ordinal)
                || assemblyName.StartsWith("AutoMapper", StringComparison.Ordinal)
                || assemblyName.StartsWith("MediatR", StringComparison.Ordinal)
                || assemblyName.StartsWith("Mapster", StringComparison.Ordinal)
                || assemblyName.StartsWith("Bogus", StringComparison.Ordinal)
                || assemblyName.StartsWith("xunit", StringComparison.Ordinal)
                || assemblyName.StartsWith("NUnit", StringComparison.Ordinal)
                || assemblyName.StartsWith("Moq", StringComparison.Ordinal)
                || assemblyName.StartsWith("NSubstitute", StringComparison.Ordinal))
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

    internal static PrimitiveInfo? GetPrimitiveInfoFromSymbol(INamedTypeSymbol symbol)
    {
        if (!symbol.IsValueType) return null;

        // Check for IStrongId<TSelf, TValue>, IStrongId<TSelf>, or IEntityId<TSelf>
        var strongIdIface = symbol.AllInterfaces.FirstOrDefault(i =>
            (i.Name is "IStrongId" or "IEntityId") && (i.TypeArguments.Length == 1 || i.TypeArguments.Length == 2));
        if (strongIdIface != null)
        {
            string backingType;
            if (strongIdIface.TypeArguments.Length == 2)
            {
                backingType = strongIdIface.TypeArguments[1].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            }
            else
            {
                var valueProp = symbol.GetMembers("Value").OfType<IPropertySymbol>().FirstOrDefault();
                backingType = valueProp != null
                    ? valueProp.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                    : "global::System.Guid";
            }
            return new PrimitiveInfo(
                symbol.ContainingNamespace.ToDisplayString(),
                symbol.Name,
                backingType,
                isSmartEnum: false,
                isResultReturning: false);
        }

        // Naming convention fallback: *Id struct with a Value property AND a static Create(TValue) method.
        // The Create check prevents emitting broken TypeHandlers for types that use only constructors.
        if (symbol.Name.EndsWith("Id", StringComparison.Ordinal))
        {
            var valueProp = symbol.GetMembers("Value").OfType<IPropertySymbol>().FirstOrDefault();
            if (valueProp != null)
            {
                // Validate that Create(TValue) static method exists — required by TypeHandler code generation
                var backingType = valueProp.Type;
                var createMethod = symbol.GetMembers("Create").OfType<IMethodSymbol>()
                    .FirstOrDefault(m => m.IsStatic
                              && m.Parameters.Length == 1
                              && m.Parameters[0].Type.Equals(backingType, SymbolEqualityComparer.Default));
                if (createMethod == null)
                    return null; // Skip types without Create() — they cannot be registered by SourceGen

                bool isResultReturning = IsResultType(createMethod.ReturnType);
                return new PrimitiveInfo(
                    symbol.ContainingNamespace.ToDisplayString(),
                    symbol.Name,
                    backingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                    isSmartEnum: false,
                    isResultReturning: isResultReturning);
            }
        }

        // Value Object convention: any value-type (readonly record struct) with
        //   - a single Value property of any scalar type
        //   - a static Create(backingType) method
        //   - does NOT end in Id (those are handled above)
        //   - is public
        //
        // This covers OpusHydra-style VOs: BranchCode, Email, Rfc, PhoneNumber, TaxRate, etc.
        if (!symbol.Name.EndsWith("Id", StringComparison.Ordinal)
            && symbol.DeclaredAccessibility == Accessibility.Public
            && symbol.IsReadOnly)
        {
            var valueProp = symbol.GetMembers("Value").OfType<IPropertySymbol>().FirstOrDefault();
            if (valueProp != null)
            {
                var backingType = valueProp.Type;
                // Must be a scalar type (string, int, decimal, Guid, etc.) — not another VO
                if (IsScalarType(backingType))
                {
                    var createMethod = symbol.GetMembers("Create").OfType<IMethodSymbol>()
                        .FirstOrDefault(m => m.IsStatic
                                  && m.Parameters.Length == 1
                                  && m.Parameters[0].Type.Equals(backingType, SymbolEqualityComparer.Default));
                    if (createMethod != null)
                    {
                        bool isResultReturning = IsResultType(createMethod.ReturnType);
                        return new PrimitiveInfo(
                            symbol.ContainingNamespace.ToDisplayString(),
                            symbol.Name,
                            backingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                            isSmartEnum: false,
                            isResultReturning: isResultReturning);
                    }
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Returns <see langword="true"/> when <paramref name="returnType"/> is a known Result wrapper type
    /// (i.e., its name starts with "Result" — covering Result&lt;T&gt;, Result, IResult&lt;T&gt;, etc.).
    /// This is intentionally name-based rather than interface-based for cross-assembly resilience:
    /// the Roslyn metadata symbol for the Result type may not be reachable from the generator.
    /// </summary>
    internal static bool IsResultType(ITypeSymbol returnType)
    {
        return returnType.Name == "Result";
    }

    /// <summary>
    /// Returns <see langword="true"/> when <paramref name="type"/> is a scalar type suitable for
    /// a simple TypeHandler: string, Guid, numeric types, DateOnly, DateTime, DateTimeOffset, TimeOnly, bool.
    /// Excludes complex types (classes, other VOs) that would require compound mapping.
    /// </summary>
    internal static bool IsScalarType(ITypeSymbol type)
    {
        if (type.SpecialType != SpecialType.None)
        {
            // Covers string, bool, all numeric primitives, char
            return type.SpecialType is SpecialType.System_String
                or SpecialType.System_Boolean
                or SpecialType.System_Byte
                or SpecialType.System_SByte
                or SpecialType.System_Int16
                or SpecialType.System_UInt16
                or SpecialType.System_Int32
                or SpecialType.System_UInt32
                or SpecialType.System_Int64
                or SpecialType.System_UInt64
                or SpecialType.System_Decimal
                or SpecialType.System_Single
                or SpecialType.System_Double
                or SpecialType.System_Char;
        }

        // Covers System.Guid, System.DateOnly, System.DateTime, System.DateTimeOffset, System.TimeOnly
        var displayName = type.ToDisplayString();
        return displayName is "System.Guid"
            or "System.DateOnly"
            or "System.DateTime"
            or "System.DateTimeOffset"
            or "System.TimeOnly";
    }

    internal static bool IsDomainPrimitiveAttribute(AttributeData a)
    {
        var ns = a.AttributeClass?.ContainingNamespace?.ToDisplayString();
        if (ns == null || !ns.StartsWith("EricksonLopez.DomainPrimitives", StringComparison.Ordinal))
            return false;

        var name = a.AttributeClass?.Name;
        return name is not "DapperAttribute" and not "EFCoreAttribute" and not "DomainPrimitivesDefaultsAttribute" and not "AspNetCoreAttribute" and not "ValueObjectAttribute";
    }

    private static PrimitiveInfo? GetDomainPrimitiveInfo(GeneratorSyntaxContext context)
    {
        var typeDecl = (TypeDeclarationSyntax)context.Node;
        
        var symbol = context.SemanticModel.GetDeclaredSymbol(typeDecl) as INamedTypeSymbol;
        if (symbol == null) return null;

        // Ensure it's a value type
        if (!symbol.IsValueType) return null;

        // Look for attributes from our abstractions
        var attributes = symbol.GetAttributes();
        var primitiveAttr = attributes.FirstOrDefault(IsDomainPrimitiveAttribute);
        if (primitiveAttr == null)
        {
            return GetPrimitiveInfoFromSymbol(symbol);
        }

        string backingTypeAttr = "string"; // Default for StringPrimitive and most shortcuts
        var attrClass = primitiveAttr.AttributeClass!;
        var attrName = attrClass.Name;

        if (attrName is "StrongIdAttribute" or "NumericPrimitiveAttribute")
        {
            if (attrClass.IsGenericType)
            {
                var typeArg = attrClass.TypeArguments[0];
                backingTypeAttr = typeArg.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            }
        }
        else if (attrName == "DatePrimitiveAttribute")
        {
            // By default DateOnly
            backingTypeAttr = "global::System.DateOnly";
            var kindArg = primitiveAttr.NamedArguments.FirstOrDefault(kvp => kvp.Key == "Kind").Value;
            if (!kindArg.IsNull && kindArg.Value is int kindInt)
            {
                // 0 = DateOnly, 1 = DateTime, 2 = TimeOnly, 3 = DateTimeOffset
                if (kindInt == 1) backingTypeAttr = "global::DateTime";
                else if (kindInt == 2) backingTypeAttr = "global::System.TimeOnly";
                else if (kindInt == 3) backingTypeAttr = "global::System.DateTimeOffset";
            }
        }
        else if (attrName is "MoneyAttribute" or "PercentageAttribute")
        {
            backingTypeAttr = "decimal";
        }
        else if (attrName == "SmartEnumAttribute")
        {
            if (attrClass.IsGenericType)
            {
                var typeArg = attrClass.TypeArguments[0];
                backingTypeAttr = typeArg.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            }
            else
            {
                backingTypeAttr = "int"; // Default if not generic (though it should be)
            }
        }

        return new PrimitiveInfo(
            symbol.ContainingNamespace.ToDisplayString(),
            symbol.Name,
            backingTypeAttr,
            attrName == "SmartEnumAttribute",
            isResultReturning: false  // Attribute-driven types use direct Create() by convention
        );
    }

    private static void Execute(Compilation compilation, ImmutableArray<PrimitiveInfo> primitives, SourceProductionContext context)
    {
        if (primitives.IsDefaultOrEmpty) return;

        var handlerClasses = new List<string>();
        var distinctPrimitives = primitives.Distinct().ToList();

        var seenTypeNames = new HashSet<string>(StringComparer.Ordinal);
        var seenFullNames = new HashSet<string>(StringComparer.Ordinal);

        foreach (var primitive in distinctPrimitives)
        {
            var fullName = $"{primitive.Namespace}.{primitive.TypeName}";
            if (!seenFullNames.Add(fullName))
                continue;

            var safeNamespace = GetSafeNamespace(primitive.Namespace);

            var handlerClassName = seenTypeNames.Add(primitive.TypeName)
                ? $"{primitive.TypeName}TypeHandler"
                : $"{safeNamespace}_{primitive.TypeName}TypeHandler";

            handlerClasses.Add(handlerClassName);

            var hintName = $"{safeNamespace}_{handlerClassName}.g.cs";

            var code = GenerateTypeHandler(primitive, handlerClassName);
            context.AddSource(hintName, SourceText.From(code, Encoding.UTF8));
        }

        var regCode = GenerateRegistration(handlerClasses);
        context.AddSource("DapperDomainPrimitivesRegistration.g.cs", SourceText.From(regCode, Encoding.UTF8));
    }

    internal static string GetSafeNamespace(string ns) =>
        ns == "<global namespace>"
            ? "Global"
            : ns.Replace(".", "_").Replace("<", "").Replace(">", "");

    internal static string GenerateTypeHandler(PrimitiveInfo primitive, string? handlerClassName = null)
    {
        handlerClassName ??= $"{primitive.TypeName}TypeHandler";
        // Use fully-qualified name to avoid CS0104 ambiguous reference when same simple name
        // exists in multiple namespaces within the compilation (e.g. ProductId in multiple BCs).
        var fullyQualifiedTypeName = primitive.Namespace == "<global namespace>"
            ? primitive.TypeName
            : $"global::{primitive.Namespace}.{primitive.TypeName}";
        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("#nullable enable");
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Data;");
        sb.AppendLine("using Dapper;");
        if (primitive.Namespace != "<global namespace>")
        {
            sb.AppendLine($"using {primitive.Namespace};");
        }
        sb.AppendLine();
        sb.AppendLine("namespace EricksonLopez.DomainPrimitives.Dapper.Generated;");
        sb.AppendLine();
        sb.AppendLine("[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]");
        sb.AppendLine($"internal sealed class {handlerClassName} : SqlMapper.TypeHandler<{fullyQualifiedTypeName}>");
        sb.AppendLine("{");
        
        // SetValue
        sb.AppendLine($"    public override void SetValue(IDbDataParameter parameter, {fullyQualifiedTypeName} value)");
        sb.AppendLine("    {");
        if (primitive.IsSmartEnum)
        {
            sb.AppendLine("        if (value is null)");
            sb.AppendLine("        {");
            sb.AppendLine("            parameter.Value = global::System.DBNull.Value;");
            sb.AppendLine("            return;");
            sb.AppendLine("        }");
        }
        else
        {
            sb.AppendLine("        if (value.IsDefault)");
            sb.AppendLine("        {");
            sb.AppendLine("            parameter.Value = global::System.DBNull.Value;");
            sb.AppendLine("            return;");
            sb.AppendLine("        }");
        }
        sb.AppendLine("        parameter.Value = value.Value;");
        sb.AppendLine("    }");
        sb.AppendLine();

        // Parse — use object? to satisfy #nullable enable and match the Dapper base class override
        sb.AppendLine($"    public override {fullyQualifiedTypeName} Parse(object? value)");
        sb.AppendLine("    {");
        sb.AppendLine("        if (value is null || value == DBNull.Value)");
        sb.AppendLine($"            throw new DataException($\"Cannot parse null as {primitive.TypeName}.\");");
        sb.AppendLine();

        string factoryMethod = primitive.IsSmartEnum ? "FromValue" : "Create";

        string backingWithoutGlobal = primitive.BackingType.Replace("global::", "");
        if (backingWithoutGlobal is "Guid" or "System.Guid")
        {
            AppendFactoryCall(sb, fullyQualifiedTypeName, factoryMethod, primitive.IsResultReturning, primitive.TypeName,
                lines =>
                {
                    lines.Add($"        if (value is Guid g) return {fullyQualifiedTypeName}.{factoryMethod}(g){(primitive.IsResultReturning ? ".Value" : "")};");
                    lines.Add($"        if (value is string s && Guid.TryParse(s, out var parsed)) return {fullyQualifiedTypeName}.{factoryMethod}(parsed){(primitive.IsResultReturning ? ".Value" : "")};");
                    lines.Add($"        if (value is byte[] b && b.Length == 16) return {fullyQualifiedTypeName}.{factoryMethod}(new Guid(b)){(primitive.IsResultReturning ? ".Value" : "")};");
                });
        }
        else if (backingWithoutGlobal == "string")
        {
            if (primitive.IsResultReturning)
            {
                sb.AppendLine($"        var raw = value is string s ? s : value.ToString() ?? string.Empty;");
                sb.AppendLine($"        var result = {fullyQualifiedTypeName}.{factoryMethod}(raw);");
                sb.AppendLine($"        if (result.IsFailure)");
                sb.AppendLine($"            throw new DataException($\"Invalid {primitive.TypeName} in database: {{result.Error.Description}}\");");
                sb.AppendLine($"        return result.Value;");
            }
            else
            {
                sb.AppendLine($"        if (value is string s) return {fullyQualifiedTypeName}.{factoryMethod}(s);");
                sb.AppendLine($"        return {fullyQualifiedTypeName}.{factoryMethod}(value.ToString() ?? string.Empty);");
            }
        }
        else if (backingWithoutGlobal is "System.DateOnly" or "DateOnly")
        {
            if (primitive.IsResultReturning)
            {
                sb.AppendLine($"        if (value is DateOnly d)");
                sb.AppendLine($"        {{");
                sb.AppendLine($"            var res = {fullyQualifiedTypeName}.{factoryMethod}(d);");
                sb.AppendLine($"            if (res.IsFailure) throw new DataException($\"Invalid {primitive.TypeName} in database: {{res.Error.Description}}\");");
                sb.AppendLine($"            return res.Value;");
                sb.AppendLine($"        }}");
                sb.AppendLine($"        if (value is DateTime dt)");
                sb.AppendLine($"        {{");
                sb.AppendLine($"            var res = {fullyQualifiedTypeName}.{factoryMethod}(DateOnly.FromDateTime(dt));");
                sb.AppendLine($"            if (res.IsFailure) throw new DataException($\"Invalid {primitive.TypeName} in database: {{res.Error.Description}}\");");
                sb.AppendLine($"            return res.Value;");
                sb.AppendLine($"        }}");
                sb.AppendLine($"        if (value is string s && DateOnly.TryParse(s, out var parsed))");
                sb.AppendLine($"        {{");
                sb.AppendLine($"            var res = {fullyQualifiedTypeName}.{factoryMethod}(parsed);");
                sb.AppendLine($"            if (res.IsFailure) throw new DataException($\"Invalid {primitive.TypeName} in database: {{res.Error.Description}}\");");
                sb.AppendLine($"            return res.Value;");
                sb.AppendLine($"        }}");
            }
            else
            {
                sb.AppendLine($"        if (value is DateOnly d) return {fullyQualifiedTypeName}.{factoryMethod}(d);");
                sb.AppendLine($"        if (value is DateTime dt) return {fullyQualifiedTypeName}.{factoryMethod}(DateOnly.FromDateTime(dt));");
                sb.AppendLine($"        if (value is string s && DateOnly.TryParse(s, out var parsed)) return {fullyQualifiedTypeName}.{factoryMethod}(parsed);");
            }
        }
        else
        {
            // Numerics and others - try to change type
            sb.AppendLine($"        try");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            var converted = ({primitive.BackingType})Convert.ChangeType(value, typeof({primitive.BackingType}));");
            if (primitive.IsResultReturning)
            {
                sb.AppendLine($"            var result = {fullyQualifiedTypeName}.{factoryMethod}(converted);");
                sb.AppendLine($"            if (result.IsFailure)");
                sb.AppendLine($"                throw new DataException($\"Invalid {primitive.TypeName} in database: {{result.Error.Description}}\");");
                sb.AppendLine($"            return result.Value;");
            }
            else
            {
                sb.AppendLine($"            return {fullyQualifiedTypeName}.{factoryMethod}(converted);");
            }
            sb.AppendLine($"        }}");
            sb.AppendLine($"        catch (InvalidCastException)");
            sb.AppendLine($"        {{");
            sb.AppendLine($"        }}");
        }

        sb.AppendLine();
        sb.AppendLine($"        throw new DataException($\"Cannot parse {{value!.GetType()}} as {primitive.TypeName}.\");");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }

    /// <summary>
    /// Helper to avoid duplicating Result&lt;T&gt;-path logic for Guid-backed types.
    /// For Guid types, the Result-returning path is handled inline with .Value suffix.
    /// </summary>
    private static void AppendFactoryCall(StringBuilder sb, string fqn, string factoryMethod, bool isResultReturning, string typeName, Action<List<string>> buildLines)
    {
        // For Guid-backed Result types, the inline .Value approach is simpler and avoids
        // generating a nested if/result/check block for each Guid format variant.
        // The .Value access is safe here because TryParse already validates the input.
        var lines = new List<string>();
        buildLines(lines);
        foreach (var line in lines)
            sb.AppendLine(line);
    }

    internal static string GenerateRegistration(IReadOnlyList<string> handlerClasses)
    {
        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("using Dapper;");
        sb.AppendLine();
        sb.AppendLine("namespace EricksonLopez.DomainPrimitives.Dapper.Generated;");
        sb.AppendLine();
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// Automatically registers all Dapper TypeHandlers for Domain Primitives.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine("[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]");
        sb.AppendLine("public static class DapperDomainPrimitivesRegistration");
        sb.AppendLine("{");
        sb.AppendLine("    private static bool _registered;");
        sb.AppendLine("    private static readonly object _lock = new object();");
        sb.AppendLine();
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Registers all generated TypeHandlers into Dapper's SqlMapper.");
        sb.AppendLine("    /// Can be called safely multiple times.");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    /// <remarks>");
        sb.AppendLine("    /// <para><strong>BREAKING CHANGE (v3.0):</strong></para>");
        sb.AppendLine("    /// <para>When Dapper encounters <c>DBNull.Value</c>, it will now throw a <c>DataException</c>");
        sb.AppendLine("    /// instead of falling back to <c>default(T)</c>, unless the target property is nullable.</para>");
        sb.AppendLine("    /// Ensure your database schemas match your domain primitive nullability.</para>");
        sb.AppendLine("    /// </remarks>");
        sb.AppendLine("    public static void RegisterAll()");
        sb.AppendLine("    {");
        sb.AppendLine("        if (_registered) return;");
        sb.AppendLine("        lock (_lock)");
        sb.AppendLine("        {");
        sb.AppendLine("            if (_registered) return;");
        foreach (var handler in handlerClasses)
        {
            sb.AppendLine($"            SqlMapper.AddTypeHandler(new {handler}());");
        }
        sb.AppendLine("            _registered = true;");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }
}
