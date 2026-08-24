# Architecture and Flow Diagrams

> All diagrams reflect the actual architecture discovered in the `EricksonLopez.DomainPrimitives` repository (Fase 0–1 inventory). No hypothetical patterns are depicted.

---

## 1. General Architecture (Package Ecosystem)

```mermaid
graph TD
    subgraph Core ["Core Library"]
        Abstractions["EricksonLopez.DomainPrimitives.Abstractions\n(Interfaces, PrimitiveError, PrimitiveBuilder,\nPrimitiveCollectionExtensions, Attributes)"]
        Core["EricksonLopez.DomainPrimitives\n(Validation pipeline, Normalizers,\nShortcut Attributes, DomainPrimitivesDefaults)"]
        Gen["EricksonLopez.DomainPrimitives.Generators\n(Roslyn Source Generator → emits partial structs)"]
        Anl["EricksonLopez.DomainPrimitives.Analyzers\n(DP0001-DP0017 diagnostics, IDE enforcement)"]
    end

    subgraph Infra ["Infrastructure Packages"]
        AspNet["AspNetCore\n(DomainPrimitiveModelBinder,\nAddDomainPrimitivesModelBinding)"]
        EFCore["EFCore\n(ValueConverter auto-registration\nvia ConfigureDomainPrimitives)"]
        Dapper["Dapper\n(TypeHandler auto-registration\nvia DapperDomainPrimitivesRegistration.RegisterAll)"]
        OpenApi["OpenApi\n(Schema filters for Swagger)"]
        Newtonsoft["NewtonsoftJson\n(AddDomainPrimitives overloads)"]
    end

    subgraph Test ["Testing Package"]
        Testing["EricksonLopez.DomainPrimitives.Testing\n(FakeFactory, TestBuilder, Scenarios,\nAssertionsExtensions, VerifyExtensions)"]
    end

    Core --> Abstractions
    Gen ==>|"compile-time\ncode generation"| Core
    Anl ==>|"IDE diagnostics"| Core

    AspNet --> Core
    EFCore --> Core
    Dapper --> Core
    OpenApi --> Core
    Newtonsoft --> Core
    Testing --> Core
```

---

## 2. Main Application Flow

```mermaid
flowchart TD
    Client((HTTP Client))

    subgraph Presentation ["Presentation Layer"]
        Json["System.Text.Json Converter\n(auto-serializes via TryCreate)"]
        Newtonsoft["Newtonsoft.Json ContractResolver\n(AddDomainPrimitives overloads)"]
        Swagger["OpenApi Schema Filters\n(generates accurate schemas)"]
        AspNet["ASP.NET Core Model Binder\n(AddDomainPrimitivesModelBinding)"]
    end

    subgraph Domain ["Domain Layer"]
        Prim["Domain Primitives\nCreate() / TryCreate() / Parse()"]
        Validator["Normalizer → Built-in Validator → ICustomValidator\n(sequential pipeline)"]
    end

    subgraph Persistence ["Persistence Layer"]
        EF["EF Core ValueConverter\n(ConfigureDomainPrimitives)"]
        Dapper["Dapper TypeHandler\n(RegisterAll)"]
        DB[(Database)]
    end

    subgraph BG ["Background Processing"]
        Channel["Channel&lt;T&gt;\n(producer / consumer)"]
        Worker["Worker\nTryCreate at boundary\nrejects invalid messages"]
    end

    Client -->|"JSON body"| Json
    Client -->|"Route / Query"| AspNet
    Client -->|"API Docs"| Swagger
    Json --> Prim
    AspNet --> Prim
    Prim --> Validator
    Validator -->|"Valid → struct created"| EF
    Validator -->|"Valid → struct created"| Dapper
    EF --> DB
    Dapper --> DB
    Prim --> Channel
    Channel --> Worker
```

---

## 3. Primitive Creation Pipeline (Sequence)

```mermaid
sequenceDiagram
    participant C as Caller
    participant P as Primitive (partial record struct)
    participant N as INormalizer&lt;T&gt; (optional)
    participant V as Built-in Validators
    participant CV as ICustomValidator&lt;T&gt; (optional)

    C->>P: TryCreate(rawValue)
    alt Has INormalizer
        P->>N: Normalize(rawValue)
        N-->>P: normalizedValue
    end
    P->>V: Apply [Trim]/[LowerCase]/[MaxLength]/[Regex]/[PrimitiveRange]...
    alt Built-in validation fails
        V-->>P: PrimitiveError (CODE, message)
        P-->>C: returns false + error
    end
    alt Has ICustomValidator
        P->>CV: Validate(normalizedValue)
        alt Custom validation fails
            CV-->>P: PrimitiveError
            P-->>C: returns false + error
        end
    end
    P-->>C: returns true + readonly record struct
```

---

## 4. Primitive Lifecycle States

```mermaid
stateDiagram-v2
    [*] --> RawInput: Caller passes raw value

    RawInput --> Normalizing: Has INormalizer / [Trim] / [LowerCase]
    RawInput --> Validating: No normalizer

    Normalizing --> Validating: normalized value

    Validating --> Invalid: Built-in rule fails\n(LENGTH / FORMAT / RANGE / EMPTY)
    Validating --> CustomValidating: Built-in passes

    CustomValidating --> Invalid: ICustomValidator fails
    CustomValidating --> Valid: All rules pass

    Invalid --> [*]: PrimitiveError returned\nor DomainPrimitiveValidationException thrown

    Valid --> Instantiated: readonly record struct allocated (stack)
    Instantiated --> Serialized: System.Text.Json / Newtonsoft.Json
    Instantiated --> Persisted: EF Core / Dapper
    Instantiated --> Queued: Channel&lt;T&gt; / background processing
    Instantiated --> [*]: Zero GC — struct lives on stack
```

---

## 5. PrimitiveBuilder Fluent Pipeline

```mermaid
flowchart LR
    A["PrimitiveBuilder&lt;T,V&gt;.For()"] --> B["WithValue(rawValue)"]
    B --> C["Must(predicate1, code1, msg1)\n(optional, repeatable)"]
    C --> D["Must(predicate2, code2, msg2)\n(optional)"]
    D --> E{Build path}
    E -->|"BuildOrThrow()"| F["DomainPrimitiveValidationException\nor TPrimitive"]
    E -->|"Build(out result)"| G["returns bool\nresult set on success"]
    E -->|"BuildResult()"| H["returns object (boxed)"]
```

---

## 6. Error Handling Flow

```mermaid
flowchart TD
    Input["Raw user input"] --> TryCreate["Primitive.TryCreate(input, out result, out error)"]

    TryCreate -->|"returns true"| Success["✅ Use result (validated struct)"]
    TryCreate -->|"returns false"| ErrorPath["❌ Inspect PrimitiveError"]

    ErrorPath --> Code["error.Code (e.g. FORMAT / LENGTH / RANGE / CUSTOM_RULE)"]
    ErrorPath --> Msg["error.Message (human-readable, safe to log)"]

    Code --> Response["Return 400 Bad Request\nor domain error"]
    Msg --> Response

    CreatePath["Primitive.Create(input)"] -->|"valid"| Success2["✅ result"]
    CreatePath -->|"invalid"| Throws["🔴 throws DomainPrimitiveValidationException\n(.Error property = PrimitiveError)"]

    Throws --> Catch["catch (DomainPrimitiveValidationException ex)\n{ ex.Error.Code / ex.Error.Message }"]
```

---

## 7. Background Processing Pattern

```mermaid
sequenceDiagram
    participant Producer as Producer\n(Controller / Service)
    participant Channel as Channel&lt;RawMessage&gt;
    participant Worker as Background Worker
    participant Domain as Domain Primitives

    Producer->>Channel: writer.WriteAsync(new RawMessage(...))
    loop Worker loop
        Channel-->>Worker: reader.ReadAsync() → RawMessage
        Worker->>Domain: OrderId.TryCreate(raw.OrderId)
        alt TryCreate fails
            Domain-->>Worker: returns false + PrimitiveError
            Worker->>Worker: Log.Warning + skip message
        else TryCreate succeeds
            Domain-->>Worker: returns true + OrderId
            Worker->>Worker: Build DomainOrderMessage
            Worker->>Worker: Process validated domain message
        end
    end
```

---

## 8. Component Dependency Graph

```mermaid
graph LR
    App["Application\n(Console / API / Worker)"]

    App --> Core
    App --> AspNet
    App --> EFCore
    App --> Dapper
    App --> Newtonsoft
    App --> Testing

    subgraph Core["EricksonLopez.DomainPrimitives (Core)"]
        Abs["Abstractions"]
        Prim["DomainPrimitives"]
        Gen["Generators (Source)"]
        Anl["Analyzers (Roslyn)"]
    end

    AspNet["AspNetCore"] --> Prim
    EFCore["EFCore"] --> Prim
    Dapper["Dapper"] --> Prim
    Newtonsoft["NewtonsoftJson"] --> Prim
    Testing["Testing"] --> Prim
    Prim --> Abs
    Gen -.->|"emit partial"| Prim
    Anl -.->|"analyze"| Prim
```
