# Level 07 — JSON Serialization (System.Text.Json & Newtonsoft.Json)

In Level 07, we explore zero-allocation JSON serialization.

---

## 1. NativeAOT System.Text.Json

Domain primitive source generators produce compile-time `JsonConverter<TPrimitive>` that serialize directly to/from primitive JSON tokens without intermediate string allocations.

---

## 2. Newtonsoft.Json Support

For legacy enterprise applications, `EricksonLopez.DomainPrimitives.NewtonsoftJson` provides bidirectional `JsonConverter` implementations.
