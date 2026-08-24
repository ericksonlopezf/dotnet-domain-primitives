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
        // Find all struct declarations
        var structDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => s is StructDeclarationSyntax or RecordDeclarationSyntax,
                transform: static (ctx, _) => GetDomainPrimitiveInfo(ctx))
            .Where(static m => m.HasValue)
            .Select(static (m, _) => m!.Value);

        // Collect all of them into a single compilation step
        var compilationAndStructs = context.CompilationProvider.Combine(structDeclarations.Collect());

        // Generate the handlers and the registration class
        context.RegisterSourceOutput(compilationAndStructs,
            (spc, source) => Execute(source.Left, source.Right, spc));
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
        if (primitiveAttr == null) return null;

        string backingType = "string"; // Default for StringPrimitive and most shortcuts
        var attrClass = primitiveAttr.AttributeClass!;
        var attrName = attrClass.Name;

        if (attrName is "StrongIdAttribute" or "NumericPrimitiveAttribute")
        {
            if (attrClass.IsGenericType)
            {
                var typeArg = attrClass.TypeArguments[0];
                backingType = typeArg.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            }
        }
        else if (attrName == "DatePrimitiveAttribute")
        {
            // By default DateOnly
            backingType = "global::System.DateOnly";
            var kindArg = primitiveAttr.NamedArguments.FirstOrDefault(kvp => kvp.Key == "Kind").Value;
            if (!kindArg.IsNull && kindArg.Value is int kindInt)
            {
                // 0 = DateOnly, 1 = DateTime, 2 = TimeOnly, 3 = DateTimeOffset
                if (kindInt == 1) backingType = "global::DateTime";
                else if (kindInt == 2) backingType = "global::System.TimeOnly";
                else if (kindInt == 3) backingType = "global::System.DateTimeOffset";
            }
        }
        else if (attrName is "MoneyAttribute" or "PercentageAttribute")
        {
            backingType = "decimal";
        }
        else if (attrName == "SmartEnumAttribute")
        {
            if (attrClass.IsGenericType)
            {
                var typeArg = attrClass.TypeArguments[0];
                backingType = typeArg.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            }
            else
            {
                backingType = "int"; // Default if not generic (though it should be)
            }
        }

        return new PrimitiveInfo(
            symbol.ContainingNamespace.ToDisplayString(),
            symbol.Name,
            backingType,
            attrName == "SmartEnumAttribute"
        );
    }

    private static void Execute(Compilation compilation, ImmutableArray<PrimitiveInfo> primitives, SourceProductionContext context)
    {
        if (primitives.IsDefaultOrEmpty) return;

        var handlerClasses = new List<string>();

        foreach (var primitive in primitives.Distinct())
        {
            var handlerClassName = $"{primitive.TypeName}TypeHandler";
            handlerClasses.Add(handlerClassName);

            var code = GenerateTypeHandler(primitive);
            context.AddSource($"{handlerClassName}.g.cs", SourceText.From(code, Encoding.UTF8));
        }

        var regCode = GenerateRegistration(handlerClasses);
        context.AddSource("DapperDomainPrimitivesRegistration.g.cs", SourceText.From(regCode, Encoding.UTF8));
    }

    internal static string GenerateTypeHandler(PrimitiveInfo primitive)
    {
        var handlerClassName = $"{primitive.TypeName}TypeHandler";
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
        sb.AppendLine($"internal sealed class {handlerClassName} : SqlMapper.TypeHandler<{primitive.TypeName}>");
        sb.AppendLine("{");
        
        // SetValue
        sb.AppendLine($"    public override void SetValue(IDbDataParameter parameter, {primitive.TypeName} value)");
        sb.AppendLine("    {");
        sb.AppendLine("        parameter.Value = value.Value;");
        sb.AppendLine("    }");
        sb.AppendLine();

        // Parse
        sb.AppendLine($"    public override {primitive.TypeName} Parse(object value)");
        sb.AppendLine("    {");
        sb.AppendLine("        if (value is null || value == DBNull.Value)");
        sb.AppendLine($"            throw new DataException($\"Cannot parse null as {primitive.TypeName}.\");");
        sb.AppendLine();

        string factoryMethod = primitive.IsSmartEnum ? "FromValue" : "Create";

        string backingWithoutGlobal = primitive.BackingType.Replace("global::", "");
        if (backingWithoutGlobal is "Guid" or "System.Guid")
        {
            sb.AppendLine("        if (value is Guid g) return " + primitive.TypeName + $".{factoryMethod}(g);");
            sb.AppendLine("        if (value is string s && Guid.TryParse(s, out var parsed)) return " + primitive.TypeName + $".{factoryMethod}(parsed);");
            sb.AppendLine("        if (value is byte[] b && b.Length == 16) return " + primitive.TypeName + $".{factoryMethod}(new Guid(b));");
        }
        else if (backingWithoutGlobal == "string")
        {
            sb.AppendLine("        if (value is string s) return " + primitive.TypeName + $".{factoryMethod}(s);");
            sb.AppendLine("        return " + primitive.TypeName + $".{factoryMethod}(value.ToString() ?? string.Empty);");
        }
        else if (backingWithoutGlobal is "System.DateOnly" or "DateOnly")
        {
            sb.AppendLine("        if (value is DateTime dt) return " + primitive.TypeName + $".{factoryMethod}(DateOnly.FromDateTime(dt));");
            sb.AppendLine("        if (value is string s && DateOnly.TryParse(s, out var parsed)) return " + primitive.TypeName + $".{factoryMethod}(parsed);");
        }
        else
        {
            // Numerics and others - try to change type
            sb.AppendLine($"        try");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            var converted = ({primitive.BackingType})Convert.ChangeType(value, typeof({primitive.BackingType}));");
            sb.AppendLine($"            return {primitive.TypeName}.{factoryMethod}(converted);");
            sb.AppendLine($"        }}");
            sb.AppendLine($"        catch (InvalidCastException)");
            sb.AppendLine($"        {{");
            sb.AppendLine($"        }}");
        }

        sb.AppendLine();
        sb.AppendLine($"        throw new DataException($\"Cannot parse {{value.GetType()}} as {primitive.TypeName}.\");");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
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
        sb.AppendLine("    /// instead of falling back to <c>default(T)</c>, unless the target property is nullable.");
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


