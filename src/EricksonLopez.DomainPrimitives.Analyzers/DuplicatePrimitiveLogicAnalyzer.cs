using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace EricksonLopez.DomainPrimitives.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DuplicatePrimitiveLogicAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => 
        ImmutableArray.Create(DiagnosticDescriptors.DP0013_PossibleDuplicatePrimitiveLogic);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        
        context.RegisterCompilationStartAction(compilationContext =>
        {
            var symbols = new ConcurrentBag<INamedTypeSymbol>();
            
            compilationContext.RegisterSymbolAction(symbolContext =>
            {
                var typeSymbol = (INamedTypeSymbol)symbolContext.Symbol;
                
                // Only consider record structs with domain primitive attributes
                if (typeSymbol.TypeKind == TypeKind.Struct && typeSymbol.IsRecord)
                {
                    bool isDomainPrimitive = typeSymbol.GetAttributes().Any(a => 
                        a.AttributeClass?.Name.Contains("Primitive") == true ||
                        a.AttributeClass?.Name == "StrongIdAttribute" ||
                        a.AttributeClass?.Name == "StrongId" ||
                        a.AttributeClass?.Name == "IdAttribute" ||
                        a.AttributeClass?.Name == "Id" ||
                        a.AttributeClass?.Name.Contains("ValueObject") == true ||
                        a.AttributeClass?.Name.EndsWith("CodeAttribute", System.StringComparison.Ordinal) == true ||
                        IsShortcutAttribute(a.AttributeClass?.Name));

                    if (isDomainPrimitive)
                    {
                        symbols.Add(typeSymbol);
                    }
                }
            }, SymbolKind.NamedType);
            
            compilationContext.RegisterCompilationEndAction(endContext =>
            {
                // Group by identical attribute configurations
                var grouped = symbols
                    .OrderBy(s => s.Name)
                    .GroupBy(s => GetAttributeSignature(s))
                    .Where(g => g.Count() > 1 && !string.IsNullOrEmpty(g.Key));

                foreach (var group in grouped)
                {
                    var symbolList = group.ToList();
                    for (int i = 0; i < symbolList.Count; i++)
                    {
                        var primary = symbolList[i];
                        for (int j = i + 1; j < symbolList.Count; j++)
                        {
                            var duplicate = symbolList[j];
                            // Report on the duplicate (or both)
                            foreach (var location in duplicate.Locations)
                            {
                                endContext.ReportDiagnostic(Diagnostic.Create(
                                    DiagnosticDescriptors.DP0013_PossibleDuplicatePrimitiveLogic,
                                    location,
                                    duplicate.Name,
                                    primary.Name));
                            }
                        }
                    }
                }
            });
        });
    }

    private static bool IsShortcutAttribute(string? name)
    {
        if (name == null) return false;
        
        string[] shortcuts = {
            "EmailAttribute", "PhoneAttribute", "UrlAttribute", "SlugAttribute", 
            "UsernameAttribute", "PasswordHashAttribute", "HexColorAttribute", 
            "IPAddressAttribute", "MacAddressAttribute", "IBANAttribute", 
            "ISBNAttribute", "VINAttribute", "LatitudeAttribute", "LongitudeAttribute", 
            "AgeAttribute", "WeightAttribute", "HeightAttribute", "DistanceAttribute", 
            "TemperatureAttribute", "ScoreAttribute", "QuantityAttribute", 
            "PriceAttribute", "TaxRateAttribute", "DiscountAttribute", 
            "RatingAttribute", "PercentageAttribute", "MoneyAttribute", 
            "BirthDateAttribute", "ExpirationDateAttribute", "BusinessDateAttribute", 
            "FiscalYearAttribute", "MonthAttribute", "QuarterAttribute", 
            "WeekAttribute", "DateRangeAttribute", "TimeRangeAttribute"
        };
        return shortcuts.Contains(name);
    }

    private static string GetAttributeSignature(INamedTypeSymbol symbol)
    {
        var attributes = symbol.GetAttributes()
            .Where(a => a.AttributeClass?.ContainingNamespace?.ToDisplayString().StartsWith("EricksonLopez.DomainPrimitives", System.StringComparison.Ordinal) == true)
            .OrderBy(a => a.AttributeClass?.Name)
            .Select(a =>
            {
                var args = string.Join(",", a.ConstructorArguments.Select(arg => arg.Value?.ToString() ?? "null"));
                var namedArgs = string.Join(",", a.NamedArguments.OrderBy(n => n.Key).Select(n => $"{n.Key}={n.Value.Value?.ToString() ?? "null"}"));
                return $"{a.AttributeClass?.Name}({args}){{{namedArgs}}}";
            });

        return string.Join("|", attributes);
    }
}
