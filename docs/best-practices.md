# Best Practices & Production Guidelines

---

## 1. Domain Primitive Modeling Rules

1. **Always Use Structs**: Primitives represent scalar concepts without identity lifecycle. Structs avoid GC allocations.
2. **Keep Validation Deterministic**: Never execute asynchronous I/O or database queries inside domain primitive validators.
3. **Use Source Generators**: Leverage `[DomainPrimitive<T>]` for automatic operators, parsers, and type converters.
