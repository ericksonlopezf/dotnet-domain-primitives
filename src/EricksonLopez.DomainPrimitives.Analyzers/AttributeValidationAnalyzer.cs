using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace EricksonLopez.DomainPrimitives.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AttributeValidationAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(
            DiagnosticDescriptors.DP0004_InvalidRegex,
            DiagnosticDescriptors.DP0005_ConflictingNormalization,
            DiagnosticDescriptors.DP0006_InvalidConstraintBounds);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        
        context.RegisterSyntaxNodeAction(AnalyzeAttributes, SyntaxKind.StructDeclaration, SyntaxKind.RecordStructDeclaration);
    }

    private void AnalyzeAttributes(SyntaxNodeAnalysisContext context)
    {
        var typeDecl = (TypeDeclarationSyntax)context.Node;
        var symbol = context.SemanticModel.GetDeclaredSymbol(typeDecl, context.CancellationToken) as INamedTypeSymbol;
        if (symbol is null) return;

        var attributes = symbol.GetAttributes();
        bool hasLowerCase = false;
        bool hasUpperCase = false;
        
        int? minLength = null;
        int? maxLength = null;
        AttributeData? lengthAttrToReport = null;

        foreach (var attr in attributes)
        {
            var name = attr.AttributeClass?.Name;
            if (name == null) continue;

            // DP0005: Conflicting Normalizations
            if (name == "LowerCaseAttribute") hasLowerCase = true;
            if (name == "UpperCaseAttribute") hasUpperCase = true;

            // DP0004: Invalid Regex
            if (name == "RegexAttribute")
            {
                if (attr.ConstructorArguments.Length > 0 && attr.ConstructorArguments[0].Value is string pattern)
                {
                    try
                    {
                        _ = new Regex(pattern);
                    }
                    catch (System.ArgumentException ex)
                    {
                        var syntax = attr.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken);
                        if (syntax != null)
                        {
                            context.ReportDiagnostic(Diagnostic.Create(
                                DiagnosticDescriptors.DP0004_InvalidRegex,
                                syntax.GetLocation(),
                                pattern,
                                ex.Message));
                        }
                    }
                }
            }

            // DP0006: Invalid Constraint Bounds
            if (name == "MinLengthAttribute" && attr.ConstructorArguments.Length > 0 && attr.ConstructorArguments[0].Value is int minVal)
            {
                minLength = minVal;
                lengthAttrToReport ??= attr;
            }
            else if (name == "MaxLengthAttribute" && attr.ConstructorArguments.Length > 0 && attr.ConstructorArguments[0].Value is int maxVal)
            {
                maxLength = maxVal;
                lengthAttrToReport ??= attr;
            }
            else if (name == "LengthAttribute" && attr.ConstructorArguments.Length >= 2 && 
                     attr.ConstructorArguments[0].Value is int lenMin && attr.ConstructorArguments[1].Value is int lenMax)
            {
                if (lenMin > lenMax)
                {
                    ReportBoundsError(context, attr, lenMin.ToString(System.Globalization.CultureInfo.InvariantCulture), lenMax.ToString(System.Globalization.CultureInfo.InvariantCulture));
                }
            }
            else if (name == "RangeAttribute")
            {
                // Assuming constructor args are Min and Max
                if (attr.ConstructorArguments.Length >= 2)
                {
                    var minArg = attr.ConstructorArguments[0].Value;
                    var maxArg = attr.ConstructorArguments[1].Value;

                    if (minArg is double minD && maxArg is double maxD && minD > maxD)
                    {
                        ReportBoundsError(context, attr, minD.ToString(System.Globalization.CultureInfo.InvariantCulture), maxD.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    }
                    else if (minArg is int minI && maxArg is int maxI && minI > maxI)
                    {
                        ReportBoundsError(context, attr, minI.ToString(System.Globalization.CultureInfo.InvariantCulture), maxI.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    }
                    else if (minArg is string minS && maxArg is string maxS)
                    {
                        if (decimal.TryParse(minS, out var minDec) && decimal.TryParse(maxS, out var maxDec) && minDec > maxDec)
                        {
                            ReportBoundsError(context, attr, minS, maxS);
                        }
                    }
                }
            }
        }

        if (minLength.HasValue && maxLength.HasValue && minLength.Value > maxLength.Value && lengthAttrToReport != null)
        {
            ReportBoundsError(context, lengthAttrToReport, minLength.Value.ToString(System.Globalization.CultureInfo.InvariantCulture), maxLength.Value.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        if (hasLowerCase && hasUpperCase)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.DP0005_ConflictingNormalization,
                typeDecl.Identifier.GetLocation(),
                symbol.Name));
        }
    }

    private static void ReportBoundsError(SyntaxNodeAnalysisContext context, AttributeData attr, string min, string max)
    {
        var syntax = attr.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken);
        if (syntax != null)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.DP0006_InvalidConstraintBounds,
                syntax.GetLocation(),
                min, max));
        }
    }
}
