// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using Xunit;
using EricksonLopez.DomainPrimitives.Generators;

namespace EricksonLopez.DomainPrimitives.Generators.Tests
{
    public class DiagnosticDescriptorsTests
    {
        [Fact]
        public void Descriptors_AreNotNull()
        {
            DiagnosticDescriptors.TypeMustBePartial.Should().NotBeNull();
            DiagnosticDescriptors.TypeMustBeReadonlyRecordStruct.Should().NotBeNull();
            DiagnosticDescriptors.UnsupportedBackingType.Should().NotBeNull();
            DiagnosticDescriptors.ConflictingAttributes.Should().NotBeNull();
            DiagnosticDescriptors.InvalidAttributeParameter.Should().NotBeNull();
        }

        [Theory]
        [InlineData("DP1001", "Domain primitive type must be partial", "Type '{0}' is decorated with a domain primitive attribute but is not declared as 'partial'", "https://github.com/ericksonlopezf/dotnet-domain-primitives/blob/main/docs/rules/dp1001.md")]
        [InlineData("DP1002", "Domain primitive type must be a readonly record struct", "Type '{0}' must be declared as 'readonly partial record struct'", "https://github.com/ericksonlopezf/dotnet-domain-primitives/blob/main/docs/rules/dp1002.md")]
        [InlineData("DP1003", "Unsupported backing type", "Backing type '{0}' is not supported for '{1}'", "https://github.com/ericksonlopezf/dotnet-domain-primitives/blob/main/docs/rules/dp1003.md")]
        [InlineData("DP1004", "Conflicting attributes", "Attributes '{0}' and '{1}' cannot be combined on type '{2}'", "https://github.com/ericksonlopezf/dotnet-domain-primitives/blob/main/docs/rules/dp1004.md")]
        [InlineData("DP1005", "Invalid attribute parameter", "{0}", "https://github.com/ericksonlopezf/dotnet-domain-primitives/blob/main/docs/rules/dp1005.md")]
        public void Descriptors_HaveExactProperties(string id, string title, string messageFormat, string helpLink)
        {
            var desc = id switch
            {
                "DP1001" => DiagnosticDescriptors.TypeMustBePartial,
                "DP1002" => DiagnosticDescriptors.TypeMustBeReadonlyRecordStruct,
                "DP1003" => DiagnosticDescriptors.UnsupportedBackingType,
                "DP1004" => DiagnosticDescriptors.ConflictingAttributes,
                "DP1005" => DiagnosticDescriptors.InvalidAttributeParameter,
                _ => throw new ArgumentException("Unknown id", nameof(id))
            };

            desc.Id.Should().Be(id);
            desc.Title.ToString().Should().Be(title);
            desc.MessageFormat.ToString().Should().Be(messageFormat);
            desc.Category.Should().Be("EricksonLopez.DomainPrimitives");
            desc.DefaultSeverity.Should().Be(Microsoft.CodeAnalysis.DiagnosticSeverity.Error);
            desc.IsEnabledByDefault.Should().BeTrue();
            desc.HelpLinkUri.Should().Be(helpLink);
        }
    }
}


