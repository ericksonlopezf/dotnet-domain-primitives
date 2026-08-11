using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace EricksonLopez.DomainPrimitives.Analyzers;

internal static class DiagnosticCategories
{
    public const string Correctness = "Correctness";
    public const string Design = "Design";
    public const string Performance = "Performance";
    public const string Migration = "Migration";
    public const string ApiReview = "ApiReview";
}

internal static class DiagnosticDescriptors
{
    // ── Diagnostic ID Range Reservation (TD-005) ───────────────────────────────
    // DP0001–DP0016 : Correctness, Design, Performance, ApiReview rules (active)
    // DP0017–DP0099 : Reserved for future user-facing analyzer rules
    // DP1001–DP1999 : Infrastructure / generator pipeline diagnostics (see AnalyzerReleases.Shipped.md)
    // DP2000+       : Reserved for future expansion
    // ──────────────────────────────────────────────────────────────────────────

    public static readonly DiagnosticDescriptor DP0001_MustBePartial = new(
        id: "DP0001",
        title: "Domain primitive must be partial",
        messageFormat: "Type '{0}' is decorated with a domain primitive attribute but is not declared as 'partial'",
        category: DiagnosticCategories.Correctness,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Domain primitive types must be declared as 'partial' so the source generator can add the required implementation.",
        helpLinkUri: "https://github.com/ericksonlopez/dotnet-domain-primitives/blob/main/docs/rules/DP0001.md");

    public static readonly DiagnosticDescriptor DP0002_MustBeReadonly = new(
        id: "DP0002",
        title: "Domain primitive must be readonly",
        messageFormat: "Type '{0}' is decorated with a domain primitive attribute but is not declared as 'readonly'",
        category: DiagnosticCategories.Correctness,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Domain primitive types must be immutable. Mark the struct as 'readonly'.",
        helpLinkUri: "https://github.com/ericksonlopez/dotnet-domain-primitives/blob/main/docs/rules/DP0002.md");

    public static readonly DiagnosticDescriptor DP0003_MustBeRecordStruct = new(
        id: "DP0003",
        title: "Domain primitive must be a record struct",
        messageFormat: "Type '{0}' must be declared as a 'record struct'",
        category: DiagnosticCategories.Correctness,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Domain primitives rely on zero-boxing, structural equality provided natively by 'record struct' in C# 10+.",
        helpLinkUri: "https://github.com/ericksonlopez/dotnet-domain-primitives/blob/main/docs/rules/DP0003.md");

    public static readonly DiagnosticDescriptor DP0004_InvalidRegex = new(
        id: "DP0004",
        title: "Invalid Regex Pattern",
        messageFormat: "The regex pattern '{0}' is invalid: {1}",
        category: DiagnosticCategories.Correctness,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Ensure the regular expression is syntactically valid.",
        helpLinkUri: "https://github.com/ericksonlopez/dotnet-domain-primitives/blob/main/docs/rules/DP0004.md");

    public static readonly DiagnosticDescriptor DP0005_ConflictingNormalization = new(
        id: "DP0005",
        title: "Conflicting normalization attributes",
        messageFormat: "Type '{0}' cannot have both [LowerCase] and [UpperCase] attributes",
        category: DiagnosticCategories.Correctness,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Remove one of the conflicting casing normalizations.",
        helpLinkUri: "https://github.com/ericksonlopez/dotnet-domain-primitives/blob/main/docs/rules/DP0005.md");

    public static readonly DiagnosticDescriptor DP0006_InvalidConstraintBounds = new(
        id: "DP0006",
        title: "Invalid constraint bounds",
        messageFormat: "Constraint bounds are invalid: Min ({0}) cannot be greater than Max ({1})",
        category: DiagnosticCategories.Correctness,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Fix the minimum and maximum values in the constraint attribute.",
        helpLinkUri: "https://github.com/ericksonlopez/dotnet-domain-primitives/blob/main/docs/rules/DP0006.md");

    public static readonly DiagnosticDescriptor DP0007_AvoidDefaultConstructor = new(
        id: "DP0007",
        title: "Avoid uninitialized domain primitive",
        messageFormat: "Avoid using the default constructor for domain primitive '{0}'",
        category: DiagnosticCategories.Design,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Domain primitives should be instantiated via their Create() factory to ensure validation rules are executed.",
        helpLinkUri: "https://github.com/ericksonlopez/dotnet-domain-primitives/blob/main/docs/rules/DP0007.md");

    public static readonly DiagnosticDescriptor DP0008_ValueObjectRequiresInit = new(
        id: "DP0008",
        title: "Value object properties must use 'init'",
        messageFormat: "Property '{0}' on ValueObject '{1}' must have an 'init' accessor",
        category: DiagnosticCategories.Correctness,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "All properties of a [ValueObject] must use 'get; init;' (and 'required' if C# 11+) to guarantee immutability.",
        helpLinkUri: "https://github.com/ericksonlopez/dotnet-domain-primitives/blob/main/docs/rules/DP0008.md");

    public static readonly DiagnosticDescriptor DP0009_MissingValidation = new(
        id: "DP0009",
        title: "Missing validation",
        messageFormat: "Domain primitive '{0}' lacks validation rules",
        category: DiagnosticCategories.Design,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Domain primitives should enforce validation rules to guarantee domain invariants.",
        helpLinkUri: "https://github.com/ericksonlopez/dotnet-domain-primitives/blob/main/docs/rules/DP0009.md");

    public static readonly DiagnosticDescriptor DP0010_StringComparedWithPrimitive = new(
        id: "DP0010",
        title: "String compared directly with domain primitive",
        messageFormat: "Comparing a raw string with domain primitive '{0}' using '==' may produce unexpected results. Use '{0}.Create(str)' or parse the string first.",
        category: DiagnosticCategories.Performance,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Domain primitives should not be compared directly to raw strings. Comparing a string to a domain primitive bypasses the type system and can mask bugs. Parse the string into the primitive type before comparison.",
        helpLinkUri: "https://github.com/ericksonlopez/dotnet-domain-primitives/blob/main/docs/rules/DP0010.md");

    public static readonly DiagnosticDescriptor DP0011_StringAssignedFromPrimitive = new(
        id: "DP0011",
        title: "String assigned directly from domain primitive",
        messageFormat: "Assigning domain primitive '{0}' to a 'string' variable discards type safety. Access '.Value' explicitly or use an explicit cast.",
        category: DiagnosticCategories.Performance,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Assigning a domain primitive directly to a 'string' variable defeats the purpose of the strong type. Access the '.Value' property explicitly to make the conversion intentional.",
        helpLinkUri: "https://github.com/ericksonlopez/dotnet-domain-primitives/blob/main/docs/rules/DP0011.md");

    public static readonly DiagnosticDescriptor DP0012_PublicConstructorBypass = new(
        id: "DP0012",
        title: "Public constructor bypasses domain primitive validation",
        messageFormat: "Domain primitive '{0}' declares a public constructor. This bypasses source-generated validation. Use the generated 'Create()' factory method pattern instead.",
        category: DiagnosticCategories.Design,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Domain primitives rely on a controlled creation path (Create/TryCreate) to guarantee invariants. Declaring a public constructor allows creating invalid instances, bypassing all validation.",
        helpLinkUri: "https://github.com/ericksonlopez/dotnet-domain-primitives/blob/main/docs/rules/DP0012.md");

    public static readonly DiagnosticDescriptor DP0013_PossibleDuplicatePrimitiveLogic = new(
        id: "DP0013",
        title: "Possible duplicate domain primitive logic",
        messageFormat: "Domain primitives '{0}' and '{1}' appear to have identical attribute configuration. Consider consolidating them or renaming to clarify their distinct domain intent.",
        category: DiagnosticCategories.Design,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "Two or more domain primitives in the same compilation share the same attribute type and configuration. This may indicate copy-paste duplication. Verify that each primitive models a distinct domain concept.",
        customTags: new[] { WellKnownDiagnosticTags.CompilationEnd },
        helpLinkUri: "https://github.com/ericksonlopez/dotnet-domain-primitives/blob/main/docs/rules/DP0013.md");

    public static readonly DiagnosticDescriptor DP0014_ApiSurfaceBudgetExceeded = new(
        id: "DP0014",
        title: "API Surface Budget Exceeded",
        messageFormat: "Domain primitive '{0}' has {1} public members, which exceeds the recommended maximum of {2}. Consider simplifying the domain type to maintain single-responsibility.",
        category: DiagnosticCategories.ApiReview,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Domain primitives should be lightweight and focused. Exceeding the API surface budget indicates the primitive might be taking on too many responsibilities.",
        helpLinkUri: "https://github.com/ericksonlopez/dotnet-domain-primitives/blob/main/docs/rules/DP0014.md");

    public static readonly DiagnosticDescriptor DP0015_MissingXmlDocumentation = new(
        id: "DP0015",
        title: "Missing XML Documentation",
        messageFormat: "Public member '{0}' on domain primitive '{1}' is missing XML documentation",
        category: DiagnosticCategories.ApiReview,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Public members on domain primitives must have XML documentation to ensure high-quality developer experience.",
        helpLinkUri: "https://github.com/ericksonlopez/dotnet-domain-primitives/blob/main/docs/rules/DP0015.md");

    public static readonly DiagnosticDescriptor DP0016_InvalidFactoryMethodName = new(
        id: "DP0016",
        title: "Invalid Factory Method Name",
        messageFormat: "Custom factory method '{0}' on domain primitive '{1}' must be named 'Create', 'TryCreate', 'Parse', or 'TryParse'",
        category: DiagnosticCategories.ApiReview,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Custom factory methods on domain primitives must follow the standard naming convention to maintain consistency.",
        helpLinkUri: "https://github.com/ericksonlopez/dotnet-domain-primitives/blob/main/docs/rules/DP0016.md");
}
