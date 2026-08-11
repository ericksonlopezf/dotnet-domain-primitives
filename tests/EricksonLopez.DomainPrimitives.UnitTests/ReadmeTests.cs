using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.DomainPrimitives;
using Xunit;

namespace EricksonLopez.DomainPrimitives.UnitTests;

[StringPrimitive]
[Trim, UpperCase, Length(2, 2)]
public readonly partial record struct CountryIsoCode;

public class ReadmeTests
{
    [Fact]
    public void ReadmeSnippet_CompilesAndRunsCorrectly()
    {
        // 2. Use it! (Validates at compile time and runtime)
        var code = CountryIsoCode.Create("  us  "); // Value is "US"
        
        Assert.Equal("US", code.Value);

        // 3. Use TryCreate to avoid exceptions on invalid input
        if (CountryIsoCode.TryCreate("invalid", out var validCode, out var error))
        {
            Assert.Fail("Should not be valid");
        }
        else
        {
            // Error message includes type name + rule: "CountryIsoCode must be at most 2 character(s). Got N."
            Assert.Equal("LENGTH", error.Code);
            Assert.Contains("CountryIsoCode", error.Message, StringComparison.Ordinal);
            Assert.Contains("must be at most 2", error.Message, StringComparison.Ordinal);
        }

        // 4. Parse directly from spans (Zero-Allocation)
        if (CountryIsoCode.TryParse("us".AsSpan(), null, out var parsedCode))
        {
            Assert.Equal("US", parsedCode.Value);
        }
    }
}
