# adr-023: Reject XML Serialization Support

**Date:** 2026-08-10
**Status:** Accepted
**Authors:** Core maintainers
**Related audit items:** REJECT-004 (feature-gaps.md)

---

## Context

XML serialization support (via `System.Xml.Serialization` or `DataContractSerializer`) has been
requested for compatibility with legacy services, SOAP endpoints, and older WCF-based systems.

`EricksonLopez.DomainPrimitives` currently generates only `System.Text.Json` converters.

---

## Decision

**`EricksonLopez.DomainPrimitives` will not add XML serialization support.**

---

## Rationale

### 1. XML is not a .NET 8+ standard

The library explicitly targets .NET 8+ (minimum TFM: `net8.0`, per adr-016). Microsoft's
official stance since .NET Core 3.0 is that `System.Text.Json` is the standard JSON serializer
and XML/SOAP is a legacy technology. New APIs in ASP.NET Core, Minimal APIs, gRPC, and
SignalR are all JSON-first.

The library's differentiation is in UTF-8-native parsing, AOT compatibility, and BCL interface
depth — all of which are meaningless in an XML context.

### 2. AOT compatibility is incompatible with most XML serialization approaches

`System.Xml.Serialization.XmlSerializer` uses runtime code generation (`Reflection.Emit`) and
is therefore incompatible with Native AOT. While `DataContractSerializer` has limited AOT
support, adding it would create a second-class serialization path that:

- Cannot be verified by the AOT CI gate.
- Requires `[RequiresDynamicCode]` annotations.
- Undermines the library's AOT-first guarantee.

### 3. Maintenance cost is disproportionate to user impact

XML serialization requires:
- `IXmlSerializable` interface implementation (or `XmlConverter<T>` for newer patterns).
- Round-trip correctness tests for all primitive types.
- Handling XML namespaces, attributes vs elements, and SOAP envelope wrapping.

Each generated type would need additional generated code for every XML target format. This
multiplies the generator surface and test matrix without serving the library's target audience
(.NET 8+ greenfield projects).

### 4. The only valid XML scenario is not owned by this library

The primary remaining use case for XML in .NET is SOAP/WCF interoperability. This requires:
- A full WCF client stack.
- WSDL-generated proxy types.
- Service references.

None of these are within the scope of a domain primitive library. The infrastructure concern
(SOAP) belongs in the Service Adapter layer, not in domain types.

---

## Alternatives Considered

| Alternative | Rejected because |
|-------------|-----------------|
| Optional `EricksonLopez.DomainPrimitives.Xml` package | The package cannot be AOT-compatible. Would require `[RequiresDynamicCode]`, contradicting the CI AOT gate. |
| `IXmlSerializable` implementation via partial methods | Pushes the problem to the user, adding boilerplate. No generator value. |
| DataContractSerializer support only (no XmlSerializer) | Still requires non-AOT paths. DataContractSerializer has its own reflection requirements. |

---

## Consequences

- **Positive:** Generator surface remains bounded.
- **Positive:** AOT guarantee is not compromised.
- **Negative:** Users in SOAP/WCF scenarios cannot use this library for their XML-serialized
  types. They should use WSDL-generated proxy types or Newtonsoft.Json XML adapters in their
  infrastructure layer.
- **Documentation action:** Documented in `docs/rejected-features.md`.
