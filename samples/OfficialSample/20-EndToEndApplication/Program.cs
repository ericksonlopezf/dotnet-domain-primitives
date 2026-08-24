// Copyright © Erickson Lopez. MIT License.
// ============================================================================
// CHAPTER 20: UNIFIED END-TO-END APPLICATION (E-COMMERCE ORDER PROCESSING)
// ============================================================================
// This is the final chapter and the masterpiece of the examples suite.
// It unifies absolutely ALL the capabilities of the ecosystem in a real
// e-commerce order processing application.
//
// INTEGRATED AND DEMONSTRATED FEATURES:
// 1. Domain Primitives: `OrderId`, `CustomerId`, `EmailAddress`, `Money`, `PaymentStatus`, `Address`.
// 2. Aggregate Roots & Entities: `OrderAggregate`, `OrderLine`.
// 3. Railway-Oriented Programming (ROP): `Result<T>` and `Error` catalogs.
// 4. Domain Events: `OrderPlacedEvent` and in-memory dispatch.
// 5. EF Core Persistence: Save & Query with ValueConverters.
// 6. Specification Pattern: High value orders filter (`HighValueOrderSpecification`).
// 7. System.Text.Json Serialization: Unwrapped JSON API response.
// ============================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading;
using Chapter20;
using EricksonLopez.DomainPrimitives;
using EricksonLopez.Result;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

Console.WriteLine("============================================================================");
Console.WriteLine(" 🏆 CHAPTER 20: END-TO-END APPLICATION (COMPLETE E-COMMERCE SYSTEM)");
Console.WriteLine("============================================================================\n");

// ----------------------------------------------------------------------------
// 1. INFRASTRUCTURE INITIALIZATION (EF CORE IN-MEMORY)
// ----------------------------------------------------------------------------
var options = new DbContextOptionsBuilder<ECommerceDbContext>()
    .UseInMemoryDatabase(databaseName: "ECommerceEndToEndDb")
    .Options;

using var dbContext = new ECommerceDbContext(options);

// ----------------------------------------------------------------------------
// 2. CREATION AND VALIDATION OF PRIMITIVES AND VALUE OBJECTS
// ----------------------------------------------------------------------------
Console.WriteLine("--- 1️⃣ CREATION OF CUSTOMER AND ADDRESS WITH DOMAIN PRIMITIVES ---");

var customerId = CustomerId.Create();
bool isEmailSuccess = EmailAddress.TryCreate("vip.customer@company.com", out var email, out var emailError);
bool isAddressSuccess = Address.TryCreate("Av. Insurgentes Sur 1500", "CDMX", "03920", out var address, out var addressError);

if (!isEmailSuccess || !isAddressSuccess)
{
    Console.WriteLine($"❌ Primitive creation error.");
    return;
}

Console.WriteLine($"Customer: {customerId}");
Console.WriteLine($"Email:    {email}");
Console.WriteLine($"Address:  {address}\n");

// ----------------------------------------------------------------------------
// 3. CREATION AND OPERATIONS ON THE AGGREGATE ROOT (ORDER)
// ----------------------------------------------------------------------------
Console.WriteLine("--- 2️⃣ CREATION AND COMPOSITION OF THE AGGREGATE ROOT (ORDER) ---");

var orderId = OrderId.Create();
var orderResult = OrderAggregate.Create(orderId, customerId, address);

if (orderResult.IsSuccess)
{
    var order = orderResult.Value;

    // Add items to the order
    _ = order.AddLine(ProductId.Create(501), Money.Create(1200.00m), 1);
    _ = order.AddLine(ProductId.Create(502), Money.Create(350.50m), 2);

    Console.WriteLine($"[Aggregate Created] Order ID: {order.Id}");
    Console.WriteLine($"  Total Products: {order.Lines.Count}");
    Console.WriteLine($"  Total Amount:   {order.Total:C}");
    Console.WriteLine($"  Payment Status: {order.PaymentStatus.Name}");

    // ----------------------------------------------------------------------------
    // 4. DATABASE PERSISTENCE (EF CORE)
    // ----------------------------------------------------------------------------
    Console.WriteLine("\n--- 3️⃣ DATABASE PERSISTENCE VIA EF CORE ---");

    var entity = new OrderEntity
    {
        Id = order.Id,
        CustomerId = order.CustomerId,
        TotalAmount = order.Total,
        Status = order.PaymentStatus.Name
    };

    dbContext.Orders.Add(entity);
    await dbContext.SaveChangesAsync();
    Console.WriteLine($"✅ Order successfully persisted in EF Core InMemory DB.");

    // ----------------------------------------------------------------------------
    // 5. QUERY AND FILTERING WITH SPECIFICATION PATTERN
    // ----------------------------------------------------------------------------
    Console.WriteLine("\n--- 4️⃣ LINQ QUERY WITH HIGH VALUE ORDERS SPECIFICATION ---");

    var highValueSpec = new HighValueOrderSpecification(Money.Create(1000.00m));
    var highValueOrders = dbContext.Orders.AsQueryable().Where(highValueSpec.ToExpression()).ToList();

    Console.WriteLine($"Orders found with amount > $1,000.00: {highValueOrders.Count}");

    // ----------------------------------------------------------------------------
    // 6. JSON SERIALIZATION AND EVENT DISPATCH
    // ----------------------------------------------------------------------------
    Console.WriteLine("\n--- 5️⃣ UNWRAPPED JSON RESPONSE SERIALIZATION & DOMAIN EVENTS ---");

    var jsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true
    };

    var dtoResponse = new OrderResponseDto(order.Id, order.CustomerId, Money.Create(order.Total), order.PaymentStatus.Name);
    string jsonOutput = JsonSerializer.Serialize(dtoResponse, jsonOptions);

    Console.WriteLine("JSON response generated for the API:");
    Console.WriteLine(jsonOutput);

    Console.WriteLine("\nDispatching domain events emitted by the Aggregate:");
    foreach (var @event in order.DomainEvents)
    {
        if (@event is OrderPlacedEvent e)
        {
            Console.WriteLine($" 📢 [DOMAIN EVENT] OrderPlaced -> OrderId: {e.OrderId}, CustomerId: {e.CustomerId}, Total: {e.Total:C}");
        }
    }

    order.ClearDomainEvents();
}

Console.WriteLine("\n============================================================================");
Console.WriteLine(" 🏆 CHAPTER 20 AND COMPLETE OFFICIAL SUITE FINISHED WITH ABSOLUTE SUCCESS!");
Console.WriteLine("============================================================================\n");


// ============================================================================
// DEFINITION OF ENTITIES, SPECIFICATIONS AND DBCONTEXT
// ============================================================================

namespace Chapter20
{
    [StrongId<Guid>]
    public readonly partial record struct OrderId;

    [StrongId<Guid>]
    public readonly partial record struct CustomerId;

    [StrongId<int>]
    public readonly partial record struct ProductId;

    [Email]
    public readonly partial record struct EmailAddress;

    [Money(Min = 0)]
    public readonly partial record struct Money;

    [ValueObject]
    public readonly partial record struct Address
    {
        public string Street { get; init; }
        public string City { get; init; }
        public string ZipCode { get; init; }
    }

    [SmartEnum<int>]
    public readonly partial record struct PaymentStatus
    {
        public static readonly PaymentStatus Pending = new(1, "Pending");
        public static readonly PaymentStatus Paid = new(2, "Paid");
        public static readonly PaymentStatus Failed = new(3, "Failed");
    }

    public record OrderPlacedEvent(OrderId OrderId, CustomerId CustomerId, decimal Total) : IDomainEvent;

    public record OrderLine(ProductId ProductId, Money UnitPrice, int Quantity);

    public record OrderResponseDto(OrderId Id, CustomerId CustomerId, Money Total, string Status);

    public class OrderAggregate : AggregateRoot<OrderId>
    {
        private readonly List<OrderLine> _lines = [];

        public CustomerId CustomerId { get; private set; }
        public Address ShippingAddress { get; private set; }
        public PaymentStatus PaymentStatus { get; private set; } = PaymentStatus.Pending;

        public IReadOnlyCollection<OrderLine> Lines => _lines.AsReadOnly();
        public decimal Total => _lines.Sum(l => l.UnitPrice.Value * l.Quantity);

        private OrderAggregate(OrderId id, CustomerId customerId, Address shippingAddress)
        {
            Id = id;
            CustomerId = customerId;
            ShippingAddress = shippingAddress;
        }

        public static Result<OrderAggregate> Create(OrderId id, CustomerId customerId, Address shippingAddress)
        {
            var p = new OrderAggregate(id, customerId, shippingAddress);
            p.AddDomainEvent(new OrderPlacedEvent(id, customerId, 0));
            return p;
        }

        public Result AddLine(ProductId productId, Money unitPrice, int quantity)
        {
            _lines.Add(new OrderLine(productId, unitPrice, quantity));
            return Result.Success();
        }
    }

    public class OrderEntity
    {
        public OrderId Id { get; set; }
        public CustomerId CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class ECommerceDbContext : DbContext
    {
        public DbSet<OrderEntity> Orders => Set<OrderEntity>();

        public ECommerceDbContext(DbContextOptions<ECommerceDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OrderEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasConversion(id => id.Value, g => OrderId.Create(g));
                entity.Property(e => e.CustomerId).HasConversion(id => id.Value, g => CustomerId.Create(g));
            });
        }
    }

    public class HighValueOrderSpecification
    {
        private readonly Money _minAmount;
        public HighValueOrderSpecification(Money minAmount) => _minAmount = minAmount;

        public Expression<Func<OrderEntity, bool>> ToExpression() => p => p.TotalAmount >= _minAmount.Value;
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


