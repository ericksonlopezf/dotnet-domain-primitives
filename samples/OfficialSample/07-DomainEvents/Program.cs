using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
// ============================================================================
// CHAPTER 07: DOMAIN EVENTS AND DECOUPLING
// ============================================================================
// In this chapter you will learn to use `IDomainEvent` and domain events
// in Aggregate Roots to decouple side effects (e.g. notifications,
// integrations, auditing).
//
// KEY CONCEPTS:
// 1. IDomainEvent: Marker interface for events that occurred in the past (e.g. `OrderCompletedEvent`).
// 2. Transactionality: Events are registered in the Aggregate's memory (`RaiseDomainEvent`),
//    and are only dispatched WHEN the transaction or persistence has been successfully committed.
// 3. ClearDomainEvents(): Cleaning the event queue after processing by infrastructure.
// ============================================================================

using Chapter07;
using EricksonLopez.DomainPrimitives;
using EricksonLopez.Result;

Console.WriteLine("=========================================================");
Console.WriteLine(" 📘 CHAPTER 07: DOMAIN EVENTS (DECOUPLING)");
Console.WriteLine("=========================================================\n");

// ----------------------------------------------------------------------------
// 1. EXECUTION OF BUSINESS OPERATION THAT GENERATES EVENTS
// ----------------------------------------------------------------------------
Console.WriteLine("--- 📦 EVENT REGISTRATION INSIDE THE AGGREGATE ROOT ---");

var orderId = OrderId.Create();
var customerId = CustomerId.Create();
var total = Money.Create(299.99m);

// Creating the order automatically generates a "OrderCreatedEvent" domain event
var orderResult = OrderAggregate.Create(orderId, customerId, total);

if (orderResult.IsSuccess)
{
    OrderAggregate order = orderResult.Value;

    // Completing the order generates another event "OrderCompletedEvent"
    order.Complete();

    Console.WriteLine($"[Aggregate] Accumulated events pending dispatch: {order.DomainEvents.Count}");

    // ----------------------------------------------------------------------------
    // 2. SIMULATION OF DISPATCH INFRASTRUCTURE / BUS
    // ----------------------------------------------------------------------------
    Console.WriteLine("\n--- 🚀 DOMAIN EVENTS DISPATCH (BUS SIMULATOR) ---");

    foreach (var @event in order.DomainEvents)
    {
        DispatchEvent(@event);
    }

    // Post-dispatch cleanup
    order.ClearDomainEvents();
    Console.WriteLine($"[Post-Dispatch] Remaining events in the Aggregate: {order.DomainEvents.Count} ✅");
}

Console.WriteLine("\nCHAPTER 07 COMPLETED SUCCESSFULLY.\n");


// ============================================================================
// EVENT BUS SIMULATOR
// ============================================================================

static void DispatchEvent(IDomainEvent domainEvent)
{
    switch (domainEvent)
    {
        case OrderCreatedEvent e:
            Console.WriteLine($" 📢 [EVENT DISPATCHED] OrderCreated -> Order: {e.OrderId}, Customer: {e.CustomerId}, Total: {e.Amount}");
            break;

        case OrderCompletedEvent e:
            Console.WriteLine($" 📢 [EVENT DISPATCHED] OrderCompleted -> Order: {e.OrderId}, Date: {e.CompletionDate:g}");
            break;

        default:
            Console.WriteLine($" 📢 [EVENT DISPATCHED] Generic Event: {domainEvent.GetType().Name}");
            break;
    }
}

// ============================================================================
// EVENT DEFINITIONS AND AGGREGATE ROOT
// ============================================================================

namespace Chapter07
{
    [StrongId<Guid>]
    public readonly partial record struct OrderId;

    [StrongId<Guid>]
    public readonly partial record struct CustomerId;

    [Money(Min = 0.01)]
    public readonly partial record struct Money;

    // Immutable domain events (Records)
    public record OrderCreatedEvent(OrderId OrderId, CustomerId CustomerId, Money Amount, DateTime OccurredOn) : IDomainEvent;

    public record OrderCompletedEvent(OrderId OrderId, DateTime CompletionDate, DateTime OccurredOn) : IDomainEvent;

    // Aggregate Root that emits domain events
    public class OrderAggregate : AggregateRoot<OrderId>
    {
        public CustomerId CustomerId { get; private set; }
        public Money Total { get; private set; }
        public bool Completed { get; private set; }

        private OrderAggregate(OrderId id, CustomerId customerId, Money total)
        {
            Id = id;
            CustomerId = customerId;
            Total = total;
            Completed = false;
        }

        public static Result<OrderAggregate> Create(OrderId id, CustomerId customerId, Money total)
        {
            var order = new OrderAggregate(id, customerId, total);

            // Register domain event inside the aggregate
            order.AddDomainEvent(new OrderCreatedEvent(id, customerId, total, DateTime.UtcNow));

            return order;
        }

        public Result Complete()
        {
            if (Completed)
                return Error.Validation("Order.AlreadyCompleted", "The order was already previously completed.");

            Completed = true;

            // Register business event
            AddDomainEvent(new OrderCompletedEvent(Id, DateTime.UtcNow, DateTime.UtcNow));

            return Result.Success();
        }
    }
}



public interface IDomainEvent { }

public abstract class AggregateRoot<TId>
{
    public TId Id { get; protected set; }
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();
}
