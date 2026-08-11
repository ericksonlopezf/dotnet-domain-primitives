using System;
using EricksonLopez.DomainPrimitives;
using EricksonLopez.DomainPrimitives.Advanced;
using FluentAssertions;
using Xunit;
using EricksonLopez.DomainPrimitives.Validation;

namespace EricksonLopez.DomainPrimitives.Abstractions.UnitTests;

public class PrimitiveBuilderTests
{
    private readonly struct BuilderTestPrimitive : IDomainPrimitive<BuilderTestPrimitive, string>
    {
        private readonly string _value;
        private readonly bool _isInitialized;
        
        public BuilderTestPrimitive(string value)
        {
            _value = value;
            _isInitialized = true;
        }

        public string Value => _value;
        
        public static string PrimitiveName => "BuilderTestPrimitive";
        
        public bool IsDefault => !_isInitialized;

        public static BuilderTestPrimitive Create(string value)
        {
            if (value == "invalid") throw new DomainPrimitiveValidationException(new PrimitiveError("Invalid", "Value cannot be invalid."));
            return new BuilderTestPrimitive(value);
        }

        public static bool TryCreate(string value, out BuilderTestPrimitive result, out PrimitiveError error)
        {
            if (value == "invalid")
            {
                result = default;
                error = new PrimitiveError("Invalid", "Value cannot be invalid.");
                return false;
            }
            result = new BuilderTestPrimitive(value);
            error = PrimitiveError.None;
            return true;
        }

    }

#if NET7_0_OR_GREATER
    [Fact]
    public void Build_WithValidValueAndNoRules_ShouldReturnPrimitive()
    {
        var builder = PrimitiveBuilder<BuilderTestPrimitive, string>.For().WithValue("valid");
        var primitive = builder.BuildOrThrow();

        primitive.Value.Should().Be("valid");
    }

    [Fact]
    public void Build_WithValidValueAndPassingRule_ShouldReturnPrimitive()
    {
        var builder = PrimitiveBuilder<BuilderTestPrimitive, string>.For()
            .WithValue("valid")
            .Must(x => x.Length > 2, "TooShort", "Value must be greater than 2.");

        var primitive = builder.BuildOrThrow();

        primitive.Value.Should().Be("valid");
    }

    [Fact]
    public void Build_WithValidValueButFailingRule_ShouldThrow()
    {
        var builder = PrimitiveBuilder<BuilderTestPrimitive, string>.For()
            .WithValue("no")
            .Must(x => x.Length > 2, "TooShort", "Value must be greater than 2.");

        Action act = () => builder.BuildOrThrow();

        act.Should().Throw<DomainPrimitiveValidationException>()
           .WithMessage("[TooShort] Value must be greater than 2.*")
           .And.ParamName.Should().Be("value");
    }

    [Fact]
    public void Build_WithoutValue_ShouldThrow()
    {
        var builder = PrimitiveBuilder<BuilderTestPrimitive, string>.For();

        Action act = () => builder.BuildOrThrow();

        act.Should().Throw<DomainPrimitiveValidationException>()
           .WithMessage("[NULL_INPUT] Value was not provided to PrimitiveBuilder.*")
           .And.ParamName.Should().Be("value");
    }

    [Fact]
    public void BuildResult_WithValidValueButFailingRule_ShouldReturnFailureResult()
    {
        var builder = PrimitiveBuilder<BuilderTestPrimitive, string>.For()
            .WithValue("no")
            .Must(x => x.Length > 2, "TooShort", "Value must be greater than 2.");

        var success = builder.Build(out var result);

        success.Should().BeFalse();
        result.IsDefault.Should().BeTrue();
    }

    [Fact]
    public void BuildResult_WithValidValueAndPassingRule_ShouldReturnSuccess()
    {
        var builder = PrimitiveBuilder<BuilderTestPrimitive, string>.For()
            .WithValue("valid")
            .Must(x => x.Length > 2, "TooShort", "Value must be greater than 2.");

        var success = builder.Build(out var result);

        success.Should().BeTrue();
        result.Value.Should().Be("valid");
    }

    [Fact]
    public void BuildResult_WithoutValue_ShouldReturnFailureResult()
    {
        var builder = PrimitiveBuilder<BuilderTestPrimitive, string>.For();

        var success = builder.Build(out var result);

        success.Should().BeFalse();
        result.IsDefault.Should().BeTrue();
    }
#endif
}
