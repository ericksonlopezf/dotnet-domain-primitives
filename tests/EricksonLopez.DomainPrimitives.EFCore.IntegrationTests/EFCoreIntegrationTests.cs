// Copyright © Erickson Lopez. MIT License.
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using EricksonLopez.DomainPrimitives;
using EricksonLopez.DomainPrimitives.EFCore.Generated;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EricksonLopez.DomainPrimitives.EFCore.IntegrationTests;

[StrongId<Guid>]
public readonly partial record struct CustomerId;

[Email]
public readonly partial record struct CustomerEmail;

[Money]
public readonly partial record struct Balance;

[NumericPrimitive<int>]
public readonly partial record struct LoyaltyPoints;

[SmartEnum<int>]
public readonly partial record struct MembershipTier
{
    public static readonly MembershipTier Standard = new(1, "Standard");
    public static readonly MembershipTier Premium = new(2, "Premium");
    public static readonly MembershipTier Vip = new(3, "Vip");
}

public sealed class CustomerEntity
{
    public CustomerId Id { get; set; }
    public CustomerEmail Email { get; set; }
    public Balance AccountBalance { get; set; }
    public LoyaltyPoints Points { get; set; }
    public MembershipTier Tier { get; set; }
}

public sealed class TestDbContext : DbContext
{
    private readonly SqliteConnection _connection;

    public DbSet<CustomerEntity> Customers => Set<CustomerEntity>();

    public TestDbContext(SqliteConnection connection)
    {
        _connection = connection;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(_connection);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.ConfigureDomainPrimitives();
    }
}

public sealed class EFCoreIntegrationTests : IAsyncLifetime
{
    private SqliteConnection _connection = null!;

    public async Task InitializeAsync()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        await _connection.OpenAsync();

        using var context = new TestDbContext(_connection);
        await context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _connection.DisposeAsync();
    }

    private TestDbContext CreateContext()
    {
        return new TestDbContext(_connection);
    }

    [Fact]
    public async Task EFCore_CanInsertAndRetrieveEntityWithDomainPrimitives_OnSqlite()
    {
        var customerId = CustomerId.Create(Guid.NewGuid());
        var email = CustomerEmail.Create("alice@example.com");
        var balance = Balance.Create(250.75m);
        var points = LoyaltyPoints.Create(1500);
        var tier = MembershipTier.Premium;

        // 1. Insert
        using (var context = CreateContext())
        {
            var entity = new CustomerEntity
            {
                Id = customerId,
                Email = email,
                AccountBalance = balance,
                Points = points,
                Tier = tier
            };

            context.Customers.Add(entity);
            await context.SaveChangesAsync();
        }

        // 2. Query by StrongId
        using (var context = CreateContext())
        {
            var loaded = await context.Customers.FirstOrDefaultAsync(c => c.Id == customerId);
            loaded.Should().NotBeNull();
            loaded!.Email.Should().Be(email);
            loaded.AccountBalance.Should().Be(balance);
            loaded.Points.Should().Be(points);
            loaded.Tier.Should().Be(tier);
        }

        // 3. Query with LINQ filtering on StringPrimitive & SmartEnum in real SQL
        using (var context = CreateContext())
        {
            var queryResult = await context.Customers
                .Where(c => c.Email == email && c.Tier == MembershipTier.Premium)
                .ToListAsync();

            queryResult.Should().HaveCount(1);
            queryResult[0].Id.Should().Be(customerId);
        }

        // 4. Update
        using (var context = CreateContext())
        {
            var loaded = await context.Customers.FirstAsync(c => c.Id == customerId);
            loaded.AccountBalance = Balance.Create(500.00m);
            loaded.Tier = MembershipTier.Vip;
            await context.SaveChangesAsync();
        }

        // 5. Verify update
        using (var context = CreateContext())
        {
            var updated = await context.Customers.FirstAsync(c => c.Id == customerId);
            updated.AccountBalance.Value.Should().Be(500.00m);
            updated.Tier.Should().Be(MembershipTier.Vip);
        }
    }

    [Fact]
    public async Task EFCore_QueryWithNumericAndMoneyComparisons_TranslatesToSqlAndFiltersCorrectly()
    {
        var id1 = CustomerId.Create(Guid.NewGuid());
        var id2 = CustomerId.Create(Guid.NewGuid());
        var id3 = CustomerId.Create(Guid.NewGuid());

        using (var context = CreateContext())
        {
            context.Customers.AddRange(
                new CustomerEntity
                {
                    Id = id1,
                    Email = CustomerEmail.Create("low@example.com"),
                    AccountBalance = Balance.Create(50.00m),
                    Points = LoyaltyPoints.Create(200),
                    Tier = MembershipTier.Standard
                },
                new CustomerEntity
                {
                    Id = id2,
                    Email = CustomerEmail.Create("mid@example.com"),
                    AccountBalance = Balance.Create(250.00m),
                    Points = LoyaltyPoints.Create(1500),
                    Tier = MembershipTier.Premium
                },
                new CustomerEntity
                {
                    Id = id3,
                    Email = CustomerEmail.Create("high@example.com"),
                    AccountBalance = Balance.Create(1200.00m),
                    Points = LoyaltyPoints.Create(5000),
                    Tier = MembershipTier.Vip
                });

            await context.SaveChangesAsync();
        }

        using (var context = CreateContext())
        {
            var minBalance = Balance.Create(100.00m);
            var minPoints = LoyaltyPoints.Create(1000);

            var highValueCustomers = await context.Customers
                .Where(c => c.AccountBalance > minBalance && c.Points >= minPoints)
                .OrderBy(c => c.Points)
                .ToListAsync();

            highValueCustomers.Should().HaveCount(2);
            highValueCustomers[0].Id.Should().Be(id2);
            highValueCustomers[1].Id.Should().Be(id3);
        }
    }

    [Fact]
    public async Task EFCore_QueryWithSortingAndPagination_OrdersByDomainPrimitiveValue()
    {
        var id1 = CustomerId.Create(Guid.NewGuid());
        var id2 = CustomerId.Create(Guid.NewGuid());
        var id3 = CustomerId.Create(Guid.NewGuid());

        using (var context = CreateContext())
        {
            context.Customers.AddRange(
                new CustomerEntity
                {
                    Id = id1,
                    Email = CustomerEmail.Create("user1@example.com"),
                    AccountBalance = Balance.Create(100.00m),
                    Points = LoyaltyPoints.Create(100),
                    Tier = MembershipTier.Standard
                },
                new CustomerEntity
                {
                    Id = id2,
                    Email = CustomerEmail.Create("user2@example.com"),
                    AccountBalance = Balance.Create(900.00m),
                    Points = LoyaltyPoints.Create(900),
                    Tier = MembershipTier.Vip
                },
                new CustomerEntity
                {
                    Id = id3,
                    Email = CustomerEmail.Create("user3@example.com"),
                    AccountBalance = Balance.Create(500.00m),
                    Points = LoyaltyPoints.Create(500),
                    Tier = MembershipTier.Premium
                });

            await context.SaveChangesAsync();
        }

        using (var context = CreateContext())
        {
            // Order by points descending and take top 2
            var topCustomers = await context.Customers
                .OrderByDescending(c => c.Points)
                .Take(2)
                .ToListAsync();

            topCustomers.Should().HaveCount(2);
            topCustomers[0].Points.Value.Should().Be(900);
            topCustomers[1].Points.Value.Should().Be(500);
        }
    }

    [Fact]
    public async Task EFCore_QueryWithMultipleTiersAndProjections_ProjectsPrimitivesAccurately()
    {
        var id1 = CustomerId.Create(Guid.NewGuid());
        var id2 = CustomerId.Create(Guid.NewGuid());

        using (var context = CreateContext())
        {
            context.Customers.AddRange(
                new CustomerEntity
                {
                    Id = id1,
                    Email = CustomerEmail.Create("alpha@example.com"),
                    AccountBalance = Balance.Create(300.00m),
                    Points = LoyaltyPoints.Create(300),
                    Tier = MembershipTier.Standard
                },
                new CustomerEntity
                {
                    Id = id2,
                    Email = CustomerEmail.Create("beta@example.com"),
                    AccountBalance = Balance.Create(700.00m),
                    Points = LoyaltyPoints.Create(700),
                    Tier = MembershipTier.Vip
                });

            await context.SaveChangesAsync();
        }

        using (var context = CreateContext())
        {
            var projected = await context.Customers
                .Where(c => c.Tier == MembershipTier.Vip || c.Tier == MembershipTier.Standard)
                .Select(c => new
                {
                    c.Id,
                    c.Email,
                    c.AccountBalance
                })
                .OrderBy(c => c.Email)
                .ToListAsync();

            projected.Should().HaveCount(2);
            projected[0].Email.Value.Should().Be("alpha@example.com");
            projected[1].Email.Value.Should().Be("beta@example.com");
        }
    }

    [Fact]
    public async Task EFCore_SavingEntityWithDefaultPrimitive_ThrowsInvalidOperationException()
    {
        using var context = CreateContext();

        // Arrange: Customer with default (uninitialized) CustomerEmail and Balance
        var entityWithDefault = new CustomerEntity
        {
            Id = CustomerId.Create(Guid.NewGuid()),
            Email = default, // Uninitialized Domain Primitive (IsDefault == true)
            AccountBalance = Balance.Create(100.00m),
            Points = LoyaltyPoints.Create(10),
            Tier = MembershipTier.Standard
        };

        context.Customers.Add(entityWithDefault);

        // Act & Assert: EF Core value converter accesses .Value which throws InvalidOperationException wrapped in DbUpdateException
        var act = async () => await context.SaveChangesAsync();
        var ex = await act.Should().ThrowAsync<DbUpdateException>();
        ex.WithInnerException<InvalidOperationException>()
            .WithMessage("*default instance of CustomerEmail. Check IsDefault before accessing Value.*");
    }

    [Fact]
    public async Task EFCore_MaterializedEntity_HasIsDefaultFalseAndPreservesValueSemantics()
    {
        var id = CustomerId.Create(Guid.NewGuid());
        var email = CustomerEmail.Create("materialize@example.com");
        var balance = Balance.Create(1250.50m);
        var points = LoyaltyPoints.Create(2500);
        var tier = MembershipTier.Vip;

        // 1. Insert in write context
        using (var writeContext = CreateContext())
        {
            writeContext.Customers.Add(new CustomerEntity
            {
                Id = id,
                Email = email,
                AccountBalance = balance,
                Points = points,
                Tier = tier
            });
            await writeContext.SaveChangesAsync();
        }

        // 2. Read in fresh, independent read context (no identity map caching)
        using (var readContext = CreateContext())
        {
            var loaded = await readContext.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

            loaded.Should().NotBeNull();
            // Verify IsDefault is false for all domain primitives
            loaded!.Id.IsDefault.Should().BeFalse();
            loaded.Email.IsDefault.Should().BeFalse();
            loaded.AccountBalance.IsDefault.Should().BeFalse();
            loaded.Points.IsDefault.Should().BeFalse();

            // Verify value semantics and equality contracts
            loaded.Id.Should().Be(id);
            loaded.Email.Should().Be(email);
            loaded.AccountBalance.Should().Be(balance);
            loaded.Points.Should().Be(points);
            loaded.Tier.Should().Be(tier);
        }
    }
}




