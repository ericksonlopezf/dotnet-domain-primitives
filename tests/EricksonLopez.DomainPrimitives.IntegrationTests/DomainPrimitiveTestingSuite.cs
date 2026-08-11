using System;
using EricksonLopez.DomainPrimitives.Testing;
using FluentAssertions;
using Xunit;

namespace EricksonLopez.DomainPrimitives.IntegrationTests;

[Email]
public readonly partial record struct TestEmail;

[StrongId<Guid>]
public readonly partial record struct TestCustomerId;

[Money]
public readonly partial record struct TestMoney;

public class DomainPrimitiveTestingSuite
{
    [Fact]
    public void Should_Create_Valid_Email_Using_FakeFactory()
    {
        // Arrange
        string validEmail = DomainPrimitiveFakeFactory.ValidEmail;
        
        // Act
        var email = TestEmail.Create(validEmail);
        
        // Assert
        email.Value.Should().Be(validEmail);
    }

    [Fact]
    public void Should_Create_Valid_Id_Using_FakeFactory()
    {
        // Arrange
        Guid validGuid = DomainPrimitiveFakeFactory.ValidGuid;
        
        // Act
        var id = TestCustomerId.Create(validGuid);
        
        // Assert
        id.Value.Should().Be(validGuid);
    }

    [Fact]
    public void Should_Create_Valid_Money_Using_FakeFactory()
    {
        // Arrange
        decimal validMoney = DomainPrimitiveFakeFactory.ValidMoneyAmount;
        
        // Act
        var money = TestMoney.Create(validMoney);
        
        // Assert
        money.Value.Should().Be(validMoney);
    }
}
