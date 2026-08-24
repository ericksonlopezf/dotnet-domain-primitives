// Copyright © Erickson Lopez. MIT License.
// ============================================================================
// CHAPTER 17: MEDIATR & CQRS INTEGRATION WITH RESULT<T>
// ============================================================================
// In this chapter you will learn to use the CQRS pattern (Command Query Responsibility Segregation)
// combining handlers with `Result<T>` and Pipeline Behaviors for a clean architecture.
//
// ADVANTAGES OF RESULT<T> IN CQRS:
// 1. Explicit Handlers: They return `Result<TResponse>` or `Result` indicating success or failure.
// 2. Elimination of try-catch around the MediatR invocation.
// 3. Pipeline Behaviors: Intercept commands for pre-validation without interrupting with exceptions.
// ============================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chapter17;
using EricksonLopez.DomainPrimitives;
using EricksonLopez.Result;

Console.WriteLine("=========================================================");
Console.WriteLine(" 📘 CHAPTER 17: CQRS & MEDIATR PIPELINE INTEGRATION");
Console.WriteLine("=========================================================\n");

// ----------------------------------------------------------------------------
// 1. CQRS COMMAND INVOCATION
// ----------------------------------------------------------------------------
Console.WriteLine("--- ✉️ COMMAND EXECUTION: CreateCustomerCommand ---");

var validCommand = new CreateCustomerCommand("Guillermo del Toro", "guillermo@cine.com");
var handler = new CreateCustomerCommandHandler();

// Execution with Validation Pipeline Behavior
var pipeline = new ValidationPipelineBehavior<CreateCustomerCommand, CustomerId>();
var okResult = await pipeline.Handle(validCommand, () => handler.Handle(validCommand));

if (okResult.IsSuccess)
{
    Console.WriteLine($"[Successful Command] Customer registered with ID: {okResult.Value} ✅");
}

Console.WriteLine("\n--- 🛑 AUTOMATIC VALIDATION IN PIPELINE BEHAVIOR ---");

var invalidCommand = new CreateCustomerCommand("Ana", "invalid-email");
var errorResult = await pipeline.Handle(invalidCommand, () => handler.Handle(invalidCommand));

if (errorResult.IsFailure)
{
    Console.WriteLine($"[Pipeline Caught Error]");
    Console.WriteLine($"  Code:        {errorResult.Error.Code}");
    Console.WriteLine($"  Description: {errorResult.Error.Description} ❌");
}

Console.WriteLine("\nCHAPTER 17 COMPLETED SUCCESSFULLY.\n");


// ============================================================================
// DEFINITIONS OF CQRS, HANDLERS AND PIPELINE BEHAVIORS
// ============================================================================

namespace Chapter17
{
    [StrongId<Guid>]
    public readonly partial record struct CustomerId;

    [Email]
    public readonly partial record struct EmailAddress;

    // CQRS Command that returns Result<CustomerId>
    public record CreateCustomerCommand(string Name, string Email);

    // Pipeline Behavior to intercept and validate the request before executing the handler
    public class ValidationPipelineBehavior<TRequest, TResponse>
        where TRequest : CreateCustomerCommand
    {
        public async Task<Result<TResponse>> Handle(TRequest request, Func<Task<Result<TResponse>>> next)
        {
            // Validate command parameters before processing
            bool isEmailSuccess = EmailAddress.TryCreate(request.Email, out var email, out var emailError);
            if (!isEmailSuccess)
            {
                return Error.Validation("CreateCustomer.InvalidEmail", emailError.Message);
            }

            return await next();
        }
    }

    // Business Handler
    public class CreateCustomerCommandHandler
    {
        public Task<Result<CustomerId>> Handle(CreateCustomerCommand command)
        {
            var newId = CustomerId.Create();
            // Simulate save in database
            return Task.FromResult(Result<CustomerId>.Success(newId));
        }
    }
}





