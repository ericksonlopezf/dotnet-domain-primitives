using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
// ============================================================================
// CHAPTER 09: SOURCE GENERATORS (INSPECTION AND ZERO REFLECTION)
// ============================================================================
// In this chapter we will explore how the library's Roslyn Source Generators 
// generate all plumbing code at compile time.
//
// WHAT IS GENERATED AUTOMATICALLY:
// 1. Factory Methods: `Create()`, `TryCreate()`, `New()`.
// 2. Explicit and implicit conversion operators (`(string)`, `(Guid)`, etc.).
// 3. Arithmetic and comparison operators (`==`, `!=`, `<`, `>`, `+`, `-`).
// 4. Implementation of `IParsable<T>`, `IUtf8SpanParsable<T>`, `ISpanFormattable`.
// 5. Support for .NET `TypeConverter` (binding in controllers, EF Core).
// ============================================================================

using System.ComponentModel;
using Chapter09;
using EricksonLopez.DomainPrimitives;

Console.WriteLine("=========================================================");
Console.WriteLine(" 📘 CHAPTER 09: ROSLYN SOURCE GENERATORS (ANATOMY)");
Console.WriteLine("=========================================================\n");

// ----------------------------------------------------------------------------
// 1. DEMONSTRATION OF GENERATED APIS FOR NUMERIC PRIMITIVE
// ----------------------------------------------------------------------------
Console.WriteLine("--- 🔢 GENERATED CODE ANATOMY: NUMERIC PRIMITIVE ---");

var balanceAcc = AccountBalance.Create(500.00m);
var deposit = AccountBalance.Create(250.50m);

// Generated arithmetic free of boxing
AccountBalance total = balanceAcc + deposit;
Console.WriteLine($"Balance A + B (Generated + operator): {total}");

// Generated comparison operators
Console.WriteLine($"balanceAcc < deposit: {balanceAcc < deposit}");
Console.WriteLine($"total > balanceAcc:    {total > balanceAcc}");

Console.WriteLine();

// ----------------------------------------------------------------------------
// 2. DEMONSTRATION OF GENERATED TYPECONVERTER FOR EF CORE / ASP.NET CORE
// ----------------------------------------------------------------------------
Console.WriteLine("--- 🔌 GENERATED TYPECONVERTER (ZERO REFLECTION) ---");

TypeConverter converter = TypeDescriptor.GetConverter(typeof(AccountId));
bool canConvertFromString = converter.CanConvertFrom(typeof(string));
Console.WriteLine($"TypeConverter registered for AccountId: {converter.GetType().Name}");
Console.WriteLine($"CanConvertFrom(string): {canConvertFromString} ✅");

if (canConvertFromString)
{
    Guid guidStr = Guid.NewGuid();
    AccountId accountFromStr = (AccountId)converter.ConvertFrom(guidStr.ToString())!;
    Console.WriteLine($"Successful ConvertFrom String: {accountFromStr}");
}

Console.WriteLine("\nCHAPTER 09 COMPLETED SUCCESSFULLY.\n");


// ============================================================================
// TYPE DEFINITIONS USED IN THE CHAPTER
// ============================================================================

namespace Chapter09
{
    [StrongId<Guid>]
    public readonly partial record struct AccountId;

    [Money(Min = 0)]
    public readonly partial record struct AccountBalance;
}


