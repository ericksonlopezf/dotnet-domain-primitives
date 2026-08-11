# RFC 0004: Multi-TFM Strategy

## Context and Problem Statement
Domain Primitives is built as a set of Incremental Source Generators. These generators require Roslyn 4.x and C# 11+ features, meaning the developer environment must be modern (.NET 10 LTS targeted). However, consumers might be building projects that still target older frameworks.

## Proposed Solution
The core library and generators will target 
etstandard2.0 (for the generator logic) and the abstractions will target multiple frameworks: 
et8.0, 
et9.0, and 
et10.0.
This aligns with Microsoft's current support lifecycle, supporting the current LTS, the previous STS, and the upcoming STS/LTS releases.

## Decision Outcome
The COMPATIBILITY_MATRIX.md formally supports 
et8.0, 
et9.0, and 
et10.0. Older frameworks (
et6.0, 
et7.0) are not explicitly tested and are treated as best-effort due to their end-of-life status.
