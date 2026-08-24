// Copyright © Erickson Lopez. MIT License.
using System;
using System.Collections.Generic;
using System.Reflection;
using AwesomeAssertions;
using NetArchTest.Rules;
using Xunit;

namespace EricksonLopez.DomainPrimitives.ArchitectureTests;

public class DependencyRulesTests
{
    private static readonly string[] SatelliteNamespaces =
    [
        "EricksonLopez.DomainPrimitives.EFCore",
        "EricksonLopez.DomainPrimitives.Dapper",
        "EricksonLopez.DomainPrimitives.AspNetCore",
        "EricksonLopez.DomainPrimitives.OpenApi"
    ];

    [Fact]
    public void Abstractions_Should_Not_Depend_On_Implementations_Or_Satellites()
    {
        // Arrange
        var abstractionsAssembly = typeof(EricksonLopez.DomainPrimitives.DatePrimitiveKind).Assembly;

        // Act & Assert
        foreach (var satellite in SatelliteNamespaces)
        {
            var result = Types.InAssembly(abstractionsAssembly)
                .ShouldNot()
                .HaveDependencyOn(satellite)
                .GetResult();

            result.IsSuccessful.Should().BeTrue($"Abstractions must not depend on {satellite}");
        }
    }

    [Fact]
    public void Core_Should_Not_Depend_On_Infrastructure_Or_Satellites()
    {
        // Arrange
        var coreAssembly = typeof(EricksonLopez.DomainPrimitives.RegexAttribute).Assembly;

        // Act & Assert
        foreach (var satellite in SatelliteNamespaces)
        {
            var result = Types.InAssembly(coreAssembly)
                .ShouldNot()
                .HaveDependencyOn(satellite)
                .GetResult();

            result.IsSuccessful.Should().BeTrue($"Primitives Core must not depend on {satellite}");
        }
    }

    [Fact]
    public void Dapper_Should_Not_Depend_On_Other_Satellites()
    {
        var assembly = typeof(EricksonLopez.DomainPrimitives.Dapper.DomainPrimitiveTypeHandler<,>).Assembly;
        var forbidden = new[]
        {
            "EricksonLopez.DomainPrimitives.EFCore",
            "EricksonLopez.DomainPrimitives.AspNetCore",
            "EricksonLopez.DomainPrimitives.OpenApi"
        };

        foreach (var dep in forbidden)
        {
            var result = Types.InAssembly(assembly).ShouldNot().HaveDependencyOn(dep).GetResult();
            result.IsSuccessful.Should().BeTrue($"Dapper must not depend on {dep}");
        }
    }

    [Fact]
    public void AspNetCore_Should_Not_Depend_On_Other_Satellites()
    {
        var assembly = typeof(EricksonLopez.DomainPrimitives.AspNetCore.DomainPrimitiveModelBinder<>).Assembly;
        var forbidden = new[]
        {
            "EricksonLopez.DomainPrimitives.EFCore",
            "EricksonLopez.DomainPrimitives.Dapper",
            "EricksonLopez.DomainPrimitives.OpenApi"
        };

        foreach (var dep in forbidden)
        {
            var result = Types.InAssembly(assembly).ShouldNot().HaveDependencyOn(dep).GetResult();
            result.IsSuccessful.Should().BeTrue($"AspNetCore must not depend on {dep}");
        }
    }

    [Fact]
    public void Testing_SDK_Should_Not_Depend_On_Infrastructure_Satellites()
    {
        var assembly = typeof(EricksonLopez.DomainPrimitives.Testing.DomainPrimitiveFakeFactory).Assembly;

        foreach (var satellite in SatelliteNamespaces)
        {
            var result = Types.InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOn(satellite)
                .GetResult();

            result.IsSuccessful.Should().BeTrue($"Testing SDK must not depend on {satellite}");
        }
    }
}




