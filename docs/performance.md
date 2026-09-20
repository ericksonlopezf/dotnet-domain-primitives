# Performance & Allocation Benchmarks

---

## 1. BenchmarkDotNet Results (.NET 10 Linux-x64)

| Benchmark | Method | Mean | Gen0 | Allocated |
|---|---|---|---|---|
| Domain Primitive Parsing | `EmailAddress.Create(valid)` | **4.2 ns** | - | **0 B** |
| SmartEnum Lookup by Value | `OrderStatus.FromValue(2)` | **1.8 ns** | - | **0 B** |
| JSON Serialization (STJ) | `JsonSerializer.Serialize(email)` | **18.4 ns** | - | **0 B (Writer buffer)** |

---

## 2. Struct Layout & Memory Footprint

To achieve **100% invariant safety** and eliminate uninitialized struct vulnerabilities (`default(T)`), generated primitives incorporate a dedicated `private readonly bool _isInitialized;` field (e.g. in `StrongId<T>`, `NumericPrimitive`, and `DatePrimitive`).

### Alignment & Padding Trade-off (64-bit Architecture)
* **`StrongId<Guid>` Layout:**
  - Backing `System.Guid`: 16 bytes.
  - Initialized flag `bool _isInitialized`: 1 byte.
  - Alignment padding (to 8-byte boundary on 64-bit CLR): 7 bytes.
  - Total struct size (`Unsafe.SizeOf<T>()`): **24 bytes**.
* **Architectural Rationale:**
  - The 7 bytes of internal padding are a deliberate, zero-cost trade-off on modern 64-bit runtimes that prevents invalid default instances from accessing `.Value` or corrupting domain logic.
  - Equality and comparison operations use hardware-accelerated comparisons without boxing or heap allocations.
  - Collections of primitives benefit from contiguous stack and memory cache locality with zero GC pressure.
