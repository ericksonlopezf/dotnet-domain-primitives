# Technical Debt Register

> **Audit Policy:** The number of Active items must not exceed 10 (Engineering Specification v4.0 §7.2).  
> **Last Updated:** 2026-08-23

---

## Active Technical Debt

The following items are open and require action in a future release.

| ID | Description | Estimated Cost | Target Version |
|----|-------------|----------------|----------------|
| TD-007 | **ValueObject `IComparable<T>`:** Composite types have no natural ordering. Design a convention (e.g., lexicographic by first property) or require consumers to implement it in their partial definition. Blocked on design decision. | 4h | v2.0.0 |
| TD-011 | **`TryParse(ReadOnlySpan<char>)` unavoidable NFC allocation.** The final `.Normalize(FormC).ToString()` call allocates once per parse on string-backed primitives. This is the minimum necessary allocation since NFC can change character count and the stored value must be `System.String`. Potential future fix: generate a pre-validated span type that defers string creation. | 8h | v2.0.0 |
| TD-012 | **`[GeneratedRegex]` not usable on generated types.** The source generator cannot emit `[GeneratedRegex]`-decorated members because `RegexGenerator` does not run on generated code. On NativeAOT, `RegexOptions.Compiled` requires JIT. Workaround: NativeAOT users can override the generated `ValidationRegex` field in their partial definition. This is a known Roslyn limitation. | 4h | v2.0.0 |
| TD-017 | **Generated code does not use C# 14 features.** `LangVersion=14` is set in `Directory.Build.props` but the generated code does not use C# 14 idioms (`params collections`, `field` keyword, etc.). This is intentional until C# 15 is finalized to avoid unnecessary churn. Re-evaluate when C# 15 lands. | 2h | v1.3.0 |

---

## Resolved Technical Debt

All items below have been closed. This archive is maintained for auditability.

| ID | Description | Resolved In | Resolution |
|----|-------------|-------------|------------|
| TD-001 | NativeAOT trimming compatibility | v1.2.0 | AOT probe app produces 0 IL3050/IL2026 warnings. Binary executes correctly. |
| TD-002 | Roslyn Analyzers performance | v1.2.0 | `ApiReviewAnalyzer` migrated to `RegisterSymbolStartAction`/`RegisterSymbolEndAction`. |
| TD-003 | `ArrayPool` limits (SEC-006) | v1.2.0 | Replaced `GetCharCount` (O(n)) with `GetMaxCharCount` (O(1)) in UTF-8 parse path. |
| TD-004 | `StrongId.RejectEmpty` default | v1.2.0 | Generator now correctly defaults `rejectEmpty = true` matching `StrongIdAttribute.RejectEmpty`. |
| TD-005 | Generator Diagnostic ID refactoring | v1.2.0 | ID range reservation comments added: DP0001-DP0099 user-facing; DP1001-DP1999 infrastructure. |
| TD-006 | `ValueObject IParsable<T>` and `IUtf8SpanParsable<T>` | v1.2.0 | rfc-0006 implemented in `ValueObjectGenerator`. |
| TD-008 | `ValueObjectGenerator` `IsRecordDeclaration` predicate too broad | CRIT-004 | Generator now uses `GeneratorShared.IsCandidateRecordStruct(node, ["ValueObject"])`. |
| TD-009 | `EventSource` and `Metrics` in `Abstractions` | v1.2.0 | `DomainPrimitiveEventSource`, `DomainPrimitivesDiagnostics`, `DomainPrimitivesMetrics` moved to Core. |
| TD-010 | `EquatableArray<T>` missing `[ExcludeFromCodeCoverage]` | v1.2.0 | Attribute added to prevent Stryker spending budget on infrastructure hash code logic. |
| TD-013 | `ValueObject IParsable<T>` RFC not filed | v1.2.0 | `rfc-0006-valueobject-iparsable.md` filed in `docs/rfcs/`. |
| TD-014 | Generator uses custom predicate instead of `ForAttributeWithMetadataName` | v1.2.0 | All 6 generators migrated. `IsReadonlyRecordStruct` is O(1). FQNs centralized in `GeneratorShared`. |
| TD-015 | API Surface Budget not measured in CI | v1.2.0 | Dedicated `API Surface Budget Gate (P2 Gate)` step added to `dotnet-build-test.yml`. |
| TD-016 | `DatePrimitive` API surface not measured | v1.2.0 | `DatePrimitive_ApiSurface_IsWithinBudget` test added; budget=37. |
| TD-018 | Newtonsoft.Json integration gap (GAP-002) | v1.1.0 | `EricksonLopez.DomainPrimitives.NewtonsoftJson` package shipped. |
| NEW-TD-A | `publish.yml` pack paths incorrect | v1.0.0 | Fixed `publish.yml` paths to match flat `src/` structure. |
| NEW-TD-B | `PackageValidationBaselineVersion` references non-existent version | v1.0.0 | Baseline removed for first release; activated at v1.0.0 post-publish. |
| NEW-TD-C | `docs/API_REFERENCE.md` (uppercase) duplicated `docs/api-reference.md` | v1.1.0 | Uppercase duplicate deleted. |
| NEW-TD-D | RFC files used two naming conventions | v1.1.0 | All files in `docs/rfcs/` renamed to lowercase `rfc-NNNN-*.md` format. |
| NEW-TD-E | `.github/CODEOWNERS` was missing | v1.0.0 | `CODEOWNERS` created assigning `@ericksonlopezf` to all paths. |
