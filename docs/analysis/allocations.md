# Allocation & Memory Profile Analysis

---

## 1. Zero-Allocation Domain Primitives

| Primitive Type | Standard Class Wrapper | `EricksonLopez.DomainPrimitives` | Improvement |
|---|---|---|---|
| Domain Primitive Instantiation | 24–32 B | **0 B** (`readonly partial struct`) | **100% Zero Allocation** |
| SmartEnum Lookup by Name/Value | 24 B heap lookup | **0 B (Amortized)** | **Zero Allocation Cache** |
| EF Core Value Conversion | String allocation | **0 B (Span-based)** | **Zero Allocation** |
| JSON Serialization (STJ) | UTF-8 Transcoding | **0 B (Direct Token Write)** | **Zero Allocation** |
