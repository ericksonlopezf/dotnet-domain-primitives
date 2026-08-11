
using System;
using EricksonLopez.DomainPrimitives;
using EricksonLopez.DomainPrimitives.Testing;
using FluentAssertions;
using Xunit;

namespace EricksonLopez.DomainPrimitives.Testing.UnitTests;

public class DomainPrimitiveTestBuilderTests
{
    private readonly record struct ValidTestPrimitive : IDomainPrimitive<ValidTestPrimitive, int>
    {
        public int Value { get; }
        
        // Private constructor for unvalidated path
        private ValidTestPrimitive(int value) => Value = value;

        public static string PrimitiveName => "ValidTestPrimitive";
        public bool IsDefault => false;

        public static ValidTestPrimitive Create(int value)
        {
            if (value < 0) throw new DomainPrimitiveValidationException(new EricksonLopez.DomainPrimitives.Validation.PrimitiveError("ValidTestPrimitive", "Must be positive"));
            return new ValidTestPrimitive(value);
        }


        public static bool TryCreate(int value, out ValidTestPrimitive result, out EricksonLopez.DomainPrimitives.Validation.PrimitiveError validationError) { validationError = EricksonLopez.DomainPrimitives.Validation.PrimitiveError.None; if (value < 0) { result = default; return false; } result = new ValidTestPrimitive(value); return true; }
    }

    private readonly record struct NoPrivateConstructorPrimitive : IDomainPrimitive<NoPrivateConstructorPrimitive, int>
    {
        public int Value { get; }
        
        // Public constructor (should cause Unvalidated to fail)
        public NoPrivateConstructorPrimitive(int value) => Value = value;

        public static string PrimitiveName => "NoPrivateConstructorPrimitive";
        public bool IsDefault => false;

        public static NoPrivateConstructorPrimitive Create(int value) => new(value);

        public static bool TryCreate(int value, out NoPrivateConstructorPrimitive result, out EricksonLopez.DomainPrimitives.Validation.PrimitiveError validationError) { validationError = EricksonLopez.DomainPrimitives.Validation.PrimitiveError.None; result = new NoPrivateConstructorPrimitive(value); return true; }
    }

    [Fact]
    public void Create_WhenValid_ShouldReturnPrimitive()
    {
        var primitive = DomainPrimitiveTestBuilder.Create<ValidTestPrimitive, int>(42);
        primitive.Value.Should().Be(42);
    }

    [Fact]
    public void AssertCreationFails_WhenFails_ShouldReturnException()
    {
        var ex = DomainPrimitiveTestBuilder.AssertCreationFails<ValidTestPrimitive, int>(-1);
        ex.Should().NotBeNull();
        ex.Message.Should().Contain("Must be positive");
    }

    [Fact]
    public void AssertCreationFails_WhenSucceeds_ShouldThrowInvalidOperationException()
    {
        Action act = () => DomainPrimitiveTestBuilder.AssertCreationFails<ValidTestPrimitive, int>(42);
        act.Should().Throw<InvalidOperationException>().WithMessage("*Expected creation of*to fail*");
    }

    [Fact]
    public void CreateUnvalidated_WhenPrivateConstructorExists_ShouldCreate()
    {
        // -1 would normally fail validation in Create, but CreateUnvalidated bypasses it.
        var primitive = DomainPrimitiveTestBuilder.CreateUnvalidated<ValidTestPrimitive, int>(-1);
        primitive.Value.Should().Be(-1);
    }

    [Fact]
    public void CreateUnvalidated_WhenPrivateConstructorDoesNotExist_ShouldThrow()
    {
        Action act = () => DomainPrimitiveTestBuilder.CreateUnvalidated<NoPrivateConstructorPrimitive, int>(42);
        act.Should().Throw<InvalidOperationException>().WithMessage("*does not have a private constructor taking*Is it a valid domain primitive*");
    }
}




