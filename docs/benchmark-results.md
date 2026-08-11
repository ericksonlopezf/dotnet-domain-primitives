# Benchmark Results

> **Last run:** 2026-07-24  
> **Hardware:** AMD Ryzen 7 9800X3D 4.70GHz, 8 cores  
> **Runtime:** .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v4  
> **BenchmarkDotNet:** v0.15.8  
> **Raw results:** [`DomainPrimitiveBenchmarks-report-github.md`](../benchmarks/EricksonLopez.DomainPrimitives.Benchmarks/BenchmarkDotNet.Artifacts/results/EricksonLopez.DomainPrimitives.Benchmarks.DomainPrimitiveBenchmarks-report-github.md)

---

## Key Performance Results

### Hot Path Benchmark Summary

| Benchmark | Mean | Allocated | Zero-alloc? |
|-----------|------|-----------|-------------|
| `RawGuid` (baseline — no wrapper) | 0.00 ns | 0 B | ✅ |
| `PrimitiveGuid.Create(Guid)` | 0.00 ns | 0 B | ✅ **Same as raw** |
| `PrimitiveGuid.TryParse(string)` | 12.63 ns | 0 B | ✅ **Zero allocation** |
| `EmailAddress.Create(string)` | 49.53 ns | 0 B | ✅ **Zero allocation** |
| `EmailAddress` JSON serialize | 102.34 ns | 64 B | ⚠️ JSON always allocates |
| `EmailAddress` JSON deserialize | 95.58 ns | 120 B | ⚠️ JSON always allocates |

> **Note:** JSON allocation is from the `Utf8JsonReader`/`Utf8JsonWriter` infrastructure, not from the domain primitive itself. The `TryParse` hot path (called internally during deserialization) is zero-allocation.

### Key Claims Verified

| Claim | Status | Evidence |
|-------|--------|---------|
| `Create(Guid)` — **zero allocation** | ✅ VERIFIED | `Allocated: -` |
| `TryParse(string)` — **zero allocation** | ✅ VERIFIED | `Allocated: -` |
| `Create(string)` with normalization — **zero allocation** (Email) | ✅ VERIFIED | `Allocated: -` |
| Comparable to raw primitive performance | ✅ VERIFIED | PrimitiveGuid ≈ RawGuid |

---

## Running Benchmarks

```bash
cd benchmarks/EricksonLopez.DomainPrimitives.Benchmarks
dotnet run -c Release --framework net9.0 -- --filter "*" --exporters json markdown
```

Results are written to `BenchmarkDotNet.Artifacts/results/`. Copy them to `benchmarks/results/` for archival.

---

## Comparative Results vs. Competitors

The following results compare `[StrongId<Guid>]` against raw `System.Guid` and popular industry alternatives.

### Creation and Parsing

| Method | Create (Mean) | Parse (Mean) | Allocated |
|--------|--------------:|-------------:|----------:|
| **Raw Guid** (baseline) | 0.00 ns | 15.32 ns | **0 B** |
| **DomainPrimitives** | **0.17 ns** | **15.81 ns** | **0 B** |
| StronglyTypedId | 0.00 ns | 15.29 ns | **0 B** |
| Vogen | 0.01 ns | 15.22 ns | **0 B** |
| TinyTypes | 0.00 ns | 15.31 ns | **0 B** |
| Meziantou | 0.00 ns | 15.43 ns | **0 B** |
| ValueOf | 2.51 ns | 16.99 ns | 32 B |

### `ToString()` Performance

| Method | Mean | Allocated |
|--------|-----:|----------:|
| **Raw Guid** (baseline) | 6.11 ns | 96 B |
| **DomainPrimitives** | **6.30 ns** | **96 B** |
| Vogen | 7.10 ns | 96 B |
| StronglyTypedId | 7.20 ns | 96 B |
| ValueOf | 9.30 ns | 128 B |
| Meziantou | 15.25 ns | 248 B |

**Takeaways:**
- Struct-based generators (DomainPrimitives, Vogen, StronglyTypedId) all achieve identical performance and zero-allocation in `Create` and `Parse`, indistinguishable from raw `Guid`.
- Class-based wrappers like `ValueOf` incur heap allocations (32 B per instance).
- DomainPrimitives slightly outperforms competitors in `ToString()` (6.30 ns vs 7.10 ns).

---

## Expected Allocations by Scenario

Per the engineering spec §ZERO ALLOCATION IN HOT PATH:

| Scenario | Expected | Pass/Fail |
|----------|---------|-----------|
| `Create(T)` with validation | **0 B** | ✅ Verified (PrimitiveGuid, EmailAddress) |
| `TryCreate(T, ...)` success | **0 B** | ✅ Verified (PrimitiveGuid) |
| `TryParse(string, ...)` success | **0 B** | ✅ Verified |
| `TryParse(ReadOnlySpan<char>)` ≤256 chars | **0 B** (stackalloc) | ✅ Per code review |
| `TryParse(ReadOnlySpan<char>)` >256 chars | **0 B** (ArrayPool) | ✅ Per code review |
| `TryParse(ReadOnlySpan<byte>)` ≤256 chars | **0 B** (stackalloc) | ✅ Per code review |
| `TryFormat(Span<char>)` | **0 B** | ✅ Per code review |
| JSON serialize | **64 B** | ✅ Expected (JSON infra) |
| JSON deserialize | **120 B** | ✅ Expected (JSON infra) |

---

## Regression Gate

Per §ANTI-REGRESSION BENCHMARKS, a 10% performance regression in any hot-path benchmark
must block the release. The CI job runs `--filter "*" --memory` and compares against the
last published results.

**Current regression gate status:** Partial — benchmark CI job exists but threshold comparison
is manual. Automated threshold gate is a v1.3.0 target.

---

## Historical Results

| Date | Runtime | Hardware | Report |
|------|---------|----------|--------|
| 2026-07-24 | .NET 10.0.10 | AMD Ryzen 7 9800X3D | [DomainPrimitiveBenchmarks (partial)](../benchmarks/EricksonLopez.DomainPrimitives.Benchmarks/BenchmarkDotNet.Artifacts/results/) |

> More runs needed for trend analysis. Target: monthly runs before each minor release.
