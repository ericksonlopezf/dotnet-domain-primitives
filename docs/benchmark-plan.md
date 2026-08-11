# Benchmark Plan

> **Version:** 2.0 | **Date:** 2026-08-10  
> **Status:** CRITICAL — Without these benchmarks, all performance claims are marketing, not evidence.

---

## Overview

This plan defines the exact BenchmarkDotNet scenarios required to validate or invalidate every performance claim in the README. Results must be committed to `benchmarks/results/` and embedded in the README.

Until these benchmarks exist and are published:
- "High-performance" is **unverified**
- "Zero-allocation" is **partially false** (known: 1 unavoidable string alloc for NFC)
- "Allocation-free hot paths" is **unverified against competitors**

---

## Benchmark Configuration

```csharp
[Config(typeof(BenchmarkConfig))]
public class BenchmarkConfig : ManualConfig
{
    public BenchmarkConfig()
    {
        AddJob(Job.Default
            .WithRuntime(CoreRuntime.Core80)
            .WithRuntime(CoreRuntime.Core90));
        AddDiagnoser(MemoryDiagnoser.Default);
        AddColumn(StatisticColumn.P95);
        AddColumn(StatisticColumn.P99);
        // Optional: HardwareCounters
        // AddHardwareCounters(HardwareCounter.CacheMisses);
    }
}
```

**Mandatory settings:**
- **Frameworks:** `net8.0`, `net9.0` (add `net10.0` when available)
- **Build:** Release, optimized
- **MemoryDiagnoser:** REQUIRED (proves "zero-allocation" claim)
- **Columns:** Mean, Error, StdDev, Gen0, Gen1, Gen2, Allocated, P95

---

## Scenario 1: Create — Success Path

**Goal:** Prove TryCreate has zero heap allocation on success without normalization.

```csharp
[Benchmark(Baseline = true)]
public string RawString() => "john@example.com";

[Benchmark]
public bool VogenCreate() => VogenEmail.TryFrom("john@example.com").IsSuccess;

[Benchmark]
public bool DomainPrimitivesCreate()
    => Email.TryCreate("john@example.com", out _, out _);
```

**Expected results:**
- DP: `Allocated = ~0 B` on success path (NFC on pre-normalized input: still 1 string)
- Vogen: `Allocated = 0 B` (struct, no boxing)
- Raw: `Allocated = 0 B`

**Note:** Input `"john@example.com"` is already NFC-normalized, so DP's NFC alloc is `0 additional`.

---

## Scenario 2: Create — Failure Path (Invalid Input)

**Goal:** Validate out-pattern vs Result<T> allocation difference.

```csharp
[Benchmark]
public bool VogenCreateInvalid()
    => VogenEmail.TryFrom("NOT_AN_EMAIL").IsSuccess;

[Benchmark]
public bool DomainPrimitivesCreateInvalid()
    => Email.TryCreate("NOT_AN_EMAIL", out _, out _);
```

**Expected results:**
- DP: Allocated = 1 string (error message interpolation) — document this honestly
- Vogen: Allocated = 0 or 1 (depends on error model)

**Honest disclosure:** DP's failure path allocates one interpolated error string. Document as known limitation.

---

## Scenario 3: TryParse — ReadOnlySpan<char> — Simple (No Normalization)

**Goal:** Prove span parsing is allocation-minimized vs string parsing.

```csharp
private readonly string _emailString = "john@example.com";
private ReadOnlySpan<char> EmailSpan => _emailString.AsSpan();

[Benchmark]
public bool ParseString()
    => Email.TryParse(_emailString, null, out _);

[Benchmark]
public bool ParseSpan()
    => Email.TryParse(EmailSpan, null, out _);

[Benchmark]
public bool ParseUtf8()
    => Email.TryParse("john@example.com"u8, null, out _);
```

**Expected results:**
- ParseString: 0 new allocs (string already exists; NFC: 1 if not pre-normalized)
- ParseSpan: 1 alloc (NFC normalization)
- ParseUtf8: 1 alloc (decode + NFC)

---

## Scenario 4: TryParse — ReadOnlySpan<char> — With Case Normalization

**Goal:** Document the allocation cost of [LowerCase]/[UpperCase] on span path.

```csharp
// CountryCode has [UpperCase] — triggers MemoryExtensions.ToUpperInvariant path
private ReadOnlySpan<char> CountrySpan => "us".AsSpan();

[Benchmark]
public bool ParseCountrySpan()
    => CountryCode.TryParse(CountrySpan, null, out _);

[Benchmark]
public bool ParseCountryString()
    => CountryCode.TryParse("us", null, out _);
```

**Expected results:**
- ParseSpan: 1 alloc (stackalloc in-place ToUpperInvariant + NFC string)
- ParseString: 1-2 allocs (ToUpperInvariant on string + NFC)

**This benchmark tracks progress of CRIT-003 fix effectiveness.**

---

## Scenario 5: TryParse UTF-8 vs String — Large Input (>256 chars)

**Goal:** Document ArrayPool behavior for large inputs.

```csharp
private readonly string _longValue = new string('a', 300) + "@example.com";
private readonly byte[] _longUtf8 = Encoding.UTF8.GetBytes(new string('a', 300) + "@example.com");

[Benchmark]
public bool ParseLargeString()
    => Email.TryParse(_longValue, null, out _);

[Benchmark]
public bool ParseLargeUtf8()
    => Email.TryParse(_longUtf8, null, out _);
```

**Expected results:**
- ParseLargeString: 1-2 allocs
- ParseLargeUtf8: 1-2 allocs + ArrayPool rent (pool may or may not GC)

---

## Scenario 6: JSON Deserialization — STJ

**Goal:** Prove Utf8JsonReader.ValueSpan path is allocation-minimized vs string-based deserializers.

```csharp
private readonly byte[] _json = """{"email":"john@example.com"}"""u8.ToArray();

[Benchmark]
public VogenUserDto VogenDeserialize()
    => JsonSerializer.Deserialize<VogenUserDto>(_json)!;

[Benchmark]
public DPUserDto DomainPrimitivesDeserialize()
    => JsonSerializer.Deserialize<DPUserDto>(_json)!;
```

**Expected results:**
- DP: 1 alloc (NFC + storage string from ValueSpan) vs 2 allocs for Vogen (GetString() + any NFC)
- If ValueSpan path not implemented: equal to Vogen

**This benchmark validates or invalidates the Utf8JsonReader.ValueSpan claim.**

---

## Scenario 7: JSON Deserialization — Array of 1000 objects

**Goal:** Aggregate allocation difference under load.

```csharp
private readonly byte[] _jsonArray = /* 1000 user objects */;

[Benchmark]
public List<DPUserDto> DomainPrimitivesDeserializeArray()
    => JsonSerializer.Deserialize<List<DPUserDto>>(_jsonArray)!;

[Benchmark]
public List<VogenUserDto> VogenDeserializeArray()
    => JsonSerializer.Deserialize<List<VogenUserDto>>(_jsonArray)!;
```

**Metrics:** Mean, Gen0, Gen1, Gen2, Allocated

---

## Scenario 8: EF Core Materialization

**Goal:** Verify ValueConverter adds <2% overhead vs raw primitives.

```csharp
// EF Core in-memory roundtrip: save entity, read back
[Benchmark(Baseline = true)]
public RawUser EFCoreRaw() => _rawContext.Users.First();

[Benchmark]
public DomainUser EFCoreDomain() => _domainContext.Users.First();
```

**Expected results:**
- Overhead: <2% mean, 0 additional Gen0/Allocated

---

## Scenario 9: Dapper Materialization

```csharp
[Benchmark(Baseline = true)]
public RawUser DapperRaw()
    => _connection.QueryFirst<RawUser>("SELECT * FROM Users WHERE Id=1");

[Benchmark]
public DomainUser DapperDomain()
    => _connection.QueryFirst<DomainUser>("SELECT * FROM Users WHERE Id=1");
```

---

## Scenario 10: Strong ID Creation

```csharp
[Benchmark]
public VogenOrderId VogenStrongId() => VogenOrderId.From(Guid.NewGuid());

[Benchmark]
public OrderId DomainPrimitivesStrongId() => OrderId.Create(Guid.NewGuid());

[Benchmark]
public OrderId DomainPrimitivesTryCreate()
    => OrderId.TryCreate(Guid.NewGuid(), out var id, out _) ? id : default;
```

---

## Scenario 11: Smart Enum Lookup

```csharp
private readonly string _name = "Pending";
private readonly int _value = 1;

[Benchmark]
public OrderStatus DPByName() => OrderStatus.FromName(_name);
[Benchmark]
public OrderStatus DPByValue() => OrderStatus.FromValue(_value);

[Benchmark]
public Ardalis.SmartEnum.OrderStatus ArdalisByName()
    => Ardalis.SmartEnum.SmartEnum<Ardalis.SmartEnum.OrderStatus>.FromName(_name);
```

**Expected results:**
- DP: O(1) dictionary lookup in static readonly dict — 0 allocs
- Ardalis: reflection scan on first call; O(1) cache after

---

## Scenario 12: Value Object Creation — Multi-Property

```csharp
[Benchmark]
public Money DPMoney() => Money.Create("USD", 99.99m);

[Benchmark]
public THKMoney THKMoney()
    => new THKMoney.Money("USD", 99.99m); // THK pattern
```

---

## Scenario 13: Format to Span<char>

**Goal:** Prove ISpanFormattable is zero-allocation vs ToString().

```csharp
private readonly char[] _buffer = new char[100];
private readonly OrderId _id = OrderId.Create(Guid.NewGuid());

[Benchmark(Baseline = true)]
public string FormatToString() => _id.ToString();

[Benchmark]
public bool TryFormatToSpan()
    => _id.TryFormat(_buffer, out _, "D", null);
```

**Expected results:**
- ToString: 1 string alloc
- TryFormat: 0 allocs (writes into caller buffer)

---

## Scenario 14: ASP.NET Core Route Parameter Binding

```csharp
// Uses Microsoft.AspNetCore.TestHost
[Benchmark]
public async Task BindOrderId()
    => await _client.GetAsync($"/orders/{_guidId}");
```

---

## Scenario 15: Normalization — Before vs After CRIT-003 Fix

**Goal:** Document improvement from span-based normalization.

```csharp
// CountryCode requires UpperCase normalization
[Benchmark]
public bool CountryCodeFromString() => CountryCode.TryCreate("us", out _, out _);

[Benchmark]
public bool CountryCodeFromSpan() => CountryCode.TryParse("us".AsSpan(), null, out _);
```

---

## Scenario 16: Regex Validation Performance

**Goal:** Confirm NonBacktracking regex is competitive with standard compiled regex.

```csharp
// Email validation — regex-heavy
[Benchmark(Baseline = true)]
[Arguments("john@example.com")]
[Arguments("invalid-email")]
public bool DomainPrimitivesEmail(string input)
    => Email.TryCreate(input, out _, out _);
```

---

## Required Metrics per Benchmark

| Metric | Column | Purpose |
|:---|:---|:---|
| Mean | Mean | Primary performance indicator |
| Error | Error | Confidence interval |
| StdDev | StdDev | Stability indicator |
| Gen0 | Gen0 | Gen0 GC pressure |
| Gen1 | Gen1 | Gen1 GC pressure |
| Gen2 | Gen2 | Full GC indicator |
| Allocated | Allocated | **Primary zero-allocation proof** |
| P95 | StatisticColumn.P95 | Tail latency |

---

## Publishing Requirements

1. Run on: `net8.0` and `net9.0` minimum
2. Machine: Document CPU, OS, .NET version
3. Commit to: `benchmarks/results/YYYY-MM-DD/`
4. Embed summary table in README
5. Include raw `.json` BDN output for reproducibility

---

## Benchmark Competitor Matrix

| Benchmark | vs Raw | vs Vogen | vs Thinktecture | vs StronglyTypedId |
|:---|:---:|:---:|:---:|:---:|
| Create valid | ✅ required | ✅ required | optional | optional |
| Create invalid | ✅ required | ✅ required | optional | optional |
| TryParse span | ✅ required | ✅ required | optional | optional |
| TryParse UTF-8 | N/A | N/A | N/A | N/A (unique to DP) |
| JSON deserialize | optional | ✅ required | optional | ✅ required |
| EF Core | ✅ required | ✅ required | optional | optional |
| Dapper | ✅ required | ✅ required | N/A | optional |
| Strong ID create | ✅ required | ✅ required | optional | ✅ required |
| Smart Enum lookup | ✅ required | N/A | optional (THK) | N/A |
| Format to span | ✅ required | N/A | N/A | N/A (unique) |

---

## Success Criteria

The benchmarks must demonstrate:

1. `TryCreate` success path: **Allocated ≤ 0 B** (for non-normalized inputs)
2. `TryParse(ReadOnlySpan<char>)`: **Allocated ≤ 1 string** (NFC unavoidable)
3. `TryParse(ReadOnlySpan<byte>)`: **Allocated ≤ 1 string** (decode + NFC)
4. JSON deserialize: **Allocated ≤ Vogen** (ValueSpan path must be faster)
5. EF Core overhead: **≤ 2% vs raw**
6. Format to Span<char>: **Allocated = 0 B**

**If any criterion fails, the corresponding claim must be removed from the README until fixed.**
