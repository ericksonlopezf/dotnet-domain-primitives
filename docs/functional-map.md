# Architecture Functional Map — Lifecycle & Layer Transitions

This functional map details the complete end-to-end data lifecycle of **EricksonLopez.DomainPrimitives** across all application tiers (such as ASP.NET Core, Domain, CQRS, and Persistence).

The flow covers all 8 architectural stages discovered in the public API inventory:
1. **Application Entry Point** (HTTP / Route / JSON Ingestion)
2. **Processing Layer** (Domain Invariants & Validation Enforcement)
3. **Dispatch Layer** (CQRS Command / Query Dispatching via MediatR)
4. **Publication Layer** (Domain Event Generation & Outbox Dispatch)
5. **Persistence Layer** (EF Core & Dapper ORM Mapping)
6. **Consumers Layer** (Background Channel Workers & Subscriptions)
7. **Confirmation Layer** (Transaction Commit, Telemetry & HTTP Response)
8. **Cleanup Layer** (Zero-Allocation Struct Disposal & Diagnostic Teardown)

---

## 1. End-to-End System Lifecycle Diagram

```mermaid
flowchart TD
    subgraph EntryPoint ["1. Application Entry Point"]
        HTTP[HTTP Request / Controller / Minimal API]
        ModelBinding[ASP.NET Core ModelBinder / JSON Deserializer]
        RawData[Raw Scalars: string, int, decimal, Guid]
        HTTP --> ModelBinding --> RawData
    end

    subgraph ProcessingLayer ["2. Processing Layer (Invariants & Validation)"]
        Normalize[INormalizer / Trim / LowerCase / UpperCase]
        BuiltInValidation[Built-in Validators: NotEmpty, Range, Length, Regex]
        CustomValidation[ICustomValidator / Complex Algorithms]
        StructCreation[Instantiate Readonly Partial Record Struct]
        RawData --> Normalize --> BuiltInValidation --> CustomValidation --> StructCreation
    end

    subgraph DispatchLayer ["3. Dispatch Layer (CQRS & Commands)"]
        Command[MediatR Command / Query with Strongly-Typed Primitives]
        PipelineBehavior[Pipeline Behaviors: Logging, Validation, UnitOfWork]
        Handler[Command / Query Handler]
        StructCreation --> Command --> PipelineBehavior --> Handler
    end

    subgraph PublicationLayer ["4. Publication Layer (Domain Events)"]
        Aggregate[Aggregate Root mutates state using Primitives]
        DomainEvent[Domain Event created with Primitive Payload]
        EventDispatch[Domain Event Dispatcher / Outbox]
        Handler --> Aggregate --> DomainEvent --> EventDispatch
    end

    subgraph PersistenceLayer ["5. Persistence Layer (EF Core & Dapper)"]
        EFValueConverter[EF Core ValueConverter&lt;T, TValue&gt;]
        DapperTypeHandler[Dapper SqlMapper.TypeHandler&lt;T&gt;]
        Database[(Relational Database: SQL Server / PostgreSQL / SQLite)]
        Handler --> EFValueConverter --> Database
        Handler --> DapperTypeHandler --> Database
    end

    subgraph ConsumersLayer ["6. Consumers (Background Workers & Channels)"]
        Channel[System.Threading.Channels Channel&lt;T&gt;]
        BackgroundWorker[BackgroundService / Worker Process]
        BoundaryValidation[Safe TryCreate() at consumer boundary]
        EventDispatch --> Channel --> BackgroundWorker --> BoundaryValidation
    end

    subgraph ConfirmationLayer ["7. Confirmation (Commit & Response)"]
        DbCommit[Database Transaction Commit]
        Telemetry[DomainPrimitivesDiagnostics / Meter metrics recorded]
        HttpResponse[HTTP 200 OK / 201 Created with JSON Serialized Primitive]
        Database --> DbCommit --> HttpResponse
        BoundaryValidation --> Telemetry
    end

    subgraph CleanupLayer ["8. Cleanup (Zero-Allocation & Teardown)"]
        StackDisposal[Zero GC Allocation: Struct disposed on stack frame return]
        DiagnosticEnd[Activity / DiagnosticSource scope closed]
        HttpResponse --> StackDisposal --> DiagnosticEnd
    end
```

---

## 2. Detailed Layer Transitions

### Transition 1 → 2: From Application Entry Point to Processing Layer
- **Mechanism:** The external incoming request reaches an HTTP endpoint (ASP.NET Core Minimal API or MVC Controller).
- **Involved Components:** `DomainPrimitivesMvcBuilderExtensions.AddDomainPrimitivesModelBinding()`, `DomainPrimitiveModelBinder<T>`, or `System.Text.Json` / `Newtonsoft.Json` converters.
- **Behavior:** The model binder extracts raw scalar values from route parameters, query strings, or the JSON payload body. Instead of instantiating untyped objects, it calls `TPrimitive.TryCreate(rawValue, out var primitive, out var error)` or `TPrimitive.Parse(rawValue)`.
- **Guarantee:** Malformed input is intercepted at the external application boundary, returning an immediate `400 Bad Request` or `ProblemDetails` response before any domain logic executes.

### Transition 2 → 3: From Processing Layer to Dispatch Layer
- **Mechanism:** Once validated, immutable primitive instances are encapsulated as properties within a CQRS command or query (e.g., `RegisterCustomerCommand(CustomerId Id, EmailAddress Email, Money InitialBalance)`).
- **Involved Components:** MediatR pipeline behaviors, dependency injection pipelines.
- **Behavior:** The command is structurally immutable and self-documenting. Compiler type safety makes it impossible to accidentally transpose identifiers (such as passing a `CustomerId` into an `OrderId` parameter).

### Transition 3 → 4: From Dispatch Layer to Publication Layer
- **Mechanism:** The CQRS handler retrieves or constructs a domain aggregate (e.g., `Order`) and executes domain business methods using the verified primitives.
- **Involved Components:** Entities, Aggregate Roots, and domain events (`IDomainEvent`).
- **Behavior:** The aggregate raises rich domain events carrying domain primitives as their payload (e.g., `OrderPlacedDomainEvent(OrderId Id, Money Amount, DateTime CreatedAt)`).

### Transition 4 → 5: From Domain Logic to Persistence Layer
- **Mechanism:** The state of the aggregate is persisted to relational storage through Entity Framework Core or Dapper.
- **Involved Components:** `ConfigureDomainPrimitives(this ModelConfigurationBuilder)` in EF Core or `DapperDomainPrimitivesRegistration.RegisterAll()` in Dapper.
- **Behavior:** Source generators emit reflection-free `ValueConverter<T, TValue>` and `SqlMapper.TypeHandler<T>` instances. When writing, they read `primitive.Value`; when reading, they reconstruct the primitive instance via compile-time factories `T.Create(dbValue)`.

### Transition 5 → 6: From Publication to Asynchronous Consumers
- **Mechanism:** Events are routed to in-memory asynchronous channels (`Channel<T>`) or distributed message brokers.
- **Involved Components:** `System.Threading.Channels.Channel<T>`, `BackgroundService`, `TryCreate()`.
- **Behavior:** When background workers dequeue a message, they validate at the boundary using `TryCreate` to ensure asynchronous deserialization does not corrupt background state.

### Transition 6 → 7: From Consumers to Confirmation
- **Mechanism:** The database transaction commits (`CommitAsync`), and successful results return to the client.
- **Involved Components:** `DomainPrimitivesDiagnostics.Source`, `DomainPrimitivesMetrics.RecordCreation()`, `JsonSerializer`.
- **Behavior:** The primitive is serialized directly as a raw scalar JSON token (a string for email, number for money, UUID string for IDs) in the HTTP response. Telemetry meters record metrics for successful operations.

### Transition 7 → 8: From Confirmation to Cleanup
- **Mechanism:** Execution frame unwind and stack return.
- **Involved Components:** .NET CLR stack allocator and `DiagnosticListener`.
- **Behavior:** As `readonly record struct` value types, primitives allocate 0 bytes on the heap across the success path, incurring zero garbage collection overhead. Diagnostic activity scopes close deterministically without memory leaks.
