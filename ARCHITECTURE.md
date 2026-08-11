# Architecture & Design Philosophy

## Forward-Compatible Design

`EricksonLopez.DomainPrimitives` targets `.NET 8+` (C# 14) and is designed with forward compatibility in mind. All generated code avoids APIs that are likely to be deprecated, and the library's reliance on stable BCL interfaces (`IParsable<T>`, `ISpanFormattable`, `IUtf8SpanParsable<T>`) ensures long-term viability as the .NET runtime evolves.

## Zero-Allocation Policy (P1)

We enforce a strict zero-allocation policy across the source generator core and all generated primitives:

1. **TryCreate `out` Pattern:** We never allocate heap wrappers like `Result<T>` on hot paths. Validation methods return `bool` and provide the parsed primitive via `out` parameters.
2. **UTF-8 Parsing:** We use `ArrayPool<T>` (e.g., `ArrayPool<char>`) when decoding `ReadOnlySpan<byte>` over the 256-character threshold (SEC-006), completely avoiding large `char[]` heap allocations.
3. **Struct-Based Primitives:** Domain primitives are `readonly record struct` instances that reside on the stack.

> **One unavoidable allocation:** Unicode NFC normalization (SEC-004) requires producing a `System.String` because normalization can change character count. This is the minimum allocation on any string path.

## Semantic Correctness (P0)

**"If optimizing performance (P3) breaks semantic correctness (P0), P0 wins."**
While performance is critical, a domain primitive must never misrepresent state. Validation rules are absolute, and invariants cannot be bypassed.

## Exception Philosophy

- `Parse()` throws `System.FormatException` to align with BCL standards (`IParsable<T>` contract). This was standardized via [RFC-0003](docs/rfcs/RFC-0003-format-exception-standardization.md).
- `Create()` throws `DomainPrimitiveValidationException` when validation fails.
- `TryCreate()` and `TryParse()` return `bool` with `out` parameters — no exceptions on failure.
- In integrations (like EF Core), `InvalidOperationException` captures data mapping corruptions (e.g., mapping a `null` string from a database into a non-nullable domain primitive).

> **Deprecation note:** `DomainPrimitiveFormatException` is deprecated with `[Obsolete(error: false)]` (standardized via RFC-0003 prior to the v1.0.0 release) and will be removed in v3.0. See the [CHANGELOG.md](../CHANGELOG.md) `[1.0.0]` section for full details.

## Source Generators & Integrations

We rely on Roslyn Incremental Source Generators (`IIncrementalGenerator`) to emit zero-overhead integration code (System.Text.Json, EF Core, Dapper, Mapster, ASP.NET Core, OpenAPI).

- Integrations are discovered **automatically**. Developers do not need to annotate domain models with integration-specific attributes.
- Generated code uses explicit interface implementations for `IParsable<T>`, `ISpanParsable<T>`, `IUtf8SpanParsable<T>`, and `IUtf8SpanFormattable` to integrate deeply with the .NET BCL.

## Design Decisions

All significant design decisions are documented as Architecture Decision Records (ADRs) in [`docs/adr/`](docs/adr/).
