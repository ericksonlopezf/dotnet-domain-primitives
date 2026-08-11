using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
// ============================================================================
// CHAPTER 11: SMART ENUMS (ENUMERATIONS RICH IN BEHAVIOR)
// ============================================================================
// In this chapter you will learn to use `[SmartEnum<TValue>]` to replace
// traditional primitive `enum`s in C# that lack methods and behavior.
//
// ADVANTAGES OF A SMART ENUM OVER A TRADITIONAL ENUM:
// 1. Methods and Business Logic: Can include behavior (e.g. `CanTransitionTo()`).
// 2. Type-Safety and Invariants: Impossible to assign arbitrary integer values outside the allowed ones.
// 3. Auto-Discovery: Access to the `All` collection with all declared items.
// 4. Parsing by Name or by Value: `TryFromValue()`, `TryFromName()`, `TryCreate()`.
// ============================================================================

using Chapter11;
using EricksonLopez.DomainPrimitives;
using EricksonLopez.Result;

Console.WriteLine("=========================================================");
Console.WriteLine(" 📘 CHAPTER 11: SMART ENUMS (ENUMERATIONS WITH BEHAVIOR)");
Console.WriteLine("=========================================================\n");

// ----------------------------------------------------------------------------
// 1. BASIC USAGE AND NAVIGATION
// ----------------------------------------------------------------------------
Console.WriteLine("--- 🏷️ ACCESS TO VALUES AND COMPLETE LISTING ---");

Console.WriteLine($"Pending State:      {OrderStatus.Pending} (Value: {OrderStatus.Pending.Value})");
Console.WriteLine($"Processing State:   {OrderStatus.Processing} (Value: {OrderStatus.Processing.Value})");
Console.WriteLine($"Shipped State:      {OrderStatus.Shipped} (Value: {OrderStatus.Shipped.Value})");

Console.WriteLine("\n[All States Registered in OrderStatus.All]:");
foreach (var status in OrderStatus.All)
{
    Console.WriteLine($"  • ID: {status.Value} -> {status.Name}");
}

Console.WriteLine();

// ----------------------------------------------------------------------------
// 2. PARSING AND SAFE SEARCH (FROM DB OR USER INPUT)
// ----------------------------------------------------------------------------
Console.WriteLine("--- 🔍 SAFE PARSING BY VALUE AND BY NAME ---");

bool isparseSuccess = OrderStatus.TryCreate(2, out var parse, out var parseError);
if (isparseSuccess)
{
    Console.WriteLine($"Found by TryCreate(2): {parse.Name} ✅");
}

if (OrderStatus.TryFromName("Shipped", out var shippedStatus))
{
    Console.WriteLine($"Found by TryFromName('Shipped'): {shippedStatus.Name} ✅");
}

bool isinvalidValueSuccess = OrderStatus.TryCreate(99, out var invalidValue, out var invalidValueError);
if (!isinvalidValueSuccess)
{
    Console.WriteLine($"Search with invalid value 99 blocked: {invalidValueError.Message} ❌");
}

Console.WriteLine();

// ----------------------------------------------------------------------------
// 3. INTEGRATED BEHAVIOR AND BUSINESS RULES
// ----------------------------------------------------------------------------
Console.WriteLine("--- ⚙️ LOGIC AND STATE TRANSITIONS IN THE SMART ENUM ---");

var current = OrderStatus.Pending;

Console.WriteLine($"Can Pending transition to Processing?: {current.CanTransitionTo(OrderStatus.Processing)} ✅");
Console.WriteLine($"Can Pending transition to Delivered directly?: {current.CanTransitionTo(OrderStatus.Delivered)} ❌");

Console.WriteLine("\nCHAPTER 11 COMPLETED SUCCESSFULLY.\n");


// ============================================================================
// DOMAIN SMART ENUM DEFINITION
// ============================================================================

namespace Chapter11
{
    [SmartEnum<int>]
    public readonly partial record struct OrderStatus
    {
        public static readonly OrderStatus Pending = new(1, "Pending");
        public static readonly OrderStatus Processing = new(2, "Processing");
        public static readonly OrderStatus Shipped = new(3, "Shipped");
        public static readonly OrderStatus Delivered = new(4, "Delivered");
        public static readonly OrderStatus Canceled = new(5, "Canceled");

        // Integrated business method within the Smart Enum
        public bool CanTransitionTo(OrderStatus newState)
        {
            if (this == Canceled || this == Delivered)
                return false; // Terminal states

            if (this == Pending)
                return newState == Processing || newState == Canceled;

            if (this == Processing)
                return newState == Shipped || newState == Canceled;

            if (this == Shipped)
                return newState == Delivered;

            return false;
        }
    }
}


