// Copyright © Erickson Lopez. MIT License.
using AwesomeAssertions;
using Microsoft.CodeAnalysis;
using Xunit;

namespace EricksonLopez.DomainPrimitives.Analyzers.Tests;

public class DiagnosticDescriptorsUnitTests
{
    [Fact]
    public void DiagnosticCategories_Constants_HaveExpectedValues()
    {
        DiagnosticCategories.Correctness.Should().Be("Correctness");
        DiagnosticCategories.Design.Should().Be("Design");
        DiagnosticCategories.Performance.Should().Be("Performance");
        DiagnosticCategories.Migration.Should().Be("Migration");
        DiagnosticCategories.ApiReview.Should().Be("ApiReview");
    }

    [Fact]
    public void DP0001_MustBePartial_HasExpectedMetadata()
    {
        var d = DiagnosticDescriptors.DP0001_MustBePartial;
        d.Id.Should().Be("DP0001");
        d.Title.ToString().Should().Be("Domain primitive must be partial");
        d.MessageFormat.ToString().Should().Be("Type '{0}' is decorated with a domain primitive attribute but is not declared as 'partial'");
        d.Category.Should().Be(DiagnosticCategories.Correctness);
        d.DefaultSeverity.Should().Be(DiagnosticSeverity.Error);
        d.IsEnabledByDefault.Should().BeTrue();
        d.Description.ToString().Should().Be("Domain primitive types must be declared as 'partial' so the source generator can add the required implementation.");
        d.HelpLinkUri.Should().Be("https://github.com/ericksonlopezf/dotnet-domain-primitives/blob/main/docs/rules/dp0001.md");
    }

    [Fact]
    public void DP0002_MustBeReadonly_HasExpectedMetadata()
    {
        var d = DiagnosticDescriptors.DP0002_MustBeReadonly;
        d.Id.Should().Be("DP0002");
        d.Title.ToString().Should().Be("Domain primitive must be readonly");
        d.MessageFormat.ToString().Should().Be("Type '{0}' is decorated with a domain primitive attribute but is not declared as 'readonly'");
        d.Category.Should().Be(DiagnosticCategories.Correctness);
        d.DefaultSeverity.Should().Be(DiagnosticSeverity.Error);
        d.IsEnabledByDefault.Should().BeTrue();
        d.Description.ToString().Should().Be("Domain primitive types must be immutable. Mark the struct as 'readonly'.");
        d.HelpLinkUri.Should().Be("https://github.com/ericksonlopezf/dotnet-domain-primitives/blob/main/docs/rules/dp0002.md");
    }

    [Fact]
    public void DP0003_MustBeRecordStruct_HasExpectedMetadata()
    {
        var d = DiagnosticDescriptors.DP0003_MustBeRecordStruct;
        d.Id.Should().Be("DP0003");
        d.Title.ToString().Should().Be("Domain primitive must be a record struct");
        d.MessageFormat.ToString().Should().Be("Type '{0}' must be declared as a 'record struct'");
        d.Category.Should().Be(DiagnosticCategories.Correctness);
        d.DefaultSeverity.Should().Be(DiagnosticSeverity.Error);
        d.IsEnabledByDefault.Should().BeTrue();
        d.Description.ToString().Should().Be("Domain primitives rely on zero-boxing, structural equality provided natively by 'record struct' in C# 10+.");
        d.HelpLinkUri.Should().Be("https://github.com/ericksonlopezf/dotnet-domain-primitives/blob/main/docs/rules/dp0003.md");
    }

    [Fact]
    public void DP0004_InvalidRegex_HasExpectedMetadata()
    {
        var d = DiagnosticDescriptors.DP0004_InvalidRegex;
        d.Id.Should().Be("DP0004");
        d.Title.ToString().Should().Be("Invalid Regex Pattern");
        d.MessageFormat.ToString().Should().Be("The regex pattern '{0}' is invalid: {1}");
        d.Category.Should().Be(DiagnosticCategories.Correctness);
        d.DefaultSeverity.Should().Be(DiagnosticSeverity.Error);
        d.IsEnabledByDefault.Should().BeTrue();
        d.Description.ToString().Should().Be("Ensure the regular expression is syntactically valid.");
        d.HelpLinkUri.Should().Be("https://github.com/ericksonlopezf/dotnet-domain-primitives/blob/main/docs/rules/dp0004.md");
    }

    [Fact]
    public void DP0005_ConflictingNormalization_HasExpectedMetadata()
    {
        var d = DiagnosticDescriptors.DP0005_ConflictingNormalization;
        d.Id.Should().Be("DP0005");
        d.Title.ToString().Should().Be("Conflicting normalization attributes");
        d.MessageFormat.ToString().Should().Be("Type '{0}' cannot have both [LowerCase] and [UpperCase] attributes");
        d.Category.Should().Be(DiagnosticCategories.Correctness);
        d.DefaultSeverity.Should().Be(DiagnosticSeverity.Error);
        d.IsEnabledByDefault.Should().BeTrue();
        d.Description.ToString().Should().Be("Remove one of the conflicting casing normalizations.");
        d.HelpLinkUri.Should().Be("https://github.com/ericksonlopezf/dotnet-domain-primitives/blob/main/docs/rules/dp0005.md");
    }

    [Fact]
    public void DP0006_InvalidConstraintBounds_HasExpectedMetadata()
    {
        var d = DiagnosticDescriptors.DP0006_InvalidConstraintBounds;
        d.Id.Should().Be("DP0006");
        d.Title.ToString().Should().Be("Invalid constraint bounds");
        d.MessageFormat.ToString().Should().Be("Constraint bounds are invalid: Min ({0}) cannot be greater than Max ({1})");
        d.Category.Should().Be(DiagnosticCategories.Correctness);
        d.DefaultSeverity.Should().Be(DiagnosticSeverity.Error);
        d.IsEnabledByDefault.Should().BeTrue();
        d.Description.ToString().Should().Be("Fix the minimum and maximum values in the constraint attribute.");
        d.HelpLinkUri.Should().Be("https://github.com/ericksonlopezf/dotnet-domain-primitives/blob/main/docs/rules/dp0006.md");
    }

    [Fact]
    public void DP0007_AvoidDefaultConstructor_HasExpectedMetadata()
    {
        var d = DiagnosticDescriptors.DP0007_AvoidDefaultConstructor;
        d.Id.Should().Be("DP0007");
        d.Title.ToString().Should().Be("Avoid uninitialized domain primitive");
        d.MessageFormat.ToString().Should().Be("Avoid using the default constructor for domain primitive '{0}'");
        d.Category.Should().Be(DiagnosticCategories.Design);
        d.DefaultSeverity.Should().Be(DiagnosticSeverity.Warning);
        d.IsEnabledByDefault.Should().BeTrue();
        d.Description.ToString().Should().Be("Domain primitives should be instantiated via their Create() factory to ensure validation rules are executed.");
        d.HelpLinkUri.Should().Be("https://github.com/ericksonlopezf/dotnet-domain-primitives/blob/main/docs/rules/dp0007.md");
    }

    [Fact]
    public void DP0008_ValueObjectRequiresInit_HasExpectedMetadata()
    {
        var d = DiagnosticDescriptors.DP0008_ValueObjectRequiresInit;
        d.Id.Should().Be("DP0008");
        d.Title.ToString().Should().Be("Value object properties must use 'init'");
        d.MessageFormat.ToString().Should().Be("Property '{0}' on ValueObject '{1}' must have an 'init' accessor");
        d.Category.Should().Be(DiagnosticCategories.Correctness);
        d.DefaultSeverity.Should().Be(DiagnosticSeverity.Error);
        d.IsEnabledByDefault.Should().BeTrue();
        d.Description.ToString().Should().Be("All properties of a [ValueObject] must use 'get; init;' (and 'required' if C# 11+) to guarantee immutability.");
        d.HelpLinkUri.Should().Be("https://github.com/ericksonlopezf/dotnet-domain-primitives/blob/main/docs/rules/dp0008.md");
    }

    [Fact]
    public void DP0009_MissingValidation_HasExpectedMetadata()
    {
        var d = DiagnosticDescriptors.DP0009_MissingValidation;
        d.Id.Should().Be("DP0009");
        d.Title.ToString().Should().Be("Missing validation");
        d.MessageFormat.ToString().Should().Be("Domain primitive '{0}' lacks validation rules");
        d.Category.Should().Be(DiagnosticCategories.Design);
        d.DefaultSeverity.Should().Be(DiagnosticSeverity.Warning);
        d.IsEnabledByDefault.Should().BeTrue();
        d.Description.ToString().Should().Be("Domain primitives should enforce validation rules to guarantee domain invariants.");
        d.HelpLinkUri.Should().Be("https://github.com/ericksonlopezf/dotnet-domain-primitives/blob/main/docs/rules/dp0009.md");
    }

    [Fact]
    public void DP0010_StringComparedWithPrimitive_HasExpectedMetadata()
    {
        var d = DiagnosticDescriptors.DP0010_StringComparedWithPrimitive;
        d.Id.Should().Be("DP0010");
        d.Title.ToString().Should().Be("String compared directly with domain primitive");
        d.MessageFormat.ToString().Should().Be("Comparing a raw string with domain primitive '{0}' using '==' may produce unexpected results. Use '{0}.Create(str)' or parse the string first.");
        d.Category.Should().Be(DiagnosticCategories.Performance);
        d.DefaultSeverity.Should().Be(DiagnosticSeverity.Warning);
        d.IsEnabledByDefault.Should().BeTrue();
        d.Description.ToString().Should().Be("Domain primitives should not be compared directly to raw strings. Comparing a string to a domain primitive bypasses the type system and can mask bugs. Parse the string into the primitive type before comparison.");
        d.HelpLinkUri.Should().Be("https://github.com/ericksonlopezf/dotnet-domain-primitives/blob/main/docs/rules/dp0010.md");
    }

    [Fact]
    public void DP0011_StringAssignedFromPrimitive_HasExpectedMetadata()
    {
        var d = DiagnosticDescriptors.DP0011_StringAssignedFromPrimitive;
        d.Id.Should().Be("DP0011");
        d.Title.ToString().Should().Be("String assigned directly from domain primitive");
        d.MessageFormat.ToString().Should().Be("Assigning domain primitive '{0}' to a 'string' variable discards type safety. Access '.Value' explicitly or use an explicit cast.");
        d.Category.Should().Be(DiagnosticCategories.Performance);
        d.DefaultSeverity.Should().Be(DiagnosticSeverity.Warning);
        d.IsEnabledByDefault.Should().BeTrue();
        d.Description.ToString().Should().Be("Assigning a domain primitive directly to a 'string' variable defeats the purpose of the strong type. Access the '.Value' property explicitly to make the conversion intentional.");
        d.HelpLinkUri.Should().Be("https://github.com/ericksonlopezf/dotnet-domain-primitives/blob/main/docs/rules/dp0011.md");
    }

    [Fact]
    public void DP0012_PublicConstructorBypass_HasExpectedMetadata()
    {
        var d = DiagnosticDescriptors.DP0012_PublicConstructorBypass;
        d.Id.Should().Be("DP0012");
        d.Title.ToString().Should().Be("Public constructor bypasses domain primitive validation");
        d.MessageFormat.ToString().Should().Be("Domain primitive '{0}' declares a public constructor. This bypasses source-generated validation. Use the generated 'Create()' factory method pattern instead.");
        d.Category.Should().Be(DiagnosticCategories.Design);
        d.DefaultSeverity.Should().Be(DiagnosticSeverity.Warning);
        d.IsEnabledByDefault.Should().BeTrue();
        d.Description.ToString().Should().Be("Domain primitives rely on a controlled creation path (Create/TryCreate) to guarantee invariants. Declaring a public constructor allows creating invalid instances, bypassing all validation.");
        d.HelpLinkUri.Should().Be("https://github.com/ericksonlopezf/dotnet-domain-primitives/blob/main/docs/rules/dp0012.md");
    }

    [Fact]
    public void DP0013_PossibleDuplicatePrimitiveLogic_HasExpectedMetadata()
    {
        var d = DiagnosticDescriptors.DP0013_PossibleDuplicatePrimitiveLogic;
        d.Id.Should().Be("DP0013");
        d.Title.ToString().Should().Be("Possible duplicate domain primitive logic");
        d.MessageFormat.ToString().Should().Be("Domain primitives '{0}' and '{1}' appear to have identical attribute configuration. Consider consolidating them or renaming to clarify their distinct domain intent.");
        d.Category.Should().Be(DiagnosticCategories.Design);
        d.DefaultSeverity.Should().Be(DiagnosticSeverity.Info);
        d.IsEnabledByDefault.Should().BeTrue();
        d.Description.ToString().Should().Be("Two or more domain primitives in the same compilation share the same attribute type and configuration. This may indicate copy-paste duplication. Verify that each primitive models a distinct domain concept.");
        d.CustomTags.Should().Contain(WellKnownDiagnosticTags.CompilationEnd);
        d.HelpLinkUri.Should().Be("https://github.com/ericksonlopezf/dotnet-domain-primitives/blob/main/docs/rules/dp0013.md");
    }

    [Fact]
    public void DP0014_ApiSurfaceBudgetExceeded_HasExpectedMetadata()
    {
        var d = DiagnosticDescriptors.DP0014_ApiSurfaceBudgetExceeded;
        d.Id.Should().Be("DP0014");
        d.Title.ToString().Should().Be("API Surface Budget Exceeded");
        d.MessageFormat.ToString().Should().Be("Domain primitive '{0}' has {1} public members, which exceeds the recommended maximum of {2}. Consider simplifying the domain type to maintain single-responsibility.");
        d.Category.Should().Be(DiagnosticCategories.ApiReview);
        d.DefaultSeverity.Should().Be(DiagnosticSeverity.Warning);
        d.IsEnabledByDefault.Should().BeTrue();
        d.Description.ToString().Should().Be("Domain primitives should be lightweight and focused. Exceeding the API surface budget indicates the primitive might be taking on too many responsibilities.");
        d.HelpLinkUri.Should().Be("https://github.com/ericksonlopezf/dotnet-domain-primitives/blob/main/docs/rules/dp0014.md");
    }

    [Fact]
    public void DP0015_MissingXmlDocumentation_HasExpectedMetadata()
    {
        var d = DiagnosticDescriptors.DP0015_MissingXmlDocumentation;
        d.Id.Should().Be("DP0015");
        d.Title.ToString().Should().Be("Missing XML Documentation");
        d.MessageFormat.ToString().Should().Be("Public member '{0}' on domain primitive '{1}' is missing XML documentation");
        d.Category.Should().Be(DiagnosticCategories.ApiReview);
        d.DefaultSeverity.Should().Be(DiagnosticSeverity.Warning);
        d.IsEnabledByDefault.Should().BeTrue();
        d.Description.ToString().Should().Be("Public members on domain primitives must have XML documentation to ensure high-quality developer experience.");
        d.HelpLinkUri.Should().Be("https://github.com/ericksonlopezf/dotnet-domain-primitives/blob/main/docs/rules/dp0015.md");
    }

    [Fact]
    public void DP0016_InvalidFactoryMethodName_HasExpectedMetadata()
    {
        var d = DiagnosticDescriptors.DP0016_InvalidFactoryMethodName;
        d.Id.Should().Be("DP0016");
        d.Title.ToString().Should().Be("Invalid Factory Method Name");
        d.MessageFormat.ToString().Should().Be("Custom factory method '{0}' on domain primitive '{1}' must be named 'Create', 'TryCreate', 'Parse', or 'TryParse'");
        d.Category.Should().Be(DiagnosticCategories.ApiReview);
        d.DefaultSeverity.Should().Be(DiagnosticSeverity.Warning);
        d.IsEnabledByDefault.Should().BeTrue();
        d.Description.ToString().Should().Be("Custom factory methods on domain primitives must follow the standard naming convention to maintain consistency.");
        d.HelpLinkUri.Should().Be("https://github.com/ericksonlopezf/dotnet-domain-primitives/blob/main/docs/rules/dp0016.md");
    }

    [Fact]
    public void DP0017_InvalidExceptionType_HasExpectedMetadata()
    {
        var d = DiagnosticDescriptors.DP0017_InvalidExceptionType;
        d.Id.Should().Be("DP0017");
        d.Title.ToString().Should().Be("Invalid DomainPrimitivesDefaults ExceptionType");
        d.MessageFormat.ToString().Should().Be("Exception type '{0}' specified in [DomainPrimitivesDefaults] must inherit from Exception and declare a public constructor accepting a string message parameter");
        d.Category.Should().Be(DiagnosticCategories.Correctness);
        d.DefaultSeverity.Should().Be(DiagnosticSeverity.Error);
        d.IsEnabledByDefault.Should().BeTrue();
        d.Description.ToString().Should().Be("Custom validation exception types specified in [assembly: DomainPrimitivesDefaults] must derive from Exception and have a constructor taking a single string argument.");
        d.CustomTags.Should().Contain(WellKnownDiagnosticTags.CompilationEnd);
        d.HelpLinkUri.Should().Be("https://github.com/ericksonlopezf/dotnet-domain-primitives/blob/main/docs/rules/dp0017.md");
    }
}
