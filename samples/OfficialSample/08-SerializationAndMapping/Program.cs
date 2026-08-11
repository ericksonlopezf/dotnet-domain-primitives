using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
// ============================================================================
// CHAPTER 08: SERIALIZATION AND MAPPING (JSON & MAPSTER)
// ============================================================================
// In this chapter you will learn how to integrate with System.Text.Json and Mapster
// using `dotnet-primitive-json` and `dotnet-primitive-mapster`.
//
// TRADITIONAL SERIALIZATION PROBLEM:
// When serializing a `CustomerId` to JSON, a naive serializer produces:
// `{ "customerId": { "value": "a1b2c3d4-..." } }` ❌
//
// SOLUTION WITH DOMAIN PRIMITIVES JSON:
// Automatic converters serialize the primitive as a flat value (unwrap):
// `{ "customerId": "a1b2c3d4-..." }` ✅
// ============================================================================

using System.Text.Json;
using Chapter08;
using EricksonLopez.DomainPrimitives;

using EricksonLopez.DomainPrimitives.Mapster;
using EricksonLopez.Result;
using Mapster;

Console.WriteLine("=========================================================");
Console.WriteLine(" 📘 CHAPTER 08: SERIALIZATION AND MAPPING (JSON & MAPSTER)");
Console.WriteLine("=========================================================\n");

// ----------------------------------------------------------------------------
// 1. JSON SERIALIZATION (SYSTEM.TEXT.JSON)
// ----------------------------------------------------------------------------
Console.WriteLine("--- 📄 JSON SERIALIZATION (TRANSPARENT / UNWRAPPED) ---");

var customerDto = new CustomerResponseDto
{
    Id = CustomerId.Create(),
    Email = EmailAddress.Create("customer.example@company.com"),
    CreditAmount = Money.Create(1500.75m)
};

// Configure JsonSerializerOptions
var jsonOptions = new JsonSerializerOptions
{
    WriteIndented = true
};

string jsonResult = JsonSerializer.Serialize(customerDto, jsonOptions);
Console.WriteLine("Generated JSON (Flat Values):");
Console.WriteLine(jsonResult);

// Automatic deserialization from flat JSON
var deserializedDto = JsonSerializer.Deserialize<CustomerResponseDto>(jsonResult, jsonOptions);
Console.WriteLine($"Successfully deserialized: {deserializedDto?.Email} (Amount: {deserializedDto?.CreditAmount})");

Console.WriteLine();

// ----------------------------------------------------------------------------
// 2. MAPPING DTOS TO DOMAIN MODELS WITH MAPSTER
// ----------------------------------------------------------------------------
Console.WriteLine("--- 🔄 MAPPING WITH MAPSTER (CONVERTING PRIMITIVES TO DTOS) ---");

// Register Mapster configurations
TypeAdapterConfig.GlobalSettings
    .AddDomainPrimitiveMapping<CustomerId, Guid>(id => CustomerId.Create(id), customerId => customerId.Value)
    .AddDomainPrimitiveMapping<EmailAddress, string>(s => EmailAddress.Create(s), email => email.Value)
    .AddDomainPrimitiveMapping<Money, decimal>(m => Money.Create(m), money => money.Value);

var requestInput = new CreateCustomerRequest("Maria Lopez", "maria.lopez@company.com", 2500.00m);

// Direct mapping to strongly typed types
var generatedCustomerId = CustomerId.Create();
var emailAddress = EmailAddress.Create(requestInput.Email);
var credit = Money.Create(requestInput.InitialCredit);

var domainModel = new CustomerModel(generatedCustomerId, requestInput.Name, emailAddress, credit);

// Convert Domain Model to flat DTO using Mapster
var mappedDto = domainModel.Adapt<CustomerResponseDto>();

Console.WriteLine($"DTO Mapped by Mapster:");
Console.WriteLine($"  ID:     {mappedDto.Id}");
Console.WriteLine($"  Email:  {mappedDto.Email}");
Console.WriteLine($"  Amount: {mappedDto.CreditAmount}");

Console.WriteLine("\nCHAPTER 08 COMPLETED SUCCESSFULLY.\n");


// ============================================================================
// DEFINITION OF DTOS AND DOMAIN TYPES
// ============================================================================

namespace Chapter08
{
    public record CreateCustomerRequest(string Name, string Email, decimal InitialCredit);

    public class CustomerResponseDto
    {
        public CustomerId Id { get; set; }
        public EmailAddress Email { get; set; }
        public Money CreditAmount { get; set; }
    }

    public class CustomerModel
    {
        public CustomerId Id { get; }
        public string Name { get; }
        public EmailAddress Email { get; }
        public Money Credit { get; }

        public CustomerModel(CustomerId id, string name, EmailAddress email, Money credit)
        {
            Id = id;
            Name = name;
            Email = email;
            Credit = credit;
        }
    }

    [StrongId<Guid>]
    public readonly partial record struct CustomerId;

    [Email]
    public readonly partial record struct EmailAddress;

    [Money(Min = 0)]
    public readonly partial record struct Money;
}


