# Differentiation, Claim Audit & Honest Test

> **Audit Version:** 2.0 | **Date:** 2026-08-10  
> **This document is intentionally adversarial. No marketing optimization.**

---

## §1 — Differentiators

### Strong Differentiators (Hard to Replicate)

These capabilities require significant generator engineering effort and are not found in any primary competitor.

| Differentiator | Evidence | Why Hard to Replicate |
|:---|:---|:---|
| **IUtf8SpanParsable<T> generated (NET8+)** | src:StringPrimitiveGenerator.Parsing.cs:152-206 | Requires UTF-8 decode pipeline with stackalloc/ArrayPool + NFC. SuperStrong.Types mentions this but evidence of completeness is thin. |
| **ISpanFormattable + IUtf8SpanFormattable generated** | cap:capability-matrix.md | Generated TryFormat into caller-provided Span<char>. Not found in Vogen/STI/THK. |
| **Declarative normalization ([Trim], [LowerCase], [UpperCase])** | src:StringPrimitiveGenerator.cs | Attribute-driven, source-generated, zero-annotation on domain layer. No competitor has this. |
| **NFC Unicode normalization (SEC-004) on all paths** | src:StringPrimitiveGenerator.Parsing.cs:39,62,112 | Security-correct; prevents Unicode homoglyph attacks. No competitor normalizes to FormC. |
| **Security gates (SEC-001 through SEC-006)** | cap:capability-matrix.md | 4096-char limit, NonBacktracking regex, 100ms timeout, NFC normalization, no-PII errors, ArrayPool limits. Zero competitors have any of these. |
| **15 semantic string shortcuts + 15 numeric shortcuts** | src:StringPrimitiveGenerator.cs:19-26 + NumericPrimitiveGenerator.cs:14-20 | Domain-specific attributes with built-in rules. Saves 10-50 lines of code per type. No competitor has this. |
| **Date primitive ([DatePrimitive])** | src:DatePrimitiveGenerator.cs | DateOnly/DateTime-backed domain primitive with ISpanParsable. No competitor targets this natively. |
| **Auto-discovery integrations** | README + cap:integration section | EF Core and Dapper converters registered without per-type attributes. No competitor auto-discovers. |
| **Architecture Decision Records** | docs/adr/ | Formal ADR process for library evolution. No competitor has this level of governance documentation. |

---

### Medium Differentiators (Useful but Replicable)

| Differentiator | Evidence | Why Replicable |
|:---|:---|:---|
| **Mapster source-generated integration** | cap:Mapster=✅ | Competitors could add a Mapster package. Medium effort for them. |
| **Dedicated OpenAPI package** | src:dotnet-primitive-openapi/ | THK has Swashbuckle integration. Vogen/STI could add this. |
| **TryCreate(out result, out error) pattern** | src:all generators | Zero-allocation vs Result<T>. Vogen could add out overloads. The pattern itself is unique; but competitors could copy it. |
| **Multi-property Value Object** | src:ValueObjectGenerator.cs | THK already has this. Vogen does not — medium effort for Vogen. |
| **Dapper auto-registration** | cap:Dapper=✅ | Vogen/STI have TypeHandler but not auto-registration. Medium replication effort. |
| **Dedicated Analyzers project** | src:EricksonLopez.DomainPrimitives.Analyzers/ | Vogen has analyzers too. Quality comparison unknown. |

---

### Weak Differentiators (Claims That Sound Different But Aren't)

| Differentiator | Reality |
|:---|:---|
| **"Source Generator-driven"** | ALL major competitors use IIncrementalGenerator. This is table stakes in 2026. Drop as primary differentiator. |
| **"Native AOT Compatible"** | ALL major competitors (Vogen, THK, STI) are AOT-compatible. Only Ardalis.SmartEnum lags. Drop as differentiator. |
| **"Strictly valid domain model"** | ALL competitors validate on construction. "Strictly valid" is the definition of a Value Object. Drop. |
| **"Deep Ecosystem Integrations"** | Vogen has EF Core + Dapper + Newtonsoft + STJ + ASP.NET. STI has the same minus Newtonsoft. "Deep integrations" is not unique. |

---

### False Differentiators (MUST Be Removed from Marketing)

| False Claim | Why False | Correct Action |
|:---|:---|:---|
| **"Zero-allocation"** (unqualified) | Parsing through normalization ALWAYS allocates 1 string for NFC+storage. URL validation allocates. Error paths allocate. | Replace with "Allocation-minimized hot paths" and publish benchmarks. |
| **"Source Generator-driven library"** (as primary tagline) | Vogen, THK, STI are all source generator-driven. | Replace with "BCL-first" or "UTF-8-native" or "AOT-first" |
| **"15-year forward-compatible horizon"** | Unprovable claim. No library can guarantee compatibility for 15 years. | Soften to "Built for .NET 10+ and AOT-first architectures with a long-term design horizon." |
| **"No Result<T> heap allocations"** | Technically true (no Result<T>), but misleading because NFC normalization DOES allocate a string. | Narrow the claim: "No Result<T> wrapper allocations. The out-based TryCreate pattern is zero-allocation on success without normalization." |

---

## §2 — README Claims Audit

| Claim | Evidence Status | Risk | Classification | Required Action |
|:---|:---:|:---:|:---:|:---|
| "High-performance" | Partially Proven | High | Partially proven | Requires BenchmarkDotNet results vs Vogen and raw types. Without benchmarks, this is a marketing claim only. |
| "Zero-allocation" | Partially FALSE | Critical | Partially proven | Must be narrowed to specific paths. Current wording is misleading. See §3 for path-by-path audit. |
| "Source Generator-driven" | Proven | Low | Proven | True, but not a differentiator. Keep as technical description, not as tagline. |
| "DDD consistency" | Mostly proven | Medium | Mostly proven | Strong for VO/StrongId/SmartEnum. Weak: no Entities, no DUs. Needs scope qualification. |
| "Auto-Discovery Integrations" | Mostly proven | Low | Mostly proven | Claim is supported by architecture. Needs one cookbook page showing the DX difference. |
| "Deep BCL Integration" | Proven | Low | Proven | ISpanParsable/IUtf8SpanParsable/IUtf8SpanFormattable confirmed in source. This is a genuine differentiator. KEEP. |
| "15-Year Horizon" | Unsupported | High | Marketing claim | Replace with specific technical statement about AOT-first design. |
| "IUtf8SpanParsable<T>" | Proven | Low | Proven | Confirmed in StringPrimitiveGenerator.Parsing.cs:152-206. Strong unique claim. |
| "ArrayPool<char> for UTF-8 decoding" | Proven | Low | Proven | Confirmed in source. |
| "No Result<T> heap allocations on validation" | Proven (narrow) | Medium | Mostly proven | True for TryCreate. But normalization allocates. Must qualify. |

---

## §3 — Zero-Allocation Audit (Aggressive)

> Evidence source: src/StringPrimitiveGenerator.Parsing.cs, StringPrimitiveGenerator.Validation.cs

### Path-by-Path Analysis

#### Success Path — TryCreate(string value)
```
Allocation analysis:
  - value is a pre-existing string (no new alloc for string itself)
  - Normalization: depends on [Trim]/[LowerCase]/[UpperCase]
    - No normalization: string passed through — 0 new allocs
    - [Trim]: string.Trim() = 1 string alloc (new trimmed string)
    - [LowerCase]: string.ToLowerInvariant() = 1 string alloc
    - NFC: .Normalize(FormC) = 1 string alloc (always, per SEC-004)
  - Validation: TryValidate(value) — struct return, 0 allocs on success
  - Out result assignment: struct on stack — 0 allocs

Verdict: NOT zero-allocation when normalization is involved.
         ZERO ALLOCATION only when: no [Trim], no [LowerCase]/[UpperCase], value is already NFC-normalized.
         Since NFC is ALWAYS applied per SEC-004: at minimum 1 alloc for .Normalize(FormC).
```

#### Failure Path — TryCreate(string value) — validation fails
```
Allocation analysis:
  - Error message: $"{info.TypeName} must be at least {info.MinLength.Value} character(s). Got {value.Length}."
    = 1 string interpolation alloc

Verdict: 1 allocation on failure. Not zero-allocation.
         Acceptable — failure paths don't need to be zero-alloc.
         BUT: if DP wants "zero-allocation failure path", needs static error objects.
```

#### Parse Path — TryParse(ReadOnlySpan<char>) — no case normalization — ≤256 chars
```
Allocation analysis:
  - stackalloc char[s.Length]: on stack — 0 heap allocs
  - .Normalize(FormC).ToString(): 1 string alloc — unavoidable (NFC + storage)
  - TryValidate(normalized): 0 allocs
  - result = new TypeName(normalized): struct — 0 allocs

Verdict: 1 unavoidable heap allocation (NFC + stored string).
         Represents "allocation-minimized" not "zero-allocation".
```

#### Parse Path — TryParse(ReadOnlySpan<byte>) UTF-8 — ≤256 chars
```
Allocation analysis:
  - GetMaxCharCount(): O(1) calculation — 0 allocs
  - stackalloc char[maxCount]: on stack — 0 heap allocs
  - GetChars(utf8, chars): writes to span — 0 heap allocs
  - delegates to TryParse(ReadOnlySpan<char>): 1 string alloc (see above)

Verdict: 1 unavoidable heap allocation.
```

#### JSON Path — STJ Deserialization
```
Claim: reads directly from Utf8JsonReader.ValueSpan
Status: VERIFIED — src:GeneratorHelpers.cs:45-51

Confirmed implementation (NET8+):
  if (!reader.HasValueSequence) {
    if ({TypeName}.TryParse(reader.ValueSpan, null, out var spanResult))
      return spanResult;
  }
  // Fallback: GetString() for fragmented/large values
  var raw = reader.GetString()!;
  return {TypeName}.Create(raw);

Allocation count: 1 (NFC normalization + storage string).
No extra string from GetString() on the fast path (spans only).
Net saving vs GetString() path: 0-1 allocs (1 string elimination).
```

#### URL Validation Path
```
Allocation analysis:
  - Uri.TryCreate(value.ToString(), UriKind.Absolute, out var uri)
    - value.ToString(): 0 (value is already string)
    - Uri.TryCreate: allocates Uri object on success

Verdict: 1 Uri allocation on URL validation success. Not zero-allocation.
         Minor — Uri is GC'd immediately if only used for scheme check.
```

#### EF Core Path
```
Allocation analysis:
  - ValueConverter<TModel, TProvider>.ConvertToProvider(TModel value)
    - value.Value (string/int/Guid): no alloc (primitive access)
    - Returns underlying primitive: struct/string — depends on type

Verdict: 0-1 allocs depending on underlying type. Effectively zero for struct-backed types.
```

#### Dapper Path
```
Allocation analysis:
  - TypeHandler<T>.SetValue(IDbDataParameter, object): boxes T if struct
    - Boxing: 1 alloc for struct boxing
    - TypeHandler.Parse: object cast then unbox — 0 additional allocs

Verdict: 1 boxing alloc on SetValue. Unavoidable in Dapper's API. Not zero-allocation.
         Acceptable — this is a Dapper API limitation, not DP's fault.
```

### Corrected Zero-Allocation Claim Matrix

| Path | Actual Status | Correct Claim |
|:---|:---|:---|
| TryCreate success (no normalization, pre-NFC value) | ✅ Zero allocation | Valid |
| TryCreate success (with Trim/Lower/Upper) | ⚠️ 1-2 allocs | "Minimal allocation" |
| TryCreate failure | ⚠️ 1 alloc | "Zero-allocation success path" |
| TryParse(span<char>) ≤256 | ⚠️ 1 alloc (NFC) | "Allocation-minimized" |
| TryParse(span<byte>) ≤256 | ⚠️ 1 alloc (NFC) | "Allocation-minimized" |
| JSON deserialize via ValueSpan | ✅ **VERIFIED** — 1 alloc (NFC+storage) | "Allocation-minimized JSON path" |
| EF Core materialization | ✅ Effectively zero | Valid for struct types |
| Dapper SetValue | ⚠️ 1 boxing alloc | Dapper limitation |

---

## §4 — Source Generator Architecture Audit

### Is DP "Generator-First" or "Attribute + Generator"?

**Answer: Attribute + Generator (with excellent generator quality)**

A truly "generator-first" library would generate types without ANY user-side annotation. DP requires `[StringPrimitive]`, `[StrongId<Guid>]`, etc. This is the "Attribute + Generator" pattern — the same as Vogen, THK, and STI.

This is NOT a criticism. It's the correct design. Zero-annotation generation would require magic conventions (fragile).

### Generator Quality Assessment

| Metric | DP | VOG | THK | STI |
|:---|:---:|:---:|:---:|:---:|
| IIncrementalGenerator | ✅ | ✅ | ✅ | ✅ |
| SyntaxProvider predicate (fast filter) | ✅ | ✅ | ✅ | ✅ |
| CancellationToken support | ✅ | ✅ | ✅ | ✅ |
| Semantic model usage (not syntax-only) | ✅ | ✅ | ✅ | ✅ |
| Deterministic output | ✅ | ✅ | ✅ | ✅ |
| Analyzer integration | ✅ (16 rules) | ✅ | ✅ | 🟡 |
| Generated code readability | ✅ | ✅ | ✅ | ✅ |
| Generated code debugging | ✅ | ✅ | ✅ | ✅ |
| Auto-discovery (no per-type attributes) | ✅ | ❌ | ❌ | ❌ |

**Conclusion:** DP's generator quality is comparable to the best-in-class. Auto-discovery is a genuine technical advantage.

---

## §5 — Honest Test

### "If you were an expert DDD/.NET developer in 2026, would you choose DomainPrimitives over Vogen, Thinktecture, or StronglyTypedId?"

**Answer: DEPENDE (Depends on context)**

#### You WOULD choose DomainPrimitives if:
- You are starting a new .NET 8+ / .NET 10 project
- You want UTF-8-native parsing (HTTP, gRPC, Kafka payloads)
- You want declarative normalization without boilerplate
- You care about ReDoS security in string validation
- You need Mapster source-generated mapping
- You want auto-discovered EF Core / Dapper registration
- You want 15 domain-specific string types + 15 numeric types out of the box
- You need a DatePrimitive

#### You WOULD NOT choose DomainPrimitives if:
- You need Discriminated Unions (→ Thinktecture)
- You want the library with the largest community and most examples (→ Vogen)
- You only need strongly typed IDs (→ StronglyTypedId is simpler)
- You want class-based Value Objects (→ Vogen or Thinktecture)
- You don't trust a library without published benchmark results (→ Vogen has them; DP partial)
- You need custom underlying types beyond `string/int/long/Guid` (→ Vogen supports any T)

---

### "What is the main reason NOT to choose DomainPrimitives?"

**The library is new, untested in production at scale, and has no public benchmark results.**

All of its technical claims (zero-allocation, high performance, BCL-native) are credible based on source code analysis, but **unproven by reproducible benchmarks**. A mature .NET team evaluating libraries for production will ask: "Show me the benchmarks." Until that exists, Vogen (with its established community and proven benchmark table) wins the evaluation.

---

### "What is the main technical moat?"

**IUtf8SpanParsable<T> + Declarative Normalization + Security Gates**

The combination of:
1. Native UTF-8 parsing (stackalloc/ArrayPool for decode)
2. NFC normalization on all paths (Unicode security)
3. NonBacktracking regex with timeout (ReDoS defense)
4. Declarative [Trim]/[LowerCase]/[UpperCase] attributes

...creates a security-first, performance-correct domain primitive that no competitor offers. This is the genuine moat.

---

### "What is the main architectural risk?"

**readonly record struct everywhere.**

`record struct` with positional properties has a default parameterless constructor that bypasses validation. DP's analyzer (DP0001+) warns about this, but:
- A user who ignores the analyzer can create invalid primitives via `default(MyEmail)`
- JSON deserialization using `Activator.CreateInstance()` can bypass constructors
- Dapper can set properties directly on structs

All of these risks are documented and mitigated, but they represent an inherent tension in the struct-based approach that Vogen's class-based option avoids.

---

### "What is the main DX risk?"

**Learning curve for a new library without sufficient documentation.**

DP has 6 generators, 15+ string shortcuts, 15+ numeric shortcuts, security gates, normalization, and 5 integration packages. New users face:
- No migration guide from Vogen/STI
- Sparse cookbook
- No published benchmarks to validate performance claims
- No community forum or Discord

A developer comparing DP to Vogen will see Vogen's extensive documentation, GitHub discussions, and blog posts. DP currently has none of this. **DX risk is HIGH.**

---

### "What is the main 15-year maintenance risk?"

**Generator complexity at scale.**

DP has 6 generators, each with partial files for factory/parsing/formatting/normalization/validation/operators. This is:
- ~100KB of generator C# code
- Complex conditional compilation (#if NET8_0_OR_GREATER)
- Multiple interaction points per feature

As .NET evolves (C# 14, 15, 16...), each new language feature may require updating all generators. Thinktecture has maintained their generators for 5+ years across many .NET versions — this is the benchmark for longevity. DP's generator surface is larger and more complex. **Complexity = maintenance risk.**

The mitigation: the strong test suite (mutation testing, Stryker config) and ADR documentation are positive signals. But the risk is real.

---

## §6 — Positioning Adjustment Recommendations

### Remove from all marketing material:
1. "Zero-allocation" (unqualified)
2. "Source Generator-driven" (as differentiator)
3. "15-Year Horizon" (replace with specific technical statement)
4. "Strictly valid" (table stakes)

### Strengthen in marketing:
1. "UTF-8-native domain primitives" — unique, provable, high-value
2. "Allocation-minimized hot paths" — accurate, defensible
3. "30 built-in domain types" (string + numeric shortcuts) — concrete, unique
4. "ReDoS-resistant by default" — unique security posture
5. "Auto-discovered integrations — zero domain contamination" — DDD purity angle
6. "ISpanFormattable + IUtf8SpanFormattable generated" — BCL-native angle
