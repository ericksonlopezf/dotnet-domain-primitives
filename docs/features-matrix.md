# Features and Target Framework Compatibility Matrix

This document details the target framework support, NativeAOT capability, trimming safety, and feature matrix for all packages in the `EricksonLopez.DomainPrimitives` ecosystem.

---

## 1. Target Frameworks & Runtime Compatibility Matrix

| Package | .NET 8.0 (LTS) | .NET 9.0 | .NET 10.0 | NativeAOT | Trimming Safe | Notes |
|---------|:--------------:|:--------:|:---------:|:---------:|:-------------:|-------|
| `EricksonLopez.DomainPrimitives` | ✅ | ✅ | ✅ | ✅ | ✅ | Zero reflection; NativeAOT-verified |
| `EricksonLopez.DomainPrimitives.Abstractions` | ✅ | ✅ | ✅ | ✅ | ✅ | Core contracts; zero dependencies |
| `EricksonLopez.DomainPrimitives.Generators` | N/A (`netstandard2.0`) | N/A | N/A | N/A | N/A | Roslyn 4.12 Incremental Source Generator |
| `EricksonLopez.DomainPrimitives.Analyzers` | N/A (`netstandard2.0`) | N/A | N/A | N/A | N/A | Roslyn 4.12 Analyzers (DP0001–DP0018) |
| `EricksonLopez.DomainPrimitives.AspNetCore` | ✅ | ✅ | ✅ | ✅ | ✅ | Model binding, minimal API endpoints |
| `EricksonLopez.DomainPrimitives.AspNetCore.SourceGenerators` | N/A (`netstandard2.0`) | N/A | N/A | N/A | N/A | Compile-time route metadata generation |
| `EricksonLopez.DomainPrimitives.EFCore` | ✅ | ✅ | ✅ | ✅ | ✅ | Compile-time Value Converter registration |
| `EricksonLopez.DomainPrimitives.EFCore.SourceGenerators` | N/A (`netstandard2.0`) | N/A | N/A | N/A | N/A | Compile-time DbContext model configurators |
| `EricksonLopez.DomainPrimitives.Dapper` | ✅ | ✅ | ✅ | ✅ | ✅ | TypeHandler mappings without reflection |
| `EricksonLopez.DomainPrimitives.Dapper.SourceGenerators` | N/A (`netstandard2.0`) | N/A | N/A | N/A | N/A | Discovers VOs across assemblies (ADR-044) |
| `EricksonLopez.DomainPrimitives.OpenApi` | ✅ | ✅ | ✅ | ✅ | ✅ | Schema transformers and filters |
| `EricksonLopez.DomainPrimitives.OpenApi.SourceGenerators` | N/A (`netstandard2.0`) | N/A | N/A | N/A | N/A | Compile-time schema generator |
| `EricksonLopez.DomainPrimitives.Testing` | ✅ | ✅ | ✅ | ✅ | ✅ | FluentAssertions extensions & Bogus generators |
| `EricksonLopez.DomainPrimitives.NewtonsoftJson` | ✅ | ✅ | ✅ | ❌ | ❌ | Uses `Newtonsoft.Json`; `[RequiresDynamicCode]` (ADR-026) |

---

## 2. Feature Capability Matrix by Category

### Domain Primitives Core

| Feature | `DomainPrimitives` | `Abstractions` | `Generators` | `Analyzers` |
|---------|:------------------:|:--------------:|:------------:|:-----------:|
| `StringPrimitive` Generator | ❌ | ❌ | ✅ | ❌ |
| `NumericPrimitive` Generator | ❌ | ❌ | ✅ | ❌ |
| `StrongId` Generator | ❌ | ❌ | ✅ | ❌ |
| `DatePrimitive` Generator | ❌ | ❌ | ✅ | ❌ |
| `DomainPrimitive<T>` Base Class | ✅ | ❌ | ❌ | ❌ |
| `ValueObject` Base Class | ✅ | ❌ | ❌ | ❌ |
| `SmartEnum<TEnum, TValue>` | ✅ | ❌ | ❌ | ❌ |
| Core Interfaces (`IDomainPrimitive<T>`, etc.) | ❌ | ✅ | ❌ | ❌ |
| Compile-Time Invariant Enforcement | ❌ | ❌ | ❌ | ✅ (DP0001–DP0018) |

### Integration Packages

| Ecosystem Integration | Package | Reflection Free | Compile-time Gen Available | NativeAOT Verified |
|-----------------------|---------|:---------------:|:--------------------------:|:------------------:|
| ASP.NET Core Minimal APIs / MVC | `.AspNetCore` | ✅ | ✅ (`.AspNetCore.SourceGenerators`) | ✅ |
| Entity Framework Core | `.EFCore` | ✅ | ✅ (`.EFCore.SourceGenerators`) | ✅ |
| Dapper Micro-ORM | `.Dapper` | ✅ | ✅ (`.Dapper.SourceGenerators`) | ✅ |
| OpenAPI / Swagger | `.OpenApi` | ✅ | ✅ (`.OpenApi.SourceGenerators`) | ✅ |
| Newtonsoft.Json Legacy Bridge | `.NewtonsoftJson` | ❌ | ❌ | ❌ (ADR-026) |
| Testing Harness | `.Testing` | ✅ | ❌ | ✅ |
