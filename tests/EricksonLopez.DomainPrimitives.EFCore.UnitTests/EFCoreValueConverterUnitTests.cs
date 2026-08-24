// Copyright © Erickson Lopez. MIT License.
using System;
using AwesomeAssertions;
using EricksonLopez.DomainPrimitives;
using EricksonLopez.DomainPrimitives.EFCore.Generated;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Xunit;

namespace EricksonLopez.DomainPrimitives.EFCore.UnitTests;

[Email]
public readonly partial record struct EFCoreTestEmail;

[StrongId<Guid>]
public readonly partial record struct EFCoreTestCustomerId;

[StrongId<int>]
public readonly partial record struct EFCoreTestOrderId;

[NumericPrimitive<int>]
public readonly partial record struct EFCoreTestScore;

[Money]
public readonly partial record struct EFCoreTestMoney;

[Percentage]
public readonly partial record struct EFCoreTestPercentage;

[SmartEnum<int>]
public readonly partial record struct EFCoreTestStatus
{
    public static readonly EFCoreTestStatus Pending = new(1, "Pending");
    public static readonly EFCoreTestStatus Completed = new(2, "Completed");
}

public class EFCoreValueConverterUnitTests
{
    [Fact]
    public void EmailValueConverter_ConvertsToAndFromProvider()
    {
        var converter = new EFCoreTestEmailValueConverter();
        var email = EFCoreTestEmail.Create("test@example.com");

        var toProvider = converter.ConvertToProviderExpression.Compile();
        var fromProvider = converter.ConvertFromProviderExpression.Compile();

        var stringVal = (string)toProvider(email)!;
        stringVal.Should().Be("test@example.com");

        var restored = (EFCoreTestEmail)fromProvider("test@example.com")!;
        restored.Value.Should().Be("test@example.com");
    }

    [Fact]
    public void EmailValueConverter_FromProviderWithNull_ThrowsException()
    {
        var converter = new EFCoreTestEmailValueConverter();
        var fromProvider = converter.ConvertFromProviderExpression.Compile();

        var act = () => fromProvider(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CustomerIdValueConverter_ConvertsToAndFromProvider()
    {
        var converter = new EFCoreTestCustomerIdValueConverter();
        var guid = Guid.NewGuid();
        var customerId = EFCoreTestCustomerId.Create(guid);

        var toProvider = converter.ConvertToProviderExpression.Compile();
        var fromProvider = converter.ConvertFromProviderExpression.Compile();

        var guidVal = (Guid)toProvider(customerId)!;
        guidVal.Should().Be(guid);

        var restored = (EFCoreTestCustomerId)fromProvider(guid)!;
        restored.Value.Should().Be(guid);
    }

    [Fact]
    public void ScoreValueConverter_ConvertsToAndFromProvider()
    {
        var converter = new EFCoreTestScoreValueConverter();
        var score = EFCoreTestScore.Create(95);

        var toProvider = converter.ConvertToProviderExpression.Compile();
        var fromProvider = converter.ConvertFromProviderExpression.Compile();

        var intVal = (int)toProvider(score)!;
        intVal.Should().Be(95);

        var restored = (EFCoreTestScore)fromProvider(95)!;
        restored.Value.Should().Be(95);
    }

    [Fact]
    public void MoneyValueConverter_ConvertsToAndFromProvider()
    {
        var converter = new EFCoreTestMoneyValueConverter();
        var money = EFCoreTestMoney.Create(149.99m);

        var toProvider = converter.ConvertToProviderExpression.Compile();
        var fromProvider = converter.ConvertFromProviderExpression.Compile();

        var decVal = (decimal)toProvider(money)!;
        decVal.Should().Be(149.99m);

        var restored = (EFCoreTestMoney)fromProvider(149.99m)!;
        restored.Value.Should().Be(149.99m);
    }

    [Fact]
    public void SmartEnumValueConverter_ConvertsToAndFromProvider()
    {
        var converter = new EFCoreTestStatusValueConverter();
        var status = EFCoreTestStatus.Completed;

        var toProvider = converter.ConvertToProviderExpression.Compile();
        var fromProvider = converter.ConvertFromProviderExpression.Compile();

        var intVal = (int)toProvider(status)!;
        intVal.Should().Be(2);

        var restored = (EFCoreTestStatus)fromProvider(2)!;
        restored.Should().Be(EFCoreTestStatus.Completed);
    }

    [Fact]
    public void ConfigureDomainPrimitives_CanBeInvokedOnModelConfigurationBuilder()
    {
        // ModelConfigurationBuilder is an EF Core builder class
        // We verify that the extension method is generated, callable and builds the model correctly
        using var conventions = new ConventionsTestDbContext();
        _ = conventions.Model;
        conventions.Model.Should().NotBeNull();
    }

    private sealed class ConventionsTestDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);
            configurationBuilder.ConfigureDomainPrimitives();
        }
    }
}


