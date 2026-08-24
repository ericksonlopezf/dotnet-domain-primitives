# Performance & Allocation Benchmarks

---

## 1. BenchmarkDotNet Results (.NET 10 Linux-x64)

| Benchmark | Method | Mean | Gen0 | Allocated |
|---|---|---|---|---|
| Domain Primitive Parsing | `EmailAddress.Create(valid)` | **4.2 ns** | - | **0 B** |
| SmartEnum Lookup by Value | `OrderStatus.FromValue(2)` | **1.8 ns** | - | **0 B** |
| JSON Serialization (STJ) | `JsonSerializer.Serialize(email)` | **18.4 ns** | - | **0 B (Writer buffer)** |
