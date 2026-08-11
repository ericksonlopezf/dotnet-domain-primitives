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

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ValueObjectAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(DiagnosticDescriptors.DP0008_ValueObjectRequiresInit);

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

        var symbol = context.SemanticModel.GetDeclaredSymbol(parentStruct, context.CancellationToken) as INamedTypeSymbol;
        if (symbol == null) return;

        bool isValueObject = symbol.GetAttributes().Any(a => a.AttributeClass?.Name == "ValueObjectAttribute");
        if (!isValueObject) return;

        var propSymbol = context.SemanticModel.GetDeclaredSymbol(propertyDecl, context.CancellationToken) as IPropertySymbol;
        if (propSymbol == null || propSymbol.IsStatic || propSymbol.DeclaredAccessibility != Accessibility.Public) return;

        bool hasInit = false;
        bool hasSet = false;
        
        if (propertyDecl.AccessorList != null)
        {
            foreach (var accessor in propertyDecl.AccessorList.Accessors)
            {
                if (accessor.IsKind(SyntaxKind.InitAccessorDeclaration))
                {
                    hasInit = true;
                }
                else if (accessor.IsKind(SyntaxKind.SetAccessorDeclaration))
                {
                    hasSet = true;
                }
            }
        }
        else if (propertyDecl.ExpressionBody != null)
        {
            // Expression bodied property (get only), perfectly valid for immutable Value Object
            return;
        }

        // We only require `init` if there is a setter. If there's no setter at all, it's immutable, which is fine.
        if (hasSet && !hasInit)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.DP0008_ValueObjectRequiresInit,
                propertyDecl.Identifier.GetLocation(),
                propSymbol.Name,
                symbol.Name));
        }
    }
}
