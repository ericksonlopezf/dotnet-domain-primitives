// Copyright © Erickson Lopez. MIT License.
using System;
using EricksonLopez.DomainPrimitives.EFCore.Generated;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EricksonLopez.DomainPrimitives.EFCore.IntegrationTests;

/// <summary>
/// DbContext for EF Core integration tests.
/// </summary>
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
