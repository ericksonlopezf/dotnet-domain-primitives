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
/// Enforces immutability on Value Objects by requiring that all
/// public instance properties declare an <c>init</c> accessor rather than <c>set</c>.
/// </summary>
/// <remarks>
/// Reports <c>DP0008</c> for each public, non-static property on a
/// <c>[ValueObject]</c> record struct or a type inheriting from <c>ValueObject</c> that declares a mutable <c>set</c> accessor.
/// Replace <c>set</c> with <c>init</c> (and <c>required</c> in C# 11+) to satisfy the rule.
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ValueObjectAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(DiagnosticDescriptors.DP0008_ValueObjectRequiresInit, DiagnosticDescriptors.DP0018_ValueObjectMutableCollection);

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
        
        // Find containing type
        var parentType = propertyDecl.FirstAncestorOrSelf<TypeDeclarationSyntax>();
        if (parentType == null)
            return;

        if (context.SemanticModel.GetDeclaredSymbol(parentType, context.CancellationToken) is not INamedTypeSymbol symbol)
            return;

        if (!IsValueObjectType(parentType, symbol))
            return;

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

        if (IsMutableCollectionOrArray(propSymbol.Type))
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.DP0018_ValueObjectMutableCollection,
                propertyDecl.Type.GetLocation(),
                propSymbol.Name,
                symbol.Name,
                propSymbol.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)));
        }
    }

    private static bool IsMutableCollectionOrArray(ITypeSymbol type)
    {
        if (type is IArrayTypeSymbol)
            return true;

        if (type is INamedTypeSymbol namedType)
        {
            var originalDef = namedType.OriginalDefinition.ToDisplayString();
            if (originalDef is "System.Collections.Generic.List<T>" or
                               "System.Collections.Generic.Dictionary<TKey, TValue>" or
                               "System.Collections.Generic.HashSet<T>" or
                               "System.Collections.Generic.Queue<T>" or
                               "System.Collections.Generic.Stack<T>" or
                               "System.Collections.Generic.LinkedList<T>" or
                               "System.Collections.Generic.SortedDictionary<TKey, TValue>" or
                               "System.Collections.Generic.SortedList<TKey, TValue>" or
                               "System.Collections.Generic.SortedSet<T>")
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsValueObjectType(TypeDeclarationSyntax parentType, INamedTypeSymbol symbol)
    {
        if (parentType.IsKind(SyntaxKind.RecordStructDeclaration) &&
            symbol.GetAttributes().Any(a => a.AttributeClass?.Name is "ValueObjectAttribute" or "ValueObject"))
        {
            return true;
        }

        var current = symbol.BaseType;
        while (current != null)
        {
            if (current.Name == "ValueObject" &&
                (current.ContainingNamespace?.ToDisplayString() == "EricksonLopez.DomainPrimitives" ||
                 current.ContainingNamespace is null or { IsGlobalNamespace: true }))
            {
                return true;
            }
            current = current.BaseType;
        }

        return false;
    }
}


