# Serialization & NativeAOT Specifications

---

## 1. System.Text.Json Integration

`EricksonLopez.DomainPrimitives.Generators` emits compile-time `JsonConverter<TPrimitive>` that serialize domain primitives directly to primitive JSON literals without boxing or string intermediary allocations.
