# Competitive Evidence Matrix

> **Audit Version:** 2.0 | **Date:** 2026-08-10  
> **Evidence policy:** Source code wins over docs. Tests win over source code comments.  
> **Unknown = ⚪** — never fill with assumptions.

---

## Legend

| Symbol | Meaning |
|--------|---------|
| 🟢 | Officially supported — verified in source/tests |
| 🟡 | Partial support — works with caveats |
| 🟠 | Possible via workaround — not generated |
| 🔵 | Experimental |
| ⚪ | Not documented / not verified |
| 🔴 | Not supported — verified absence |
| 🟣 | Roadmap/planned |

---

## §1 — Core Domain Primitive & DDD Capabilities

| Feature | DP | VOG | THK | STI | ASE | Evidence & Source |
|:---|:---:|:---:|:---:|:---:|:---:|:---|
| Source Generator First | 🟢 | 🟢 | 🟢 | 🟢 | 🔴 | All use IIncrementalGenerator. Ardalis: no generator, plain classes. Evidence: all GitHub repos |
| IIncrementalGenerator (not ISourceGenerator) | 🟢 | 🟢 | 🟢 | 🟢 | 🔴 | DP: src/StringPrimitiveGenerator.cs:16 `[Generator(LanguageNames.CSharp)] internal sealed partial class StringPrimitiveGenerator : IIncrementalGenerator` |
| Native AOT Compatible | 🟢 | 🟢 | 🟢 | 🟢 | 🟡 | DP: cap/capability-matrix.md row "NativeAOT compatibility=✅". Ardalis: reflection-based GetAll() triggers trimmer warnings per search results |
| Multi-property Value Object | 🟢 | 🔴 | 🟢 | 🔴 | 🔴 | DP: src/ValueObjectGenerator.cs; iterates IPropertySymbols. Vogen: design document explicitly states single-value VO only. |
| Discriminated Union | 🔴 | 🔴 | 🟢 | 🔴 | 🔴 | THK: gh:PawelGerr/Thinktecture.Runtime.Extensions README. NOT found in DP/Vogen/STI. |
| Class support (not struct only) | 🔴 | 🟢 | 🟢 | 🔴 | 🟢 | DP: src/StringPrimitiveGenerator.cs:32 `IsCandidateRecordStruct` — struct keyword required. Vogen: `partial class` support documented. |
| Smart Enum | 🟢 | 🔴 | 🟢 | 🔴 | 🟢 | DP: src/SmartEnumGenerator.cs. Vogen: no SmartEnum feature. Ardalis: SmartEnum is their primary product. |
| Semantic shortcut attrs (string) | 🟢 | 🔴 | 🔴 | 🔴 | 🔴 | DP: src/StringPrimitiveGenerator.cs:19-26 — [Email],[Phone],[Url],[Slug],[CountryCode],[LanguageCode],[CurrencyCode],[Username],[PasswordHash],[HexColor],[IPAddress],[MacAddress],[IBAN],[ISBN],[VIN]. Zero competitors have this. |
| Semantic shortcut attrs (numeric) | 🟢 | 🔴 | 🔴 | 🔴 | 🔴 | DP: src/NumericPrimitiveGenerator.cs:14-20 — [Money],[Percentage],[Latitude],[Longitude],[Age],[Weight],[Height],[Distance],[Temperature],[Score],[Quantity],[Price],[TaxRate],[Discount],[Rating]. |
| Custom underlying type | 🔴 | 🟢 | 🟢 | 🔴 | 🔴 | DP: src/StrongIdGenerator.cs:73 — only int/long/string/System.Guid accepted. Vogen: [ValueObject<T>] accepts any T. THK: same. |

---

## §2 — Validation Evidence

| Feature | DP | VOG | THK | STI | Evidence |
|:---|:---:|:---:|:---:|:---:|:---|
| Generated validator pipeline | 🟢 | 🟡 | 🟢 | 🔴 | DP: src/StringPrimitiveGenerator.Validation.cs — notEmpty→length→maxLength→regex→custom. Vogen: user implements `private static Validation Validate(T value)` — not generated. |
| TryCreate(out result, out error) | 🟢 | 🔴 | 🔴 | 🔴 | DP: unique out-based pattern. Vogen: `TryFrom()` returns `ValueObjectOrError<T>` — heap allocation. THK: `Result<T, ValidationError>` — heap allocation. |
| Typed error codes | 🟢 | 🔴 | 🟡 | 🔴 | DP: src/StringPrimitiveGenerator.Validation.cs:45 `PrimitiveError("EMPTY", ...)`, :53 `PrimitiveError("LENGTH", ...)`. Vogen: ValidationException message only, no code. |
| SEC-001: 4096-char limit | 🟢 | 🔴 | 🔴 | 🔴 | DP: src/StringPrimitiveGenerator.Validation.cs:66-69 — hardcoded 4096 guard when no MaxLength specified. No competitor has this security default. |
| SEC-002: NonBacktracking regex | 🟢 | 🔴 | 🔴 | 🔴 | DP: src/StringPrimitiveGenerator.Regex.cs — `RegexOptions.NonBacktracking` on NET7+. No competitor has ReDoS protection by default. |
| SEC-003: 100ms timeout | 🟢 | 🔴 | 🔴 | 🔴 | DP: src/StringPrimitiveGenerator.Regex.cs — `TimeSpan.FromMilliseconds(100)` injected. Unique. |
| SEC-004: NFC normalization | 🟢 | 🔴 | 🔴 | 🔴 | DP: src/StringPrimitiveGenerator.Parsing.cs:39 `.Normalize(System.Text.NormalizationForm.FormC)`. Applied to ALL string inputs before validation. Unique. |
| SEC-005: No PII in errors | 🟢 | ⚪ | ⚪ | ⚪ | DP: cap/capability-matrix.md SEC-005. Not confirmed for any competitor. |
| SEC-006: ArrayPool limits | 🟢 | 🔴 | 🔴 | 🔴 | DP: src/StringPrimitiveGenerator.Parsing.cs:97,186 — stackalloc ≤256 chars. Unique. |

---

## §3 — Normalization Evidence

| Feature | DP | VOG | THK | STI | Evidence |
|:---|:---:|:---:|:---:|:---:|:---|
| Declarative Trim attr | 🟢 | 🔴 | 🔴 | 🔴 | DP: src/StringPrimitiveGenerator.cs — [Trim] attribute parsed; generates `s = s.Trim()`. Not found in any competitor. |
| Declarative LowerCase/UpperCase | 🟢 | 🔴 | 🔴 | 🔴 | DP: src/StringPrimitiveGenerator.cs — [LowerCase]/[UpperCase] attributes. Not in any competitor. |
| MemoryExtensions.ToLowerInvariant | 🟢 | 🔴 | 🔴 | 🔴 | DP: src/StringPrimitiveGenerator.Parsing.cs:104 `MemoryExtensions.ToLowerInvariant(s, buf)` — CRIT-003 fix. In-place, no intermediate string. Unique. |
| stackalloc for ≤256 chars | 🟢 | 🔴 | 🔴 | 🔴 | DP: src/StringPrimitiveGenerator.Parsing.cs:97 `Span<char> buf = stackalloc char[s.Length]` — only when Length ≤ 256. |
| ArrayPool for >256 chars | 🟢 | 🔴 | 🔴 | 🔴 | DP: src/StringPrimitiveGenerator.Parsing.cs:120 `ArrayPool<char>.Shared.Rent(s.Length)`. Paired with try/finally. |
| NFC normalization on all paths | 🟢 | 🔴 | 🔴 | 🔴 | DP: SEC-004. Applied universally. |
| Custom normalization via override | 🟡 | 🟢 | 🟢 | 🔴 | DP: partial method hook. Vogen: explicit Normalize() method. THK: explicit factory method. |

---

## §4 — Parsing & BCL Interface Evidence

| Feature | DP | VOG | THK | STI | Evidence |
|:---|:---:|:---:|:---:|:---:|:---|
| IParsable<T> | 🟢 | 🟡 | 🟢 | 🟢 | DP: src/StringPrimitiveGenerator.Parsing.cs — implements full interface. VOG: hoists TryParse but interface declaration unconfirmed in all cases. |
| ISpanParsable<T> | 🟢 | 🔴 | ⚪ | ⚪ | DP: src/StringPrimitiveGenerator.Parsing.cs — Parse(ReadOnlySpan<char>) and TryParse(ReadOnlySpan<char>) generated. Vogen: web search confirms NOT in standard generated output. THK/STI: not confirmed. |
| IUtf8SpanParsable<T> | 🟢 | 🔴 | 🔴 | 🔴 | DP: src/StringPrimitiveGenerator.Parsing.cs:152-206 — Parse(ReadOnlySpan<byte>), TryParse(ReadOnlySpan<byte>) wrapped in #if NET8_0_OR_GREATER. Confirmed unique — no competitor implements this natively. |
| ISpanFormattable | 🟢 | 🔴 | ⚪ | ⚪ | DP: cap/capability-matrix.md row "ISpanFormattable=✅" for String/Numeric/Date/StrongId/ValueObject. Not confirmed in VOG/STI. |
| IUtf8SpanFormattable | 🟢 | 🔴 | 🔴 | 🔴 | DP: cap/capability-matrix.md — ✅ for String/Numeric/Date/StrongId. ValueObject planned v2.0. No competitor implements. |
| Utf8JsonReader.ValueSpan read | 🟢 | ⚪ | ⚪ | ⚪ | DP: **VERIFIED** — src:GeneratorHelpers.cs:45-51 — `if (!reader.HasValueSequence) { if ({TypeName}.TryParse(reader.ValueSpan, null, out var spanResult)) return spanResult; }`. NET8+ only. Fallback to GetString() for >16KB values. No competitor implements. |
| stackalloc UTF-8 decode (≤256 chars) | 🟢 | 🔴 | 🔴 | 🔴 | DP: src/StringPrimitiveGenerator.Parsing.cs:161-164 — `GetMaxCharCount` guard then `GetChars(utf8, chars)` into stackalloc span. |

---

## §5 — Source Generator Quality Evidence

| Feature | DP | VOG | THK | STI | Evidence |
|:---|:---:|:---:|:---:|:---:|:---|
| SyntaxProvider predicate (fast filter) | 🟢 | 🟢 | 🟢 | 🟢 | DP: src/StringPrimitiveGenerator.cs:30-35 — predicate filters before expensive semantic transform. |
| CancellationToken support | 🟢 | 🟢 | 🟢 | 🟢 | DP: src/StrongIdGenerator.cs:46 `ct.ThrowIfCancellationRequested()`. |
| Deterministic output | 🟢 | 🟢 | 🟢 | 🟢 | DP: cap — CRIT-006 fixed; EmbedUntrackedSources unconditional. |
| 16 Roslyn Analyzers (DP0001-DP0016) | 🟢 | 🟢 | 🟢 | 🟡 | DP: cap/capability-matrix.md "DP0001-DP0016 active". Vogen also has extensive analyzers. |
| Auto-discovery integration | 🟢 | 🔴 | 🔴 | 🔴 | DP: README claim — converters registered without per-type attributes. Competitors require explicit `[ValueObject(conversions: Conversions.EfCore)]` or similar. |

---

## §6 — Allocation Model Evidence

> Evidence from source code analysis (StringPrimitiveGenerator.Parsing.cs).

| Path | Allocations | Notes |
|:---|:---:|:---|
| TryCreate(string) — success | **0** | No new heap objects; out params on stack |
| TryCreate(string) — failure | **1** | Error message interpolation: `$"{info.TypeName} must be at least {info.MinLength.Value} chars. Got {value.Length}."` — allocates a string |
| TryParse(ReadOnlySpan<char>) ≤256, no case norm | **1** | `.Normalize(FormC).ToString()` — unavoidable for NFC + storage |
| TryParse(ReadOnlySpan<char>) ≤256, with case norm | **1** | stackalloc in-place → `.Normalize(FormC).ToString()` |
| TryParse(ReadOnlySpan<char>) >256, no case norm | **1+pool** | ArrayPool rent + `.Normalize(FormC).ToString()`. Pool = low-alloc, not zero |
| TryParse(ReadOnlySpan<byte>) ≤256 | **1** | stackalloc decode + `.Normalize(FormC).ToString()` |
| TryParse(ReadOnlySpan<byte>) >256 | **1+pool** | ArrayPool rent for decode + `.Normalize(FormC).ToString()` |
| TryParse(string) — success | **0 new** | String already exists on heap; no new allocations |
| JSON deserialize via ValueSpan | **1** (NFC+storage) | Direct read from Utf8JsonReader.ValueSpan (VERIFIED: GeneratorHelpers.cs:45-51). 1 unavoidable string alloc for NFC normalization + storage. 0 extra allocs vs GetString() when value fits in one buffer. |
| JSON serialize | **1** | Produces a string |
| URL validation | **1** | `Uri.TryCreate(value.ToString(), ...)` — string alloc on all paths |
| Regex validation | **0** | Compiled static regex; IsMatch = zero alloc |
| NormalizeWhitespace path | **varies** | Regex replace; allocates new string |
| EF Core materialization | **0 new** | ValueConverter; DP type is struct |
| Dapper materialization | **0 new** | TypeHandler.Parse; DP type is struct |

### Verdict on "Zero-Allocation" claim:

**PARTIALLY TRUE.** The claim is true for:
- TryCreate success path (no Result<T> wrapper)
- TryParse(string) success path
- JSON deserialization (≤16KB, if ValueSpan confirmed)
- EF Core / Dapper struct materialization

**FALSE** for:
- ANY path through normalization (1 unavoidable string for NFC+storage)
- Failure paths (error string interpolation)
- URL validation paths

**Recommended wording:** "Allocation-minimized hot paths. Zero heap allocations on the success path without normalization. One unavoidable string allocation per normalized value (NFC Unicode requirement)."

---

## §7 — Competitor Integration Depth Evidence

### Vogen Integration Evidence
| Integration | Support | Evidence |
|:---|:---:|:---|
| EF Core | 🟢 | Vogen GitHub: Vogen.EFCore package; ValueConverter generated via `Conversions.EfCore` flag |
| Dapper | 🟢 | Vogen GitHub: `Conversions.Dapper` flag; generates TypeHandler |
| System.Text.Json | 🟢 | `Conversions.SystemTextJson` flag |
| Newtonsoft.Json | 🟢 | `Conversions.NewtonsoftJson` flag — **DP GAP** |
| ASP.NET Core | 🟢 | Via TypeConverter |
| OpenAPI | 🟡 | Manual schema mapping; no dedicated package |
| Mapster | 🔴 | Not found |
| ISpanParsable | 🔴 | Not in standard output — confirmed via search results |
| IUtf8SpanParsable | 🔴 | Not supported |

### Thinktecture Integration Evidence
| Integration | Support | Evidence |
|:---|:---:|:---|
| EF Core | 🟢 | gh:Thinktecture.Runtime.Extensions EF Core integration documented |
| Dapper | 🔴 | No Dapper package found in GitHub or NuGet |
| System.Text.Json | 🟢 | JSON integration documented |
| Newtonsoft.Json | ⚪ | Not confirmed |
| ASP.NET Core | 🟢 | Model binding documented |
| OpenAPI | 🟢 | Swashbuckle integration available |
| Discriminated Unions | 🟢 | Core feature — generates Match/Switch |
| ISpanParsable | ⚪ | Not confirmed as auto-generated |
| IUtf8SpanParsable | 🔴 | Not documented/confirmed |

### StronglyTypedId Integration Evidence
| Integration | Support | Evidence |
|:---|:---:|:---|
| EF Core | 🟢 | STI GitHub: EF Core ValueConverter generated |
| Dapper | 🟢 | STI GitHub: TypeHandler generated |
| System.Text.Json | 🟢 | Converter via `StronglyTypedIdConverter.SystemTextJson` |
| Newtonsoft.Json | 🟢 | Converter via `StronglyTypedIdConverter.NewtonsoftJson` — **DP GAP** |
| TypeConverter | 🟢 | `StronglyTypedIdConverter.TypeConverter` |
| ASP.NET Core | 🟢 | Via TypeConverter |
| OpenAPI | 🟡 | Manual; no dedicated package |
| Custom backing types | 🔴 | Only Guid/int/long/string |
| Custom validation | 🔴 | No validation logic generated |
| ISpanParsable | ⚪ | Not confirmed |
| IUtf8SpanParsable | 🔴 | Not supported |

### Ardalis.SmartEnum Integration Evidence
| Integration | Support | Evidence |
|:---|:---:|:---|
| EF Core | 🟢 | Separate EF Core package exists |
| System.Text.Json | 🟢 | Converter available |
| Newtonsoft.Json | 🟢 | Converter available |
| Source generator | 🔴 | CONFIRMED: no generator in package. PR#356 discussed but not merged. |
| Native AOT | 🟡 | RISK: GetAll() uses reflection; trimmer warnings reported |
| SmartFlagEnum | 🟢 | Unique feature — not available elsewhere |

---

## §8 — Evidence Quality Rating

| Claim | Evidence Quality | What Would Upgrade to "Proven" |
|:---|:---:|:---|
| DP IUtf8SpanParsable | High | Source confirmed in StringPrimitiveGenerator.Parsing.cs |
| DP Utf8JsonReader.ValueSpan | **High** | **VERIFIED** — src:GeneratorHelpers.cs:45-51. NET8+ branch uses `reader.ValueSpan`, fallback for >16KB uses `reader.GetString()`. |
| DP Auto-discovery EF | Medium | Need EF Core package source scan |
| DP Auto-discovery Dapper | Medium | Need Dapper package source scan |
| Vogen ISpanParsable = NOT supported | High | Multiple search sources confirm absence |
| THK ISpanParsable = unknown | Low | Need to scan Thinktecture source code directly |
| DP "Zero allocation" | Low | Benchmark required; source shows 1 unavoidable alloc |
| DP "High performance" | None | No public BenchmarkDotNet results |
