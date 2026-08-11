using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

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
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(
            DiagnosticDescriptors.DP0010_StringComparedWithPrimitive,
            DiagnosticDescriptors.DP0011_StringAssignedFromPrimitive);

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

        if (leftType == null || rightType == null) return;

        // Case 1: string == primitive  or  string != primitive
        if (IsStringType(leftType) && IsDomainPrimitive(rightType))
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.DP0010_StringComparedWithPrimitive,
                binaryExpr.GetLocation(),
                rightType.Name));
            return;
        }

        // Case 2: primitive == string  or  primitive != string
        if (IsDomainPrimitive(leftType) && IsStringType(rightType))
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.DP0010_StringComparedWithPrimitive,
                binaryExpr.GetLocation(),
                leftType.Name));
        }
    }

    // ─── DP0011: string s = primitive (implicit or explicit assignment) ───────

    private static void AnalyzeAssignment(SyntaxNodeAnalysisContext context)
    {
        var assignment = (AssignmentExpressionSyntax)context.Node;

        var leftType = context.SemanticModel.GetTypeInfo(assignment.Left, context.CancellationToken).Type;
        var rightType = context.SemanticModel.GetTypeInfo(assignment.Right, context.CancellationToken).Type;

        if (leftType == null || rightType == null) return;

        if (IsStringType(leftType) && IsDomainPrimitive(rightType))
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.DP0011_StringAssignedFromPrimitive,
                assignment.GetLocation(),
                rightType.Name));
        }
    }

    private static void AnalyzeLocalDeclaration(SyntaxNodeAnalysisContext context)
    {
        var localDecl = (LocalDeclarationStatementSyntax)context.Node;

        foreach (var variable in localDecl.Declaration.Variables)
        {
            if (variable.Initializer == null) continue;

            var declaredType = context.SemanticModel.GetTypeInfo(localDecl.Declaration.Type, context.CancellationToken).Type;
            var initType = context.SemanticModel.GetTypeInfo(variable.Initializer.Value, context.CancellationToken).Type;

            if (declaredType == null || initType == null) continue;

            if (IsStringType(declaredType) && IsDomainPrimitive(initType))
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

        foreach (var variable in fieldDecl.Declaration.Variables)
        {
            if (variable.Initializer == null) continue;

            var declaredType = context.SemanticModel.GetTypeInfo(fieldDecl.Declaration.Type, context.CancellationToken).Type;
            var initType = context.SemanticModel.GetTypeInfo(variable.Initializer.Value, context.CancellationToken).Type;

            if (declaredType == null || initType == null) continue;

            if (IsStringType(declaredType) && IsDomainPrimitive(initType))
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
        
        // Find the parameter this argument corresponds to
        if (argument.Parent is ArgumentListSyntax argList &&
            argList.Parent is InvocationExpressionSyntax invocation)
        {
            var symbol = context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol as IMethodSymbol;
            if (symbol == null) return;
            
            // Find the index of the argument
            int index = -1;
            for (int i = 0; i < argList.Arguments.Count; i++)
            {
                if (argList.Arguments[i] == argument)
                {
                    index = i;
                    break;
                }
            }
            
            if (index == -1 || index >= symbol.Parameters.Length) return;
            
            var paramType = symbol.Parameters[index].Type;
            var argType = context.SemanticModel.GetTypeInfo(argument.Expression, context.CancellationToken).Type;
            
            if (paramType == null || argType == null) return;
            
            if (IsStringType(paramType) && IsDomainPrimitive(argType))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.DP0011_StringAssignedFromPrimitive,
                    argument.GetLocation(),
                    argType.Name));
            }
        }
    }

    // ─── Helpers ──────────────────────────────────────────────────────────────

    private static bool IsStringType(ITypeSymbol type) =>
        type.SpecialType == SpecialType.System_String;

    private static bool IsDomainPrimitive(ITypeSymbol type)
    {
        if (type is not INamedTypeSymbol namedType) return false;

        return namedType.AllInterfaces.Any(i =>
            i.OriginalDefinition.ContainingNamespace.ToDisplayString() == "EricksonLopez.DomainPrimitives" &&
            (i.OriginalDefinition.Name == "IDomainPrimitive" || i.OriginalDefinition.Name == "IStrongId"));
    }
}
