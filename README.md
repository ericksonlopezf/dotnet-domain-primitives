# EricksonLopez.DomainPrimitives 🛡️

[![NuGet](https://img.shields.io/nuget/v/EricksonLopez.DomainPrimitives?style=for-the-badge&logo=nuget&logoColor=white&color=512BD4)](https://www.nuget.org/packages/EricksonLopez.DomainPrimitives)
[![NuGet Downloads](https://img.shields.io/nuget/dt/EricksonLopez.DomainPrimitives?style=for-the-badge&logo=nuget&logoColor=white&color=004880)](https://www.nuget.org/packages/EricksonLopez.DomainPrimitives)
[![CI](https://img.shields.io/github/actions/workflow/status/ericksonlopezf/dotnet-domain-primitives/ci.yml?branch=main&style=for-the-badge&logo=githubactions&logoColor=white&label=CI)](https://github.com/ericksonlopezf/dotnet-domain-primitives/actions)
[![Coverage](https://img.shields.io/codecov/c/github/ericksonlopezf/dotnet-domain-primitives?style=for-the-badge&logo=codecov&logoColor=white)](https://codecov.io/gh/ericksonlopezf/dotnet-domain-primitives)
[![Mutation Score](https://img.shields.io/badge/Mutation_Score-%E2%89%A595%25-brightgreen?style=for-the-badge&logo=stryker&logoColor=white)](stryker-config.json)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=for-the-badge)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET_8_%7C_9_%7C_10-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com)
[![NativeAOT](https://img.shields.io/badge/NativeAOT-Compatible-brightgreen?style=for-the-badge)](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot)

**DomainPrimitives** is a BCL-native, AOT-first domain primitive library for .NET 8+. It uses Roslyn Incremental Source Generators to produce strictly valid, immutable domain types with the deepest BCL interface coverage in the .NET ecosystem — including `IUtf8SpanParsable<T>`, `ISpanFormattable`, and `IUtf8SpanFormattable`.

## 🆚 Why DomainPrimitives?

| Capability | DomainPrimitives | Vogen | Thinktecture | StronglyTypedId |
|:---|:---:|:---:|:---:|:---:|
| `IUtf8SpanParsable<T>` generated (NET8+) | ✅ | ❌ | ❌ | ❌ |
| `ISpanFormattable` generated | ✅ | ❌ | — | ❌ |
| `IUtf8SpanFormattable` generated | ✅ | ❌ | ❌ | ❌ |
| Declarative normalization ([Trim], [LowerCase]...) | ✅ | ❌ | ❌ | ❌ |
| NFC Unicode normalization (SEC-004) | ✅ | ❌ | ❌ | ❌ |
| ReDoS-resistant regex (NonBacktracking + 100ms) | ✅ | ❌ | ❌ | ❌ |
| 30 semantic domain type shortcuts | ✅ | ❌ | ❌ | ❌ |
| Auto-discovered EF Core & Dapper (no annotations) | ✅ | ❌ | ❌ | ❌ |
| Multi-property Value Object | ✅ | ❌ | ✅ | ❌ |
| Smart Enum (source-generated, AOT-safe) | ✅ | ❌ | ✅ | ❌ |
| `TryCreate(out result, out error)` (zero-alloc success) | ✅ | ❌ | ❌ | ❌ |
| Native AOT compatible | ✅ | ✅ | ✅ | ✅ |

> **Not in DomainPrimitives yet:** Discriminated Unions (Thinktecture only), Newtonsoft.Json converters (Vogen/StronglyTypedId), class-based primitives. See [docs/feature-gaps.md](docs/feature-gaps.md) for the full gap list.

## 📦 Supported Primitives

| Type | Description | Example |
|------|-------------|---------|
| `[StringPrimitive]` | String-backed primitive with validation pipeline. | `FirstName`, `Description` |
| `[NumericPrimitive<T>]`| Numeric-backed (int, decimal, double, etc.). | `Age`, `Money`, `Score` |
| `[DatePrimitive]` | Date-backed (DateOnly, DateTime). | `BirthDate`, `ExpirationDate` |
| `[StrongId]` | Strongly-typed IDs (Guid, int, long, string). | `UserId`, `OrderId` |
| `[ValueObject]` | Multi-property immutable value objects. | `Address`, `Money` |
| `[SmartEnum]` | Strongly-typed enums with behavior and AOT-safe static list. | `OrderStatus`, `Role` |

## 🎯 Semantic Shortcut Attributes

Instead of repeating validations, use built-in shortcut attributes that combine validation and normalization rules:

**String shortcuts (15 types):**
- **Identity**: `[Email]`, `[Username]`, `[PasswordHash]`
- **Network**: `[Url]`, `[IPAddress]`, `[MacAddress]`
- **Commerce**: `[Phone]`, `[CountryCode]`, `[CurrencyCode]`, `[LanguageCode]`, `[IBAN]`
- **Content**: `[Slug]`, `[HexColor]`, `[ISBN]`, `[VIN]`

**Numeric shortcuts (15 types):**
`[Money]`, `[Percentage]`, `[Latitude]`, `[Longitude]`, `[Age]`, `[Weight]`, `[Height]`, `[Distance]`, `[Temperature]`, `[Score]`, `[Quantity]`, `[Price]`, `[TaxRate]`, `[Discount]`, `[Rating]`

## ⚡ Quick Start

```bash
dotnet add package EricksonLopez.DomainPrimitives
```

```csharp
using EricksonLopez.DomainPrimitives;

// 1. Define your primitive
[CountryCode] // Implies: [StringPrimitive], [Trim], [UpperCase], [Length(2, 2)]
public readonly partial record struct CountryIsoCode;

// 2. Create — throws DomainPrimitiveValidationException on invalid input
var code = CountryIsoCode.Create("  us  "); // Value is "US" (trimmed + uppercased)

// 3. TryCreate — out-based, zero allocation on success
if (CountryIsoCode.TryCreate("us", out var validCode, out var error))
    Console.WriteLine($"Valid: {validCode}");
else
    Console.WriteLine($"Error [{error.Code}]: {error.Message}");

// 4. Parse from Span<char> — allocation-minimized path
if (CountryIsoCode.TryParse("us".AsSpan(), null, out var parsedCode))
    Console.WriteLine($"Parsed: {parsedCode}");

// 5. Parse from UTF-8 bytes — native for HTTP/gRPC/Kafka scenarios (NET8+)
ReadOnlySpan<byte> utf8 = "us"u8;
if (CountryIsoCode.TryParse(utf8, null, out var utf8Code))
    Console.WriteLine($"UTF-8 parsed: {utf8Code}");
```

## 🔐 Security Gates

DomainPrimitives is the only domain primitive library with built-in security gates applied automatically to all string types:

| Gate | Rule | Protection |
|------|------|-----------|
| **SEC-001** | Default 4096-character limit on all string types without explicit `MaxLength` | Prevents memory exhaustion attacks |
| **SEC-002** | `RegexOptions.NonBacktracking` on .NET 7+ | Eliminates ReDoS vulnerabilities |
| **SEC-003** | 100ms regex timeout on older TFMs | Caps worst-case regex time |
| **SEC-004** | NFC Unicode normalization on all string inputs before validation | Prevents Unicode homoglyph attacks |
| **SEC-005** | No PII echoed in error messages on sensitive types | Prevents information leakage |
| **SEC-006** | Stackalloc ≤ 256 chars on char-span path; ≤ 256 **bytes** on UTF-8 byte path; `ArrayPool<char>` for larger inputs | Prevents stack overflow on large inputs |

## ⚡ Performance & Benchmarks

> BenchmarkDotNet v0.15.8 · .NET 10.0.10 (10.0.1026.32716) · AMD Ryzen 7 9800X3D

DomainPrimitives is built with extreme performance and **zero-allocation** in mind. See the [full benchmark methodology and results](docs/benchmark-results.md).

### Hot Path Benchmarks

| Benchmark | Mean | Allocated | Zero-alloc? |
|-----------|------|-----------|-------------|
| `RawGuid` (baseline — no wrapper) | 0.00 ns | 0 B | ✅ |
| `PrimitiveGuid.Create(Guid)` | 0.00 ns | 0 B | ✅ **Same as raw** |
| `PrimitiveGuid.TryParse(string)` | 12.63 ns | 0 B | ✅ **Zero allocation** |
| `EmailAddress.Create(string)` | 49.53 ns | 0 B | ✅ **Zero allocation** |
| `EmailAddress` JSON serialize | 102.34 ns | 64 B | ⚠️ JSON infra |
| `EmailAddress` JSON deserialize | 95.58 ns | 120 B | ⚠️ JSON infra |

> **Note:** JSON allocation is from the `Utf8JsonReader`/`Utf8JsonWriter` infrastructure, not from the domain primitive itself. The `TryParse` hot path (called internally during deserialization) is zero-allocation.

### vs. Industry Competitors (StrongId<Guid>)

| Method | Create | Parse | Allocated |
|:---|---:|---:|---:|
| **Raw Guid** (baseline) | 0.00 ns | 15.32 ns | **0 B** |
| **DomainPrimitives** | **0.17 ns** | **15.81 ns** | **0 B** |
| Vogen | 0.01 ns | 15.22 ns | **0 B** |
| StronglyTypedId | 0.00 ns | 15.29 ns | **0 B** |
| ValueOf | 2.51 ns | 16.99 ns | 32 B |

*Results show DomainPrimitives maintains zero-allocation in hot paths and performs virtually identically to raw `Guid` and other struct-based generators, while avoiding the heap allocation overhead seen in class-based wrappers (e.g., ValueOf).*

### Allocation Model Audit

DomainPrimitives minimizes heap allocations in hot paths. Here is the honest per-path allocation audit:

| Path | Allocations | Notes |
|:---|:---:|:---|
| `TryCreate(string)` — success, no normalization | **0** | Zero new heap objects |
| `TryCreate(string)` — success, with normalization | **1** | NFC `.Normalize(FormC)` — required by SEC-004 |
| `TryCreate(string)` — failure | **1** | Error message string |
| `TryParse(ReadOnlySpan<char>)` ≤ 256 chars | **1** | stackalloc + 1 string for NFC + storage |
| `TryParse(ReadOnlySpan<char>)` > 256 chars | **1 + pool** | `ArrayPool<char>` + 1 string |
| `TryParse(ReadOnlySpan<byte>)` ≤ 256 chars (NET8+) | **1** | stackalloc decode + 1 string |
| JSON deserialize via `Utf8JsonReader.ValueSpan` | **1** | Direct span read + 1 string for NFC + storage |
| `TryFormat(Span<char>)` — formatting | **0** | Writes into caller-provided span |
| EF Core materialization (struct types) | **0** | `ValueConverter` — no boxing for structs |

> **Why 1 unavoidable allocation?** Unicode NFC normalization (SEC-004) requires producing a `System.String` — normalization can change character count (combining characters → composed), so the result cannot be stored as a span. The stored domain value is always NFC-normalized, which is correct and prevents homoglyph attacks.

> **No Result\<T\> overhead.** `TryCreate(out result, out error)` is zero-allocation on the success path because both `result` (a struct) and `error` (a struct) live on the caller's stack. Unlike `Result<T>` wrapper patterns, no heap object is created.

## 🧩 Ecosystem Integrations

DomainPrimitives provides seamless integration via dedicated packages. Converters are **auto-discovered** — no per-type attributes needed in your domain layer:

| Package | Integration | Auto-discovery |
|---------|-------------|:-:|
| `EricksonLopez.DomainPrimitives.AspNetCore` | Model binding, route params, OpenAPI | ✅ |
| `EricksonLopez.DomainPrimitives.EFCore` | `ValueConverter` for all domain types | ✅ |
| `EricksonLopez.DomainPrimitives.Dapper` | `SqlMapper.TypeHandler` for all domain types | ✅ |
| `EricksonLopez.DomainPrimitives.Mapster` | Mapster type mapping for composite ValueObjects (source-generated) | ✅ |
| `EricksonLopez.DomainPrimitives.OpenApi` | Swagger/OpenAPI schema filters | ✅ |
| `EricksonLopez.DomainPrimitives.Testing` | Assertions, builders, fakes for xUnit | — |

> **Known gap:** Newtonsoft.Json converters are not yet supported. If your project uses Newtonsoft.Json, consider [Vogen](https://github.com/SteveDunn/Vogen) or [StronglyTypedId](https://github.com/andrewlock/StronglyTypedId) for this scenario.

> **Mapster note:** For **scalar primitives** (`[StringPrimitive]`, `[StrongId]`, `[NumericPrimitive<T>]`), Mapster resolves the generated `explicit operator` automatically — **no package needed**. Add `EricksonLopez.DomainPrimitives.Mapster` only when mapping **composite `[ValueObject]` types** or when using Mapster in AOT source-generation mode. See [ADR-017](docs/adr/ADR-017-mapster-integration-rationale.md).

## 🏗️ Architecture

### Target Framework Requirements

| Package | Minimum TFM | Notes |
|---------|-------------|-------|
| `EricksonLopez.DomainPrimitives` | **net8.0** | Full feature set: generators, analyzers, integrations. |
| `EricksonLopez.DomainPrimitives.Abstractions` | **netstandard2.0** | Attributes, interfaces, and `PrimitiveError` only. No generators. |
| `EricksonLopez.DomainPrimitives.Generators` | **netstandard2.0** | Source Generator — compile-time only, no runtime reference needed. |

> **Minimum runtime: net8.0.** `IUtf8SpanParsable<T>`, `RegexOptions.NonBacktracking`, `System.Buffers.ArrayPool<T>`, and `MemoryExtensions.ToLowerInvariant` all require .NET 8+. Use `EricksonLopez.DomainPrimitives.Abstractions` for shared contracts in netstandard2.0 projects.  
> **Primary development target: net10.0 LTS.** All benchmarks and new feature development target NET 10. See [ADR-016](docs/adr/ADR-016-target-runtime-primary-vs-minimum.md) for the full rationale.

### Native AOT

All generated code is AOT-compatible:
- Zero reflection in hot paths (`Type.GetMethod()`, `Activator`, `Expression<>` are never used)
- `SmartEnum.GetAll()` is a static readonly array — no runtime reflection
- `IsAotCompatible=true` in project metadata
- CI gate: `dotnet publish` with Native AOT verifies compatibility on every commit

### Generated BCL Interfaces

For every domain primitive, the generator emits:

| Interface | String | Numeric | Date | StrongId | ValueObject |
|:---|:---:|:---:|:---:|:---:|:---:|
| `IParsable<T>` | ✅ | ✅ | ✅ | ✅ | 🔜 v2.0 |
| `ISpanParsable<T>` | ✅ | ✅ | ✅ | ✅ | 🔜 v2.0 |
| `IUtf8SpanParsable<T>` (NET8+) | ✅ | ✅ | ✅ | ✅ | 🔜 v2.0 |
| `IFormattable` | ✅ | ✅ | ✅ | ✅ | ✅ |
| `ISpanFormattable` | ✅ | ✅ | ✅ | ✅ | ✅ |
| `IUtf8SpanFormattable` (NET8+) | ✅ | ✅ | ✅ | ✅ | 🔜 v2.0 |
| `IComparable<T>` | ✅ | ✅ | ✅ | ✅ | N/A* |
| `IEqualityOperators<T,T,bool>` | ✅ | ✅ | ✅ | ✅ | ✅ |

> *`IComparable<T>` on ValueObject is intentionally not generated — composite types have no canonical ordering unless the domain explicitly defines one.

## 📚 Documentation

- 🛡️ [**Security Gates**](docs/security.md) — SEC-001 through SEC-006 explained
- 🗺️ [**Feature Gaps**](docs/feature-gaps.md) — What's missing and what we explicitly reject
- 🍳 [**Cookbook**](docs/cookbook.md) — Common problems solved with DomainPrimitives
- 📖 [**API Reference**](docs/api-reference.md) — Interfaces, exceptions, and factory methods
- 📦 [**Packages**](docs/packages.md) — All 15 NuGet packages, TFM matrix, and dependency graph
- 🔄 [**Migration from Vogen**](docs/migration/from-vogen.md) — Step-by-step migration guide
- 🔄 [**Migration from StronglyTypedId**](docs/migration/from-stronglytypedid.md) — Step-by-step migration guide
- 📊 [**Benchmark Results**](docs/benchmark-results.md) — Performance data and allocation audit
- 📊 [**Benchmark Plan**](docs/benchmark-plan.md) — 16 BenchmarkDotNet scenarios
- 🏗️ [**System Overview**](docs/system-overview.md) — Architecture and project dependency diagram
- ⚙️ [**CI/CD Pipelines**](docs/ci-cd-pipelines.md) — Build, test, quality gates, and supply chain security
- 🗺️ [**Roadmap**](ROADMAP.md) — NOW / NEXT / LATER horizon planning
- 📝 [**Changelog**](CHANGELOG.md) — All notable changes per version

## 🚀 Sample Projects

The `samples/OfficialSample/` folder contains step-by-step integration examples:
- `1-GettingStarted`: Core concepts and Quick Start
- `4-ValueObjects`: Structural validation and built-in primitive catalog
- `5-StronglyTypedIds`: Eliminating Primitive Obsession
- `15-AspNetCoreIntegration`: ASP.NET Core, HTTP validation and JSON
- `16-EFCoreIntegration`: Domain persistence with EF Core
- `17-MediatRIntegration`: Advanced pipeline behavior integration
- `20-EndToEndApplication`: Enterprise architecture (scalability, observability, error handling)
- `23-DapperIntegration`: Dapper TypeHandlers with auto-discovery (`DapperDomainPrimitivesRegistration.RegisterAll()`)
- `24-OpenApiIntegration`: Swagger/OpenAPI schema filters — domain primitives shown as correct JSON types

## 🤝 Contributing & Community

- [Contributing Guide](CONTRIBUTING.md) — Build, test, and PR process
- [Code of Conduct](CODE_OF_CONDUCT.md) — Contributor Covenant v2.1
- [Security Policy](SECURITY.md) — Vulnerability reporting and supply chain security
- [Support](SUPPORT.md) — Getting help and support channels
- [Governance](GOVERNANCE.md) — RFC process and design principles
- [License](LICENSE) — MIT
