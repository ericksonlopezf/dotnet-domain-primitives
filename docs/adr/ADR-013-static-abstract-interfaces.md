# ADR-011: Static Abstract Interface Members and netstandard2.0

## Status
Accepted

## Context
The v4.0 Specification indicates that the `IDomainPrimitive<TSelf, TValue>` interface should declare `static abstract` factory members (`Create`, `TryCreate`) so that consumers can abstractly construct primitives. The library targets `netstandard2.0` (as required by the spec to maximize reach), but `static abstract` members were not introduced to C# until C# 11 and .NET 7.

## Decision
We compile the `static abstract` interface members under an `#if NET7_0_OR_GREATER` preprocessor directive. For `netstandard2.0`, these interface members do not exist.

## Consequences
- **Positive**: Consumers targeting modern .NET 7+ get the full power of generic mathematics and abstract static factory methods.
- **Positive**: The core library remains consumable from `netstandard2.0` legacy applications.
- **Negative**: Consumers using `netstandard2.0` cannot write generic factories (`where T : IDomainPrimitive<T, string>`) that invoke `T.Create()` since the interface lacks the member in their Target Framework Moniker (TFM). They must construct primitives directly or use reflection. This is an unavoidable language limitation accepted as a compromise.
