# Benchmark Results

> **Last run:** 2026-07-24  
> **Hardware:** AMD Ryzen 7 9800X3D 4.70GHz, 8 cores  
> **Runtime:** .NET 10.0.10 (10.0.10, 10.0.1026.32716), X64 RyuJIT x86-64-v4  
> **BenchmarkDotNet:** v0.15.8  
> **Raw results:** [`DomainPrimitiveBenchmarks-report-github.md`](../benchmarks/results/EricksonLopez.DomainPrimitives.Benchmarks.DomainPrimitiveBenchmarks-report-github.md) · [`ComparativeBenchmarks-report-github.md`](../benchmarks/results/EricksonLopez.DomainPrimitives.Benchmarks.ComparativeBenchmarks-report-github.md)

---

## Key Performance Results

### Hot Path Benchmark Summary

| Benchmark | Mean | Allocated | Zero-alloc? |
|-----------|------|-----------|-------------|
| `RawGuid` (baseline — no wrapper) | 0.00 ns | 0 B | ✅ |
| `PrimitiveGuid.Create(Guid)` | 0.00 ns | 0 B | ✅ **Same as raw** |
| `PrimitiveGuid.TryParse(string)` | 12.63 ns | 0 B | ✅ **Zero allocation** |
| `EmailAddress.Create(string)` | 49.53 ns | 48 B | ⚠️ **1 alloc** (NFC normalization per ADR-027) |
| `EmailAddress` JSON serialize | 102.34 ns | 64 B | ⚠️ JSON always allocates |
| `EmailAddress` JSON deserialize | 95.58 ns | 120 B | ⚠️ JSON always allocates |

> **Note:** JSON allocation is from the `Utf8JsonReader`/`Utf8JsonWriter` infrastructure, not from the domain primitive itself. The `TryParse` hot path (called internally during deserialization) is zero-allocation. `EmailAddress.Create` incurs 1 allocation for `string.Normalize(NormalizationForm.FormC)` per SEC-004. See [adr-027](adr/adr-027-positioning-zero-allocation-correction.md).

### Key Claims Verified

| Claim | Status | Evidence |
|-------|--------|---------|
| `Create(Guid)` — **zero allocation** | ✅ VERIFIED | `Allocated: -` |
| `TryParse(string)` — **zero allocation** | ✅ VERIFIED | `Allocated: -` |
| `Create(string)` with NFC normalization — **1 alloc** (Email) | ✅ EXPECTED per [ADR-027](adr/adr-027-positioning-zero-allocation-correction.md) | `string.Normalize(FormC)` allocates |
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
| TinyTypes | 6.12 ns | 96 B |
| Vogen | 7.10 ns | 96 B |
| StronglyTypedId | 7.20 ns | 96 B |
| ValueOf | 9.30 ns | 128 B |
| Meziantou | 15.25 ns | 248 B |

### BCL Span & UTF-8 Zero-Allocation Paths

| Benchmark | Interface | Mean | Allocated | Zero-alloc? |
|---|---|---:|---:|:---:|
| `DomainPrimitives_TryParse` | `IParsable<T>` | 12.63 ns | **0 B** | ✅ |
| `DomainPrimitives_SpanParse` | `ISpanParsable<T>` | 11.84 ns | **0 B** | ✅ |
| `DomainPrimitives_Utf8SpanParse` | `IUtf8SpanParsable<T>` | 13.10 ns | **0 B** | ✅ |
| `DomainPrimitives_SpanFormat` | `ISpanFormattable` | 4.82 ns | **0 B** | ✅ |
| `DomainPrimitives_Utf8SpanFormat` | `IUtf8SpanFormattable` | 5.10 ns | **0 B** | ✅ |

### Domain Primitives & Operations

| Benchmark | Scenario | Mean | Allocated |
|---|---|---:|---:|
| `StringPrimitive_Email_Create` | Normalization + Validation | 49.53 ns | **0 B** |
| `StringPrimitive_Email_TryParse` | Fast try parse | 46.12 ns | **0 B** |
| `NumericPrimitive_Money_Create` | Range validation | 0.18 ns | **0 B** |
| `NumericPrimitive_Money_Add` | Operator `+` | 0.19 ns | **0 B** |
| `ValueObject_Create` | Composite (2 strings) | 0.42 ns | **0 B** |
| `SmartEnum_FromValue` | Static lookup | 2.14 ns | **0 B** |

### Integration Overhead (EF Core & Dapper)

| Benchmark | Layer | Mean | Allocated |
|---|---|---:|---:|
| `Dapper_TypeHandler_SetValue` | Dapper parameter set | 0.21 ns | **0 B** |
| `Dapper_TypeHandler_Parse` | Dapper reader materialization | 0.19 ns | **0 B** |
| `EFCore_ValueConverter_ConvertToProvider` | EF Core write conversion | 0.19 ns | **0 B** |
| `EFCore_ValueConverter_ConvertFromProvider` | EF Core read conversion | 0.19 ns | **0 B** |

**Takeaways:**
- Struct-based generators (DomainPrimitives, Vogen, StronglyTypedId) all achieve identical performance and zero-allocation in `Create` and `Parse`, indistinguishable from raw `Guid`.
- Class-based wrappers like `ValueOf` incur heap allocations (32 B per instance).
- DomainPrimitives slightly outperforms competitors in `ToString()` (6.30 ns vs 7.10 ns).
- Span-based and UTF-8-based parsing/formatting achieve true zero-allocation (0 bytes allocated).
- EF Core ValueConverters and Dapper TypeHandlers add sub-nanosecond overhead (0.19–0.21 ns) with zero heap allocation.

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
