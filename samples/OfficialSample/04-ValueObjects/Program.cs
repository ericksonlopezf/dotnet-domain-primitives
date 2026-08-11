using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
// ============================================================================
// CHAPTER 04: VALUE OBJECTS AND STRUCTURAL EQUALITY
// ============================================================================
// In this chapter you will learn about Value Objects in DDD: types defined
// exclusively by the equality of their attributes, with no identity of their own.
//
// FUNDAMENTAL CHARACTERISTICS OF A VALUE OBJECT:
// 1. Strict Immutability: Once created, its values never change.
// 2. Structural Equality: Two addresses with the same street, city and zip code are EQUAL.
// 3. Self-validation: A Value Object cannot exist in an invalid state.
// 4. Replacement: Modifications generate a new instance instead of mutating the existing one.
// ============================================================================

using Chapter04;
using EricksonLopez.DomainPrimitives;
using EricksonLopez.DomainPrimitives.Validation;


Console.WriteLine("=========================================================");
Console.WriteLine(" 📘 CHAPTER 04: VALUE OBJECTS (STRUCTURAL EQUALITY)");
Console.WriteLine("=========================================================\n");

// ----------------------------------------------------------------------------
// 1. TRADITIONAL CODE (BEFORE) - ACCIDENTAL REFERENCE EQUALITY
// ----------------------------------------------------------------------------
Console.WriteLine("--- ❌ BEFORE (TRADITIONAL CLASSES COMPARED BY REFERENCE) ---");

var dirRef1 = new TraditionalAddress("Av. Reforma 100", "CDMX", "01000");
var dirRef2 = new TraditionalAddress("Av. Reforma 100", "CDMX", "01000");

Console.WriteLine($"dirRef1 == dirRef2: {dirRef1 == dirRef2} ❌ (They are different objects in memory)");
Console.WriteLine($"dirRef1.Equals(dirRef2): {dirRef1.Equals(dirRef2)} ❌ (Missing manual override of Equals/GetHashCode)");

Console.WriteLine();

// ----------------------------------------------------------------------------
// 2. CODE WITH DOMAIN PRIMITIVES (AFTER) - SOURCE GENERATED VALUE OBJECTS
// ----------------------------------------------------------------------------
Console.WriteLine("--- ✅ AFTER (VALUE OBJECTS WITH AUTOMATIC STRUCTURAL EQUALITY) ---");

var isValid1 = Address.TryCreate("Av. Reforma 100", "CDMX", "01000", out var address1, out var error1);
var isValid2 = Address.TryCreate("Av. Reforma 100", "CDMX", "01000", out var address2, out var error2);

if (isValid1 && isValid2)
{
    Console.WriteLine($"address1 == address2: {address1 == address2} ✅ (Exact structural comparison)");
    Console.WriteLine($"address1.Equals(address2): {address1.Equals(address2)} ✅ (Zero boxing in hot paths)");
    Console.WriteLine($"Identical HashCode: {address1.GetHashCode() == address2.GetHashCode()} ✅");
    Console.WriteLine($"Generated ToString(): {address1}");
}

Console.WriteLine("\n--- 🛡️ STRUCTURAL VALIDATOR AND REPLACEMENT ---");

var isInvalidValid = Address.TryCreate("", "CDMX", "01000", out var invalidAddress, out var invalidError);
if (!isInvalidValid)
{
    Console.WriteLine($"❌ Validation Blocked Invalid Address: {invalidError.Message}");
}

var originalAddress = Address.Create("Calle Luna 45", "Madrid", "28001");
Console.WriteLine($"Original: {originalAddress}");

// Instead of mutating properties (which would violate DDD), we create a new modified version:
var updatedAddress = Address.Create("Calle Luna 46", originalAddress.City, originalAddress.ZipCode);
Console.WriteLine($"New version: {updatedAddress}");

Console.WriteLine("\nCHAPTER 04 COMPLETED SUCCESSFULLY.\n");
Chapter04.BuiltIn.BuiltInPrimitivesDemo.Run();

// ============================================================================
// TYPE DEFINITIONS (BEFORE vs AFTER)
// ============================================================================

// Before: Traditional mutable class with defective reference equality
public class TraditionalAddress
{
    public string Street { get; set; }
    public string City { get; set; }
    public string ZipCode { get; set; }

    public TraditionalAddress(string street, string city, string zipCode)
    {
        Street = street;
        City = city;
        ZipCode = zipCode;
    }
}

// After: Source-Generated Value Object (Fully immutable and validated via partial hook)
namespace Chapter04
{
    [ValueObject]
    public readonly partial record struct Address
    {
        public string Street { get; init; }
        public string City { get; init; }
        public string ZipCode { get; init; }

        static partial void Validate(ref Address address, ref PrimitiveError error)
        {
            if (string.IsNullOrWhiteSpace(address.Street))
                error = new PrimitiveError("Address.StreetRequired", "The street cannot be empty.");
            else if (string.IsNullOrWhiteSpace(address.City))
                error = new PrimitiveError("Address.CityRequired", "The city cannot be empty.");
            else if (string.IsNullOrWhiteSpace(address.ZipCode))
                error = new PrimitiveError("Address.ZipCodeRequired", "The zip code cannot be empty.");
        }
    }
}


