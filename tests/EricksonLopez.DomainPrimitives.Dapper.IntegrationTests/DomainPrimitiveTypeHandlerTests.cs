using System;
using System.Data;
using System.Globalization;
using EricksonLopez.DomainPrimitives;
using EricksonLopez.DomainPrimitives.Dapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace EricksonLopez.DomainPrimitives.Dapper.Tests;

public class DomainPrimitiveTypeHandlerTests
{
    private readonly struct StubPrimitive : IDomainPrimitive<StubPrimitive, string>
    {
        private readonly string _value;
        private readonly bool _isInitialized;

        public StubPrimitive(string value)
        {
            _value = value;
            _isInitialized = true;
        }

        public string Value => _value;
        public static string PrimitiveName => "StubPrimitive";
        public bool IsDefault => !_isInitialized;

        public static StubPrimitive Create(string value)
        {
            if (value == "invalid") throw new DomainPrimitiveValidationException(new EricksonLopez.DomainPrimitives.Validation.PrimitiveError(PrimitiveName, "Invalid value"));
            return new StubPrimitive(value);
        }

        public static bool TryCreate(string value, out StubPrimitive result, out global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError validationError)
        {
            validationError = global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError.None;
            if (value == "invalid")
            {
                result = default;
                return false;
            }
            result = new StubPrimitive(value);
            return true;
        }

            }

    private readonly struct GuidStubPrimitive : IDomainPrimitive<GuidStubPrimitive, Guid>
    {
        private readonly Guid _value;
        private readonly bool _isInitialized;

        public GuidStubPrimitive(Guid value)
        {
            _value = value;
            _isInitialized = true;
        }

        public Guid Value => _value;
        public static string PrimitiveName => "GuidStubPrimitive";
        public bool IsDefault => !_isInitialized;

        public static GuidStubPrimitive Create(Guid value) => new GuidStubPrimitive(value);
        public static bool TryCreate(Guid value, out GuidStubPrimitive result, out global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError validationError)
        {
            validationError = global::EricksonLopez.DomainPrimitives.Validation.PrimitiveError.None;
            result = new GuidStubPrimitive(value);
            return true;
        }

            }

    private sealed class StringConvertible : IConvertible
    {
        private readonly string _val;
        public StringConvertible(string val) => _val = val;

        public TypeCode GetTypeCode() => TypeCode.String;
        public bool ToBoolean(IFormatProvider? provider) => throw new NotImplementedException();
        public byte ToByte(IFormatProvider? provider) => throw new NotImplementedException();
        public char ToChar(IFormatProvider? provider) => throw new NotImplementedException();
        public DateTime ToDateTime(IFormatProvider? provider) => throw new NotImplementedException();
        public decimal ToDecimal(IFormatProvider? provider) => throw new NotImplementedException();
        public double ToDouble(IFormatProvider? provider) => throw new NotImplementedException();
        public short ToInt16(IFormatProvider? provider) => throw new NotImplementedException();
        public int ToInt32(IFormatProvider? provider) => throw new NotImplementedException();
        public long ToInt64(IFormatProvider? provider) => throw new NotImplementedException();
        public sbyte ToSByte(IFormatProvider? provider) => throw new NotImplementedException();
        public float ToSingle(IFormatProvider? provider) => throw new NotImplementedException();
        public string ToString(IFormatProvider? provider) => _val;
        public object ToType(Type conversionType, IFormatProvider? provider) => _val;
        public ushort ToUInt16(IFormatProvider? provider) => throw new NotImplementedException();
        public uint ToUInt32(IFormatProvider? provider) => throw new NotImplementedException();
        public ulong ToUInt64(IFormatProvider? provider) => throw new NotImplementedException();
    }

#if NET7_0_OR_GREATER
    [Fact]
    public void SetValue_ShouldSetCorrectParameterValue()
    {
        // Arrange
        var handler = new DomainPrimitiveTypeHandler<StubPrimitive, string>();
        var parameter = Substitute.For<IDbDataParameter>();
        var primitive = StubPrimitive.Create("test_value");

        // Act
        handler.SetValue(parameter, primitive);

        // Assert
        parameter.Received().Value = "test_value";
    }

    [Fact]
    public void Parse_WithNull_ShouldThrowDomainPrimitiveValidationException()
    {
        // Arrange
        var handler = new DomainPrimitiveTypeHandler<StubPrimitive, string>();

        // Act & Assert
        Action act = () => handler.Parse(null!);
        act.Should().Throw<DomainPrimitiveValidationException>()
           .WithMessage("*Cannot parse null database value*");
    }

    [Fact]
    public void Parse_WithDBNull_ShouldThrowDomainPrimitiveValidationException()
    {
        // Arrange
        var handler = new DomainPrimitiveTypeHandler<StubPrimitive, string>();

        // Act & Assert
        Action act = () => handler.Parse(DBNull.Value);
        act.Should().Throw<DomainPrimitiveValidationException>()
           .WithMessage("*Cannot parse null database value*");
    }

    [Fact]
    public void Parse_WithMatchingType_ShouldReturnPrimitive()
    {
        // Arrange
        var handler = new DomainPrimitiveTypeHandler<StubPrimitive, string>();

        // Act
        var result = handler.Parse("test_value");

        // Assert
        result.Value.Should().Be("test_value");
    }

    [Fact]
    public void Parse_WithGuidString_ShouldReturnPrimitive()
    {
        // Arrange
        var handler = new DomainPrimitiveTypeHandler<GuidStubPrimitive, Guid>();
        var guid = Guid.NewGuid();

        // Act
        var result = handler.Parse(guid.ToString());

        // Assert
        result.Value.Should().Be(guid);
    }

    [Fact]
    public void Parse_WithGuid_ShouldReturnPrimitive()
    {
        // Arrange
        var handler = new DomainPrimitiveTypeHandler<GuidStubPrimitive, Guid>();
        var guid = Guid.NewGuid();

        // Act
        var result = handler.Parse(guid);

        // Assert
        result.Value.Should().Be(guid);
    }

    [Fact]
    public void Parse_WithInvalidGuidString_ShouldThrowDomainPrimitiveValidationException()
    {
        // Arrange
        var handler = new DomainPrimitiveTypeHandler<GuidStubPrimitive, Guid>();

        // Act & Assert
        Action act = () => handler.Parse("invalid-guid");
        act.Should().Throw<DomainPrimitiveValidationException>()
           .WithMessage("*Failed to convert database value*");
    }

    [Fact]
    public void Parse_WithInvalidTValue_ShouldThrowDomainPrimitiveValidationExceptionDirectly()
    {
        // Arrange
        var handler = new DomainPrimitiveTypeHandler<StubPrimitive, string>();

        // Act & Assert
        Action act = () => handler.Parse("invalid");
        act.Should().Throw<DomainPrimitiveValidationException>()
           .WithMessage("*Invalid value*");
    }

    [Fact]
    public void Parse_WithIConvertible_ShouldReturnPrimitive()
    {
        // Arrange
        var handler = new DomainPrimitiveTypeHandler<StubPrimitive, string>();
        var convertible = new StringConvertible("test_value");

        // Act
        var result = handler.Parse(convertible);

        // Assert
        result.Value.Should().Be("test_value");
    }

    [Fact]
    public void Parse_WithInvalidIConvertibleValue_ShouldThrowDomainPrimitiveValidationExceptionDirectly()
    {
        // Arrange
        var handler = new DomainPrimitiveTypeHandler<StubPrimitive, string>();
        var convertible = new StringConvertible("invalid");

        // Act & Assert
        Action act = () => handler.Parse(convertible);
        act.Should().Throw<DomainPrimitiveValidationException>()
           .WithMessage("*Invalid value*");
    }

    [Fact]
    public void Parse_WithInvalidType_ShouldThrowDomainPrimitiveValidationException()
    {
        // Arrange
        var handler = new DomainPrimitiveTypeHandler<StubPrimitive, string>();

        // Act & Assert
        Action act = () => handler.Parse(new object()); // Cannot cast object to string
        act.Should().Throw<DomainPrimitiveValidationException>()
           .WithMessage("*Failed to convert database value*");
    }
#endif
}
