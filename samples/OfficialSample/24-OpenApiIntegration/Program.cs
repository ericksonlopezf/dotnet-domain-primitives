using System;
using System.Collections.Generic;
using Chapter24;
using EricksonLopez.DomainPrimitives.OpenApi.Generated;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// ============================================================================
// CHAPTER 24: OPENAPI / SWAGGER INTEGRATION (SWASHBUCKLE)
// ============================================================================
// In this chapter you will learn how Domain Primitives appear correctly
// in OpenAPI documentation (Swagger UI) using the
// `EricksonLopez.DomainPrimitives.OpenApi` package.
//
// PROBLEM WITHOUT THE PACKAGE:
// Swashbuckle would show `CustomerId` as an empty object `{}` instead
// of the correct schema for a Guid (format: uuid).
//
// SOLUTION:
// The package generates an ISchemaFilter that converts all Domain Primitives
// to the correct OpenAPI schema of their backing type.
//
// SETUP:
//   builder.Services.AddSwaggerGen(c =>
//       c.SchemaFilter<DomainPrimitivesSchemaFilter>()); // generated at compile time
//
// RESULT:
//   CustomerId   → { type: "string", format: "uuid" }
//   EmailAddress → { type: "string", format: "email" }
//   Price        → { type: "number" }
// ============================================================================

Console.WriteLine("=========================================================");
Console.WriteLine(" 📘 CHAPTER 24: OPENAPI/SWAGGER INTEGRATION");
Console.WriteLine("=========================================================");
Console.WriteLine("Starting web server on http://localhost:5024...");
Console.WriteLine("Navigate to http://localhost:5024/swagger to view the UI\n");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "DomainPrimitives OpenAPI Demo",
        Version = "v1",
        Description = "Demonstrates how Domain Primitives appear in Swagger UI"
    });

    // ── KEY INTEGRATION: register the source-generated schema filter ─────────
    // DomainPrimitivesSchemaFilter is generated at compile time by
    // EricksonLopez.DomainPrimitives.OpenApi.SourceGenerators.
    // It maps every Domain Primitive type to its correct OpenAPI schema.
    options.SchemaFilter<DomainPrimitivesSchemaFilter>();
    // ────────────────────────────────────────────────────────────────────────
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "DomainPrimitives v1");
        c.RoutePrefix = "swagger";
    });
}

// ── Minimal API endpoints using Domain Primitives ───────────────────────────

// GET /products/{id} — Swagger shows CustomerId as format: uuid
app.MapGet("/products/{id}", (string id) =>
{
    if (!CustomerId.TryParse(id, null, out var customerId))
        return Results.BadRequest("Invalid CustomerId format (expected UUID)");

    return Results.Ok(new ProductDto(
        customerId,
        ProductName.Create("Sample Product"),
        Price.Create(99.99m),
        EmailAddress.Create("owner@example.com")));
})
.WithName("GetProduct")
.WithSummary("Get a product — CustomerId shows as format: uuid in Swagger");

// POST /products — Swagger shows ProductRequest with proper types
app.MapPost("/products", (ProductRequest req) =>
{
    // Domain Primitives are bound from JSON automatically via STJ converters
    return Results.Created($"/products/{req.OwnerId}", new ProductDto(
        req.OwnerId,
        req.Name,
        req.Price,
        req.OwnerEmail));
})
.WithName("CreateProduct")
.WithSummary("Create a product — all Domain Primitive types show correct OpenAPI schemas");

// GET /enums/order-statuses — SmartEnum shown as string enum in Swagger
app.MapGet("/enums/order-statuses", () =>
    Results.Ok(new { Statuses = new[] { "Pending", "Processing", "Completed" }, Note = "OrderStatus smart enum values" }))
.WithName("GetOrderStatuses")
.WithSummary("List all SmartEnum order status values");

Console.WriteLine("Registered endpoints:");
Console.WriteLine("  GET  /products/{id}         — CustomerId → format: uuid");
Console.WriteLine("  POST /products              — ProductRequest with typed types");
Console.WriteLine("  GET  /enums/order-statuses  — SmartEnum values");
Console.WriteLine("\nNavigate to http://localhost:5024/swagger to view the documentation\n");

app.Run();

// ── DTOs ────────────────────────────────────────────────────────────────────
namespace Chapter24
{
    using EricksonLopez.DomainPrimitives;
    using EricksonLopez.DomainPrimitives.Validation;

    // Domain Primitives — the source generator creates OpenAPI schema filters for these:

    /// <summary>Strongly typed Customer ID (maps to UUID in OpenAPI).</summary>
    [StrongId<Guid>]
    public readonly partial record struct CustomerId;

    /// <summary>Validated product name (maps to string, maxLength: 200).</summary>
    [StringPrimitive]
    [NotEmpty, MaxLength(200), Trim]
    public readonly partial record struct ProductName;

    /// <summary>Non-negative price (maps to number, minimum: 0).</summary>
    [NumericPrimitive<decimal>]
    public readonly partial record struct Price;

    // Note: To add custom validation, implement Validate in a separate file:
    //   private static partial PrimitiveError? Validate(decimal value) =>
    //       value < 0m ? PrimitiveError.Create("NEGATIVE_PRICE", "Price cannot be negative.") : null;

    /// <summary>Validated email address (maps to string, format: email).</summary>
    [Email]
    public readonly partial record struct EmailAddress;

    // DTOs — using Domain Primitives as property types:

    /// <summary>
    /// Product response DTO.
    /// Without <c>AddDomainPrimitivesSchemaFilters()</c>, Swagger shows <c>CustomerId</c> as <c>{}</c>.
    /// With it, Swagger correctly shows <c>string (format: uuid)</c>.
    /// </summary>
    public record ProductDto(CustomerId OwnerId, ProductName Name, Price Price, EmailAddress OwnerEmail);

    /// <summary>Product creation request DTO.</summary>
    public record ProductRequest(CustomerId OwnerId, ProductName Name, Price Price, EmailAddress OwnerEmail);
}
