# Compatibility Matrix

This document outlines the compatibility matrix for `EricksonLopez.DomainPrimitives` and its integration packages. Because the library is based on C# 14+ Incremental Source Generators, it has specific compiler and framework requirements.

## Framework & Language Support

| Feature | Supported | Notes |
|---------|-----------|-------|
| **C# Language Version** | 14.0+ | Required for modern `readonly record struct` features and optimizations. |
| **.NET SDK** | 10.0+ | Required to run the latest Roslyn analyzers and generators. |
| **Target Framework (Consumers)** | `net8.0`, `net9.0`, `net10.0` | Support for earlier versions is strictly best-effort. |
| **NativeAOT** | :white_check_mark: Yes | Fully trim-safe and AOT compatible (`IsAotCompatible=true`, `IsTrimmable=true`). |
| **Visual Studio** | 2022 (latest) | Required for C# 14 and modern Roslyn support. |
| **Rider** | Latest | |

## Package Target Framework Matrix

| Package | `netstandard2.0` | `net8.0` | `net9.0` | `net10.0` | NativeAOT |
|---------|:---:|:---:|:---:|:---:|:---:|
| `EricksonLopez.DomainPrimitives` | ❌ | ✅ | ✅ | ✅ | ✅ |
| `EricksonLopez.DomainPrimitives.Abstractions` | ✅ | ✅ | ✅ | ✅ | ✅ |
| `EricksonLopez.DomainPrimitives.Generators` | ✅ (compile-time only) | ❌ | ❌ | ❌ | N/A |
| `EricksonLopez.DomainPrimitives.Analyzers` | ✅ (compile-time only) | ❌ | ❌ | ❌ | N/A |
| `EricksonLopez.DomainPrimitives.AspNetCore` | ❌ | ✅ | ✅ | ✅ | ✅ |
| `EricksonLopez.DomainPrimitives.AspNetCore.SourceGenerators` | ✅ (compile-time only) | ❌ | ❌ | ❌ | N/A |
| `EricksonLopez.DomainPrimitives.EFCore` | ❌ | ✅ | ✅ | ✅ | ✅ |
| `EricksonLopez.DomainPrimitives.EFCore.SourceGenerators` | ✅ (compile-time only) | ❌ | ❌ | ❌ | N/A |
| `EricksonLopez.DomainPrimitives.Dapper` | ❌ | ✅ | ✅ | ✅ | ✅ |
| `EricksonLopez.DomainPrimitives.Dapper.SourceGenerators` | ✅ (compile-time only) | ❌ | ❌ | ❌ | N/A |
| `EricksonLopez.DomainPrimitives.OpenApi` | ❌ | ✅ | ✅ | ✅ | ✅ |
| `EricksonLopez.DomainPrimitives.OpenApi.SourceGenerators` | ✅ (compile-time only) | ❌ | ❌ | ❌ | N/A |
| `EricksonLopez.DomainPrimitives.Mapster` | ❌ | ✅ | ✅ | ✅ | ✅ |
| `EricksonLopez.DomainPrimitives.Mapster.SourceGenerators` | ✅ (compile-time only) | ❌ | ❌ | ❌ | N/A |
| `EricksonLopez.DomainPrimitives.Testing` | ❌ | ✅ | ✅ | ✅ | N/A |

> **Minimum runtime: `net8.0`.** `IUtf8SpanParsable<T>`, `RegexOptions.NonBacktracking`, `ArrayPool<T>`, and `MemoryExtensions.ToLowerInvariant` all require .NET 8+. Use `Abstractions` for shared contracts in `netstandard2.0` projects.
>
> **Primary development target: `net10.0` LTS.** All benchmarks and new feature development target .NET 10. See [ADR-016](adr/ADR-016-target-runtime-primary-vs-minimum.md) for the full rationale.

## Integration Packages — Versioning

Because the repository uses Central Package Management, integration packages are built against specific versions of external libraries.

| Package | Integration Target | Pinned Version | Backwards Compatibility |
|---------|--------------------|----------------|-------------------------|
| `EricksonLopez.DomainPrimitives.EFCore` | Entity Framework Core | `8.0.11` / `9.0.0` / `10.0.0` (per TFM) | Compatible with EF Core 8.x, 9.x, and 10.x |
| `EricksonLopez.DomainPrimitives.AspNetCore` | ASP.NET Core | Framework Reference (per TFM) | Compatible with .NET 8, 9, 10 |
| `EricksonLopez.DomainPrimitives.OpenApi` | Swashbuckle.AspNetCore | `6.6.2` | |
| `EricksonLopez.DomainPrimitives.Dapper` | Dapper | `2.1.35` | |
| `EricksonLopez.DomainPrimitives.Mapster` | Mapster | `7.4.0` | |
| `EricksonLopez.DomainPrimitives.Testing` | AwesomeAssertions, xUnit, NSubstitute, Verify.Xunit | `8.0.1` / `2.9.3` / `5.1.0` / `22.5.0` | |

> [!WARNING]
> The source generators rely on modern compiler features. We strictly test and support modern .NET (8.0, 9.0, 10.0+).

## Feature Compatibility by Primitive Type

| Interface | `[StringPrimitive]` | `[NumericPrimitive<T>]` | `[DatePrimitive]` | `[StrongId]` | `[ValueObject]` | `[SmartEnum]` |
|:----------|:---:|:---:|:---:|:---:|:---:|:---:|
| `IParsable<T>` | ✅ | ✅ | ✅ | ✅ | 🔜 v2.0 | ❌ |
| `ISpanParsable<T>` | ✅ | ✅ | ✅ | ✅ | 🔜 v2.0 | ❌ |
| `IUtf8SpanParsable<T>` (net8+) | ✅ | ✅ | ✅ | ✅ | 🔜 v2.0 | ❌ |
| `IFormattable` | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| `ISpanFormattable` | ✅ | ✅ | ✅ | ✅ | ✅ | ❌ |
| `IUtf8SpanFormattable` (net8+) | ✅ | ✅ | ✅ | ✅ | 🔜 v2.0 | ❌ |
| `IComparable<T>` | ✅ | ✅ | ✅ | ✅ | ❌* | ❌ |
| `IEqualityOperators<T,T,bool>` | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| `GetAll()` static list | ❌ | ❌ | ❌ | ❌ | ❌ | ✅ |
| `FromName()` / `TryFromName()` | ❌ | ❌ | ❌ | ❌ | ❌ | ✅ |

> *`IComparable<T>` on `[ValueObject]` is intentionally not generated. Composite types have no canonical ordering unless the domain explicitly defines one. See [ADR-007](adr/ADR-007-zero-allocation-error-model.md).
