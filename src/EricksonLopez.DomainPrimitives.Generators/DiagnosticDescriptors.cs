using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace EricksonLopez.DomainPrimitives.Generators;

/// <summary>
/// All diagnostic descriptors emitted by the DomainPrimitives source generators.
/// </summary>
internal static class DiagnosticDescriptors
{
    private const string Category = "EricksonLopez.DomainPrimitives";

    public static readonly DiagnosticDescriptor TypeMustBePartial = new(
        id: "DP1001",
        title: "Domain primitive type must be partial",
        messageFormat: "Type '{0}' is decorated with a domain primitive attribute but is not declared as 'partial'",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        helpLinkUri: "https://github.com/ericksonlopez/dotnet-domain-primitives/blob/main/docs/rules/DP1001.md");

    public static readonly DiagnosticDescriptor TypeMustBeReadonlyRecordStruct = new(
        id: "DP1002",
        title: "Domain primitive type must be a readonly record struct",
        messageFormat: "Type '{0}' must be declared as 'readonly partial record struct'",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        helpLinkUri: "https://github.com/ericksonlopez/dotnet-domain-primitives/blob/main/docs/rules/DP1002.md");

    public static readonly DiagnosticDescriptor UnsupportedBackingType = new(
        id: "DP1003",
        title: "Unsupported backing type",
        messageFormat: "Backing type '{0}' is not supported for '{1}'",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        helpLinkUri: "https://github.com/ericksonlopez/dotnet-domain-primitives/blob/main/docs/rules/DP1003.md");

    public static readonly DiagnosticDescriptor ConflictingAttributes = new(
        id: "DP1004",
        title: "Conflicting attributes",
        messageFormat: "Attributes '{0}' and '{1}' cannot be combined on type '{2}'",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        helpLinkUri: "https://github.com/ericksonlopez/dotnet-domain-primitives/blob/main/docs/rules/DP1004.md");

    public static readonly DiagnosticDescriptor InvalidAttributeParameter = new(
        id: "DP1005",
        title: "Invalid attribute parameter",
        messageFormat: "{0}",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        helpLinkUri: "https://github.com/ericksonlopez/dotnet-domain-primitives/blob/main/docs/rules/DP1005.md");
}

