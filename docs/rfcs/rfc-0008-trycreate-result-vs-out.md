# RFC 0003: TryCreate returning Result<T> vs bool out

## Context and Problem Statement
Domain Primitives require a way to handle validation failures gracefully without throwing exceptions, especially in high-throughput API endpoints where invalid user input is common. The standard BCL pattern is the TryParse pattern which uses a ool return type and an out parameter.

## Proposed Solution
While we implement the BCL TryParse(string, out T) to satisfy standard parsing interfaces (ISpanParsable<T>, IUtf8SpanParsable<T>), the primary domain factory method TryCreate(TValue) will return a custom Result<T> struct.

This allows us to return rich validation errors (e.g., "String length must be between 1 and 100") without exceptions, which is impossible with a simple ool return type.

## Decision Outcome
We provide both:
1. ool TryCreate(TValue value, out TSelf result) for high-performance zero-allocation scenarios where the error reason is irrelevant.
2. Result<TSelf> TryCreate(TValue value) for domain validation where rich error reporting is required.
