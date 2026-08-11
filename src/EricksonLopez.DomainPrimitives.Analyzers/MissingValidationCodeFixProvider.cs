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

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MissingValidationCodeFixProvider)), Shared]
public sealed class MissingValidationCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
        DiagnosticDescriptors.DP0009_MissingValidation.Id);

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

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Add [NotEmpty] validation attribute",
                    createChangedDocument: c => AddNotEmptyAttributeAsync(context.Document, node, c),
                    equivalenceKey: "DP0009_AddNotEmptyAttribute"),
                diagnostic);
        }
    }

    private static async Task<Document> AddNotEmptyAttributeAsync(Document document, TypeDeclarationSyntax node, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root == null) return document;

        var attributeName = SyntaxFactory.ParseName("NotEmpty");
        var attribute = SyntaxFactory.Attribute(attributeName);
        var attributeList = SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(attribute));

        var newNode = node.AddAttributeLists(attributeList);
        var newRoot = root.ReplaceNode(node, newNode);

        return document.WithSyntaxRoot(newRoot);
    }
}
