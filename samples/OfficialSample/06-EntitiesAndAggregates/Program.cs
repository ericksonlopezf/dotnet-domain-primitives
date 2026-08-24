// Copyright © Erickson Lopez. MIT License.
// ============================================================================
// CHAPTER 06: ENTITIES AND AGGREGATE ROOTS IN DDD
// ============================================================================
// In this chapter you will learn the difference between Value Objects, Entities and Aggregate Roots.
//
// 1. ENTITY (Entity<TId>): Object with unique and immutable identity (Id). Two entities
//    are equal IF AND ONLY IF they share the same Id and type, even if their attributes vary.
// 2. AGGREGATE ROOT (AggregateRoot<TId>): A main entity that serves as a boundary
//    of transactional consistency and gateway to the aggregate.
// 3. ENCAPSULATION: Aggregate Roots do not allow mutating their state directly.
//    They only expose methods with business intent (e.g. `AddItem()`, `Cancel()`).
// ============================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Chapter06;
using EricksonLopez.DomainPrimitives;
using EricksonLopez.Result;
using System.Threading.Tasks;

Console.WriteLine("=========================================================");
Console.WriteLine(" 📘 CHAPTER 06: ENTITIES AND AGGREGATE ROOTS");
Console.WriteLine("=========================================================\n");

// ----------------------------------------------------------------------------
// 1. DEMONSTRATION OF EQUALITY BY IDENTITY IN ENTITIES
// ----------------------------------------------------------------------------
Console.WriteLine("--- 🆔 ENTITIES: EQUALITY BY ID (NOT BY ATTRIBUTES) ---");

var customerId = CustomerId.Create();

// Create two instances of the same Customer entity with the same ID but different name
var originalCustomer = Customer.Create(customerId, "John Doe", "john@company.com");
var modifiedCustomer = Customer.Create(customerId, "John Doe Lopez", "john.doe@company.com");

Console.WriteLine($"originalCustomer == modifiedCustomer: {originalCustomer == modifiedCustomer} ✅ (Same ID)");
Console.WriteLine($"Previous structural equality: {originalCustomer.Name != modifiedCustomer.Name} (Attributes changed, but the entity is the same)");

Console.WriteLine();

// ----------------------------------------------------------------------------
// 2. AGGREGATE ROOT AND CONSISTENCY BOUNDARY
// ----------------------------------------------------------------------------
Console.WriteLine("--- 🛡️ AGGREGATE ROOT: ENCAPSULATION AND TRANSACTIONALITY ---");

var orderId = OrderId.Create();
var orderResult = Order.Create(orderId, customerId);

if (orderResult.IsSuccess)
{
    Order order = orderResult.Value;
    Console.WriteLine($"[Aggregate Created] Order ID: {order.Id} for Customer: {order.CustomerId}");
    
    // Add items through the Aggregate Root (respecting invariants)
    Result itemResult1 = order.AddLine(ProductId.Create(101), Money.Create(50.00m), 2);
    Result itemResult2 = order.AddLine(ProductId.Create(102), Money.Create(120.00m), 1);

    if (itemResult1.IsSuccess && itemResult2.IsSuccess)
    {
        Console.WriteLine($"Total calculated by the Aggregate: {order.Total:C}");
        Console.WriteLine($"Current Order state: {order.State}");
    }
}

Console.WriteLine("\nCHAPTER 06 COMPLETED SUCCESSFULLY.\n");


// ============================================================================
// DEFINITION OF DOMAIN ENTITIES AND AGGREGATES
// ============================================================================

namespace Chapter06
{
    [StrongId<Guid>]
    public readonly partial record struct CustomerId;

    [StrongId<Guid>]
    public readonly partial record struct OrderId;

    [StrongId<int>]
    public readonly partial record struct ProductId;

    [Money(Min = 0.01)]
    public readonly partial record struct Money;

    // Simple entity
    public class Customer : Entity<CustomerId>
    {
        public string Name { get; private set; }
        public string Email { get; private set; }

        private Customer(CustomerId id, string name, string email)
        {
            Id = id;
            Name = name;
            Email = email;
        }

        public static Customer Create(CustomerId id, string name, string email) => new(id, name, email);
    }

    // Aggregate Root
    public class Order : AggregateRoot<OrderId>
    {
        private readonly List<OrderLine> _lines = [];

        public CustomerId CustomerId { get; private set; }
        public string State { get; private set; } = "Draft";
        public IReadOnlyList<OrderLine> Lines => _lines.AsReadOnly();
        public decimal Total => _lines.Sum(l => l.Subtotal.Value);

        private Order(OrderId id, CustomerId customerId)
        {
            Id = id;
            CustomerId = customerId;
        }

        public static Result<Order> Create(OrderId id, CustomerId customerId)
        {
            if (customerId == CustomerId.Empty)
                return Error.Validation("Order.CustomerRequired", "The order must belong to a valid customer.");

            return new Order(id, customerId);
        }

        public Result AddLine(ProductId productId, Money unitPrice, int quantity)
        {
            if (State != "Draft")
                return Error.Validation("Order.NotEditable", "Cannot add products to a finalized order.");

            if (quantity <= 0)
                return Error.Validation("Order.InvalidQuantity", "Quantity must be greater than 0.");

            var subtotal = Money.Create(unitPrice.Value * quantity);
            _lines.Add(new OrderLine(productId, unitPrice, quantity, subtotal));
            return Result.Success();
        }
    }

    // Secondary entity inside the Aggregate
    public class OrderLine
    {
        public ProductId ProductId { get; }
        public Money UnitPrice { get; }
        public int Quantity { get; }
        public Money Subtotal { get; }

        public OrderLine(ProductId productId, Money unitPrice, int quantity, Money subtotal)
        {
            ProductId = productId;
            UnitPrice = unitPrice;
            Quantity = quantity;
            Subtotal = subtotal;
        }
    }
}



public interface IDomainEvent { }

public abstract class Entity<TId>
{
    public TId Id { get; protected set; }
    
    // Simplification for the example
    public override bool Equals(object? obj) => obj is Entity<TId> entity && EqualityComparer<TId>.Default.Equals(Id, entity.Id);
    public override int GetHashCode() => EqualityComparer<TId>.Default.GetHashCode(Id!);
}

public abstract class AggregateRoot<TId> : Entity<TId>
{
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();
}


