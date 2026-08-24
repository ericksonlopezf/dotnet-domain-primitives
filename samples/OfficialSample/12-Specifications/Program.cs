// Copyright © Erickson Lopez. MIT License.
// ============================================================================
// CHAPTER 12: SPECIFICATION PATTERN (COMPOSITE DOMAIN RULES)
// ============================================================================
// In this chapter you will learn about the Specification pattern in DDD: encapsulating
// complex business rules in reusable and combinable objects (`And`, `Or`, `Not`).
//
// USE CASES:
// 1. Invariant Validation: Evaluate if an entity meets a condition (`IsSatisfiedBy`).
// 2. Query Filters: Express search criteria translatable to LINQ / EF Core.
// 3. Composite Rules: Combine simple specifications into advanced business rules.
// ============================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Chapter12;
using EricksonLopez.DomainPrimitives;
using System.Threading.Tasks;

Console.WriteLine("=========================================================");
Console.WriteLine(" 📘 CHAPTER 12: SPECIFICATION PATTERN (REUSABLE RULES)");
Console.WriteLine("=========================================================\n");

// ----------------------------------------------------------------------------
// 1. CREATING TEST CUSTOMERS
// ----------------------------------------------------------------------------
var vipCustomer = new Customer(CustomerId.Create(), "Carlos VIP", EmailAddress.Create("carlos@company.com"), Money.Create(5000m), IsActive: true);
var riskCustomer = new Customer(CustomerId.Create(), "Ana Defaulting", EmailAddress.Create("ana@company.com"), Money.Create(100m), IsActive: true);
var inactiveCustomer = new Customer(CustomerId.Create(), "Pedro Inactive", EmailAddress.Create("pedro@company.com"), Money.Create(10000m), IsActive: false);

var customers = new List<Customer> { vipCustomer, riskCustomer, inactiveCustomer };

// ----------------------------------------------------------------------------
// 2. INDIVIDUAL SPECIFICATIONS AND COMPOSITION (AND, OR, NOT)
// ----------------------------------------------------------------------------
Console.WriteLine("--- 🛡️ EVALUATION OF COMPOSITE SPECIFICATIONS ---");

var isActiveSpec = new CustomerActiveSpecification();
var hasVIPCreditSpec = new CustomerMinimumCreditSpecification(Money.Create(3000m));

// Composite Business Rule: A customer qualifies for a loan IF THEY ARE ACTIVE AND HAVE VIP CREDIT
var qualifiesForLoanSpec = isActiveSpec.And(hasVIPCreditSpec);

foreach (var customer in customers)
{
    bool qualifies = qualifiesForLoanSpec.IsSatisfiedBy(customer);
    string status = qualifies ? "✅ QUALIFIES FOR LOAN" : "❌ REJECTED";
    Console.WriteLine($"Customer '{customer.Name}' (Active: {customer.IsActive}, Credit: {customer.Credit}): {status}");
}

Console.WriteLine("\n--- 🔍 IN-MEMORY FILTERING USING LINQ (TOEXPRESSION) ---");

var qualifiedCustomers = customers.AsQueryable().Where(qualifiesForLoanSpec.ToExpression()).ToList();
Console.WriteLine($"Customers filtered by the specification ({qualifiedCustomers.Count}):");
foreach (var c in qualifiedCustomers)
{
    Console.WriteLine($"  • {c.Name}");
}

Console.WriteLine("\nCHAPTER 12 COMPLETED SUCCESSFULLY.\n");


// ============================================================================
// SPECIFICATION PATTERN IMPLEMENTATION
// ============================================================================

namespace Chapter12
{
    [StrongId<Guid>]
    public readonly partial record struct CustomerId;

    [Email]
    public readonly partial record struct EmailAddress;

    [Money(Min = 0)]
    public readonly partial record struct Money;

    public record Customer(CustomerId Id, string Name, EmailAddress Email, Money Credit, bool IsActive);

    // Abstract Base Specification Class
    public abstract class Specification<T>
    {
        public abstract Expression<Func<T, bool>> ToExpression();

        public bool IsSatisfiedBy(T entity)
        {
            Func<T, bool> predicate = ToExpression().Compile();
            return predicate(entity);
        }

        public Specification<T> And(Specification<T> other) => new AndSpecification<T>(this, other);
        public Specification<T> Or(Specification<T> other) => new OrSpecification<T>(this, other);
        public Specification<T> Not() => new NotSpecification<T>(this);
    }

    // Rule 1: Active Customer
    public class CustomerActiveSpecification : Specification<Customer>
    {
        public override Expression<Func<Customer, bool>> ToExpression() => c => c.IsActive;
    }

    // Rule 2: Minimum Credit
    public class CustomerMinimumCreditSpecification : Specification<Customer>
    {
        private readonly Money _minimumAmount;

        public CustomerMinimumCreditSpecification(Money minimumAmount)
        {
            _minimumAmount = minimumAmount;
        }

        public override Expression<Func<Customer, bool>> ToExpression() => c => c.Credit >= _minimumAmount;
    }

    // Operational Combinators
    internal class AndSpecification<T> : Specification<T>
    {
        private readonly Specification<T> _left;
        private readonly Specification<T> _right;

        public AndSpecification(Specification<T> left, Specification<T> right)
        {
            _left = left;
            _right = right;
        }

        public override Expression<Func<T, bool>> ToExpression()
        {
            var leftExpr = _left.ToExpression();
            var rightExpr = _right.ToExpression();

            var param = Expression.Parameter(typeof(T), "x");
            var body = Expression.AndAlso(
                Expression.Invoke(leftExpr, param),
                Expression.Invoke(rightExpr, param)
            );

            return Expression.Lambda<Func<T, bool>>(body, param);
        }
    }

    internal class OrSpecification<T> : Specification<T>
    {
        private readonly Specification<T> _left;
        private readonly Specification<T> _right;

        public OrSpecification(Specification<T> left, Specification<T> right)
        {
            _left = left;
            _right = right;
        }

        public override Expression<Func<T, bool>> ToExpression()
        {
            var leftExpr = _left.ToExpression();
            var rightExpr = _right.ToExpression();

            var param = Expression.Parameter(typeof(T), "x");
            var body = Expression.OrElse(
                Expression.Invoke(leftExpr, param),
                Expression.Invoke(rightExpr, param)
            );

            return Expression.Lambda<Func<T, bool>>(body, param);
        }
    }

    internal class NotSpecification<T> : Specification<T>
    {
        private readonly Specification<T> _spec;

        public NotSpecification(Specification<T> spec)
        {
            _spec = spec;
        }

        public override Expression<Func<T, bool>> ToExpression()
        {
            var expr = _spec.ToExpression();
            var param = Expression.Parameter(typeof(T), "x");
            var body = Expression.Not(Expression.Invoke(expr, param));

            return Expression.Lambda<Func<T, bool>>(body, param);
        }
    }
}




