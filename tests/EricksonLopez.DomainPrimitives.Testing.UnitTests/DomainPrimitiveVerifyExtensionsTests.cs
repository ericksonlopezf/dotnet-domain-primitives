// Copyright © Erickson Lopez. MIT License.
using System;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using EricksonLopez.DomainPrimitives;
using EricksonLopez.DomainPrimitives.Testing;
using VerifyXunit;
using Xunit;

namespace EricksonLopez.DomainPrimitives.Testing.UnitTests;

public class DomainPrimitiveVerifyExtensionsTests
{
    private readonly record struct VerifyTestPrimitive(string Value) : IDomainPrimitive<VerifyTestPrimitive, string>
    {
        public static string PrimitiveName => "VerifyTestPrimitive";
        public bool IsDefault => false;
        public static VerifyTestPrimitive Create(string value) => new(value);
        public static bool TryCreate(string value, out VerifyTestPrimitive result, out global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError validationError) { result = new VerifyTestPrimitive(value); validationError = global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError.None; return true; }
    }

    public DomainPrimitiveVerifyExtensionsTests()
    {
        DomainPrimitiveVerifyExtensions.Initialize();
    }

    [Fact]
    public Task Verify_ShouldSerializeAsInnerValue()
    {
        var primitive = VerifyTestPrimitive.Create("HelloVerify");
        // Verify will serialize it. The converter should output just the string.
        return Verifier.Verify(primitive);
    }

    [Fact]
    public void Initialize_CanBeCalledMultipleTimes()
    {
        // Act
        var action = () => DomainPrimitiveVerifyExtensions.Initialize();
        action();
        
        // Assert
        action.Should().NotThrow();
    }
}







