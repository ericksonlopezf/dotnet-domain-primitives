using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
#pragma warning disable CS1591
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace EricksonLopez.DomainPrimitives.OpenApi.SourceGenerators;

[Generator(LanguageNames.CSharp)]
internal sealed class OpenApiSchemaFilterGenerator : IIncrementalGenerator
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
        bool isSmartEnum = false;
        string openApiType = "string";
        string openApiFormat = "";

        foreach (var attr in attributes)
        {
            var attrClass = attr.AttributeClass;
            if (attrClass == null) continue;

            if (attrClass.ContainingNamespace.ToString() == "EricksonLopez.DomainPrimitives")
            {
                var name = attrClass.Name;
                if (name == "StrongIdAttribute" || 
                    name == "StringPrimitiveAttribute" ||
                    name == "NumericPrimitiveAttribute" ||
                    name == "DatePrimitiveAttribute" ||
                    name == "EmailAttribute" || name == "PhoneAttribute" ||
                    name == "UrlAttribute" || name == "SlugAttribute" ||
                    name == "CountryCodeAttribute" || name == "LanguageCodeAttribute" ||
                    name == "CurrencyCodeAttribute" || name == "UsernameAttribute" ||
                    name == "PasswordHashAttribute" || name == "HexColorAttribute" ||
                    name == "MoneyAttribute" || name == "PercentageAttribute" ||
                    name == "BirthDateAttribute" || name == "ExpirationDateAttribute" ||
                    name == "SmartEnumAttribute")
                {
                    isDomainPrimitive = true;
                    
                    if (name == "NumericPrimitiveAttribute" || name == "MoneyAttribute" || name == "PercentageAttribute")
                    {
                        openApiType = "number";
                        if (name == "MoneyAttribute") openApiFormat = "double";
                    }
                    else if (name == "DatePrimitiveAttribute" || name == "BirthDateAttribute" || name == "ExpirationDateAttribute")
                    {
                        openApiType = "string";
                        openApiFormat = "date";
                    }
                    else if (name == "StrongIdAttribute")
                    {
                        if (attrClass.IsGenericType && attrClass.TypeArguments[0].Name == "Guid")
                        {
                            openApiType = "string";
                            openApiFormat = "uuid";
                        }
                        else if (attrClass.IsGenericType && (attrClass.TypeArguments[0].Name == "Int32" || attrClass.TypeArguments[0].Name == "Int64"))
                        {
                            openApiType = "integer";
                        }
                    }
                    else if (name == "EmailAttribute")
                    {
                        openApiFormat = "email";
                    }
                    else if (name == "UrlAttribute")
                    {
                        openApiFormat = "uri";
                    }
                    else if (name == "SmartEnumAttribute")
                    {
                        isSmartEnum = true;
                        if (attrClass.IsGenericType)
                        {
                            var typeArg = attrClass.TypeArguments[0];
                            if (typeArg.Name == "Int32" || typeArg.Name == "Int64")
                                openApiType = "integer";
                            else
                                openApiType = "string";
                        }
                    }
                    
                    break;
                }
            }
        }

        if (!isDomainPrimitive) return null;

        return new PrimitiveInfo(symbol.ContainingNamespace.ToDisplayString(), symbol.Name, openApiType, openApiFormat, isSmartEnum);
    }

    private static void Execute(Compilation compilation, ImmutableArray<PrimitiveInfo> primitives, SourceProductionContext context)
    {
        if (primitives.IsDefaultOrEmpty) return;

        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("using System;");
        sb.AppendLine("using Microsoft.OpenApi.Models;");
        sb.AppendLine("using Microsoft.OpenApi.Any;");
        sb.AppendLine("using Swashbuckle.AspNetCore.SwaggerGen;");
        sb.AppendLine();
        sb.AppendLine("namespace EricksonLopez.DomainPrimitives.OpenApi.Generated;");
        sb.AppendLine();
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// Automatically maps Domain Primitives to their correct OpenAPI schema types without reflection.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine("public class DomainPrimitivesSchemaFilter : ISchemaFilter");
        sb.AppendLine("{");
        sb.AppendLine("    private readonly System.Collections.Generic.Dictionary<Type, Action<OpenApiSchema>> _schemaConfigs = new()");
        sb.AppendLine("    {");
        
        foreach (var primitive in primitives.Distinct())
        {
            string fqdn = primitive.Namespace == "<global namespace>" ? primitive.TypeName : $"{primitive.Namespace}.{primitive.TypeName}";
            
            sb.AppendLine($"        {{ typeof({fqdn}), schema => ");
            sb.AppendLine("            {");
            sb.AppendLine($"                schema.Type = \"{primitive.OpenApiType}\";");
            if (!string.IsNullOrEmpty(primitive.OpenApiFormat))
            {
                sb.AppendLine($"                schema.Format = \"{primitive.OpenApiFormat}\";");
            }
            if (primitive.IsSmartEnum)
            {
                sb.AppendLine($"                schema.Enum = new System.Collections.Generic.List<IOpenApiAny>();");
                sb.AppendLine($"                foreach (var item in {fqdn}.All)");
                sb.AppendLine($"                {{");
                if (primitive.OpenApiType == "integer")
                {
                    sb.AppendLine($"                    schema.Enum.Add(new OpenApiInteger((int)Convert.ChangeType(item.Value, typeof(int))));");
                }
                else
                {
                    sb.AppendLine($"                    schema.Enum.Add(new OpenApiString(item.Name));");
                }
                sb.AppendLine($"                }}");
            }
            sb.AppendLine("            }");
            sb.AppendLine("        },");
        }
        
        sb.AppendLine("    };");
        sb.AppendLine();
        sb.AppendLine("    public void Apply(OpenApiSchema schema, SchemaFilterContext context)");
        sb.AppendLine("    {");
        sb.AppendLine("        if (_schemaConfigs.TryGetValue(context.Type, out var configAction))");
        sb.AppendLine("        {");
        sb.AppendLine("            configAction(schema);");
        sb.AppendLine("        }");
        
        sb.AppendLine("    }");
        sb.AppendLine("}");

        context.AddSource("DomainPrimitivesSchemaFilter.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
    }
}

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal readonly struct PrimitiveInfo
{
    public string Namespace { get; }
    public string TypeName { get; }
    public string OpenApiType { get; }
    public string OpenApiFormat { get; }
    public bool IsSmartEnum { get; }

    public PrimitiveInfo(string @namespace, string typeName, string openApiType, string openApiFormat, bool isSmartEnum = false)
    {
        Namespace = @namespace;
        TypeName = typeName;
        OpenApiType = openApiType;
        OpenApiFormat = openApiFormat;
        IsSmartEnum = isSmartEnum;
    }
}
