// Copyright © Erickson Lopez. MIT License.
using System;
using System.Linq;
using AwesomeAssertions;
using EricksonLopez.DomainPrimitives.UnitTests.TestTypes;
using EricksonLopez.DomainPrimitives.Validation;
using Xunit;

namespace EricksonLopez.DomainPrimitives.UnitTests;

public class DatePrimitiveTests
{
    [Fact]
    public void RegistrationTimestamp_PastOnly_AllowsPast()
    {
        var fixedPast = new DateTime(2020, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var reg = RegistrationTimestamp.Create(fixedPast);
        reg.Value.Should().Be(fixedPast);

        var pastDate = new DateTime(2023, 6, 15, 8, 30, 0, DateTimeKind.Utc);
        var regRelative = RegistrationTimestamp.Create(pastDate);
        regRelative.Value.Should().Be(pastDate);
    }

    [Fact]
    public void RegistrationTimestamp_PastOnly_RejectsFuture_WithTemporalError()
    {
        var future = DateTime.UtcNow.AddYears(1);
        Action act = () => RegistrationTimestamp.Create(future);

        act.Should().Throw<DomainPrimitiveValidationException>()
            .WithMessage("*RegistrationTimestamp must be in the past.*")
            .Where(e => e.Error.Code == "TEMPORAL");
    }

    [Fact]
    public void RegistrationTimestamp_PastOnly_ExplicitFixedFuture_ThrowsTemporalError()
    {
        var fixedFuture = new DateTime(2099, 12, 31, 23, 59, 59, DateTimeKind.Utc);
        Action act = () => RegistrationTimestamp.Create(fixedFuture);

        act.Should().Throw<DomainPrimitiveValidationException>()
            .WithMessage("*RegistrationTimestamp must be in the past.*")
            .Where(e => e.Error.Code == "TEMPORAL");
    }

    [Fact]
    public void CustomerBirthDate_BirthDateShortcut_Works()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var bday = today.AddYears(-20);
        var birthDate = CustomerBirthDate.Create(bday);
        
        birthDate.Value.Should().Be(bday);
        birthDate.Age.Should().Be(20);
    }

    [Fact]
    public void CustomerBirthDate_LeapYearBirthDate_WorksCorrectly()
    {
        // 2000 was a leap year
        var leapDay = new DateOnly(2000, 2, 29);
        var birthDate = CustomerBirthDate.Create(leapDay);

        birthDate.Value.Should().Be(leapDay);
        birthDate.Age.Should().BeGreaterThanOrEqualTo(24);
    }

    [Fact]
    public void CustomerBirthDate_FutureDate_Throws_WithTemporalError()
    {
        var future = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1);
        Action act = () => CustomerBirthDate.Create(future);

        act.Should().Throw<DomainPrimitiveValidationException>()
            .WithMessage("*CustomerBirthDate must be in the past.*")
            .Where(e => e.Error.Code == "TEMPORAL");
    }

    [Fact]
    public void CustomerBirthDate_TooOld_Throws_WithTemporalErrorAndMaxAge()
    {
        // MaxAge = 120
        var tooOld = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-121);
        Action act = () => CustomerBirthDate.Create(tooOld);

        act.Should().Throw<DomainPrimitiveValidationException>()
            .WithMessage("*CustomerBirthDate exceeds maximum age of 120.*")
            .Where(e => e.Error.Code == "TEMPORAL");
    }

    [Fact]
    public void CreditCardExpiration_ExpirationShortcut_Works()
    {
        var expiry = DateOnly.FromDateTime(DateTime.UtcNow).AddMonths(6);
        var cardExp = CreditCardExpiration.Create(expiry);
        
        cardExp.Value.Should().Be(expiry);
        cardExp.IsExpired().Should().BeFalse();
        cardExp.DaysUntilExpiration().Should().BeGreaterThan(0);
    }

    [Fact]
    public void CreditCardExpiration_PastDate_Throws_WithTemporalError()
    {
        var past = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1);
        Action act = () => CreditCardExpiration.Create(past);

        act.Should().Throw<DomainPrimitiveValidationException>()
            .WithMessage("*CreditCardExpiration must be in the future.*")
            .Where(e => e.Error.Code == "TEMPORAL");
    }

    [Fact]
    public void RegistrationTimestamp_TryCreate_Validation_Works()
    {
        var validPast = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var success = RegistrationTimestamp.TryCreate(validPast, out var reg, out var error);
        success.Should().BeTrue();
        reg.IsDefault.Should().BeFalse();
        reg.Value.Should().Be(validPast);
        error.Should().Be(PrimitiveError.None);

        var invalidFuture = new DateTime(2099, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var fail = RegistrationTimestamp.TryCreate(invalidFuture, out var failReg, out var failError);
        fail.Should().BeFalse();
        failReg.IsDefault.Should().BeTrue();
        failError.Code.Should().Be("TEMPORAL");
        failError.Message.Should().Contain("RegistrationTimestamp must be in the past");
    }

    [Fact]
    public void RegistrationTimestamp_Parse_And_TryParse_Works()
    {
        var validPastStr = "2022-06-15T12:00:00";
        var parsed = RegistrationTimestamp.Parse(validPastStr, System.Globalization.CultureInfo.InvariantCulture);
        parsed.Value.Year.Should().Be(2022);
        parsed.Value.Month.Should().Be(6);

        RegistrationTimestamp.TryParse(validPastStr, System.Globalization.CultureInfo.InvariantCulture, out var tryParsed).Should().BeTrue();
        tryParsed.Value.Year.Should().Be(2022);
        tryParsed.Value.Month.Should().Be(6);

        RegistrationTimestamp.TryParse("not-a-date", null, out var invalidDate).Should().BeFalse();
        invalidDate.IsDefault.Should().BeTrue();

        RegistrationTimestamp.TryParse((string?)null, null, out var nullDate).Should().BeFalse();
        nullDate.IsDefault.Should().BeTrue();
    }

    [Fact]
    public void RegistrationTimestamp_PastOnly_ExactBoundaryAtOrAfterNow_IsRejected_WithTemporalError()
    {
        // PastOnly uses strictly less than now (value >= now is rejected with TEMPORAL).
        // This test documents the exact microsecond boundary behavior.
        var futureBoundary = DateTime.UtcNow.AddMilliseconds(50);
        var success = RegistrationTimestamp.TryCreate(futureBoundary, out var reg, out var error);

        success.Should().BeFalse();
        reg.IsDefault.Should().BeTrue();
        error.Code.Should().Be("TEMPORAL");
        error.Message.Should().Contain("RegistrationTimestamp must be in the past");
    }

    [Fact]
    public void ShiftStartTime_FutureOnly_RejectsPastOrCurrentTime_WithTemporalError()
    {
        // TimeOnly.MinValue is 00:00:00, which is unconditionally <= now for all UTC times of day
        var pastTime = TimeOnly.MinValue;
        var fail = ShiftStartTime.TryCreate(pastTime, out var shiftPast, out var errorPast);

        fail.Should().BeFalse();
        shiftPast.IsDefault.Should().BeTrue();
        errorPast.Code.Should().Be("TEMPORAL");
        errorPast.Message.Should().Contain("ShiftStartTime must be in the future");
    }

    [Fact]
    public void ShiftStartTime_FutureOnly_WhenInPast_ThrowsDomainPrimitiveValidationException()
    {
        var pastTime = TimeOnly.MinValue;
        Action act = () => ShiftStartTime.Create(pastTime);

        act.Should().Throw<DomainPrimitiveValidationException>()
            .WithMessage("*ShiftStartTime must be in the future.*")
            .Where(e => e.Error.Code == "TEMPORAL");
    }

    [Fact]
    public void ShiftStartTime_FutureOnly_WhenInFuture_CreatesSuccessfully()
    {
        // TimeOnly.MaxValue is 23:59:59.9999999. In practical CI environments, this is > now.
        // It's the most deterministic future TimeOnly without clock abstraction.
        var futureTime = TimeOnly.MaxValue;
        var success = ShiftStartTime.TryCreate(futureTime, out var shiftFuture, out var error);

        success.Should().BeTrue();
        shiftFuture.IsDefault.Should().BeFalse();
        shiftFuture.Value.Should().Be(futureTime);
        error.Should().Be(PrimitiveError.None);
    }

    [Fact]
    public void GlobalTimestamp_DateTimeOffset_PastOnly_Validation_Works()
    {
        var pastTime = DateTimeOffset.UtcNow.AddMinutes(-5);
        var futureTime = DateTimeOffset.UtcNow.AddMinutes(5);

        // Creates successfully in the past
        var success = GlobalTimestamp.TryCreate(pastTime, out var ts, out var error);
        success.Should().BeTrue();
        ts.Value.Should().Be(pastTime);

        // Fails in the future
        GlobalTimestamp.TryCreate(futureTime, out var futureTs, out var futureErr).Should().BeFalse();
        futureTs.IsDefault.Should().BeTrue();
        futureErr.Code.Should().Be("TEMPORAL");
    }
}





