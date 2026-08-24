// Copyright © Erickson Lopez. MIT License.
// ============================================================================
// CHAPTER 23: DAPPER INTEGRATION (TYPE HANDLERS & AUTO-DISCOVERY)
// ============================================================================
// In this chapter you will learn to use Domain Primitives with Dapper using
// the `EricksonLopez.DomainPrimitives.Dapper` package.
//
// MAIN ADVANTAGE:
// Dapper needs to know how to convert values from the database to Domain
// Primitives and vice versa. The package automatically generates TypeHandlers
// for ALL your primitives without reflection — compatible with NativeAOT.
//
// SETUP (once at application startup):
//   DapperDomainPrimitivesRegistration.RegisterAll(); // auto-discovers all primitives
//
// LATER: use Dapper normally — primitives are mapped like any type.
// ============================================================================
using System;
using System.Data;
using System.Threading;
using Chapter23;
using Dapper;
using EricksonLopez.DomainPrimitives.Dapper.Generated;
using Microsoft.Data.Sqlite;
using System.Threading.Tasks;

Console.WriteLine("=========================================================");
Console.WriteLine(" 📘 CHAPTER 23: DAPPER INTEGRATION");
Console.WriteLine("=========================================================\n");

// ----------------------------------------------------------------------------
// 1. SETUP: REGISTER ALL TYPE HANDLERS (once)
// ----------------------------------------------------------------------------
Console.WriteLine("--- ⚙️  SETUP: REGISTRATION OF TYPE HANDLERS ---");

// This line AUTOMATICALLY registers the Dapper TypeHandlers for all
// Domain Primitives in the assembly. Zero reflection — generated at compile time.
DapperDomainPrimitivesRegistration.RegisterAll();

Console.WriteLine("[Dapper] TypeHandlers registered for: CustomerId, ProductId, EmailAddress, ProductName, Price\n");

// ----------------------------------------------------------------------------
// 2. IN-MEMORY SQLite DATABASE
// ----------------------------------------------------------------------------
await using var connection = new SqliteConnection("Data Source=:memory:");
await connection.OpenAsync();

// Create table — columns use backend primitive types (TEXT, REAL)
await connection.ExecuteAsync("""
    CREATE TABLE Products (
        Id TEXT NOT NULL PRIMARY KEY,
        Name TEXT NOT NULL,
        Price REAL NOT NULL,
        OwnerEmail TEXT NOT NULL
    )
    """);

Console.WriteLine("--- 💾 INSERT PRODUCTS WITH DOMAIN PRIMITIVES ---");

// Create strongly typed primitives
var product1Id = ProductId.Create();
var product2Id = ProductId.Create();

// Dapper automatically maps ProductId → TEXT, Price → REAL using TypeHandlers
await connection.ExecuteAsync(
    "INSERT INTO Products (Id, Name, Price, OwnerEmail) VALUES (@Id, @Name, @Price, @OwnerEmail)",
    new
    {
        Id = product1Id,
        Name = ProductName.Create("Laptop Pro 15"),
        Price = Price.Create(1299.99m),
        OwnerEmail = EmailAddress.Create("alice@techstore.com")
    });

await connection.ExecuteAsync(
    "INSERT INTO Products (Id, Name, Price, OwnerEmail) VALUES (@Id, @Name, @Price, @OwnerEmail)",
    new
    {
        Id = product2Id,
        Name = ProductName.Create("Mechanical Keyboard"),
        Price = Price.Create(149.99m),
        OwnerEmail = EmailAddress.Create("bob@techstore.com")
    });

Console.WriteLine($"[Dapper] 2 products inserted.");
Console.WriteLine($"  Product 1: {product1Id} — Laptop Pro 15");
Console.WriteLine($"  Product 2: {product2Id} — Mechanical Keyboard\n");

// ----------------------------------------------------------------------------
// 3. QUERY: DAPPER CONVERTS BACK TO DOMAIN PRIMITIVES AUTOMATICALLY
// ----------------------------------------------------------------------------
Console.WriteLine("--- 🔍 QUERY WITH DOMAIN PRIMITIVES ---");

var products = await connection.QueryAsync<ProductRow>(
    "SELECT Id, Name, Price, OwnerEmail FROM Products");

foreach (var product in products)
{
    Console.WriteLine($"[Query] Id: {product.Id} | Name: {product.Name} | Price: {product.Price:F2} | Owner: {product.OwnerEmail}");

    // Verify they are real Domain Primitives (not loose strings)
    Console.WriteLine($"  → Id.IsDefault: {product.Id.IsDefault}");
    Console.WriteLine($"  → Price.Value: {product.Price.Value:F2}");
    Console.WriteLine($"  → Email domain: {product.OwnerEmail.Value.Split('@')[1]}");
}

// ----------------------------------------------------------------------------
// 4. FILTERING BY PRIMITIVE (TYPE-SAFE)
// ----------------------------------------------------------------------------
Console.WriteLine("\n--- 🔍 FILTER BY STRONGLY TYPED ID ---");

var found = await connection.QueryFirstOrDefaultAsync<ProductRow>(
    "SELECT Id, Name, Price, OwnerEmail FROM Products WHERE Id = @Id",
    new { Id = product1Id });

if (found is not null)
{
    Console.WriteLine($"[Found] {found.Name} — ${found.Price.Value:F2}");
}

// ----------------------------------------------------------------------------
// 5. ADVANTAGE: TYPE SAFETY IN QUERIES
// ----------------------------------------------------------------------------
Console.WriteLine("\n--- ✅ ADVANTAGES OF TYPE HANDLER ---");
Console.WriteLine("  ✓ Zero allocation: TypeHandlers use direct Set/Parse without boxing");
Console.WriteLine("  ✓ NativeAOT compatible: no hot reflection");
Console.WriteLine("  ✓ Type-safe: it is not possible to pass a ProductId where a CustomerId is expected");
Console.WriteLine("  ✓ Auto-discovery: a single DomainPrimitivesDapperTypeHandlers.Register()");
Console.WriteLine("  ✓ Works with all primitives: StrongId, StringPrimitive, NumericPrimitive");

Console.WriteLine("\n✅ CHAPTER 23 COMPLETED: Dapper Integration with Domain Primitives");


