// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;

namespace EricksonLopez.DomainPrimitives.Analyzers;

/// <summary>
/// Provides code fixes for <c>DP0015</c> (missing XML documentation) and <c>DP0016</c>
/// (invalid factory method name) reported by <see cref="ApiReviewAnalyzer"/>.
/// </summary>
/// <remarks>
/// The fix for <c>DP0015</c> inserts a skeleton XML <c>&lt;summary&gt;</c> comment.
/// The fix for <c>DP0016</c> renames the non-conforming factory method to <c>Create</c>
/// across the entire solution using the Roslyn rename service.
/// </remarks>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ApiReviewCodeFixProvider)), Shared]
public sealed class ApiReviewCodeFixProvider : CodeFixProvider
{
    /// <inheritdoc />
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("DP0015", "DP0016");

    /// <inheritdoc />
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <inheritdoc />
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = (await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;

        foreach (var diagnostic in context.Diagnostics)
        {
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            if (diagnostic.Id == "DP0015")
            {
                var declaration = root.FindToken(diagnosticSpan.Start).Parent?.FirstAncestorOrSelf<MemberDeclarationSyntax>();
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
                var methodDeclaration = root.FindToken(diagnosticSpan.Start).Parent?.FirstAncestorOrSelf<MethodDeclarationSyntax>();
                if (methodDeclaration != null)
                {
                    var newName = "Create";

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

    /// <summary>
    /// Returns the identifier text of the given <paramref name="memberDeclaration"/>
    /// (method name, property name, or first variable declarator name).
    /// </summary>
    /// <param name="memberDeclaration">The member declaration syntax node to inspect.</param>
    /// <returns>The member's identifier text, or <c>"member"</c> if the kind is unrecognised.</returns>
    internal static string GetMemberName(MemberDeclarationSyntax memberDeclaration) => memberDeclaration switch
    {
        MethodDeclarationSyntax method => method.Identifier.Text,
        PropertyDeclarationSyntax prop => prop.Identifier.Text,
        FieldDeclarationSyntax field when field.Declaration.Variables.Count > 0 => field.Declaration.Variables[0].Identifier.Text,
        _ => "member"
    };

    /// <summary>
    /// Builds the leading XML documentation trivia for the given <paramref name="memberDeclaration"/>,
    /// inserting a skeleton <c>&lt;summary&gt;</c> block that preserves the existing indentation.
    /// </summary>
    /// <param name="memberDeclaration">The member declaration syntax node to prepend documentation to.</param>
    /// <returns>
    /// A <see cref="SyntaxTriviaList"/> containing the merged original trivia plus the generated XML comment block.
    /// </returns>
    internal static SyntaxTriviaList CreateXmlDocTrivia(MemberDeclarationSyntax memberDeclaration)
    {
        var name = GetMemberName(memberDeclaration);
        var existingTrivia = memberDeclaration.GetLeadingTrivia();
        bool hasTrailingWhitespace = existingTrivia.Count > 0 && existingTrivia.Last().IsKind(SyntaxKind.WhitespaceTrivia);
        var leadingWhitespace = hasTrailingWhitespace ? existingTrivia.Last().ToString() : string.Empty;

        var xmlDoc = SyntaxFactory.ParseLeadingTrivia(
            $"{leadingWhitespace}/// <summary>\r\n{leadingWhitespace}/// Gets or sets the {name}.\r\n{leadingWhitespace}/// </summary>\r\n");

        int insertIndex = hasTrailingWhitespace ? existingTrivia.Count - 1 : existingTrivia.Count;
        return existingTrivia.InsertRange(insertIndex, xmlDoc);
    }

    private static async Task<Document> AddXmlDocumentationAsync(Document document, MemberDeclarationSyntax memberDeclaration, CancellationToken cancellationToken)
    {
        var root = (await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false))!;
        var newMember = memberDeclaration.WithLeadingTrivia(CreateXmlDocTrivia(memberDeclaration));
        var newRoot = root.ReplaceNode(memberDeclaration, newMember);

        return document.WithSyntaxRoot(newRoot);
    }

    private static async Task<Solution> RenameMethodAsync(Document document, MethodDeclarationSyntax methodDeclaration, string newName, CancellationToken cancellationToken)
    {
        var semanticModel = (await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false))!;
        var methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration, cancellationToken)!;

        var originalSolution = document.Project.Solution;

        return await Renamer.RenameSymbolAsync(
            originalSolution, 
            methodSymbol, 
            new SymbolRenameOptions(),
            newName, 
            cancellationToken).ConfigureAwait(false);
    }
}



