using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using EricksonLopez.DomainPrimitives.Mapster;
using EricksonLopez.DomainPrimitives.Tests.TestTypes;
using Mapster;
using Mapster.Models;
using Xunit;

namespace EricksonLopez.DomainPrimitives.Mapster.Tests;

public class MapsterExtensionsTests
{
    [Fact]
    public void AddDomainPrimitiveMapping_Registers_Bidirectional_Mapping()
    {
        var config = new TypeAdapterConfig();
        
        // Manual mapping fallback (using explicit delegates)
        // We add an offset to ensure we aren't relying on Mapster's auto-mapping
        config.AddDomainPrimitiveMapping<Score, int>(
            value => Score.Create(value - 10),
            primitive => primitive.Value - 10);

        // Test mapping: value -> primitive
        var mappedScore = 85.Adapt<Score>(config);
        
        // The factory delegate should have subtracted 10
        Assert.Equal(75, mappedScore.Value);

        // Test mapping: primitive -> value
        var score = Score.Create(85);
        var intValue = score.Adapt<int>(config);
        
        // The projection delegate should have subtracted 10
        Assert.Equal(75, intValue);
    }

    [Fact]
    public void AddDomainPrimitivesMapster_Scans_And_Registers_Config()
    {
        var config = new TypeAdapterConfig();

        // Should return the config for chaining
        var result = config.AddDomainPrimitivesMapster(Assembly.GetExecutingAssembly());

        Assert.Same(config, result);
        
        // Verify that the DummyRegister from this assembly was picked up
        Assert.True(config.RuleMap.ContainsKey(new TypeTuple(typeof(int), typeof(string))));
    }

    [Fact]
    public void AddDomainPrimitivesMapster_WithMarkerType_Scans_And_Registers_Config()
    {
        var config = new TypeAdapterConfig();

        // Should return the config for chaining
        var result = config.AddDomainPrimitivesMapster<MapsterExtensionsTests>();

        Assert.Same(config, result);
        
        // Verify that the DummyRegister from this assembly was picked up
        Assert.True(config.RuleMap.ContainsKey(new TypeTuple(typeof(int), typeof(string))));
    }
}

// Dummy register to ensure config.Scan actually processes IRegisters in the test assembly
public class DummyRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<int, string>();
    }
}
