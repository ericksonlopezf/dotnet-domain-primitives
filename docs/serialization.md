# Serialization & NativeAOT Specifications

This document defines the serialization architecture, JSON serializer integrations, and NativeAOT trimming guarantees of the `EricksonLopez.DomainPrimitives` ecosystem.

---

## Serialization Philosophy

In traditional .NET libraries, domain types either:
1. Serialize as complex JSON objects containing internal wrapper fields (e.g. `{"value": "admin@example.com"}`).
2. Rely on runtime reflection (`JsonConverterFactory`, `Activator.CreateInstance`, dynamic type inspection) to serialize as raw literals, breaking NativeAOT compilation and degrading throughput.

`EricksonLopez.DomainPrimitives` rejects both compromises. All JSON converters are emitted at **compile time** via Roslyn incremental source generators, serializing primitives directly as raw JSON literals (`"admin@example.com"`, `150.00`, `"3fa85f64-5717-4562-b3fc-2c963f66afa6"`) with **zero runtime reflection** and **zero heap allocation**.

---

## 1. System.Text.Json (STJ) Integration

### Compile-Time Inline Converters
For every declared `[StrongId]`, `[StringPrimitive]`, `[NumericPrimitive<T>]`, `[DatePrimitive]`, and `[SmartEnum]`, `EricksonLopez.DomainPrimitives.Generators` emits a dedicated, strongly-typed `JsonConverter<TPrimitive>`:

```csharp
// Compile-time emitted converter snippet
public sealed class EmailAddressJsonConverter : JsonConverter<EmailAddress>
{
    public override EmailAddress Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException("Expected string token for EmailAddress.");

        string? value = reader.GetString();
        return EmailAddress.Create(value!); // Enforces invariant validation!
    }

    public override void Write(Utf8JsonWriter writer, EmailAddress value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value); // Writes raw string literal directly
    }
}
```

### Invariant Validation on Deserialization (BC-004)
Unlike naïve serializers that populate backing fields directly via uninitialized object allocators or reflection, our generated converters construct primitives through `Create(value)`:
- Inbound JSON payloads with invalid values (e.g., malformed emails, negative amounts) immediately trigger domain validation and throw `DomainPrimitiveValidationException`.
- Invalid state is impossible to smuggle into your domain model through JSON deserialization.

### Composite `[ValueObject]` Serialization
For composite value objects, the generator emits a `ValueObjectJsonConverter` that reads properties sequentially and passes them into the constructor or `Create()` factory hook, running the partial `Validate` method prior to returning the instance.

---

## 2. Multi-Assembly Discovery & Dapper JSON Registration

When primitives reside across multi-project solution boundaries, `EricksonLopez.DomainPrimitives.Dapper.SourceGenerators` automatically emits `JsonConverterGenerator`:
- Discovers primitive and entity ID types implementing `IEntityId<TSelf>` or `IStrongId<TSelf, TValue>` across all referenced compilation assemblies.
- Emits a centralized registration helper:
  ```csharp
  // Auto-generated bulk registration
  DomainPrimitivesJsonRegistration.RegisterAll(jsonSerializerOptions);
  ```
- Guarantees zero reflection during startup while ensuring all domain IDs serialize seamlessly.

---

## 3. Newtonsoft.Json (Json.NET) Legacy Migration

For brownfield applications transitioning to modern .NET or maintaining legacy Web API endpoints, `EricksonLopez.DomainPrimitives.NewtonsoftJson` provides full Json.NET integration:

```csharp
using EricksonLopez.DomainPrimitives.NewtonsoftJson;

// Configure global Newtonsoft.Json settings
var settings = new JsonSerializerSettings
{
    ContractResolver = new DomainPrimitivesContractResolver()
};
settings.AddDomainPrimitives();
```

### Supported Types
- `DomainPrimitiveNewtonsoftJsonConverter<TPrimitive, TValue>`: Generic converter for strongly-typed primitives.
- `DomainPrimitiveUniversalNewtonsoftJsonConverter`: Non-generic fallback for heterogeneous collections.
- `DomainPrimitivesContractResolver`: Resolves custom contract metadata without interfering with third-party types.

> [!WARNING]
> **NativeAOT Limitation:** `Newtonsoft.Json` is intrinsically built on runtime reflection. The `EricksonLopez.DomainPrimitives.NewtonsoftJson` package is intentionally decorated with `[RequiresDynamicCode]` and `[RequiresUnreferencedCode]` attributes. It is not compatible with NativeAOT.

---

## 4. NativeAOT and Trimming Guarantees

| Category | NativeAOT Status | Verification Mechanism |
|:---|:---:|:---|
| **Core Primitives (`DomainPrimitives`)** | ✅ 100% Trim-Safe | Continuous `aot-smoke-test.yml` execution |
| **Abstractions (`DomainPrimitives.Abstractions`)** | ✅ 100% Trim-Safe | Zero dependencies, no reflection |
| **Source Generators (`Generators`)** | ✅ Compile-Time | Emits static C# code |
| **ASP.NET Core Binding (`AspNetCore`)** | ✅ 100% Trim-Safe | Direct `TryParse` model binding without reflection |
| **EF Core Conventions (`EFCore`)** | ✅ 100% Trim-Safe | Emits static `ValueConverter` conventions |
| **Dapper Type Handlers (`Dapper`)** | ✅ 100% Trim-Safe | Compile-time `SqlMapper.TypeHandler<T>` instances |
| **OpenAPI Schema Filters (`OpenApi`)** | ✅ 100% Trim-Safe | Compile-time schema mapping filters |
| **Newtonsoft.Json (`NewtonsoftJson`)** | ❌ Incompatible by design | Annotated with `[RequiresDynamicCode]` |

### How Zero IL Warnings are Maintained
- **No `MakeGenericType`:** All generic implementations are specialized at compile time.
- **No `Activator.CreateInstance`:** Instances are instantiated directly through emitted constructors or static abstract factory methods (`TSelf.Create`).
- **Zero Trimmer Suppressions:** We do not suppress `IL2026` or `IL3050` warnings with `#pragma` or `[UnconditionalSuppressMessage]`; code is structured to be genuinely reflection-free.
