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

    private static PrimitiveInfo? GetDomainPrimitiveInfo(GeneratorSyntaxContext context)
    {
        var typeDecl = (TypeDeclarationSyntax)context.Node;
        
        var symbol = context.SemanticModel.GetDeclaredSymbol(typeDecl) as INamedTypeSymbol;
        if (symbol == null) return null;

        // Ensure it's a value type
        if (!symbol.IsValueType) return null;

        // Look for attributes from our abstractions
        var attributes = symbol.GetAttributes();
        

        AttributeData? primitiveAttr = null;
        
        foreach (var attr in attributes)
        {
            var attrClass = attr.AttributeClass;
            if (attrClass == null) continue;
            
            if (attrClass.ContainingNamespace.ToString() == "EricksonLopez.DomainPrimitives")
            {
                // We only care about single-value primitives for ValueConverters. ValueObjects are handled separately if needed.
                if (attrClass.Name == "ValueObjectAttribute") return null;
                
                // If it's one of our primitive attributes, capture it
                if (attrClass.Name == "StrongIdAttribute" || 
                    attrClass.Name == "StringPrimitiveAttribute" ||
                    attrClass.Name == "NumericPrimitiveAttribute" ||
                    attrClass.Name == "DatePrimitiveAttribute" ||
                    // Shortcuts
                    attrClass.Name == "EmailAttribute" || attrClass.Name == "PhoneAttribute" ||
                    attrClass.Name == "UrlAttribute" || attrClass.Name == "SlugAttribute" ||
                    attrClass.Name == "CountryCodeAttribute" || attrClass.Name == "LanguageCodeAttribute" ||
                    attrClass.Name == "CurrencyCodeAttribute" || attrClass.Name == "UsernameAttribute" ||
                    attrClass.Name == "PasswordHashAttribute" || attrClass.Name == "HexColorAttribute" ||
                    attrClass.Name == "IPAddressAttribute" || attrClass.Name == "MacAddressAttribute" ||
                    attrClass.Name == "IBANAttribute" || attrClass.Name == "ISBNAttribute" ||
                    attrClass.Name == "VINAttribute" ||
                    attrClass.Name == "MoneyAttribute" || attrClass.Name == "PercentageAttribute" ||
                    attrClass.Name == "LatitudeAttribute" || attrClass.Name == "LongitudeAttribute" ||
                    attrClass.Name == "AgeAttribute" || attrClass.Name == "WeightAttribute" ||
                    attrClass.Name == "HeightAttribute" || attrClass.Name == "DistanceAttribute" ||
                    attrClass.Name == "TemperatureAttribute" || attrClass.Name == "ScoreAttribute" ||
                    attrClass.Name == "QuantityAttribute" || attrClass.Name == "PriceAttribute" ||
                    attrClass.Name == "TaxRateAttribute" || attrClass.Name == "DiscountAttribute" ||
                    attrClass.Name == "RatingAttribute" ||
                    attrClass.Name == "BirthDateAttribute" || attrClass.Name == "ExpirationDateAttribute" ||
                    attrClass.Name == "BusinessDateAttribute" || attrClass.Name == "FiscalYearAttribute" ||
                    attrClass.Name == "MonthAttribute" || attrClass.Name == "QuarterAttribute" ||
                    attrClass.Name == "WeekAttribute" || attrClass.Name == "DateRangeAttribute" ||
                    attrClass.Name == "TimeRangeAttribute" ||
                    attrClass.Name == "SmartEnumAttribute")
                {
                    primitiveAttr = attr;
                    break;
                }
            }
        }

        if (primitiveAttr == null) return null;

        // Determine backing type
        string backingType = "string"; // Default for StringPrimitive and most shortcuts
        bool isGuidBacked = false;

        var attrName = primitiveAttr.AttributeClass!.Name;
        if (attrName == "StrongIdAttribute" || attrName == "NumericPrimitiveAttribute")
        {
            if (primitiveAttr.AttributeClass.IsGenericType)
            {
                var typeArg = primitiveAttr.AttributeClass.TypeArguments[0];
                backingType = typeArg.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                isGuidBacked = typeArg.Name == "Guid";
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
                var typeArg = primitiveAttr.AttributeClass.TypeArguments[0];
                backingType = typeArg.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            }
            else
            {
                backingType = "int"; // Default if not generic (though it should be)
            }
        }

        int? maxLength = null;
        int? precision = null;
        int? scale = null;

        // Parse validation attributes for EF constraints
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
            // Explicit decimal precision/scale could be added via custom attributes if present.
        }

        if (attrName == "MoneyAttribute")
        {
            precision = 18;
            scale = 4;
        }
        else if (attrName == "PercentageAttribute")
        {
            precision = 5;
            scale = 2; // e.g. 100.00
        }

        return new PrimitiveInfo(
            symbol.ContainingNamespace.ToDisplayString(),
            symbol.Name,
            backingType,
            isGuidBacked,
            attrName == "SmartEnumAttribute",
            maxLength,
            precision,
            scale
        );
    }

    private static void Execute(Compilation compilation, ImmutableArray<PrimitiveInfo> primitives, SourceProductionContext context)
    {
        if (primitives.IsDefaultOrEmpty) return;

        var sb = new StringBuilder();
        var converterClasses = new List<string>();

        foreach (var primitive in primitives.Distinct())
        {
            var converterClassName = $"{primitive.TypeName}ValueConverter";
            converterClasses.Add(converterClassName);

            sb.Clear();
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
            
            sb.AppendLine($"/// <summary>");
            sb.AppendLine($"/// EF Core ValueConverter for the {primitive.TypeName} domain primitive.");
            sb.AppendLine($"/// </summary>");
            sb.AppendLine($"public sealed class {converterClassName} : ValueConverter<{primitive.TypeName}, {primitive.BackingType}>");
            sb.AppendLine("{");
            
            sb.AppendLine($"    public {converterClassName}(ConverterMappingHints? mappingHints = null)");
            sb.AppendLine($"        : base(");
            sb.AppendLine($"            model => model.Value,");
            string factoryMethod = primitive.IsSmartEnum ? "FromValue" : "Create";
            if (primitive.BackingType == "string" || primitive.BackingType == "global::System.String")
            {
                sb.AppendLine($"            provider => provider == null ? throw new InvalidOperationException(\"Cannot convert null from database to non-nullable {primitive.TypeName}.\") : {primitive.TypeName}.{factoryMethod}(provider),");
            }
            else
            {
                sb.AppendLine($"            provider => {primitive.TypeName}.{factoryMethod}(provider),");
            }
            sb.AppendLine($"            mappingHints)");
            sb.AppendLine("    {");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            context.AddSource($"{converterClassName}.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
        }

        // Generate Extension Class for easy registration in DbContext
        sb.Clear();
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
            else if (primitive.Precision.HasValue)
            {
                sb.AppendLine($"            .HavePrecision({primitive.Precision.Value})");
            }
            sb.AppendLine($"            ;");
        }
        sb.AppendLine("    }");
        sb.AppendLine("}");

        context.AddSource("DomainPrimitivesEFCoreExtensions.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
    }
}

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal readonly struct PrimitiveInfo : IEquatable<PrimitiveInfo>
{
    public string Namespace { get; }
    public string TypeName { get; }
    public string BackingType { get; }
    public bool IsGuidBacked { get; }
    public bool IsSmartEnum { get; }
    public int? MaxLength { get; }
    public int? Precision { get; }
    public int? Scale { get; }

    public PrimitiveInfo(string @namespace, string typeName, string backingType, bool isGuidBacked, bool isSmartEnum = false, int? maxLength = null, int? precision = null, int? scale = null)
    {
        Namespace = @namespace;
        TypeName = typeName;
        BackingType = backingType;
        IsGuidBacked = isGuidBacked;
        IsSmartEnum = isSmartEnum;
        MaxLength = maxLength;
        Precision = precision;
        Scale = scale;
    }
    
    public bool Equals(PrimitiveInfo other)
    {
        return Namespace == other.Namespace &&
               TypeName == other.TypeName &&
               BackingType == other.BackingType &&
               IsGuidBacked == other.IsGuidBacked &&
               IsSmartEnum == other.IsSmartEnum &&
               MaxLength == other.MaxLength &&
               Precision == other.Precision &&
               Scale == other.Scale;
    }

    public override bool Equals(object? obj) => obj is PrimitiveInfo other && Equals(other);

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Namespace.GetHashCode();
            hashCode = (hashCode * 397) ^ TypeName.GetHashCode();
            hashCode = (hashCode * 397) ^ BackingType.GetHashCode();
            hashCode = (hashCode * 397) ^ IsGuidBacked.GetHashCode();
            hashCode = (hashCode * 397) ^ IsSmartEnum.GetHashCode();
            hashCode = (hashCode * 397) ^ (MaxLength ?? 0);
            hashCode = (hashCode * 397) ^ (Precision ?? 0);
            hashCode = (hashCode * 397) ^ (Scale ?? 0);
            return hashCode;
        }
    }
}
