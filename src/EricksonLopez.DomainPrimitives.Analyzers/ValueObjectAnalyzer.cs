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
/// Enforces immutability on <c>[ValueObject]</c> properties by requiring that all
/// public instance properties declare an <c>init</c> accessor rather than <c>set</c>.
/// </summary>
/// <remarks>
/// Reports <c>DP0008</c> for each public, non-static property on a
/// <c>[ValueObject]</c> record struct that declares a mutable <c>set</c> accessor.
/// Replace <c>set</c> with <c>init</c> (and <c>required</c> in C# 11+) to satisfy the rule.
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ValueObjectAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(DiagnosticDescriptors.DP0008_ValueObjectRequiresInit);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        
        context.RegisterSyntaxNodeAction(AnalyzePropertyDeclaration, SyntaxKind.PropertyDeclaration);
    }

    private void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
    {
        var propertyDecl = (PropertyDeclarationSyntax)context.Node;
        
        // Find containing struct
        var parentStruct = propertyDecl.FirstAncestorOrSelf<TypeDeclarationSyntax>();
        if (parentStruct == null || !parentStruct.IsKind(SyntaxKind.RecordStructDeclaration))
            return;

        var symbol = (INamedTypeSymbol)context.SemanticModel.GetDeclaredSymbol(parentStruct, context.CancellationToken)!;

        bool isValueObject = symbol.GetAttributes().Any(a => a.AttributeClass?.Name == "ValueObjectAttribute");
        if (!isValueObject) return;

        if (context.SemanticModel.GetDeclaredSymbol(propertyDecl, context.CancellationToken) is not IPropertySymbol propSymbol ||
            propSymbol.IsStatic ||
            propSymbol.DeclaredAccessibility != Accessibility.Public)
            return;

        if (propertyDecl.AccessorList?.Accessors.Any(a => a.IsKind(SyntaxKind.SetAccessorDeclaration)) == true)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.DP0008_ValueObjectRequiresInit,
                propertyDecl.Identifier.GetLocation(),
                propSymbol.Name,
                symbol.Name));
        }
    }
}


