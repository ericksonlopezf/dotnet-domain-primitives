using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
// ============================================================================
// CHAPTER 10: ROSLYN ANALYZERS & COMPILATION RULES (DOMAIN GUARDIANS)
// ============================================================================
// In this chapter we will explore the suite of Roslyn Analyzers included in
// `EricksonLopez.DomainPrimitives.Analyzers`.
//
// MAIN COMPILATION SECURITY RULES:
// DP0001: The domain primitive must be declared as `partial`.
// DP0002: The domain primitive must be `readonly`.
// DP0003: The domain primitive must be a `record struct`.
// DP0004: Invalid Regex pattern at attribute level.
// DP0005: Conflicting normalizations (e.g. `[LowerCase]` and `[UpperCase]` together).
// DP0006: Invalid numeric invariants.
// DP0007: Avoid uninitialized default constructor (`new CustomerId()`).
// DP0008: The properties of a `[ValueObject]` must have `init`.
// DP0010: Direct comparison between raw string and domain primitive.
// DP0012: Bypassing validations by declaring public constructors.
// ============================================================================

using Chapter10;
using EricksonLopez.DomainPrimitives;

Console.WriteLine("=========================================================");
Console.WriteLine(" 📘 CHAPTER 10: ROSLYN ANALYZERS & DIAGNOSTICS DP0001-DP0012");
Console.WriteLine("=========================================================\n");

// ----------------------------------------------------------------------------
// 1. CORRECT USAGE PROTECTED BY ANALYZERS
// ----------------------------------------------------------------------------
Console.WriteLine("--- 🛡️ COMPILED CODE THAT COMPLIES WITH ALL DP0001-DP0012 RULES ---");

var id = UserAccountId.Create();
var email = UserEmail.Create("correct.user@domain.com");
var age = UserAge.Create(25);

Console.WriteLine($"[Validated at Compilation]");
Console.WriteLine($"  ID:    {id}");
Console.WriteLine($"  Email: {email}");
Console.WriteLine($"  Age:   {age}");

Console.WriteLine("\n--- 🛑 CATALOG OF ERRORS PREVENTED AT COMPILE TIME ---");
Console.WriteLine("The following patterns generate ERRORS and WARNINGS at compile time:");
Console.WriteLine("  • DP0001: Forgetting the 'partial' modifier.");
Console.WriteLine("  • DP0002: Forgetting the 'readonly' modifier.");
Console.WriteLine("  • DP0003: Declaring as 'class' instead of 'record struct'.");
Console.WriteLine("  • DP0005: Using [LowerCase] and [UpperCase] simultaneously.");
Console.WriteLine("  • DP0007: Using the default constructor without initialization.");

Console.WriteLine("\nCHAPTER 10 COMPLETED SUCCESSFULLY.\n");


// ============================================================================
// EXAMPLES OF CORRECT CODE THAT PASS THE ANALYZER
// ============================================================================

namespace Chapter10
{
    // Complies with DP0001, DP0002, DP0003
    [StrongId<Guid>]
    public readonly partial record struct UserAccountId;

    // Complies with DP0005 (Single casing)
    [Email]
    public readonly partial record struct UserEmail;

    // Complies with DP0006
    [NumericPrimitive<int>]
    public readonly partial record struct UserAge;
}

/*
// ============================================================================
// DEMONSTRATION OF EXAMPLES THAT WOULD TRIGGER ROSLYN DIAGNOSTICS (UNCOMMENT IN IDE):
// ============================================================================

namespace Chapter10.Violations
{
    // ❌ Error DP0001: Must be partial
    // [StrongId<Guid>]
    // public readonly record struct ErrorWithoutPartialId;

    // ❌ Error DP0002: Must be readonly
    // [StrongId<Guid>]
    // public partial record struct ErrorWithoutReadonlyId;

    // ❌ Error DP0005: Conflicting normalizations
    // [StringPrimitive]
    // [LowerCase]
    // [UpperCase]
    // public readonly partial record struct ErrorCasingConflict;
}
*/


