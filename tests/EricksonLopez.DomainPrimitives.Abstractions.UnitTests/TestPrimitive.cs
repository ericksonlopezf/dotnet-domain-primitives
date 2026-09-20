// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.Abstractions.UnitTests;

/// <summary>
/// Domain primitive implementation for testing collection extension methods.
/// </summary>
public readonly struct TestPrimitive : IDomainPrimitive<TestPrimitive, int>
{
    private readonly int _value;
    private readonly bool _isInitialized;
    
    public TestPrimitive(int value)
    {
        _value = value;
        _isInitialized = true;
    }

    public int Value => _value;
    
    public static string PrimitiveName => "TestPrimitive";
    
    public bool IsDefault => !_isInitialized;

    public static TestPrimitive Create(int value)
    {
        if (value < 0)
        {
            throw new DomainPrimitiveValidationException(new EricksonLopez.DomainPrimitives.Validation.PrimitiveError("Invalid", "Value cannot be negative."));
        }
        return new TestPrimitive(value);
    }

    public static bool TryCreate(int value, out TestPrimitive result, out EricksonLopez.DomainPrimitives.Validation.PrimitiveError validationError)
    {
        if (value < 0)
        {
            result = default;
            validationError = new EricksonLopez.DomainPrimitives.Validation.PrimitiveError("TestPrimitive", "Must be positive");
            return false;
        }
        result = new TestPrimitive(value);
        validationError = EricksonLopez.DomainPrimitives.Validation.PrimitiveError.None;
        return true;
    }
}
