# API Reference (Version 1.0.0)

This documentation covers the main public APIs (Interfaces, Attributes, and Builders) available in `EricksonLopez.DomainPrimitives`.

---

## 1. `IDomainPrimitive<TSelf, TValue>` (Interface)

Main interface that defines the contract of any generated Domain Primitive.

**Namespace:** `EricksonLopez.DomainPrimitives`
**Assembly:** `EricksonLopez.DomainPrimitives.Abstractions`

### Properties
- **`TValue Value { get; }`**: Gets the underlying normalized and validated value.
- **`string PrimitiveName { get; }`**: Name of the primitive in question.
- **`bool IsDefault { get; }`**: Indicates if the primitive has not been initialized (default struct).

### Methods
- **`static TSelf Create(TValue value)`**: Instantiates the primitive by validating `value`. If validation fails, it throws `DomainPrimitiveException`.
- **`static Result<TSelf> TryCreate(TValue value)`**: Instantiates the primitive encapsulating errors in the `Result` type without throwing exceptions.

**When to use it:** When you need to create generic functions that accept any primitive from the library as a parameter: `public void Process<T>(T primitive) where T : IDomainPrimitive<T, string>`.

---

## 2. `PrimitiveBuilder<TPrimitive, TValue>` (Class)

Allows building, instantiating, and validating a primitive by adding additional rules on the fly fluently (Fluent API).

**Namespace:** `EricksonLopez.DomainPrimitives`

### Methods
- **`static PrimitiveBuilder<TPrimitive, TValue> For()`**: Initializes the builder.
- **`PrimitiveBuilder<TPrimitive, TValue> Must(Func<TValue, bool> predicate, string errorCode, string errorMessage)`**: Adds an inline custom rule.
- **`Result<TPrimitive> BuildResult()`**: Returns the instance inside a `Result` capturing native or provided failed validations.
- **`TPrimitive Build()`**: Builds the primitive, throwing an exception if invalid.

**Basic Example:**
```csharp
var id = PrimitiveBuilder<CustomerId, Guid>.For()
    .BuildResult();
```

---

## 3. `[StrongId]` (Attribute)

Generates a strongly typed identifier.

**Parameters:** None (the base type is inferred).
**Associated Exceptions:** `DomainPrimitiveValidationException` (EmptyGuid, etc.).
**Usage example:** Primary identifiers of databases.

---

## 4. `[NumericPrimitive<TValue>]` (Attribute)

Defines a safe numeric value in the domain.

**Namespace:** `EricksonLopez.DomainPrimitives`

### Configuration Properties
- **`ArithmeticPolicy Policy`**: Configuration of `ArithmeticPolicy` (Flags Enum): `None`, `Addition`, `Additive`, `ScalarMultiplication`, `ScalarDivision`, `Multiplicative`, `All`.

### Observations
When policies are applied, the Source Generator will create `operator +`, `operator -`, `operator *`, or `operator /` methods that return new safe instances without breaking encapsulation.

**When NOT to use it:** If the number is merely descriptive (e.g., a phone number or zip code). For those cases use `[StringPrimitive]` with `[Regex]`.
