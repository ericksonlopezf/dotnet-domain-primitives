# adr-036: Case-Insensitive Name Parsing for `[SmartEnum<T>]`

## Status
Accepted

## Context
When parsing Smart Enum names from HTTP route parameters, query strings, or external JSON payloads, incoming values frequently vary in casing (e.g., `pending`, `Pending`, `PENDING`). Developers need an explicit, ergonomic API to parse enum members either with exact casing or case-insensitively.

## Decision
Generate the following overloads for name-based lookup on `[SmartEnum<T>]`:
1. `TryFromName(string name, out TSelf result)`: Default case-insensitive lookup.
2. `TryFromName(string name, bool ignoreCase, out TSelf result)`: Explicit case-sensitivity control.
3. `FromName(string name, bool ignoreCase = false)`: Throws `ArgumentException` (or configured custom exception) on lookup failure.

## Consequences
### Positive
- High ergonomics for web APIs, JSON parsers, and CLI applications.
- Zero reflection: Iterates over the static compile-time generated `All` array.
- Native AOT compliant and allocation-free.
