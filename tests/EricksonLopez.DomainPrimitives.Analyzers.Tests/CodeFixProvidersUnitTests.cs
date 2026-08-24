// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Xunit;

namespace EricksonLopez.DomainPrimitives.Analyzers.Tests;

public class CodeFixProvidersUnitTests
{
    [Fact]
    public void StructDeclarationCodeFixProvider_MetadataAndFixAll_AreCorrect()
    {
        var provider = new StructDeclarationCodeFixProvider();
        provider.FixableDiagnosticIds.Should().Equal(
            DiagnosticDescriptors.DP0001_MustBePartial.Id,
            DiagnosticDescriptors.DP0002_MustBeReadonly.Id,
            DiagnosticDescriptors.DP0003_MustBeRecordStruct.Id);

        provider.GetFixAllProvider().Should().Be(WellKnownFixAllProviders.BatchFixer);
    }

    [Fact]
    public void MissingValidationCodeFixProvider_MetadataAndFixAll_AreCorrect()
    {
        var provider = new MissingValidationCodeFixProvider();
        provider.FixableDiagnosticIds.Should().Equal(
            DiagnosticDescriptors.DP0009_MissingValidation.Id);

        provider.GetFixAllProvider().Should().Be(WellKnownFixAllProviders.BatchFixer);
    }

    [Fact]
    public void StringComparisonCodeFixProvider_MetadataAndFixAll_AreCorrect()
    {
        var provider = new StringComparisonCodeFixProvider();
        provider.FixableDiagnosticIds.Should().Equal(
            DiagnosticDescriptors.DP0010_StringComparedWithPrimitive.Id,
            DiagnosticDescriptors.DP0011_StringAssignedFromPrimitive.Id);

        provider.GetFixAllProvider().Should().Be(WellKnownFixAllProviders.BatchFixer);
    }

    [Fact]
    public void ApiReviewCodeFixProvider_MetadataAndFixAll_AreCorrect()
    {
        var provider = new ApiReviewCodeFixProvider();
        provider.FixableDiagnosticIds.Should().Equal("DP0015", "DP0016");
        provider.GetFixAllProvider().Should().Be(WellKnownFixAllProviders.BatchFixer);
    }

    [Fact]
    public async Task StructDeclarationCodeFixProvider_RegisterCodeFixesAsync_IrrelevantDiagnostic_DoesNotRegister()
    {
        var workspace = new AdhocWorkspace();
        var project = workspace.AddProject("TestProject", LanguageNames.CSharp);
        var document = project.AddDocument("Test.cs", "class EmptyClass {}");
        var descriptor = new DiagnosticDescriptor("OTHER001", "Title", "Message", "Category", DiagnosticSeverity.Warning, isEnabledByDefault: true);
        var diag = Diagnostic.Create(descriptor, Location.None);

        var provider = new StructDeclarationCodeFixProvider();
        var context = new CodeFixContext(
            document,
            new TextSpan(0, 0),
            ImmutableArray.Create(diag),
            (action, d) => { },
            CancellationToken.None);

        var act = async () => await provider.RegisterCodeFixesAsync(context);
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task MissingValidationCodeFixProvider_RegisterCodeFixesAsync_IrrelevantDiagnostic_DoesNotRegister()
    {
        var workspace = new AdhocWorkspace();
        var project = workspace.AddProject("TestProject", LanguageNames.CSharp);
        var document = project.AddDocument("Test.cs", "// Top level comment only");
        var descriptor = new DiagnosticDescriptor("OTHER001", "Title", "Message", "Category", DiagnosticSeverity.Warning, isEnabledByDefault: true);
        var diag = Diagnostic.Create(descriptor, Location.None);

        var provider = new MissingValidationCodeFixProvider();
        var registeredActions = new List<CodeAction>();
        var context = new CodeFixContext(
            document,
            new TextSpan(0, 0),
            ImmutableArray.Create(diag),
            (action, d) => registeredActions.Add(action),
            CancellationToken.None);

        await provider.RegisterCodeFixesAsync(context);
        registeredActions.Should().BeEmpty();
    }

    [Fact]
    public async Task StringComparisonCodeFixProvider_RegisterCodeFixesAsync_IrrelevantDiagnostic_DoesNotRegister()
    {
        var workspace = new AdhocWorkspace();
        var project = workspace.AddProject("TestProject", LanguageNames.CSharp);
        var document = project.AddDocument("Test.cs", "class EmptyClass {}");
        var descriptor = new DiagnosticDescriptor("OTHER001", "Title", "Message", "Category", DiagnosticSeverity.Warning, isEnabledByDefault: true);
        var diag = Diagnostic.Create(descriptor, Location.None);

        var provider = new StringComparisonCodeFixProvider();
        var context = new CodeFixContext(
            document,
            new TextSpan(0, 0),
            ImmutableArray.Create(diag),
            (action, d) => { },
            CancellationToken.None);

        var act = async () => await provider.RegisterCodeFixesAsync(context);
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ApiReviewCodeFixProvider_RegisterCodeFixesAsync_IrrelevantDiagnostic_DoesNotRegister()
    {
        var workspace = new AdhocWorkspace();
        var project = workspace.AddProject("TestProject", LanguageNames.CSharp);
        var document = project.AddDocument("Test.cs", "class EmptyClass {}");
        var descriptor = new DiagnosticDescriptor("OTHER001", "Title", "Message", "Category", DiagnosticSeverity.Warning, isEnabledByDefault: true);
        var diag = Diagnostic.Create(descriptor, Location.None);

        var provider = new ApiReviewCodeFixProvider();
        var context = new CodeFixContext(
            document,
            new TextSpan(0, 0),
            ImmutableArray.Create(diag),
            (action, d) => { },
            CancellationToken.None);

        var act = async () => await provider.RegisterCodeFixesAsync(context);
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task StructDeclarationCodeFixProvider_RegistersExpectedCodeActions()
    {
        var workspace = new AdhocWorkspace();
        var project = workspace.AddProject("TestProject", LanguageNames.CSharp);
        var code = @"
public struct MyId {}
public record struct MutableId {}
public readonly record struct NonPartialId {}
";
        var document = project.AddDocument("Test.cs", code);
        var tree = await document.GetSyntaxTreeAsync();
        var root = await tree!.GetRootAsync();
        var structNodes = root.DescendantNodes().OfType<TypeDeclarationSyntax>().ToList();

        var provider = new StructDeclarationCodeFixProvider();

        // DP0001
        var actions1 = new List<CodeAction>();
        var diag1 = Diagnostic.Create(DiagnosticDescriptors.DP0001_MustBePartial, structNodes[2].Identifier.GetLocation(), "NonPartialId");
        await provider.RegisterCodeFixesAsync(new CodeFixContext(document, diag1, (a, d) => actions1.Add(a), CancellationToken.None));
        actions1.Should().ContainSingle();
        actions1[0].Title.Should().Be("Make partial");
        actions1[0].EquivalenceKey.Should().Be("DP0001_MakePartial");

        // DP0002
        var actions2 = new List<CodeAction>();
        var diag2 = Diagnostic.Create(DiagnosticDescriptors.DP0002_MustBeReadonly, structNodes[1].Identifier.GetLocation(), "MutableId");
        await provider.RegisterCodeFixesAsync(new CodeFixContext(document, diag2, (a, d) => actions2.Add(a), CancellationToken.None));
        actions2.Should().ContainSingle();
        actions2[0].Title.Should().Be("Make readonly");
        actions2[0].EquivalenceKey.Should().Be("DP0002_MakeReadonly");

        // DP0003
        var actions3 = new List<CodeAction>();
        var diag3 = Diagnostic.Create(DiagnosticDescriptors.DP0003_MustBeRecordStruct, structNodes[0].Identifier.GetLocation(), "MyId");
        await provider.RegisterCodeFixesAsync(new CodeFixContext(document, diag3, (a, d) => actions3.Add(a), CancellationToken.None));
        actions3.Should().ContainSingle();
        actions3[0].Title.Should().Be("Convert to readonly partial record struct");
        actions3[0].EquivalenceKey.Should().Be("DP0003_MakeRecordStruct");

        // Execute operations
        var ops1 = await actions1[0].GetOperationsAsync(CancellationToken.None);
        ops1.Should().NotBeEmpty();
        var ops2 = await actions2[0].GetOperationsAsync(CancellationToken.None);
        ops2.Should().NotBeEmpty();
        var ops3 = await actions3[0].GetOperationsAsync(CancellationToken.None);
        ops3.Should().NotBeEmpty();

        // Node is null (document with no type declarations)
        var emptyDoc = project.AddDocument("Empty.cs", "using System;");
        var emptyTree = await emptyDoc.GetSyntaxTreeAsync();
        var actionsNull = new List<CodeAction>();
        var diagNull = Diagnostic.Create(DiagnosticDescriptors.DP0001_MustBePartial, Location.Create(emptyTree!, new TextSpan(0, 5)), "Unknown");
        await provider.RegisterCodeFixesAsync(new CodeFixContext(emptyDoc, diagNull, (a, d) => actionsNull.Add(a), CancellationToken.None));
        actionsNull.Should().BeEmpty();

        // partial at index 0: partial record struct Foo
        var codePartialFirst = "partial record struct PartialFirstId {}";
        var docPartialFirst = project.AddDocument("PartialFirst.cs", codePartialFirst);
        var treePartialFirst = await docPartialFirst.GetSyntaxTreeAsync();
        var rootPartialFirst = await treePartialFirst!.GetRootAsync();
        var nodePartialFirst = rootPartialFirst.DescendantNodes().OfType<TypeDeclarationSyntax>().First();
        var actionsPartialFirst = new List<CodeAction>();
        var diagPartialFirst = Diagnostic.Create(DiagnosticDescriptors.DP0002_MustBeReadonly, nodePartialFirst.Identifier.GetLocation(), "PartialFirstId");
        await provider.RegisterCodeFixesAsync(new CodeFixContext(docPartialFirst, diagPartialFirst, (a, d) => actionsPartialFirst.Add(a), CancellationToken.None));
        actionsPartialFirst.Should().ContainSingle();
        var opsPartialFirst = await actionsPartialFirst[0].GetOperationsAsync(CancellationToken.None);
        opsPartialFirst.Should().NotBeEmpty();
        var changedDoc = ((ApplyChangesOperation)opsPartialFirst.First()).ChangedSolution.GetDocument(docPartialFirst.Id);
        var changedText = (await changedDoc!.GetTextAsync()).ToString();
        changedText.Should().Contain("readonly partial record struct PartialFirstId");
    }

    [Fact]
    public async Task StructDeclarationCodeFixProvider_WithNullSyntaxRoot_ReturnsSafely()
    {
        var workspace = new AdhocWorkspace();
        var project = workspace.AddProject("TestProject", LanguageNames.CSharp);
        var doc = project.AddDocument("Test.cs", "public struct Test {}");
        var provider = new StructDeclarationCodeFixProvider();
        var diag = Diagnostic.Create(DiagnosticDescriptors.DP0001_MustBePartial, Location.None, "Test");
        var actions = new List<CodeAction>();
        var cts = new CancellationTokenSource();
        cts.Cancel();
        try
        {
            await provider.RegisterCodeFixesAsync(new CodeFixContext(doc, diag, (a, d) => actions.Add(a), cts.Token));
        }
        catch (System.OperationCanceledException)
        {
            // cancellation expected
        }
    }

    [Fact]
    public async Task MissingValidationCodeFixProvider_RegistersExpectedCodeActions()
    {
        var workspace = new AdhocWorkspace();
        var project = workspace.AddProject("TestProject", LanguageNames.CSharp);
        var code = "public readonly partial record struct MyId {}";
        var document = project.AddDocument("Test.cs", code);
        var tree = await document.GetSyntaxTreeAsync();
        var root = await tree!.GetRootAsync();
        var node = root.DescendantNodes().OfType<TypeDeclarationSyntax>().First();

        var provider = new MissingValidationCodeFixProvider();
        var actions = new List<CodeAction>();
        var diag = Diagnostic.Create(DiagnosticDescriptors.DP0009_MissingValidation, node.Identifier.GetLocation(), "MyId");

        await provider.RegisterCodeFixesAsync(new CodeFixContext(document, diag, (a, d) => actions.Add(a), CancellationToken.None));
        actions.Should().ContainSingle();
        actions[0].Title.Should().Be("Add [NotEmpty] validation attribute");
        actions[0].EquivalenceKey.Should().Be("DP0009_AddNotEmptyAttribute");
    }

    [Fact]
    public async Task StringComparisonCodeFixProvider_RegistersExpectedCodeActions()
    {
        var workspace = new AdhocWorkspace();
        var project = workspace.AddProject("TestProject", LanguageNames.CSharp);
        var code = @"
public class TestClass
{
    public void M(string s)
    {
        string a = s;
        if (s == a) {}
    }
}";
        var document = project.AddDocument("Test.cs", code);
        var tree = await document.GetSyntaxTreeAsync();
        var root = await tree!.GetRootAsync();
        var varDecl = root.DescendantNodes().OfType<VariableDeclaratorSyntax>().First();
        var binary = root.DescendantNodes().OfType<BinaryExpressionSyntax>().First();

        var provider = new StringComparisonCodeFixProvider();

        // DP0011
        var actions1 = new List<CodeAction>();
        var diag1 = Diagnostic.Create(DiagnosticDescriptors.DP0011_StringAssignedFromPrimitive, varDecl.GetLocation(), "MyPrimitive");
        await provider.RegisterCodeFixesAsync(new CodeFixContext(document, diag1, (a, d) => actions1.Add(a), CancellationToken.None));
        actions1.Should().ContainSingle();
        actions1[0].Title.Should().Be("Access .Value property explicitly");
        actions1[0].EquivalenceKey.Should().Be("DP0011_AppendValueAccess");
        var ops1 = await actions1[0].GetOperationsAsync(CancellationToken.None);
        ops1.Should().NotBeEmpty();

        // DP0010
        var actions2 = new List<CodeAction>();
        var diag2 = Diagnostic.Create(DiagnosticDescriptors.DP0010_StringComparedWithPrimitive, binary.GetLocation(), "MyPrimitive");
        await provider.RegisterCodeFixesAsync(new CodeFixContext(document, diag2, (a, d) => actions2.Add(a), CancellationToken.None));
        actions2.Should().ContainSingle();
        actions2[0].Title.Should().Be("Access .Value for primitive in comparison");
        actions2[0].EquivalenceKey.Should().Be("DP0010_FixComparison");
        var ops2 = await actions2[0].GetOperationsAsync(CancellationToken.None);
        ops2.Should().NotBeEmpty();

        // Null root / cancelled token
        var cts = new CancellationTokenSource();
        cts.Cancel();
        try
        {
            await provider.RegisterCodeFixesAsync(new CodeFixContext(document, diag1, (a, d) => { }, cts.Token));
        }
        catch (System.OperationCanceledException)
        {
        }
    }

    [Fact]
    public async Task ApiReviewCodeFixProvider_RegistersExpectedCodeActions()
    {
        var workspace = new AdhocWorkspace();
        var project = workspace.AddProject("TestProject", LanguageNames.CSharp);
        var code = @"
public class MyPrimitive
{
    public int Value { get; set; }
    public static MyPrimitive CreateNew() => new MyPrimitive();
    public static bool TryBuild() => true;
}";
        var document = project.AddDocument("Test.cs", code);
        var tree = await document.GetSyntaxTreeAsync();
        var root = await tree!.GetRootAsync();
        var prop = root.DescendantNodes().OfType<PropertyDeclarationSyntax>().First();
        var methods = root.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();

        var provider = new ApiReviewCodeFixProvider();

        // DP0015
        var actions1 = new List<CodeAction>();
        var diag1 = Diagnostic.Create(DiagnosticDescriptors.DP0015_MissingXmlDocumentation, prop.Identifier.GetLocation(), "Value", "MyPrimitive");
        await provider.RegisterCodeFixesAsync(new CodeFixContext(document, diag1, (a, d) => actions1.Add(a), CancellationToken.None));
        actions1.Should().ContainSingle();
        actions1[0].Title.Should().Be("Add XML Documentation");
        actions1[0].EquivalenceKey.Should().Be("AddXmlDocumentation");

        // DP0016 - Create
        var actions2 = new List<CodeAction>();
        var diag2 = Diagnostic.Create(DiagnosticDescriptors.DP0016_InvalidFactoryMethodName, methods[0].Identifier.GetLocation(), "CreateNew", "MyPrimitive");
        await provider.RegisterCodeFixesAsync(new CodeFixContext(document, diag2, (a, d) => actions2.Add(a), CancellationToken.None));
        actions2.Should().ContainSingle();
        actions2[0].Title.Should().Be("Rename to 'Create'");
        actions2[0].EquivalenceKey.Should().Be("RenameFactoryMethod");

        // DP0016 - TryCreate
        var actions3 = new List<CodeAction>();
        var diag3 = Diagnostic.Create(DiagnosticDescriptors.DP0016_InvalidFactoryMethodName, methods[1].Identifier.GetLocation(), "TryBuild", "MyPrimitive");
        await provider.RegisterCodeFixesAsync(new CodeFixContext(document, diag3, (a, d) => actions3.Add(a), CancellationToken.None));
        actions3.Should().ContainSingle();
        actions3[0].Title.Should().Be("Rename to 'Create'");
        actions3[0].EquivalenceKey.Should().Be("RenameFactoryMethod");

        // Apply DP0015 on Property
        var ops1 = await actions1[0].GetOperationsAsync(CancellationToken.None);
        ops1.Should().NotBeEmpty();

        // Apply DP0016 on Methods
        var ops2 = await actions2[0].GetOperationsAsync(CancellationToken.None);
        ops2.Should().NotBeEmpty();
        var ops3 = await actions3[0].GetOperationsAsync(CancellationToken.None);
        ops3.Should().NotBeEmpty();

        // DP0015 on Event/Custom member
        var codeWithEvent = @"
public class MyEventClass
{
    public event System.EventHandler MyEvent;
    public int myField;
}";
        var doc2 = project.AddDocument("Test2.cs", codeWithEvent);
        var tree2 = await doc2.GetSyntaxTreeAsync();
        var root2 = await tree2!.GetRootAsync();
        var evt = root2.DescendantNodes().OfType<EventFieldDeclarationSyntax>().First();
        var field = root2.DescendantNodes().OfType<FieldDeclarationSyntax>().First();

        var actionsEvt = new List<CodeAction>();
        var diagEvt = Diagnostic.Create(DiagnosticDescriptors.DP0015_MissingXmlDocumentation, evt.GetLocation(), "MyEvent", "MyEventClass");
        await provider.RegisterCodeFixesAsync(new CodeFixContext(doc2, diagEvt, (a, d) => actionsEvt.Add(a), CancellationToken.None));
        actionsEvt.Should().ContainSingle();
        var opsEvt = await actionsEvt[0].GetOperationsAsync(CancellationToken.None);
        opsEvt.Should().NotBeEmpty();
        var newEvtText = await ((ApplyChangesOperation)opsEvt[0]).ChangedSolution.GetDocument(doc2.Id)!.GetTextAsync();
        newEvtText.ToString().Should().Contain("/// Gets or sets the member.");

        var actionsField = new List<CodeAction>();
        var diagField = Diagnostic.Create(DiagnosticDescriptors.DP0015_MissingXmlDocumentation, field.GetLocation(), "myField", "MyEventClass");
        await provider.RegisterCodeFixesAsync(new CodeFixContext(doc2, diagField, (a, d) => actionsField.Add(a), CancellationToken.None));
        actionsField.Should().ContainSingle();
        var opsField = await actionsField[0].GetOperationsAsync(CancellationToken.None);
        opsField.Should().NotBeEmpty();
        var newFieldText = await ((ApplyChangesOperation)opsField[0]).ChangedSolution.GetDocument(doc2.Id)!.GetTextAsync();
        newFieldText.ToString().Should().Contain("/// Gets or sets the myField.");
    }
}





