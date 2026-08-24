// Copyright © Erickson Lopez. MIT License.
// ============================================================================
// CHAPTER 16: ENTITY FRAMEWORK CORE INTEGRATION (VALUE CONVERTERS & LINQ)
// ============================================================================
// In this chapter you will learn to map Domain Primitives and Strongly Typed IDs
// in Entity Framework Core (EF Core) using `ValueConverter<TPrimitive, TValue>`.
//
// MAIN ADVANTAGE:
// In the database, the column is stored as `UNIQUEIDENTIFIER` (Guid) or `NVARCHAR` (string).
// In the C# model, the property is a strongly typed `CustomerId` or `EmailAddress`.
// LINQ queries (`.Where(c => c.Id == id)`) are natively translated to SQL.
// ============================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Chapter16;
using EricksonLopez.DomainPrimitives;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

Console.WriteLine("=========================================================");
Console.WriteLine(" 📘 CHAPTER 16: ENTITY FRAMEWORK CORE INTEGRATION");
Console.WriteLine("=========================================================\n");

// ----------------------------------------------------------------------------
// 1. IN-MEMORY DBCONTEXT CONFIGURATION
// ----------------------------------------------------------------------------
var options = new DbContextOptionsBuilder<AppSampleDbContext>()
    .UseInMemoryDatabase(databaseName: "SampleOfficialDb")
    .Options;

using var dbContext = new AppSampleDbContext(options);

// ----------------------------------------------------------------------------
// 2. INSERTING DATA USING DOMAIN PRIMITIVES
// ----------------------------------------------------------------------------
Console.WriteLine("--- 💾 PERSISTENCE IN EF CORE WITH STRONGLY TYPED IDS ---");

var customerId1 = CustomerId.Create();
var customerId2 = CustomerId.Create();

var customer1 = new CustomerEntity { Id = customerId1, Name = "Roberto Gomez", Email = EmailAddress.Create("roberto@company.com") };
var customer2 = new CustomerEntity { Id = customerId2, Name = "Elena Torres", Email = EmailAddress.Create("elena@company.com") };

dbContext.Customers.AddRange(customer1, customer2);
await dbContext.SaveChangesAsync();

Console.WriteLine($"[EF Core] 2 Customers successfully saved to the database.");

// ----------------------------------------------------------------------------
// 3. LINQ QUERIES WITH DOMAIN TYPE FILTERING
// ----------------------------------------------------------------------------
Console.WriteLine("\n--- 🔍 TRANSPARENT LINQ QUERIES BY STRONGLY TYPED ID ---");

var searchedCustomer = await dbContext.Customers.FirstOrDefaultAsync(c => c.Id == customerId1);

if (searchedCustomer is not null)
{
    Console.WriteLine($"[Query Found]");
    Console.WriteLine($"  ID:     {searchedCustomer.Id}");
    Console.WriteLine($"  Name:   {searchedCustomer.Name}");
    Console.WriteLine($"  Email:  {searchedCustomer.Email}");
}

Console.WriteLine("\nCHAPTER 16 COMPLETED SUCCESSFULLY.\n");


// ============================================================================
// DEFINITION OF EF CORE ENTITY AND DBCONTEXT
// ============================================================================

namespace Chapter16
{
    [StrongId<Guid>]
    public readonly partial record struct CustomerId;

    [Email]
    public readonly partial record struct EmailAddress;

    public class CustomerEntity
    {
        public CustomerId Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public EmailAddress Email { get; set; }
    }

    public class AppSampleDbContext : DbContext
    {
        public DbSet<CustomerEntity> Customers => Set<CustomerEntity>();

        public AppSampleDbContext(DbContextOptions<AppSampleDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CustomerEntity>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Configure explicit zero-reflection ValueConverters
                entity.Property(e => e.Id)
                      .HasConversion(
                          id => id.Value,
                          guid => CustomerId.Create(guid));

                entity.Property(e => e.Email)
                      .HasConversion(
                          email => email.Value,
                          str => EmailAddress.Create(str));
            });
        }
    }
}




