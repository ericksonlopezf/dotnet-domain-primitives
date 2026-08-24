# Level 03 — Roslyn Analyzers & Compile-Time Diagnostics

In Level 03, we explore Roslyn analyzers that enforce domain primitive rules during compilation.

---

## 1. Roslyn Diagnostic Rules

- **ELDP001**: Domain primitive structs must be declared `readonly partial struct`.
- **ELDP002**: Validation method `Validate` must be private static returning `Result<T, ValidationError>`.
- **ELDP003**: Domain primitive types must not expose public mutable fields or setters.
