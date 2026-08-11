using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EricksonLopez.DomainPrimitives.Analyzers;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StringComparisonCodeFixProvider)), Shared]
public sealed class StringComparisonCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
        DiagnosticDescriptors.DP0010_StringComparedWithPrimitive.Id,
        DiagnosticDescriptors.DP0011_StringAssignedFromPrimitive.Id);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root == null) return;

        foreach (var diagnostic in context.Diagnostics)
        {
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var token = root.FindToken(diagnosticSpan.Start);

            if (diagnostic.Id == DiagnosticDescriptors.DP0011_StringAssignedFromPrimitive.Id)
            {
                var node = root.FindNode(diagnosticSpan);
                ExpressionSyntax targetExpr = null;

                if (node is VariableDeclaratorSyntax varDecl && varDecl.Initializer != null)
                {
                    targetExpr = varDecl.Initializer.Value;
                }
                else if (node is AssignmentExpressionSyntax assignExpr)
                {
                    targetExpr = assignExpr.Right;
                }
                else if (node is ArgumentSyntax argSyntax)
                {
                    targetExpr = argSyntax.Expression;
                }

                if (targetExpr != null)
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: "Access .Value property explicitly",
                            createChangedDocument: c => AppendValueAccessAsync(context.Document, targetExpr, c),
                            equivalenceKey: "DP0011_AppendValueAccess"),
                        diagnostic);
                }
            }
            else if (diagnostic.Id == DiagnosticDescriptors.DP0010_StringComparedWithPrimitive.Id)
            {
                var binaryExpr = root.FindNode(diagnosticSpan) as BinaryExpressionSyntax;
                if (binaryExpr != null)
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: "Access .Value for primitive in comparison",
                            createChangedDocument: c => FixComparisonAsync(context.Document, binaryExpr, c),
                            equivalenceKey: "DP0010_FixComparison"),
                        diagnostic);
                }
            }
        }
    }

    private static async Task<Document> AppendValueAccessAsync(Document document, ExpressionSyntax targetExpr, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root == null) return document;

        var memberAccess = SyntaxFactory.MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            targetExpr,
            SyntaxFactory.IdentifierName("Value"));

        var newRoot = root.ReplaceNode(targetExpr, memberAccess);
        return document.WithSyntaxRoot(newRoot);
    }

    private static async Task<Document> FixComparisonAsync(Document document, BinaryExpressionSyntax binaryExpr, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root == null) return document;

        var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
        if (semanticModel == null) return document;

        var leftType = semanticModel.GetTypeInfo(binaryExpr.Left, cancellationToken).Type;
        var rightType = semanticModel.GetTypeInfo(binaryExpr.Right, cancellationToken).Type;

        ExpressionSyntax newLeft = binaryExpr.Left;
        ExpressionSyntax newRight = binaryExpr.Right;

        if (leftType != null && IsDomainPrimitive(leftType))
        {
            newLeft = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                binaryExpr.Left,
                SyntaxFactory.IdentifierName("Value"));
        }

        if (rightType != null && IsDomainPrimitive(rightType))
        {
            newRight = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                binaryExpr.Right,
                SyntaxFactory.IdentifierName("Value"));
        }

        var newBinary = binaryExpr.WithLeft(newLeft).WithRight(newRight);
        var newRoot = root.ReplaceNode(binaryExpr, newBinary);
        return document.WithSyntaxRoot(newRoot);
    }

    private static bool IsDomainPrimitive(ITypeSymbol typeSymbol)
    {
        return typeSymbol.AllInterfaces.Any(i =>
            i.Name is "IDomainPrimitive" or "IStrongId");
    }
}
