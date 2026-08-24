// Copyright © Erickson Lopez. MIT License.
using System;

namespace EricksonLopez.DomainPrimitives.Dapper;

using System.Data;

/// <summary>
/// Provides a base Dapper <see cref="global::Dapper.SqlMapper.TypeHandler{T}"/> that maps domain primitives
/// to and from database column values using the primitive's backing value.
/// </summary>
/// <typeparam name="TPrimitive">The domain primitive struct type to handle.</typeparam>
/// <typeparam name="TValue">The backing value type that maps to the database column type.</typeparam>
public class DomainPrimitiveTypeHandler<TPrimitive, TValue> : global::Dapper.SqlMapper.TypeHandler<TPrimitive>
#if NET7_0_OR_GREATER
    where TPrimitive : struct, IDomainPrimitive<TPrimitive, TValue>
#else
    where TPrimitive : struct
#endif
    where TValue : notnull
{
    /// <inheritdoc/>
    public override void SetValue(IDbDataParameter parameter, TPrimitive value)
    {
#if NET7_0_OR_GREATER
        parameter.Value = value.Value;
#else
        throw new NotSupportedException("DomainPrimitiveTypeHandler requires .NET 7.0 or greater.");
#endif
    }

    /// <inheritdoc/>
    public override TPrimitive Parse(object value)
    {
#if NET7_0_OR_GREATER
        if (value is null or DBNull)
        {
            throw new DomainPrimitiveValidationException(new EricksonLopez.DomainPrimitives.Validation.PrimitiveError("NULL_INPUT", $"Cannot parse null database value into primitive '{typeof(TPrimitive).Name}'."));
        }

        if (value is TValue typedValue)
        {
            return TPrimitive.Create(typedValue);
        }

        if (value is string stringValue && typeof(TValue) == typeof(Guid) && Guid.TryParse(stringValue, out var parsedGuid))
        {
            return TPrimitive.Create((TValue)(object)parsedGuid);
        }

        try
        {
            if (value is IConvertible convertible)
            {
                var converted = (TValue)convertible.ToType(typeof(TValue), System.Globalization.CultureInfo.InvariantCulture);
                return TPrimitive.Create(converted);
            }
            throw new InvalidCastException();
        }
        catch (Exception ex) when (ex is not DomainPrimitiveValidationException)
        {
            throw new DomainPrimitiveValidationException(new EricksonLopez.DomainPrimitives.Validation.PrimitiveError("INVALID_CAST", $"Failed to convert database value '{value}' of type '{value.GetType().Name}' to {typeof(TValue).Name} for primitive '{TPrimitive.PrimitiveName}'."));
        }
#else
        throw new NotSupportedException("DomainPrimitiveTypeHandler requires .NET 7.0 or greater.");
#endif
    }
}


