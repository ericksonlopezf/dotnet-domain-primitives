using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
// ============================================================================
// CHAPTER 14: PERFORMANCE OPTIMIZATIONS (ZERO ALLOCATION AND NATIVEAOT)
// ============================================================================
// In this chapter you will learn how the Domain Primitives architecture
// maximizes performance and minimizes pressure on the Garbage Collector (GC).
//
// PERFORMANCE KEYS IN .NET 10:
// 1. Zero Boxing: Use of `readonly record struct` and `IEquatable<T>` to avoid boxing in the Heap.
// 2. Allocation-free formatting: Implementation of `ISpanFormattable` and `IUtf8SpanFormattable`.
// 3. NativeAOT Ready: Zero hot reflection, optimized for AOT compilation.
// ============================================================================

using System.Diagnostics;
using Chapter14;
using EricksonLopez.DomainPrimitives;

Console.WriteLine("=========================================================");
Console.WriteLine(" 📘 CHAPTER 14: PERFORMANCE AND ZERO ALLOCATIONS (.NET 10)");
Console.WriteLine("=========================================================\n");

// ----------------------------------------------------------------------------
// 1. FORMATTING IN SPAN<CHAR> (ZERO HEAP ALLOCATION)
// ----------------------------------------------------------------------------
Console.WriteLine("--- ⚡ DIRECT FORMATTING IN SPAN<CHAR> (ZERO ALLOCATION) ---");

var customerId = CustomerId.Create();
Span<char> buffer = stackalloc char[36];

if (customerId.TryFormat(buffer, out int charsWritten, default, default))
{
    Console.WriteLine($"Formatted into stackalloc buffer: {buffer[..charsWritten]} ({charsWritten} characters)");
    Console.WriteLine("✅ Zero Heap allocations when converting to text.");
}

Console.WriteLine();

// ----------------------------------------------------------------------------
// 2. PERFORMANCE COMPARISON: CLASS VS RECORD STRUCT PRIMITIVE
// ----------------------------------------------------------------------------
Console.WriteLine("--- 🚀 SPEED TEST: STRUCT EQUALITY IN HOT PATH ---");

var id1 = CustomerId.Create();
var id2 = CustomerId.Create();

const int Iterations = 1_000_000;
var sw = Stopwatch.StartNew();

bool areEqual = false;
for (int i = 0; i < Iterations; i++)
{
    areEqual = (id1 == id2);
}
sw.Stop();

Console.WriteLine($"Comparisons executed: {Iterations:N0}");
Console.WriteLine($"Total elapsed time: {sw.ElapsedMilliseconds} ms ({sw.Elapsed.TotalMicroseconds / Iterations:F4} μs/op)");
Console.WriteLine($"Comparison result: {areEqual}");

Console.WriteLine("\nCHAPTER 14 COMPLETED SUCCESSFULLY.\n");


// ============================================================================
// DEFINITION OF PRIMITIVES AND TYPES
// ============================================================================

namespace Chapter14
{
    [StrongId<Guid>]
    public readonly partial record struct CustomerId;
}


