# Migration Guide & Version Upgrades

---

## 1. Migrating to Roslyn Source Generated Primitives

To migrate from manual primitive classes to source-generated struct primitives:
1. Replace `class` with `readonly partial struct`.
2. Add `[DomainPrimitive<TUnderlying>]`.
3. Provide `private static Result<TUnderlying, ValidationError> Validate(TUnderlying value)`.
4. Delete manual `Equals`, `GetHashCode`, and operator overloads.
