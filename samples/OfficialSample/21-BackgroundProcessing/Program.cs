// Copyright © Erickson Lopez. MIT License.
// ============================================================================
// CHAPTER 21: BACKGROUND PROCESSING WITH DOMAIN PRIMITIVES
// ============================================================================
// In this chapter you will learn to use domain primitives inside background
// processing pipelines: typed queues, hosted workers, and cancellation.
//
// COVERED PATTERNS:
// 1. Channel<T> typed queue with domain primitive messages.
// 2. Consumer loop: TryCreate() for safe reconstruction of domain values
//    from raw data arriving from queues/buses.
// 3. Cancellation: graceful shutdown respects CancellationToken.
// 4. Error isolation: invalid messages are rejected without stopping the worker.
// 5. Strongly-typed message envelopes using domain primitives.
// ============================================================================
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Chapter21;
using EricksonLopez.DomainPrimitives;

Console.WriteLine("=========================================================");
Console.WriteLine(" 📘 CHAPTER 21: BACKGROUND PROCESSING WITH DOMAIN PRIMITIVES");
Console.WriteLine("=========================================================\n");

// ─────────────────────────────────────────────────────────────────────────────
// SECTION 1: Typed Channel Queue with domain primitive messages
// ─────────────────────────────────────────────────────────────────────────────
Console.WriteLine("--- 📨 SECTION 1: TYPED CHANNEL WITH DOMAIN PRIMITIVE MESSAGES ---\n");

// A bounded channel acts as the message queue between producer and consumer.
// Messages are strongly typed: RawOrderMessage wraps unvalidated raw data.
var channel = Channel.CreateBounded<RawOrderMessage>(capacity: 10);

using var cts = new CancellationTokenSource();

// ─────────────────────────────────────────────────────────────────────────────
// SECTION 2: Producer — enqueues raw order messages
// ─────────────────────────────────────────────────────────────────────────────
var producerTask = Task.Run(async () =>
{
    Console.WriteLine("[Producer] Starting — enqueuing 5 raw messages...");

    var rawMessages = new[]
    {
        // Valid messages
        new RawOrderMessage(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 99.99m, "user@shop.com"),
        new RawOrderMessage(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 249.00m, "buyer@commerce.io"),
        // Message with invalid amount (negative — will fail Money validation)
        new RawOrderMessage(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), -5.00m, "bad@example.com"),
        // Message with invalid email (will fail Email validation)
        new RawOrderMessage(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 50.00m, "not-an-email"),
        // Another valid message
        new RawOrderMessage(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 15.00m, "customer@brand.net"),
    };

    foreach (var msg in rawMessages)
    {
        await channel.Writer.WriteAsync(msg, cts.Token);
        await Task.Delay(50, cts.Token); // Simulate interval between messages
    }

    channel.Writer.Complete(); // Signal that no more messages will be written
    Console.WriteLine("[Producer] All messages enqueued. Channel closed.\n");
});

// ─────────────────────────────────────────────────────────────────────────────
// SECTION 3: Consumer worker — reads messages and reconstructs domain types
// ─────────────────────────────────────────────────────────────────────────────
Console.WriteLine("--- ⚙️  SECTION 2: CONSUMER WORKER — SAFE DOMAIN RECONSTRUCTION ---\n");

int processed = 0;
int rejected = 0;

// The consumer reads from the channel until it is closed or cancellation is requested.
// Each message is reconstructed as strongly-typed domain primitives.
// Invalid messages are skipped (logged) without crashing the worker.
await foreach (var rawMsg in channel.Reader.ReadAllAsync(cts.Token))
{
    Console.WriteLine($"[Consumer] Received raw message...");

    // Safe reconstruction of OrderId from raw string
    if (!OrderId.TryCreate(Guid.Parse(rawMsg.OrderId), out var orderId, out var orderIdError))
    {
        Console.WriteLine($"  ❌ Invalid OrderId: [{orderIdError.Code}] {orderIdError.Message} — skipping.");
        rejected++;
        continue;
    }

    // Safe reconstruction of CustomerId from raw string
    if (!CustomerId.TryCreate(Guid.Parse(rawMsg.CustomerId), out var customerId, out var customerIdError))
    {
        Console.WriteLine($"  ❌ Invalid CustomerId: [{customerIdError.Code}] {customerIdError.Message} — skipping.");
        rejected++;
        continue;
    }

    // Safe reconstruction of OrderAmount (Money) from raw decimal
    if (!OrderAmount.TryCreate(rawMsg.Amount, out var amount, out var amountError))
    {
        Console.WriteLine($"  ❌ Invalid Amount ({rawMsg.Amount}): [{amountError.Code}] {amountError.Message} — skipping.");
        rejected++;
        continue;
    }

    // Safe reconstruction of CustomerEmail from raw string
    if (!CustomerEmail.TryCreate(rawMsg.Email, out var email, out var emailError))
    {
        Console.WriteLine($"  ❌ Invalid Email ('{rawMsg.Email}'): [{emailError.Code}] {emailError.Message} — skipping.");
        rejected++;
        continue;
    }

    // At this point, all domain invariants are satisfied.
    // Construct the strongly-typed domain message.
    var domainMsg = new DomainOrderMessage(orderId, customerId, amount, email);
    ProcessDomainOrder(domainMsg);
    processed++;
}

Console.WriteLine($"\n[Consumer] Processing complete.");
Console.WriteLine($"  ✅ Processed: {processed} messages");
Console.WriteLine($"  ❌ Rejected:  {rejected} messages (failed domain invariants)");

Console.WriteLine("\nCHAPTER 21 COMPLETED SUCCESSFULLY.\n");

// ─────────────────────────────────────────────────────────────────────────────
// Business processing of a validated domain message
// ─────────────────────────────────────────────────────────────────────────────
static void ProcessDomainOrder(DomainOrderMessage msg)
{
    Console.WriteLine($"  ✅ ORDER PROCESSED:");
    Console.WriteLine($"       Order ID:   {msg.OrderId}");
    Console.WriteLine($"       Customer:   {msg.CustomerId}");
    Console.WriteLine($"       Amount:     {msg.Amount.Value:C}");
    Console.WriteLine($"       Email:      {msg.Email.Value}");
}

// ============================================================================
// DOMAIN TYPE DEFINITIONS
// ============================================================================

namespace Chapter21
{
    // ─── Domain Primitives ──────────────────────────────────────────────────

    /// <summary>Strongly-typed order identifier.</summary>
    [StrongId<Guid>]
    public readonly partial record struct OrderId;

    /// <summary>Strongly-typed customer identifier.</summary>
    [StrongId<Guid>]
    public readonly partial record struct CustomerId;

    /// <summary>Order amount — must be > 0.</summary>
    [Money(Min = 0.01)]
    public readonly partial record struct OrderAmount;

    /// <summary>Customer email address.</summary>
    [Email]
    public readonly partial record struct CustomerEmail;


    // ─── Message Envelopes ──────────────────────────────────────────────────

    /// <summary>
    /// Raw unvalidated message arriving from a queue or bus.
    /// All fields are primitives — they must be validated before use.
    /// </summary>
    public record RawOrderMessage(
        string OrderId,
        string CustomerId,
        decimal Amount,
        string Email);

    /// <summary>
    /// Validated domain message — all fields are domain primitives.
    /// Only created after successful validation in the consumer loop.
    /// </summary>
    public record DomainOrderMessage(
        OrderId OrderId,
        CustomerId CustomerId,
        OrderAmount Amount,
        CustomerEmail Email);
}
