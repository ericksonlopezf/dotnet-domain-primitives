// Copyright © Erickson Lopez. MIT License.
using System;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using Dapper;
using EricksonLopez.DomainPrimitives;
using EricksonLopez.DomainPrimitives.Dapper.Generated;
using Microsoft.Data.Sqlite;
using Xunit;

namespace EricksonLopez.DomainPrimitives.Dapper.Tests;

[ValueObject]
public readonly partial record struct Address
{
    public string Street { get; init; }
    public string City { get; init; }
}

public class DapperValueObjectTests : IAsyncLifetime
{
    private SqliteConnection _connection = null!;

    public async Task InitializeAsync()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        await _connection.OpenAsync();
        
        await _connection.ExecuteAsync(@"
            CREATE TABLE Addresses (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Street TEXT NOT NULL,
                City TEXT NOT NULL
            )");
    }

    public async Task DisposeAsync()
    {
        await _connection.DisposeAsync();
    }

    [Fact]
    public async Task Insert_And_Query_ValueObject_Succeeds()
    {
        // Arrange
        var address = Address.Create("123 Main St", "Springfield");

        // Act - Insert
        var parameters = new DynamicParameters();
        parameters.AddParameters(address); // Using generated extension method
        
        await _connection.ExecuteAsync(@"
            INSERT INTO Addresses (Street, City)
            VALUES (@Street, @City)", 
            parameters);

        // Act - Query
        using var reader = await _connection.ExecuteReaderAsync("SELECT Street, City FROM Addresses LIMIT 1");
        reader.Read();
        
        // Using generated extension method
        var result = reader.ParseValueObject();

        // Assert
        result.Should().Be(address);
        result.Street.Should().Be("123 Main St");
        result.City.Should().Be("Springfield");
    }
}







