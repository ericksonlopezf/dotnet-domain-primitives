using System;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using FluentAssertions.Primitives;
using FluentAssertions.Specialized;

namespace EricksonLopez.DomainPrimitives.Testing;

/// <summary>
/// Provides FluentAssertions extension methods for asserting on domain primitive creation results and instances.
/// </summary>
[ExcludeFromCodeCoverage]
public static class DomainPrimitiveAssertionsExtensions
{
    // ─── Value assertions (type-safe, no Reflection) ─────────────────────────

    /// <summary>
    /// Validates that the domain primitive holds the expected backing value via its <c>Value</c> property.
    /// </summary>
    /// <typeparam name="TPrimitive">The domain primitive type.</typeparam>
    /// <typeparam name="TValue">The backing value type.</typeparam>
    /// <param name="assertions">The FluentAssertions <see cref="ObjectAssertions"/> context.</param>
    /// <param name="expectedValue">The backing value the primitive is expected to hold.</param>
    /// <returns>An <see cref="AndConstraint{T}"/> for chaining further assertions.</returns>
    public static AndConstraint<ObjectAssertions> HavePrimitiveValue<TPrimitive, TValue>(
        this ObjectAssertions assertions, TValue expectedValue)
        where TPrimitive : struct, IDomainPrimitive<TPrimitive, TValue>
        where TValue : notnull
    {
        assertions.Subject.Should().BeOfType<TPrimitive>();
        var primitive = (TPrimitive)assertions.Subject;
        primitive.Value.Should().Be(expectedValue,
            $"the domain primitive {typeof(TPrimitive).Name} should hold value '{expectedValue}'");

        return new AndConstraint<ObjectAssertions>(assertions);
    }


    // ─── Exception assertions ─────────────────────────────────────────────────

    /// <summary>
    /// Validates that the action throws a <see cref="DomainPrimitiveValidationException"/>.
    /// </summary>
    /// <param name="assertions">The FluentAssertions <see cref="ActionAssertions"/> context.</param>
    /// <returns>An <see cref="ExceptionAssertions{T}"/> for inspecting the thrown exception.</returns>
    public static ExceptionAssertions<DomainPrimitiveValidationException> ThrowDomainPrimitiveException(
        this ActionAssertions assertions)
    {
        return assertions.Throw<DomainPrimitiveValidationException>();
    }

    /// <summary>
    /// Validates that the action throws a <see cref="DomainPrimitiveValidationException"/> containing a specific error code.
    /// </summary>
    /// <param name="assertions">The FluentAssertions <see cref="ActionAssertions"/> context.</param>
    /// <param name="errorCode">The error code that the exception message must contain.</param>
    /// <returns>An <see cref="ExceptionAssertions{T}"/> for chaining further exception assertions.</returns>
    public static ExceptionAssertions<DomainPrimitiveValidationException> ThrowDomainPrimitiveExceptionWithPrimitiveErrorCode(
        this ActionAssertions assertions, string errorCode)
    {
        return assertions.Throw<DomainPrimitiveValidationException>()
            .Where(e => e.Message.Contains(errorCode),
                   $"the exception message should contain the error code '{errorCode}'");
    }

    // ─── Creation helpers ─────────────────────────────────────────────────────

    /// <summary>
    /// Validates that creating a domain primitive from the given value fails with a specific error code.
    /// </summary>
    /// <typeparam name="TPrimitive">The domain primitive type.</typeparam>
    /// <typeparam name="TValue">The backing value type.</typeparam>
    /// <param name="value">The value expected to fail validation.</param>
    /// <param name="errorCode">The error code the thrown exception must contain.</param>
    public static void ShouldFailCreationWith<TPrimitive, TValue>(TValue value, string errorCode)
        where TPrimitive : struct, IDomainPrimitive<TPrimitive, TValue>
        where TValue : notnull
    {
        Action act = () => TPrimitive.Create(value);
        act.Should().Throw<DomainPrimitiveValidationException>()
            .WithMessage($"*{errorCode}*", $"creating {typeof(TPrimitive).Name} from '{value}' should fail with error code '{errorCode}'");
    }

    /// <summary>
    /// Validates that creating a domain primitive from the given value succeeds and returns the created primitive.
    /// </summary>
    /// <typeparam name="TPrimitive">The domain primitive type.</typeparam>
    /// <typeparam name="TValue">The backing value type.</typeparam>
    /// <param name="value">The value expected to pass validation.</param>
    /// <returns>The created domain primitive for further assertions.</returns>
    public static TPrimitive ShouldSucceedCreation<TPrimitive, TValue>(TValue value)
        where TPrimitive : struct, IDomainPrimitive<TPrimitive, TValue>
        where TValue : notnull
    {
        bool success = TPrimitive.TryCreate(value, out var result, out _);

        success.Should().BeTrue($"creating {typeof(TPrimitive).Name} from '{value}' should succeed");

        return result;
    }

    /// <summary>
    /// Validates that the subject value can be successfully converted to the specified domain primitive type.
    /// </summary>
    /// <typeparam name="TPrimitive">The domain primitive type.</typeparam>
    /// <typeparam name="TValue">The backing value type.</typeparam>
    /// <param name="assertions">The FluentAssertions <see cref="ObjectAssertions"/> context, where the subject must be of type <typeparamref name="TValue"/>.</param>
    /// <returns>An <see cref="AndConstraint{T}"/> for chaining further assertions.</returns>
    public static AndConstraint<ObjectAssertions> ShouldBeValidPrimitive<TPrimitive, TValue>(
        this ObjectAssertions assertions)
        where TPrimitive : struct, IDomainPrimitive<TPrimitive, TValue>
        where TValue : notnull
    {
        var value = (TValue)assertions.Subject;
        bool success = TPrimitive.TryCreate(value, out var result, out _);

        success.Should().BeTrue($"value '{value}' should be a valid {typeof(TPrimitive).Name}");

        return new AndConstraint<ObjectAssertions>(assertions);
    }

    /// <summary>
    /// Validates that the subject value fails to be converted to the specified domain primitive type and that the thrown exception carries the expected error code.
    /// </summary>
    /// <typeparam name="TPrimitive">The domain primitive type.</typeparam>
    /// <typeparam name="TValue">The backing value type.</typeparam>
    /// <param name="assertions">The FluentAssertions <see cref="ObjectAssertions"/> context, where the subject must be of type <typeparamref name="TValue"/>.</param>
    /// <param name="errorCode">The <see cref="DomainPrimitiveValidationException.Error"/> code that the validation failure must produce.</param>
    /// <returns>An <see cref="AndConstraint{T}"/> for chaining further assertions.</returns>
    public static AndConstraint<ObjectAssertions> ShouldHaveValidationPrimitiveError<TPrimitive, TValue>(
        this ObjectAssertions assertions, string errorCode)
        where TPrimitive : struct, IDomainPrimitive<TPrimitive, TValue>
        where TValue : notnull
    {
        var value = (TValue)assertions.Subject;
        Action act = () => TPrimitive.Create(value);
        
        act.Should().Throw<DomainPrimitiveValidationException>()
            .Where(e => e.Error.Code == errorCode, $"the validation error code for '{value}' should be '{errorCode}'");

        return new AndConstraint<ObjectAssertions>(assertions);
    }
}





