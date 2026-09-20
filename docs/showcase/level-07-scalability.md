# Level 7 — Scalability and Performance

This level details memory allocation profiles, `Span<T>`-based processing, and throughput optimization techniques implemented in DomainPrimitives.

---

## 1. Memory Allocation Profile: Zero GC Allocations

Domain primitives are generated as **`readonly partial record struct`**. In contrast to traditional DDD class-based Value Objects:
- **Stack Residence**: When passed across local methods, they incur zero heap allocation.
- **Zero GC Pressure**: Millions of primitives can be created and destroyed per second without triggering garbage collection pauses (Gen0/Gen1).
- **Clean Success Path**: `TryCreate` returns the struct by value and a sentinel `PrimitiveError.None` with zero allocations.

---

## 2. Span-Based Ingestion with `ISpanParsable<T>`

Every generated primitive implements `ISpanParsable<TSelf>`, `IParsable<TSelf>`, and `IUtf8SpanParsable<TSelf>` (on .NET 8+), allowing parsing from character and UTF-8 byte buffers without allocating intermediate strings:

```csharp
ReadOnlySpan<char> buffer = "user@example.com".AsSpan();

if (CustomerEmail.TryParse(buffer, null, out var email))
{
    // Zero intermediate string allocations
    Console.WriteLine(email.Value);
}
```

---

## 3. Arithmetic Operator Optimization (`NumericOperations`)

For high-frequency numeric domains (e.g. real-time market pricing, high-speed billing engines), `[NumericPrimitive<TValue>]` allows emitting strongly-typed arithmetic operators (`+`, `-`, `*`, `/`) inlined by the JIT compiler:

```csharp
[NumericPrimitive<decimal>(Operations = NumericOperations.Additive)]
[PrimitiveRange(0, 1_000_000)]
public readonly partial record struct AccountBalance;

var balance1 = AccountBalance.Create(100.50m);
var balance2 = AccountBalance.Create(50.25m);

// Operator '+' is inlined by the JIT compiler with zero boxing
AccountBalance total = balance1 + balance2;
```

---

## Showcase Reference
- Executable project: [`14-Performance`](file:///d:/DevData/ericksonlopez.dev/dotnet-domain-primitives/samples/OfficialSample/14-Performance/Program.cs)
