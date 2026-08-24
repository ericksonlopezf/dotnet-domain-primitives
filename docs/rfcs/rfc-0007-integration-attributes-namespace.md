# RFC 0002: Move IntegrationAttributes to a Sub-Namespace

## Context and Problem Statement
Currently, all integration-specific attributes ([Json], [Dapper], [EFCore], etc.) are defined in the core EricksonLopez.DomainPrimitives.Abstractions assembly and reside in the root EricksonLopez.DomainPrimitives namespace. 
This violates the Separation of Concerns principle, as consumers who only want to use domain primitives without any third-party integrations are still exposed to these integration markers in IntelliSense.

## Proposed Solution
In version 2.0, we will introduce a new namespace: EricksonLopez.DomainPrimitives.Integrations.
All integration attributes will be moved to this namespace.

### Migration Path for v1.x
To avoid breaking changes before v2.0:
1. We will retain the existing attributes in the EricksonLopez.DomainPrimitives namespace.
2. We will mark them as [Obsolete("Use EricksonLopez.DomainPrimitives.Integrations.<Attribute> instead.")] in a future v1.x minor release.
3. We will duplicate the attributes into the new EricksonLopez.DomainPrimitives.Integrations namespace.
4. The source generators will be updated to recognize attributes from both namespaces.

### v2.0
The obsolete attributes in the root namespace will be permanently removed.

## Alternatives Considered
- Moving them into their respective integration packages (e.g., [Dapper] into EricksonLopez.DomainPrimitives.Dapper). This was rejected because it would require the consumer to add a package reference to Dapper in their Domain/Core project, which violates Clean Architecture principles (the Domain should not depend on Data Access technologies). Keeping the marker attributes in the core abstractions assembly allows the Domain to remain clean while signaling intent to the outer layers.

## Decision Outcome
Approved for implementation in v1.2 (additive) and enforcement in v2.0 (removal of old namespace).