// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace EricksonLopez.DomainPrimitives.EFCore.SourceGenerators;

[Generator(LanguageNames.CSharp)]
internal sealed class EFCoreValueConverterGenerator : IIncrementalGenerator
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

        // Generate the converters and the configuration extensions
        context.RegisterSourceOutput(compilationAndStructs,
            (spc, source) => Execute(source.Left, source.Right, spc));
    }

    internal static bool IsDomainPrimitiveAttribute(AttributeData a)
    {
        var ns = a.AttributeClass?.ContainingNamespace?.ToDisplayString();
        if (ns == null || !ns.StartsWith("EricksonLopez.DomainPrimitives", StringComparison.Ordinal))
            return false;

        var name = a.AttributeClass?.Name;
        return name is not "EFCoreAttribute" and not "DomainPrimitivesDefaultsAttribute" and not "AspNetCoreAttribute" and not "ValueObjectAttribute";
    }

    internal static PrimitiveInfo? GetDomainPrimitiveInfo(GeneratorSyntaxContext context)
    {
        var typeDecl = (TypeDeclarationSyntax)context.Node;
        if (context.SemanticModel.GetDeclaredSymbol(typeDecl) is not INamedTypeSymbol symbol || !symbol.IsValueType)
            return null;

        var attributes = symbol.GetAttributes();
        var primitiveAttr = attributes.FirstOrDefault(IsDomainPrimitiveAttribute);
        if (primitiveAttr == null) return null;

        string backingType = "string";
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
            backingType = "global::System.DateOnly";
            var kindArg = primitiveAttr.NamedArguments.FirstOrDefault(kvp => kvp.Key == "Kind").Value;
            if (!kindArg.IsNull && kindArg.Value is int kindInt)
            {
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
                backingType = "int";
            }
        }

        int? maxLength = null;
        int? precision = null;
        int? scale = null;

        foreach (var attr in attributes)
        {
            var name = attr.AttributeClass?.Name;
            if (name == "MaxLengthAttribute" && attr.ConstructorArguments.Length > 0 && attr.ConstructorArguments[0].Value is int max)
            {
                maxLength = max;
            }
            else if (name == "LengthAttribute" && attr.ConstructorArguments.Length >= 2 && attr.ConstructorArguments[1].Value is int lMax)
            {
                maxLength = lMax;
            }
        }

        if (attrName == "MoneyAttribute")
        {
            precision = 18;
            scale = 4;
        }
        else if (attrName == "PercentageAttribute")
        {
            precision = 5;
            scale = 2;
        }

        return new PrimitiveInfo(
            symbol.ContainingNamespace.ToDisplayString(),
            symbol.Name,
            backingType,
            attrName == "SmartEnumAttribute",
            maxLength,
            precision,
            scale
        );
    }

    internal static string GenerateConverterSource(PrimitiveInfo primitive)
    {
        var converterClassName = $"{primitive.TypeName}ValueConverter";
        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("#nullable enable");
        sb.AppendLine("using System;");
        sb.AppendLine("using Microsoft.EntityFrameworkCore.Storage.ValueConversion;");
        if (primitive.Namespace != "<global namespace>")
        {
            sb.AppendLine($"using {primitive.Namespace};");
        }
        sb.AppendLine();
        sb.AppendLine("namespace EricksonLopez.DomainPrimitives.EFCore.Generated;");
        sb.AppendLine();
        sb.AppendLine("/// <summary>");
        sb.AppendLine($"/// EF Core ValueConverter for the {primitive.TypeName} domain primitive.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine($"public sealed class {converterClassName} : ValueConverter<{primitive.TypeName}, {primitive.BackingType}>");
        sb.AppendLine("{");
        sb.AppendLine($"    public {converterClassName}() : this(null)");
        sb.AppendLine("    {");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine($"    public {converterClassName}(ConverterMappingHints? mappingHints)");
        sb.AppendLine("        : base(");
        sb.AppendLine("            model => model.Value,");
        string factoryMethod = primitive.IsSmartEnum ? "FromValue" : "Create";
        sb.AppendLine($"            provider => {primitive.TypeName}.{factoryMethod}(provider),");
        sb.AppendLine("            mappingHints)");
        sb.AppendLine("    {");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        return sb.ToString();
    }

    internal static string GenerateExtensionsSource(IEnumerable<PrimitiveInfo> primitives)
    {
        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("#nullable enable");
        sb.AppendLine("using Microsoft.EntityFrameworkCore;");
        sb.AppendLine();
        sb.AppendLine("namespace EricksonLopez.DomainPrimitives.EFCore.Generated;");
        sb.AppendLine();
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// Extension methods to register Domain Primitives ValueConverters in EF Core.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine("public static class DomainPrimitivesEFCoreExtensions");
        sb.AppendLine("{");
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Configures all generated ValueConverters in the provided ModelConfigurationBuilder.");
        sb.AppendLine("    /// Call this inside your DbContext.ConfigureConventions method.");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public static void ConfigureDomainPrimitives(this ModelConfigurationBuilder configurationBuilder)");
        sb.AppendLine("    {");
        foreach (var primitive in primitives.Distinct())
        {
            string nsPrefix = primitive.Namespace == "<global namespace>" ? "" : primitive.Namespace + ".";
            sb.AppendLine($"        configurationBuilder.Properties<{nsPrefix}{primitive.TypeName}>()");
            sb.AppendLine($"            .HaveConversion<{primitive.TypeName}ValueConverter>()");
            if (primitive.MaxLength.HasValue)
            {
                sb.AppendLine($"            .HaveMaxLength({primitive.MaxLength.Value})");
            }
            if (primitive.Precision.HasValue && primitive.Scale.HasValue)
            {
                sb.AppendLine($"            .HavePrecision({primitive.Precision.Value}, {primitive.Scale.Value})");
            }
            sb.AppendLine("            ;");
        }
        sb.AppendLine("    }");
        sb.AppendLine("}");
        return sb.ToString();
    }

    private static void Execute(Compilation compilation, ImmutableArray<PrimitiveInfo> primitives, SourceProductionContext context)
    {
        if (primitives.IsDefaultOrEmpty) return;

        foreach (var primitive in primitives.Distinct())
        {
            var source = GenerateConverterSource(primitive);
            context.AddSource($"{primitive.TypeName}ValueConverter.g.cs", SourceText.From(source, Encoding.UTF8));
        }

        var extSource = GenerateExtensionsSource(primitives);
        context.AddSource("DomainPrimitivesEFCoreExtensions.g.cs", SourceText.From(extSource, Encoding.UTF8));
    }
}
