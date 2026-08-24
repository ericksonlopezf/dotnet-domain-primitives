// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Threading.Tasks;

namespace EricksonLopez.DomainPrimitives.Analyzers;

/// <summary>
/// Validates attribute configurations on domain primitive types, detecting invalid regex
/// patterns, conflicting normalization attributes, and illegal constraint bounds.
/// </summary>
/// <remarks>
/// <para>Reports the following diagnostics:</para>
/// <list type="bullet">
///   <item><term>DP0004</term><description>Regex pattern is syntactically invalid.</description></item>
///   <item><term>DP0005</term><description><c>[LowerCase]</c> and <c>[UpperCase]</c> used simultaneously.</description></item>
///   <item><term>DP0006</term><description>Minimum constraint value exceeds maximum.</description></item>
///   <item><term>DP0017</term><description>Custom exception type specified in <c>[DomainPrimitivesDefaults]</c> is invalid.</description></item>
/// </list>
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AttributeValidationAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(
            DiagnosticDescriptors.DP0004_InvalidRegex,
            DiagnosticDescriptors.DP0005_ConflictingNormalization,
            DiagnosticDescriptors.DP0006_InvalidConstraintBounds,
            DiagnosticDescriptors.DP0017_InvalidExceptionType);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        
        context.RegisterSyntaxNodeAction(AnalyzeAttributes, SyntaxKind.StructDeclaration, SyntaxKind.RecordStructDeclaration);
        context.RegisterCompilationAction(AnalyzeCompilation);
    }

    private void AnalyzeAttributes(SyntaxNodeAnalysisContext context)
    {
        var typeDecl = (TypeDeclarationSyntax)context.Node;
        var symbol = (INamedTypeSymbol)context.SemanticModel.GetDeclaredSymbol(typeDecl, context.CancellationToken)!;

        var attributes = symbol.GetAttributes();
        bool hasLowerCase = false;
        bool hasUpperCase = false;
        
        int? minLength = null;
        int? maxLength = null;
        AttributeData? lengthAttrToReport = null;

        foreach (var attr in attributes)
        {
            var name = attr.AttributeClass?.Name;

            // DP0005: Conflicting Normalizations
            if (name == "LowerCaseAttribute") hasLowerCase = true;
            if (name == "UpperCaseAttribute") hasUpperCase = true;

            // DP0004: Invalid Regex
            if (name == "RegexAttribute" &&
                attr.ConstructorArguments.Length > 0 &&
                attr.ConstructorArguments[0].Value is string pattern)
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

            // DP0006: Invalid Constraint Bounds
            if (name == "MinLengthAttribute" &&
                attr.ConstructorArguments.Length > 0 &&
                attr.ConstructorArguments[0].Value is int minVal)
            {
                minLength = minVal;
                lengthAttrToReport ??= attr;
            }
            else if (name == "MaxLengthAttribute" &&
                     attr.ConstructorArguments.Length > 0 &&
                     attr.ConstructorArguments[0].Value is int maxVal)
            {
                maxLength = maxVal;
                lengthAttrToReport ??= attr;
            }
            else if (name == "LengthAttribute")
            {
                if (attr.ConstructorArguments.Length >= 2 && 
                    attr.ConstructorArguments[0].Value is int lenMin &&
                    attr.ConstructorArguments[1].Value is int lenMax &&
                    lenMin > lenMax)
                {
                    ReportBoundsError(context, attr, lenMin.ToString(System.Globalization.CultureInfo.InvariantCulture), lenMax.ToString(System.Globalization.CultureInfo.InvariantCulture));
                }
            }
            else if (name == "RangeAttribute")
            {
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
                    else if (minArg is string minS && maxArg is string maxS &&
                             decimal.TryParse(minS, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var minDec) &&
                             decimal.TryParse(maxS, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var maxDec) &&
                             minDec > maxDec)
                    {
                        ReportBoundsError(context, attr, minS, maxS);
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

    private static void AnalyzeCompilation(CompilationAnalysisContext context)
    {
        foreach (var attr in context.Compilation.Assembly.GetAttributes())
        {
            if (attr.AttributeClass is { Name: "DomainPrimitivesDefaultsAttribute" } attrClass &&
                attrClass.ContainingNamespace?.ToDisplayString() == "EricksonLopez.DomainPrimitives")
            {
                foreach (var named in attr.NamedArguments)
                {
                    if (named.Key == "ExceptionType" && named.Value.Value is INamedTypeSymbol excType)
                    {
                        var exceptionTypeSymbol = context.Compilation.GetTypeByMetadataName("System.Exception");
                        bool inheritsFromException = false;
                        for (var current = excType.BaseType; current is not null && !inheritsFromException; current = current.BaseType)
                        {
                            if (SymbolEqualityComparer.Default.Equals(current, exceptionTypeSymbol))
                            {
                                inheritsFromException = true;
                            }
                        }

                        bool hasStringConstructor = excType.Constructors.Any(ctor =>
                            ctor.DeclaredAccessibility == Accessibility.Public &&
                            ctor.Parameters.Length == 1 &&
                            ctor.Parameters[0].Type.SpecialType == SpecialType.System_String);

                        if (!inheritsFromException || !hasStringConstructor)
                        {
                            var syntax = attr.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken);
                            context.ReportDiagnostic(Diagnostic.Create(
                                DiagnosticDescriptors.DP0017_InvalidExceptionType,
                                syntax?.GetLocation() ?? Location.None,
                                excType.Name));
                        }
                    }
                }
            }
        }
    }
}


