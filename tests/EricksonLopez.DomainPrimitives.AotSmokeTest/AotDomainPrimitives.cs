// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;
using EricksonLopez.DomainPrimitives.Validation;

namespace EricksonLopez.DomainPrimitives.AotSmokeTest;

[StrongId<Guid>]
public readonly partial record struct AotOrderId;

[StringPrimitive]
[Trim]
[MinLength(2)]
[MaxLength(50)]
public readonly partial record struct AotCustomerName;

[NumericPrimitive<decimal>]
[PrimitiveRange(0, 100000)]
public readonly partial record struct AotPrice;

[DatePrimitive(Kind = DatePrimitiveKind.DateOnly)]
public readonly partial record struct AotOrderDate;

[SmartEnum<int>]
public readonly partial record struct AotPriority
{
    public static readonly AotPriority Low = new(1, nameof(Low));
    public static readonly AotPriority Medium = new(2, nameof(Medium));
    public static readonly AotPriority High = new(3, nameof(High));
}

[ValueObject]
public readonly partial record struct AotAddress
{
    public required string Street { get; init; }
    public required string City { get; init; }
}

