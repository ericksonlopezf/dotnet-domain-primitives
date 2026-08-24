// Copyright © Erickson Lopez. MIT License.
// ============================================================================
// CHAPTER 03: STRUCTURED ERROR HANDLING WITH ERROR
// ============================================================================
// In this chapter you will learn to catalog, structure and prioritize domain
// errors using the immutable type `Error` from `dotnet-result`.
//
// WHY ERROR CODES AND STRUCTURED ERRORS?
// 1. Magic strings ("Error: Customer does not exist"): They are not translatable, cannot be accurately tested.
// 2. HTTP Mapping: Error types (`NotFound`, `Validation`, `Conflict`) translate 1 to 1 to HTTP codes (404, 400, 409).
// 3. Metadata: Allows attaching key context (e.g. `Timestamp`, `AttemptCount`, `Property`) without breaking the error contract.
// ============================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Chapter03;
using EricksonLopez.DomainPrimitives;
using EricksonLopez.Result;
using System.Threading.Tasks;

Console.WriteLine("=========================================================");
Console.WriteLine(" 📘 CHAPTER 03: STRUCTURED AND CATALOGED ERRORS");
Console.WriteLine("=========================================================\n");

// ----------------------------------------------------------------------------
// 1. TRADITIONAL CODE (BEFORE) - PLAIN TEXT MESSAGES
// ----------------------------------------------------------------------------
Console.WriteLine("--- ❌ BEFORE (ERROR-PRONE PLAIN TEXT MESSAGES) ---");

string traditionalError = "Error 404: Customer 12345 is not active";
Console.WriteLine($"[Traditional] Message: {traditionalError}");
// Difficulty: The API layer would have to parse the string to know whether to return HTTP 404 or HTTP 400.

Console.WriteLine();

// ----------------------------------------------------------------------------
// 2. CODE WITH STRUCTURED ERROR (AFTER)
// ----------------------------------------------------------------------------
Console.WriteLine("--- ✅ AFTER (TYPED DOMAIN ERROR CATALOG) ---");

var customerId = CustomerId.Create();

// Simulating search in domain catalog
Result<Customer> searchResult = FindCustomer(customerId);

if (searchResult.IsFailure)
{
    Error error = searchResult.Error;
    Console.WriteLine($"[Error Detected] Code:        {error.Code}");
    Console.WriteLine($"[Error Detected] Description: {error.Description}");
    Console.WriteLine($"[Error Detected] Type:        {ErrorType.Validation}");
    Console.WriteLine($"[Error Detected] Severity:    {error.Severity}");

    if (error.HasMetadata)
    {
        Console.WriteLine("[Error Metadata]:");
        foreach (var kvp in error.Metadata)
        {
            Console.WriteLine($"   • {kvp.Key} = {kvp.Value}");
        }
    }
}

Console.WriteLine("\n--- 🏗️ ERROR COMPOSITION WITH INNER ERRORS ---");

// Create a composite error (Aggregate/Batch)
var e1 = Error.Validation("Customer.InvalidEmail", "Email format is invalid.");
var e2 = Error.Validation("Customer.MinimumAge", "Customer must be over 18 years old.");

var compositeError = Error.Custom(
    code: "Customer.RegistrationFailed",
    description: "Could not register the customer due to multiple validation errors.",
    type: ErrorType.Validation,
    severity: ErrorSeverity.Warning,
    metadata: new Dictionary<string, object> { ["TotalErrors"] = 2 },
    innerErrors: [e1, e2]
);

Console.WriteLine($"Root Error: {compositeError.Code} - {compositeError.Description}");
if (compositeError.HasInnerErrors)
{
    Console.WriteLine("Validation sub-errors:");
    foreach (var sub in compositeError.InnerErrors)
    {
        Console.WriteLine($"  ➜ [{sub.Code}] {sub.Description}");
    }
}

Console.WriteLine("\nCHAPTER 03 COMPLETED SUCCESSFULLY.\n");


// ============================================================================
// HELPER METHODS AND STATIC DOMAIN ERRORS
// ============================================================================

static Result<Customer> FindCustomer(CustomerId id)
{
    // We use the static error catalog factory
    return CustomerErrors.NotFound(id);
}

public record struct Customer(CustomerId Id, string Name);

public static class CustomerErrors
{
    public static Error NotFound(CustomerId id) => Error.Custom(
        code: "Customer.NotFound",
        description: $"No active customer found with identifier '{id}'.",
        type: ErrorType.NotFound,
        severity: ErrorSeverity.Error,
        metadata: new Dictionary<string, object>
        {
            ["CustomerId"] = id.Value.ToString(),
            ["QueryDate"] = DateTime.UtcNow.ToString("o")
        }
    );

    public static readonly Error Inactive = Error.Failure(
        code: "Customer.Inactive",
        description: "The customer is temporarily suspended."
    );
}

// ============================================================================
// DOMAIN PRIMITIVE DEFINITIONS
// ============================================================================
namespace Chapter03
{
    [StrongId<Guid>]
    public readonly partial record struct CustomerId;
}




