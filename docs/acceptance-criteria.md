# Acceptance Criteria

> **Version:** 1.0  
> **Date:** 2026-08-10  
> **Required by:** AUDIT.md §11.5 · MED-V4-002  
> **Format:** Given/When/Then (BDD-style)

---

## Purpose

Per AUDIT.md §11.5, each capability must have formal acceptance criteria in Given/When/Then format.
This document provides the minimum AC set for the six primitive categories.

---

## StringPrimitive

### AC-SP-01: Basic Creation

```gherkin
Given a [StringPrimitive] type with [Trim] and [MinLength(1)] [MaxLength(100)]
When a developer calls Create("  Hello  ")
Then a valid instance is returned with Value = "Hello" (trimmed)
And no exception is thrown
```

### AC-SP-02: Null Rejection

```gherkin
Given any [StringPrimitive] type
When a developer calls Create(null)
Then ArgumentNullException is thrown
And the error does NOT contain the null value
```

### AC-SP-03: Empty Rejection with [NotEmpty]

```gherkin
Given a [StringPrimitive] type with [NotEmpty]
When a developer calls Create("")
Then DomainPrimitiveValidationException is thrown
And error.Code = "EMPTY"
And the exception message contains "[EMPTY]"
```

### AC-SP-04: Length Validation

```gherkin
Given a [StringPrimitive] type with [MaxLength(10)]
When a developer calls Create("12345678901") (11 chars)
Then DomainPrimitiveValidationException is thrown
And error.Code = "LENGTH"
```

### AC-SP-05: TryCreate Zero-Allocation Success Path

```gherkin
Given a [StringPrimitive] type
When a developer calls TryCreate("valid", out result, out error)
Then result is a valid instance
And error = PrimitiveError.None
And Allocated = 0B (measured with BenchmarkDotNet)
```

### AC-SP-06: Span Parse

```gherkin
Given a [StringPrimitive] type
When a developer calls TryParse("valid".AsSpan(), null, out result)
Then result is a valid instance
And the return value is true
```

### AC-SP-07: UTF-8 Span Parse (NET8+)

```gherkin
Given a [StringPrimitive] type running on .NET 8+
When a developer calls TryParse("valid"u8, null, out result)
Then result is a valid instance
And the return value is true
```

### AC-SP-08: JSON Round-trip

```gherkin
Given a [StringPrimitive] type
When an instance is serialized to JSON with System.Text.Json
And the JSON is deserialized back
Then the deserialized instance equals the original
And the JSON representation is a plain JSON string (not a JSON object)
```

### AC-SP-09: IsDefault Detection

```gherkin
Given a [StringPrimitive] type
When a developer creates a default instance with default(T)
Then IsDefault returns true
And accessing Value throws InvalidOperationException
```

### AC-SP-10: Regex Validation (SEC-002)

```gherkin
Given a [StringPrimitive] type with [Regex("^[A-Z]{2}-\d{4}$")]
When a developer calls Create("INVALID")
Then DomainPrimitiveValidationException is thrown
And error.Code = "FORMAT"
When a developer calls Create("US-1234")
Then a valid instance is returned
```

---

## StrongId

### AC-SI-01: GUID-backed Creation

```gherkin
Given a [StrongId<Guid>] type
When a developer calls Create(Guid.NewGuid())
Then a valid instance is returned
And Value equals the provided Guid
```

### AC-SI-02: Empty GUID Rejection (Default)

```gherkin
Given a [StrongId<Guid>] type with default RejectEmpty=true
When a developer calls Create(Guid.Empty)
Then DomainPrimitiveValidationException is thrown
And error.Code = "EMPTY_ID"
```

### AC-SI-03: Type Safety at Compile Time

```gherkin
Given two [StrongId<Guid>] types CustomerId and OrderId
When a developer tries to pass a CustomerId where OrderId is expected
Then the compiler emits an error CS0029
And no runtime check is needed
```

### AC-SI-04: EF Core Value Converter

```gherkin
Given a DbContext using modelBuilder.AddDomainPrimitivesConverters()
When a [StrongId<Guid>] entity property is stored and retrieved
Then the stored column type is uniqueidentifier
And the retrieved value equals the original
And no boxing or reflection occurs during conversion
```

---

## NumericPrimitive

### AC-NP-01: Range Validation

```gherkin
Given a [NumericPrimitive<int>] type with [PrimitiveRange(0, 100)]
When a developer calls Create(101)
Then DomainPrimitiveValidationException is thrown
And error.Code = "RANGE"
When a developer calls Create(50)
Then a valid instance is returned with Value = 50
```

### AC-NP-02: Addition Operator (with NumericOperations.Addition)

```gherkin
Given a [NumericPrimitive<double>] type with Operations = Addition
When a developer writes: var result = distanceA + distanceB
Then result is a valid Distance instance
And result.Value = distanceA.Value + distanceB.Value
```

### AC-NP-03: No Division by Zero (Compile-time)

```gherkin
Given a [NumericPrimitive<decimal>] type WITHOUT ScalarDivision in Operations
When a developer tries to write: var result = price / 2
Then the compiler emits an error (operator / not defined for Price)
```

---

## DatePrimitive

### AC-DP-01: Future Date Validation

```gherkin
Given a [DatePrimitive] type with future-only constraint
When a developer calls Create(DateOnly.FromDateTime(DateTime.Today.AddDays(-1)))
Then DomainPrimitiveValidationException is thrown
And error.Code = "RANGE"
```

### AC-DP-02: Parse from ISO 8601

```gherkin
Given a [DatePrimitive] type
When a developer calls Parse("2026-08-10", null)
Then a valid instance is returned
And Value = DateOnly.Parse("2026-08-10")
```

---

## ValueObject

### AC-VO-01: Basic Creation

```gherkin
Given a [ValueObject] composite type with required string Street, City, State, ZipCode
When a developer creates an instance with all valid values
Then the instance is valid
And all properties are accessible
```

### AC-VO-02: Partial Validation Failure

```gherkin
Given a [ValueObject] with custom Validate partial method checking Street is not empty
When a developer passes an empty Street
Then DomainPrimitiveValidationException is thrown
And error.Code is set by the validator implementation
```

### AC-VO-03: Structural Equality

```gherkin
Given two [ValueObject] instances with identical property values
When a developer calls address1.Equals(address2)
Then the result is true (record struct equality)
```

---

## SmartEnum

### AC-SE-01: Static Instance Access

```gherkin
Given a [SmartEnum<int>] type with static instances Pending, Processing, Completed
When a developer accesses TestOrderStatus.Pending
Then the instance has Value = 1 and Name = "Pending"
```

### AC-SE-02: FromValue Lookup

```gherkin
Given a [SmartEnum<int>] type
When a developer calls TryFromValue(2, out status)
Then status.Name = "Processing"
And the return value is true
```

### AC-SE-03: FromName Lookup

```gherkin
Given a [SmartEnum<int>] type
When a developer calls TryFromName("Completed", out status)
Then status.Value = 3
And the return value is true
```

### AC-SE-04: Invalid Value Rejection

```gherkin
Given a [SmartEnum<int>] type with values 1, 2, 3
When a developer calls TryFromValue(99, out status)
Then the return value is false
And status = default(T)
```

### AC-SE-05: All() Enumeration

```gherkin
Given a [SmartEnum<int>] type with 3 instances
When a developer calls TestOrderStatus.All
Then the result contains exactly 3 instances
And no instance is repeated
```

---

## Cross-Cutting Acceptance Criteria

### AC-CC-01: NativeAOT Compatibility

```gherkin
Given any generated domain primitive type
When the assembly is published with PublishAot=true
Then the build produces 0 IL2026 warnings
And 0 IL3050 warnings
And the published binary executes correctly
```

### AC-CC-02: No Reflection in Hot Paths

```gherkin
Given any generated domain primitive type
When the IL of Create(), TryCreate(), TryParse(), Equals(), GetHashCode() is inspected
Then zero "box" opcodes are emitted for value types
And zero "callvirt" on dynamic dispatch paths
And zero Reflection.* API calls
```

### AC-CC-03: Analyzer Enforcement (DP0001)

```gherkin
Given a project with the EricksonLopez.DomainPrimitives.Analyzers package
When a developer declares: class MyId { public Guid Id; } (not using [StrongId])
Then the analyzer emits DP0001: "Consider using [StrongId<Guid>] for type safety"
```
