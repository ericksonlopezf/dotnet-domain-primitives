// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Threading.Tasks;

namespace EricksonLopez.DomainPrimitives.Analyzers;

/// <summary>
/// Detects usage of the default constructor or <c>default</c> literal on domain primitive types,
/// where the standard <c>Create()</c> factory should be used instead.
/// </summary>
/// <remarks>
/// Reports <c>DP0007</c> for any <c>new PrimitiveType()</c>, implicit <c>new()</c>,
/// <c>default(PrimitiveType)</c>, or <c>default</c> literal expression that resolves to
/// a type implementing <c>IDomainPrimitive</c> or <c>IStrongId</c>.
/// Such expressions bypass validation and produce an uninitialized primitive instance.
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PrimitiveUsageAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(DiagnosticDescriptors.DP0007_AvoidDefaultConstructor);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        
        context.RegisterSyntaxNodeAction(AnalyzeObjectCreation, SyntaxKind.ObjectCreationExpression);
        context.RegisterSyntaxNodeAction(AnalyzeImplicitObjectCreation, SyntaxKind.ImplicitObjectCreationExpression);
        context.RegisterSyntaxNodeAction(AnalyzeDefaultExpression, SyntaxKind.DefaultExpression);
        context.RegisterSyntaxNodeAction(AnalyzeDefaultLiteralExpression, SyntaxKind.DefaultLiteralExpression);
    }

    private void AnalyzeObjectCreation(SyntaxNodeAnalysisContext context)
    {
        var objectCreation = (ObjectCreationExpressionSyntax)context.Node;
        
        // We only care about parameterless constructors
        if (objectCreation.ArgumentList != null && objectCreation.ArgumentList.Arguments.Count > 0)
            return;

        var symbolInfo = context.SemanticModel.GetSymbolInfo(objectCreation.Type, context.CancellationToken);
        if (symbolInfo.Symbol is not INamedTypeSymbol typeSymbol) return;

        if (IsDomainPrimitive(typeSymbol))
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.DP0007_AvoidDefaultConstructor,
                objectCreation.GetLocation(),
                typeSymbol.Name));
        }
    }

    private void AnalyzeImplicitObjectCreation(SyntaxNodeAnalysisContext context)
    {
        var objectCreation = (ImplicitObjectCreationExpressionSyntax)context.Node;
        
        // We only care about parameterless constructors
        if (objectCreation.ArgumentList != null && objectCreation.ArgumentList.Arguments.Count > 0)
            return;

        var symbolInfo = context.SemanticModel.GetSymbolInfo(objectCreation, context.CancellationToken);
        if (symbolInfo.Symbol is IMethodSymbol methodSymbol && methodSymbol.ContainingType is INamedTypeSymbol typeSymbol)
        {
            if (IsDomainPrimitive(typeSymbol))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.DP0007_AvoidDefaultConstructor,
                    objectCreation.GetLocation(),
                    typeSymbol.Name));
            }
        }
    }

    private void AnalyzeDefaultExpression(SyntaxNodeAnalysisContext context)
    {
        var defaultExpr = (DefaultExpressionSyntax)context.Node;
        
        var symbolInfo = context.SemanticModel.GetSymbolInfo(defaultExpr.Type, context.CancellationToken);
        if (symbolInfo.Symbol is not INamedTypeSymbol typeSymbol) return;

        if (IsDomainPrimitive(typeSymbol))
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.DP0007_AvoidDefaultConstructor,
                defaultExpr.GetLocation(),
                typeSymbol.Name));
        }
    }

    private void AnalyzeDefaultLiteralExpression(SyntaxNodeAnalysisContext context)
    {
        var defaultLiteralExpr = (LiteralExpressionSyntax)context.Node;
        
        var typeInfo = context.SemanticModel.GetTypeInfo(defaultLiteralExpr, context.CancellationToken);
        if (typeInfo.Type is not INamedTypeSymbol typeSymbol) return;

        if (IsDomainPrimitive(typeSymbol))
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.DP0007_AvoidDefaultConstructor,
                defaultLiteralExpr.GetLocation(),
                typeSymbol.Name));
        }
    }

    private static bool IsDomainPrimitive(INamedTypeSymbol typeSymbol)
    {
        // Simple check: does it have any attributes that contain "Primitive", "Id", "ValueObject" etc.?
        // A robust check would look for IDomainPrimitive<,> in AllInterfaces
        return typeSymbol.AllInterfaces.Any(i => 
            i.OriginalDefinition.Name == "IDomainPrimitive" || 
            i.OriginalDefinition.Name == "IStrongId");
    }
}


