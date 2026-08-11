using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
// ============================================================================
// CHAPTER 02: THE RESULT PATTERN - FLOW CONTROL WITHOUT EXCEPTIONS
// ============================================================================
// In this chapter you will learn to manage domain flow through `Result<T>`
// and the "Railway-Oriented Programming" (ROP) approach using `dotnet-result`.
//
// WHY NOT USE EXCEPTIONS FOR FLOW CONTROL?
// 1. Performance: Throwing exceptions generates an expensive StackTrace (up to 100x slower).
// 2. Transparency: A method signature that throws exceptions is deceiving `decimal Divide(decimal a, decimal b)`
//    does not indicate that it might throw `DivideByZeroException`.
// 3. Complexity: The code fills up with nested try-catch blocks.
// ============================================================================

using Chapter02;
using EricksonLopez.DomainPrimitives;
using EricksonLopez.Result;

Console.WriteLine("=========================================================");
Console.WriteLine(" 📘 CHAPTER 02: FLOW CONTROL WITH RESULT<T>");
Console.WriteLine("=========================================================\n");

// ----------------------------------------------------------------------------
// 1. TRADITIONAL CODE (BEFORE) - WITH EXCEPTIONS
// ----------------------------------------------------------------------------
Console.WriteLine("--- ❌ BEFORE (EXCEPTIONS FOR VALID BUSINESS RULES) ---");

try
{
    decimal invalidAmount = -50m;
    ProcessTraditionalPayment(invalidAmount);
}
catch (ArgumentException ex)
{
    Console.WriteLine($"[Catch Caught] Business error: {ex.Message}");
}

Console.WriteLine();

// ----------------------------------------------------------------------------
// 2. CODE WITH RESULT (AFTER) - ROP AND FUNCTIONAL FLOW
// ----------------------------------------------------------------------------
Console.WriteLine("--- ✅ AFTER (RESULT PATTERN WITH ZERO STACKTRACE ALLOCATIONS) ---");

// Creating the primitive via TryCreate returns a Result<Money>
Result<Money> moneyResult = Money.TryCreate(150.00m, out var money, out var error) ? money : Error.Validation(error.Code, error.Message);

// Monadic Composition via Railway-Oriented Programming (Match)
string message = moneyResult.Match(
    onSuccess: money => $"[Success] Payment processed successfully for value of {money}",
    onFailure: error => $"[Failure] Payment could not be processed: {error.Description}"
);

Console.WriteLine(message);

Console.WriteLine("\n--- 🔄 MONADIC COMPOSITION (MAP AND BIND) ---");

// Attempt to process an invalid deposit using ROP chaining
Result<Money> firstResult = Money.TryCreate(-20m, out var money2, out var error2) ? Result.Success(money2) : Result.Failure<Money>(Error.Validation(error2.Code, error2.Message));
Result<Money> transactionResult = firstResult.IsSuccess ? ValidateAllowedAmount(firstResult.Value) : firstResult;

if (transactionResult.IsFailure)
{
    Console.WriteLine($"❌ Transaction rejected: {transactionResult.Error.Code} - {transactionResult.Error.Description}");
}

Console.WriteLine("\nCHAPTER 02 COMPLETED SUCCESSFULLY.\n");


// ============================================================================
// HELPER METHODS
// ============================================================================

static void ProcessTraditionalPayment(decimal amount)
{
    if (amount <= 0)
        throw new ArgumentException("Amount must be strictly positive.");

    Console.WriteLine($"[Traditional] Payment processed: {amount:C}");
}

static Result<Money> ValidateAllowedAmount(Money money)
{
    if (money.Value > 10000m)
        return Error.Validation("Money.ExceedsLimit", "Amount exceeds the maximum allowed limit per transaction.");

    return money;
}

// ============================================================================
// DOMAIN PRIMITIVE DEFINITION
// ============================================================================
namespace Chapter02
{
    [Money(Min = 0.01, Max = 1_000_000)]
    public readonly partial record struct Money;
}


