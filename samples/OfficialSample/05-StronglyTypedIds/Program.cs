// Copyright © Erickson Lopez. MIT License.
// ============================================================================
// CHAPTER 05: STRONGLY TYPED IDS
// ============================================================================
// In this chapter you will learn to use `[StrongId<T>]` to completely eradicate
// the "ID Swap Bug" at compile time.
//
// SUPPORTED BACKING TYPES:
// 1. Guid: Generation with `.Create()`, `.Parse()`, `.Empty`.
// 2. Int / Long: Assigned by database or numeric sequences.
// 3. String: Product SKUs, serial numbers, etc.
// ============================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Chapter05;
using EricksonLopez.DomainPrimitives;
using EricksonLopez.Result;
using System.Threading.Tasks;

Console.WriteLine("=========================================================");
Console.WriteLine(" 📘 CHAPTER 05: STRONGLY TYPED IDS ([StrongId<T>])");
Console.WriteLine("=========================================================\n");

// ----------------------------------------------------------------------------
// 1. CREATING IDS OF DIFFERENT BACKING TYPES
// ----------------------------------------------------------------------------
Console.WriteLine("--- 🔑 FABRICATING STRONGLY TYPED IDS ---");

// Guid-backed ID
var customerId = CustomerId.Create();
var orderId = OrderId.Create();

// Int-backed ID
var productId = ProductId.Create(10542);

// String-backed ID
var skuId = SkuId.Create("PROD-AMZ-2026-X");

Console.WriteLine($"CustomerId (Guid):   {customerId}");
Console.WriteLine($"OrderId (Guid):      {orderId}");
Console.WriteLine($"ProductId (int):     {productId}");
Console.WriteLine($"SkuId (string):      {skuId}");

Console.WriteLine("\n--- 🛡️ PREVENTION OF THE 'ID SWAP' BUG (BEFORE vs AFTER) ---");

// Safe method that requires exactly CustomerId and OrderId
ProcessShipment(customerId, orderId, productId);

// ❌ INCORRECT CODE (Uncomment to see the compilation error):
// ProcessShipment(orderId, customerId, productId); // CS0029: Cannot implicitly convert type 'OrderId' to 'CustomerId'

Console.WriteLine("✅ The compiler prevented swapping CustomerId with OrderId.");

Console.WriteLine("\n--- ⚡ PARSING AND SENTINEL CONVERSION ---");

// Predefined sentinels
Console.WriteLine($"CustomerId.Empty: {CustomerId.Empty}");

// Safe parsing via IParsable<T>
string guidString = customerId.Value.ToString();
CustomerId parsedCustomer = CustomerId.Parse(guidString);
Console.WriteLine($"Parsed from string '{guidString}': {parsedCustomer == customerId}");

Console.WriteLine("\nCHAPTER 05 COMPLETED SUCCESSFULLY.\n");


static void ProcessShipment(CustomerId customerId, OrderId orderId, ProductId productId)
{
    Console.WriteLine($"[Shipment Registered] Order {orderId} from Customer {customerId} (Product {productId})");
}

// ============================================================================
// DOMAIN STRONGLY TYPED IDS DEFINITION
// ============================================================================
namespace Chapter05
{
    // Guid-backed strong ID
    [StrongId<Guid>]
    public readonly partial record struct CustomerId;

    // Guid-backed strong ID
    [StrongId<Guid>]
    public readonly partial record struct OrderId;

    // Integer-backed strong ID
    [StrongId<int>]
    public readonly partial record struct ProductId;

    // String-backed strong ID
    [StrongId<string>]
    public readonly partial record struct SkuId;
}




