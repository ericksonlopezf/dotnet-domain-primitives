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
/// Detects incorrect usage of domain primitives in string comparisons and assignments.
/// </summary>
/// <remarks>
/// <para>
/// Reports <c>DP0010</c> when a raw <c>string</c> is compared to a domain primitive via <c>==</c>
/// or <c>!=</c>, which bypasses the type system and can produce unexpected results.
/// </para>
/// <para>
/// Reports <c>DP0011</c> when a domain primitive is implicitly assigned to a <c>string</c>
/// variable, discarding the strong typing without an explicit <c>.Value</c> access.
/// </para>
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class StringComparisonAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(
            DiagnosticDescriptors.DP0010_StringComparedWithPrimitive,
            DiagnosticDescriptors.DP0011_StringAssignedFromPrimitive);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeBinaryExpression, SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression);
        context.RegisterSyntaxNodeAction(AnalyzeAssignment, SyntaxKind.SimpleAssignmentExpression);
        context.RegisterSyntaxNodeAction(AnalyzeLocalDeclaration, SyntaxKind.LocalDeclarationStatement);
        context.RegisterSyntaxNodeAction(AnalyzeFieldDeclaration, SyntaxKind.FieldDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeArgument, SyntaxKind.Argument);
    }

    // ─── DP0010: string == primitive or primitive == string ──────────────────

    private static void AnalyzeBinaryExpression(SyntaxNodeAnalysisContext context)
    {
        var binaryExpr = (BinaryExpressionSyntax)context.Node;
        var leftType = context.SemanticModel.GetTypeInfo(binaryExpr.Left, context.CancellationToken).Type;
        var rightType = context.SemanticModel.GetTypeInfo(binaryExpr.Right, context.CancellationToken).Type;

        // Case 1: string == primitive  or  string != primitive
        if (IsStringType(leftType) && IsDomainPrimitive(rightType))
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.DP0010_StringComparedWithPrimitive,
                binaryExpr.GetLocation(),
                rightType!.Name));
        }
        // Case 2: primitive == string  or  primitive != string
        else if (IsDomainPrimitive(leftType) && IsStringType(rightType))
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.DP0010_StringComparedWithPrimitive,
                binaryExpr.GetLocation(),
                leftType!.Name));
        }
    }

    // ─── DP0011: string s = primitive (implicit or explicit assignment) ───────

    private static void AnalyzeAssignment(SyntaxNodeAnalysisContext context)
    {
        var assignment = (AssignmentExpressionSyntax)context.Node;
        var leftType = context.SemanticModel.GetTypeInfo(assignment.Left, context.CancellationToken).Type;
        var rightType = context.SemanticModel.GetTypeInfo(assignment.Right, context.CancellationToken).Type;

        if (IsStringType(leftType) && IsDomainPrimitive(rightType))
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.DP0011_StringAssignedFromPrimitive,
                assignment.GetLocation(),
                rightType!.Name));
        }
    }

    private static void AnalyzeLocalDeclaration(SyntaxNodeAnalysisContext context)
    {
        var localDecl = (LocalDeclarationStatementSyntax)context.Node;
        var declaredType = context.SemanticModel.GetTypeInfo(localDecl.Declaration.Type, context.CancellationToken).Type;
        if (!IsStringType(declaredType))
            return;

        foreach (var variable in localDecl.Declaration.Variables)
        {
            if (variable.Initializer is not null &&
                IsDomainPrimitive(context.SemanticModel.GetTypeInfo(variable.Initializer.Value, context.CancellationToken).Type) is true &&
                context.SemanticModel.GetTypeInfo(variable.Initializer.Value, context.CancellationToken).Type is { } initType)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.DP0011_StringAssignedFromPrimitive,
                    variable.GetLocation(),
                    initType.Name));
            }
        }
    }

    private static void AnalyzeFieldDeclaration(SyntaxNodeAnalysisContext context)
    {
        var fieldDecl = (FieldDeclarationSyntax)context.Node;
        var declaredType = context.SemanticModel.GetTypeInfo(fieldDecl.Declaration.Type, context.CancellationToken).Type;
        if (!IsStringType(declaredType))
            return;

        foreach (var variable in fieldDecl.Declaration.Variables)
        {
            if (variable.Initializer is not null &&
                IsDomainPrimitive(context.SemanticModel.GetTypeInfo(variable.Initializer.Value, context.CancellationToken).Type) is true &&
                context.SemanticModel.GetTypeInfo(variable.Initializer.Value, context.CancellationToken).Type is { } initType)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.DP0011_StringAssignedFromPrimitive,
                    variable.GetLocation(),
                    initType.Name));
            }
        }
    }

    private static void AnalyzeArgument(SyntaxNodeAnalysisContext context)
    {
        var argument = (ArgumentSyntax)context.Node;
        
        if (argument.Parent is ArgumentListSyntax argList &&
            argList.Parent is InvocationExpressionSyntax invocation &&
            context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol is IMethodSymbol symbol)
        {
            int index = argList.Arguments.IndexOf(argument);
            if (index < symbol.Parameters.Length)
            {
                var paramType = symbol.Parameters[index].Type;
                var argType = context.SemanticModel.GetTypeInfo(argument.Expression, context.CancellationToken).Type;
                if (IsStringType(paramType) && IsDomainPrimitive(argType))
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        DiagnosticDescriptors.DP0011_StringAssignedFromPrimitive,
                        argument.GetLocation(),
                        argType!.Name));
                }
            }
        }
    }

    // ─── Helpers ──────────────────────────────────────────────────────────────

    private static bool IsStringType(ITypeSymbol? type) =>
        type is not null && type.SpecialType == SpecialType.System_String;

    private static bool IsDomainPrimitive(ITypeSymbol? type) =>
        type is INamedTypeSymbol namedType &&
        namedType.AllInterfaces.Any(i =>
            i.OriginalDefinition.ContainingNamespace?.ToDisplayString() == "EricksonLopez.DomainPrimitives" &&
            i.OriginalDefinition.Name is "IDomainPrimitive" or "IStrongId");
}


