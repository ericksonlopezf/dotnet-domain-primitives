# Level 04 — Source Generators & NativeAOT Compilation

In Level 04, we explore compile-time source generation for domain primitives and NativeAOT trimming safety.

---

## 1. Incremental Roslyn Generators

Decorating structs with `[DomainPrimitive<T>]` automatically emits:
- `Create(T value)` and `TryCreate(T value, out TPrimitive result)`
- `IEquatable<TPrimitive>`, `IComparable<TPrimitive>`
- Equality operators (`==`, `!=`)
- Implicit/explicit conversions
- NativeAOT `JsonConverter` and `TypeConverter` implementations
