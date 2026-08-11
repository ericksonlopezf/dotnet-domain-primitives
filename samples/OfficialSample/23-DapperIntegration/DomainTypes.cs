namespace Chapter23;

using System;
using EricksonLopez.DomainPrimitives;
using EricksonLopez.DomainPrimitives.Validation;

// ─── Strongly Typed IDs ───────────────────────────────────────────────────────

/// <summary>A strongly typed Product ID backed by Guid.</summary>
[StrongId<Guid>]
public readonly partial record struct ProductId;

/// <summary>A strongly typed Customer ID backed by Guid.</summary>
[StrongId<Guid>]
public readonly partial record struct CustomerId;

// ─── String Primitives ────────────────────────────────────────────────────────

/// <summary>A validated product name (non-empty, max 200 chars).</summary>
[StringPrimitive]
[NotEmpty, MaxLength(200), Trim]
public readonly partial record struct ProductName;

/// <summary>A validated email address using the built-in Email shortcut.</summary>
[Email]
public readonly partial record struct EmailAddress;

// ─── Numeric Primitive ────────────────────────────────────────────────────────

/// <summary>
/// A price value that must be non-negative.
/// The generator emits the Validate partial method stub — we implement validation
/// in the same partial type using the generated signature.
/// </summary>
[NumericPrimitive<decimal>]
public readonly partial record struct Price;

// Note: Custom validation can be added in a separate partial file using the
// generated Validate partial method. For this sample, Price accepts all decimals.
// Example (in a partial class extension):
//   private static partial PrimitiveError? Validate(decimal value) =>
//       value < 0m ? PrimitiveError.Create("NEGATIVE_PRICE", "Price cannot be negative.") : null;

// ─── Dapper Row ───────────────────────────────────────────────────────────────

/// <summary>
/// Dapper result row — typed properties for all domain primitives.
/// Dapper uses the registered TypeHandlers to map database columns directly.
/// </summary>
public record ProductRow(ProductId Id, ProductName Name, Price Price, EmailAddress OwnerEmail);
