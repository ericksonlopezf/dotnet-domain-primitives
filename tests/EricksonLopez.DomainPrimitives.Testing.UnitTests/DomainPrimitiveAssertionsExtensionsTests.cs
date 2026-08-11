using System;
using EricksonLopez.DomainPrimitives;
using EricksonLopez.DomainPrimitives.Testing;
using FluentAssertions;
using Xunit;

namespace EricksonLopez.DomainPrimitives.Testing.UnitTests;

public class DomainPrimitiveAssertionsExtensionsTests
{
    private readonly record struct TestPrimitive(int Value) : IDomainPrimitive<TestPrimitive, int>
    {
        public static string PrimitiveName => "TestPrimitive";
        public bool IsDefault => false;

        public static TestPrimitive Create(int value)
        {
            if (value < 0) throw new DomainPrimitiveValidationException(new EricksonLopez.DomainPrimitives.Validation.PrimitiveError("RANGE", "Must be positive"));
            return new TestPrimitive(value);
        }

        public static bool TryCreate(int value, out TestPrimitive result, out EricksonLopez.DomainPrimitives.Validation.PrimitiveError validationError)
        {
            if (value < 0)
            {
                result = default;
                validationError = new EricksonLopez.DomainPrimitives.Validation.PrimitiveError("RANGE", "Must be positive");
                return false;
            }
            result = new TestPrimitive(value);
            validationError = EricksonLopez.DomainPrimitives.Validation.PrimitiveError.None;
            return true;
        }
    }

    private sealed class NotAPrimitive { }

    [Fact]
    public void HavePrimitiveValue_WhenValueMatches_ShouldNotThrow()
    {
        var primitive = TestPrimitive.Create(42);
        primitive.Should().HavePrimitiveValue<TestPrimitive, int>(42);
    }

    [Fact]
    public void HavePrimitiveValue_WhenValueDiffers_ShouldThrow()
    {
        var primitive = TestPrimitive.Create(42);
        Action act = () => primitive.Should().HavePrimitiveValue<TestPrimitive, int>(10);
        act.Should().Throw<Exception>().WithMessage("*should hold value*");
    }

    [Fact]
    public void HavePrimitiveValue_WhenSubjectIsNotPrimitive_ShouldThrow()
    {
        object notPrimitive = new NotAPrimitive();
        Action act = () => notPrimitive.Should().HavePrimitiveValue<TestPrimitive, int>(42);
        act.Should().Throw<Exception>().WithMessage("*Expected*to be*TestPrimitive*but found*NotAPrimitive*");
    }



    [Fact]
    public void ThrowDomainPrimitiveException_WhenThrows_ShouldNotThrow()
    {
        Action act = () => TestPrimitive.Create(-1);
        act.Should().ThrowDomainPrimitiveException();
    }

    [Fact]
    public void ThrowDomainPrimitiveException_WhenNoException_ShouldThrow()
    {
        Action act = () => TestPrimitive.Create(42);
        Action assertAct = () => act.Should().ThrowDomainPrimitiveException();
        assertAct.Should().Throw<Exception>().WithMessage("*Expected a <EricksonLopez.DomainPrimitives.DomainPrimitiveValidationException> to be thrown*");
    }

    [Fact]
    public void ThrowDomainPrimitiveExceptionWithPrimitiveErrorCode_WhenCodeMatches_ShouldNotThrow()
    {
        Action act = () => TestPrimitive.Create(-1);
        act.Should().ThrowDomainPrimitiveExceptionWithPrimitiveErrorCode("RANGE");
    }

    [Fact]
    public void ThrowDomainPrimitiveExceptionWithPrimitiveErrorCode_WhenNoException_ShouldThrow()
    {
        Action act = () => TestPrimitive.Create(42);
        Action assertAct = () => act.Should().ThrowDomainPrimitiveExceptionWithPrimitiveErrorCode("RANGE");
        assertAct.Should().Throw<Exception>().WithMessage("*Expected a <EricksonLopez.DomainPrimitives.DomainPrimitiveValidationException> to be thrown*");
    }





    [Fact]
    public void ShouldFailCreationWith_WhenFailsWithCode_ShouldNotThrow()
    {
        DomainPrimitiveAssertionsExtensions.ShouldFailCreationWith<TestPrimitive, int>(-1, "Must be positive");
    }

    [Fact]
    public void ShouldFailCreationWith_WhenSucceeds_ShouldThrow()
    {
        Action act = () => DomainPrimitiveAssertionsExtensions.ShouldFailCreationWith<TestPrimitive, int>(42, "Must be positive");
        act.Should().Throw<Exception>().WithMessage("*Expected a <EricksonLopez.DomainPrimitives.DomainPrimitiveValidationException> to be thrown*");
    }

    [Fact]
    public void ShouldFailCreationWith_WhenWrongErrorCode_ShouldThrow()
    {
        Action act = () => DomainPrimitiveAssertionsExtensions.ShouldFailCreationWith<TestPrimitive, int>(-1, "WrongCode");
        act.Should().Throw<Exception>().WithMessage("*creating TestPrimitive from '-1' should fail with error code 'WrongCode'*");
    }

    [Fact]
    public void ShouldSucceedCreation_WhenSucceeds_ShouldReturnPrimitive()
    {
        var primitive = DomainPrimitiveAssertionsExtensions.ShouldSucceedCreation<TestPrimitive, int>(42);
        primitive.Value.Should().Be(42);
    }

    [Fact]
    public void ShouldSucceedCreation_WhenFails_ShouldThrow()
    {
        Action act = () => DomainPrimitiveAssertionsExtensions.ShouldSucceedCreation<TestPrimitive, int>(-1);
        act.Should().Throw<Exception>().WithMessage("*should succeed*");
    }

    [Fact]
    public void ShouldBeValidPrimitive_WhenValid_ShouldNotThrow()
    {
        ((object)42).Should().ShouldBeValidPrimitive<TestPrimitive, int>();
    }

    [Fact]
    public void ShouldBeValidPrimitive_WhenInvalid_ShouldThrow()
    {
        Action act = () => ((object)-1).Should().ShouldBeValidPrimitive<TestPrimitive, int>();
        act.Should().Throw<Exception>().WithMessage("*should be a valid*");
    }

    [Fact]
    public void ShouldHaveValidationPrimitiveError_WhenInvalid_ShouldNotThrow()
    {
        ((object)-1).Should().ShouldHaveValidationPrimitiveError<TestPrimitive, int>("RANGE");
    }

    [Fact]
    public void ShouldHaveValidationPrimitiveError_WhenValid_ShouldThrow()
    {
        Action act = () => ((object)42).Should().ShouldHaveValidationPrimitiveError<TestPrimitive, int>("RANGE");
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void ShouldHaveValidationPrimitiveError_WhenWrongErrorCode_ShouldThrow()
    {
        Action act = () => ((object)-1).Should().ShouldHaveValidationPrimitiveError<TestPrimitive, int>("WrongCode");
        act.Should().Throw<Exception>().WithMessage("*the validation error*");
    }
}




