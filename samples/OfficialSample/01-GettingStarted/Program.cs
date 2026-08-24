// Copyright © Erickson Lopez. MIT License.
// ============================================================================
// CHAPTER 01: GETTING STARTED - INTRODUCTION TO DOMAIN PRIMITIVES
// ============================================================================
// This chapter teaches the fundamentals of the Domain Primitives ecosystem in .NET 10.
// ============================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Chapter01;
using EricksonLopez.DomainPrimitives;
using EricksonLopez.Result;
using System.Threading.Tasks;

Console.WriteLine("=========================================================");
Console.WriteLine(" 📘 CHAPTER 01: INTRODUCTION TO DOMAIN PRIMITIVES (.NET 10)");
Console.WriteLine("=========================================================\n");

// ----------------------------------------------------------------------------
// 1. TRADITIONAL CODE (BEFORE) - PRIMITIVE OBSESSION
// ----------------------------------------------------------------------------
Console.WriteLine("--- ❌ BEFORE (TRADITIONAL CODE WITH PRIMITIVE OBSESSION) ---");

string userEmail = "user@company.com";
string userAddress = "Main Street 123";
Guid userId = Guid.NewGuid();
Guid orderId = Guid.NewGuid();

ProcessTraditionalOrder(userId, orderId, userEmail, userAddress);

Console.WriteLine();

// ----------------------------------------------------------------------------
// 2. CODE WITH DOMAIN PRIMITIVES (AFTER) - TYPE SAFE
// ----------------------------------------------------------------------------
Console.WriteLine("--- ✅ AFTER (STRONGLY TYPED DOMAIN WITH ZERO OVERHEAD) ---");

// Direct creation of strongly typed IDs
var customerId = CustomerId.Create();
var domainOrderId = OrderId.Create();

// Method 1: TryCreate (Safe, returns Result<EmailAddress> without throwing exceptions)
Result<EmailAddress> emailResult = EmailAddress.TryCreate("user@company.com", out var emailOut, out var error) ? emailOut : Error.Validation(error.Code, error.Message);

if (emailResult.IsSuccess)
{
    EmailAddress email = emailResult.Value;
    Console.WriteLine($"[Validated Domain] Customer ID: {customerId}");
    Console.WriteLine($"[Validated Domain] Order ID:  {domainOrderId}");
    Console.WriteLine($"[Validated Domain] Email:      {email}");
    
    ProcessDomainOrder(customerId, domainOrderId, email);
}

// Method 2: Create (Throws DomainPrimitiveException if invalid)
EmailAddress directEmail = EmailAddress.Create("admin@empresa.com");
Console.WriteLine($"[Direct Domain] Email: {directEmail}");

// ----------------------------------------------------------------------------
// 3. COMPILE TIME SECURITY DEMONSTRATION
// ----------------------------------------------------------------------------
Console.WriteLine("\n--- 🛡️ COMPILE TIME PROTECTION ---");
Console.WriteLine("The C# compiler blocks assignment of incompatible types.");

// ❌ INCORRECT CODE (Uncomment to see the compilation error):
// CustomerId customer = OrderId.Create(); // CS0029: Cannot implicitly convert type 'OrderId' to 'CustomerId'

Console.WriteLine("✅ The compiler prevented swapping CustomerId with OrderId.");
Console.WriteLine("✅ Invalid emails cannot be instantiated without going through domain rules.");
Console.WriteLine("\nCHAPTER 01 COMPLETED SUCCESSFULLY.\n");


static void ProcessTraditionalOrder(Guid customerId, Guid orderId, string email, string address)
{
    Console.WriteLine($"[Traditional] Processing order {orderId} for customer {customerId}");
}

static void ProcessDomainOrder(CustomerId customerId, OrderId orderId, EmailAddress email)
{
    Console.WriteLine($"[Domain Primitives] Processing order {orderId} for customer {customerId} ({email})");
}

// ============================================================================
// DOMAIN TYPES DEFINITION IN A DEDICATED NAMESPACE
// ============================================================================
namespace Chapter01
{
    [StrongId<Guid>]
    public readonly partial record struct CustomerId;

    [StrongId<Guid>]
    public readonly partial record struct OrderId;

    [Email]
    public readonly partial record struct EmailAddress;
}




