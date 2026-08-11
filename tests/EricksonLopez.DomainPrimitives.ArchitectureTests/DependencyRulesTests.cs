using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NetArchTest.Rules;
using Xunit;

namespace EricksonLopez.DomainPrimitives.ArchitectureTests;

public class DependencyRulesTests
{
    [Fact]
    public void Abstractions_Should_Not_Depend_On_Implementations()
    {
        // Arrange
        var abstractionsAssembly = typeof(EricksonLopez.DomainPrimitives.DatePrimitiveKind).Assembly;

        // Act
        var result = Types.InAssembly(abstractionsAssembly)
            .ShouldNot()
            .HaveDependencyOn("EricksonLopez.DomainPrimitives.EFCore")
            .And()
            .HaveDependencyOn("EricksonLopez.DomainPrimitives.AspNetCore")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue("Abstractions are agnostic to concrete infrastructure implementations");
    }

    [Fact]
    public void Core_Should_Not_Depend_On_Infrastructure()
    {
        // Arrange
        var coreAssembly = typeof(EricksonLopez.DomainPrimitives.RegexAttribute).Assembly;

        // Act
        var result = Types.InAssembly(coreAssembly)
            .ShouldNot()
            .HaveDependencyOn("EricksonLopez.DomainPrimitives.EFCore")
            .And()
            .HaveDependencyOn("EricksonLopez.DomainPrimitives.Dapper")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue("Primitives Core must not depend on ORMs");
    }
}
