using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace EricksonLopez.DomainPrimitives.Mapster.SourceGenerators;

[Generator(LanguageNames.CSharp)]
internal sealed class MapsterRegisterGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var structDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => s is StructDeclarationSyntax or RecordDeclarationSyntax,
                transform: static (ctx, _) => GetDomainPrimitiveInfo(ctx))
            .Where(static m => m.HasValue)
            .Select(static (m, _) => m!.Value);

        var compilationAndStructs = context.CompilationProvider.Combine(structDeclarations.Collect());

        context.RegisterSourceOutput(compilationAndStructs,
            static (spc, source) => Execute(source.Left, source.Right, spc));
    }

    private static PrimitiveInfo? GetDomainPrimitiveInfo(GeneratorSyntaxContext context)
    {
        var typeDecl = (TypeDeclarationSyntax)context.Node;
        var symbol = context.SemanticModel.GetDeclaredSymbol(typeDecl) as INamedTypeSymbol;
        if (symbol == null || !symbol.IsValueType) return null;

        var attributes = symbol.GetAttributes();


        bool isDomainPrimitive = false;

        foreach (var attr in attributes)
        {
            var attrClass = attr.AttributeClass;
            if (attrClass == null) continue;

            if (attrClass.ContainingNamespace.ToString() == "EricksonLopez.DomainPrimitives")
            {
                if (attrClass.Name == "StrongIdAttribute" || 
                    attrClass.Name == "StringPrimitiveAttribute" ||
                    attrClass.Name == "NumericPrimitiveAttribute" ||
                    attrClass.Name == "DatePrimitiveAttribute" ||
                    attrClass.Name == "EmailAttribute" || attrClass.Name == "PhoneAttribute" ||
                    attrClass.Name == "UrlAttribute" || attrClass.Name == "SlugAttribute" ||
                    attrClass.Name == "CountryCodeAttribute" || attrClass.Name == "LanguageCodeAttribute" ||
                    attrClass.Name == "CurrencyCodeAttribute" || attrClass.Name == "UsernameAttribute" ||
                    attrClass.Name == "PasswordHashAttribute" || attrClass.Name == "HexColorAttribute" ||
                    attrClass.Name == "MoneyAttribute" || attrClass.Name == "PercentageAttribute" ||
                    attrClass.Name == "BirthDateAttribute" || attrClass.Name == "ExpirationDateAttribute" ||
                    attrClass.Name == "SmartEnumAttribute")
                {
                    isDomainPrimitive = true;
                    break;
                }
            }
        }

        if (!isDomainPrimitive) return null;
        
        string backingType = "string";
        var primitiveAttr = attributes.First(a => a.AttributeClass!.ContainingNamespace.ToString() == "EricksonLopez.DomainPrimitives");
        var attrName = primitiveAttr.AttributeClass!.Name;

        if (attrName == "StrongIdAttribute" || attrName == "NumericPrimitiveAttribute")
        {
            if (primitiveAttr.AttributeClass.IsGenericType)
            {
                backingType = primitiveAttr.AttributeClass.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            }
        }
        else if (attrName == "DatePrimitiveAttribute")
        {
            backingType = "global::System.DateOnly";
            var kindArg = primitiveAttr.NamedArguments.FirstOrDefault(kvp => kvp.Key == "Kind").Value;
            if (!kindArg.IsNull && kindArg.Value is int kindInt)
            {
                if (kindInt == 1) backingType = "global::System.DateTime";
                else if (kindInt == 2) backingType = "global::System.TimeOnly";
                else if (kindInt == 3) backingType = "global::System.DateTimeOffset";
            }
        }
        else if (attrName == "MoneyAttribute" || attrName == "PercentageAttribute")
        {
            backingType = "decimal";
        }
        else if (attrName == "SmartEnumAttribute")
        {
            if (primitiveAttr.AttributeClass.IsGenericType)
            {
                backingType = primitiveAttr.AttributeClass.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            }
            else
            {
                backingType = "int";
            }
        }

        return new PrimitiveInfo(symbol.ContainingNamespace.ToDisplayString(), symbol.Name, backingType, attrName == "SmartEnumAttribute");
    }

    private static void Execute(Compilation compilation, ImmutableArray<PrimitiveInfo> primitives, SourceProductionContext context)
    {
        if (primitives.IsDefaultOrEmpty) return;

        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("using Mapster;");
        sb.AppendLine();
        sb.AppendLine("namespace EricksonLopez.DomainPrimitives.Mapster.Generated;");
        sb.AppendLine();
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// Extension method to register Mapster configs for all Domain Primitives statically.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine("public static class DomainPrimitivesMapsterExtensions");
        sb.AppendLine("{");
        sb.AppendLine("    public static void AddDomainPrimitives(this TypeAdapterConfig config)");
        sb.AppendLine("    {");
        
        foreach (var primitive in primitives.Distinct())
        {
            string fqdn = primitive.Namespace == "<global namespace>" ? primitive.TypeName : $"{primitive.Namespace}.{primitive.TypeName}";
            string factoryMethod = primitive.IsSmartEnum ? "FromValue" : "Create";
            sb.AppendLine($"        config.NewConfig<{primitive.BackingType}, {fqdn}>().MapWith(src => {fqdn}.{factoryMethod}(src));");
            sb.AppendLine($"        config.NewConfig<{fqdn}, {primitive.BackingType}>().MapWith(src => src.Value);");
        }
        
        sb.AppendLine("    }");
        sb.AppendLine("}");

        context.AddSource("DomainPrimitivesMapsterRegister.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
    }
}

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal readonly struct PrimitiveInfo
{
    public string Namespace { get; }
    public string TypeName { get; }
    public string BackingType { get; }
    public bool IsSmartEnum { get; }

    public PrimitiveInfo(string @namespace, string typeName, string backingType, bool isSmartEnum = false)
    {
        Namespace = @namespace;
        TypeName = typeName;
        BackingType = backingType;
        IsSmartEnum = isSmartEnum;
    }
}
