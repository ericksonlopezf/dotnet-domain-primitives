using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
// ============================================================================
// CHAPTER 13: DOMAIN COLLECTIONS (LIST ENCAPSULATION)
// ============================================================================
// In this chapter you will learn to protect internal collections within
// Aggregate Roots and Domain Entities.
//
// COMMON ERRORS IN TRADITIONAL ARCHITECTURES:
// Exposing `public List<Item> Items { get; set; }` allows external layers to:
//  • Modify the list directly bypassing validations (`Items.Add(...)`).
//  • Empty the list (`Items.Clear()`).
//  • Leave the aggregate in an inconsistent state.
//
// SOLUTION WITH CLEAN ARCHITECTURE AND DDD:
// Expose only read-only views (`IReadOnlyCollection<T>`) and force all
// additions or removals to go through methods with business intent.
// ============================================================================

using Chapter13;
using EricksonLopez.DomainPrimitives;
using EricksonLopez.Result;

Console.WriteLine("=========================================================");
Console.WriteLine(" 📘 CHAPTER 13: DOMAIN COLLECTIONS AND ENCAPSULATION");
Console.WriteLine("=========================================================\n");

// ----------------------------------------------------------------------------
// 1. CONTROLLED MANAGEMENT OF DOMAIN COLLECTIONS
// ----------------------------------------------------------------------------
Console.WriteLine("--- 📦 AGGREGATION AND INVARIANT PROTECTION IN COLLECTION ---");

var cartId = CartId.Create();
var customerId = CustomerId.Create();

var cart = ShoppingCart.Create(cartId, customerId);

Console.WriteLine($"[Cart Created] ID: {cart.Id}");

// Add products respecting the maximum limit rule (Maximum 3 products per cart in this demo)
var res1 = cart.AddProduct(ProductId.Create(101), Money.Create(29.99m), 1);
var res2 = cart.AddProduct(ProductId.Create(102), Money.Create(49.99m), 2);
var res3 = cart.AddProduct(ProductId.Create(103), Money.Create(15.00m), 1);

Console.WriteLine($"Items added. Total in Cart: {cart.TotalItems} items ({cart.TotalAmount:C})");

// Attempt to exceed the collection limit
var exceedsLimitRes = cart.AddProduct(ProductId.Create(104), Money.Create(100.00m), 1);
if (exceedsLimitRes.IsFailure)
{
    Console.WriteLine($"❌ Protected Invariant: {exceedsLimitRes.Error.Description}");
}

Console.WriteLine("\n--- 🛡️ EXTERNAL IMMUTABILITY VERIFICATION ---");
Console.WriteLine($"The 'Items' property exposes IReadOnlyCollection<{nameof(CartItem)}>.");
Console.WriteLine($"It is not possible to do `cart.Items.Add(...)` or `cart.Items.Clear()` from the outside.");

Console.WriteLine("\nCHAPTER 13 COMPLETED SUCCESSFULLY.\n");


// ============================================================================
// DEFINITION OF DOMAIN ENTITIES AND COLLECTIONS
// ============================================================================

namespace Chapter13
{
    [StrongId<Guid>]
    public readonly partial record struct CartId;

    [StrongId<Guid>]
    public readonly partial record struct CustomerId;

    [StrongId<int>]
    public readonly partial record struct ProductId;

    [Money(Min = 0.01)]
    public readonly partial record struct Money;

    public record CartItem(ProductId ProductId, Money Price, int Quantity);

    public class ShoppingCart : AggregateRoot<CartId>
    {
        private const int MaxAllowedItems = 3;
        private readonly List<CartItem> _items = [];

        public CustomerId CustomerId { get; private set; }
        
        // Immutable exposure of the collection
        public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();
        
        public int TotalItems => _items.Sum(i => i.Quantity);
        public decimal TotalAmount => _items.Sum(i => i.Price.Value * i.Quantity);

        private ShoppingCart(CartId id, CustomerId customerId)
        {
            Id = id;
            CustomerId = customerId;
        }

        public static ShoppingCart Create(CartId id, CustomerId customerId) => new(id, customerId);

        public Result AddProduct(ProductId productId, Money price, int quantity)
        {
            if (_items.Count >= MaxAllowedItems)
            {
                return Error.Validation(
                    "Cart.LimitExceeded",
                    $"Cannot add more than {MaxAllowedItems} types of products to the cart."
                );
            }

            _items.Add(new CartItem(productId, price, quantity));
            return Result.Success();
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
    private readonly System.Collections.Generic.List<IDomainEvent> _domainEvents = new();
    public System.Collections.Generic.IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();
}
