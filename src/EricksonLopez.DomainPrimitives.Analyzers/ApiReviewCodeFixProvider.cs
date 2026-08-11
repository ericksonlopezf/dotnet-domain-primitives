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
using Microsoft.CodeAnalysis.Rename;

namespace EricksonLopez.DomainPrimitives.Analyzers;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ApiReviewCodeFixProvider)), Shared]
public sealed class ApiReviewCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("DP0015", "DP0016");

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root == null) return;

        foreach (var diagnostic in context.Diagnostics)
        {
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            if (diagnostic.Id == "DP0015")
            {
                var declaration = root.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf().OfType<MemberDeclarationSyntax>().FirstOrDefault();
                if (declaration != null)
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: "Add XML Documentation",
                            createChangedDocument: c => AddXmlDocumentationAsync(context.Document, declaration, c),
                            equivalenceKey: "AddXmlDocumentation"),
                        diagnostic);
                }
            }
            else if (diagnostic.Id == "DP0016")
            {
                var methodDeclaration = root.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().FirstOrDefault();
                if (methodDeclaration != null)
                {
                    var isTry = methodDeclaration.ReturnType.ToString() == "bool";
                    var newName = isTry ? "TryCreate" : "Create";

                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: $"Rename to '{newName}'",
                            createChangedSolution: c => RenameMethodAsync(context.Document, methodDeclaration, newName, c),
                            equivalenceKey: "RenameFactoryMethod"),
                        diagnostic);
                }
            }
        }
    }

    private static async Task<Document> AddXmlDocumentationAsync(Document document, MemberDeclarationSyntax memberDeclaration, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root == null) return document;

        var name = memberDeclaration switch
        {
            MethodDeclarationSyntax method => method.Identifier.Text,
            PropertyDeclarationSyntax prop => prop.Identifier.Text,
            FieldDeclarationSyntax field => field.Declaration.Variables.FirstOrDefault()?.Identifier.Text ?? "member",
            _ => "member"
        };

        // Extract the existing leading whitespace (e.g. 4 spaces)
        var leadingWhitespace = string.Empty;
        var existingTrivia = memberDeclaration.GetLeadingTrivia();
        if (existingTrivia.Count > 0 && existingTrivia.Last().IsKind(SyntaxKind.WhitespaceTrivia))
        {
            leadingWhitespace = existingTrivia.Last().ToString();
        }

        var xmlString = $"{leadingWhitespace}/// <summary>\r\n{leadingWhitespace}/// Gets or sets the {name}.\r\n{leadingWhitespace}/// </summary>\r\n";
        var xmlDoc = SyntaxFactory.ParseLeadingTrivia(xmlString);

        // We insert before the trailing whitespace, so the summary inherits the whitespace
        int insertIndex = existingTrivia.Count > 0 && existingTrivia.Last().IsKind(SyntaxKind.WhitespaceTrivia)
            ? existingTrivia.Count - 1
            : existingTrivia.Count;
            
        var newTrivia = existingTrivia.InsertRange(insertIndex, xmlDoc);

        var newMember = memberDeclaration.WithLeadingTrivia(newTrivia);
        var newRoot = root.ReplaceNode(memberDeclaration, newMember);

        return document.WithSyntaxRoot(newRoot);
    }

    private static async Task<Solution> RenameMethodAsync(Document document, MethodDeclarationSyntax methodDeclaration, string newName, CancellationToken cancellationToken)
    {
        var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
        if (semanticModel == null) return document.Project.Solution;

        var methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration, cancellationToken);
        if (methodSymbol == null) return document.Project.Solution;

        var originalSolution = document.Project.Solution;

        return await Renamer.RenameSymbolAsync(
            originalSolution, 
            methodSymbol, 
            new SymbolRenameOptions(),
            newName, 
            cancellationToken).ConfigureAwait(false);
    }
}
