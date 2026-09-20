# Level 3 — Real Use Cases

This level demonstrates real-world domain modeling scenarios derived strictly from the official public API inventory.

---

## 1. Specialized Semantic Shortcuts

DomainPrimitives provides over 38 semantic shortcuts that enforce strict normalization and validation without repetitive boilerplate:

```csharp
using EricksonLopez.DomainPrimitives;

// ─── Identity & Networking ───────────────────────────────────────────
[Email]
public readonly partial record struct ContactEmail;

[Phone]
public readonly partial record struct MobilePhone;

[Url]
public readonly partial record struct WebhookEndpoint;

[IPAddress]
public readonly partial record struct ClientIp;

// ─── Commerce & Finance ──────────────────────────────────────────────
[Money]
public readonly partial record struct WalletBalance;

[Price]
public readonly partial record struct CatalogPrice;

[TaxRate]
public readonly partial record struct VatRate;

[Discount]
public readonly partial record struct PromotionalDiscount;

[CurrencyCode]
public readonly partial record struct TransactionCurrency;

[IBAN]
public readonly partial record struct BankAccountNumber;

// ─── Geography & Physical Metrics ────────────────────────────────────
[Latitude]
public readonly partial record struct WarehouseLatitude;

[Longitude]
public readonly partial record struct WarehouseLongitude;

[Weight]
public readonly partial record struct PackageWeight;

[Distance]
public readonly partial record struct DeliveryDistance;
```

---

## 2. Strongly-Typed Identifiers (`[StrongId<T>]`)

Strongly-typed IDs eliminate accidental transposition of foreign and primary keys:

```csharp
[StrongId<Guid>]
public readonly partial record struct AccountId;

[StrongId<long>]
public readonly partial record struct InvoiceNumber;

[StrongId<string>]
public readonly partial record struct TenantCode;
```

---

## 3. Composite Value Objects (`[ValueObject]`)

For domain concepts that bundle multiple primitives while maintaining structural value equality:

```csharp
[ValueObject]
public readonly partial record struct Address(
    string Street,
    string City,
    CountryCode Country,
    string PostalCode
);
```

---

## 4. Bulk Operations on Collections (`PrimitiveCollectionExtensions`)

Safely and efficiently convert raw scalar sequences into validated domain collections:

```csharp
using EricksonLopez.DomainPrimitives;

var rawEmails = new List<string> { "a@test.com", "b@test.com" };

// Fluent conversion to immutable domain lists
IReadOnlyList<ContactEmail> validEmails = rawEmails.ToDomainPrimitiveList<ContactEmail, string>();

// Optimized operations over ReadOnlySpan<T>
ReadOnlySpan<string> span = rawEmails.ToArray();
ContactEmail[] emailsArray = span.ToDomainPrimitiveArray<ContactEmail, string>();
```

---

## Showcase Reference
- Executable projects:
  - [`04-ValueObjects`](file:///d:/DevData/ericksonlopez.dev/dotnet-domain-primitives/samples/OfficialSample/04-ValueObjects/Program.cs)
  - [`05-StronglyTypedIds`](file:///d:/DevData/ericksonlopez.dev/dotnet-domain-primitives/samples/OfficialSample/05-StronglyTypedIds/Program.cs)
  - [`13-DomainCollections`](file:///d:/DevData/ericksonlopez.dev/dotnet-domain-primitives/samples/OfficialSample/13-DomainCollections/Program.cs)
