# EricksonLopez.DomainPrimitives.OpenApi

OpenAPI / Swagger schema integration for `EricksonLopez.DomainPrimitives`.

## Overview

This package operates as a **Roslyn Source Generator analyzer package**. It contains zero runtime DLL overhead and generates Swashbuckle / OpenAPI `ISchemaFilter` configurations at compile time.

## Key Features

- **Accurate Schema Representation**: Maps domain primitive structs to their underlying primitive OpenAPI types (e.g. `string` format `uuid` for `StrongId<Guid>`, `string` format `email` for `EmailAddress`, `integer` for numeric primitives) rather than complex object schemas.
- **Zero Runtime Reflection**: Generates static schema filter dictionaries and registration helpers without runtime reflection.
- **Native AOT Compatible**: Safe for trimming and ahead-of-time compilation.

## Usage

```csharp
services.AddSwaggerGen(c =>
{
    c.SchemaFilter<DomainPrimitivesSchemaFilter>();
});
```
