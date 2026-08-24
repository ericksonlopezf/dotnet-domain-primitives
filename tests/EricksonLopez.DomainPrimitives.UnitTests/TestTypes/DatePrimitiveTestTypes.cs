// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.UnitTests.TestTypes;

[DatePrimitive(Kind = DatePrimitiveKind.DateTime, PastOnly = true)]
public readonly partial record struct RegistrationTimestamp;

[BirthDate(MaxAge = 120)]
public readonly partial record struct CustomerBirthDate;

[ExpirationDate]
public readonly partial record struct CreditCardExpiration;

[DatePrimitive(Kind = DatePrimitiveKind.TimeOnly, FutureOnly = true)]
public readonly partial record struct ShiftStartTime;

[TimeRange]
public readonly partial record struct WorkShiftTime;

[DatePrimitive(Kind = DatePrimitiveKind.DateTimeOffset, PastOnly = true)]
public readonly partial record struct GlobalTimestamp;

