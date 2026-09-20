# EricksonLopez.DomainPrimitives.NewtonsoftJson

[LEGACY COMPATIBILITY] Newtonsoft.Json (Json.NET) converters and contract resolver for `EricksonLopez.DomainPrimitives`.

## ⚠️ Native AOT & Trimming Incompatibility Notice

> [!WARNING]
> **This package is NOT compatible with Native AOT or Trimming.**  
> `Newtonsoft.Json` (Json.NET) relies heavily on dynamic runtime reflection and runtime code generation, which triggers linker warnings (`IL2026`, `IL3050`) and runtime crashes when compiled under `<PublishAot>true</PublishAot>`.
> 
> For Native AOT, trimming, high-performance microservices, and modern .NET applications, use the core `EricksonLopez.DomainPrimitives` package which provides zero-allocation, source-generated `System.Text.Json` converters out of the box with 100% Native AOT compatibility.

## Usage

This package is intended exclusively for legacy applications or integrations that still require `Newtonsoft.Json`:

```csharp
using Newtonsoft.Json;
using EricksonLopez.DomainPrimitives.NewtonsoftJson;

var settings = new JsonSerializerSettings
{
    Converters = { new DomainPrimitiveJsonConverter() }
};

string json = JsonConvert.SerializeObject(myPrimitive, settings);
```
