// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Threading.Tasks;

namespace EricksonLopez.DomainPrimitives.Analyzers;

/// <summary>
/// Detects domain primitive types that share identical attribute configurations within the
/// same compilation, indicating potential copy-paste duplication of domain concepts.
/// </summary>
/// <remarks>
/// Reports <c>DP0013</c> at compile-end for each pair of <c>readonly partial record struct</c>
/// types whose attribute signatures are identical. The diagnostic is informational — the
/// types may intentionally share structure but represent distinct concepts, in which case
/// the warning should be suppressed with a justification comment.
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DuplicatePrimitiveLogicAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => 
        ImmutableArray.Create(DiagnosticDescriptors.DP0013_PossibleDuplicatePrimitiveLogic);

    /// <inheritdoc />
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
                    bool isDomainPrimitive = typeSymbol.GetAttributes().Any(IsDomainPrimitiveAttribute);

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
                    .GroupBy(GetAttributeSignature)
                    .Where(g => !string.IsNullOrEmpty(g.Key));

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

    /// <summary>
    /// Returns <see langword="true"/> when <paramref name="name"/> is the name of a
    /// domain shortcut attribute (e.g., <c>EmailAttribute</c>, <c>PhoneAttribute</c>).
    /// </summary>
    /// <param name="name">The attribute class name to test. May be <see langword="null"/>.</param>
    internal static bool IsShortcutAttribute(string? name) =>
        name != null && MissingValidationAnalyzer.DomainShortcutAttributeNames.Contains(name);

    /// <summary>
    /// Returns <see langword="true"/> when <paramref name="a"/> originates from the
    /// <c>EricksonLopez.DomainPrimitives</c> attribute namespace.
    /// </summary>
    /// <param name="a">The attribute data to inspect.</param>
    internal static bool IsDomainPrimitiveAttribute(AttributeData a) =>
        a.AttributeClass?.ContainingNamespace?.ToDisplayString().StartsWith("EricksonLopez.DomainPrimitives", StringComparison.Ordinal) == true;

    /// <summary>
    /// Computes a canonical string that uniquely identifies the combination of domain primitive
    /// attributes applied to <paramref name="symbol"/>, used to group types with identical configurations.
    /// </summary>
    /// <param name="symbol">The named type symbol to derive a signature from.</param>
    /// <returns>
    /// A pipe-delimited string encoding each attribute name, its constructor arguments, and its
    /// named arguments in sorted order. Returns an empty string when no domain primitive
    /// attributes are present.
    /// </returns>
    internal static string GetAttributeSignature(INamedTypeSymbol symbol)
    {
        var attributes = symbol.GetAttributes()
            .Where(IsDomainPrimitiveAttribute)
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


