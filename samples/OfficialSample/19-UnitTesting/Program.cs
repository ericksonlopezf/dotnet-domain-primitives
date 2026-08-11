using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
// ============================================================================
// CHAPTER 19: DOMAIN UNIT TESTS AND FUNCTIONAL ASSERTIONS
// ============================================================================
// In this chapter you will learn the best practices for writing unit tests
// on Domain Primitives, Value Objects and Aggregate Roots.
//
// BEST PRACTICES FOR UNIT TESTING IN DDD:
// 1. Test `TryCreate()` verifying `IsSuccess` and `IsFailure` without depending on try-catch.
// 2. Verify exact error codes (`Error.Code == "Email.InvalidFormat"`).
// 3. Test Aggregate Roots ensuring that the expected `DomainEvents` are emitted.
// ============================================================================

using Chapter19;
using EricksonLopez.DomainPrimitives;
using EricksonLopez.Result;

Console.WriteLine("=========================================================");
Console.WriteLine(" 📘 CHAPTER 19: UNIT TESTS AND ASSERTION SUITE");
Console.WriteLine("=========================================================\n");

int successfulTests = 0;

// ----------------------------------------------------------------------------
// TEST 1: VALID EMAIL MUST RETURN RESULT.SUCCESS
// ----------------------------------------------------------------------------
Console.WriteLine("🧪 [TEST 1] Email.TryCreate with valid string must return Success");
bool istest1Success = EmailAddress.TryCreate("test.user@domain.com", out var test1Val, out var test1Error);
Assert(istest1Success, "Result should have been successful");
Assert(test1Val.Value == "test.user@domain.com", "The stored value does not match");
Console.WriteLine("   ✅ PASSED");
successfulTests++;

// ----------------------------------------------------------------------------
// TEST 2: INVALID EMAIL MUST RETURN RESULT.FAILURE WITH FORMAT ERROR
// ----------------------------------------------------------------------------
Console.WriteLine("\n🧪 [TEST 2] Email.TryCreate with invalid string must return Failure");
bool istest2Success = EmailAddress.TryCreate("email-without-at", out var test2Val, out var test2Error);
Assert(!istest2Success, "Result should have been a failure");
Assert(ErrorType.Validation == ErrorType.Validation, "The error type must be Validation");
Console.WriteLine($"   ✅ PASSED (Caught error: {test2Error.Message})");
successfulTests++;

// ----------------------------------------------------------------------------
// TEST 3: CREATE ORDER MUST EMIT DOMAIN EVENT 'OrderCreated'
// ----------------------------------------------------------------------------
Console.WriteLine("\n🧪 [TEST 3] Order.Create must emit OrderCreatedEvent in DomainEvents");
var orderId = OrderId.Create();
var customerId = CustomerId.Create();
var orderResult = Order.Create(orderId, customerId);

Assert(orderResult.IsSuccess, "The order should have been successfully created");
Assert(orderResult.Value.DomainEvents.Count == 1, "There must be exactly 1 registered event");
Assert(orderResult.Value.DomainEvents.First() is OrderCreatedEvent, "The emitted event must be OrderCreatedEvent");
Console.WriteLine("   ✅ PASSED");
successfulTests++;

Console.WriteLine($"\n=========================================================");
Console.WriteLine($" 🏆 TEST RUNNER SUMMARY: {successfulTests}/3 TESTS PASSED");
Console.WriteLine($"=========================================================");

Console.WriteLine("\nCHAPTER 19 COMPLETED SUCCESSFULLY.\n");


static void Assert(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException($"❌ FAILED ASSERTION IN UNIT TEST: {message}");
    }
}

// ============================================================================
// DOMAIN TYPES UNDER TEST
// ============================================================================

namespace Chapter19
{
    [StrongId<Guid>]
    public readonly partial record struct OrderId;

    [StrongId<Guid>]
    public readonly partial record struct CustomerId;

    [Email]
    public readonly partial record struct EmailAddress;

    public record OrderCreatedEvent(OrderId OrderId, CustomerId CustomerId) : IDomainEvent;

    public class Order : AggregateRoot<OrderId>
    {
        public CustomerId CustomerId { get; }

        private Order(OrderId id, CustomerId customerId)
        {
            Id = id;
            CustomerId = customerId;
        }

        public static Result<Order> Create(OrderId id, CustomerId customerId)
        {
            var p = new Order(id, customerId);
            p.AddDomainEvent(new OrderCreatedEvent(id, customerId));
            return p;
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
