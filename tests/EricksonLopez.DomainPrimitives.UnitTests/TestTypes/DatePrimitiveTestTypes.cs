using System;
using EricksonLopez.DomainPrimitives;

namespace EricksonLopez.DomainPrimitives.Tests.TestTypes;

[DatePrimitive(Kind = DatePrimitiveKind.DateTime, PastOnly = true)]
public readonly partial record struct RegistrationTimestamp;

[BirthDate(MaxAge = 120)]
public readonly partial record struct CustomerBirthDate;

[ExpirationDate]
public readonly partial record struct CreditCardExpiration;

[DatePrimitive(Kind = DatePrimitiveKind.TimeOnly, FutureOnly = true)]
public readonly partial record struct ShiftStartTime;
