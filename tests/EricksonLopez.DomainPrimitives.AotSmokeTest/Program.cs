// Copyright © Erickson Lopez. MIT License.
// TD-001: NativeAOT trimming probe for EricksonLopez.DomainPrimitives.Abstractions
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.DomainPrimitives;
using EricksonLopez.DomainPrimitives.AotSmokeTest;
using EricksonLopez.DomainPrimitives.Validation;

Console.WriteLine("NativeAOT probe started.");

var error = new PrimitiveError("TEST", "Test message");
Console.WriteLine($"PrimitiveError: {error.Code} / IsError: {error.IsError}");

var noError = PrimitiveError.None;
Console.WriteLine($"None.IsError: {noError.IsError}");

var stringAttr = typeof(StringPrimitiveAttribute);
var strongIdAttr = typeof(StrongIdAttribute<Guid>);
Console.WriteLine($"Attributes: {stringAttr.Name}, {strongIdAttr.Name}");

// 1. StrongId
var sampleId = AotOrderId.Create();
Console.WriteLine($"AotOrderId generated: {sampleId} (IsDefault: {sampleId.IsDefault})");

// 2. StringPrimitive
if (AotCustomerName.TryCreate("Alice Cooper", out var customer, out var createErr))
{
    Console.WriteLine($"AotCustomerName created: {customer.Value} (Length: {customer.Value.Length})");
}
else
{
    Console.WriteLine($"Failed to create customer: {createErr.Message}");
}

// Span parsing probe
if (AotCustomerName.TryParse("Bob Dylan".AsSpan(), null, out var parsedCustomer))
{
    Console.WriteLine($"AotCustomerName span parsed: {parsedCustomer.Value}");
}

// 3. NumericPrimitive
if (AotPrice.TryCreate(99.99m, out var price, out var priceErr))
{
    Console.WriteLine($"AotPrice created: {price.Value} (ToString: {price})");
}
else
{
    Console.WriteLine($"Failed to create price: {priceErr.Message}");
}

// 4. DatePrimitive
var today = DateOnly.FromDateTime(DateTime.UtcNow);
if (AotOrderDate.TryCreate(today, out var orderDate, out var dateErr))
{
    Console.WriteLine($"AotOrderDate created: {orderDate.Value} (ToString: {orderDate})");
}
else
{
    Console.WriteLine($"Failed to create order date: {dateErr.Message}");
}

// 5. SmartEnum
var priority = AotPriority.High;
Console.WriteLine($"AotPriority: {priority.Name} = {priority.Value}");
if (AotPriority.TryFromName("Medium", out var parsedPriority))
{
    Console.WriteLine($"AotPriority parsed: {parsedPriority.Name}");
}

// 6. ValueObject
var address = AotAddress.Create("123 Main St", "Seattle");
Console.WriteLine($"AotAddress created: {address.Street}, {address.City}");

Console.WriteLine("NativeAOT probe completed successfully.");



