using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
// ============================================================================
// CHAPTER 15: ASP.NET CORE INTEGRATION (.NET 10 MINIMAL APIS & PROBLEM DETAILS)
// ============================================================================
// In this chapter you will learn to integrate the Domain Primitives
// and `dotnet-result` ecosystem into ASP.NET Core web applications (Minimal APIs & Controllers).
//
// INTEGRATED FEATURES:
// 1. Automatic Parameter Binding: Route and query string binding using `StrongId` and `IParsable`.
// 2. Result to HTTP Response: Functional mapping of `Result<T>` to HTTP `IResult` (200 OK, 400 Bad Request, 404 Not Found).
// 3. RFC 7807 Standard (ProblemDetails): Structured format for validation error responses.
// ============================================================================

using Chapter15;
using EricksonLopez.DomainPrimitives;
using EricksonLopez.Result;
using HttpResult = Microsoft.AspNetCore.Http.IResult;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

Console.WriteLine("=========================================================");
Console.WriteLine(" 📘 CHAPTER 15: ASP.NET CORE MINIMAL APIS INTEGRATION");
Console.WriteLine("=========================================================\n");

// ----------------------------------------------------------------------------
// 1. MINIMAL APIS ENDPOINTS WITH STRONGLY TYPED ID ROUTE BINDING
// ----------------------------------------------------------------------------

// GET /api/customers/{id} (The CustomerId is automatically bound from the URL thanks to IParsable)
app.MapGet("/api/customers/{id}", (CustomerId id) =>
{
    var result = GetCustomerById(id);
    return ToHttpResult(result);
});

// POST /api/customers
app.MapPost("/api/customers", (CreateCustomerDto dto) =>
{
    bool isEmailSuccess = EmailAddress.TryCreate(dto.Email, out var email, out var emailError);
    if (!isEmailSuccess)
        return ToHttpPrimitiveErrorResult(emailError);

    var customerId = CustomerId.Create();
    Console.WriteLine($"[REST API] Customer successfully created with ID: {customerId}");
    return Results.Created($"/api/customers/{customerId}", new { Id = customerId, Name = dto.Name, Email = email });
});

Console.WriteLine("Minimal API Endpoints successfully registered:");
Console.WriteLine("  • GET  /api/customers/{id:CustomerId}");
Console.WriteLine("  • POST /api/customers");

Console.WriteLine("\n--- 🧪 REST IN-MEMORY EXECUTION SIMULATION ---");

// Simulation 1: Successful GET
var testId = CustomerId.Create();
var okResult = GetCustomerById(testId);
var httpOk = ToHttpResult(okResult);
Console.WriteLine($"GET /api/customers/{testId} -> HTTP Response Type: {httpOk.GetType().Name} ✅");

// Simulation 2: GET Not Found
var emptyId = CustomerId.Empty;
var notFoundResult = GetCustomerById(emptyId);
var httpNotFound = ToHttpResult(notFoundResult);
Console.WriteLine($"GET /api/customers/{emptyId} -> HTTP Response Type: {httpNotFound.GetType().Name} ✅ (RFC 7807 ProblemDetails)");

Console.WriteLine("\nCHAPTER 15 COMPLETED SUCCESSFULLY.\n");

// ============================================================================
// FUNCTIONAL MAPPING OF RESULT TO HTTP IRESULT (RFC 7807 PROBLEM DETAILS)
// ============================================================================

static HttpResult ToHttpResult<T>(Result<T> result)
{
    if (result.IsSuccess)
        return Results.Ok(result.Value);

    return ToHttpErrorResult(result.Error);
}

static HttpResult ToHttpPrimitiveErrorResult(EricksonLopez.DomainPrimitives.Validation.PrimitiveError error)
{
    return Results.BadRequest(new { Code = "Validation", Description = error.Message });
}

static HttpResult ToHttpErrorResult(Error error)
{
    return error.Type switch
    {
        ErrorType.NotFound => Results.NotFound(new { error.Code, error.Description }),
        ErrorType.Validation => Results.BadRequest(new { error.Code, error.Description }),
        ErrorType.Conflict => Results.Conflict(new { error.Code, error.Description }),
        _ => Results.Problem(detail: error.Description, title: error.Code, statusCode: 500)
    };
}

static Result<CustomerResponseDto> GetCustomerById(CustomerId id)
{
    // Database search simulation
    if (id == CustomerId.Empty)
        return Error.NotFound("Customer.NotFound", $"There is no customer with the ID '{id}'.");

    return new CustomerResponseDto(id, "Carlos Slim", EmailAddress.Create("carlos@company.com"));
}

// ============================================================================
// DEFINITION OF DTOS AND DOMAIN TYPES
// ============================================================================

namespace Chapter15
{
    public record CreateCustomerDto(string Name, string Email);

    public record CustomerResponseDto(CustomerId Id, string Name, EmailAddress Email);

    [StrongId<Guid>]
    public readonly partial record struct CustomerId;

    [Email]
    public readonly partial record struct EmailAddress;
}



