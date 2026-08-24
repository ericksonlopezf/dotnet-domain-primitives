// Copyright © Erickson Lopez. MIT License.
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
// 6. [assembly: DomainPrimitivesDefaults] — assembly-level global configuration.
// ============================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Chapter09;
using EricksonLopez.DomainPrimitives;
using System.Threading.Tasks;

// Assembly-level global defaults: Trim=true auto-trims all string primitives in this assembly.
// Must appear after 'using' directives and before any top-level statements or type declarations.
[assembly: DomainPrimitivesDefaults(Trim = true, NotEmpty = false, MaxLength = 4096)]

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


// ─────────────────────────────────────────────────────────────────────────
// SECTION 3: [assembly: DomainPrimitivesDefaults(...)]
// ─────────────────────────────────────────────────────────────────────────
Console.WriteLine("--- ⚙️  SECTION 3: [assembly: DomainPrimitivesDefaults] ---");
Console.WriteLine();
Console.WriteLine("Place this once in any .cs file in your assembly:");
Console.WriteLine("  [assembly: DomainPrimitivesDefaults(");
Console.WriteLine("      Trim       = true,    // auto-trim all string primitives in this assembly");
Console.WriteLine("      NotEmpty   = true,    // auto-require non-empty string for all primitives");
Console.WriteLine("      MaxLength  = 2048,    // default security max length (default is 4096)");
Console.WriteLine("  )]");
Console.WriteLine();
Console.WriteLine("Semantics:");
Console.WriteLine("  • Trim=true means every string primitive in this assembly auto-trims whitespace.");
Console.WriteLine("  • Per-type attributes ([Trim], [NotEmpty]) override assembly defaults.");
Console.WriteLine("  • ExceptionType property allows swapping DomainPrimitiveValidationException");
Console.WriteLine("    with a custom exception type (must have a constructor accepting string message).");
Console.WriteLine();

// Demonstrate runtime behavior: this assembly uses [assembly: DomainPrimitivesDefaults(Trim=true)]
// declared at the bottom of this file.
bool trimDefault = AccountNumber.TryCreate("  ACC-001  ", out var accountNum, out _);
Console.WriteLine($"  AccountNumber.TryCreate('  ACC-001  ') with assembly Trim=true:");
Console.WriteLine($"    Success={trimDefault}, StoredValue='{(trimDefault ? accountNum.Value : "(failed)")}'");
Console.WriteLine($"    Leading/trailing whitespace automatically removed: \u2705");
Console.WriteLine();
Console.WriteLine("CHAPTER 09 COMPLETED SUCCESSFULLY (INCLUDING SECTION 3).\n");


// ============================================================================
// TYPE DEFINITIONS USED IN THE CHAPTER
// ============================================================================

namespace Chapter09
{
    [StrongId<Guid>]
    public readonly partial record struct AccountId;

    [Money(Min = 0)]
    public readonly partial record struct AccountBalance;

    // Used in Section 3: demonstrates that [assembly: DomainPrimitivesDefaults(Trim=true)]
    // automatically trims whitespace for this primitive without an explicit [Trim] attribute.
    [StringPrimitive]
    [NotEmpty]
    public readonly partial record struct AccountNumber;
}




