# Troubleshooting Guide

This guide compiles the most common compilation errors, Roslyn analyzer diagnostics, runtime persistence issues, and integration gotchas encountered when working with `EricksonLopez.DomainPrimitives`, along with verified resolutions.

---

## 1. Compilation Errors

### CS0260: Missing 'partial' modifier on type declaration
- **Cause:** The domain primitive struct or record was declared without the `partial` keyword.
- **Message:** `Missing partial modifier on declaration of type 'CustomerId'; another partial declaration of this type exists`.
- **Solution:** Add `partial` to your type declaration:
```csharp
// ❌ Incorrect
[StrongId<Guid>]
public readonly record struct CustomerId;

// ✅ Correct
[StrongId<Guid>]
public readonly partial record struct CustomerId;
```

### CS0568: Structs cannot contain explicit parameterless constructors
- **Cause:** Declaring an explicit parameterless constructor on a value object or primitive struct (or in older C# versions).
- **Solution:** Rely on the source-generated factory methods (`Create`, `TryCreate`) and private constructor emitted by the generator.

---

## 2. Roslyn Analyzer Diagnostics (DP0001–DP0018)

The `EricksonLopez.DomainPrimitives.Analyzers` package enforces design invariants, immutability, and API surface budgets at design time:

| Diagnostic | Title | Severity | Cause & Resolution |
|---|---|:---:|---|
| **DP0001** | Domain primitive must be partial | Error | Type missing `partial` modifier. Add `partial` keyword. |
| **DP0002** | Domain primitive must be readonly | Error | Struct is not marked `readonly`. Add `readonly` to enforce immutability. |
| **DP0003** | Domain primitive must be a record struct | Error | Primitives require zero-boxing value equality. Declare as `record struct`. |
| **DP0004** | Invalid Regex Pattern | Error | Pattern in `[StringPrimitive(Pattern = "...")]` is syntactically invalid. Correct the regex. |
| **DP0005** | Conflicting normalization attributes | Error | Both `[LowerCase]` and `[UpperCase]` applied. Keep only one casing rule. |
| **DP0006** | Invalid constraint bounds | Error | `Min` is greater than `Max` in range/length attribute. Fix constraint bounds. |
| **DP0007** | Avoid uninitialized domain primitive | Warning | Calling parameterless `new T()` bypasses validation. Use `T.Create()` instead. |
| **DP0008** | Value object properties must use 'init' | Error | Property on `[ValueObject]` has mutable `set` accessor. Change to `get; init;`. |
| **DP0009** | Missing validation | Warning | Domain primitive has no validation attributes or custom validation rule. |
| **DP0010** | String compared directly with domain primitive | Warning | Using `==` between raw string and primitive. Parse string or compare `.Value`. |
| **DP0011** | String assigned directly from domain primitive | Warning | Direct assignment to `string` discards strong type safety. Use `.Value`. |
| **DP0012** | Public constructor bypasses validation | Warning | Explicit public constructor declared. Remove it and use `Create()` / `TryCreate()`. |
| **DP0013** | Possible duplicate domain primitive logic | Info | Multiple primitives share identical attributes. Verify distinct domain intent. |
| **DP0014** | API Surface Budget Exceeded | Warning | Type has exceeded recommended member count threshold (maintain single-responsibility). |
| **DP0015** | Missing XML Documentation | Warning | Public members on domain primitives must have XML documentation. |
| **DP0016** | Invalid Factory Method Name | Warning | Custom factory method must be named `Create`, `TryCreate`, `Parse`, or `TryParse`. |
| **DP0017** | Invalid ExceptionType | Error | Custom exception in `[DomainPrimitivesDefaults]` must inherit `Exception` and accept `(string message)`. |
| **DP0018** | Value object mutable collection | Warning | ValueObject property uses mutable collection (`List<T>`, array). Use `ImmutableArray<T>`. |

---

## 3. Persistence and Database Issues

### Dapper: `System.Data.DataException: Cannot parse null as CustomerId`
- **Cause:** The database column returned `NULL`, but the C# entity property was declared as non-nullable (`CustomerId`).
- **Solution:** Mark the property as nullable in your entity model:
```csharp
public CustomerId? CustomerId { get; set; }
```
- **Secondary Cause:** Dapper type handlers were not initialized at application startup. Ensure `DapperDomainPrimitivesRegistration.RegisterAll()` is called in `Program.cs`.

### EF Core: Column mapped to unrecognized type or migration fails
- **Cause:** `ConfigureDomainPrimitives()` was not called or was placed inside `OnModelCreating` instead of `ConfigureConventions`.
- **Solution:** Register domain primitive value converters in `ConfigureConventions`:
```csharp
protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
{
    configurationBuilder.ConfigureDomainPrimitives();
}
```

---

## 4. ASP.NET Core and OpenAPI / Swagger Issues

### Swagger UI shows the primitive as an empty object `{}` instead of scalar `string` / `uuid`
- **Cause:** Swashbuckle inspects type properties via reflection and cannot infer scalar representations from custom structs.
- **Solution:** Register the compile-time generated schema filter in `Program.cs`:
```csharp
builder.Services.AddSwaggerGen(c =>
{
    c.SchemaFilter<EricksonLopez.DomainPrimitives.OpenApi.Generated.DomainPrimitivesSchemaFilter>();
});
```

### Route or query parameters return unexpected 400 Bad Request
- **Cause:** The incoming string failed invariant validation (e.g. invalid format, range violation, or failing regex).
- **Diagnosis:** Inspect validation failure diagnostics by checking the problem details response or subscribing to domain validation events.
