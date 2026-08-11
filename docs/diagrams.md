# 📊 Architecture and Flow Diagrams

## 1. General Architecture and Main Flow
*See [Functional Map](functional-map.md)*.

## 2. Validation Sequence (Error Handling)

```mermaid
sequenceDiagram
    participant C as Client (API/UI)
    participant B as Binder / JSON Deserializer
    participant V as FluentValidation
    participant P as Primitive (EmailAddress)
    
    C->>B: Sends data (e.g. "invalid-email")
    B->>P: EmailAddress.TryCreate("invalid-email")
    P-->>B: Returns Result.Failure("Invalid format")
    B->>V: FluentValidation evaluates
    V-->>C: 400 Bad Request (ValidationProblemDetails)
    
    C->>B: Sends data (e.g. "valid@email.com")
    B->>P: EmailAddress.TryCreate("valid@email.com")
    P-->>B: Returns Result.Success(EmailAddress)
    B->>V: FluentValidation OK
    V-->>C: 200 OK (Processed)
```

## 3. Primitive States (Lifecycle)

```mermaid
stateDiagram-v2
    [*] --> RawInput: Data Reception (string/int)
    RawInput --> Validating: TryCreate() or Create()
    
    Validating --> Invalid: Fails regex/range
    Validating --> Valid: Rules passed
    
    Invalid --> [*]: Returns Error or Throws Exception
    
    Valid --> Instantiated: Immutable Struct Created
    Instantiated --> Transformed: .Value / Persistence
    Instantiated --> [*]: Destruction (GC)
```

## 4. Component Dependencies (Pipeline)

```mermaid
graph LR
    A[EricksonLopez.DomainPrimitives] --> B[EricksonLopez.DomainPrimitives.Abstractions]
    
    C[AspNetCore.Integration] -.-> A
    D[EFCore.Integration] -.-> A
    E[Dapper.Integration] -.-> A
    F[Mapster.Integration] -.-> A
    
    G[Generators] ==>|Injects Code| A
    H[Analyzers] ==>|IDE Rules| A
```

## 5. Processing and Pipeline Behaviors (MediatR)

```mermaid
flowchart TD
    Req[Request DTO] --> |Dispatch| MediatR
    
    subgraph Pipeline [MediatR Pipeline]
        Logging[Logging Behavior] --> Validation[Validation Behavior]
        Validation -->|If error| Fallback[Returns Result.Failure]
        Validation -->|If Ok| Handler[Command/Query Handler]
    end
    
    MediatR --> Logging
    Handler --> |Data Access| Repository
    Repository --> DB[(Database)]
```
