using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EricksonLopez.DomainPrimitives.Tests.TestTypes;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EricksonLopez.DomainPrimitives.Tests;

public class StringPrimitiveTests
{
    [Fact]
    public void FirstName_Create_ValidValue_CreatesInstance()
    {
        // Trim + MinLength(1) + MaxLength(100)
        var name = FirstName.Create("  John  ");
        Assert.Equal("John", name.Value);
    }

    [Fact]
    public void FirstName_Create_TooShort_ThrowsDomainPrimitiveValidationException()
    {
        var ex = Assert.Throws<DomainPrimitiveValidationException>(() => FirstName.Create("   ")); // Trims to empty
        Assert.Equal("LENGTH", ex.Error.Code);
        Assert.Contains("must be at least 1", ex.Message);
    }

    [Fact]
    public void EmailAddress_Create_ValidValue_NormalizesAndValidates()
    {
        // Email: Trim + LowerCase + NotEmpty + Regex
        var email = EmailAddress.Create("  USER@Example.COM  ");
        Assert.Equal("user@example.com", email.Value);
    }

    [Fact]
    public void EmailAddress_Create_InvalidFormat_ThrowsDomainPrimitiveValidationException()
    {
        var ex = Assert.Throws<DomainPrimitiveValidationException>(() => EmailAddress.Create("invalid-email"));
        Assert.Equal("FORMAT", ex.Error.Code);
        Assert.Contains("Invalid email format", ex.Message);
    }

    [Fact]
    public void EmailAddress_TryCreate_InvalidFormat_ReturnsFailureResult()
    {
        var success = EmailAddress.TryCreate("invalid-email", out _, out _);
        Assert.False(success);
    }

    [Fact]
    public void Country_Create_ValidValue_NormalizesAndValidates()
    {
        // CountryCode: Trim + UpperCase + Length(2,2)
        var country = Country.Create(" us ");
        Assert.Equal("US", country.Value);
    }

    [Fact]
    public void Country_Create_InvalidLength_ThrowsDomainPrimitiveValidationException()
    {
        Assert.Throws<DomainPrimitiveValidationException>(() => Country.Create("USA"));
        Assert.Throws<DomainPrimitiveValidationException>(() => Country.Create("U"));
    }

    [Fact]
    public void ProductCode_Create_ValidValue_MatchesRegex()
    {
        // Regex: ^[A-Z]{2}-\d{4}$
        var product = ProductCode.Create("AB-1234");
        Assert.Equal("AB-1234", product.Value);
    }

    [Fact]
    public void ProductCode_Create_InvalidValue_ReturnsCustomRegexErrorMessage()
    {
        var ex = Assert.Throws<DomainPrimitiveValidationException>(() => ProductCode.Create("AB1234"));
        Assert.Equal("FORMAT", ex.Error.Code);
        Assert.Contains("Must be in format XX-0000", ex.Message);
    }

    [Fact]
    public void DisplayName_Create_MultipleNormalizations_WorksCorrectly()
    {
        // Trim + UpperCase + NormalizeWhitespace
        var displayName = DisplayName.Create("  john   doe \t smith  ");
        Assert.Equal("JOHN DOE SMITH", displayName.Value);
    }

    [Fact]
    public void Equals_SameValues_ReturnsTrue()
    {
        var email1 = EmailAddress.Create("test@test.com");
        var email2 = EmailAddress.Create("TEST@TEST.COM"); // normalized to test@test.com

        Assert.Equal(email1, email2);
        Assert.True(email1 == email2);
    }

    [Fact]
    public void CompareTo_WorksCorrectly()
    {
        var countryA = Country.Create("AT");
        var countryB = Country.Create("BE");

        Assert.True(countryA < countryB);
        Assert.True(countryA.CompareTo(countryB) < 0);
    }

    [Fact]
    public void ExplicitCast_String_Works()
    {
        var str = "test@test.com";
        var email = (EmailAddress)str;
        Assert.Equal(str, email.Value);

        var castBack = (string)email;
        Assert.Equal(str, castBack);
    }

    [Fact]
    public void Parse_WorksCorrectly()
    {
        var email = EmailAddress.Parse("user@example.com", CultureInfo.InvariantCulture);
        Assert.Equal("user@example.com", email.Value);
    }

    [Fact]
    public void TypeConverter_ConvertsFromString()
    {
        var converter = TypeDescriptor.GetConverter(typeof(EmailAddress));
        Assert.True(converter.CanConvertFrom(typeof(string)));
        
        var email = (EmailAddress)converter.ConvertFrom("user@example.com")!;
        Assert.Equal("user@example.com", email.Value);
    }

    [Fact]
    public void TypeConverter_ConvertsToString()
    {
        var converter = TypeDescriptor.GetConverter(typeof(EmailAddress));
        var email = EmailAddress.Create("user@example.com");
        
        Assert.True(converter.CanConvertTo(typeof(string)));
        
        var str = (string)converter.ConvertTo(email, typeof(string))!;
        Assert.Equal("user@example.com", str);
    }
}
