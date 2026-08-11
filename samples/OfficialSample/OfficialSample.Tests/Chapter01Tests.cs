using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chapter01;
using EricksonLopez.DomainPrimitives;

namespace OfficialSample.Tests;

public class Chapter01Tests
{
    [Fact]
    public void CustomerId_New_ShouldCreateValidId()
    {
        // Arrange & Act
        var id = CustomerId.Create();

        // Assert
        Assert.NotEqual(Guid.Empty, id.Value);
    }

    [Fact]
    public void OrderId_New_ShouldCreateValidId()
    {
        // Arrange & Act
        var id = OrderId.Create();

        // Assert
        Assert.NotEqual(Guid.Empty, id.Value);
    }

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name+tag@domain.co")]
    public void EmailAddress_Create_WithValidEmail_ShouldSucceed(string email)
    {
        // Arrange & Act
        bool isresultSuccess = EmailAddress.TryCreate(email, out var resultVal, out var resultError);

        // Assert
        Assert.True(isresultSuccess);
        Assert.Equal(email, resultVal.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid-email")]
    [InlineData("@domain.com")]
    public void EmailAddress_Create_WithInvalidEmail_ShouldFail(string email)
    {
        // Arrange & Act
        bool isresultSuccess = EmailAddress.TryCreate(email, out var resultVal, out var resultError);

        // Assert
        Assert.False(isresultSuccess);
    }
}


