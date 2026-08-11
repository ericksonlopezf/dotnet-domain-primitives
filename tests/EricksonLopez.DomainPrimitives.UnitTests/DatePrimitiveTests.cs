using System;
using EricksonLopez.DomainPrimitives.Tests.TestTypes;

namespace EricksonLopez.DomainPrimitives.Tests;

public class DatePrimitiveTests
{
    [Fact]
    public void RegistrationTimestamp_PastOnly_AllowsPast()
    {
        var dt = DateTime.UtcNow.AddMinutes(-5);
        var reg = RegistrationTimestamp.Create(dt);
        Assert.Equal(dt, reg.Value);
    }

    [Fact]
    public void RegistrationTimestamp_PastOnly_RejectsFuture()
    {
        var dt = DateTime.UtcNow.AddMinutes(5);
        Assert.Throws<DomainPrimitiveValidationException>(() => RegistrationTimestamp.Create(dt));
    }

    [Fact]
    public void CustomerBirthDate_BirthDateShortcut_Works()
    {
        // 20 years ago today
        var bday = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20));
        var birthDate = CustomerBirthDate.Create(bday);
        
        Assert.Equal(bday, birthDate.Value);
        Assert.Equal(20, birthDate.Age);
    }

    [Fact]
    public void CustomerBirthDate_FutureDate_Throws()
    {
        var future = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        Assert.Throws<DomainPrimitiveValidationException>(() => CustomerBirthDate.Create(future));
    }

    [Fact]
    public void CustomerBirthDate_TooOld_Throws()
    {
        // MaxAge = 120
        var tooOld = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-121));
        var ex = Assert.Throws<DomainPrimitiveValidationException>(() => CustomerBirthDate.Create(tooOld));
        Assert.Contains("120", ex.Message);
    }

    [Fact]
    public void CreditCardExpiration_ExpirationShortcut_Works()
    {
        var expiry = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(6));
        var cardExp = CreditCardExpiration.Create(expiry);
        
        Assert.Equal(expiry, cardExp.Value);
        Assert.False(cardExp.IsExpired());
        Assert.True(cardExp.DaysUntilExpiration() > 0);
    }

    [Fact]
    public void CreditCardExpiration_PastDate_Throws()
    {
        var past = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
        Assert.Throws<DomainPrimitiveValidationException>(() => CreditCardExpiration.Create(past));
    }
}
