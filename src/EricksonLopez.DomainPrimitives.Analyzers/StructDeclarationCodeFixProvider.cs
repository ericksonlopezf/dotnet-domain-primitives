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

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StructDeclarationCodeFixProvider)), Shared]
public sealed class StructDeclarationCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
        DiagnosticDescriptors.DP0001_MustBePartial.Id, 
        DiagnosticDescriptors.DP0002_MustBeReadonly.Id, 
        DiagnosticDescriptors.DP0003_MustBeRecordStruct.Id);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root == null) return;

        foreach (var diagnostic in context.Diagnostics)
        {
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var node = root.FindToken(diagnosticSpan.Start).Parent?.FirstAncestorOrSelf<TypeDeclarationSyntax>();

            if (node == null) continue;

            if (diagnostic.Id == DiagnosticDescriptors.DP0001_MustBePartial.Id)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: "Make partial",
                        createChangedDocument: c => MakePartialAsync(context.Document, node, c),
                        equivalenceKey: "DP0001_MakePartial"),
                    diagnostic);
            }
            else if (diagnostic.Id == DiagnosticDescriptors.DP0002_MustBeReadonly.Id)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: "Make readonly",
                        createChangedDocument: c => MakeReadonlyAsync(context.Document, node, c),
                        equivalenceKey: "DP0002_MakeReadonly"),
                    diagnostic);
            }
            else if (diagnostic.Id == DiagnosticDescriptors.DP0003_MustBeRecordStruct.Id)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: "Convert to readonly partial record struct",
                        createChangedDocument: c => MakeRecordStructAsync(context.Document, node, c),
                        equivalenceKey: "DP0003_MakeRecordStruct"),
                    diagnostic);
            }
        }
    }

    private static async Task<Document> MakePartialAsync(Document document, TypeDeclarationSyntax node, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        
        var newModifiers = node.Modifiers.Add(SyntaxFactory.Token(SyntaxKind.PartialKeyword));
        var newNode = node.WithModifiers(newModifiers);

        var newRoot = root!.ReplaceNode(node, newNode);
        return document.WithSyntaxRoot(newRoot);
    }

    private static async Task<Document> MakeReadonlyAsync(Document document, TypeDeclarationSyntax node, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        
        // Add readonly before partial if partial exists, otherwise at the end
        var modifiers = node.Modifiers;
        int insertIndex = modifiers.Count;
        for (int i = 0; i < modifiers.Count; i++)
        {
            if (modifiers[i].IsKind(SyntaxKind.PartialKeyword))
            {
                insertIndex = i;
                break;
            }
        }

        var newModifiers = modifiers.Insert(insertIndex, SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword));
        var newNode = node.WithModifiers(newModifiers);

        var newRoot = root!.ReplaceNode(node, newNode);
        return document.WithSyntaxRoot(newRoot);
    }

    private static async Task<Document> MakeRecordStructAsync(Document document, TypeDeclarationSyntax node, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

        // This is a simplified replacement. If the user had a class with body, we attempt to turn it into a record struct.
        // Usually, domain primitives are empty structs with just the attribute.
        // Ensure modifiers include `readonly` and `partial`
        var newModifiers = node.Modifiers;
        if (!newModifiers.Any(SyntaxKind.ReadOnlyKeyword))
        {
            newModifiers = newModifiers.Insert(0, SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword));
        }
        if (!newModifiers.Any(SyntaxKind.PartialKeyword))
        {
            newModifiers = newModifiers.Add(SyntaxFactory.Token(SyntaxKind.PartialKeyword));
        }

        var recordStruct = SyntaxFactory.RecordDeclaration(
            SyntaxKind.RecordStructDeclaration,
            node.AttributeLists,
            newModifiers,
            SyntaxFactory.Token(SyntaxKind.RecordKeyword),
            SyntaxFactory.Token(SyntaxKind.StructKeyword),
            node.Identifier,
            node.TypeParameterList,
            null,
            node.BaseList,
            node.ConstraintClauses,
            node.OpenBraceToken,
            node.Members,
            node.CloseBraceToken,
            node.SemicolonToken);

        var newRoot = root!.ReplaceNode(node, recordStruct);
        return document.WithSyntaxRoot(newRoot);
    }
}
